using Carubbi.Mailer.Implementation;
using Carubbi.Utils.Data;
using Itanio.MailMarketing.AgenteDisparador.DAL;
using Itanio.MailMarketing.AgenteDisparador.Domain;
using NLog;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Threading;

namespace Itanio.MailMarketing.AgenteDisparador
{
    [DisallowConcurrentExecution]
    public class MonitorarFilaSolicitacoes : IJob
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();


        public void Execute(IJobExecutionContext context)
        {
            Execute();
        }

        public void Execute()
        {
            var service = new SolicitacaoService();
            ISolicitacaoRepository repo = new DapperSolicitacaoRepository();

            var solicitacoes = service.ListarProximasSolicitacoesPendentes(1);
            foreach (var item in solicitacoes)
            {
                try
                {
                    if (Processar(item))
                    {
                        if (item.Tipo == TipoSolicitacao.Parar || item.Tipo == TipoSolicitacao.Cancelar)
                        {
                            item.Status = StatusSolicitacao.Processada;
                            item.DataProcessamento = DateTime.Now;
                        }
                        else
                        {
                            item.Status = StatusSolicitacao.Lido;
                        }
                        repo.Atualizar(item);
                    }
                    else
                    {
                        item.Status = StatusSolicitacao.Pendente;
                        repo.Atualizar(item);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Erro ao processar solicitação");
                    item.Status = StatusSolicitacao.Erro;
                    repo.Atualizar(item);
                }
            }
        }

        private bool Processar(Solicitacao item)
        {
            Console.WriteLine($"Processando solicitacao de {item.Tipo.ToString()} id: {item.Id}");
            bool resultado = true;

            switch (item.Tipo)
            {
                case TipoSolicitacao.Enviar:
                case TipoSolicitacao.Resumir:
                    if (!ServicoDisparadorEmail.Canceladores.ContainsKey(item.IdMensagem))
                    {
                        try
                        {
                            var cancelationToken = new CancellationTokenSource();
                            if (ThreadPool.QueueUserWorkItem(token => Enviar(item, (CancellationTokenSource)token), cancelationToken))
                            {
                                resultado = true;
                                ServicoDisparadorEmail.Canceladores.Add(item.IdMensagem, cancelationToken);
                            }
                            else
                            {
                                resultado = false;
                            }
                        }
                        catch (NotSupportedException)
                        {
                            resultado = false;
                        }
                    }
                    else
                    {
                        resultado = true;
                    }
                    break;
                case TipoSolicitacao.Parar:
                case TipoSolicitacao.Cancelar:
                    IMensagemRepository repoMsg = new DapperMensagemRepository();
                    if (ServicoDisparadorEmail.Canceladores.ContainsKey(item.IdMensagem))
                    {
                        ServicoDisparadorEmail.Canceladores[item.IdMensagem].Cancel();
                        ServicoDisparadorEmail.Canceladores.Remove(item.IdMensagem);
                    }
                    resultado = true;

                    if (item.Tipo == TipoSolicitacao.Cancelar)
                    {
                        repoMsg.AtualizarStatus(StatusEnvio.Cancelado, item.IdMensagem);
                    }
                    else
                    {
                        repoMsg.AtualizarStatus(StatusEnvio.Iniciado, item.IdMensagem);
                    }
                    
                    break;
            }

            Console.WriteLine($"Solicitacao de {item.Tipo.ToString()} id: {item.Id} processada");
            return resultado;
        }

        private void Enviar(Solicitacao solicitacao, CancellationTokenSource s)
        {
            
            Console.WriteLine($"Início do processo de envio da solicitacao {solicitacao.Id}");
            ISolicitacaoRepository repo = new DapperSolicitacaoRepository();

            solicitacao.Status = StatusSolicitacao.Processando;
            repo.Atualizar(solicitacao);

            CancellationToken token = s.Token;

            IMensagemRepository repoMsg = new DapperMensagemRepository();
            Mensagem mensagem = repoMsg.ObterPorId(solicitacao.IdMensagem);
            var solicitacoesMensagem = repo.ListarPorMensagem(solicitacao.IdMensagem);
            var total = mensagem.Segmentos.SelectMany(m => m.Mailings).SelectMany(x => x.Contatos).Count();
            var qtd = solicitacoesMensagem.Max(x => x.Quantidade);

           
            mensagem.StatusEnvio = StatusEnvio.EmAndamento;
            if (qtd == 0)
                mensagem.InicioEnvio = DateTime.Now;

            repoMsg.Atualizar(mensagem);

         
            
            SmtpSender sender = new SmtpSender();
            foreach (KeyValuePair<int, MailMessage> envio in GeradorMensagens.Gerar(mensagem))
            {
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine($"Processo de envio interrompido solicitacao {solicitacao.Id}");
                    solicitacao.Status = StatusSolicitacao.Interrompida;
                    solicitacao.DataProcessamento = DateTime.Now;
                    repo.Atualizar(solicitacao);
                    ServicoDisparadorEmail.Canceladores.Remove(solicitacao.IdMensagem);
                    return;
                }
                qtd++;
                try
                {
                    sender.Username = ConfigurationManager.AppSettings["SMTPUsuario"];
                    sender.Password = ConfigurationManager.AppSettings["SMTPSenha"];
                    Thread.Sleep(ConfigurationManager.AppSettings["TempoEspera"].To<int>(0) * 1000);
                    sender.Send(envio.Value);
                    
                    repoMsg.InserirEnvioInfo(new EnvioInfo { IdMensagem = mensagem.Id, IdContato = envio.Key, Status = 1 });
                }
                catch (Exception ex)
                {
                    repoMsg.InserirEnvioInfo(new EnvioInfo { IdMensagem = mensagem.Id, IdContato = envio.Key, Status = 3, Erro = ex.Message });
                }
                finally
                {
                    var percentual = qtd * 100M / total;
                    solicitacao.Percentual = percentual;
                    solicitacao.Quantidade = qtd;
                    solicitacao.Total = total;
                    repo.Atualizar(solicitacao);
                }
            }

            Console.WriteLine($"Fim do processo de envio da solicitacao {solicitacao.Id}");
            solicitacao.Status = StatusSolicitacao.Processada;
            solicitacao.Percentual = 100;
            solicitacao.Quantidade = qtd;
            solicitacao.Total = total;
            solicitacao.DataProcessamento = DateTime.Now;
            repo.Atualizar(solicitacao);
            ServicoDisparadorEmail.Canceladores.Remove(solicitacao.IdMensagem);
            mensagem.StatusEnvio = StatusEnvio.Completado;
            mensagem.FimEnvio = DateTime.Now;
            repoMsg.Atualizar(mensagem);

            token.WaitHandle.WaitOne(1000);
        }


    }
}

