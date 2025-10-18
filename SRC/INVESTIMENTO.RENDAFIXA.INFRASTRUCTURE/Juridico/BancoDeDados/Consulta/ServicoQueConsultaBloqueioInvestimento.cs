using Dapper;
using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Juridico;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Juridico.BancoDeDados.Consulta;
using System.Data;
using System.Data.Common;

namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Juridico.BancoDeDados.Consulta;

public class ServicoQueConsultaBloqueioInvestimento(IDbConnection _dbConnection) : IServicoQueConsultaBloqueioInvestimento
{
    public async Task<decimal> ObtemValorBloqueadoTotalAsync(BloqueioInvestimento bloqueioInvestimento, CancellationToken cancellationToken)
    {
        const string sql = @"IF EXISTS(SELECT 1 FROM [dbo].[BLOQUEIOINVESTIMENTO] WITH (NOLOCK) WHERE [ID_INVESTIMENTO] = @IdInvestimento)
                                SELECT SUM([NM_VALORBLOQUEADO]) AS NM_VALORBLOQUEADO
                                  FROM [dbo].[BLOQUEIOINVESTIMENTO] WITH (NOLOCK)
                                 WHERE [ID_INVESTIMENTO] = @IdInvestimento
	                         ELSE
		                        SELECT 0.0 AS NM_VALORBLOQUEADO";

        try
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            using var reader = (DbDataReader)await _dbConnection.ExecuteReaderAsync(new CommandDefinition(sql, parameters: new { bloqueioInvestimento.IdInvestimento }, cancellationToken: cancellationToken));

            if (await reader.ReadAsync(cancellationToken))
                return Convert.ToDecimal(reader["NM_VALORBLOQUEADO"]);

            throw new NotFoundException($"Nenhum valor bloqueado total foi encontrado para os parâmetros informados: [{bloqueioInvestimento.IdInvestimento}]");
        }
        catch (Exception ex) when (ex is not NotFoundException && ex is not OperationCanceledException)
        {
            throw new DataBaseException($"Erro ao consultar valor bloqueado total de investimento: [{bloqueioInvestimento.IdInvestimento}]!", ex);
        }
        finally
        {
            if (_dbConnection.State == ConnectionState.Open)
                _dbConnection.Close();
        }
    }
}
