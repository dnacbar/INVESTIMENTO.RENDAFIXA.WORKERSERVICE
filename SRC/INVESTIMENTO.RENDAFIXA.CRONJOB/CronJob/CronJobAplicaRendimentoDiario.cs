using DN.LOG.LIBRARY.MODEL;
using DN.LOG.LIBRARY.MODEL.ENUM;
using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.Servico;
using Quartz;
using System.Diagnostics;
using System.Security.Cryptography;

namespace INVESTIMENTO.RENDAFIXA.CRONJOB.CronJob;

public class CronJobAplicaRendimentoDiario(ILogger<CronJobAplicaRendimentoDiario> _logger,
    AplicaORendimentoNaPosicaoDeHoje _aplicaRendimentoNaPosicaoDeHoje) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        context.CancellationToken.ThrowIfCancellationRequested();
        var stopwatch = Stopwatch.StartNew();
        try
        {
            using var scope = _logger.BeginScope("Job {JobId}.", context.FireInstanceId);
            _logger.LogWarning("Iniciando processamento do rendimento diário - {data}.", [DateTimeOffset.Now.Date.ToLongDateString().ToUpperInvariant()]);

            await _aplicaRendimentoNaPosicaoDeHoje.ExecutaAsync(context.CancellationToken);
        }
        catch (TaskCanceledException ex)
        {
            ex.CreateLog(_logger, EnumLogLevel.Warning, new System.Net.IPAddress(1));
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex.Message);
        }
        catch (DataBaseException ex)
        {
            ex.CreateLog(_logger, EnumLogLevel.Error, new System.Net.IPAddress(1));
        }
        catch (DomainException ex)
        {
            ex.CreateLog(_logger, EnumLogLevel.Error, new System.Net.IPAddress(1));
        }
        catch (CryptographicException ex)
        {
            ex.CreateLog(_logger, EnumLogLevel.Critical, new System.Net.IPAddress(1));
            throw;
        }
        catch (Exception ex)
        {
            ex.CreateLog(_logger, EnumLogLevel.Critical, new System.Net.IPAddress(1));
        }
        finally
        {
            stopwatch.Stop();
            _logger.LogWarning("Processamento concluído em {tempo} segundos. - {data}", [stopwatch.ElapsedMilliseconds / 1000, DateTimeOffset.Now.Date.ToLongDateString().ToUpperInvariant()]);
        }
    }
}
