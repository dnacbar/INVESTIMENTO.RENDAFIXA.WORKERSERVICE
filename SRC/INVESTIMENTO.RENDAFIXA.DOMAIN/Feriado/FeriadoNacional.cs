namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Feriado;

public class FeriadoNacional(short idFeriado, DateTime dtFeriado, string txNome)
{
    public short IdFeriado { get; } = idFeriado;
    public DateTime DtFeriado { get; private set; } = dtFeriado;
    public string TxNome { get; } = txNome;

    public void AtualizaDataFeriado() => DtFeriado = DtFeriado.AddYears(1);
}