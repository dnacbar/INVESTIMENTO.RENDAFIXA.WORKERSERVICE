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
        var sql = @"USE DBRENDAFIXA

                    UPDATE [INVESTIMENTO]
                       SET [NM_VALORFINAL] = @NmValorFinal
                          ,[NM_VALORIMPOSTO] = @NmValorImposto
                          ,[BO_LIQUIDADO] = @BoLiquidado
                          ,[TX_USUARIOATUALIZACAO] = @Usuario
                          ,[DT_ATUALIZACAO] = GETDATE()
                     WHERE [ID_INVESTIMENTO] = @IdInvestimento
                       AND [ID_INVESTIDOR] = @IdInvestidor";

        return Task.Run(() => AtualizaInvestimento(sql, investimento), token);
    }

    private void AtualizaInvestimento(string sql, Investimento investimento)
    {
        if (_dbConnection.State != ConnectionState.Open)
            _dbConnection.Open();

        try
        {
            _dbConnection.Execute(sql, new
            {
                investimento.IdInvestimento,
                investimento.NmValorFinal,
                investimento.NmValorImposto,
                investimento.BoLiquidado,
                investimento.IdInvestidor,
                _usuarioInvestimentoRendaFixaCronJob.Usuario
            });
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new DataBaseException($"Erro ao atualizar o investimento: [{investimento.IdInvestimento}]!", ex);
        }
        finally
        {
            if (_dbConnection.State == ConnectionState.Open)
                _dbConnection.Close();
        }
    }
}