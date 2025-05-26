namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE;

public class ObjetoSql(string sql, object? listaDeParametro = null)
{
    public string Sql { get; set; } = sql;
    public object? ListaDeParametro { get; set; } = listaDeParametro;
}
