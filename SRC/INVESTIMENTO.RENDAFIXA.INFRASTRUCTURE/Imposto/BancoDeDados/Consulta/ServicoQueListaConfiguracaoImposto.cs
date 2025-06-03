using Dapper;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Imposto;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Imposto.BancoDeDados.Consulta;
using System.Data;

namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Imposto.BancoDeDados.Consulta;

public class ServicoQueListaConfiguracaoImposto(IDbConnection _dbConnection) : IServicoQueListaConfiguracaoImposto
{
    public Task<List<ConfiguracaoImposto>> ListaConfiguracaoImpostoAsync(CancellationToken token)
    {
        var sql = @"USE [DBRENDAFIXA]

	                SELECT I.[ID_IMPOSTO]
	                      ,[ID_CONFIGURACAOIMPOSTO]
	                      ,[NM_RENDIMENTO]
	                      ,[NM_DIASUTEIS]
	                  FROM [IMPOSTO] I
	                 INNER JOIN [CONFIGURACAOIMPOSTO] CI
	                    ON I.ID_IMPOSTO = CI.ID_IMPOSTO";

        return Task.Run(() => ConsultaConfiguracaoImposto(sql), token);
    }

    private List<ConfiguracaoImposto> ConsultaConfiguracaoImposto(string sql)
    {
        if (_dbConnection.State != ConnectionState.Open)
            _dbConnection.Open();

        var retorno = new List<ConfiguracaoImposto>();
        try
        {
            using var dReader = _dbConnection.ExecuteReader(sql);

            while (dReader.Read())
                retorno.Add(new ConfiguracaoImposto(Convert.ToByte(dReader["ID_IMPOSTO"]),
                                                     Convert.ToByte(dReader["ID_CONFIGURACAOIMPOSTO"]),
                                                     Convert.ToDecimal(dReader["NM_RENDIMENTO"]),
                                                     Convert.ToInt16(dReader["NM_DIASUTEIS"])));

            return retorno;
        }
        finally
        {
            if (_dbConnection.State == ConnectionState.Open)
                _dbConnection.Close();
        }
    }
}