namespace Itanio.MailMarketing.AgenteDisparador.Domain
{
    public class Contato
    {
        public int Id { get; set; }

        public int IdMailing { get; set; }

        public string Nome { get; set; }

        public string Email { get; set; }

        public string Empresa { get; set; }

        public int Status { get; set; }

        public bool Opt { get; set; }

        public bool EnviadoPergunta { get; set; }

        public int IdUsuarioInclusao { get; set; }

        public int IdUsuarioAlteracao { get; set; }

        public string Cargo { get; set; }

        public string Tratamento { get; set; }
    }
}