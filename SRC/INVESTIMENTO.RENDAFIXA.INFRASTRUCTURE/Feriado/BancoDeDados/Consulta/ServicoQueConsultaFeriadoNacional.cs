using Dapper;
using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Feriado;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Feriado.BancoDeDados.Consulta;
using System.Data;
using System.Data.Common;

namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Feriado.BancoDeDados.Consulta;

public class ServicoQueConsultaFeriadoNacional(IDbConnection _dbConnection) : IServicoQueConsultaFeriadoNacional
{
    public async Task<List<FeriadoNacional>> ListaAsync(CancellationToken cancellationToken)
    {
        const string sql = @"SELECT [ID_FERIADO]
                                   ,[DT_FERIADO]
                                   ,[TX_NOME]
                               FROM [dbo].[FERIADONACIONAL] WITH (NOLOCK)";

        try
        {
            using var dReader = (DbDataReader)await _dbConnection.ExecuteReaderAsync(new CommandDefinition(sql, cancellationToken: cancellationToken));

            if (!dReader.HasRows)
                throw new NotFoundException($"Nenhum feriado nacional foi encontrado!");

            var retorno = new List<FeriadoNacional>();

            while (await dReader.ReadAsync(cancellationToken))
                retorno.Add(new FeriadoNacional(
                       Convert.ToByte(dReader["ID_FERIADO"]),
                       Convert.ToDateTime(dReader["DT_FERIADO"]),
                       dReader["TX_NOME"].ToString()!));

            return retorno;
        }
        catch (Exception ex) when (ex is not NotFoundException && ex is not OperationCanceledException)
        {
            throw new DataBaseException("Erro ao consultar feriados nacionais!", ex);
        }
    }
}
