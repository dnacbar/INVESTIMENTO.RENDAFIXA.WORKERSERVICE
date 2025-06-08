using DN.LOG.LIBRARY.MODEL;
using DN.LOG.LIBRARY.MODEL.ENUM;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Indice;

namespace INVESTIMENTO.RENDAFIXA.CRONJOB.Servico;

public class ConsultaOIndexadorDaTaxa (ILogger<ConsultaOIndexadorDaTaxa> _logger)
{
    public async Task<IEnumerable<Indexador>> ExecutaAsync(CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        try
        {
            return []; //await ObtemListaDeIndexadorDaTaxa(token);
        }
        catch (Exception ex)
        {
            LogObjectExtension.CreateLog(ex, _logger, EnumLogLevel.Error, new System.Net.IPAddress(1));
            return [];
        }
    }
}
