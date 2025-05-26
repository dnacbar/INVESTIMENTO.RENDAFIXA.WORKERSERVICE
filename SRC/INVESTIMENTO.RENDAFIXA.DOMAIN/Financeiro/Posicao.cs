using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Imposto;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Imposto.Enum;

namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro;

public class Posicao
{
    private const short IdPosicaoInicial = 1;

    public Posicao(Investimento investimento, short idPosicao, DateTime dtPosicao, decimal nmValorBrutoTotal, decimal nmValorLiquidoTotal, decimal nmValorBruto, decimal nmValorLiquido)
    {
        IdInvestimento = investimento.IdInvestimento;
        IdPosicao = idPosicao;
        DtPosicao = dtPosicao;
        NmValorBrutoTotal = nmValorBrutoTotal;
        NmValorLiquidoTotal = nmValorLiquidoTotal;
        NmValorBruto = nmValorBruto;
        NmValorLiquido = nmValorLiquido;
        TxUsuario = investimento.TxUsuario;
        DtCriacao = investimento.DtCriacao;

        Investimento = investimento;

        ValidaPosicao();
    }

    public Posicao(Investimento investimento)
    {
        IdInvestimento = investimento.IdInvestimento;
        Investimento = investimento;
        IdPosicao = IdPosicaoInicial;
        DtPosicao = DateTime.Now.Date;
    }

    public ICollection<PosicaoImposto> ListaDePosicaoImposto { get; set; } = [];
    public Investimento Investimento { get; } = null!;

    public Guid IdInvestimento { get; }
    public short IdPosicao { get; }
    public DateTime DtPosicao { get; }
    public decimal NmValorBrutoTotal { get; set; }
    public decimal NmValorLiquidoTotal { get; set; }
    public decimal NmValorBruto { get; set; }
    public decimal NmValorLiquido { get; set; }
    public string TxUsuario { get; } = string.Empty;
    public DateTime DtCriacao { get; }
    public string? TxUsuarioAtualizacao { get; }
    public DateTime? DtAtualizacao { get; }

    public Task CalculaPosicaoInvestimentoAsync(IEnumerable<ConfiguracaoImposto> listaDeImposto, CancellationToken token) =>
        Task.Run(() => { token.ThrowIfCancellationRequested(); VerificaSeCalculaPosicaoInicialOuRecorrente(listaDeImposto); }, token);
    public bool VerificaSeValorLiquidoTotalEstaZerado() => NmValorLiquidoTotal == decimal.Zero;

