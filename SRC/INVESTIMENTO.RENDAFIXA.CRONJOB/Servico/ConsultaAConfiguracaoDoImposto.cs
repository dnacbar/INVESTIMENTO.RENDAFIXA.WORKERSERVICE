using DN.LOG.LIBRARY.MODEL;
using DN.LOG.LIBRARY.MODEL.ENUM;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Imposto;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Imposto.BancoDeDados.Consulta;

namespace INVESTIMENTO.RENDAFIXA.CRONJOB.Servico;

public class ConsultaAConfiguracaoDoImposto(ILogger<ConsultaAConfiguracaoDoImposto> _logger, IServicoQueListaConfiguracaoImposto _servicoQueListaConfiguracaoImposto)
{
    public async Task<IEnumerable<ConfiguracaoImposto>> ExecutaAsync(CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        try
        {
            return [];
        }
        catch (Exception ex)
        {
            LogObjectExtension.CreateLog(ex, _logger, EnumLogLevel.Error, new System.Net.IPAddress(1));
            return [];
        }
    }

}
