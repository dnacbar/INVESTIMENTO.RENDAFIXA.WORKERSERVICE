using DN.LOG.LIBRARY.MODEL;
using DN.LOG.LIBRARY.MODEL.ENUM;
using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Feriado.Servico;
using System.Security.Cryptography;

namespace INVESTIMENTO.RENDAFIXA.CRONJOB.CronJob;

public class CronJobConsultaEAtualizaFeriadoNacional(ILogger<CronJobConsultaEAtualizaFeriadoNacional> _logger,
    AtualizaOAnoDaListaDeFeriadoNacional _atualizaOAnoDaListaDeFeriadoNacional) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {   
        while (!stoppingToken.IsCancellationRequested)
        {
            if (DateTime.Today == new DateTime(DateTime.Today.Year, 12, 31))
            {
                try
                {
                    _logger.LogWarning("Iniciando processamento do feriado nacional - {data}.", [DateTimeOffset.Now.Date.ToLongDateString().ToUpperInvariant()]);

                    await _atualizaOAnoDaListaDeFeriadoNacional.ExecutaAsync(stoppingToken);
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
                    _logger.LogWarning("Processamento do feriado nacional concluído - {data}.", [DateTimeOffset.Now.Date.ToLongDateString().ToUpperInvariant()]);
                }

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
            else
            {
                _logger.LogWarning("Aguardando até {ProximoDia} para próxima verificação...", [DateTime.Today.AddDays(1).ToLongDateString().ToUpperInvariant()]);

                await Task.Delay(DateTime.Today.AddDays(1) - DateTime.Now, stoppingToken);
            }
        }
    }
}
