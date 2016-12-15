
using Dapper;
using Itanio.MailMarketing.AgenteDisparador.Domain;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System;

namespace Itanio.MailMarketing.AgenteDisparador
{
    public class DapperMensagemRepository : IMensagemRepository
    {
        SqlConnection _conexao;

        string selectMensagem = @"
                        select
                            msg.msg_id                           Id 
                            , msg.msg_nome                       Nome
                            , msg.msg_descricao                  Descricao
                            , msg.msg_remetenteNome              RemententeNome
                            , msg.msg_remetenteEmail             RemententeEmail
                            , msg.msg_assunto                    Assunto
                            , msg.msg_titulo_corpo               TituloCorpo
                            , msg.msg_corpo                      Corpo
                            , msg.msg_corpoPreparado             CorpoPreparado
                            , msg.msg_intervalo                  Intervalo
                            , msg.msg_tempoResposta              TempoResposta
                            , msg.msg_inicioEnvio                InicioEnvio
                            , msg.msg_fimEnvio                   FimEnvio
                            , msg.msg_iniciarEnvio               IniciarEnvio
                            , msg.msg_status                     Status
                            , msg.msg_textoOptIN_optOUT          TextoOpt
                            , msg.msg_numeroTotalEnvio           TotalEnvio
                            , msg.msg_noticia                    Noticia
                            , msg.msg_personalizada              Personalizada
                            , msg.msg_produto                    Produto
                            , msg.msg_ordem_noticia              OrdemNoticia
                            , msg.msg_ordem_personalizada        OrdemPersonalizada
                            , msg.msg_ordem_produto              OrdemProduto
                            , msg.logininclusao                  IdUsuarioInclusao
                            , msg.loginalteracao                 IdUsuarioAlteracao
                            , msg.datainclusao                   DataInclusao
                            , msg.dataalteracao                  DataAlteracao
                            , msg.msg_textoTopo                  TextoTopo
                        from
                            UPM_MSGEMAIL msg
                        where
                            msg.msg_id = @id;";

        string selectNoticias = @"select
                             con.codigo Id 
                            , con.Secao Secao
                            , con.Titulo Titulo
                            , con.figura Figura
                            , con.figuraDesc FiguraDescricao
                            , con.Resumo Resumo
                            , con.ResumoEnews ResumoEnews
                            , con.Enews Enews
                            , con.Corpo Corpo
                            , con.Data_Inclusao DataInclusao
                            , con.Data_Modificacao DataModificacao
                            , con.autor Autor
                            , con.Status Status
                            , con.visitas Visitas
                            , con.Edicao Edicao
                            , con.privado Privado
                            , con.Editoria Editoria
                            , con.Enviada Enviada
                            , con.Regiao Regiao
                            , con.Tipo Tipo
                            , con.exibirCase ExibirCase
                            , con.exibirArtigo ExibirArtigo
                            , con.ordem Ordem
                            , con.Destaque Destaque
                            , con.Relacionado Relacionado
                            , con.Destaque_Painel DestaquePainel
                            , con.Entrevista Entrevista
                            , con.LideresAudiencia LideresAudiencia
                            , con.EditorMercado EditorMercado
                            , con.Figura_Indice FiguraIndice
                            , con.Figure_Indice_Descricao FiguraIndiceDescricao
                            , con.IdEditoria IdEditoria
                            , con.IdEdicao IdEdicao
                            , con.Figura_Newsletter FiguraNewsletter
                            , con.Descricao_Figura_Newsletter DescricaoFiguraNewsletter
                        from
                            UPM_MSGEMAIL_CONTEUDO mc 
                        join conteudo con on mc.con_id = con.codigo
                        where
                            mc.msg_id = @id;";

