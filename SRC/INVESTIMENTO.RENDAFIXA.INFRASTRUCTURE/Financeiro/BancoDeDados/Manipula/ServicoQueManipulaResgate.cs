using Dapper;
using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Configuracao;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Manipula;
using System.Data;

namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Financeiro.BancoDeDados.Manipula;

public class ServicoQueManipulaResgate(IDbConnection _dbConnection, IInvestimentoRendaFixaWorkerService _investimentoRendaFixaWorkerService) : IServicoQueManipulaResgate
{
    public Task AdicionaAsync(Resgate resgate, CancellationToken cancellationToken)
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
                            ,@TxUsuario)";

        var listaDeParametro = new
        {
            resgate.IdInvestimento,
            resgate.IdResgate,
            resgate.NmValor,
            resgate.NmValorImposto,
            TxUsuario = _investimentoRendaFixaWorkerService.Usuario
        };

        try
        {
            return _dbConnection.ExecuteAsync(new CommandDefinition(sql, listaDeParametro, cancellationToken: cancellationToken));
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new DataBaseException("Erro ao adicionar resgate de investimento no banco de dados.", ex);
        }
    }
}
