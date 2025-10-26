namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Configuracao;

public interface IInvestimentoRendaFixaWorkerService
{
    ConnectionString ConnectionString { get; }
    short TempoLimiteTransaction { get; }
}


public class ConnectionString(string dbRendaFixa)
{
    public string DBRENDAFIXA { get; } = dbRendaFixa;
}