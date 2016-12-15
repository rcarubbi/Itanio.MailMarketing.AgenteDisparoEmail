using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Itanio.MailMarketing.AgenteDisparador.Domain;
using Itanio.MailMarketing.AgenteDisparador.DAL;
using System.Linq;
using System.Threading;

namespace Itanio.MailMarketing.AgenteDisparador.Tests
{
    [TestClass]
    public class DalTests
    {
        [TestMethod]
        public void ObterMensagem()
        {
            var dal = new DapperMensagemRepository();
            var mensagem = dal.ObterPorId(2255);

            Assert.IsTrue(mensagem != null);
        }

        


        [TestMethod]
        public void LerSolicitacao()
        {
            var service = new SolicitacaoService();
            ISolicitacaoRepository repo = new DapperSolicitacaoRepository();
            var solicitacoes = service.ListarProximasSolicitacoesPendentes(1);
            
            Assert.IsTrue(solicitacoes.Count > 0);
        }


        [TestMethod]
        public void ProcessarSolicitacao()
        {
            var service = new SolicitacaoService();
            ISolicitacaoRepository repo = new DapperSolicitacaoRepository();
            var solicitacao = service.ListarProximasSolicitacoesPendentes(1).First();

            MonitorarFilaSolicitacoes m = new MonitorarFilaSolicitacoes();
            PrivateObject p = new PrivateObject(m);
            p.Invoke("Enviar", solicitacao, new CancellationTokenSource());
            
            Assert.IsTrue(true);

        }
    }
}
