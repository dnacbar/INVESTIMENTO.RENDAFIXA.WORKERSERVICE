using Dapper;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Manipula;
using System.Data;

namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Financeiro.BancoDeDados.Manipula;

public class ServicoQueAdicionaOuAtualizaPosicaoInvestimento(IDbConnection _dbConnection, IUsuarioInvestimentoRendaFixaCronJob _usuarioInvestimentoRendaFixaCronJob) : IServicoQueAdicionaOuAtualizaPosicaoInvestimento
{
    public Task AdicionaPosicaoInvestimentoAsync(Posicao posicao, CancellationToken token)
    {
        var sql = @"USE DBRENDAFIXA

                   INSERT INTO POSICAO ([ID_INVESTIMENTO]
                          ,[ID_POSICAO]
                          ,[DT_POSICAO]
                          ,[NM_VALORBRUTOTOTAL]
                          ,[NM_VALORLIQUIDOTOTAL]
                          ,[NM_VALORBRUTO]
                          ,[NM_VALORLIQUIDO]
                          ,[TX_USUARIO]
                          ,[DT_CRIACAO])
                   VALUES (@IdInvestimento,
                          @IdPosicao,
                          @DtPosicao,
                          @NmValorBrutoTotal,
                          @NmValorLiquidoTotal,
                          @NmValorBruto,
                          @NmValorLiquido,
                          @Usuario,
                          GETDATE());";

        return Task.Run(() => AdicionaPosicaoInvestimento(sql, posicao), token);
    }

    private void AdicionaPosicaoInvestimento(string sql, Posicao posicao)
    {
        if (_dbConnection.State != ConnectionState.Open)
            _dbConnection.Open();

        try
        {
            _dbConnection.Execute(sql, new
            {
                posicao.IdInvestimento,
                posicao.IdPosicao,
                posicao.DtPosicao,
                posicao.NmValorBrutoTotal,
                posicao.NmValorLiquidoTotal,
                posicao.NmValorBruto,
                posicao.NmValorLiquido,
                _usuarioInvestimentoRendaFixaCronJob.Usuario
            });
        }
        finally
        {
            if (_dbConnection.State == ConnectionState.Open)
                _dbConnection.Close();
        }
    }
}