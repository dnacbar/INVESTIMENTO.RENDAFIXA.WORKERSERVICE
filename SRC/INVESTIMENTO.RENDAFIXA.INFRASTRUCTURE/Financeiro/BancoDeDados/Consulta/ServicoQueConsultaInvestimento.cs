using Dapper;
using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Indice;
using System.Data;
using System.Data.Common;

namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Consulta;

public class ServicoQueConsultaInvestimento(IDbConnection _dbConnection) : IServicoQueConsultaInvestimento
{
    public async Task<List<Investimento>> ListaInvestimentoLiquidadoParaAdicaoDeResgateAsync(CancellationToken token)
    {
        const string sql = @"SELECT I.ID_INVESTIMENTO, 
	                         	    I.CD_INVESTIMENTO,
	                         	    I.NM_VALORFINAL,
	                         	    I.NM_VALORIMPOSTO
	                           FROM INVESTIMENTO I WITH (NOLOCK)
	                           LEFT JOIN RESGATE R WITH (NOLOCK)
	                             ON I.ID_INVESTIMENTO = R.ID_INVESTIMENTO
	                            AND I.CD_INVESTIMENTO = R.ID_RESGATE
	                          WHERE I.BO_LIQUIDADO = CAST(1 AS BIT)
	                            AND R.ID_INVESTIMENTO IS NULL
	                            AND I.CD_INVESTIMENTO = (SELECT MAX(CD_INVESTIMENTO) FROM INVESTIMENTO WHERE ID_INVESTIMENTO = I.ID_INVESTIMENTO)";

        try
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            using var dReader = (DbDataReader)await _dbConnection.ExecuteReaderAsync(new CommandDefinition(sql, cancellationToken: token));

            if (!dReader.HasRows)
                throw new NotFoundException($"Nenhum investimento liquidado foi encontrado para adicionar resgate!");

            var retorno = new List<Investimento>();

            while (await dReader.ReadAsync(token))
                retorno.Add(new Investimento(Guid.Parse(dReader["ID_INVESTIMENTO"].ToString()!),
                       Convert.ToByte(dReader["CD_INVESTIMENTO"]),
                       Convert.ToDecimal(dReader["NM_VALORFINAL"]),
                       Convert.ToDecimal(dReader["NM_VALORIMPOSTO"])));

            return retorno;
        }
        catch (Exception ex) when (ex is not NotFoundException && ex is not OperationCanceledException)
        {
            throw new DataBaseException("Erro ao consultar investimentos liquidados para adicionar resgate!", ex);
        }
    }

    public async Task<List<Investimento>> ListaInvestimentoParaCalculoDePosicaoAsync(CancellationToken token)
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
	                 WHERE I.CD_INVESTIMENTO = (SELECT MAX(CD_INVESTIMENTO) FROM INVESTIMENTO WHERE I.ID_INVESTIMENTO = ID_INVESTIMENTO)
					   AND BO_LIQUIDADO = CAST(0 AS BIT)
					   AND CAST(GETDATE() AS DATE) <= I.DT_FINAL
					   AND P.ID_INVESTIMENTO IS NULL";

        try
        {
            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            using var dReader = (DbDataReader)await _dbConnection.ExecuteReaderAsync(new CommandDefinition(sql, cancellationToken: token));

            if (!dReader.HasRows)
                throw new NotFoundException($"Nenhum investimento foi encontrado para aplicar rendimento diário!");

            var retorno = new List<Investimento>();

            while (await dReader.ReadAsync(token))
                retorno.Add(new Investimento(new Indexador(Convert.ToByte(dReader["ID_INDEXADOR"]),
                                                           Convert.ToDecimal(dReader["NM_RENDIMENTO"])),
                       Guid.Parse(dReader["ID_INVESTIMENTO"].ToString()!),
                       Convert.ToByte(dReader["CD_INVESTIMENTO"]),
                       Guid.Parse(dReader["ID_INVESTIDOR"].ToString()!),
                       dReader["TX_DOCUMENTOFEDERAL"].ToString()!,
                       Convert.ToDecimal(dReader["NM_VALORINICIAL"]),
                       Convert.ToDecimal(dReader["NM_VALORFINAL"]),
                       Convert.ToDecimal(dReader["NM_VALORIMPOSTO"]),
                       Convert.ToDecimal(dReader["NM_TAXARENDIMENTO"]),
                       Convert.ToDecimal(dReader["NM_TAXAADICIONAL"]),
                       Convert.ToDateTime(dReader["DT_INICIAL"]),
                       Convert.ToDateTime(dReader["DT_FINAL"]),
                       Convert.ToBoolean(dReader["BO_LIQUIDADO"]),
                       Convert.ToBoolean(dReader["BO_ISENTOIMPOSTO"])));

            return retorno;
        }
        catch (Exception ex) when (ex is not NotFoundException && ex is not OperationCanceledException)
        {
            throw new DataBaseException("Erro ao consultar investimentos para aplicar rendimento diário!", ex);
        }
    }
}