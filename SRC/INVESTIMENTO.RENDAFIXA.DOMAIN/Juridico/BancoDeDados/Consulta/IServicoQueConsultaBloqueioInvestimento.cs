namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Juridico.BancoDeDados.Consulta;

public interface IServicoQueConsultaBloqueioInvestimento
{
    Task<decimal> ObtemValorBloqueadoTotalAsync(BloqueioInvestimento bloqueioInvestimento, CancellationToken cancellationToken);
}
