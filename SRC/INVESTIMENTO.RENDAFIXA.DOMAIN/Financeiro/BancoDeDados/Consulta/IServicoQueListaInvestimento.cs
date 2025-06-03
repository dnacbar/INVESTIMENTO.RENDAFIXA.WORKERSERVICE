namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Consulta;

public interface IServicoQueListaInvestimento
{
    Task<List<Investimento>> ListaInvestimentoParaCalculoDePosicaoAsync(CancellationToken token);
}
