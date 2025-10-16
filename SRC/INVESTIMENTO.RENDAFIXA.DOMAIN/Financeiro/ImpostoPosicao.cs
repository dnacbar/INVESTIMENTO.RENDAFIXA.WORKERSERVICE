using INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.DataHora.Servico;
using INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Imposto;
using INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Imposto.Enum;

namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Financeiro;

public class ImpostoPosicao
{
    public ImpostoPosicao() { }
    public ImpostoPosicao(Posicao posicao, IEnumerable<ConfiguracaoImposto> listaDeConfiguracaoImposto)
    {
        Posicao = posicao;
        ListaDeConfiguracaoImposto = listaDeConfiguracaoImposto;

        var quantidadeDeDiasUteis = Posicao.Investimento.DtInicial.Date.CalculaDiaUtilEntreDatas(DateTime.Today);

        if (Posicao.Investimento.VerificaSeCalculaIof())
        {
            NmValorImpostoIof = Posicao.NmValorBruto * (ConfiguracaoImpostoExtension.ObtemIof(ListaDeConfiguracaoImposto, quantidadeDeDiasUteis).NmRendimento / 100);
            NmValorImpostoIrrf = (Posicao.NmValorBruto - NmValorImpostoIof) * (ConfiguracaoImpostoExtension.ObtemIrrf(ListaDeConfiguracaoImposto, quantidadeDeDiasUteis).NmRendimento / 100);

            ListaDeImpostoCalculadoPorTipo.Add((EnumTipoImposto.Iof, NmValorImpostoIof));
            ListaDeImpostoCalculadoPorTipo.Add((EnumTipoImposto.Irrf, NmValorImpostoIrrf));

            return;
        }

        NmValorImpostoIrrf = (Posicao.NmValorBruto - decimal.Zero) * (ConfiguracaoImpostoExtension.ObtemIrrf(ListaDeConfiguracaoImposto, quantidadeDeDiasUteis).NmRendimento / 100);

        ListaDeImpostoCalculadoPorTipo.Add((EnumTipoImposto.Irrf, NmValorImpostoIrrf));
    }

    public Posicao Posicao { get; } = null!;

    public decimal NmValorImpostoIof { get; private set; }
    public decimal NmValorImpostoIrrf { get; private set; }
    public decimal NmValorImpostoSomado => NmValorImpostoIrrf + NmValorImpostoIof;
    public List<(EnumTipoImposto, decimal)> ListaDeImpostoCalculadoPorTipo { get; } = [];

    private IEnumerable<ConfiguracaoImposto> ListaDeConfiguracaoImposto { get; } = [];
}
