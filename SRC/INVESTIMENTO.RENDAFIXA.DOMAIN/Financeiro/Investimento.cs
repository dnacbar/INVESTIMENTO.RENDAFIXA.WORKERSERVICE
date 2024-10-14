using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Indice;

namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro;

public class Investimento
{
    private const byte TamanhoDocumentoPessoaJuridica = 14;
    private const byte TamanhoDocumentoPessoaFisica = 11;

    public Investimento(Guid idInvestidor, string txDocumentoFederal, decimal nmValorInicial, decimal nmTaxarendimento,
        decimal nmTaxaAdicional, DateTime dtInicial, DateTime dtFinal, EnumIndexador idIndexador, bool boIsentoImposto, string txUsuario)
    {
        IdInvestimento = Guid.NewGuid();
        IdInvestidor = idInvestidor;
        TxDocumentoFederal = txDocumentoFederal;
        NmValorInicial = nmValorInicial;
        NmValorFinal = NmValorInicial;
        NmTaxaRendimento = nmTaxarendimento;
        NmTaxaAdicional = nmTaxaAdicional;
        DtInicial = dtInicial;
        DtFinal = dtFinal;
        IdIndexador = (EnumIndexador)idIndexador;
        BoIsentoImposto = boIsentoImposto;

        TxUsuario = txUsuario;

        Movimentacao = new Movimentacao(this);
        BoLiquidado = Movimentacao.VerificaSeValorLiquidoTotalEstaZerado();
    }

    public Investimento(Guid idInvestimento, Guid idInvestidor, string txDocumentoFederal, decimal nmValorInicial, decimal nmValorFinal, decimal nmValorImposto,
        decimal nmTaxaRendimento, decimal nmTaxaAdicional, DateTime dtInicial, DateTime dtFinal, byte idIndexador, bool boLiquidado, bool boIsentoImposto)
    {
        IdInvestimento = idInvestimento;
        IdInvestidor = idInvestidor;
        TxDocumentoFederal = txDocumentoFederal;
        NmValorInicial = nmValorInicial;
        NmValorFinal = nmValorFinal;
        NmValorImposto = nmValorImposto;
        NmTaxaRendimento = nmTaxaRendimento;
        NmTaxaAdicional = nmTaxaAdicional;
        DtInicial = dtInicial;
        DtFinal = dtFinal;
        IdIndexador = (EnumIndexador)idIndexador;
        BoLiquidado = boLiquidado;
        BoIsentoImposto = boIsentoImposto;

        ValidaInvestimento();
    }

    public Investimento(Movimentacao movimentacao, Guid idInvestidor, string txDocumentoFederal, decimal nmValorInicial, decimal nmValorFinal, decimal nmValorImposto,
        decimal nmTaxaRendimento, decimal nmTaxaAdicional, DateTime dtInicial, DateTime dtFinal, byte idIndexador, bool boLiquidado, bool boIsentoImposto)
    {
        IdInvestimento = movimentacao.IdInvestimento;
        IdInvestidor = idInvestidor;
        TxDocumentoFederal = txDocumentoFederal;
        NmValorInicial = nmValorInicial;
        NmValorFinal = nmValorFinal;
        NmValorImposto = nmValorImposto;
        NmTaxaRendimento = nmTaxaRendimento;
        NmTaxaAdicional = nmTaxaAdicional;
        DtInicial = dtInicial;
        DtFinal = dtFinal;
        IdIndexador = (EnumIndexador)idIndexador;
        BoLiquidado = boLiquidado;
        BoIsentoImposto = boIsentoImposto;

        Movimentacao = movimentacao;

        ValidaInvestimentoMovimentacao();
    }

    public Investimento(Guid idInvestimento, Guid idInvestidor, string txDocumentoFederal, short idMovimentacao)
    {
        Movimentacao = new Movimentacao(idInvestimento, idMovimentacao);
        IdInvestidor = idInvestidor;
        TxDocumentoFederal = txDocumentoFederal;
    }

    public Investimento(Guid idInvestimento, Guid idInvestidor, string txDocumentoFederal)
    {
        IdInvestimento = idInvestimento;
        IdInvestidor = idInvestidor;
        TxDocumentoFederal = txDocumentoFederal;
    }

    public virtual Movimentacao Movimentacao { get; } = null!;

    public Guid IdInvestimento { get; }
    public Guid IdInvestidor { get; }
    public string TxDocumentoFederal { get; } = string.Empty;
    public decimal NmValorInicial { get; }
    public decimal NmValorFinal { get; }
    public decimal NmValorImposto { get; }
    public decimal NmTaxaRendimento { get; }
    public decimal NmTaxaAdicional { get; private set; }
    public DateTime DtInicial { get; }
    public DateTime DtFinal
    {
        get
        { return _dtFinal; }
        set
        {
            _dtFinal = value;
            BoLiquidado = DateTime.Today >= _dtFinal.Date;
        }
    }
    public EnumIndexador IdIndexador
    {
        get
        { return _idIndexador; }
        private set
        {
            _idIndexador = value;
            NmTaxaAdicional = _idIndexador == EnumIndexador.Pre ? decimal.Zero : NmTaxaAdicional;
        }
    }
    public bool BoLiquidado { get; private set; }
    public bool BoIsentoImposto { get; }
    public string TxUsuario
    {
        get { return _txUsuario; }
        private set { _txUsuario = string.IsNullOrEmpty(value) ? "DN" : value; }
    }
    public DateTime DtCriacao { get; }
    public string? TxUsuarioAtualizacao { get; }
    public DateTime? DtAtualizacao { get; }