        string selectEmpresas = @"select 
	                                     EMP.codigo                 Id
	                                    ,EMP.destaque               Destaque
	                                    ,EMP.nomeFantasia           NomeFantasia
	                                    ,EMP.razao                  RazaoSocial
	                                    ,EMP.Endereco               Endereco
	                                    ,EMP.Bairro                 Bairro
	                                    ,EMP.Cidade                 Cidade
	                                    ,EMP.Estado                 Estado
	                                    ,EMP.CEP                    CEP
	                                    ,EMP.ddd_telefone           DDDTelefone
	                                    ,EMP.Telefone               Telefone
	                                    ,EMP.ddd_fax                DDDFax
	                                    ,EMP.Fax                    Fax
	                                    ,EMP.Email                  Email
	                                    ,EMP.Website                Website
	                                    ,EMP.video                  Video
	                                    ,EMP.Atuacao                Atuacao
	                                    ,EMP.Contato                Contato
	                                    ,EMP.Figura                 Figura
	                                    ,EMP.figura_destaque        FiguraDestaque
	                                    ,EMP.Descricao              Descricao
	                                    ,EMP.VU                     VU
	                                    ,EMP.Ativo                  Ativo
	                                    ,EMP.Exibir                 Exibir
	                                    ,EMP.PV                     PV
                                    from
	                                    UPM_MSGEMAIL_PRODUTO ME
	                                    JOIN EMPRESAS EMP ON ME.PRO_ID = EMP.CODIGO
                                    WHERE 
	                                    MSG_ID = @id";


        string selectSegmentos = @"
                        select
                            seg.seg_id                          Id 
                            , seg.seg_nome                       Nome
                            , seg.seg_descricao                  Descricao
                            , seg.seg_status                     Status
                            , seg.logininclusao                  IdUsuarioInclusao
                            , seg.loginalteracao                 IdUsuarioAlteracao
                            , seg.datainclusao                   DataInclusao
                            , seg.dataalteracao                  DataAlteracao
                        from
                            UPM_MSGEMAIL_SEGMENTO ms    
                        join UPM_SEGMENTO seg on ms.seg_id = seg.seg_id
                        where
                            ms.msg_id = @id;";


        string selectMailings = @"select 
                              mai.mai_id                         Id 
                            , mai.seg_id                         IdSegmento
                            , mai.mai_nome                       Nome
                            , mai.mai_descricao                  Descricao
                            , mai.mai_status                     Status
                            , mai.logininclusao                  IdUsuarioInclusao
                            , mai.loginalteracao                 IdUsuarioAlteracao
                            , mai.datainclusao                   DataInclusao
                            , mai.dataalteracao                  DataAlteracao
                        from
                            UPM_MSGEMAIL_SEGMENTO ms    
                            join UPM_SEGMENTO seg on ms.seg_id = seg.seg_id
                            join UPM_MAILING mai on mai.seg_id = seg.seg_id
                        where
                            ms.msg_id = @id;";

        string selectContatos = @"select 
                              ema.ema_id                         Id
                            , ema.mai_id                         IdMailing
                            , ema.ema_nome                       Nome
                            , ema.ema_email                      Email
                            , ema.ema_empresa                    Empresa
                            , ema.ema_status                     Status
                            , ema.ema_opt                        Opt
                            , ema.ema_enviadoPergunta            EnviadoPergunta
                            , ema.ema_codigoPortal               CodigoPortal
                            , ema.logininclusao                  IdUsuarioInclusao
                            , ema.loginalteracao                 IdUsuarioAlteracao
                            , ema.datainclusao                   DataInclusao
                            , ema.dataalteracao                  DataAlteracao
                            , ema.ema_cargo                      Cargo
                            , ema.ema_tratamento                 Tratamento
                        from
                            UPM_MSGEMAIL_SEGMENTO ms    
                            join UPM_SEGMENTO seg on ms.seg_id = seg.seg_id
                            join UPM_MAILING mai on mai.seg_id = seg.seg_id
                          	join UPM_EMAIL ema on ema.mai_id = mai.mai_id
                        where
                            ms.msg_id = @id;";


        string selectContatosJaEnviados = @"select
                                                ema.ema_id                          IdEmail
                                                , ema.mai_id                         IdMailing
                                                , ema.ema_nome                       Nome
                                                , ema.ema_email                      Email
                                                , ema.ema_empresa                    Empresa
                                                , ema.ema_status                     Status
                                                , ema.ema_opt                        Opt
                                                , ema.ema_enviadoPergunta            EnviadoPergunta
                                                , ema.ema_codigoPortal               CodigoPortal
                                                , ema.logininclusao                  IdUsuarioInclusao
                                                , ema.loginalteracao                 IdUsuarioAlteracao
                                                , ema.datainclusao                   DataInclusao
                                                , ema.dataalteracao                  DataAlteracao
                                                , ema.ema_cargo                      Cargo
                                                , ema.ema_tratamento                 Tratamento
                                            from
                                                UPM_MSGEMAIL_EMAIL me
                                                inner join UPM_EMAIL ema on ema.ema_id = me.ema_id
                                            where
                                                me.msg_id = @id";



