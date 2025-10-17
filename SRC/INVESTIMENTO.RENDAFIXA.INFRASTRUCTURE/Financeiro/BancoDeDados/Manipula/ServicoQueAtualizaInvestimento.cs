using Dapper;
using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Configuracao;
using System.Data;

namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Manipula;

public class ServicoQueAtualizaInvestimento(IDbConnection _dbConnection, IInvestimentoRendaFixaWorkerService _usuarioInvestimentoRendaFixaCronJob) : IServicoQueAtualizaInvestimento
{
    public Task AtualizaInvestimentoComRendimentoDaPosicaoAsync(Investimento investimento, CancellationToken token)
    {
        const string sql = @"UPDATE [INVESTIMENTO]
                                SET [NM_VALORFINAL] = @NmValorFinal
                                   ,[NM_VALORIMPOSTO] = @NmValorImposto
                                   ,[BO_LIQUIDADO] = @BoLiquidado
                                   ,[TX_USUARIOATUALIZACAO] = @Usuario
                                   ,[DT_ATUALIZACAO] = GETDATE()
                              WHERE [ID_INVESTIMENTO] = @IdInvestimento
                                AND [CD_INVESTIMENTO] = @CdInvestimento
                                AND [ID_INVESTIDOR] = @IdInvestidor";

        var listaDeParametro = new
        {
            investimento.IdInvestimento,
            investimento.CdInvestimento,
            investimento.IdInvestidor,
            investimento.NmValorFinal,
            investimento.NmValorImposto,
            investimento.BoLiquidado,
            _usuarioInvestimentoRendaFixaCronJob.Usuario
        };

        try
        {
            return _dbConnection.ExecuteAsync(new CommandDefinition(sql, listaDeParametro, cancellationToken: token));
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new DataBaseException($"Erro ao atualizar o investimento: [{investimento.IdInvestimento}]!", ex);
        }
    }
}