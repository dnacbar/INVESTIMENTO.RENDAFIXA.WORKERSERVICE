using Quartz;

namespace INVESTIMENTO.RENDAFIXA.CRONJOB.CronJob;

public sealed class CronJobConsultaEAplicaRendimentoDiarioComErro : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        Console.Write("OK");
        return Task.CompletedTask;
    }
}
