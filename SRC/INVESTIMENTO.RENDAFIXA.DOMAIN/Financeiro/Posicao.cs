using DN.LOG.LIBRARY.MODEL.EXCEPTION;

namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro;

public class Posicao
{
    private const int PrimeiroRegistroPosicao = 1;

    public Posicao(Investimento investimento)
    {
        IdInvestimento = investimento.IdInvestimento;
        IdPosicao = PrimeiroRegistroPosicao;
        DtPosicao = DateTime.Today;
        NmValorBrutoTotal = investimento.NmValorInicial;
        NmValorLiquidoTotal = NmValorBrutoTotal;
        NmValorBruto = decimal.Zero;
        NmValorLiquido = decimal.Zero;
        TxUsuario = investimento.TxUsuario;
        DtCriacao = DateTime.Now;
    }

    public Posicao(Guid idInvestimento, short idPosicao, DateTime dtPosicao, decimal nmValorBrutoTotal,
        decimal nmValorLiquidoTotal, decimal nmValorBruto, decimal nmValorLiquido)
    {
        
        IdInvestimento = idInvestimento;
        IdPosicao = idPosicao;
        DtPosicao = dtPosicao;
        NmValorBrutoTotal = nmValorBrutoTotal;
        NmValorLiquidoTotal = nmValorLiquidoTotal;
        NmValorBruto = nmValorBruto;
        NmValorLiquido = nmValorLiquido;

        ValidaPosicao();
    }

    public Posicao(Guid idInvestimento, short idPosicao)
    {
        IdInvestimento = idInvestimento;
        IdPosicao = idPosicao;
    }

    public virtual IEnumerable<PosicaoImposto> ListaDePosicaoImposto { get; } = null!;

    public Guid IdInvestimento { get; }
    public short IdPosicao { get; }
    public DateTime DtPosicao { get; }
    public decimal NmValorBrutoTotal { get; }
    public decimal NmValorLiquidoTotal { get; }
    public decimal NmValorBruto { get; }
    public decimal NmValorLiquido { get; }
    public string TxUsuario { get; } = string.Empty;
    public DateTime DtCriacao { get; }
    public string? TxUsuarioAtualizacao { get; }
    public DateTime? DtAtualizacao { get; }

    public bool VerificaSeValorLiquidoTotalEstaZerado() => NmValorLiquidoTotal == decimal.Zero;

    private void ValidaPosicao()
    {
        if (!VerificaSeValorBrutoTotalEhMaiorQueOValorLiquidoTotal())
            throw new BadRequestException($"Valor bruto total tem que ser maior que o valor líquido total! Valor bruto total:[{NmValorBruto}] Valor líquido total:[{NmValorLiquido}]");

        if (!VerificaSeValorBrutoTotalEhMaiorQueOValorBruto())
            throw new BadRequestException($"Valor bruto total tem que ser maior que o valor bruto! Valor bruto total:[{NmValorBrutoTotal}] Valor bruto:[{NmValorBruto}]");

        if (!VerificaSeValorLiquidoTotalEhMaiorQueOValorLiquido())
            throw new BadRequestException($"Valor líquido total tem que ser maior que o valor líquido! Valor líquido total:[{NmValorLiquidoTotal}] Valor líquido:[{NmValorLiquido}]");

        if (!VerificaSeValorBrutoEhMaiorQueOValorLiquido())
            throw new BadRequestException($"Valor bruto tem que ser maior que o valor líquido! Valor bruto:[{NmValorBruto}] Valor líquido:[{NmValorLiquido}]");

        if (!VerificaSeValoresTotaisSaoMaioresQueOValorImposto())
            throw new BadRequestException($"Valor bruto total e valor líquido total tem que ser maior que o valor do imposto!");

        if (!VerificaSeValoresTotaisSaoMaioresQueASomaDoValorDeImposto())
            throw new BadRequestException($"Valor bruto total e valor líquido total que ser maior que o valor da soma dos impostos! Valor bruto:[{NmValorBruto}] Valor líquido:[{NmValorLiquido}] Valor imposto somado:[{ListaDePosicaoImposto.Sum(x => x.NmValorImposto)}]");
    }

    private bool VerificaSeValorBrutoEhMaiorQueOValorLiquido() => NmValorBruto > NmValorLiquido;
    private bool VerificaSeValorBrutoTotalEhMaiorQueOValorBruto() => NmValorBrutoTotal > NmValorBruto;
    private bool VerificaSeValorBrutoTotalEhMaiorQueOValorLiquidoTotal() => NmValorBrutoTotal > NmValorLiquidoTotal;
    private bool VerificaSeValoresTotaisSaoMaioresQueASomaDoValorDeImposto()
    {
        var valorImpostoSomado = ListaDePosicaoImposto.Sum(x => x.NmValorImposto);
        return NmValorBrutoTotal > valorImpostoSomado && NmValorLiquidoTotal > valorImpostoSomado;
    }
    private bool VerificaSeValoresTotaisSaoMaioresQueOValorImposto() => ListaDePosicaoImposto.All(x => NmValorLiquidoTotal > x.NmValorImposto && NmValorBrutoTotal > x.NmValorImposto);
    private bool VerificaSeValorLiquidoTotalEhMaiorQueOValorLiquido() => NmValorLiquidoTotal > NmValorLiquido;
}