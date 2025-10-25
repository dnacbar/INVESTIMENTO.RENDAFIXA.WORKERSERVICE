namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Feriado.BancoDeDados.Consulta;

public interface IServicoQueConsultaFeriadoNacional
{
    Task<IEnumerable<FeriadoNacional>> ListaAsync(CancellationToken cancellationToken);
}
