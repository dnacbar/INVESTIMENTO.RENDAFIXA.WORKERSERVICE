﻿using DN.LOG.LIBRARY.MODEL.EXCEPTION;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Imposto;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Imposto.Enum;

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
        IdInvestimento = investimento.IdInvestimento;
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
        IdInvestimento = investimento.IdInvestimento;
        IdPosicao = IdPosicaoInicial;
        DtPosicao = DateTime.Today;
    }

    public ICollection<ImpostoPosicao> ListaDePosicaoImposto { get; set; } = [];
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

    /// <summary>
    /// Calcula a posição do investimento de forma assíncrona, atualizando os valores brutos e líquidos
    /// e aplicando os impostos conforme as regras de negócio.
    /// </summary>
    /// <param name="listaDeImposto">Lista de configurações de impostos a serem aplicados</param>
    /// <param name="token">Token de cancelamento para a operação assíncrona</param>
    /// <returns>Task representando a operação assíncrona de cálculo</returns>
    /// <exception cref="OperationCanceledException">Lançada quando a operação é cancelada</exception>
    /// <exception cref="DomainException">Lançada quando a lista de impostos é nula ou quando as validações falham</exception>
    public Task CalculaPosicaoInvestimentoAsync(IEnumerable<ConfiguracaoImposto> listaDeImposto, CancellationToken token) => Task.Run(() => { VerificaSeCalculaPosicaoInicialOuRecorrente(listaDeImposto); }, token);
    /// <summary>
    /// Verifica se o valor líquido total da posição está zerado.
    /// </summary>
    /// <returns>True se o valor líquido total é zero, False caso contrário</returns>
    public bool VerificaSeValorLiquidoTotalEstaZerado() => NmValorLiquidoTotal == decimal.Zero;

    /// <summary>
    /// Calcula a posição inicial do investimento, aplicando a taxa de rendimento e impostos quando aplicável.
    /// </summary>
    /// <param name="listaDeImposto">Lista de configurações de impostos a serem aplicados</param>
    /// <exception cref="DomainException">Lançada quando a lista de impostos é nula para investimentos não isentos</exception>
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

            ImpostoPosicao.CalculaImposto(this, listaDeImposto);

            var impostoIof = ListaDePosicaoImposto.FirstOrDefault(x => x.IdTipoImposto == EnumTipoImposto.Iof);
            var impostoIrrf = ListaDePosicaoImposto.FirstOrDefault(x => x.IdTipoImposto == EnumTipoImposto.Irrf);

            NmValorLiquido = NmValorBruto - (impostoIof?.NmValorImposto ?? 0) - (impostoIrrf?.NmValorImposto ?? 0);
        }

        NmValorLiquidoTotal = Investimento.NmValorInicial + NmValorLiquido;

        Investimento.AdicionaCalculoPosicaoNoInvestimento(this);

        ValidaPosicaoInvestimento();
    }

    /// <summary>
    /// Calcula a posição recorrente do investimento, atualizando valores e aplicando impostos quando aplicável.
    /// </summary>
    /// <param name="listaDeImposto">Lista de configurações de impostos a serem aplicados</param>
    /// <exception cref="DomainException">Lançada quando a lista de impostos é nula para investimentos não isentos</exception>
    private void CalculaPosicaoRecorrenteInvestimento(IEnumerable<ConfiguracaoImposto> listaDeImposto)
    {
        NmValorBruto = NmValorBruto + NmValorBrutoTotal * Investimento.CalculaValorTaxaDiaria();
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

            ImpostoPosicao.CalculaImposto(this, listaDeImposto);

            var impostoIof = ListaDePosicaoImposto.FirstOrDefault(x => x.IdTipoImposto == EnumTipoImposto.Iof);
            var impostoIrrf = ListaDePosicaoImposto.FirstOrDefault(x => x.IdTipoImposto == EnumTipoImposto.Irrf);

            NmValorLiquido = NmValorBruto -
                (impostoIof?.NmValorImposto ?? 0) -
                (impostoIrrf?.NmValorImposto ?? 0);
        }

        NmValorLiquidoTotal = Investimento.NmValorInicial + NmValorLiquido;

        Investimento.AdicionaCalculoPosicaoNoInvestimento(this);

        ValidaPosicaoInvestimento();
    }

    /// <summary>
    /// Valida os valores da posição, garantindo que as relações entre valores brutos e líquidos estejam corretas.
    /// </summary>
    /// <exception cref="DomainException">Lançada quando alguma validação de valores falha</exception>
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
    private bool VerificaSeValorBrutoEhMaiorQueOValorLiquido() => NmValorBruto > NmValorLiquido;

    /// <summary>
    /// Verifica se o valor bruto total é maior que o valor bruto.
    /// </summary>
    /// <returns>True se o valor bruto total é maior que o valor bruto, False caso contrário</returns>
    private bool VerificaSeValorBrutoTotalEhMaiorQueOValorBruto() => NmValorBrutoTotal > NmValorBruto;

    /// <summary>
    /// Verifica se o valor bruto total é maior que o valor líquido total.
    /// </summary>
    /// <returns>True se o valor bruto total é maior que o valor líquido total, False caso contrário</returns>
    private bool VerificaSeValorBrutoTotalEhMaiorQueOValorLiquidoTotal() => NmValorBrutoTotal > NmValorLiquidoTotal;

    /// <summary>
    /// Verifica se a data final do investimento é maior ou igual à data da posição.
    /// </summary>
    /// <returns>True se a data final é maior ou igual à data da posição, False caso contrário</returns>
    private bool VerificaSeDataFinalEhMaiorOuIgualDataPosicao() => Investimento.DtFinal >= DtPosicao;

    /// <summary>
    /// Verifica se os valores totais são maiores que a soma dos valores de impostos.
    /// </summary>
    /// <returns>True se os valores totais são maiores que a soma dos impostos, False caso contrário</returns>
    private bool VerificaSeValoresTotaisSaoMaioresQueASomaDoValorDeImposto()
    {
        var valorImpostoSomado = ListaDePosicaoImposto.Sum(x => x.NmValorImposto);
        return NmValorBrutoTotal > valorImpostoSomado && NmValorLiquidoTotal > valorImpostoSomado;
    }

    /// <summary>
    /// Verifica se os valores totais são maiores que cada valor individual de imposto.
    /// </summary>
    /// <returns>True se os valores totais são maiores que cada imposto individual, False caso contrário</returns>
    private bool VerificaSeValoresTotaisSaoMaioresQueOValorImposto() => ListaDePosicaoImposto.All(x => NmValorLiquidoTotal > x.NmValorImposto && NmValorBrutoTotal > x.NmValorImposto);

    /// <summary>
    /// Verifica se o valor líquido total é maior que o valor líquido.
    /// </summary>
    /// <returns>True se o valor líquido total é maior que o valor líquido, False caso contrário</returns>
    private bool VerificaSeValorLiquidoTotalEhMaiorQueOValorLiquido() => NmValorLiquidoTotal > NmValorLiquido;
}