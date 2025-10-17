namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Imposto.BancoDeDados.Consulta;

public interface IServicoQueConsultaConfiguracaoImposto
{
    Task<List<ConfiguracaoImposto>> ListaConfiguracaoImpostoAsync(CancellationToken token);
}