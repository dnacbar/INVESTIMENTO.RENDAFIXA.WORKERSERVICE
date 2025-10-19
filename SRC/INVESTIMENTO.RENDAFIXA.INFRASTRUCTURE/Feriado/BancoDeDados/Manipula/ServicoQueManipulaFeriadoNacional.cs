using Dapper;
using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Configuracao;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Feriado;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Feriado.BancoDeDados.Manipula;
using System.Data;

namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Feriado.BancoDeDados.Manipula;

public class ServicoQueManipulaFeriadoNacional(IDbConnection _dbConnection, IInvestimentoRendaFixaWorkerService _investimentoRendaFixaWorkerService) : IServicoQueManipulaFeriadoNacional
{
    public Task AtualizaAsync(FeriadoNacional feriadoNacional, CancellationToken cancellationToken)
    {
        const string sql = @"UPDATE [dbo].[FERIADONACIONAL]
                                SET [DT_FERIADO] = @DtFeriado
                                   ,[TX_USUARIOATUALIZACAO] = @TxUsuario
                                   ,[DT_ATUALIZACAO] = GETDATE()
                              WHERE [ID_FERIADO] = @IdFeriado";

        var listaDeParametro = new
        {
            feriadoNacional.IdFeriado,
            feriadoNacional.DtFeriado,
            TxUsuario = _investimentoRendaFixaWorkerService.Usuario
        };

        try
        {
            return _dbConnection.ExecuteAsync(new CommandDefinition(sql, listaDeParametro, cancellationToken: cancellationToken));
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new DataBaseException("Erro ao atualizar feriado nacional.", ex);
        }
    }
}
