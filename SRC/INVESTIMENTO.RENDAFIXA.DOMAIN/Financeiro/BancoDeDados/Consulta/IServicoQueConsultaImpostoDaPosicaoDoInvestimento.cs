namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Consulta
{
    public interface IServicoQueConsultaImpostoDaPosicaoDoInvestimento
    {
        Task<ImpostoPosicao> ObtemImpostoDaPosicaoDoInvestimentoAsync(CancellationToken token);
    }
}
