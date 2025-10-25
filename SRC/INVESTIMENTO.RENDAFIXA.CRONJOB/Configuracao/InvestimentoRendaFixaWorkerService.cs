using INVESTIMENTO.RENDAFIXA.DOMAIN.Configuracao;

namespace INVESTIMENTO.RENDAFIXA.CRONJOB.Configuracao;

internal sealed class InvestimentoRendaFixaWorkerService(ConnectionString connectionString, 
    CronJobAdicionaRendimento cronJobAdicionaRendimento, 
    string cronJobLiquidaPelaData,
    string cronJobResgataLiquidado,
    string usuario, 
    short tempoLimiteTransaction) : IInvestimentoRendaFixaWorkerService
{
    public ConnectionString ConnectionString { get; } = connectionString;
    public CronJobAdicionaRendimento CronJobAdicionaRendimento { get; } = cronJobAdicionaRendimento;
    public string CronJobLiquidaPelaData { get; } = cronJobLiquidaPelaData;
    public string CronJobResgataLiquidado { get; } = cronJobResgataLiquidado;
    public string Usuario { get; } = usuario;
    public short TempoLimiteTransaction { get; } = tempoLimiteTransaction;
}

internal class CronJobAdicionaRendimento(string diario, string erro)
{
    public string Diario { get; } = diario;
    public string Erro { get; } = erro;
}