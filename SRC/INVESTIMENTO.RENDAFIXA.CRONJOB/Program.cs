using INVESTIMENTO.RENDAFIXA.CRONJOB.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<CronJobQueAplicaRendimentoDiario>();

var host = builder.Build();
host.Run();
