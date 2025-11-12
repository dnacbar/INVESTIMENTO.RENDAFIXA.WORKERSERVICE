using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Imposto.Enum;

namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Imposto;

public class ConfiguracaoImposto(byte idImposto, byte idConfiguracaoImposto, decimal nmRendimento, short nmDiasCorridos) : Imposto(idImposto)
{
    public new byte IdImposto { get; } = idImposto;
    public byte IdConfiguracaoImposto { get; } = idConfiguracaoImposto;
    public decimal NmRendimento { get; } = nmRendimento;
    public short NmDiasCorridos { get; } = nmDiasCorridos;
}

public static class ConfiguracaoImpostoExtension
{
    public static ConfiguracaoImposto ObtemIof(this IEnumerable<ConfiguracaoImposto> listaDeImposto, int posicaoDiaUtil)
    {
        var listaDeImpostoFiltrada = listaDeImposto.Where(x => (EnumTipoImposto)x.IdImposto == EnumTipoImposto.Iof);

        return posicaoDiaUtil >= Iof.DiasCorridosParaIsencao
            ? listaDeImpostoFiltrada.MaxBy(x => x.NmDiasCorridos) ?? throw new DomainException($"Configuração de IOF não encontrada.")
            : listaDeImpostoFiltrada.FirstOrDefault(x => x.NmDiasCorridos == posicaoDiaUtil) ?? throw new DomainException($"Configuração de IOF não encontrada para {posicaoDiaUtil} dias úteis.");
    }

    public static ConfiguracaoImposto ObtemIrrf(this IEnumerable<ConfiguracaoImposto> listaDeImposto, int posicaoDiaUtil)
    {
        var listaDeImpostoFiltrada = listaDeImposto.Where(x => (EnumTipoImposto)x.IdImposto == EnumTipoImposto.Irrf);

        return posicaoDiaUtil >= Irrf.DiasUteisParaMenorAliquota
            ? listaDeImpostoFiltrada.MaxBy(x => x.NmDiasCorridos) ?? throw new DomainException("Configuração de IRRF não encontrada")
            : listaDeImpostoFiltrada.FirstOrDefault(x => posicaoDiaUtil <= x.NmDiasCorridos) ?? throw new DomainException($"Configuração de IRRF não encontrada para {posicaoDiaUtil} dias úteis.");
    }
}
