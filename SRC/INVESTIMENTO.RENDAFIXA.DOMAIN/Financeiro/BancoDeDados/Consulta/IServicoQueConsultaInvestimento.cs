namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Consulta;

public interface IServicoQueConsultaInvestimento
{
    Task<List<Investimento>> ListaInvestimentoParaCalculoDePosicaoAsync(CancellationToken token);
}
