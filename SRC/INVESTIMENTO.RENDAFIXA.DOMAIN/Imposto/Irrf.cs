namespace INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Imposto;

public class Irrf(byte idImposto, byte idConfiguracaoImposto, decimal nmRendimento, short nmDiasUteis) : ConfiguracaoImposto(idImposto, idConfiguracaoImposto, nmRendimento, nmDiasUteis)
{
    public const int DiasUteisParaMenorAliquota = 721;
}