namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Imposto;

public abstract class Imposto(byte idImposto)
{
    public byte IdImposto { get; } = idImposto;
    public string TxNome { get; } = string.Empty;
    public string TxDescricao { get; } = string.Empty;
}
