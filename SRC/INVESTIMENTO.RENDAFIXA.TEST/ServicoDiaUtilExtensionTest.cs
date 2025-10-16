using INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.DataHora.Servico;

namespace INVESTIMENTO.RENDAFIXA.TEST
{
    public class ServicoDiaUtilExtensionTest
    {
        [Fact]
        public void DeveCalcularDiasUteisEntreDuasDatas()
        {
            // Prepara��o
            var dataInicial = new DateTime(2025, 5, 5);
            var dataFinal = new DateTime(2025, 5, 26);
            var diasUteisEsperados = 16; // Considerando que finais de semana n�o s�o dias �teis

            // Execu��o
            var diasUteisCalculados = ServicoDiaUtilExtension.CalculaDiaUtilEntreDatas(dataInicial, dataFinal);

            // Valida��o
            Assert.Equal(diasUteisEsperados, diasUteisCalculados);
        }

        [Fact]
        public void DeveRetornarUmQuandoMesmoDiaUtil()
        {
            // Prepara��o
            var data = new DateTime(2025, 5, 5); // Segunda-feira

            // Execu��o
            var diasUteisCalculados = ServicoDiaUtilExtension.CalculaDiaUtilEntreDatas(data, data);

            // Valida��o
            Assert.Equal(1, diasUteisCalculados);
        }

        [Fact]
        public void DeveRetornarZeroQuandoApenasFinaisDeSemana()
        {
            // Prepara��o
            var dataInicial = new DateTime(2025, 5, 3); // S�bado
            var dataFinal = new DateTime(2025, 5, 4);   // Domingo

            // Execu��o
            var diasUteisCalculados = ServicoDiaUtilExtension.CalculaDiaUtilEntreDatas(dataInicial, dataFinal);

            // Valida��o
            Assert.Equal(0, diasUteisCalculados);
        }

        [Fact]
        public void DeveCalcularDiasUteisEntreMeses()
        {
            // Prepara��o
            var dataInicial = new DateTime(2025, 5, 30);
            var dataFinal = new DateTime(2025, 6, 3);
            var diasUteisEsperados = 3; // 30/05 (sexta), 02/06 (segunda), 03/06 (ter�a)

            // Execu��o
            var diasUteisCalculados = ServicoDiaUtilExtension.CalculaDiaUtilEntreDatas(dataInicial, dataFinal);

            // Valida��o
            Assert.Equal(diasUteisEsperados, diasUteisCalculados);
        }

        [Fact]
        public void DeveRetornarExcecaoQuandoDataFinalMenorQueInicial()
        {
            // Prepara��o
            var dataInicial = new DateTime(2025, 5, 5);
            var dataFinal = new DateTime(2025, 5, 4);

            // Execu��o
            Assert.Throws<ArgumentException>(() => ServicoDiaUtilExtension.CalculaDiaUtilEntreDatas(dataInicial, dataFinal));
        }

        [Fact]
        public void DeveCalcularDiasUteisSemanaCompleta()
        {
            // Prepara��o
            var dataInicial = new DateTime(2025, 5, 5);  // Segunda-feira
            var dataFinal = new DateTime(2025, 5, 9);    // Sexta-feira
            var diasUteisEsperados = 5;

            // Execu��o
            var diasUteisCalculados = ServicoDiaUtilExtension.CalculaDiaUtilEntreDatas(dataInicial, dataFinal);

            // Valida��o
            Assert.Equal(diasUteisEsperados, diasUteisCalculados);
        }
    }
}