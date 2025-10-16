namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Configuracao;

public interface IInvestimentoRendaFixaWorkerService
{
    string Usuario { get; }
    short TempoLimiteTransacion { get; }
}
