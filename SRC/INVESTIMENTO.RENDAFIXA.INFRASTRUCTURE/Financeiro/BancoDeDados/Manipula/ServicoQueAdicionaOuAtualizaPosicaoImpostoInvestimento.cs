using Dapper;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Manipula;
using System.Data;

namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Financeiro.BancoDeDados.Manipula;

public class ServicoQueAdicionaOuAtualizaPosicaoImpostoInvestimento(IDbConnection _dbConnection) : IServicoQueAdicionaOuAtualizaPosicaoImpostoInvestimento
{
    public Task AdicionaPosicaoImpostoInvestimentoAsync(PosicaoImposto posicaoImposto, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        var sql = @"USE DBRENDAFIXA

                    INSERT POSICAOIMPOSTO
                          ([ID_INVESTIMENTO]
                           ,[ID_POSICAO]
                           ,[ID_IMPOSTO]
                           ,[NM_VALORIMPOSTO])
                    VALUES (@IdInvestimento,
                           @IdPosicao,
                           @IdImposto,
                           @NmValorImposto);";

        return Task.Run(() => AdicionaPosicaoImpostoInvestimento(sql, posicaoImposto), token);
    }

    public void AdicionaPosicaoImpostoInvestimento(string sql, PosicaoImposto posicaoImposto)
    {
        if (_dbConnection.State != ConnectionState.Open)
            _dbConnection.Open();

        try
        {
            _dbConnection.Execute(sql, new
            {
                posicaoImposto.IdInvestimento,
                posicaoImposto.IdPosicao,
                IdImposto = (byte)posicaoImposto.IdImposto,
                posicaoImposto.NmValorImposto
            });
        }
        finally
        {
            if (_dbConnection.State == ConnectionState.Open)
                _dbConnection.Close();
        }
    }
}
