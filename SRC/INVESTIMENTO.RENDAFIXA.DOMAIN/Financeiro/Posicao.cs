using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Imposto;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Imposto.Enum;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Indice;

namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro;

public class Posicao
{
    private const byte IdPosicaoInicial = 1;

    /// <summary>
    /// Inicializa uma nova posição para um investimento existente com valores específicos.
    /// </summary>
    /// <param name="investimento">O investimento associado à posição</param>
    /// <param name="idPosicao">Identificador sequencial da posição</param>
    /// <param name="dtPosicao">Data da posição</param>
    /// <param name="nmValorBrutoTotal">Valor bruto total acumulado</param>
    /// <param name="nmValorLiquidoTotal">Valor líquido total acumulado</param>
    /// <param name="nmValorBruto">Valor bruto do rendimento</param>
    /// <param name="nmValorLiquido">Valor líquido do rendimento</param>
    /// <exception cref="DomainException">Lançada quando as validações de valores falham</exception>
    public Posicao(Investimento investimento, short idPosicao, decimal nmValorBrutoTotal, decimal nmValorLiquidoTotal, decimal nmValorBruto, decimal nmValorLiquido)
    {
        IdPosicao = ++idPosicao;
        DtPosicao = DateTime.Today;
        NmValorBrutoTotal = nmValorBrutoTotal;
        NmValorLiquidoTotal = nmValorLiquidoTotal;
        NmValorBruto = nmValorBruto;
        NmValorLiquido = nmValorLiquido;
        TxUsuario = investimento.TxUsuario;
        DtCriacao = investimento.DtCriacao;

        Investimento = investimento;

        ValidaPosicao();
    }

    /// <summary>
    /// Inicializa uma nova posição inicial para um investimento.
    /// </summary>
    /// <param name="investimento">O investimento para criar a posição inicial</param>
    public Posicao(Investimento investimento)
    {
        Investimento = investimento ?? throw new DomainException("Investimento tem que estar preenchido!");
        IdPosicao = IdPosicaoInicial;
        DtPosicao = DateTime.Today;
    }

    public ImpostoPosicao ImpostoPosicao { get; private set; } = new();
    public Investimento Investimento { get; } = null!;

    public short IdPosicao { get; }
    public DateTime DtPosicao { get; }
    public decimal NmValorBrutoTotal { get; set; }
    public decimal NmValorLiquidoTotal { get; set; }
    public decimal NmValorBruto { get; set; }
    public decimal NmValorLiquido { get; set; }
    public string TxUsuario { get; } = "WORKERSERVICE";
    public DateTime DtCriacao { get; }

    /// <summary>
    /// Calcula a posição do investimento de forma assíncrona, atualizando os valores brutos e líquidos
    /// e aplicando os impostos conforme as regras de negócio.
    /// </summary>
    /// <param name="listaDeConfiguracaoImposto">Lista de configurações de impostos a serem aplicados</param>
    /// <param name="token">Token de cancelamento para a operação assíncrona</param>
    /// <returns>Task representando a operação assíncrona de cálculo</returns>
    /// <exception cref="OperationCanceledException">Lançada quando a operação é cancelada</exception>
    /// <exception cref="DomainException">Lançada quando a lista de impostos é nula ou quando as validações falham</exception>
    public Task CalculaPosicaoInvestimentoAsync(IEnumerable<ConfiguracaoImposto> listaDeConfiguracaoImposto, CancellationToken token) => Task.Run(() => { VerificaSeCalculaPosicaoInicialOuRecorrente(listaDeConfiguracaoImposto); }, token);

    /// <summary>
    /// Calcula a posição inicial do investimento, aplicando a taxa de rendimento e impostos quando aplicável.
    /// </summary>
    /// <param name="listaDeConfiguracaoImposto">Lista de configurações de impostos a serem aplicados</param>
    /// <exception cref="DomainException">Lançada quando a lista de impostos é nula para investimentos não isentos</exception>
    private void CalculaPosicaoInicialInvestimento(IEnumerable<ConfiguracaoImposto> listaDeConfiguracaoImposto)
    {
        NmValorBruto = Investimento.NmValorInicial * CalculaValorTaxaDiaria();
        NmValorBrutoTotal = Investimento.NmValorInicial + NmValorBruto;

        if (Investimento.VerificaSeInvestimentoEhIsentoDeImposto())
        {
            ImpostoPosicao = new();
            NmValorLiquido = NmValorBruto;
        }
        else
        {
            if (listaDeConfiguracaoImposto == null)
                throw new DomainException($"Lista de configuração de imposto tem que ser preenchida!");

            ImpostoPosicao = new ImpostoPosicao(this, listaDeConfiguracaoImposto);

            NmValorLiquido = NmValorBruto - ImpostoPosicao.NmValorImpostoIof - ImpostoPosicao.NmValorImpostoIrrf;
        }

        NmValorLiquidoTotal = Investimento.NmValorInicial + NmValorLiquido;

        Investimento.AdicionaCalculoPosicaoNoInvestimento(this);

        ValidaPosicaoInvestimento();
    }

