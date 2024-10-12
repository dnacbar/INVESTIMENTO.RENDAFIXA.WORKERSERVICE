using Quartz;

namespace INVESTIMENTO.RENDAFIXA.CRONJOB.CronJob;

public class ServicoCronJobQueConsultaEAplicaRendimentoDiario(ILogger<ServicoCronJobQueConsultaEAplicaRendimentoDiario> logger) : IJob
{
    private readonly ILogger<ServicoCronJobQueConsultaEAplicaRendimentoDiario> _logger = logger;

    public Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("OK DIARIO");
        return Task.CompletedTask;
    }
}
