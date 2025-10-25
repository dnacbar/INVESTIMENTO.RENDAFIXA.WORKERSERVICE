namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Imposto.BancoDeDados.Consulta;

public interface IServicoQueConsultaConfiguracaoImposto
{
    Task<IEnumerable<ConfiguracaoImposto>> ListaConfiguracaoImpostoAsync(CancellationToken token);
}