using INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Usuario;

namespace INVESTIMENTO.RENDAFIXA.CRONJOB.Configuracao;

public class InvestimentoRendaFixaCronJob(ConnectionString connectionString, ConfiguraCronJobAplicaRendimento configuraCronJobAplicaRendimento, string usuario) : IUsuarioInvestimentoRendaFixaCronJob
{
    public ConnectionString ConnectionString { get; } = connectionString;
    public ConfiguraCronJobAplicaRendimento ConfiguraCronJobAplicaRendimento { get; } = configuraCronJobAplicaRendimento;
    public string Usuario { get; } = usuario;
}

public class ConnectionString(string dbRendaFixa)
{
    public string DBRENDAFIXA { get; } = dbRendaFixa;
}

public class ConfiguraCronJobAplicaRendimento(string diario, string erro)
{
    public string Diario { get; } = diario;
    public string Erro { get; } = erro;
}