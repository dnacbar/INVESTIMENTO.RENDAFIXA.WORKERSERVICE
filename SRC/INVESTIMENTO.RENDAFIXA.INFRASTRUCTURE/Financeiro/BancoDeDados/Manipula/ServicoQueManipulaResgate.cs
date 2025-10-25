using Dapper;
using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Configuracao;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Manipula;

namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Financeiro.BancoDeDados.Manipula;

public sealed class ServicoQueManipulaResgate(IInvestimentoRendaFixaWorkerService _investimentoRendaFixaWorkerService,
    ISqlConnectionFactory _sqlConnectionFactory) : IServicoQueManipulaResgate
{
    public async Task AdicionaAsync(Resgate resgate, CancellationToken cancellationToken)
    {
        const string sql = @"INSERT INTO [dbo].[RESGATE]
                            ([ID_INVESTIMENTO]
                            ,[ID_RESGATE]
                            ,[NM_VALOR]
                            ,[NM_VALORIMPOSTO]
                            ,[NM_VALORIOF]
                            ,[NM_VALORIRRF]
                            ,[DT_RESGATE]
                            ,[TX_USUARIO]
                            )
                        VALUES
                            (@IdInvestimento
                            ,@IdResgate
                            ,@NmValor
                            ,@NmValorImposto
                            ,0
                            ,@NmValorImposto
                            ,GETDATE()
                            ,@Usuario)";

        var listaDeParametro = new
        {
            resgate.IdInvestimento,
            resgate.IdResgate,
            resgate.NmValor,
            resgate.NmValorImposto,
            _investimentoRendaFixaWorkerService.Usuario
        };

        try
        {
            using var conn = _sqlConnectionFactory.CreateConnection();
            await conn.OpenAsync(cancellationToken);
            await conn.ExecuteAsync(new CommandDefinition(sql, listaDeParametro, cancellationToken: cancellationToken));
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new DataBaseException($"Erro ao adicionar resgate de investimento! Investimento: [{resgate.IdInvestimento}] resgate: [{resgate.IdResgate}]", ex);
        }
    }
}
