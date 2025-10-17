namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Manipula;

public interface IServicoQueManipulaInvestimento
{
    Task AtualizaInvestimentoComRendimentoDaPosicaoAsync(Investimento investimento, CancellationToken token);
}
