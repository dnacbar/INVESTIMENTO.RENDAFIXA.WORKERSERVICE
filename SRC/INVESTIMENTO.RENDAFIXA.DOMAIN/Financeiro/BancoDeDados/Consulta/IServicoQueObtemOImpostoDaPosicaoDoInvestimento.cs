namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Consulta
{
    public interface IServicoQueObtemOImpostoDaPosicaoDoInvestimento
    {
        Task<ImpostoPosicao> ObtemImpostoDaPosicaoDoInvestimentoAsync(CancellationToken token);
    }
}
