using INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Usuario;

namespace INVESTIMENTO.RENDAFIXA.CRONJOB.Configuracao;

internal class InvestimentoRendaFixaWorkerService(ConnectionString connectionString, ConfiguraCronJobAplicaRendimento configuraCronJobAplicaRendimento, string usuario) : IInvestimentoRendaFixaWorkerService
{
    public ConnectionString ConnectionString { get; } = connectionString;
    public ConfiguraCronJobAplicaRendimento ConfiguraCronJobAplicaRendimento { get; } = configuraCronJobAplicaRendimento;
    public string Usuario { get; } = usuario;
}

internal class ConnectionString(string dbRendaFixa)
{
    public string DBRENDAFIXA { get; } = dbRendaFixa;
}

internal class ConfiguraCronJobAplicaRendimento(string diario, string erro)
{
    public string Diario { get; } = diario;
    public string Erro { get; } = erro;
}