    public bool VerificaSeEhPreFixado() => IdIndexador == EnumIndexador.Pre;

    private void ValidaInvestimento()
    {
        if (!VerificaSeCodigoInvestimentoEstaPreenchido())
            throw new BadRequestException($"Código investimento tem que ser preenchido! Código investimento:[{IdInvestimento}]");

        if (!VerificaSeCodigoInvestidorEstaPreenchido())
            throw new BadRequestException($"Código investidor tem que ser preenchido! Código investidor:[{IdInvestidor}]");

        if (!VerificaSeDataInvestimentoEstaValida())
            throw new BadRequestException($"Data inicial tem que ser menor ou igual a data de hoje e menor do que a data final! Data inicial:[{DtFinal}] Data final:[{DtInicial}]");

        if (!VerificaSeValorInicialFinalEstaPositivo())
            throw new BadRequestException($"Valor inicial ou valor final que ser positivo! Valor inicial:[{NmValorInicial}] Valor final:[{NmValorFinal}]");

        if (!VerificaSeValorInicialEhMenorOuIgualValorFinal())
            throw new BadRequestException($"Valor inicial tem que ser menor ou igual ao valor final! Valor inicial:[{NmValorInicial}] Valor final:[{NmValorFinal}]");

        if (!VerificaSeValorImpostoEstaPositivo())
            throw new BadRequestException($"Valor imposto que ser positivo! Valor imposto:[{NmValorImposto}]");

        if (!VerificaSeValorTaxaRendimentoEhMaiorQueZero())
            throw new BadRequestException($"Valor taxa rendimento tem que ser maior que zero! Valor imposto:[{NmTaxaRendimento}]");

        if (!VerificaSeValorTaxaAdicionalEstaPositiva())
            throw new BadRequestException($"Valor taxa adicional tem que ser positiva! Valor imposto:[{NmTaxaAdicional}]");

        if (!VerificaSeDocumentoEstaValido())
            throw new BadRequestException($"Documento federal que ser uma numeração válida e tamanho valido! Documento federal:[{TxDocumentoFederal}]");
    }
    private void ValidaInvestimentoMovimentacao()
    {
        ValidaInvestimento();

        if (!VerificaSeValorFinalEhIgualValorLiquidoTotal())
            throw new BadRequestException($"Valor final tem que ser igual ao valor líquido total! Valor final:[{NmValorFinal}] Valor líquido total:[{Movimentacao.NmValorLiquidoTotal}]");

        if (!VerificaSeDataFinalEhMaiorOuIgualDataMovimentacao())
            throw new BadRequestException($"Data final tem que ser maior ou igual a data de movimentação! Data final:[{DtFinal}] Data movimentação:[{Movimentacao.DtMovimentacao}]");
    }

    private bool VerificaSeCodigoInvestidorEstaPreenchido() => IdInvestidor != Guid.Empty;
    private bool VerificaSeCodigoInvestimentoEstaPreenchido() => IdInvestimento != Guid.Empty;
    private bool VerificaSeDataFinalEhMaiorOuIgualDataMovimentacao() => DtFinal >= Movimentacao.DtMovimentacao;
    private bool VerificaSeDataInvestimentoEstaValida() => DtInicial <= DateTime.Today && DtInicial < DtFinal;
    private bool VerificaSeDocumentoEstaValido() => ulong.TryParse(TxDocumentoFederal, out _) && TxDocumentoFederal.Length == TamanhoDocumentoPessoaFisica || TxDocumentoFederal.Length == TamanhoDocumentoPessoaJuridica;
    private bool VerificaSeValorFinalEhIgualValorLiquidoTotal() => NmValorFinal == Movimentacao.NmValorLiquidoTotal;
    private bool VerificaSeValorImpostoEstaPositivo() => NmValorImposto >= decimal.Zero;
    private bool VerificaSeValorInicialEhMenorOuIgualValorFinal() => NmValorInicial <= NmValorFinal;
    private bool VerificaSeValorInicialFinalEstaPositivo() => NmValorInicial >= decimal.Zero && NmValorFinal >= decimal.Zero;
    private bool VerificaSeValorTaxaAdicionalEstaPositiva() => NmTaxaAdicional >= decimal.Zero;
    private bool VerificaSeValorTaxaRendimentoEhMaiorQueZero() => NmTaxaRendimento > decimal.Zero;

    private DateTime _dtFinal;
    private EnumIndexador _idIndexador;
    private string _txUsuario = string.Empty;
}
