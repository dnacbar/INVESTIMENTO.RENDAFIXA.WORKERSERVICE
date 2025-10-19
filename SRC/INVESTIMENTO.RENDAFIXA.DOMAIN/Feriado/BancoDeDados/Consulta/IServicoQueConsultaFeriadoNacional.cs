namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Feriado.BancoDeDados.Consulta;

public interface IServicoQueConsultaFeriadoNacional
{
    Task<List<FeriadoNacional>> ListaAsync(CancellationToken cancellationToken);
}
