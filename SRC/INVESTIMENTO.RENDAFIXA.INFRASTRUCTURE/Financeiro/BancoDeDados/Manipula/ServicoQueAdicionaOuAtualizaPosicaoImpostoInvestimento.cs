using Dapper;
using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Manipula;
using System.Data;

namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Financeiro.BancoDeDados.Manipula;

public class ServicoQueAdicionaOuAtualizaPosicaoImpostoInvestimento(IDbConnection _dbConnection) : IServicoQueAdicionaOuAtualizaPosicaoImpostoInvestimento
{
    public Task AdicionaPosicaoImpostoInvestimentoAsync(ImpostoPosicao posicaoImposto, CancellationToken token)
    {
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

    public void AdicionaPosicaoImpostoInvestimento(string sql, ImpostoPosicao posicaoImposto)
    {
        if (_dbConnection.State != ConnectionState.Open)
            _dbConnection.Open();

        try
        {
            _dbConnection.Execute(sql, new
            {
                posicaoImposto.IdInvestimento,
                posicaoImposto.IdPosicao,
                IdImposto = (byte)posicaoImposto.IdTipoImposto,
                posicaoImposto.NmValorImposto
            });
        }
        catch (Exception ex)
        {
            throw new DataBaseException($"Erro ao adicionar a posição do imposto do investimento: [{posicaoImposto.IdInvestimento}]!", ex);
        }
        finally
        {
            if (_dbConnection.State == ConnectionState.Open)
                _dbConnection.Close();
        }
    }
}
