using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Consulta;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Manipula;
using Microsoft.Extensions.Logging;

namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.Servico;

public sealed class AtualizaOInvestimentoLiquidadoPelaData(ILogger<AtualizaOInvestimentoLiquidadoPelaData> _logger,
    IServicoQueConsultaInvestimento _servicoQueConsultaInvestimento,
    IServicoQueManipulaInvestimento _servicoQueManipulaInvestimento)
{
    public async Task ExecutaAsync(CancellationToken token)
    {
        _logger.LogInformation("Iniciando liquidação pela data - {horario}.", [DateTimeOffset.Now.ToLocalTime()]);

        var listaDeInvestimentoQueDeveSerLiquidadoPelaData = await _servicoQueConsultaInvestimento.ListaInvestimentoQueDeveSerLiquidadoPelaDataAsync(token);

        await ProcessaOInvestimentoQueDeveSerLiquidadoPelaDataAsync(listaDeInvestimentoQueDeveSerLiquidadoPelaData, token);

        _logger.LogInformation("Finalizado processamento de {qtdeInvestimento} investimentos liquidados pela data - {horario}.", [listaDeInvestimentoQueDeveSerLiquidadoPelaData.Count(), DateTimeOffset.Now.ToLocalTime()]);
    }

    private async Task ProcessaOInvestimentoQueDeveSerLiquidadoPelaDataAsync(IEnumerable<Investimento> listaDeInvestimentoQueDeveSerLiquidadoPelaData, CancellationToken token)
    { 
        foreach (var investimento in listaDeInvestimentoQueDeveSerLiquidadoPelaData)
        {
            try
            {
                investimento.AtualizaUsuarioAtualizacao();
                await _servicoQueManipulaInvestimento.AtualizaInvestimentoLiquidadoPelaDataAsync(investimento, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar investimento liquidado pela data {idInvestimento}.", investimento.IdInvestimento);
            }
        }
    }
}
