using Dapper;
using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Feriado;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Feriado.BancoDeDados.Manipula;

namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Feriado.BancoDeDados.Manipula;

public sealed class ServicoQueManipulaFeriadoNacional(ISqlConnectionFactory _sqlConnectionFactory) : IServicoQueManipulaFeriadoNacional
{
    public async Task AtualizaAsync(FeriadoNacional feriadoNacional, CancellationToken cancellationToken)
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
            feriadoNacional.TxUsuario
        };

        try
        {
            using var conn = _sqlConnectionFactory.CreateConnection();
            await conn.OpenAsync(cancellationToken);

            await conn.ExecuteAsync(new CommandDefinition(sql, listaDeParametro, cancellationToken: cancellationToken));
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new DataBaseException("Erro ao atualizar feriado nacional.", ex);
        }
    }
}
