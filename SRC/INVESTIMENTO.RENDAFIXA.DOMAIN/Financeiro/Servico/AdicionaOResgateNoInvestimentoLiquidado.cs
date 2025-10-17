using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Consulta;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Manipula;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Juridico;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Juridico.BancoDeDados.Consulta;
using Microsoft.Extensions.Logging;

namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.Servico;

public class AdicionaOResgateNoInvestimentoLiquidado(ILogger<AdicionaORendimentoNaPosicaoDeHoje> _logger,
    IServicoQueConsultaInvestimento _servicoQueConsultaInvestimento,
    IServicoQueConsultaBloqueioInvestimento _servicoQueConsultaBloqueioInvestimento,
    IServicoQueManipulaResgate _servicoQueManipulaResgate)
{
    public async Task ExecutaAsync(CancellationToken token)
    {
        _logger.LogInformation("Iniciando adição de resgate em investimentos liquidados - {horario}", [DateTimeOffset.Now.ToLocalTime()]);

        var listaDeInvestimentoLiquidado = await _servicoQueConsultaInvestimento.ListaInvestimentoLiquidadoParaAdicaoDeResgateAsync(token);

        await ProcessaInvestimentoLiquidadoAsync(listaDeInvestimentoLiquidado, token);

        _logger.LogWarning("Finalizado processamento de {qtdeInvestimento} investimentos - {horario}.", [listaDeInvestimentoLiquidado.Count, DateTimeOffset.Now.ToLocalTime()]);
    }

    private async Task ProcessaInvestimentoLiquidadoAsync(List<Investimento> listaDeInvestimento, CancellationToken token)
    {
        var processados = 0;
        var falhas = 0;

        foreach (var investimento in listaDeInvestimento)
        {
            try
            {
                await ProcessaInvestimentoLiquidadoIndividualAsync(investimento, token);
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

    private async Task ProcessaInvestimentoLiquidadoIndividualAsync(Investimento investimento, CancellationToken token)
    {
        var nmValorBloqueadoTotal = await _servicoQueConsultaBloqueioInvestimento.ObtemValorBloqueadoTotalAsync(new BloqueioInvestimento(investimento.IdInvestimento), token);

        var resgate = new Resgate(investimento.IdInvestimento, investimento.CdInvestimento, investimento.NmValorFinal - nmValorBloqueadoTotal, investimento.NmValorImposto);

        await _servicoQueManipulaResgate.AdicionaAsync(resgate, token);
    }
}