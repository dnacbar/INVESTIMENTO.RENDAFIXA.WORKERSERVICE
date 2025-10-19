using INVESTIMENTO.RENDAFIXA.DOMAIN.Configuracao;

namespace INVESTIMENTO.RENDAFIXA.CRONJOB.Configuracao;

internal class InvestimentoRendaFixaWorkerService(ConnectionString connectionString, 
    CronJobAdicionaRendimento cronJobAdicionaRendimento, 
    CronJobLiquidaPelaData cronJobLiquidaPelaData,
    CronJobResgataLiquidado cronJobResgataLiquidado,
    string usuario, short tempoLimiteTransacion) : IInvestimentoRendaFixaWorkerService
{
    public ConnectionString ConnectionString { get; } = connectionString;
    public CronJobAdicionaRendimento CronJobAdicionaRendimento { get; } = cronJobAdicionaRendimento;
    public CronJobLiquidaPelaData CronJobLiquidaPelaData { get; } = cronJobLiquidaPelaData;
    public CronJobResgataLiquidado CronJobResgataLiquidado { get; } = cronJobResgataLiquidado;
    public string Usuario { get; } = usuario;
    public short TempoLimiteTransacion { get; } = tempoLimiteTransacion;
}

internal class ConnectionString(string dbRendaFixa)
{
    public string DBRENDAFIXA { get; } = dbRendaFixa;
}

internal class CronJobAdicionaRendimento(string diario, string erro)
{
    public string Diario { get; } = diario;
    public string Erro { get; } = erro;
}

internal class CronJobLiquidaPelaData(string diario)
{
    public string Diario { get; } = diario;
}

internal class CronJobResgataLiquidado(string diario)
{
    public string Diario { get; } = diario;
}