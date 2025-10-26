using Dapper;
using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Manipula;

namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Financeiro.BancoDeDados.Manipula;

public sealed class ServicoQueManipulaInvestimento(ISqlConnectionFactory _sqlConnectionFactory) : IServicoQueManipulaInvestimento
{
    public async Task AtualizaInvestimentoComRendimentoDaPosicaoAsync(Investimento investimento, CancellationToken token)
    {
        const string sql = @"UPDATE [INVESTIMENTO]
                                SET [NM_VALORFINAL] = @NmValorFinal
                                   ,[NM_VALORIMPOSTO] = @NmValorImposto
                                   ,[BO_LIQUIDADO] = @BoLiquidado
                                   ,[TX_USUARIOATUALIZACAO] = @TxUsuario
                                   ,[DT_ATUALIZACAO] = GETDATE()
                              WHERE [ID_INVESTIMENTO] = @IdInvestimento
                                AND [CD_INVESTIMENTO] = @CdInvestimento";

        var listaDeParametro = new
        {
            investimento.IdInvestimento,
            investimento.CdInvestimento,
            investimento.IdInvestidor,
            investimento.NmValorFinal,
            investimento.NmValorImposto,
            investimento.BoLiquidado,
            investimento.TxUsuario,
        };

        try
        {
            using var conn = _sqlConnectionFactory.CreateConnection();
            await conn.OpenAsync(token);
            await conn.ExecuteAsync(new CommandDefinition(sql, listaDeParametro, cancellationToken: token));
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new DataBaseException($"Erro ao atualizar o investimento! Investimento: [{investimento.IdInvestimento}] código investimento: [{investimento.CdInvestimento}]", ex);
        }
    }

    public async Task AtualizaInvestimentoLiquidadoPelaDataAsync(Investimento investimento, CancellationToken token)
    {
        const string sql = @"UPDATE [INVESTIMENTO]
                                SET [BO_LIQUIDADO] = @BoLiquidado
                                   ,[TX_USUARIOATUALIZACAO] = @TxUsuario
                                   ,[DT_ATUALIZACAO] = GETDATE()
                              WHERE [ID_INVESTIMENTO] = @IdInvestimento
                                AND [CD_INVESTIMENTO] = @CdInvestimento";

        var listaDeParametro = new
        {
            investimento.IdInvestimento,
            investimento.CdInvestimento,
            investimento.BoLiquidado,
            investimento.TxUsuario
        };

        try
        {
            using var conn = _sqlConnectionFactory.CreateConnection();
            await conn.OpenAsync(token);
            await conn.ExecuteAsync(new CommandDefinition(sql, listaDeParametro, cancellationToken: token));
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new DataBaseException($"Erro ao atualizar o investimento liquidado pela data! Investimento: [{investimento.IdInvestimento}] código investimento: [{investimento.CdInvestimento}]", ex);
        }
    }
}