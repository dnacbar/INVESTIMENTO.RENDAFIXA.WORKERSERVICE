using Dapper;
using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Juridico;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Juridico.BancoDeDados.Consulta;
using INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Configuracao;

namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Juridico.BancoDeDados.Consulta;

public sealed class ServicoQueConsultaBloqueioInvestimento(IConfiguracaoInfraWorkerService _sqlConnectionFactory) : IServicoQueConsultaBloqueioInvestimento
{
    public async Task<decimal> ObtemValorBloqueadoTotalAsync(BloqueioInvestimento bloqueioInvestimento, CancellationToken cancellationToken)
    {
        const string sql = @"IF EXISTS(SELECT 1 FROM [dbo].[BLOQUEIOINVESTIMENTO] WITH (NOLOCK) WHERE [ID_INVESTIMENTO] = @IdInvestimento)
                                SELECT SUM([NM_VALORBLOQUEADO]) AS NM_VALORBLOQUEADO
                                  FROM [dbo].[BLOQUEIOINVESTIMENTO] WITH (NOLOCK)
                                 WHERE [ID_INVESTIMENTO] = @IdInvestimento
	                         ELSE
		                        SELECT 0.0 AS NM_VALORBLOQUEADO";

        try
        {
            using var conn = _sqlConnectionFactory.CreateConnectionSqlServer();
            await conn.OpenAsync(cancellationToken);

            var dynamicBloqueadoTotal = await conn.QuerySingleOrDefaultAsync(new CommandDefinition(sql, parameters: new { bloqueioInvestimento.IdInvestimento }, cancellationToken: cancellationToken)) ?? throw new NotFoundException($"Nenhum valor bloqueado total foi encontrado para os parâmetros informados: [{bloqueioInvestimento.IdInvestimento}]");
            
            return Convert.ToDecimal(((IDictionary<string, object>)dynamicBloqueadoTotal)["NM_VALORBLOQUEADO"]);
        }
        catch (Exception ex) when (ex is not NotFoundException && ex is not OperationCanceledException)
        {
            throw new DataBaseException($"Erro ao consultar valor bloqueado total de investimento! Investimento: [{bloqueioInvestimento.IdInvestimento}]", ex);
        }
    }
}
