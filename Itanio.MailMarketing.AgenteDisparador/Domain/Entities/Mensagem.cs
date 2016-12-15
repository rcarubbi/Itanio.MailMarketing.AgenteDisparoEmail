using System;
using System.Collections.Generic;

namespace Itanio.MailMarketing.AgenteDisparador.Domain
{
    public class Mensagem
    {
        public Mensagem()
        {
            Empresas = new List<Empresa>();
            Noticias = new List<Noticia>();
            EmailsEnviados = new List<Contato>();
            Segmentos = new List<Segmento>();
        }

        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string RemententeNome { get; set; }
        public string RemententeEmail { get; set; }
        public string Assunto { get; set; }
        public string TituloCorpo { get; set; }
        public string Corpo { get; set; }
        public string CorpoPreparado { get; set; }
        public decimal Intervalo { get; set; }
        public decimal TempoResposta { get; set; }
        public DateTime? InicioEnvio { get; set; }
        public DateTime? FimEnvio { get; set; }
        public bool IniciarEnvio { get; set; }
        public bool Status { get; set; }
        public bool TextoOpt { get; set; }
        public int TotalEnvio { get; set; }
        public bool Noticia { get; set; }
        public bool Personalizada { get; set; }
        public bool Produto { get; set; }
        public int OrdemNoticia { get; set; }
        public int OrdemPersonalizada { get; set; }
        public int OrdemProduto { get; set; }
        public int IdUsuarioInclusao { get; set; }
        public int IdUsuarioAlteracao { get; set; }
        public DateTime DataInclusao { get; set; }
        public DateTime DataAlteracao { get; set; }
        public string TextoTopo { get; set; }

        public List<Empresa> Empresas { get; set; }

        public List<Noticia> Noticias { get; set; }

        public List<Contato> EmailsEnviados { get; set; }

        public List<Segmento> Segmentos { get; set; }

        public int IdLinkHome { get; set; }

        public int IdLinkBloqueio { get; set; }
        public StatusEnvio StatusEnvio { get; internal set; }
    }
}