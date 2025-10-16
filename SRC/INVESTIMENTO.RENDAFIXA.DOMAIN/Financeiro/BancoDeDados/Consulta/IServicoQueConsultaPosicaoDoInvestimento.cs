namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Financeiro.BancoDeDados.Consulta;

public interface IServicoQueConsultaPosicaoDoInvestimento
{
    Task<Posicao> ObtemPosicaoDoInvestimentoParaCalculoDePosicaoAsync(Investimento investimento, CancellationToken token);
}
