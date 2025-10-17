namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Manipula;

public interface IServicoQueManipulaPosicaoImpostoInvestimento
{
    Task AdicionaPosicaoImpostoInvestimentoAsync(ImpostoPosicao posicaoImposto, CancellationToken token);
}
