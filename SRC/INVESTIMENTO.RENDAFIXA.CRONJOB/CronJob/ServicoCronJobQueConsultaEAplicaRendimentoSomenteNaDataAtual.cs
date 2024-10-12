using Quartz;

namespace INVESTIMENTO.RENDAFIXA.CRONJOB.CronJob;

public class ServicoCronJobQueConsultaEAplicaRendimentoSomenteNaDataAtual(ILogger<ServicoCronJobQueConsultaEAplicaRendimentoSomenteNaDataAtual> logger) : IJob
{
    private readonly ILogger<ServicoCronJobQueConsultaEAplicaRendimentoSomenteNaDataAtual> _logger = logger;

    public Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("OK DATA ATUAL");
        return Task.CompletedTask;
    }
}
