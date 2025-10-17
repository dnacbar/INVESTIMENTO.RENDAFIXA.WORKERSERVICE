using INVESTIMENTO.RENDAFIXA.DOMAIN.Imposto;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Configuracao;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Consulta;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Manipula;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Imposto.BancoDeDados.Consulta;
using Microsoft.Extensions.Logging;
using System.Transactions;

namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.Servico;

public class AplicaORendimentoNaPosicaoDeHoje(IInvestimentoRendaFixaWorkerService _investimentoRendaFixaWorkerService,
    ILogger<AplicaORendimentoNaPosicaoDeHoje> _logger,
    IServicoQueAdicionaOuAtualizaPosicaoImpostoInvestimento _servicoQueAdicionaOuAtualizaPosicaoImpostoInvestimento,
    IServicoQueAdicionaOuAtualizaPosicaoInvestimento _servicoQueAdicionaOuAtualizaPosicaoInvestimento,
    IServicoQueAtualizaInvestimento _servicoQueAtualizaInvestimentoComRendimento,
    IServicoQueListaConfiguracaoImposto _servicoQueListaConfiguracaoImposto,
    IServicoQueConsultaInvestimento _servicoQueListaInvestimentoSemBloqueio,
    IServicoQueConsultaPosicaoDoInvestimento _servicoQueObtemAPosicaoDoInvestimento)
{
    public async Task ExecutaAsync(CancellationToken token)
    {
        _logger.LogWarning("Iniciando aplicação de rendimento diário - {horario}.", [DateTimeOffset.Now.ToLocalTime()]);

        var listaDeConfiguracaoDoImposto = await _servicoQueListaConfiguracaoImposto.ListaConfiguracaoImpostoAsync(token);
        var listaDeInvestimento = await _servicoQueListaInvestimentoSemBloqueio.ListaInvestimentoParaCalculoDePosicaoAsync(token);

        await ProcessaPosicaoInvestimentoAsync(listaDeInvestimento, listaDeConfiguracaoDoImposto, token);

        _logger.LogWarning("Finalizado processamento de {qtdeInvestimento} investimentos - {horario}.", [listaDeInvestimento.Count, DateTimeOffset.Now.ToLocalTime()]);
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

        await AtualizaInvestimentoEAdicionaPosicaoEImpostoAsync(investimento, posicao, token);
    }

    private async Task AtualizaInvestimentoEAdicionaPosicaoEImpostoAsync(Investimento investimento, Posicao posicao, CancellationToken token)
    {
        using var scope = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 0, _investimentoRendaFixaWorkerService.TempoLimiteTransacion), TransactionScopeAsyncFlowOption.Enabled);

        await _servicoQueAtualizaInvestimentoComRendimento.AtualizaInvestimentoComRendimentoDaPosicaoAsync(investimento, token);

        await _servicoQueAdicionaOuAtualizaPosicaoInvestimento.AdicionaPosicaoInvestimentoAsync(posicao, token);

        await _servicoQueAdicionaOuAtualizaPosicaoImpostoInvestimento.AdicionaPosicaoImpostoInvestimentoAsync(posicao.ImpostoPosicao, token);

        scope.Complete();
    }
}