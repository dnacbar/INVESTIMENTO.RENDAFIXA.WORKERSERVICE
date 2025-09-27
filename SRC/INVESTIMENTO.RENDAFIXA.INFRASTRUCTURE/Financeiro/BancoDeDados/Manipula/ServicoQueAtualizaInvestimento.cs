using Dapper;
using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Manipula;
using System.Data;

namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Financeiro.BancoDeDados.Manipula;

public class ServicoQueAtualizaInvestimento(IDbConnection _dbConnection, IUsuarioInvestimentoRendaFixaCronJob _usuarioInvestimentoRendaFixaCronJob) : IServicoQueAtualizaInvestimento
{
    public Task AtualizaInvestimentoComRendimentoDaPosicaoAsync(Investimento investimento, CancellationToken token)
    {
        var parametros = new
        {
            investimento.IdInvestimento,
            investimento.IdInvestidor,
            investimento.NmValorFinal,
            investimento.NmValorImposto,
            investimento.BoLiquidado,
            _usuarioInvestimentoRendaFixaCronJob.Usuario
        };

        var sql = @"USE DBRENDAFIXA

                    UPDATE [INVESTIMENTO]
                       SET [NM_VALORFINAL] = @NmValorFinal
                          ,[NM_VALORIMPOSTO] = @NmValorImposto
                          ,[BO_LIQUIDADO] = @BoLiquidado
                          ,[TX_USUARIOATUALIZACAO] = @Usuario
                          ,[DT_ATUALIZACAO] = GETDATE()
                     WHERE [ID_INVESTIMENTO] = @IdInvestimento
                       AND [ID_INVESTIDOR] = @IdInvestidor";

        try
        {
            return _dbConnection.ExecuteAsync(new CommandDefinition(sql, parametros, cancellationToken: token));
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new DataBaseException($"Erro ao atualizar o investimento: [{investimento.IdInvestimento}]!", ex);
        }
    }
}