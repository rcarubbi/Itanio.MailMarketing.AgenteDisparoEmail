using Quartz;
using Topshelf;
using Topshelf.Quartz;

namespace Itanio.MailMarketing.AgenteDisparador
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            HostFactory.Run(serviceConfig =>
            {
                serviceConfig.UseNLog();


                serviceConfig.Service<ServicoDisparadorEmail>(serviceInstance =>
                {
                    serviceInstance.ConstructUsing(name => new ServicoDisparadorEmail());
                    serviceInstance.WhenStarted(execute => execute.Iniciar());
                    serviceInstance.WhenStopped(execute => execute.Parar());

                    serviceInstance.ScheduleQuartzJob(q =>
                        q.WithJob(() =>
                                JobBuilder.Create<MonitorarFilaSolicitacoes>().Build())
                            .AddTrigger(() =>
                                TriggerBuilder.Create()
                                    .WithSimpleSchedule(builder => builder
                                        .WithIntervalInSeconds(5)
                                        .RepeatForever())
                                    .Build())
                    );
                });

                serviceConfig.EnableServiceRecovery(recoveryOption => { recoveryOption.RestartService(5); });

                serviceConfig.SetServiceName("ItanioAgenteDisparadorEmail");
                serviceConfig.SetDisplayName("Itanio - Agente de disparo de emails");
                serviceConfig.SetDescription("Agente responsável pelo envio de emails em massa");
                serviceConfig.StartAutomatically();
            });
        }
    }
}