using Carubbi.Utils.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace Itanio.MailMarketing.AgenteDisparador.Domain
{
    public static class GeradorMensagens
    {
    


        internal static IEnumerable<KeyValuePair<int, MailMessage>> Gerar(Mensagem mensagem)
        {
            GerarLinks(mensagem);
            string corpo = GerarCorpo(mensagem);
     


            var contatos = mensagem.Segmentos.SelectMany(x => x.Mailings).SelectMany(x => x.Contatos).ToList();
            var equalityComparer = new GenericEqualityComparer<Contato>("Email");
            var contatosNaoEnviados = contatos.Distinct(equalityComparer)
                                              .Except(mensagem.EmailsEnviados, equalityComparer)
                                              .Where(x => x.Opt);

          
            
            foreach (var contato in contatosNaoEnviados)
            {
                MailMessage email = new MailMessage();
                email.Body = corpo.Replace("_$Emai$_", contato.Id.ToString())
                                  .Replace("IdUsuario", contato.Id.ToString())
                                  .Replace("#NomeCompleto#", contato.Nome)
                                  .Replace("#PrimeiroNome#", contato.Nome.Split(' ')[0])
                                  .Replace("#Tratamento#", (contato.Tratamento ?? string.Empty).Replace(" ", string.Empty))
                                  .Replace("#Email#", contato.Email)
                                  .Replace("#Cargo#", (contato.Cargo ?? string.Empty))
                                  .Replace("#Empresa#", (contato.Empresa ?? string.Empty));

                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(email.Body, null, MediaTypeNames.Text.Html);
                AlternateView textView = AlternateView.CreateAlternateViewFromString(email.Body, null, MediaTypeNames.Text.Plain);
                email.AlternateViews.Add(textView);
                email.AlternateViews.Add(htmlView);
                email.IsBodyHtml = true;
                email.Subject = mensagem.Assunto;
                email.From = new MailAddress(mensagem.RemententeEmail, mensagem.RemententeNome);
                email.To.Add(new MailAddress(contato.Email, contato.Nome));
                yield return new KeyValuePair<int, MailMessage>(contato.Id, email);
            }
        }

        private static string GerarCorpo(Mensagem mensagem)
        {
            Uri url = new Uri(ConfigurationManager.AppSettings["url"]);
           

            string htmlMensagemCabecalho = $@"<table style='display:none' cellspacing=0 cellpadding=0><tr><td><img src='{url.ToString()}acessou.asp?m={mensagem.Id}&e=_$Emai$_' alt='Acesse o site da Infra'></td></tr></table>
                <table align='center' border='0' cellpadding='0' cellspacing='0' class='main' style='padding: 0 0 30px 0;' width='600'>
                <tbody>
                <tr><td style = 'color: #ffffff; font-size: 11px; font-family: Arial; padding: 5px 10px;'>&nbsp;</td></tr>
                <tr><td height = '175' >
                    <table border='0' cellpadding='0' cellspacing='0' class='title' height='163' style='background-color: #5b6789; width:100%;'>
                        <tbody>
                            <tr><td colspan = '3' height='138' style='text-align:center; background-color:#d93044;'>
                                <a href='{url.ToString()}link.asp?l={mensagem.IdLinkHome}&e={mensagem.Id}&m=IdUsuario'>
                                <img alt='Veja o que temos de novidades no mercado Facilities' 
                                     src='http://www.revistainfra.com.br/arquivos/topo_news_Revista-Infra.jpg' 
                                     style='margin-top:10px; margin-bottom:5px;' />
                                </a></td></tr>
                            <tr><td colspan = '3' style='text-align:center; background-color:#d93044; font-size: 16px;font-family: Arial;line-height:38px; color:white'>
                            Boletim informativo semanal da Revista Infra - {DateTime.Today.Day} de {DateTimeFormatInfo.CurrentInfo.GetMonthName(DateTime.Today.Month)}</td></tr>
                        </tbody>
                    </table>
                </td></tr>
                <tr><td>
                    <table border = '0' cellpadding='0' cellspacing='0' class='top-news' style='background-color: #eaf0f0;'>
                        <tbody><tr>
                        <td>&nbsp;</td>
                        <td valign='top'>&nbsp;</td>
                        <td>&nbsp;</td></tr>";

            StringBuilder corpo = new StringBuilder(htmlMensagemCabecalho);


            var ordens = new Tuple<int, string>[] {
                Tuple.Create(mensagem.OrdemNoticia, "Notícia"),
                Tuple.Create(mensagem.OrdemPersonalizada, "Personalizada"),
                Tuple.Create(mensagem.OrdemProduto, "Empresa") };

            foreach (var secao in ordens.OrderBy(x => x.Item1))
            {
                switch (secao.Item2)
                {
                    case "Notícia":
                        corpo.Append(GerarParteMensagemNoticias(mensagem));
                        break;
                    case "Personalizada":
                        corpo.Append(GerarParteMensagemPersonalizada(mensagem));
                        break;
                    case "Empresa":
                        corpo.Append(GerarParteMensagemEmpresas(mensagem));
                        break;
                    default:
                        break;
                }
            }

            string htmlMensagemRodape = $@"<tr><td>&nbsp;</td><td valign='top'>&nbsp;</td><td>&nbsp;</td></tr></tbody></table></td></tr><tr><td> 
                                           <table border='0' cellpadding='0' cellspacing='0' class='final-news' style='background-color: #bb3642;'>
                                            <tbody><tr><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td></tr><tr> 
                                            <td style='background-color:#bb3642;' width='30'>&nbsp;</td><td style='background-color:#bb3642' width='540'> 
                                            <table border='0' cellpadding='10' cellspacing='10' width='100%'><tbody> 
                                            <tr><td style='font-family:arial; font-size:12px; color:#ffffff; text-align:center;'><div> 
                                            <div>COPYRIGHT &copy; 2003-2016 TALEN EDITORA<br />TODOS OS DIREITOS RESERVADOS</div>
                                            <div>Avenida Jabaquara, 99 3. andar - Cj. 35 - Mirand&oacute;polis<br /> Cep 04045-000 S&atilde;o Paulo/SP - Tel. 11 5582.3044</div> 
                                            </div></td></tr></tbody></table></td><td style='background-color:#bb3642;' width='30'>&nbsp;</td></tr><tr> 
                                            <td style='background-color:#bb3642;'>&nbsp;</td><td style='background-color:#bb3642'>&nbsp;</td> 
                                            <td style='background-color:#bb3642;'>&nbsp;</td></tr></tbody></table></td></tr><tr><td>
                                            <div style='width:100%; margin:20px auto; text-align:center;'> 
                                            <a style='color:#000; text-decoration:none; font-size:11px;' href='{url.ToString()}link.asp?b=1&l={mensagem.IdLinkBloqueio}&e={mensagem.Id}&m=IdUsuario'> 
                                            Não desejo receber informativos da Revista Infra </a></div></td></tr></tbody></table>";

            corpo.Append(htmlMensagemRodape);

            string strCorpo = corpo.ToString().Replace("href=mailto", "href2=mailto")
                    .Replace("href='mailto", "href2='mailto")
                    .Replace(@"href=""mailto", @"href2=""mailto")
                    .Replace(@"src=""http://", @"src2=""http://")
                    .Replace("src='http://", "src2='http://")
                    .Replace("href='http://", "href2='http://")
                    .Replace(@"href=""http://", @"href2=""http://")
                    .Replace($"src='", $"src='{url}")
                    .Replace($@"src=""", $@"src=""{url}")
                    .Replace($@"background=""", $@"background=""{url}")
                    .Replace("background='", $"background='{url}")
                    .Replace(@"href=""", $@"href=""{url}")
                    .Replace("href = '", $"href = '{url}")
                    .Replace("href='", $"href='{url}")
                    .Replace(@"<param name=""movie"" value=""", $@"<param name=""movie"" value=""{url}")
                    .Replace("href2=", "href=")
                    .Replace("src2=", "src=")
                    .Replace("/novoportal/novoportal/", "/novoportal/")
                    .Replace("/novoportal//novoportal/", "/novoportal/")
                    .Replace("_$siteServer$_", url.Scheme + url.Authority)
                    .Replace("_$MsgEmail$_", mensagem.Id.ToString())
                    .Replace("&lt;_form", "<form")
                    .Replace("&lt;_/form&gt;", "</form>");
            return strCorpo;
        }

        private static string GerarParteMensagemPersonalizada(Mensagem mensagem)
        {
            string htmlMensagemPersonalizada = string.Empty;
            if (mensagem.Personalizada)
            {
                htmlMensagemPersonalizada = $@"<tr>  
                       <td width='30'> 
                            &nbsp; 
                         </td> 
                         <td width='540'>{mensagem.Corpo}</td>
                            <td width='30'> 
                                &nbsp; 
                            </td> 
                         </tr>";
            }

            return htmlMensagemPersonalizada;
        }

        private static void GerarLinks(Mensagem mensagem)
        {
            DapperMensagemRepository repo = new DapperMensagemRepository();
            mensagem.IdLinkHome = repo.InserirLink(mensagem.Id, "http://www.revistainfra.com.br");
            mensagem.IdLinkBloqueio = repo.InserirLink(mensagem.Id, $"http://www.revistainfra.com.br/tool/upmail/Bloquear.asp?e=IdUsuario&m={mensagem.Id}");
            mensagem.Empresas.ForEach(empresa => empresa.IdLink = repo.InserirLink(mensagem.Id, empresa.WebSite));
            mensagem.Noticias.ForEach(noticia => noticia.IdLink = repo.InserirLink(mensagem.Id, $"http://www.revistainfra.com.br/Textos/{noticia.Id}/{noticia.Titulo.Replace(' ', '-')}"));
        }


        private static string GerarParteMensagemEmpresas(Mensagem mensagem)
        {
            StringBuilder htmlEmpresas = new StringBuilder();

            if (mensagem.Empresas.Count > 0)
            {
                htmlEmpresas.Append($@"
                <tr>
                    <td width='30'>&nbsp;</td>
                    <td style='color:#172d59;font-weight: bold;font-size: 34px;font-family: Arial;margin:0; padding-top:30px;'>Empresas em destaque no site da Revista Infra</td>
                    <td width='30'>&nbsp;</td>
                </tr>
                <tr>
                    <td width='30'>&nbsp;</td>
                    <td style='color:#172d59;font-normal: bold;font-size: 20px;font-family: Arial;margin:0; padding-top:0px;'>Abaixo você encontrará empresas do mercado de FM que são destaques em nosso setor.</td>
                    <td width='30'>&nbsp;</td>
                </tr>");
            }

            foreach (var item in mensagem.Empresas)
            {
                htmlEmpresas.Append("<tr><td width='30'>&nbsp;</td><td width='540'>");
                htmlEmpresas.Append("<table border='0' cellpadding='0' cellspacing='0' height='146' width='540'>");
                htmlEmpresas.Append("<tbody><tr><td>&nbsp;</td></tr>");
                if (item.Figura.Length > 0)
                {
                    htmlEmpresas.Append("<tr><td style='background: #fff;text-align: center;'>");
                    htmlEmpresas.Append($"<a href='http://www.revistainfra.com.br/tool/upmail/link.asp?m={item.Id}&l={item.IdLink}&e={mensagem.Id}' target='_blank' style='text-decoration:none;color: #172d59;font-weight:normal;'>");
                    htmlEmpresas.Append($"<img alt='' src='http://www.revistainfra.com.br/arquivos/{item.Figura}'/></a></td></tr>");
                }
                htmlEmpresas.Append("<tr><td><h2 style='color: #172d59;font-weight: normal;font-size: 25px;font-family: Arial;margin:12px 0 0 0px;'>");
                htmlEmpresas.Append($"<tr><td><h2 style='color: #172d59;font-weight: normal;font-size: 25px;font-family: Arial;margin:12px 0 0 0px;'>{item.RazaoSocial}</h2>");
                htmlEmpresas.Append($"</td></tr><tr><td width='365'><p style='color: #515354;font-size: 16px;font-family: Arial;line-height: 22px; martin-bottom:5px;'>");
                htmlEmpresas.Append($"{item.Endereco}<br />{item.Cidade}/{item.Estado}<br />{item.Bairro}-{item.CEP}<br />({item.DDDTelefone}) {item.Telefone}<br />{item.WebSite}");
                htmlEmpresas.Append($"</p></td></tr><tr><td><table border='0' cellpadding='10' cellspacing='5' width='240'>");
                htmlEmpresas.Append($"<tbody><tr><td style='background-color:#d93044'>");
                htmlEmpresas.Append($"<a href='http://www.revistainfra.com.br/tool/upmail/link.asp?m={item.Id}&l={item.IdLink}&e={mensagem.Id}' target='_blank' style='font-family:arial; font-size:16px; font-weight:bold; color:white; text-decoration:none;'>");
                htmlEmpresas.Append($"Visite o site dessa empresa</a></td></tr></tbody></table></td></tr><tr><td>&nbsp;</td></tr></tbody></table></td><td width='30'>");
                htmlEmpresas.Append($"&nbsp;</td></tr>");
            }

            return htmlEmpresas.ToString();
        }


        private static string GerarParteMensagemNoticias(Mensagem mensagem)
        {
            StringBuilder htmlNoticias = new StringBuilder();

            if (mensagem.Noticias.Count > 0)
            {
                htmlNoticias.Append(@"<tr><td width='30'>&nbsp;</td><td style='color:#172d59;font-weight: bold;font-size: 34px;font-family: Arial;margin:0; padding-top:30px;'>Notícias do setor de FM</td><td width='30'>&nbsp;</td></tr>
                                <tr><td width='30'>&nbsp;</td><td style='color:#172d59;font-normal: bold;font-size: 20px;font-family: Arial;margin:0; padding-top:0px;'>Abaixo selecionamos conteúdos de grande relevância do mercado de FM para você.</td><td width='30'>&nbsp;</td></tr>");
            }

            foreach (var item in mensagem.Noticias)
            {
                htmlNoticias.Append(@"<tr><td width='30'>&nbsp;</td><td width='540'> 
                                      <table border='0' cellpadding='0' cellspacing='0' height='146' width='540'> 
                                      <tbody><tr><td>&nbsp;</td></tr>");
                if (item.FiguraNewsletter.Length > 0)
                {
                    htmlNoticias.Append($@"<tr><td><a href='http://www.revistainfra.com.br/tool/upmail/link.asp?m={item.Id}&l={item.IdLink}&e={mensagem.Id}' target='_blank' style='text-decoration:none;color: #172d59;font-weight:normal;'>
                                           <img alt='{item.DescricaoFiguraNewsletter}' height='221' src='http://www.revistainfra.com.br/arquivos/{item.FiguraNewsletter}' width='539' />
                                           </a></td></tr>");
                }

                htmlNoticias.Append(@"<tr><td><h2 style='color:#172d59;font-weight: normal;font-size: 25px;font-family: Arial;margin: 0 0 0px;'>");
                htmlNoticias.Append($"{item.Titulo}</h2></td></tr><tr><td width='365'>");
                htmlNoticias.Append($"<p style='color: #515354;font-size: 16px;font-family: Arial;line-height: 22px;martin-bottom:5px;'>{item.Resumo}");
                htmlNoticias.Append($"</p></td></tr><tr><td><table border='0' cellpadding='10' cellspacing='5' width='150'>");
                htmlNoticias.Append($"<tbody><tr><td style='background-color:#d93044'>  ");
                htmlNoticias.Append($"<a href='http://www.revistainfra.com.br/tool/upmail/link.asp?m={item.Id}&l={item.IdLink}&e={mensagem.Id}' target='_blank' style='font-family:arial; font-size:16px; font-weight:bold; color:white; text-decoration:none;'>");
                htmlNoticias.Append($"Leia na &iacute;ntegra</a></td></tr></tbody></table></td></tr><tr><td>&nbsp;</td></tr></tbody></table></td><td width='30'>");
                htmlNoticias.Append($"&nbsp;</td></tr>");

            }

            return htmlNoticias.ToString();
        }
    }
}

