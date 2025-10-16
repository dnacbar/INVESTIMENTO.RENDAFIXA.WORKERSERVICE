namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Financeiro.BancoDeDados.Consulta;

public class ServicoQueConsultaImpostoDaPosicaoDoInvestimento() : IServicoQueConsultaImpostoDaPosicaoDoInvestimento
{
    public Task<ImpostoPosicao> ObtemImpostoDaPosicaoDoInvestimentoAsync(CancellationToken token)
    {
        throw new NotImplementedException();
        //var sql = @"USE DBRENDAFIXA
        //
        //           SELECT PI.[ID_INVESTIMENTO]
        //                  ,PI.[ID_POSICAO]
        //                  ,I.[TX_NOME]
        //                  ,I.[ID_IMPOSTO]
        //                  ,PI.[NM_VALORIMPOSTO]
        //           FROM [POSICAOIMPOSTO] PI
        //           JOIN [IMPOSTO] I ON PI.ID_IMPOSTO = I.ID_IMPOSTO
        //           WHERE CAST(PI.[DT_POSICAO] AS DATE) = CAST(GETDATE() AS DATE)";
        //
        //return Task.Run(() => ConsultaPosicaoImposto(sql), token);
    }

    private ImpostoPosicao ConsultaPosicaoImposto(string sql)
    {
        throw new NotImplementedException();
        //if (_dbConnection.State != ConnectionState.Open)
        //    _dbConnection.Open();
        //
        //try
        //{
        //    using var dReader = _dbConnection.ExecuteReader(sql);
        //
        //    if (dReader.Read())
        //        return new PosicaoImposto(
        //            Guid.Parse(dReader["ID_INVESTIMENTO"].ToString()!),
        //            Convert.ToInt16(dReader["ID_POSICAO"]),
        //            dReader["TX_NOME"]?.ToString() ?? string.Empty,
        //            (EnumTipoImposto)Convert.ToByte(dReader["ID_IMPOSTO"]),
        //            Convert.ToDecimal(dReader["NM_VALORIMPOSTO"]));
        //
        //    throw new NotFoundException("Nenhum imposto encontrado para a posição na data atual.");
        //}
        //catch (Exception ex) when (ex is NotFoundException)
        //{
        //    throw new DataBaseException($"Erro ao consultar o imposto da posição do investimento: [{inve}]", ex);
        //}
        //finally
        //{
        //    if (_dbConnection.State == ConnectionState.Open)
        //        _dbConnection.Close();
        //}
    }
}
