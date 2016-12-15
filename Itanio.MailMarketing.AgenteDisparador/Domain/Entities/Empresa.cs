namespace Itanio.MailMarketing.AgenteDisparador.Domain
{
    public class Empresa
    {
        public int Id { get; set; }

        public string Destaque{ get; set; }

        public string NomeFantasia { get; set; }

        public string RazaoSocial { get; set; }

        public string Endereco { get; set; }

        public string Bairro { get; set; }

        public string Cidade { get; set; }

        public string Estado{ get; set; }

        public string CEP { get; set; }

        public string DDDTelefone { get; set; }

        public string Telefone { get; set; }

        public string DDDFax { get; set; }

        public string Fax { get; set; }

        public string Email { get; set; }

        public string WebSite { get; set; }

        public string  Video { get; set; }

        public string  Atuacao { get; set; }

        public string Contato { get; set; }

        public string Figura { get; set; }

        public string FiguraDestaque { get; set; }

        public string Descricao { get; set; }

        public int? VU { get; set; }

        public string Ativo { get; set; }

        public int? Exibir { get; set; }

        public int? PV { get; set; }

        public int IdLink { get; set; }
    }
}