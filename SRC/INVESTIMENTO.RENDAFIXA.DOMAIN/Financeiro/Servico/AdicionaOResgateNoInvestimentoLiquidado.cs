using INVESTIMENTO.RENDAFIXA.DOMAIN.Feriado;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Feriado.BancoDeDados.Consulta;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Consulta;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Manipula;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Juridico;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Juridico.BancoDeDados.Consulta;
using Microsoft.Extensions.Logging;

namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.Servico;

public sealed class AdicionaOResgateNoInvestimentoLiquidado(ILogger<AdicionaORendimentoNaPosicaoDeHoje> _logger,
    IServicoQueConsultaBloqueioInvestimento _servicoQueConsultaBloqueioInvestimento,
    IServicoQueConsultaFeriadoNacional _servicoQueConsultaFeriadoNacional,
    IServicoQueConsultaInvestimento _servicoQueConsultaInvestimento,
    IServicoQueManipulaResgate _servicoQueManipulaResgate)
{
    public async Task ExecutaAsync(CancellationToken token)
    {
        var listaDeFeriadoNacional = await _servicoQueConsultaFeriadoNacional.ListaAsync(token);

        if (listaDeFeriadoNacional.VerificaSeDataAtualEstaNaListaDeFeriadoNacional())
        {
            _logger.LogInformation("Hoje é feriado nacional. Não será aplicado resgate de investimento liquidado - {horario}.", [DateTimeOffset.Now.ToLocalTime()]);
            return;
        }

        await ProcessaOResgateDoInvestimentoLiquidadoAsync(token);
    }

    private async Task ProcessaOResgateDoInvestimentoLiquidadoAsync(CancellationToken token)
    {
        _logger.LogInformation("Iniciando adição de resgate em investimentos liquidados - {horario}", [DateTimeOffset.Now.ToLocalTime()]);

        var listaDeInvestimentoLiquidado = await _servicoQueConsultaInvestimento.ListaInvestimentoLiquidadoParaAdicaoDeResgateAsync(token);

        var processados = 0;
        var falhas = 0;

        foreach (var investimento in listaDeInvestimentoLiquidado)
        {
            try
            {
                await ProcessaOResgateDoInvestimentoLiquidadoIndividualAsync(investimento, token);
                processados++;
            }
            catch (Exception ex)
            {
                falhas++;
                _logger.LogError(ex, "Erro ao processar investimento liquidado {idInvestimento}.", investimento.IdInvestimento);
            }
        }

        _logger.LogInformation("Processados {processados} de {total} investimentos liquidados.", [processados, listaDeInvestimentoLiquidado.Count()]);

        if (falhas > decimal.Zero)
            _logger.LogError("Processamento concluído com {falhas} falhas de {total} investimentos liquidados.", [falhas, listaDeInvestimentoLiquidado.Count()]);

        _logger.LogInformation("Finalizado processamento de {qtdeInvestimento} investimentos liquidados - {horario}.", [listaDeInvestimentoLiquidado.Count(), DateTimeOffset.Now.ToLocalTime()]);
    }

    private async Task ProcessaOResgateDoInvestimentoLiquidadoIndividualAsync(Investimento investimento, CancellationToken token)
    {
        var nmValorBloqueadoTotal = await _servicoQueConsultaBloqueioInvestimento.ObtemValorBloqueadoTotalAsync(new BloqueioInvestimento(investimento.IdInvestimento), token);

        var resgate = new Resgate(investimento.IdInvestimento, investimento.CdInvestimento, investimento.NmValorFinal - nmValorBloqueadoTotal, investimento.NmValorImposto);

        await _servicoQueManipulaResgate.AdicionaAsync(resgate, token);
    }
}