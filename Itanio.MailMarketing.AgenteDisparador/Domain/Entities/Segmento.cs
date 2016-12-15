using System;
using System.Collections.Generic;

namespace Itanio.MailMarketing.AgenteDisparador.Domain
{
    public class Segmento
    {
        public Segmento()
        {
            Mailings = new List<Mailing>();
        }
        public int Id { get; set; }

        public string Nome { get; set; }

        public string Descricao { get; set; }

        public bool Status { get; set; }

        public int IdUsuarioInclusao { get; set; }

        public int IdUsuarioAlteracao { get; set; }

        public DateTime DataInclusao { get; set; }

        public DateTime DataAlteracao { get; set; }

        public List<Mailing> Mailings { get; set; }
    }
}