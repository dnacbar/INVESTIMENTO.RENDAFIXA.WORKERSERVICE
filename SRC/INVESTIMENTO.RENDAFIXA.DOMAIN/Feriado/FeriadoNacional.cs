namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Feriado;

public class FeriadoNacional(short idFeriado, DateTime dtFeriado, string txNome)
{
    public short IdFeriado { get; } = idFeriado;
    public DateTime DtFeriado { get; private set; } = dtFeriado;
    public string TxNome { get; } = txNome;
    public string TxUsuario { get; } = "WORKERSERVICE";

    public void AtualizaDataFeriado() => DtFeriado = DtFeriado.AddYears(1);
    public bool VerificaSeDataAtualEhFeriado() => DtFeriado.Date == DateTime.Today;
}

public static class FeriadoNacionalExtension
{
    public static bool VerificaSeDataAtualEstaNaListaDeFeriadoNacional(this IEnumerable<FeriadoNacional> listaDeFeriadoNacional) => listaDeFeriadoNacional.Any(feriado => feriado.VerificaSeDataAtualEhFeriado());
}