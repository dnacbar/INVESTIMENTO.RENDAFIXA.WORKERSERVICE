namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Configuracao;

public interface IInvestimentoRendaFixaWorkerService
{
    string Usuario { get; }
    short TempoLimiteTransacion { get; }
}
