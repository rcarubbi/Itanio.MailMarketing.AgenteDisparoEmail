using System.Collections.Generic;

namespace Itanio.MailMarketing.AgenteDisparador.Domain
{
    public class Mailing
    {
        public Mailing()
        {
            Contatos = new List<Contato>();
        }
        public int Id { get; set; }

        public int IdSegmento { get; set; }

        public List<Contato> Contatos { get; set; }
    }
}