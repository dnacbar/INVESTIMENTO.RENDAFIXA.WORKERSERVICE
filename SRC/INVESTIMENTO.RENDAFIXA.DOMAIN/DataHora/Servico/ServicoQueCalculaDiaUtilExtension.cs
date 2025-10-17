namespace INVESTIMENTO.RENDAFIXA.DOMAIN.DataHora.Servico;

public static class ServicoQueCalculaDiaUtilExtension
{
    public static int CalculaDiaUtilEntreDatas(this DateTime dataInicial, DateTime dataFinal)
    {
        if (dataInicial > dataFinal)
            throw new ArgumentException($"A data inicial tem que ser menor que a data final! Data inicial: [{dataInicial}] data final: [{dataFinal}]");

        int totalDias = (dataFinal - dataInicial).Days + 1;
        int semanasCompletas = totalDias / 7;
        int diasUteis = semanasCompletas * 5;

        int diasRestantes = totalDias % 7;
        DateTime dataParcial = dataFinal.AddDays(-diasRestantes + 1);

        for (int i = 0; i < diasRestantes; i++)
        {
            if (dataParcial.DayOfWeek != DayOfWeek.Saturday && dataParcial.DayOfWeek != DayOfWeek.Sunday)
                diasUteis++;

            dataParcial = dataParcial.AddDays(1);
        }

        return diasUteis;
    }
}
