using Dapper;
using DN.HELPER.LIBRARY.EXTENSION;
using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Consulta;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Indice;
using System.Data;
using System.Data.Common;

namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Financeiro.BancoDeDados.Consulta;

public class ServicoQueListaInvestimento(IDbConnection _dbConnection) : IServicoQueListaInvestimento
{
    public Task<List<Investimento>> ListaInvestimentoParaCalculoDePosicaoAsync(CancellationToken token)
    {
        var sql = @"USE DBRENDAFIXA

                    SELECT TOP 100
	                	   I.[ID_INVESTIMENTO]
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
	                  FROM [INVESTIMENTO] I
	                  JOIN [INDEXADOR] IX
                        ON I.ID_INDEXADOR = IX.ID_INDEXADOR
	                  LEFT JOIN [POSICAO] P 
	                    ON I.ID_INVESTIMENTO = P.ID_INVESTIMENTO
	                   AND CAST(GETDATE() AS DATE) = P.DT_POSICAO
	                 WHERE CAST(GETDATE() AS DATE) <= I.DT_FINAL 
	                   AND BO_LIQUIDADO = CAST(0 AS BIT)
	                   AND P.ID_INVESTIMENTO IS NULL";

        return Task.Run(() => ConsultaInvestimento(sql), token);
    }

    private List<Investimento> ConsultaInvestimento(string sql)
    {
        if (_dbConnection.State != ConnectionState.Open)
            _dbConnection.Open();

        var retorno = new List<Investimento>();

        try
        {
            using var dReader = (DbDataReader)_dbConnection.ExecuteReader(sql);

            if (!dReader.HasRows)
                throw new NotFoundException($"Nenhum investimento foi encontrado para aplicar rendimento diário!");

            while (dReader.Read())
                retorno.Add(new Investimento(new Indexador(Convert.ToByte(dReader["ID_INDEXADOR"]),
                                                           Convert.ToDecimal(dReader["NM_RENDIMENTO"])),
                       Guid.Parse(dReader["ID_INVESTIMENTO"].ToString()!),
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
        catch (Exception ex) when (ex is not NotFoundException || ex is not OperationCanceledException)
        {
            throw new DataBaseException("Erro ao consultar investimentos para aplicar rendimento diário.", ex);
        }
        finally
        {
            if (_dbConnection.State == ConnectionState.Open)
                _dbConnection.Close();
        }
    }
}