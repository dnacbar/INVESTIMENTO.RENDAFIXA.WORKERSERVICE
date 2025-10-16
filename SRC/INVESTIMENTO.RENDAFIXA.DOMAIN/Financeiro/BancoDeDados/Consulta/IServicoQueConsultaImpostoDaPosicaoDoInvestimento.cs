namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Financeiro.BancoDeDados.Consulta
{
    public interface IServicoQueConsultaImpostoDaPosicaoDoInvestimento
    {
        Task<ImpostoPosicao> ObtemImpostoDaPosicaoDoInvestimentoAsync(CancellationToken token);
    }
}
