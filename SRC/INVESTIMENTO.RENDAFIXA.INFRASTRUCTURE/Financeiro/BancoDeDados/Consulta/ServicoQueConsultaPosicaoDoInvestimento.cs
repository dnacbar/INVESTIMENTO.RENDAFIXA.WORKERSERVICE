using Dapper;
using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Consulta;
using System.Data;
using System.Data.Common;

namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Financeiro.BancoDeDados.Consulta;

public sealed class ServicoQueConsultaPosicaoDoInvestimento(IDbConnection _dbConnection) : IServicoQueConsultaPosicaoDoInvestimento
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
                                AND P.CD_INVESTIMENTO = I.CD_INVESTIMENTO
                              WHERE P.ID_INVESTIMENTO = @IdInvestimento
                                AND P.CD_INVESTIMENTO = @CdInvestimento
                                AND P.[DT_POSICAO] <= CAST(GETDATE() AS DATE)
                                AND P.[ID_POSICAO] = (SELECT MAX([ID_POSICAO]) FROM [POSICAO] WHERE [ID_INVESTIMENTO] = @IdInvestimento AND [CD_INVESTIMENTO] = @CdInvestimento)";
        try
        {
            using var dReader = (DbDataReader)await _dbConnection.ExecuteReaderAsync(new CommandDefinition(sql, new { investimento.IdInvestimento, investimento.CdInvestimento }, cancellationToken: token));

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
            throw new DataBaseException($"Erro ao consultar a posição do investimento! Investimento: [{investimento.IdInvestimento}] código investimento: [{investimento.CdInvestimento}]", ex);
        }
    }
}
