using Dapper;
using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Manipula;
using System.Data;

namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Financeiro.BancoDeDados.Manipula;

public class ServicoQueAdicionaOuAtualizaPosicaoImpostoInvestimento(IDbConnection _dbConnection) : IServicoQueAdicionaOuAtualizaPosicaoImpostoInvestimento
{
    public async Task AdicionaPosicaoImpostoInvestimentoAsync(ImpostoPosicao posicaoImposto, CancellationToken token)
    {
        foreach (var item in posicaoImposto.ListaDeImpostoCalculadoPorTipo)
        {
            var listaDeParametro = new
            {
                posicaoImposto.Posicao.IdInvestimento,
                posicaoImposto.Posicao.IdPosicao,
                IdImposto = (int)item.Item1,
                NmValorImposto = item.Item2
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

            try
            {
                await _dbConnection.ExecuteAsync(new CommandDefinition(sql, listaDeParametro, cancellationToken: token));
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                throw new DataBaseException($"Erro ao adicionar a posição do imposto do investimento: [{posicaoImposto.Posicao.IdInvestimento}]!", ex);
            }
        }
    }
}
