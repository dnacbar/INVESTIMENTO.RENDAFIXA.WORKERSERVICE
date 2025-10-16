namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Financeiro.BancoDeDados.Manipula;

public interface IServicoQueAdicionaOuAtualizaPosicaoInvestimento
{
    Task AdicionaPosicaoInvestimentoAsync(Posicao posicao, CancellationToken token);
}
