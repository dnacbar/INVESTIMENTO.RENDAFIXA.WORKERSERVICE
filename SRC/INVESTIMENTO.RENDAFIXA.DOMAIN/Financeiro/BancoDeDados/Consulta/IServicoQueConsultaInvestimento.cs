namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Financeiro.BancoDeDados.Consulta;

public interface IServicoQueConsultaInvestimento
{
    Task<List<Investimento>> ListaInvestimentoParaCalculoDePosicaoAsync(CancellationToken token);
}
