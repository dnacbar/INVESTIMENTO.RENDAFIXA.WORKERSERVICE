namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Feriado.BancoDeDados.Manipula;

public interface IServicoQueManipulaFeriadoNacional
{
    Task AtualizaAsync(FeriadoNacional feriadoNacional, CancellationToken cancellationToken);
}
