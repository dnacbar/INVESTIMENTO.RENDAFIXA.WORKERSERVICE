namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Consulta
{
    public interface IServicoQueObtemOImpostoDaPosicaoDoInvestimento
    {
        Task<PosicaoImposto> ObtemImpostoDaPosicaoDoInvestimentoAsync(CancellationToken token);
    }
}
