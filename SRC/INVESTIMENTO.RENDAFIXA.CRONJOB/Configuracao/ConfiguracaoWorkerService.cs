using INVESTIMENTO.RENDAFIXA.DOMAIN.Configuracao;
using INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Configuracao;
using System.Data.SqlClient;

namespace INVESTIMENTO.RENDAFIXA.CRONJOB.Configuracao;

internal sealed class ConfiguracaoWorkerService(
    ConnectionString connectionString,
    CronJobAdicionaRendimento cronJobAdicionaRendimento,
    string cronJobLiquidaPelaData,
    string cronJobResgataLiquidado,
    short tempoLimiteTransaction,
    string txUsuario) : IConfiguracaoDomainWorkerService, IConfiguracaoInfraWorkerService
{
    public CronJobAdicionaRendimento CronJobAdicionaRendimento { get; } = cronJobAdicionaRendimento;
    public string CronJobLiquidaPelaData { get; } = cronJobLiquidaPelaData;
    public string CronJobResgataLiquidado { get; } = cronJobResgataLiquidado;
    public short TempoLimiteTransaction { get; } = tempoLimiteTransaction;
    public string TxUsuario { get; } = txUsuario;
    public SqlConnection CreateConnectionSqlServer() => new(ConnectionString.DBRENDAFIXA);

    private ConnectionString ConnectionString { get; } = connectionString;
}

internal class CronJobAdicionaRendimento(string diario, string erro)
{
    public string Diario { get; } = diario;
    public string Erro { get; } = erro;
}

internal class ConnectionString(string dBRENDAFIXA)
{
    public string DBRENDAFIXA { get; } = dBRENDAFIXA;
}