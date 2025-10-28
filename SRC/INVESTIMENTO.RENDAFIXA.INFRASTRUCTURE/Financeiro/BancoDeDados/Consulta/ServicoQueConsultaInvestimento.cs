using Dapper;
using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Consulta;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Indice;
using INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Configuracao;
using System.Data;

namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Financeiro.BancoDeDados.Consulta;

public sealed class ServicoQueConsultaInvestimento(IConfiguracaoInfraWorkerService sqlConnectionFactory) : IServicoQueConsultaInvestimento
{
    public async Task<IEnumerable<Investimento>> ListaInvestimentoLiquidadoParaAdicaoDeResgateAsync(CancellationToken token)
    {
        const string sql = @"SELECT I.ID_INVESTIMENTO, 
                                           I.CD_INVESTIMENTO,
                                           I.NM_VALORFINAL,
                                           I.NM_VALORIMPOSTO
                                        FROM [INVESTIMENTO] I WITH (NOLOCK)
                                        LEFT JOIN [RESGATE] R WITH (NOLOCK)
                                          ON I.ID_INVESTIMENTO = R.ID_INVESTIMENTO
                                         AND I.CD_INVESTIMENTO = R.ID_RESGATE
                                       WHERE I.BO_LIQUIDADO = CAST(1 AS BIT)
                                         AND R.ID_INVESTIMENTO IS NULL
                                         AND I.CD_INVESTIMENTO = (SELECT MAX(CD_INVESTIMENTO) FROM INVESTIMENTO WITH (NOLOCK) WHERE ID_INVESTIMENTO = I.ID_INVESTIMENTO)";

        try
        {
            using var conn = sqlConnectionFactory.CreateConnectionSqlServer();
            await conn.OpenAsync(token);

            var listaDynamicInvestimento = await conn.QueryAsync(sql, new CommandDefinition(sql, cancellationToken: token));

            if (listaDynamicInvestimento.Any())
                throw new NotFoundException($"Nenhum investimento liquidado foi encontrado para adicionar resgate!");

            return listaDynamicInvestimento.Select(r =>
            {
                var row = (IDictionary<string, object>)r;
                return new Investimento(
                    Guid.Parse(row["ID_INVESTIMENTO"].ToString()!),
                    Convert.ToByte(row["CD_INVESTIMENTO"]),
                    Convert.ToDecimal(row["NM_VALORFINAL"]),
                    Convert.ToDecimal(row["NM_VALORIMPOSTO"])
                );
            });
        }
        catch (Exception ex) when (ex is not NotFoundException && ex is not OperationCanceledException)
        {
            throw new DataBaseException("Erro ao consultar investimentos liquidados para adicionar resgate!", ex);
        }
    }

