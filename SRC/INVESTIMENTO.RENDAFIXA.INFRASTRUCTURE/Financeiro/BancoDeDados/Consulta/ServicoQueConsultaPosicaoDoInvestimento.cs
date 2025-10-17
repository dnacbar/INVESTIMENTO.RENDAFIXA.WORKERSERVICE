using Dapper;
using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using System.Data;
using System.Data.Common;

namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Consulta;

public class ServicoQueConsultaPosicaoDoInvestimento(IDbConnection _dbConnection) : IServicoQueConsultaPosicaoDoInvestimento
{
    public async Task<Posicao> ObtemPosicaoDoInvestimentoParaCalculoDePosicaoAsync(Investimento investimento, CancellationToken token)
    {
        const string sql = @"SELECT P.[ID_POSICAO]
                          ,P.[NM_VALORBRUTOTOTAL]
                          ,P.[NM_VALORLIQUIDOTOTAL]
                          ,P.[NM_VALORBRUTO]
                          ,P.[NM_VALORLIQUIDO]
                      FROM [POSICAO] P WITH(NOLOCK)
                      JOIN [INVESTIMENTO] I WITH(NOLOCK)
                        ON P.ID_INVESTIMENTO = I.ID_INVESTIMENTO
                     WHERE P.ID_INVESTIMENTO = @IdInvestimento
                       AND P.[DT_POSICAO] <= CAST(GETDATE() AS DATE)
                       AND P.[ID_POSICAO] = (SELECT MAX([ID_POSICAO]) FROM [POSICAO] WHERE [ID_INVESTIMENTO] = @IdInvestimento)";
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
    }
}