    /// <summary>
    /// Calcula a posição recorrente do investimento, atualizando valores e aplicando impostos quando aplicável.
    /// </summary>
    /// <param name="listaDeConfiguracaoImposto">Lista de configurações de impostos a serem aplicados</param>
    /// <exception cref="DomainException">Lançada quando a lista de impostos é nula para investimentos não isentos</exception>
    private void CalculaPosicaoRecorrenteInvestimento(IEnumerable<ConfiguracaoImposto> listaDeConfiguracaoImposto)
    {
        NmValorBruto += NmValorBrutoTotal * CalculaValorTaxaDiaria();
        NmValorBrutoTotal = Investimento.NmValorInicial + NmValorBruto;

        if (Investimento.VerificaSeInvestimentoEhIsentoDeImposto())
        {
            ImpostoPosicao = new();
            NmValorLiquido = NmValorBruto;
        }
        else
        {
            if (listaDeConfiguracaoImposto == null)
                throw new DomainException($"Lista de configuração de imposto tem que ser preenchida!");

            ImpostoPosicao = new ImpostoPosicao(this, listaDeConfiguracaoImposto);

            NmValorLiquido = NmValorBruto - ImpostoPosicao.NmValorImpostoIof - ImpostoPosicao.NmValorImpostoIrrf;
        }

        NmValorLiquidoTotal = Investimento.NmValorInicial + NmValorLiquido;

        Investimento.AdicionaCalculoPosicaoNoInvestimento(this);

        ValidaPosicaoInvestimento();
    }

    /// <summary>
    /// Calcula a taxa anual do investimento considerando o indexador e taxa adicional.
    /// </summary>
    /// <returns>Taxa anual calculada</returns>
    private decimal CalculaValorTaxaAnual()
    {
        if (Investimento.VerificaSeEhPreFixado())
            return Investimento.NmTaxaRendimento / 100;

        return (Investimento.Indexador.NmRendimento + Investimento.NmTaxaAdicional) * Investimento.NmTaxaRendimento / 10000;
    }

    /// <summary>
    /// Calcula a taxa diária do investimento.
    /// </summary>
    /// <returns>Taxa diária calculada com base na taxa anual</returns>
    public decimal CalculaValorTaxaDiaria() => (decimal)Math.Pow(1 + (double)CalculaValorTaxaAnual(), 1D / Investimento.QuantidadeDeDiaUtil) - 1;

    /// <summary>
    /// Valida os valores da posição, garantindo que as relações entre valores brutos e líquidos estejam corretas.
    /// </summary>
    /// <exception cref="DomainException">Lançada quando alguma validação de valores falha</exception>
    private void ValidaPosicao()
    {
        if (!VerificaSeValorBrutoTotalEhMaiorQueOValorLiquidoTotal())
            throw new DomainException($"Valor bruto total tem que ser maior que o valor líquido total! Valor bruto total:[{NmValorBruto}] valor líquido total:[{NmValorLiquido}]");

        if (!VerificaSeValorBrutoTotalEhMaiorQueOValorBruto())
            throw new DomainException($"Valor bruto total tem que ser maior que o valor bruto! Valor bruto total:[{NmValorBrutoTotal}] valor bruto:[{NmValorBruto}]");

        if (!VerificaSeValorLiquidoTotalEhMaiorQueOValorLiquido())
            throw new DomainException($"Valor líquido total tem que ser maior que o valor líquido! Valor líquido total:[{NmValorLiquidoTotal}] valor líquido:[{NmValorLiquido}]");

        if (!VerificaSeValorBrutoEhMaiorQueOValorLiquido())
            throw new DomainException($"Valor bruto tem que ser maior que o valor líquido! Valor bruto:[{NmValorBruto}] valor líquido:[{NmValorLiquido}]");

        if (!VerificaSeValoresTotaisSaoMaioresQueOValorImposto())
            throw new DomainException($"Valor bruto total e valor líquido total tem que ser maior que o valor do imposto!");

        if (!VerificaSeValoresTotaisSaoMaioresQueASomaDoValorDeImposto())
            throw new DomainException($"Valor bruto total e valor líquido total que ser maior que o valor da soma dos impostos! Valor bruto:[{NmValorBruto}] Valor líquido:[{NmValorLiquido}] Valor imposto somado:[{ImpostoPosicao.NmValorImpostoSomado}]");
    }

