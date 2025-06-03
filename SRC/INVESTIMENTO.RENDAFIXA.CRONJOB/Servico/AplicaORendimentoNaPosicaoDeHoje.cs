using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Consulta;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Manipula;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Imposto;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Imposto.BancoDeDados.Consulta;
using System.Transactions;

namespace INVESTIMENTO.RENDAFIXA.CRONJOB.Servico;

public class AplicaORendimentoNaPosicaoDeHoje(ILogger<AplicaORendimentoNaPosicaoDeHoje> _logger,
    IServicoQueAdicionaOuAtualizaPosicaoImpostoInvestimento _servicoQueAdicionaOuAtualizaPosicaoImpostoInvestimento,
    IServicoQueAdicionaOuAtualizaPosicaoInvestimento _servicoQueAdicionaOuAtualizaPosicaoInvestimento,
    IServicoQueAtualizaInvestimento _servicoQueAtualizaInvestimentoComRendimento,
    IServicoQueListaConfiguracaoImposto _servicoQueListaConfiguracaoImposto,
    IServicoQueListaInvestimento _servicoQueListaInvestimentoSemBloqueio,
    IServicoQueObtemAPosicaoDoInvestimento _servicoQueObtemAPosicaoDoInvestimento)
{
    public async Task ExecutaAsync(CancellationToken token)
    {
        _logger.LogWarning("Iniciando aplicação de rendimento diário - {horario}.", [DateTimeOffset.Now.ToLocalTime()]);

        var (listaAConfiguracaoDoImposto, listaDeInvestimento) = await ObtemDadosParaProcessamentoAsync(token);

        await ProcessaPosicaoInvestimentoAsync(listaDeInvestimento, listaAConfiguracaoDoImposto, token);

        _logger.LogWarning("Finalizado processamento de {qtdeInvestimento} investimentos - {horario}.", [listaDeInvestimento.Count, DateTimeOffset.Now.ToLocalTime()]);
    }

    private async Task<(List<ConfiguracaoImposto> listaDeImposto, List<Investimento> listaDeInvestimento)> ObtemDadosParaProcessamentoAsync(CancellationToken token)
    {
        var listaDeConfiguracaoImposto = await _servicoQueListaConfiguracaoImposto.ListaConfiguracaoImpostoAsync(token);
        var listaDeInvestimento = await _servicoQueListaInvestimentoSemBloqueio.ListaInvestimentoParaCalculoDePosicaoAsync(token);

        return (listaDeConfiguracaoImposto, listaDeInvestimento);
    }

    private async Task ProcessaPosicaoInvestimentoAsync(List<Investimento> listaDeInvestimento, List<ConfiguracaoImposto> listaDeConfiguracaoImposto, CancellationToken token)
    {
        var processados = 0;
        var falhas = 0;

        foreach (var investimento in listaDeInvestimento)
        {
            try
            {
                await ProcessaInvestimentoIndividualAsync(investimento, listaDeConfiguracaoImposto, token);
                processados++;
            }
            catch (Exception ex)
            {
                falhas++;
                _logger.LogError(ex, "Erro ao processar investimento {idInvestimento}.", investimento.IdInvestimento);
            }
        }

        _logger.LogWarning("Processados {processados} de {total} investimentos.", [processados, listaDeInvestimento.Count]);

        if (falhas > decimal.Zero)
            _logger.LogWarning("Processamento concluído com {falhas} falhas de {total} investimentos.", [falhas, listaDeInvestimento.Count]);
    }

    private async Task ProcessaInvestimentoIndividualAsync(Investimento investimento, List<ConfiguracaoImposto> configuracaoImpostos, CancellationToken token)
    {
        var posicao = await _servicoQueObtemAPosicaoDoInvestimento.ObtemPosicaoDoInvestimentoParaCalculoDePosicaoAsync(investimento, token);

        await posicao.CalculaPosicaoInvestimentoAsync(configuracaoImpostos, token);

        using var scope = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 0, 0, 500), TransactionScopeAsyncFlowOption.Enabled);

        await _servicoQueAtualizaInvestimentoComRendimento.AtualizaInvestimentoComRendimentoDaPosicaoAsync(investimento, token);
        await _servicoQueAdicionaOuAtualizaPosicaoInvestimento.AdicionaPosicaoInvestimentoAsync(posicao, token);

        foreach (var posicaoImposto in posicao.ListaDePosicaoImposto)
            await _servicoQueAdicionaOuAtualizaPosicaoImpostoInvestimento.AdicionaPosicaoImpostoInvestimentoAsync(posicaoImposto, token);

        scope.Complete();
    }
}