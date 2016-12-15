using NLog;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Itanio.MailMarketing.AgenteDisparador
{
    public class ServicoDisparadorEmail
    {
        public static Dictionary<int, CancellationTokenSource> Canceladores { get; set; }

        private static Logger logger = LogManager.GetCurrentClassLogger();
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