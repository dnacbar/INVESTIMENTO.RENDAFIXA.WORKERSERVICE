namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Consulta;

public interface IServicoQueListaInvestimentoSemBloqueio
{
    Task<List<Investimento>> ListaInvestimentoParaCalculoDePosicaoAsync(CancellationToken token);
}
