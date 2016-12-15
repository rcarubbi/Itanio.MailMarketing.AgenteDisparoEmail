using System;

namespace Itanio.MailMarketing.AgenteDisparador.Domain
{
    public class Solicitacao
    {
        public long Id { get; set; }

        public StatusSolicitacao Status { get; set; }

        public int IdMensagem { get; set; }

        public TipoSolicitacao Tipo { get; set; }

        public decimal Percentual { get; set; }
        public DateTime? DataProcessamento { get;  set; }
        public decimal Quantidade { get; set; }
        public int Total { get; set; }

        [Obsolete("Utilize a propriedade DataAtualizacao")]
        public bool Atualizada { get; set; }
        public DateTime DataAtualizacao { get; set; }
    }
}