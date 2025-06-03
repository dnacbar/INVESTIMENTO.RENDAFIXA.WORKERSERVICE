using DN.LOG.LIBRARY.MODEL;
using DN.LOG.LIBRARY.MODEL.ENUM;
using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using INVESTIMENTO.RENDAFIXA.CRONJOB.Servico;
using Quartz;
using System.Diagnostics;
using System.Security.Cryptography;

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
            context.CancellationToken.ThrowIfCancellationRequested();
            await _aplicaRendimentoNaPosicaoDeHoje.ExecutaAsync(context.CancellationToken);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex.Message);
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
        catch (CryptographicException ex)
        {
            ex.CreateLog(_logger, EnumLogLevel.Critical);
            throw;
        }
        catch (Exception ex)
        {
            ex.CreateLog(_logger, EnumLogLevel.Critical);
        }
        finally
        {
            stopwatch.Stop();
            _logger.LogWarning("Processamento concluído em {tempo} segundos. - {data}", [stopwatch.ElapsedMilliseconds / 1000, DateTimeOffset.Now.Date.ToLongDateString().ToUpperInvariant()]);
        }
    }
}