    public async Task<IEnumerable<Investimento>> ListaInvestimentoParaCalculoDePosicaoAsync(CancellationToken token)
    {
        const string sql = @"SELECT I.[ID_INVESTIMENTO]
                                      ,I.[CD_INVESTIMENTO]
                                      ,[ID_INVESTIDOR]
                                      ,[TX_DOCUMENTOFEDERAL]
                                      ,[NM_VALORINICIAL]
                                      ,[NM_VALORFINAL]  
                                      ,[NM_VALORIMPOSTO]
                                      ,[NM_TAXARENDIMENTO]
                                      ,[NM_TAXAADICIONAL]
                                      ,[DT_INICIAL]
                                      ,[DT_FINAL]
                                      ,IX.[ID_INDEXADOR]
                                      ,IX.[NM_RENDIMENTO]
                                      ,[BO_LIQUIDADO]
                                      ,[BO_ISENTOIMPOSTO]
                                  FROM [INVESTIMENTO] I WITH (NOLOCK)
                                  JOIN [INDEXADOR] IX WITH (NOLOCK)
                                       ON I.ID_INDEXADOR = IX.ID_INDEXADOR
                                  LEFT JOIN [POSICAO] P WITH (NOLOCK)
                                    ON I.ID_INVESTIMENTO = P.ID_INVESTIMENTO
                                   AND I.CD_INVESTIMENTO = P.CD_INVESTIMENTO
                                   AND CAST(GETDATE() AS DATE) = P.DT_POSICAO
                                 WHERE I.CD_INVESTIMENTO = (SELECT MAX(CD_INVESTIMENTO) FROM INVESTIMENTO WITH (NOLOCK) WHERE I.ID_INVESTIMENTO = ID_INVESTIMENTO)
                                   AND BO_LIQUIDADO = CAST(0 AS BIT)
                                   AND CAST(GETDATE() AS DATE) <= I.DT_FINAL
                                   AND P.ID_INVESTIMENTO IS NULL";

        try
        {
            using var conn = sqlConnectionFactory.CreateConnectionSqlServer();
            await conn.OpenAsync(token);

            var listaDynamicInvestimento = await conn.QueryAsync(sql, new CommandDefinition(sql, cancellationToken: token));

            if (!listaDynamicInvestimento.Any())
                throw new NotFoundException($"Nenhum investimento foi encontrado para aplicar rendimento diário!");

            return listaDynamicInvestimento.Select(r =>
            {
                var row = (IDictionary<string, object>)r;
                return new Investimento(
                    new Indexador(
                        Convert.ToByte(row["ID_INDEXADOR"]),
                        Convert.ToDecimal(row["NM_RENDIMENTO"])
                    ),
                    Guid.Parse(row["ID_INVESTIMENTO"].ToString()!),
                    Convert.ToByte(row["CD_INVESTIMENTO"]),
                    Guid.Parse(row["ID_INVESTIDOR"].ToString()!),
                    row["TX_DOCUMENTOFEDERAL"].ToString()!,
                    Convert.ToDecimal(row["NM_VALORINICIAL"]),
                    Convert.ToDecimal(row["NM_VALORFINAL"]),
                    Convert.ToDecimal(row["NM_VALORIMPOSTO"]),
                    Convert.ToDecimal(row["NM_TAXARENDIMENTO"]),
                    Convert.ToDecimal(row["NM_TAXAADICIONAL"]),
                    Convert.ToDateTime(row["DT_INICIAL"]),
                    Convert.ToDateTime(row["DT_FINAL"]),
                    Convert.ToBoolean(row["BO_LIQUIDADO"]),
                    Convert.ToBoolean(row["BO_ISENTOIMPOSTO"])
                );
            });
        }
        catch (Exception ex) when (ex is not NotFoundException && ex is not OperationCanceledException)
        {
            throw new DataBaseException("Erro ao consultar investimentos para aplicar rendimento diário!", ex);
        }
    }

    public async Task<IEnumerable<Investimento>> ListaInvestimentoQueDeveSerLiquidadoPelaDataAsync(CancellationToken token)
    {
        const string sql = @"SELECT I.ID_INVESTIMENTO, 
                                    I.CD_INVESTIMENTO,
                                    I.DT_FINAL
                               FROM [INVESTIMENTO] I WITH (NOLOCK)
                              WHERE I.BO_LIQUIDADO = CAST(0 AS BIT)
                                AND CAST(GETDATE() AS DATE) > I.DT_FINAL";

        try
        {
            using var conn = sqlConnectionFactory.CreateConnectionSqlServer();
            await conn.OpenAsync(token);

            var listaDynamicInvestimento = await conn.QueryAsync(sql, new CommandDefinition(sql, cancellationToken: token));

            return listaDynamicInvestimento.Select(r =>
            {
                var row = (IDictionary<string, object>)r;
                return new Investimento(
                    Guid.Parse(row["ID_INVESTIMENTO"].ToString()!),
                    Convert.ToByte(row["CD_INVESTIMENTO"]),
                    Convert.ToDateTime(row["DT_FINAL"])
                );
            });
        }
        catch (Exception ex) when (ex is not NotFoundException && ex is not OperationCanceledException)
        {
            throw new DataBaseException("Erro ao consultar investimentos liquidados para adicionar resgate!", ex);
        }
    }
}