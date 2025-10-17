namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Manipula;

public interface IServicoQueManipulaResgate
{
    Task AdicionaAsync(Resgate resgate, CancellationToken cancellationToken);
}