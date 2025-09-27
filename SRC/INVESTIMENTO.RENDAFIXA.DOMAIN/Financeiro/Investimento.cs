using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Indice;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Indice.Enum;

namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro;

/// <summary>
/// Representa um investimento de renda fixa com suas características, valores e regras de negócio.
/// </summary>
public class Investimento
{
    private const byte TamanhoDocumentoPessoaJuridica = 14;
    private const byte TamanhoDocumentoPessoaFisica = 11;
    private const byte LimiteDeQuantidadeDeDiaParaCalculoDeIof = 30;
    private const int QuantidadeDeDiaUtil = 252;

    /// <summary>
    /// Inicializa uma nova instância de um investimento de renda fixa.
    /// </summary>
    /// <param name="indexador">Indexador do investimento</param>
    /// <param name="idInvestimento">Identificador único do investimento</param>
    /// <param name="idInvestidor">Identificador único do investidor</param>
    /// <param name="txDocumentoFederal">Documento federal (CPF/CNPJ) do investidor</param>
    /// <param name="nmValorInicial">Valor inicial do investimento</param>
    /// <param name="nmValorFinal">Valor final do investimento</param>
    /// <param name="nmValorImposto">Valor do imposto</param>
    /// <param name="nmTaxaRendimento">Taxa de rendimento do investimento</param>
    /// <param name="nmTaxaAdicional">Taxa adicional do investimento</param>
    /// <param name="dtInicial">Data inicial do investimento</param>
    /// <param name="dtFinal">Data final do investimento</param>
    /// <param name="boLiquidado">Indica se o investimento está liquidado</param>
    /// <param name="boIsentoImposto">Indica se o investimento é isento de impostos</param>
    /// <exception cref="DomainException">Lançada quando alguma validação falha</exception>
    public Investimento(Indexador indexador, Guid idInvestimento, Guid idInvestidor, string txDocumentoFederal, decimal nmValorInicial, decimal nmValorFinal, decimal nmValorImposto,
        decimal nmTaxaRendimento, decimal nmTaxaAdicional, DateTime dtInicial, DateTime dtFinal, bool boLiquidado, bool boIsentoImposto)
    {
        Indexador = indexador ?? throw new DomainException("Indexador tem que ser preenchido!");
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

    /// <summary>
    /// Indexador associado ao investimento.
    /// </summary>
    public virtual Indexador Indexador { get; }

    /// <summary>
    /// Posição atual do investimento.
    /// </summary>
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

    /// <summary>
    /// Data final do investimento. Atualiza o status de liquidação quando modificada.
    /// </summary>
    public DateTime DtFinal
    {
        get { return _dtFinal; }
        set
        {
            _dtFinal = value;
            BoLiquidado = DateTime.Today >= _dtFinal.Date;
        }
    }

    /// <summary>
    /// Tipo de indexador do investimento. Quando alterado, ajusta a taxa adicional para zero se for pré-fixado.
    /// </summary>
    public EnumIndexador IdIndexador
    {
        get { return _idIndexador; }
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

    /// <summary>
    /// Adiciona os resultados do cálculo de posição ao investimento.
    /// </summary>
    /// <param name="posicao">Posição calculada do investimento</param>
    public void AdicionaCalculoPosicaoNoInvestimento(Posicao posicao)
    {
        NmValorFinal = posicao.NmValorLiquidoTotal;
        NmValorImposto = posicao.ImpostoPosicao.NmValorImpostoSomado;
    }

    /// <summary>
    /// Calcula a taxa diária do investimento.
    /// </summary>
    /// <returns>Taxa diária calculada com base na taxa anual</returns>
    public decimal CalculaValorTaxaDiaria() => (decimal)Math.Pow(1 + (double)CalculaValorTaxaAnual(), Math.Round(1D / QuantidadeDeDiaUtil, 8)) - 1;

    /// <summary>
    /// Verifica se é necessário calcular IOF com base no período do investimento.
    /// </summary>
    /// <returns>True se o investimento está dentro do período de cobrança de IOF, False caso contrário</returns>
    public bool VerificaSeCalculaIof() => (DateTime.Today.ToUniversalTime() - DtInicial.Date.ToUniversalTime()).Days < LimiteDeQuantidadeDeDiaParaCalculoDeIof;

    /// <summary>
    /// Verifica se o investimento é pré-fixado.
    /// </summary>
    /// <returns>True se o investimento é pré-fixado, False caso contrário</returns>
    public bool VerificaSeEhPreFixado() => IdIndexador == EnumIndexador.Pre;

    /// <summary>
    /// Verifica se o investimento é isento de impostos.
    /// </summary>
    /// <returns>True se o investimento é isento de impostos, False caso contrário</returns>
    public bool VerificaSeInvestimentoEhIsentoDeImposto() => BoIsentoImposto;

    /// <summary>
    /// Calcula a taxa anual do investimento considerando o indexador e taxa adicional.
    /// </summary>
    /// <returns>Taxa anual calculada</returns>
    private decimal CalculaValorTaxaAnual()
    {
        if (VerificaSeEhPreFixado())
            return NmTaxaRendimento / 100;

        return (Indexador.NmRendimento + NmTaxaAdicional) * NmTaxaRendimento / 10000;
    }

    /// <summary>
    /// Realiza todas as validações necessárias para garantir a consistência do investimento.
    /// </summary>
    /// <exception cref="DomainException">Lançada quando alguma validação falha</exception>
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

    /// <summary>
    /// Verifica se o código do investidor está preenchido.
    /// </summary>
    /// <returns>True se o ID do investidor não é vazio, False caso contrário</returns>
    private bool VerificaSeCodigoInvestidorEstaPreenchido() => IdInvestidor != Guid.Empty;

    /// <summary>
    /// Verifica se o código do investimento está preenchido.
    /// </summary>
    /// <returns>True se o ID do investimento não é vazio, False caso contrário</returns>
    private bool VerificaSeCodigoInvestimentoEstaPreenchido() => IdInvestimento != Guid.Empty;

    /// <summary>
    /// Verifica se o valor do imposto está positivo.
    /// </summary>
    /// <returns>True se o valor do imposto é maior ou igual a zero, False caso contrário</returns>
    private bool VerificaSeValorImpostoEstaPositivo() => NmValorImposto >= decimal.Zero;

    /// <summary>
    /// Verifica se o valor inicial é menor ou igual ao valor final do investimento.
    /// </summary>
    /// <returns>True se o valor inicial é menor ou igual ao valor final, False caso contrário</returns>
    private bool VerificaSeValorInicialEhMenorOuIgualValorFinal() => NmValorInicial <= NmValorFinal;

    /// <summary>
    /// Verifica se os valores inicial e final do investimento são positivos.
    /// </summary>
    /// <returns>True se ambos os valores são maiores ou iguais a zero, False caso contrário</returns>
    private bool VerificaSeValorInicialFinalEstaPositivo() => NmValorInicial >= decimal.Zero && NmValorFinal >= decimal.Zero;

    /// <summary>
    /// Verifica se o valor da taxa adicional é positivo.
    /// </summary>
    /// <returns>True se o valor da taxa adicional é maior ou igual a zero, False caso contrário</returns>
    private bool VerificaSeValorTaxaAdicionalEstaPositiva() => NmTaxaAdicional >= decimal.Zero;

    /// <summary>
    /// Verifica se o valor da taxa de rendimento é maior que zero.
    /// </summary>
    /// <returns>True se o valor da taxa de rendimento é maior que zero, False caso contrário</returns>
    private bool VerificaSeValorTaxaRendimentoEhMaiorQueZero() => NmTaxaRendimento > decimal.Zero;

    /// <summary>
    /// Verifica se a data do investimento é válida.
    /// </summary>
    /// <returns>True se a data inicial é menor ou igual a hoje e menor que a data final</returns>
    private bool VerificaSeDataInvestimentoEstaValida() => DtInicial <= DateTime.Today && DtInicial < DtFinal;

    /// <summary>
    /// Verifica se o documento federal (CPF/CNPJ) é válido.
    /// </summary>
    /// <returns>True se o documento é válido, False caso contrário</returns>
    private bool VerificaSeDocumentoEstaValido() =>
        ulong.TryParse(TxDocumentoFederal, out _) &&
        (TxDocumentoFederal.Length == TamanhoDocumentoPessoaFisica ||
         TxDocumentoFederal.Length == TamanhoDocumentoPessoaJuridica);

    private DateTime _dtFinal;
    private EnumIndexador _idIndexador;
    private string _txUsuario = string.Empty;
}