    private void CalculaPosicaoInicialInvestimento(IEnumerable<ConfiguracaoImposto> listaDeImposto)
    {
        NmValorBruto = Investimento.NmValorInicial * Investimento.CalculaValorTaxaDiaria();
        NmValorBrutoTotal = Investimento.NmValorInicial + NmValorBruto;

        if (Investimento.VerificaSeInvestimentoEhIsentoDeImposto())
        {
            ListaDePosicaoImposto = [];
            NmValorLiquido = NmValorBruto;
        }
        else
        {
            if (listaDeImposto == null)
                throw new DomainException($"Lista de configuração de imposto tem que ser preenchida!");

            PosicaoImposto.CalculaImposto(this, listaDeImposto);

            var impostoIof = ListaDePosicaoImposto.FirstOrDefault(x => x.IdImposto == EnumTipoImposto.Iof);
            var impostoIrrf = ListaDePosicaoImposto.FirstOrDefault(x => x.IdImposto == EnumTipoImposto.Irrf);

            NmValorLiquido = NmValorBruto - (impostoIof?.NmValorImposto ?? 0) - (impostoIrrf?.NmValorImposto ?? 0);
        }

        NmValorLiquidoTotal = Investimento.NmValorInicial + NmValorLiquido;

        Investimento.AdicionaCalculoPosicaoNoInvestimento(this);

        ValidaPosicaoInvestimento();
    }
    private void CalculaPosicaoRecorrenteInvestimento(IEnumerable<ConfiguracaoImposto> listaDeImposto)
    {
        NmValorBruto = Investimento.NmValorInicial * Investimento.CalculaValorTaxaDiaria();
        NmValorBrutoTotal = Investimento.NmValorInicial + NmValorBruto;

        if (Investimento.VerificaSeInvestimentoEhIsentoDeImposto())
        {
            ListaDePosicaoImposto = [];
            NmValorLiquido = NmValorBruto;
        }
        else
        {
            if (listaDeImposto == null)
                throw new DomainException($"Lista de configuração de imposto tem que ser preenchida!");

            PosicaoImposto.CalculaImposto(this, listaDeImposto);

            var impostoIof = ListaDePosicaoImposto.FirstOrDefault(x => x.IdImposto == EnumTipoImposto.Iof);
            var impostoIrrf = ListaDePosicaoImposto.FirstOrDefault(x => x.IdImposto == EnumTipoImposto.Irrf);

            NmValorLiquido = NmValorBruto -
                (impostoIof?.NmValorImposto ?? 0) -
                (impostoIrrf?.NmValorImposto ?? 0);
        }

        NmValorLiquidoTotal = Investimento.NmValorInicial + NmValorLiquido;

        Investimento.AdicionaCalculoPosicaoNoInvestimento(this);

        ValidaPosicaoInvestimento();
    }
    private void ValidaPosicao()
    {
        if (!VerificaSeValorBrutoTotalEhMaiorQueOValorLiquidoTotal())
            throw new DomainException($"Valor bruto total tem que ser maior que o valor líquido total! Valor bruto total:[{NmValorBruto}] Valor líquido total:[{NmValorLiquido}]");

        if (!VerificaSeValorBrutoTotalEhMaiorQueOValorBruto())
            throw new DomainException($"Valor bruto total tem que ser maior que o valor bruto! Valor bruto total:[{NmValorBrutoTotal}] Valor bruto:[{NmValorBruto}]");

        if (!VerificaSeValorLiquidoTotalEhMaiorQueOValorLiquido())
            throw new DomainException($"Valor líquido total tem que ser maior que o valor líquido! Valor líquido total:[{NmValorLiquidoTotal}] Valor líquido:[{NmValorLiquido}]");

        if (!VerificaSeValorBrutoEhMaiorQueOValorLiquido())
            throw new DomainException($"Valor bruto tem que ser maior que o valor líquido! Valor bruto:[{NmValorBruto}] Valor líquido:[{NmValorLiquido}]");

        if (!VerificaSeValoresTotaisSaoMaioresQueOValorImposto())
            throw new DomainException($"Valor bruto total e valor líquido total tem que ser maior que o valor do imposto!");

        if (!VerificaSeValoresTotaisSaoMaioresQueASomaDoValorDeImposto())
            throw new DomainException($"Valor bruto total e valor líquido total que ser maior que o valor da soma dos impostos! Valor bruto:[{NmValorBruto}] Valor líquido:[{NmValorLiquido}] Valor imposto somado:[{ListaDePosicaoImposto.Sum(x => x.NmValorImposto)}]");
    }
    private void ValidaPosicaoInvestimento()
    {
        ValidaPosicao();

        if (!VerificaSeDataFinalEhMaiorOuIgualDataPosicao())
            throw new DomainException($"Data final tem que ser maior ou igual a data de posição! Data final:[{Investimento.DtFinal}] Data posição:[{DtPosicao}]");
    }
    private void VerificaSeCalculaPosicaoInicialOuRecorrente(IEnumerable<ConfiguracaoImposto> listaDeImposto)
    {
        if (VerificaSePossuiPosicaoInicial())
        {
            CalculaPosicaoRecorrenteInvestimento(listaDeImposto);
            return;
        }

        CalculaPosicaoInicialInvestimento(listaDeImposto);
    }
    private bool VerificaSePossuiPosicaoInicial() => IdPosicao > decimal.Zero;
    private bool VerificaSeValorBrutoEhMaiorQueOValorLiquido() => NmValorBruto > NmValorLiquido;
    private bool VerificaSeValorBrutoTotalEhMaiorQueOValorBruto() => NmValorBrutoTotal > NmValorBruto;
    private bool VerificaSeValorBrutoTotalEhMaiorQueOValorLiquidoTotal() => NmValorBrutoTotal > NmValorLiquidoTotal;
    private bool VerificaSeDataFinalEhMaiorOuIgualDataPosicao() => Investimento.DtFinal >= DtPosicao;
    private bool VerificaSeValoresTotaisSaoMaioresQueASomaDoValorDeImposto()
    {
        var valorImpostoSomado = ListaDePosicaoImposto.Sum(x => x.NmValorImposto);
        return NmValorBrutoTotal > valorImpostoSomado && NmValorLiquidoTotal > valorImpostoSomado;
    }
    private bool VerificaSeValoresTotaisSaoMaioresQueOValorImposto() => ListaDePosicaoImposto.All(x => NmValorLiquidoTotal > x.NmValorImposto && NmValorBrutoTotal > x.NmValorImposto);
    private bool VerificaSeValorLiquidoTotalEhMaiorQueOValorLiquido() => NmValorLiquidoTotal > NmValorLiquido;
}