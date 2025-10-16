namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Financeiro.BancoDeDados.Manipula;

public interface IServicoQueAtualizaInvestimento
{
    Task AtualizaInvestimentoComRendimentoDaPosicaoAsync(Investimento investimento, CancellationToken token);
}
