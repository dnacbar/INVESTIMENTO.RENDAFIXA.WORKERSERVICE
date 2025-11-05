using Dapper;
using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Manipula;
using INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Configuracao;

namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Financeiro.BancoDeDados.Manipula;

public sealed class ServicoQueManipulaPosicaoImpostoInvestimento(IConfiguracaoInfraWorkerService _configuracaoInfraWorkerService) : IServicoQueManipulaPosicaoImpostoInvestimento
{
    public async Task AdicionaPosicaoImpostoInvestimentoAsync(PosicaoImposto posicaoImposto, CancellationToken token)
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

        try
        {
            using var conn = _configuracaoInfraWorkerService.CreateConnectionSqlServer();
            await conn.OpenAsync(token);

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

                await conn.ExecuteAsync(new CommandDefinition(sql, listaDeParametro, cancellationToken: token));
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new DataBaseException($"Erro ao adicionar a posição do imposto do investimento! Investimento: [{posicaoImposto.Posicao.Investimento.IdInvestimento}] código investimento: [{posicaoImposto.Posicao.Investimento.CdInvestimento}]", ex);
        }
    }
}
