using Dapper;
using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Configuracao;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Manipula;

namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Financeiro.BancoDeDados.Manipula;

public sealed class ServicoQueManipulaPosicaoInvestimento(IInvestimentoRendaFixaWorkerService _usuarioInvestimentoRendaFixaCronJob,
    ISqlConnectionFactory _sqlConnectionFactory) : IServicoQueManipulaPosicaoInvestimento
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
                          @Usuario)";

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
            _usuarioInvestimentoRendaFixaCronJob.Usuario
        };

        try
        {
            using var conn = _sqlConnectionFactory.CreateConnection();
            await conn.OpenAsync(token);
            await conn.ExecuteAsync(new CommandDefinition(sql, parametros, cancellationToken: token));
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new DataBaseException($"Erro ao adicionar a posição do investimento! Investimento: [{posicao.Investimento.IdInvestimento}] código investimento: [{posicao.Investimento.CdInvestimento}]", ex);
        }
    }
}