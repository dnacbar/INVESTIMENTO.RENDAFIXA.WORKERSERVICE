namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Financeiro.BancoDeDados.Manipula;

public interface IServicoQueAdicionaOuAtualizaPosicaoImpostoInvestimento
{
    Task AdicionaPosicaoImpostoInvestimentoAsync(ImpostoPosicao posicaoImposto, CancellationToken token);
}
