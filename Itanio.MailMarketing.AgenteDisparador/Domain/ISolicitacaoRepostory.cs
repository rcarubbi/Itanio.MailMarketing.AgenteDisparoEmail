using System.Linq;

namespace Itanio.MailMarketing.AgenteDisparador.Domain
{
    public interface ISolicitacaoRepository
    {
        IQueryable<Solicitacao> ListarPorStatus(StatusSolicitacao pendente);
        void Atualizar(Solicitacao item);
        Solicitacao ObterPorId(long id);
        IQueryable<Solicitacao> ListarPorMensagem(int idMensagem);
    }
}