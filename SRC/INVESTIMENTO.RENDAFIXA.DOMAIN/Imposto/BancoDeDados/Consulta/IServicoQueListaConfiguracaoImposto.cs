namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Imposto.BancoDeDados.Consulta;

public interface IServicoQueListaConfiguracaoImposto
{
    Task<List<ConfiguracaoImposto>> ListaConfiguracaoImpostoAsync(CancellationToken token);
}