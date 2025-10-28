using Dapper;
using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Feriado;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Feriado.BancoDeDados.Consulta;
using INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Configuracao;
using System.Data;

namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Feriado.BancoDeDados.Consulta;

public sealed class ServicoQueConsultaFeriadoNacional(IConfiguracaoInfraWorkerService _sqlConnectionFactory) : IServicoQueConsultaFeriadoNacional
{
    private static readonly SemaphoreSlim _semaphore = new(1);
    
    public async Task<IEnumerable<FeriadoNacional>> ListaAsync(CancellationToken cancellationToken)
    {
        const string sql = @"SELECT [ID_FERIADO] 
                                   ,[DT_FERIADO] 
                                   ,[TX_NOME] 
                               FROM [dbo].[FERIADONACIONAL] WITH (NOLOCK)";

        await _semaphore.WaitAsync(cancellationToken);
        try
        {
            using var conn = _sqlConnectionFactory.CreateConnectionSqlServer();
            await conn.OpenAsync(cancellationToken);

            var listaDynamicFeriadoNacional = await conn.QueryAsync(new CommandDefinition(sql, cancellationToken: cancellationToken));

            if (!listaDynamicFeriadoNacional.Any())
                throw new NotFoundException($"Nenhum feriado nacional foi encontrado!");

            return listaDynamicFeriadoNacional.Select(r =>
            {
                var row = (IDictionary<string, object>)r;
                return new FeriadoNacional(
                    Convert.ToInt16(row["ID_FERIADO"]),
                    Convert.ToDateTime(row["DT_FERIADO"]),
                    row["TX_NOME"].ToString()!
                );
            });
        }
        catch (Exception ex) when (ex is not NotFoundException && ex is not OperationCanceledException)
        {
            throw new DataBaseException("Erro ao consultar feriados nacionais!", ex);
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
