using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Consulta;
using Quartz;

namespace INVESTIMENTO.RENDAFIXA.CRONJOB.CronJob;

public class ServicoCronJobQueConsultaEAplicaRendimentoDiario(ILogger<ServicoCronJobQueConsultaEAplicaRendimentoDiario> logger,
    IServicoQueConsultaInvestimentoParaAplicarRendimento servicoQueConsultaInvestimentoParaAplicarRendimento) : IJob
{
    private readonly ILogger<ServicoCronJobQueConsultaEAplicaRendimentoDiario> _logger = logger;
    private readonly IServicoQueConsultaInvestimentoParaAplicarRendimento _servicoQueConsultaInvestimentoParaAplicarRendimento = servicoQueConsultaInvestimentoParaAplicarRendimento;

    public Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("OK DIARIO");

        return Task.CompletedTask;
    }
}
