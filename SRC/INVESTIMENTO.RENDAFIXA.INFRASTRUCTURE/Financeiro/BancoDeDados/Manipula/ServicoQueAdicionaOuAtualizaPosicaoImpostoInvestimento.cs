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
        var listaDeParametro = new
        {
            posicaoImposto.IdInvestimento,
            posicaoImposto.IdPosicao,
            IdImposto = (byte)posicaoImposto.IdTipoImposto,
            posicaoImposto.NmValorImposto
        };

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

        if (_dbConnection.State != ConnectionState.Open)
            _dbConnection.Open();

        try
        {
            return _dbConnection.ExecuteAsync(new CommandDefinition(sql, listaDeParametro, cancellationToken: token));
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
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
