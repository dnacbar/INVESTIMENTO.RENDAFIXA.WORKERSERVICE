using System.Data.SqlClient;

namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE;

public interface ISqlConnectionFactory
{
    SqlConnection CreateConnection();
}
