using System;
using System.Collections.Generic;
using System.Threading;
using NLog;

namespace Itanio.MailMarketing.AgenteDisparador
{
    public class ServicoDisparadorEmail
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public static Dictionary<int, CancellationTokenSource> Canceladores { get; set; }

        public bool Iniciar()
        {
            Canceladores = new Dictionary<int, CancellationTokenSource>();
            Console.WriteLine($"Agente de disparo de emails iniciado");
            logger.Trace("Servico iniciado com sucesso");
            return true;
        }

        public bool Parar()
        {
            logger.Trace("Serviço encerrado com sucesso");
            return true;
        }
    }
}