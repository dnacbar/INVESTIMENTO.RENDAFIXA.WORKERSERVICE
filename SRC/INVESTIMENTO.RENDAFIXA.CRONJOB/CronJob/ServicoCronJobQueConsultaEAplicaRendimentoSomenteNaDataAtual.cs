using Quartz;
using DN.LOG.LIBRARY.MODEL;
using DN.LOG.LIBRARY.MODEL.ENUM;

namespace INVESTIMENTO.RENDAFIXA.CRONJOB.CronJob;

public class ServicoCronJobQueConsultaEAplicaRendimentoSomenteNaDataAtual(ILogger<ServicoCronJobQueConsultaEAplicaRendimentoSomenteNaDataAtual> logger) : IJob
{
    private readonly ILogger<ServicoCronJobQueConsultaEAplicaRendimentoSomenteNaDataAtual> _logger = logger;

    public Task Execute(IJobExecutionContext context)
    {
        try
        {
            throw new Exception("VAI PRA CASA DO RAIO MESMO QUE LÁ VAI SER LEGAL O ROLÊ!");
        }
        catch (Exception ex)
        {
            ex.CreateLog(_logger, EnumLogLevel.Warning);
        }

        return Task.CompletedTask;
    }
}
