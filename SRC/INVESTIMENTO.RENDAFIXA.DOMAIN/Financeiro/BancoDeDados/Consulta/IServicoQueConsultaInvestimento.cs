namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Consulta;

public interface IServicoQueConsultaInvestimento
{
    Task<IEnumerable<Investimento>> ListaInvestimentoLiquidadoParaAdicaoDeResgateAsync(CancellationToken token);
    Task<IEnumerable<Investimento>> ListaInvestimentoParaCalculoDePosicaoAsync(CancellationToken token);
    Task<IEnumerable<Investimento>> ListaInvestimentoQueDeveSerLiquidadoPelaDataAsync(CancellationToken token);
}
