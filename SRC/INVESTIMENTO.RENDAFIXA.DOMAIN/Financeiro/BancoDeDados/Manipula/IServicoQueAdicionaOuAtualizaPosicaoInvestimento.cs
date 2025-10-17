namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Manipula;

public interface IServicoQueAdicionaOuAtualizaPosicaoInvestimento
{
    Task AdicionaPosicaoInvestimentoAsync(Posicao posicao, CancellationToken token);
}
