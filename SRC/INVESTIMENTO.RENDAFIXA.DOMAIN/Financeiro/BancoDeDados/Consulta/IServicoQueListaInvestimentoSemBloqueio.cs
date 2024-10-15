namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Consulta;

public interface IServicoQueListaInvestimentoSemBloqueio
{
    Task<IEnumerable<Investimento>> ListaInvestimento();
}
