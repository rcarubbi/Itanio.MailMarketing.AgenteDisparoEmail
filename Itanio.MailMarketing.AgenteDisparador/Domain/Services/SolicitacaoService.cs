using System.Collections.Generic;
using System.Linq;
using Itanio.MailMarketing.AgenteDisparador.DAL;

namespace Itanio.MailMarketing.AgenteDisparador.Domain
{
    public class SolicitacaoService
    {
        public List<Solicitacao> ListarProximasSolicitacoesPendentes(int qtd)
        {
            ISolicitacaoRepository repo = new DapperSolicitacaoRepository();
            return repo.ListarPorStatus(StatusSolicitacao.Pendente).Take(qtd).ToList();
        }
    }
}