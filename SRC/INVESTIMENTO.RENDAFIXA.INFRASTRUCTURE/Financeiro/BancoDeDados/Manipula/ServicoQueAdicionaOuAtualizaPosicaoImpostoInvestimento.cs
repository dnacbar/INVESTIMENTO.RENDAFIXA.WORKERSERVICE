using Dapper;
using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Financeiro;
using INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Financeiro.BancoDeDados.Manipula;
using INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Configuracao;
using System.Data;

namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Financeiro.BancoDeDados.Manipula;

public class ServicoQueAdicionaOuAtualizaPosicaoImpostoInvestimento(IDbConnection _dbConnection) : IServicoQueAdicionaOuAtualizaPosicaoImpostoInvestimento
{
    public async Task AdicionaPosicaoImpostoInvestimentoAsync(ImpostoPosicao posicaoImposto, CancellationToken token)
    {
        const string sql = @"INSERT POSICAOIMPOSTO
                               ([ID_INVESTIMENTO]
                                ,[CD_INVESTIMENTO]
                                ,[ID_POSICAO]
                                ,[ID_IMPOSTO]
                                ,[NM_VALORIMPOSTO])
                         VALUES (@IdInvestimento,
                                @CdInvestimento,
                                @IdPosicao,
                                @IdImposto,
                                @NmValorImposto)";

        foreach (var item in posicaoImposto.ListaDeImpostoCalculadoPorTipo)
        {
            var listaDeParametro = new
            {
                posicaoImposto.Posicao.Investimento.IdInvestimento,
                posicaoImposto.Posicao.Investimento.CdInvestimento,
                posicaoImposto.Posicao.IdPosicao,
                IdImposto = (int)item.Item1,
                NmValorImposto = item.Item2
            };

            try
            {
                await _dbConnection.ExecuteAsync(new CommandDefinition(sql, listaDeParametro, cancellationToken: token));
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                throw new DataBaseException($"Erro ao adicionar a posição do imposto do investimento: [{posicaoImposto.Posicao.Investimento.IdInvestimento}]!", ex);
            }
        }
    }
}
