using INVESTIMENTO.RENDAFIXA.DOMAIN.Imposto;

namespace INVESTIMENTO.RENDAFIXA.DOMAIN.Imposto;

public class Irrf(byte idImposto, byte idConfiguracaoImposto, decimal nmRendimento, short nmDiasUteis) : ConfiguracaoImposto(idImposto, idConfiguracaoImposto, nmRendimento, nmDiasUteis)
{
    public const int DiasUteisParaMenorAliquota = 721;
}