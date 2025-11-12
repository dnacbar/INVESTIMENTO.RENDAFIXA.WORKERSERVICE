using INVESTIMENTO.RENDAFIXA.DOMAIN.Imposto;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Imposto.Enum;

namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro;

public class PosicaoImposto
{
    public PosicaoImposto() { }
    public PosicaoImposto(Posicao posicao, IEnumerable<ConfiguracaoImposto> listaDeConfiguracaoImposto)
    {
        Posicao = posicao;
        ListaDeConfiguracaoImposto = listaDeConfiguracaoImposto;

        var quantidadeDeDiasCorridos = (DateTime.Today - Posicao.Investimento.DtInicial.Date).Days;  

        if (Posicao.Investimento.VerificaSeCalculaIof())
        {
            NmValorImpostoIof = Posicao.NmValorBruto * (ConfiguracaoImpostoExtension.ObtemIof(ListaDeConfiguracaoImposto, quantidadeDeDiasCorridos).NmRendimento / 100);
            NmValorImpostoIrrf = (Posicao.NmValorBruto - NmValorImpostoIof) * (ConfiguracaoImpostoExtension.ObtemIrrf(ListaDeConfiguracaoImposto, quantidadeDeDiasCorridos).NmRendimento / 100);

            ListaDeImpostoCalculadoPorTipo.Add((EnumTipoImposto.Iof, NmValorImpostoIof));
            ListaDeImpostoCalculadoPorTipo.Add((EnumTipoImposto.Irrf, NmValorImpostoIrrf));

            return;
        }

        NmValorImpostoIrrf = Posicao.NmValorBruto * (ConfiguracaoImpostoExtension.ObtemIrrf(ListaDeConfiguracaoImposto, quantidadeDeDiasCorridos).NmRendimento / 100);

        ListaDeImpostoCalculadoPorTipo.Add((EnumTipoImposto.Irrf, NmValorImpostoIrrf));
    }

    public Posicao Posicao { get; } = null!;

    public decimal NmValorImpostoIof { get; private set; }
    public decimal NmValorImpostoIrrf { get; private set; }
    public decimal NmValorImpostoSomado => NmValorImpostoIrrf + NmValorImpostoIof;
    public List<(EnumTipoImposto, decimal)> ListaDeImpostoCalculadoPorTipo { get; } = [];

    private IEnumerable<ConfiguracaoImposto> ListaDeConfiguracaoImposto { get; } = [];
}
