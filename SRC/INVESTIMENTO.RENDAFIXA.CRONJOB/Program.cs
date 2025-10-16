using INVESTIMENTO.RENDAFIXA.CRONJOB.Configuracao;

var builder = Host.CreateApplicationBuilder(args);

InjecaoDeDependencia.AdicionaInjecaoDeDependencia(builder);

if (!builder.Environment.IsProduction())
    builder.Services.AddWindowsService();
else
    builder.Services.AddSystemd();

var host = builder.Build();
host.Run();