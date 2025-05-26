using Quartz;

namespace INVESTIMENTO.RENDAFIXA.CRONJOB.CronJob;

public class CronJobConsultaEAplicaRendimentoComErro : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        Console.Write("OK");
        return Task.CompletedTask;
    }
}
