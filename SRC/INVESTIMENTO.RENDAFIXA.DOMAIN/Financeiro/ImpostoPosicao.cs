using INVESTIMENTO.RENDAFIXA.DOMAIN.DataHora.Servico;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Imposto;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Imposto.Enum;

namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro;

public class ImpostoPosicao(Posicao posicao, EnumTipoImposto idTipoImposto, decimal nmValorImposto)
{
    public Guid IdInvestimento { get; } = posicao.IdInvestimento;
    public short IdPosicao { get; } = posicao.IdPosicao;
    public EnumTipoImposto IdTipoImposto { get; } = idTipoImposto;
    public decimal NmValorImposto { get; } = nmValorImposto;

    public static void CalculaImposto(Posicao posicao, IEnumerable<ConfiguracaoImposto> listaDeImposto)
    {
        ImpostoPosicao impostoIrrf;
        var quantidadeDeDiasUteis = posicao.Investimento.DtInicial.Date.CalculaDiaUtilEntreDatas(DateTime.Today);

        if (posicao.Investimento.VerificaSeCalculaIof())
        {
            var impostoIof = new ImpostoPosicao(posicao, EnumTipoImposto.Iof, posicao.NmValorBruto * (ConfiguracaoImpostoExtension.ObtemIof(listaDeImposto, quantidadeDeDiasUteis).NmRendimento / 100));
            impostoIrrf = new ImpostoPosicao(posicao, EnumTipoImposto.Irrf, (posicao.NmValorBruto - impostoIof.NmValorImposto) * (ConfiguracaoImpostoExtension.ObtemIrrf(listaDeImposto, quantidadeDeDiasUteis).NmRendimento / 100));
            posicao.ListaDePosicaoImposto.Add(impostoIof);
            posicao.ListaDePosicaoImposto.Add(impostoIrrf);
            return;
        }

        impostoIrrf = new ImpostoPosicao(posicao, EnumTipoImposto.Irrf, (posicao.NmValorBruto - decimal.Zero) * (ConfiguracaoImpostoExtension.ObtemIrrf(listaDeImposto, quantidadeDeDiasUteis).NmRendimento / 100));
        posicao.ListaDePosicaoImposto.Add(impostoIrrf);
    }
}
