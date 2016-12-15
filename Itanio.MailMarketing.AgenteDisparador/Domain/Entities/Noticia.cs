using System;

namespace Itanio.MailMarketing.AgenteDisparador.Domain
{
    public class Noticia
    {
        public int Id { get; set; }

        public string Secao { get; set; }

        public string Titulo { get; set; }

        public string Figura { get; set; }

        public string FiguraDescricao { get; set; }

        public string Resumo { get; set; }


        public string ResumoEnews { get; set; }

        public string Enews { get; set; }

        public string Corpo { get; set; }

        public DateTime? DataInclusao { get; set; }

        public DateTime? DataModificacao { get; set; }

        public string Autor { get; set; }

        public string Status { get; set; }

        public int Visitas { get; set; }

        public string Edicao { get; set; }

        public bool Privado { get; set; }

        public string Editoria { get; set; }

        public string Enviada { get; set; }

        public string Regiao { get; set; }

        public string Tipo { get; set; }

        public bool ExibirCase { get; set; }

        public bool ExibirArtigo { get; set; }

        public int Ordem { get; set; }

        public string Destaque { get; set; }

        public int Relacionado { get; set; }

        public bool DestaquePainel { get; set; }

        public string Entrevista { get; set; }

        public bool LideresAudiencia { get; set; }

        public bool EditorMErcado { get; set; }

        public string FiguraIndice { get; set; }

        public string FiguraIndiceDescricao { get; set; }

        public int IdEditora { get; set; }

        public int IdEdicao { get; set; }

        public string FiguraNewsletter { get; set; }

        public string DescricaoFiguraNewsletter { get; set; }

        public int IdLink { get; set; }

    }
}