        public Mensagem ObterPorId(int idMensagem)
        {
            var mensagem = new Mensagem();
            using (_conexao = new SqlConnection(ConfigurationManager.ConnectionStrings["revista_infra"].ConnectionString))
            {
                _conexao.Open();
                using (var resultados = _conexao.QueryMultiple(selectMensagem
                    + selectNoticias
                    + selectEmpresas
                    + selectSegmentos
                    + selectMailings
                    + selectContatos
                    + selectContatosJaEnviados, new { Id = idMensagem }))
                {
                    mensagem = resultados.Read<Mensagem>().Single();
                    mensagem.Noticias.AddRange(resultados.Read<Noticia>());
                    mensagem.Empresas.AddRange(resultados.Read<Empresa>());
                    mensagem.Segmentos.AddRange(resultados.Read<Segmento>());

                    var mailings = resultados.Read<Mailing>();
                    foreach (var mailingsPorSegmento in mailings.GroupBy(x => x.IdSegmento))
                    {
                        mensagem.Segmentos.Find(x => x.Id == mailingsPorSegmento.Key).Mailings.AddRange(mailingsPorSegmento.ToList());
                    }

                    var contatos = resultados.Read<Contato>();
                    foreach (var contatosPorMailings in contatos.GroupBy(x => x.IdMailing))
                    {
                        mensagem.Segmentos.SelectMany(x => x.Mailings).Single(x => x.Id == contatosPorMailings.Key).Contatos.AddRange(contatosPorMailings.ToList());
                    }

                    mensagem.EmailsEnviados.AddRange(resultados.Read<Contato>());
                }
            }


            return mensagem;
        }

        public int InserirLink(int idMensagem, string link)
        {
            using (_conexao = new SqlConnection(ConfigurationManager.ConnectionStrings["revista_infra"].ConnectionString))
            {
                _conexao.Open();
                return _conexao.Query<int>("INSERT INTO upm_link(MSG_ID, LIN_URL, LIN_STATUS) values (@Id, @Link, 1); SELECT SCOPE_IDENTITY() ", new { Link = link, Id = idMensagem }).Single();
            }
        }



        public void InserirEnvioInfo(EnvioInfo envioInfo)
        {
            using (_conexao = new SqlConnection(ConfigurationManager.ConnectionStrings["revista_infra"].ConnectionString))
            {
                _conexao.Open();
                _conexao.Execute("INSERT INTO upm_msgemail_email(msg_id,ema_id,mem_dataEnvio,mem_status, mem_erro) values (@IdMensagem, @IdContato, getdate(), @Status, @Erro) ", envioInfo);
            }
        }

        public void Atualizar(Mensagem mensagem)
        {
            using (_conexao = new SqlConnection(ConfigurationManager.ConnectionStrings["revista_infra"].ConnectionString))
            {
                _conexao.Open();
                _conexao.Execute("UPDATE upm_msgemail set statusEnvio = @status, msg_inicioEnvio = @inicioEnvio, msg_fimEnvio = @fimEnvio where msg_id = @id ",
                    new
                    {
                        status = (int)mensagem.StatusEnvio,
                        id = mensagem.Id,
                        inicioEnvio = mensagem.InicioEnvio,
                        fimEnvio = mensagem.FimEnvio
                    });
            }
        }

        public void AtualizarStatus(StatusEnvio status, int idMensagem)
        {
            using (_conexao = new SqlConnection(ConfigurationManager.ConnectionStrings["revista_infra"].ConnectionString))
            {
                _conexao.Open();
                _conexao.Execute("UPDATE upm_msgemail set statusEnvio = @status where msg_id = @id ",
                    new
                    {
                        status = (int)status,
                        id = idMensagem,
                    });
            }
        }
    }
}