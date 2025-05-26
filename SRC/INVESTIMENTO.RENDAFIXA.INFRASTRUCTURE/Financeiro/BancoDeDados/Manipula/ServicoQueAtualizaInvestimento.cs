using Dapper;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Manipula;
using System.Data;

namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Financeiro.BancoDeDados.Manipula;

public class ServicoQueAtualizaInvestimento(IDbConnection _dbConnection) : IServicoQueAtualizaInvestimento
{
    public Task AtualizaInvestimentoComRendimentoDaPosicaoAsync(Investimento investimento, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        var sql = @"USE DBRENDAFIXA

                    UPDATE [INVESTIMENTO]
                       SET [NM_VALORFINAL] = @NmValorFinal
                          ,[NM_VALORIMPOSTO] = @NmValorImposto
                          ,[TX_USUARIOATUALIZACAO] = @TxUsuarioAtualizacao
                          ,[DT_ATUALIZACAO] = @DtAtualizacao
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
                investimento.IdInvestidor,
                investimento.TxUsuarioAtualizacao,
                investimento.DtAtualizacao
            });
        }
        finally
        {
            if (_dbConnection.State == ConnectionState.Open)
                _dbConnection.Close();
        }
    }
}