using DN.LOG.LIBRARY.MODEL;
using DN.LOG.LIBRARY.MODEL.ENUM;
using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using INVESTIMENTO.RENDAFIXA.CRONJOB.Servico;
using Quartz;
using System.Diagnostics;

namespace INVESTIMENTO.RENDAFIXA.CRONJOB.CronJob;

public class CronJobAplicaRendimentoDiario(ILogger<CronJobAplicaRendimentoDiario> _logger,
    AplicaORendimentoNaPosicaoDeHoje _aplicaRendimentoNaPosicaoDeHoje) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        using var scope = _logger.BeginScope("Job {JobId}.", context.FireInstanceId);
        _logger.LogWarning("Iniciando processamento do rendimento diário - {data}.", [DateTimeOffset.Now.Date.ToLongDateString().ToUpperInvariant()]);

        var stopwatch = Stopwatch.StartNew();
        try
        {
            await _aplicaRendimentoNaPosicaoDeHoje.ExecutaAsync(context.CancellationToken);
            stopwatch.Stop();
            _logger.LogWarning("Processamento concluído em {tempo} segundos. - {data}", [stopwatch.ElapsedMilliseconds / 1000, DateTimeOffset.Now.Date.ToLongDateString().ToUpperInvariant()]);
        }
        catch (NotFoundException ex)
        {
            ex.CreateLog(_logger, EnumLogLevel.Information);
        }
        catch (TaskCanceledException ex)
        {
            ex.CreateLog(_logger, EnumLogLevel.Warning);
        }
        catch (AggregateException ex)
        {
            foreach (var innerEx in ex.Flatten().InnerExceptions)
                innerEx.CreateLog(_logger, EnumLogLevel.Error);
        }
        catch (InvalidOperationException ex)
        {
            ex.CreateLog(_logger, EnumLogLevel.Error);
        }
        catch (Exception ex)
        {
            ex.CreateLog(_logger, EnumLogLevel.Critical);
            //throw; // Re-throw after logging to ensure the job is marked as failed
        }
    }
}
