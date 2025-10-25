using Dapper;
using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Imposto;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Imposto.BancoDeDados.Consulta;
using System.Data;

namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Imposto.BancoDeDados.Consulta;

public sealed class ServicoQueConsultaConfiguracaoImposto(ISqlConnectionFactory _sqlConnectionFactory) : IServicoQueConsultaConfiguracaoImposto
{
    public async Task<IEnumerable<ConfiguracaoImposto>> ListaConfiguracaoImpostoAsync(CancellationToken token)
    {
        const string sql = @"SELECT I.[ID_IMPOSTO]
                                   ,[ID_CONFIGURACAOIMPOSTO]
                                   ,[NM_RENDIMENTO]
                                   ,[NM_DIASUTEIS]
                               FROM [IMPOSTO] I
                              INNER JOIN [CONFIGURACAOIMPOSTO] CI
                                 ON I.ID_IMPOSTO = CI.ID_IMPOSTO";

        try
        {
            using var conn = _sqlConnectionFactory.CreateConnection();
            await conn.OpenAsync(token);

            var listaDynamicConfiguracaoImposto = await conn.QueryAsync(new CommandDefinition(sql, cancellationToken: token));

            return listaDynamicConfiguracaoImposto.Select(r =>
            {
                var row = (IDictionary<string, object>)r;
                return new ConfiguracaoImposto(
                    Convert.ToByte(row["ID_IMPOSTO"]),
                    Convert.ToByte(row["ID_CONFIGURACAOIMPOSTO"]),
                    Convert.ToDecimal(row["NM_RENDIMENTO"]),
                    Convert.ToInt16(row["NM_DIASUTEIS"])
                );
            });
        }
        catch (Exception ex) when (ex is not NotFoundException && ex is not OperationCanceledException)
        {
            throw new DataBaseException("Erro ao consultar configuração de impostos para aplicar rendimento diário.", ex);
        }
    }
}