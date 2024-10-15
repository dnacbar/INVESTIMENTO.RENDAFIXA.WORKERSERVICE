using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Consulta;

namespace INVESTIMENTO.RENDAFIXA.CRONJOB.Servico;

public class ServicoQueAplicaRendimentoNaPosicaoDeHoje(IServicoQueListaInvestimentoSemBloqueio servicoQueListaInvestimentoSemBloqueio)
{
    private readonly IServicoQueListaInvestimentoSemBloqueio _servicoQueListaInvestimentoSemBloqueio = servicoQueListaInvestimentoSemBloqueio;

    public Task ExecutaAsync()
    {
        return Task.CompletedTask;
    }
}