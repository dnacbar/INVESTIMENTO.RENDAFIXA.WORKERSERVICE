using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Consulta;
using System.Data;
using Dapper;

namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Financeiro.BancoDeDados.Consulta;

public class ServicoQueListaInvestimentoSemBloqueio(IDbConnection dbConnection) : IServicoQueListaInvestimentoSemBloqueio
{
    private readonly IDbConnection _dbConnection = dbConnection;

    public Task<IEnumerable<Investimento>> ListaInvestimento()
    {
        var sql = @"USE DBRENDAFIXA

                    SELECT I.[ID_INVESTIMENTO]
                          ,[ID_INVESTIDOR]
                          ,[TX_DOCUMENTOFEDERAL]
                          ,[NM_VALORINICIAL]
                          ,[NM_VALORFINAL]
                          ,[NM_VALORIMPOSTO]
                          ,[NM_TAXARENDIMENTO]
                          ,[NM_TAXAADICIONAL]
                          ,[DT_INICIAL]
                          ,[DT_FINAL]
                          ,[ID_INDEXADOR]
                          ,[BO_LIQUIDADO]
                          ,[BO_ISENTOIMPOSTO]
                    	  ,[ID_POSICAO]
                          ,[DT_POSICAO]
                          ,[NM_VALORBRUTOTOTAL]
                          ,[NM_VALORLIQUIDOTOTAL]
                          ,[NM_VALORBRUTO]
                          ,[NM_VALORLIQUIDO]
                      FROM [INVESTIMENTO] I
                      LEFT JOIN [POSICAO] P
                        ON I.ID_INVESTIMENTO = P.ID_INVESTIMENTO
                     WHERE CAST(GETDATE() AS DATE) <= I.DT_FINAL
                       AND (CAST(GETDATE() AS DATE) > P.DT_POSICAO OR P.DT_POSICAO IS NULL)";

        return Task.Factory.StartNew(ListaDeInvestimento, sql);
    }

    public IEnumerable<Investimento> ListaDeInvestimento(object sql)
    {
        using var dReader = _dbConnection.ExecuteReader(sql.ToString());

        while (dReader.Read())
        {
            yield return new Investimento(new Posicao(Guid.Parse(dReader["ID_INVESTIMENTO"].ToString()),
                                                      Convert.ToInt16(dReader["ID_POSICAO"]),
                                                      Convert.ToDateTime(dReader["DT_POSICAO"]),
                                                      Convert.ToDecimal(dReader["NM_VALORBRUTOTOTAL"]),
                                                      Convert.ToDecimal(dReader["NM_VALORLIQUIDOTOTAL"]),
                                                      Convert.ToDecimal(dReader["NM_VALORBRUTO"]),
                                                      Convert.ToDecimal(dReader["NM_VALORLIQUIDO"])),
                    Guid.Parse(dReader["ID_INVESTIDOR"].ToString()),
                    dReader["TX_DOCUMENTOFEDERAL"].ToString(),
                    Convert.ToDecimal(dReader["NM_VALORINICIAL"]),
                    Convert.ToDecimal(dReader["NM_VALORFINAL"]),
                    Convert.ToDecimal(dReader["NM_VALORIMPOSTO"]),
                    Convert.ToDecimal(dReader["NM_TAXARENDIMENTO"]),
                    Convert.ToDecimal(dReader["NM_TAXAADICIONAL"]),
                    Convert.ToDateTime(dReader["DT_INICIAL"]),
                    Convert.ToDateTime(dReader["DT_FINAL"]),
                    Convert.ToByte(dReader["ID_INDEXADOR"]),
                    Convert.ToBoolean(dReader["BO_LIQUIDADO"]),
                    Convert.ToBoolean(dReader["BO_ISENTOIMPOSTO"]));
        }
    }
}