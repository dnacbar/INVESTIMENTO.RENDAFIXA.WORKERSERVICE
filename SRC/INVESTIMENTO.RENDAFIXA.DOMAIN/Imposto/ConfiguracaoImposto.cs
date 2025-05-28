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
        var listaDeImpostoFiltrada = listaDeImposto.Where(x => (EnumTipoImposto)x.IdImposto == EnumTipoImposto.Iof);

        return posicaoDiaUtil >= Iof.DiasUteisParaIsencao
            ? listaDeImpostoFiltrada.MaxBy(x => x.NmDiasUteis) ?? throw new InvalidOperationException($"Configuração de IOF não encontrada.")
            : listaDeImpostoFiltrada.FirstOrDefault(x => x.NmDiasUteis == posicaoDiaUtil) ?? throw new InvalidOperationException($"Configuração de IOF não encontrada para {posicaoDiaUtil} dias úteis.");
    }

    public static ConfiguracaoImposto ObtemIrrf(this IEnumerable<ConfiguracaoImposto> listaDeImposto, int posicaoDiaUtil)
    {
        var listaDeImpostoFiltrada = listaDeImposto.Where(x => (EnumTipoImposto)x.IdImposto == EnumTipoImposto.Irrf);

        return posicaoDiaUtil >= Irrf.DiasUteisParaMenorAliquota
            ? listaDeImpostoFiltrada.MaxBy(x => x.NmDiasUteis) ?? throw new InvalidOperationException("Configuração de IRRF não encontrada")
            : listaDeImpostoFiltrada.FirstOrDefault(x => posicaoDiaUtil <= x.NmDiasUteis) ?? throw new InvalidOperationException($"Configuração de IRRF não encontrada para {posicaoDiaUtil} dias úteis.");
    }
}
