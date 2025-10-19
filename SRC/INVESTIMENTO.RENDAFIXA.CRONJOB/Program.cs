using INVESTIMENTO.RENDAFIXA.CRONJOB.Configuracao;
using System.Reflection;

var builder = Host.CreateApplicationBuilder(args);

InjecaoDeDependencia.AdicionaInjecaoDeDependencia(builder);

//if (!builder.Environment.IsProduction())
builder.Services.AddWindowsService();
//else
//    builder.Services.AddSystemd();

var host = builder.Build();

var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();

var nomeDoProjeto = Assembly.GetExecutingAssembly().GetName().Name;

lifetime.ApplicationStarted.Register(() =>
{
    var logger = host.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("{new}{app} - Aplicação iniciada! Ambiente: {env}{new}", Environment.NewLine, nomeDoProjeto, builder.Environment.EnvironmentName, Environment.NewLine);
});

lifetime.ApplicationStopping.Register(() =>
{
    var logger = host.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("{new}{app} - Aplicação está parando!{new}", Environment.NewLine, nomeDoProjeto, Environment.NewLine);
});

lifetime.ApplicationStopped.Register(() =>
{
    var logger = host.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("{new}{app} - Aplicação finalizada!{new}", Environment.NewLine, nomeDoProjeto, Environment.NewLine);
});

host.Run();