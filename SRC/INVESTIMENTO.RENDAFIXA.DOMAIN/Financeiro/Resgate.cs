namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro;

public class Resgate(Guid idInvestimento, byte idResgate, decimal nmValor, decimal nmValorImposto)
{
    public Guid IdInvestimento { get; } = idInvestimento;
    public byte IdResgate { get; } = idResgate;
    public decimal NmValor { get; private set; } = nmValor;
    public decimal NmValorImposto { get; private set; } = nmValorImposto;
    public string TxUsuario { get; } = "WORKERSERVICE";
}
