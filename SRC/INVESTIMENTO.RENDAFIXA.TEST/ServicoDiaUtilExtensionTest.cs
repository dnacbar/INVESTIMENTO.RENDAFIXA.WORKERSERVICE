using INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.DataHora.Servico;

namespace INVESTIMENTO.RENDAFIXA.TEST
{
    public class ServicoDiaUtilExtensionTest
    {
        [Fact]
        public void DeveCalcularDiasUteisEntreDuasDatas()
        {
            // Preparação
            var dataInicial = new DateTime(2025, 5, 5);
            var dataFinal = new DateTime(2025, 5, 26);
            var diasUteisEsperados = 16; // Considerando que finais de semana não são dias úteis

            // Execução
            var diasUteisCalculados = ServicoDiaUtilExtension.CalculaDiaUtilEntreDatas(dataInicial, dataFinal);

            // Validação
            Assert.Equal(diasUteisEsperados, diasUteisCalculados);
        }

        [Fact]
        public void DeveRetornarUmQuandoMesmoDiaUtil()
        {
            // Preparação
            var data = new DateTime(2025, 5, 5); // Segunda-feira

            // Execução
            var diasUteisCalculados = ServicoDiaUtilExtension.CalculaDiaUtilEntreDatas(data, data);

            // Validação
            Assert.Equal(1, diasUteisCalculados);
        }

        [Fact]
        public void DeveRetornarZeroQuandoApenasFinaisDeSemana()
        {
            // Preparação
            var dataInicial = new DateTime(2025, 5, 3); // Sábado
            var dataFinal = new DateTime(2025, 5, 4);   // Domingo

            // Execução
            var diasUteisCalculados = ServicoDiaUtilExtension.CalculaDiaUtilEntreDatas(dataInicial, dataFinal);

            // Validação
            Assert.Equal(0, diasUteisCalculados);
        }

        [Fact]
        public void DeveCalcularDiasUteisEntreMeses()
        {
            // Preparação
            var dataInicial = new DateTime(2025, 5, 30);
            var dataFinal = new DateTime(2025, 6, 3);
            var diasUteisEsperados = 3; // 30/05 (sexta), 02/06 (segunda), 03/06 (terça)

            // Execução
            var diasUteisCalculados = ServicoDiaUtilExtension.CalculaDiaUtilEntreDatas(dataInicial, dataFinal);

            // Validação
            Assert.Equal(diasUteisEsperados, diasUteisCalculados);
        }

        [Fact]
        public void DeveRetornarExcecaoQuandoDataFinalMenorQueInicial()
        {
            // Preparação
            var dataInicial = new DateTime(2025, 5, 5);
            var dataFinal = new DateTime(2025, 5, 4);

            // Execução
            Assert.Throws<ArgumentException>(() => ServicoDiaUtilExtension.CalculaDiaUtilEntreDatas(dataInicial, dataFinal));
        }

        [Fact]
        public void DeveCalcularDiasUteisSemanaCompleta()
        {
            // Preparação
            var dataInicial = new DateTime(2025, 5, 5);  // Segunda-feira
            var dataFinal = new DateTime(2025, 5, 9);    // Sexta-feira
            var diasUteisEsperados = 5;

            // Execução
            var diasUteisCalculados = ServicoDiaUtilExtension.CalculaDiaUtilEntreDatas(dataInicial, dataFinal);

            // Validação
            Assert.Equal(diasUteisEsperados, diasUteisCalculados);
        }
    }
}