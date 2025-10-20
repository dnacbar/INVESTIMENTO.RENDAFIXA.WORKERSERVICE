using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Consulta;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Manipula;
using Microsoft.Extensions.Logging;

namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.Servico;

public class AtualizaOInvestimentoLiquidadoPelaData(ILogger<AtualizaOInvestimentoLiquidadoPelaData> _logger,
    IServicoQueConsultaInvestimento _servicoQueConsultaInvestimento,
    IServicoQueManipulaInvestimento _servicoQueManipulaInvestimento)
{
    public async Task ExecutaAsync(CancellationToken token)
    {
        _logger.LogWarning("Iniciando liquidação de investimento pela data - {horario}.", [DateTimeOffset.Now.ToLocalTime()]);

        var listaDeInvestimentoQueDeveSerLiquidadoPelaData = await _servicoQueConsultaInvestimento.ListaInvestimentoQueDeveSerLiquidadoPelaDataAsync(token);

        await ProcessaOInvestimentoQueDeveSerLiquidadoPelaDataAsync(listaDeInvestimentoQueDeveSerLiquidadoPelaData, token);

        _logger.LogWarning("Finalizado processamento de {qtdeInvestimento} investimentos - {horario}.", [listaDeInvestimentoQueDeveSerLiquidadoPelaData.Count, DateTimeOffset.Now.ToLocalTime()]);
    }

    private async Task ProcessaOInvestimentoQueDeveSerLiquidadoPelaDataAsync(List<Investimento> listaDeInvestimentoQueDeveSerLiquidadoPelaData, CancellationToken token)
    {
        foreach (var investimento in listaDeInvestimentoQueDeveSerLiquidadoPelaData)
        {
            try
            {
                await _servicoQueManipulaInvestimento.AtualizaInvestimentoLiquidadoPelaDataAsync(investimento, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar investimento liquidado {idInvestimento}.", investimento.IdInvestimento);
            }
        }
    }
}
