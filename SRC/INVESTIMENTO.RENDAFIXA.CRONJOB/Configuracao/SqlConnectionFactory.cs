using INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE;
using System.Data.SqlClient;

namespace INVESTIMENTO.RENDAFIXA.CRONJOB.Configuracao;

internal class SqlConnectionFactory(string connectionString) : ISqlConnectionFactory
{
    public SqlConnection CreateConnection() => new(connectionString);
}
