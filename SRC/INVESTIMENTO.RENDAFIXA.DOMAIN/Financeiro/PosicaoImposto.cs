using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Imposto.Enum;

namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro;

public class PosicaoImposto
{
    public PosicaoImposto(Guid idInvestimento, short idPosicao, string txNome, EnumTipoImposto enumTipoImposto, decimal nmValorImposto)
    {
        IdInvestimento = idInvestimento;
        IdPosicao = idPosicao;
        TxNome = txNome;
        IdImposto = enumTipoImposto;
        NmValorImposto = nmValorImposto;
    }

    public PosicaoImposto(Guid idInvestimento, short idPosicao, short idImposto, decimal nmValorImposto)
    {
        IdInvestimento = idInvestimento;
        IdPosicao = idPosicao;
        IdImposto = (EnumTipoImposto)idImposto;
        NmValorImposto = nmValorImposto;

        ValidaPosicaoImposto();
    }

    public Guid IdInvestimento { get; }
    public short IdPosicao { get; }
    public string TxNome { get; } = string.Empty;
    public EnumTipoImposto IdImposto { get; }
    public decimal NmValorImposto { get; }

    private void ValidaPosicaoImposto()
    {
        if (!VerificaSeValorImpostoEhPositivo())
            throw new BadRequestException($"Valor imposto tem que ser positivo! Valor imposto:[{NmValorImposto}]");
    }

    private bool VerificaSeValorImpostoEhPositivo() => NmValorImposto > decimal.Zero;
}