    /// <summary>
    /// Valida a posição do investimento, incluindo validações de valores e datas.
    /// </summary>
    /// <exception cref="DomainException">Lançada quando alguma validação falha</exception>
    private void ValidaPosicaoInvestimento()
    {
        ValidaPosicao();

        if (!VerificaSeDataFinalEhMaiorOuIgualDataPosicao())
            throw new DomainException($"Data final tem que ser maior ou igual a data de posição! Data final:[{Investimento.DtFinal}] Data posição:[{DtPosicao}]");
    }

    /// <summary>
    /// Determina e executa o cálculo apropriado da posição (inicial ou recorrente).
    /// </summary>
    /// <param name="listaDeImposto">Lista de configurações de impostos a serem aplicados</param>
    private void VerificaSeCalculaPosicaoInicialOuRecorrente(IEnumerable<ConfiguracaoImposto> listaDeImposto)
    {
        if (VerificaSePossuiPosicaoInicial())
        {
            CalculaPosicaoRecorrenteInvestimento(listaDeImposto);
            return;
        }

        CalculaPosicaoInicialInvestimento(listaDeImposto);
    }

    /// <summary>
    /// Verifica se a posição atual já possui uma posição inicial.
    /// </summary>
    /// <returns>True se o ID da posição é maior que zero, False caso contrário</returns>
    private bool VerificaSePossuiPosicaoInicial() => IdPosicao > decimal.One;

    /// <summary>
    /// Verifica se o valor bruto é maior que o valor líquido.
    /// </summary>
    /// <returns>True se o valor bruto é maior que o valor líquido, False caso contrário</returns>
    private bool VerificaSeValorBrutoEhMaiorQueOValorLiquido() => NmValorBruto >= NmValorLiquido;

    /// <summary>
    /// Verifica se o valor bruto total é maior que o valor bruto.
    /// </summary>
    /// <returns>True se o valor bruto total é maior que o valor bruto, False caso contrário</returns>
    private bool VerificaSeValorBrutoTotalEhMaiorQueOValorBruto() => NmValorBrutoTotal > NmValorBruto;

    /// <summary>
    /// Verifica se o valor bruto total é maior que o valor líquido total.
    /// </summary>
    /// <returns>True se o valor bruto total é maior que o valor líquido total, False caso contrário</returns>
    private bool VerificaSeValorBrutoTotalEhMaiorQueOValorLiquidoTotal() => NmValorBrutoTotal >= NmValorLiquidoTotal;

    /// <summary>
    /// Verifica se a data final do investimento é maior ou igual à data da posição.
    /// </summary>
    /// <returns>True se a data final é maior ou igual à data da posição, False caso contrário</returns>
    private bool VerificaSeDataFinalEhMaiorOuIgualDataPosicao() => Investimento.DtFinal >= DtPosicao;

    /// <summary>
    /// Verifica se os valores totais são maiores que a soma dos valores de impostos.
    /// </summary>
    /// <returns>True se os valores totais são maiores que a soma dos impostos, False caso contrário</returns>
    private bool VerificaSeValoresTotaisSaoMaioresQueASomaDoValorDeImposto() => NmValorBrutoTotal > ImpostoPosicao.NmValorImpostoSomado && NmValorLiquidoTotal > ImpostoPosicao.NmValorImpostoSomado;

    /// <summary>
    /// Verifica se os valores totais são maiores que cada valor individual de imposto.
    /// </summary>
    /// <returns>True se os valores totais são maiores que cada imposto individual, False caso contrário</returns>
    private bool VerificaSeValoresTotaisSaoMaioresQueOValorImposto() => NmValorLiquidoTotal > ImpostoPosicao.NmValorImpostoIof && NmValorBrutoTotal > ImpostoPosicao.NmValorImpostoIof;

    /// <summary>
    /// Verifica se o valor líquido total é maior que o valor líquido.
    /// </summary>
    /// <returns>True se o valor líquido total é maior que o valor líquido, False caso contrário</returns>
    private bool VerificaSeValorLiquidoTotalEhMaiorQueOValorLiquido() => NmValorLiquidoTotal > NmValorLiquido;
}