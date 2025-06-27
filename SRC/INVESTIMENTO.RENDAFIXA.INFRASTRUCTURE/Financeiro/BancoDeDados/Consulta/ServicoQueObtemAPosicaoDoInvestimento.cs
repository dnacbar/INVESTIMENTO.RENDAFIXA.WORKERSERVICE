using Dapper;
using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Consulta;
using System.Data;
using System.Data.Common;

namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Financeiro.BancoDeDados.Consulta;

public class ServicoQueObtemAPosicaoDoInvestimento(IDbConnection _dbConnection) : IServicoQueObtemAPosicaoDoInvestimento
{
    public async Task<Posicao> ObtemPosicaoDoInvestimentoParaCalculoDePosicaoAsync(Investimento investimento, CancellationToken token)
    {
        var sql = @"USE DBRENDAFIXA

                    SELECT P.[ID_INVESTIMENTO]
                          ,P.[ID_POSICAO]
                          ,P.[NM_VALORBRUTOTOTAL]
                          ,P.[NM_VALORLIQUIDOTOTAL]
                          ,P.[NM_VALORBRUTO]
                          ,P.[NM_VALORLIQUIDO]
                      FROM [POSICAO] P
                      JOIN [INVESTIMENTO] I
                        ON P.ID_INVESTIMENTO = I.ID_INVESTIMENTO
                     WHERE P.ID_INVESTIMENTO = @IdInvestimento
                       AND P.[DT_POSICAO] <= CAST(GETDATE() AS DATE)
                       AND P.[ID_POSICAO] = (SELECT MAX([ID_POSICAO]) FROM [POSICAO] WHERE [ID_INVESTIMENTO] = @IdInvestimento)";

        if (_dbConnection.State != ConnectionState.Open)
            _dbConnection.Open();

        try
        {
            using var dReader = (DbDataReader)await _dbConnection.ExecuteReaderAsync(new CommandDefinition(sql, new { investimento.IdInvestimento }, cancellationToken: token));

            if (await dReader.ReadAsync(token))
            {
                return new Posicao(
                    investimento,
                    Convert.ToInt16(dReader["ID_POSICAO"]),
                    Convert.ToDecimal(dReader["NM_VALORBRUTOTOTAL"]),
                    Convert.ToDecimal(dReader["NM_VALORLIQUIDOTOTAL"]),
                    Convert.ToDecimal(dReader["NM_VALORBRUTO"]),
                    Convert.ToDecimal(dReader["NM_VALORLIQUIDO"]));
            }

            return new Posicao(investimento);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new DataBaseException($"Erro ao consultar a posição do investimento: [{investimento.IdInvestimento}]!", ex);
        }
        finally
        {
            if (_dbConnection.State == ConnectionState.Open)
                _dbConnection.Close();
        }
    }
}
