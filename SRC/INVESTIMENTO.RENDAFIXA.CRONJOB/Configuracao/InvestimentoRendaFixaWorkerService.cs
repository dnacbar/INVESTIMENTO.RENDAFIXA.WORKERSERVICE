using INVESTIMENTO.RENDAFIXA.DOMAIN.Configuracao;

namespace INVESTIMENTO.RENDAFIXA.CRONJOB.Configuracao;

internal class InvestimentoRendaFixaWorkerService(ConnectionString connectionString, CronJobAplicaRendimento cronJobAplicaRendimento, 
    string usuario, short tempoLimiteTransacion) : IInvestimentoRendaFixaWorkerService
{
    public ConnectionString ConnectionString { get; } = connectionString;
    public CronJobAplicaRendimento CronJobAplicaRendimento { get; } = cronJobAplicaRendimento;
    public string Usuario { get; } = usuario;
    public short TempoLimiteTransacion { get; } = tempoLimiteTransacion;
}

internal class ConnectionString(string dbRendaFixa)
{
    public string DBRENDAFIXA { get; } = dbRendaFixa;
}

internal class CronJobAplicaRendimento(string diario, string erro)
{
    public string Diario { get; } = diario;
    public string Erro { get; } = erro;
}