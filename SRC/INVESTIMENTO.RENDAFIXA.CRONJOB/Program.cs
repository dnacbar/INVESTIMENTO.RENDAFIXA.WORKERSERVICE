using INVESTIMENTO.RENDAFIXA.CRONJOB.Configuracao;

var builder = Host.CreateApplicationBuilder(args);

InjecaoDeDependencia.AdicionaInjecaoDeDependencia(builder);

var host = builder.Build();
host.Run();


class ConfiguraCronJob
{
    public string Diario { get; set; } = string.Empty;
    public string Erro { get; set; } = string.Empty;
}