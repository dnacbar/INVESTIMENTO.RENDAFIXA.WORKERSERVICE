namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Manipula;

public interface IServicoQueManipulaPosicaoInvestimento
{
    Task AdicionaPosicaoInvestimentoAsync(Posicao posicao, CancellationToken token);
}
