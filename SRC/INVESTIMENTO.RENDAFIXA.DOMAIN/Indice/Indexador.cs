namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Indice;

public class Indexador
{
    public Indexador() { }
    public Indexador(byte idIndexador, decimal nmRendimento)
    {
        IdIndexador = idIndexador;
        NmRendimento = nmRendimento;
    }

    public byte IdIndexador { get; }
    public string TxNome { get; } = null!;
    public string? TxDescricao { get; }
    public decimal NmRendimento { get; }
}
