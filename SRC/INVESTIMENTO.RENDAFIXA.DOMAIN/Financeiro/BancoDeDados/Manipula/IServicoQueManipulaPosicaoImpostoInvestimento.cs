namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Manipula;

public interface IServicoQueManipulaPosicaoImpostoInvestimento
{
    Task AdicionaPosicaoImpostoInvestimentoAsync(PosicaoImposto posicaoImposto, CancellationToken token);
}
