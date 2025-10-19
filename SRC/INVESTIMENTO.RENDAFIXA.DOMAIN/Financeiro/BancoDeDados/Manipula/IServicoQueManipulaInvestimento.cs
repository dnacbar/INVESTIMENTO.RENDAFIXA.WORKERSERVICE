namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Manipula;

public interface IServicoQueManipulaInvestimento
{
    Task AtualizaInvestimentoComRendimentoDaPosicaoAsync(Investimento investimento, CancellationToken token);
    Task AtualizaInvestimentoLiquidadoPelaDataAsync(Investimento investimento, CancellationToken token);
}
