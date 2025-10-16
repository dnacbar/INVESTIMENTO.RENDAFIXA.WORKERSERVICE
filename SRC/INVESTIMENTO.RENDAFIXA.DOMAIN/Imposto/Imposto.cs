namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Imposto;

public abstract class Imposto(byte idImposto)
{
    public const string Iof = "IOF";
    public const string Irrf = "IRRF";

    public byte IdImposto { get; } = idImposto;
    public string TxNome { get; } = string.Empty;
    public string TxDescricao { get; } = string.Empty;
}
