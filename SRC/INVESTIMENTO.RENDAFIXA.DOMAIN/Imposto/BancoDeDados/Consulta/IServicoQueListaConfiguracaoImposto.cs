namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Imposto.BancoDeDados.Consulta;

public interface IServicoQueListaConfiguracaoImposto
{
    Task<List<ConfiguracaoImposto>> ListaConfiguracaoImpostoAsync(CancellationToken token);
}