using DN.LOG.LIBRARY.MODEL;
using DN.LOG.LIBRARY.MODEL.ENUM;
using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.Servico;
using Quartz;
using System.Diagnostics;
using System.Security.Cryptography;

namespace INVESTIMENTO.RENDAFIXA.CRONJOB.CronJob;

public sealed class CronJobConsultaEResgataInvestimentoLiquidado(ILogger<CronJobConsultaEResgataInvestimentoLiquidado> _logger,
    AdicionaOResgateNoInvestimentoLiquidado _adicionaOResgateNoInvestimentoLiquidado) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        context.CancellationToken.ThrowIfCancellationRequested();
        var stopwatch = Stopwatch.StartNew();
        try
        {
            using var scope = _logger.BeginScope("Job {JobId}.", context.FireInstanceId);
            _logger.LogWarning("Iniciando processamento do resgate em investimento liquidado - {data}.", [DateTimeOffset.Now.Date.ToLongDateString().ToUpperInvariant()]);

            await _adicionaOResgateNoInvestimentoLiquidado.ExecutaAsync(context.CancellationToken);
        }
        catch (Exception ex)
        {
            if (ex is TaskCanceledException || ex is NotFoundException)
            {
                ex.CreateLog(_logger, EnumLogLevel.Warning);
            }
            else if (ex is DataBaseException || ex is DomainException)
            {
                ex.CreateLog(_logger, EnumLogLevel.Error);
            }
            else if (ex is CryptographicException)
            {
                ex.CreateLog(_logger, EnumLogLevel.Critical);
                throw;
            }
            else
                ex.CreateLog(_logger, EnumLogLevel.Critical);
        }
        finally
        {
            stopwatch.Stop();
            _logger.LogWarning("Processamento do resgate em investimento liquidado concluído em {tempo} segundos - {data}.", [stopwatch.ElapsedMilliseconds / 1000, DateTimeOffset.Now.Date.ToLongDateString().ToUpperInvariant()]);
        }
    }
}
