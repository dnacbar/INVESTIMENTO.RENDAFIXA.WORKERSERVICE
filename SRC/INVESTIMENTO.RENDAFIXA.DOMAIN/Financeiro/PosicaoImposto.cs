using INVESTIMENTO.RENDAFIXA.DOMAIN.DataHora.Servico;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Imposto;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Imposto.Enum;

namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro;

public class PosicaoImposto
{
    public PosicaoImposto(Posicao posicao, EnumTipoImposto enumTipoImposto, decimal nmValorImposto)
    {
        IdInvestimento = posicao.IdInvestimento;
        IdPosicao = posicao.IdPosicao;
        IdImposto = enumTipoImposto;
        NmValorImposto = nmValorImposto;
    }

    public PosicaoImposto(Guid idInvestimento, short idPosicao, string txNome, EnumTipoImposto idImposto, decimal nmValorImposto)
    {
        ArgumentException.ThrowIfNullOrEmpty(txNome);

        IdInvestimento = idInvestimento;
        IdPosicao = idPosicao;
        TxNome = txNome;
        IdImposto = idImposto;
        NmValorImposto = nmValorImposto;
    }

    public Guid IdInvestimento { get; }
    public short IdPosicao { get; }
    public string TxNome { get; } = string.Empty;
    public EnumTipoImposto IdImposto { get; }
    public decimal NmValorImposto { get; }

    public static void CalculaImposto(Posicao posicao, IEnumerable<ConfiguracaoImposto> listaDeImposto)
    {
        PosicaoImposto impostoIrrf;
        var quantidadeDeDiasUteis =  posicao.Investimento.DtInicial.Date.CalculaDiaUtilEntreDatas(DateTime.Today);

        if (posicao.Investimento.VerificaSeCalculaIof())
        {
            var impostoIof = new PosicaoImposto(posicao, EnumTipoImposto.Iof, posicao.NmValorBruto * (ConfiguracaoImpostoExtension.ObtemIof(listaDeImposto, quantidadeDeDiasUteis).NmRendimento / 100));
            impostoIrrf = new PosicaoImposto(posicao, EnumTipoImposto.Irrf, (posicao.NmValorBruto - impostoIof.NmValorImposto) * (ConfiguracaoImpostoExtension.ObtemIrrf(listaDeImposto, quantidadeDeDiasUteis).NmRendimento / 100));
            posicao.ListaDePosicaoImposto.Add(impostoIof);
            posicao.ListaDePosicaoImposto.Add(impostoIrrf);
            return;
        }

        impostoIrrf = new PosicaoImposto(posicao, EnumTipoImposto.Irrf, (posicao.NmValorBruto - decimal.Zero) * (ConfiguracaoImpostoExtension.ObtemIrrf(listaDeImposto, quantidadeDeDiasUteis).NmRendimento / 100));
        posicao.ListaDePosicaoImposto.Add(impostoIrrf);
    }
}
