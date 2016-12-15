using Itanio.MailMarketing.AgenteDisparador.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Itanio.MailMarketing.AgenteDisparador.Domain
{
    public class SolicitacaoService
    {
        public List<Solicitacao> ListarProximasSolicitacoesPendentes(int qtd) {
            ISolicitacaoRepository repo = new DapperSolicitacaoRepository();
            return repo.ListarPorStatus(StatusSolicitacao.Pendente).Take(qtd).ToList();
        }
    }
}
