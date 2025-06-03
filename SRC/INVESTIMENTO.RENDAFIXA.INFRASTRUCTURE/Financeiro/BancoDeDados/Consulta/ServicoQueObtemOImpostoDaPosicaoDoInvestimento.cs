using Dapper;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Consulta;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Imposto.Enum;
using System.Data;

namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Financeiro.BancoDeDados.Consulta;

public class ServicoQueObtemOImpostoDaPosicaoDoInvestimento(IDbConnection _dbConnection) : IServicoQueObtemOImpostoDaPosicaoDoInvestimento
{
    public Task<PosicaoImposto> ObtemImpostoDaPosicaoDoInvestimentoAsync(CancellationToken token)
    {
        var sql = @"USE DBRENDAFIXA

                    SELECT PI.[ID_INVESTIMENTO]
                           ,PI.[ID_POSICAO]
                           ,I.[TX_NOME]
                           ,I.[ID_IMPOSTO]
                           ,PI.[NM_VALORIMPOSTO]
                    FROM [POSICAOIMPOSTO] PI
                    JOIN [IMPOSTO] I ON PI.ID_IMPOSTO = I.ID_IMPOSTO
                    WHERE CAST(PI.[DT_POSICAO] AS DATE) = CAST(GETDATE() AS DATE)";

        return Task.Run(() => ConsultaPosicaoImposto(sql), token);
    }

    private PosicaoImposto ConsultaPosicaoImposto(string sql)
    {
        if (_dbConnection.State != ConnectionState.Open)
            _dbConnection.Open();

        try
        {
            using var dReader = _dbConnection.ExecuteReader(sql);

            if (dReader.Read())
            {
                return new PosicaoImposto(
                    dReader["ID_INVESTIMENTO"] == DBNull.Value ? Guid.Empty : Guid.Parse(dReader["ID_INVESTIMENTO"].ToString()!),
                    Convert.ToInt16(dReader["ID_POSICAO"]),
                    dReader["TX_NOME"]?.ToString() ?? string.Empty,
                    (EnumTipoImposto)Convert.ToByte(dReader["ID_IMPOSTO"]),
                    Convert.ToDecimal(dReader["NM_VALORIMPOSTO"]));
            }

            throw new InvalidOperationException("Nenhum imposto encontrado para a posição na data atual.");
        }
        finally
        {
            if (_dbConnection.State == ConnectionState.Open)
                _dbConnection.Close();
        }
    }
}
