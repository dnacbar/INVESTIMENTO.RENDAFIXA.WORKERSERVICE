namespace INVESTIMENTO.RENDAFIXA.CRONJOB.Worker;

public class WorkerInvestimentoDiario(ILogger<WorkerInvestimentoDiario> logger) : BackgroundService
{
    private readonly ILogger<WorkerInvestimentoDiario> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }
            await Task.Delay(1000, stoppingToken);
        }
    }
}
