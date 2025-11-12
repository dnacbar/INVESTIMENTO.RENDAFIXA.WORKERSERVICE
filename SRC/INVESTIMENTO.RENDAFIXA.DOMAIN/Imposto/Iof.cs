namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Imposto;

public class Iof(byte idImposto, byte idConfiguracaoImposto, decimal nmRendimento, short nmDiasUteis) : ConfiguracaoImposto(idImposto, idConfiguracaoImposto, nmRendimento, nmDiasUteis)
{
    public const int DiasCorridosParaIsencao = 30;
}