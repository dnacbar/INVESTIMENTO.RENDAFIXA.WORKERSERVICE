using Dapper;
using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Manipula;
using INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Configuracao;

namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Financeiro.BancoDeDados.Manipula;

public sealed class ServicoQueManipulaPosicaoInvestimento(IConfiguracaoInfraWorkerService _configuracaoInfraWorkerService) : IServicoQueManipulaPosicaoInvestimento
{
    public async Task AdicionaPosicaoInvestimentoAsync(Posicao posicao, CancellationToken token)
    {
        const string sql = @"INSERT INTO POSICAO 
                          ([ID_INVESTIMENTO]
                          ,[CD_INVESTIMENTO]
                          ,[ID_POSICAO]
                          ,[DT_POSICAO]
                          ,[NM_VALORBRUTOTOTAL]
                          ,[NM_VALORLIQUIDOTOTAL]
                          ,[NM_VALORBRUTO]
                          ,[NM_VALORLIQUIDO]
                          ,[TX_USUARIO])
                   VALUES (@IdInvestimento,
                          @CdInvestimento,
                          @IdPosicao,
                          @DtPosicao,
                          @NmValorBrutoTotal,
                          @NmValorLiquidoTotal,
                          @NmValorBruto,
                          @NmValorLiquido,
                          @TxUsuario)";

        var parametros = new
        {
            posicao.Investimento.IdInvestimento,
            posicao.Investimento.CdInvestimento,
            posicao.IdPosicao,
            posicao.DtPosicao,
            posicao.NmValorBrutoTotal,
            posicao.NmValorLiquidoTotal,
            posicao.NmValorBruto,
            posicao.NmValorLiquido,
            _configuracaoInfraWorkerService.TxUsuario
        };

        try
        {
            using var conn = _configuracaoInfraWorkerService.CreateConnectionSqlServer();
            await conn.OpenAsync(token);
            await conn.ExecuteAsync(new CommandDefinition(sql, parametros, cancellationToken: token));
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new DataBaseException($"Erro ao adicionar a posição do investimento! Investimento: [{posicao.Investimento.IdInvestimento}] código investimento: [{posicao.Investimento.CdInvestimento}]", ex);
        }
    }
}