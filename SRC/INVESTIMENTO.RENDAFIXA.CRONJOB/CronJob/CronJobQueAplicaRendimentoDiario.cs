using INVESTIMENTO.RENDAFIXA.CRONJOB.Servico;
using Quartz;

namespace INVESTIMENTO.RENDAFIXA.CRONJOB.CronJob;

public class CronJobQueAplicaRendimentoDiario(ILogger<CronJobQueAplicaRendimentoDiario> logger,
    ServicoQueAplicaRendimentoNaPosicaoDeHoje servicoQueListaInvestimentoSemBloqueio) : IJob
{
    private readonly ILogger<CronJobQueAplicaRendimentoDiario> _logger = logger;
    private readonly ServicoQueAplicaRendimentoNaPosicaoDeHoje _servicoQueListaInvestimentoSemBloqueio = servicoQueListaInvestimentoSemBloqueio;

    public Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("OK DIARIO");

        return Task.CompletedTask;
    }
}
