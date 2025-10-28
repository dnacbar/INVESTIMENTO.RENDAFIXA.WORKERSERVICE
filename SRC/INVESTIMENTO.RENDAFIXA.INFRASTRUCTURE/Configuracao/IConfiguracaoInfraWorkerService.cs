using System.Data.SqlClient;

namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Configuracao;

public interface IConfiguracaoInfraWorkerService
{
    SqlConnection CreateConnectionSqlServer();
    string TxUsuario { get; }
}
