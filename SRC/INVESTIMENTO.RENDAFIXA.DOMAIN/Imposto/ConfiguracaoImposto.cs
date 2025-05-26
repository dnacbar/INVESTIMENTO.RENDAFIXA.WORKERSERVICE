using INVESTIMENTO.RENDAFIXA.DOMAIN.Imposto.Enum;

namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Imposto;

public class ConfiguracaoImposto(byte idImposto, byte idConfiguracaoImposto, decimal nmRendimento, short nmDiasUteis) : Imposto(idImposto)
{
    public new byte IdImposto { get; } = idImposto;
    public byte IdConfiguracaoImposto { get; } = idConfiguracaoImposto;
    public decimal NmRendimento { get; } = nmRendimento;
    public short NmDiasUteis { get; } = nmDiasUteis;
}

public static class ConfiguracaoImpostoExtension
{
    public static ConfiguracaoImposto ObtemIof(this IEnumerable<ConfiguracaoImposto> listaDeImposto, int posicaoDiaUtil)
    {
        if (posicaoDiaUtil >= Iof.DiasUteisParaIsencao)
            return listaDeImposto.Where(x => (EnumTipoImposto)x.IdImposto == EnumTipoImposto.Iof)
                                 .Select(x => new Iof(x.IdImposto, x.IdConfiguracaoImposto, x.NmRendimento, x.NmDiasUteis))
                                 .Last();

        return listaDeImposto.Where(x => (EnumTipoImposto)x.IdImposto == EnumTipoImposto.Iof && x.NmDiasUteis == posicaoDiaUtil)
                             .Select(x => new Iof(x.IdImposto, x.IdConfiguracaoImposto, x.NmRendimento, x.NmDiasUteis))
                             .First();
    }

    public static ConfiguracaoImposto ObtemIrrf(this IEnumerable<ConfiguracaoImposto> listaDeImposto, int posicaoDiaUtil)
    {
        if (posicaoDiaUtil >= Irrf.DiasUteisParaMenorAliquota)
            return listaDeImposto.Where(x => (EnumTipoImposto)x.IdImposto == EnumTipoImposto.Irrf)
                                 .Select(x => new Irrf(x.IdImposto, x.IdConfiguracaoImposto, x.NmRendimento, x.NmDiasUteis))
                                 .Last();

        return listaDeImposto.Where(x => (EnumTipoImposto)x.IdImposto == EnumTipoImposto.Irrf && posicaoDiaUtil <= x.NmDiasUteis )
                             .Select(x => new Irrf(x.IdImposto, x.IdConfiguracaoImposto, x.NmRendimento, x.NmDiasUteis))
                             .First();
    }
}
