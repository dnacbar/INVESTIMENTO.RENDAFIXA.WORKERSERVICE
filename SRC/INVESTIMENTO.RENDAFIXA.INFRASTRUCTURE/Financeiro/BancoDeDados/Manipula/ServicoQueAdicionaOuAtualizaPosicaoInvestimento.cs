﻿using Dapper;
using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Manipula;
using System.Data;

namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Financeiro.BancoDeDados.Manipula;

public class ServicoQueAdicionaOuAtualizaPosicaoInvestimento(IDbConnection _dbConnection, IUsuarioInvestimentoRendaFixaCronJob _usuarioInvestimentoRendaFixaCronJob) : IServicoQueAdicionaOuAtualizaPosicaoInvestimento
{
    public Task AdicionaPosicaoInvestimentoAsync(Posicao posicao, CancellationToken token)
    {
        var parametros = new
        {
            posicao.IdInvestimento,
            posicao.IdPosicao,
            posicao.DtPosicao,
            posicao.NmValorBrutoTotal,
            posicao.NmValorLiquidoTotal,
            posicao.NmValorBruto,
            posicao.NmValorLiquido,
            _usuarioInvestimentoRendaFixaCronJob.Usuario
        };

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

        if (_dbConnection.State != ConnectionState.Open)
            _dbConnection.Open();

        try
        {
            return _dbConnection.ExecuteAsync(new CommandDefinition(sql, parametros, cancellationToken: token));
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new DataBaseException($"Erro ao adicionar a posição do investimento: [{posicao.IdInvestimento}]!", ex);
        }
        finally
        {
            if (_dbConnection.State == ConnectionState.Open)
                _dbConnection.Close();
        }
    }
}