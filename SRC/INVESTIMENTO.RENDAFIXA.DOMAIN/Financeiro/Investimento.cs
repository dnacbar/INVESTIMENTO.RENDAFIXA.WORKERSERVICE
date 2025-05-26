using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Imposto;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Imposto.Enum;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Indice;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Indice.Enum;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro;

public class Investimento
{
    private const byte TamanhoDocumentoPessoaJuridica = 14;
    private const byte TamanhoDocumentoPessoaFisica = 11;

    public Investimento(Indexador indexador, Guid idInvestimento, Guid idInvestidor, string txDocumentoFederal, decimal nmValorInicial, decimal nmValorFinal, decimal nmValorImposto,
        decimal nmTaxaRendimento, decimal nmTaxaAdicional, DateTime dtInicial, DateTime dtFinal, bool boLiquidado, bool boIsentoImposto)
    {
        Indexador = indexador;
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
        IdIndexador = (EnumIndexador)indexador.IdIndexador;
        BoLiquidado = boLiquidado;
        BoIsentoImposto = boIsentoImposto;

        ValidaInvestimento();
    }

    public virtual Indexador Indexador { get; }
    public virtual Posicao Posicao { get; private set; } = null!;

    public Guid IdInvestimento { get; }
    public Guid IdInvestidor { get; }
    public string TxDocumentoFederal { get; } = string.Empty;
    public decimal NmValorInicial { get; }
    public decimal NmValorFinal { get; private set; }
    public decimal NmValorImposto { get; private set; }
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
    public string? TxUsuarioAtualizacao { get; private set; }
    public DateTime? DtAtualizacao { get; private set; }

    public void AdicionaCalculoPosicaoNoInvestimento(Posicao posicao)
    {
        NmValorFinal = posicao.NmValorLiquidoTotal;
        NmValorImposto = posicao.ListaDePosicaoImposto.Sum(x => x.NmValorImposto);
        TxUsuarioAtualizacao = "DN";
        DtAtualizacao = DateTime.Now;
    }

    public decimal CalculaValorTaxaDiaria() => CalculaValorTaxaAnual() / 36000;
    public bool VerificaSeCalculaIof() => (DateTime.Today.ToUniversalTime() - DtInicial.Date.ToUniversalTime()).Days < 30;
    public bool VerificaSeEhPreFixado() => IdIndexador == EnumIndexador.Pre;
    public bool VerificaSeInvestimentoEhIsentoDeImposto() => BoIsentoImposto;

    private decimal CalculaValorTaxaAnual() => Indexador.NmRendimento / 100 * NmTaxaRendimento + NmTaxaAdicional;
    private void ValidaInvestimento()
    {
        if (!VerificaSeCodigoInvestimentoEstaPreenchido())
            throw new DomainException($"Código investimento tem que ser preenchido! Código investimento:[{IdInvestimento}]");

        if (!VerificaSeCodigoInvestidorEstaPreenchido())
            throw new DomainException($"Código investidor tem que ser preenchido! Código investidor:[{IdInvestidor}]");

        if (!VerificaSeDataInvestimentoEstaValida())
            throw new DomainException($"Data inicial tem que ser menor ou igual a data de hoje e menor do que a data final! Data inicial:[{DtInicial}] Data final:[{DtFinal}]");

        if (!VerificaSeValorInicialFinalEstaPositivo())
            throw new DomainException($"Valor inicial ou valor final que ser positivo! Valor inicial:[{NmValorInicial}] Valor final:[{NmValorFinal}]");

        if (!VerificaSeValorInicialEhMenorOuIgualValorFinal())
            throw new DomainException($"Valor inicial tem que ser menor ou igual ao valor final! Valor inicial:[{NmValorInicial}] Valor final:[{NmValorFinal}]");

        if (!VerificaSeValorImpostoEstaPositivo())
            throw new DomainException($"Valor imposto que ser positivo! Valor imposto:[{NmValorImposto}]");

        if (!VerificaSeValorTaxaRendimentoEhMaiorQueZero())
            throw new DomainException($"Valor taxa rendimento tem que ser maior que zero! Valor imposto:[{NmTaxaRendimento}]");

        if (!VerificaSeValorTaxaAdicionalEstaPositiva())
            throw new DomainException($"Valor taxa adicional tem que ser positiva! Valor imposto:[{NmTaxaAdicional}]");

        if (!VerificaSeDocumentoEstaValido())
            throw new DomainException($"Documento federal que ser uma numeração válida e tamanho valido! Documento federal:[{TxDocumentoFederal}]");
    }
    private bool VerificaSeCodigoInvestidorEstaPreenchido() => IdInvestidor != Guid.Empty;
    private bool VerificaSeCodigoInvestimentoEstaPreenchido() => IdInvestimento != Guid.Empty;
    private bool VerificaSeDataInvestimentoEstaValida() => DtInicial <= DateTime.Today && DtInicial < DtFinal;
    private bool VerificaSeDocumentoEstaValido() =>
        ulong.TryParse(TxDocumentoFederal, out _) &&
        (TxDocumentoFederal.Length == TamanhoDocumentoPessoaFisica ||
         TxDocumentoFederal.Length == TamanhoDocumentoPessoaJuridica);
    private bool VerificaSeValorImpostoEstaPositivo() => NmValorImposto >= decimal.Zero;
    private bool VerificaSeValorInicialEhMenorOuIgualValorFinal() => NmValorInicial <= NmValorFinal;
    private bool VerificaSeValorInicialFinalEstaPositivo() => NmValorInicial >= decimal.Zero && NmValorFinal >= decimal.Zero;
    private bool VerificaSeValorTaxaAdicionalEstaPositiva() => NmTaxaAdicional >= decimal.Zero;
    private bool VerificaSeValorTaxaRendimentoEhMaiorQueZero() => NmTaxaRendimento > decimal.Zero;


    private DateTime _dtFinal;
    private EnumIndexador _idIndexador;
    private string _txUsuario = string.Empty;
}
