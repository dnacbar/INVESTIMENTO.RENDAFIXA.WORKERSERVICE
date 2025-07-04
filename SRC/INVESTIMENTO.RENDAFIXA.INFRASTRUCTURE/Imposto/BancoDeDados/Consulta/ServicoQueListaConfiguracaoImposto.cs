﻿    using Dapper;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Imposto;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Imposto.BancoDeDados.Consulta;
using System.Data;
using System.Data.Common;

namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Imposto.BancoDeDados.Consulta;

public class ServicoQueListaConfiguracaoImposto(IDbConnection _dbConnection) : IServicoQueListaConfiguracaoImposto
{
    public async Task<List<ConfiguracaoImposto>> ListaConfiguracaoImpostoAsync(CancellationToken token)
    {
        var sql = @"USE [DBRENDAFIXA]

	                SELECT I.[ID_IMPOSTO]
	                      ,[ID_CONFIGURACAOIMPOSTO]
	                      ,[NM_RENDIMENTO]
	                      ,[NM_DIASUTEIS]
	                  FROM [IMPOSTO] I
	                 INNER JOIN [CONFIGURACAOIMPOSTO] CI
	                    ON I.ID_IMPOSTO = CI.ID_IMPOSTO";

        if (_dbConnection.State != ConnectionState.Open)
            _dbConnection.Open();
        try
        {
            using var dReader = (DbDataReader)await _dbConnection.ExecuteReaderAsync(new CommandDefinition(sql, cancellationToken: token));

            var retorno = new List<ConfiguracaoImposto>();

            while (await dReader.ReadAsync(token))
            {
                retorno.Add(new ConfiguracaoImposto(
                    Convert.ToByte(dReader["ID_IMPOSTO"]),
                    Convert.ToByte(dReader["ID_CONFIGURACAOIMPOSTO"]),
                    Convert.ToDecimal(dReader["NM_RENDIMENTO"]),
                    Convert.ToInt16(dReader["NM_DIASUTEIS"])
                ));
            }

            return retorno;
        }
        finally
        {
            if (_dbConnection.State == ConnectionState.Open)
                _dbConnection.Close();
        }
    }
}