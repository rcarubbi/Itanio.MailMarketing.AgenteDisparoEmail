using Itanio.MailMarketing.AgenteDisparador.Domain;

namespace Itanio.MailMarketing.AgenteDisparador
{
    internal interface IMensagemRepository
    {
        Mensagem ObterPorId(int idMensagem);
        int InserirLink(int idMensagem, string link);
        void InserirEnvioInfo(EnvioInfo envioInfo);
        void Atualizar(Mensagem mensagem);
        void AtualizarStatus(StatusEnvio status, int idMensagem);
    }
}