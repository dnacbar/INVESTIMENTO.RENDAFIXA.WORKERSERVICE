using INVESTIMENTO.RENDAFIXA.DOMAIN.Feriado.BancoDeDados.Consulta;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Feriado.BancoDeDados.Manipula;
using Microsoft.Extensions.Logging;

namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Feriado.Servico;

public sealed class AtualizaOAnoDaListaDeFeriadoNacional(ILogger<AtualizaOAnoDaListaDeFeriadoNacional> _logger,
    IServicoQueConsultaFeriadoNacional _servicoQueConsultaFeriadoNacional,
    IServicoQueManipulaFeriadoNacional _servicoQueManipulaFeriadoNacional)
{
    public async Task ExecutaAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando atualização de feriados nacionais - {horario}", [DateTimeOffset.Now.ToLocalTime()]);

        var listaDeFeriadoNacional = await _servicoQueConsultaFeriadoNacional.ListaAsync(cancellationToken);

        foreach (var item in listaDeFeriadoNacional)
        {
            item.AtualizaDataFeriado();
            await _servicoQueManipulaFeriadoNacional.AtualizaAsync(item, cancellationToken);
        }

        _logger.LogInformation("Finalizado processamento de {qtdeInvestimento} feriados nacionais - {horario}.", [listaDeFeriadoNacional.Count(), DateTimeOffset.Now.ToLocalTime()]);
    }
}
