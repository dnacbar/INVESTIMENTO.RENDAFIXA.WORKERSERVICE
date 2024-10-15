using INVESTIMENTO.RENDAFIXA.CRONJOB.CronJob;
using Quartz;
using System.Data.SqlClient;
using System.Data;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<QuartzOptions>(x =>
{
    x.Scheduling.IgnoreDuplicates = true;
    x.Scheduling.OverWriteExistingData = true;
});

builder.Services.AddQuartz(x =>
{
    x.SchedulerId = "RendaFixaCronJobId";
    x.SchedulerName = "RendaFixaCronJobName";

    var rendimentoDiarioJobKey = new JobKey("rendimentoDiarioJobKey", "aplicaRendimentoGroup");
    var rendimentoComErroJobKey = new JobKey("rendimentoComErroJobKey", "aplicaRendimentoGroup");

    x.AddJob<CronJobQueAplicaRendimentoDiario>(x => x.WithIdentity(rendimentoDiarioJobKey));
    x.AddJob<CronJobQueConsultaEAplicaRendimentoComErro>(x => x.WithIdentity(rendimentoComErroJobKey));

    var verificaSeEstaNoAmbienteDeProducao = builder.Environment.IsProduction();

    var configurationSection = new ConfiguraCronJob();

    builder.Configuration.GetSection("ConfiguraCronJobAplicaRendimento").Bind(configurationSection);

    x.AddTrigger(x => x
        .ForJob(rendimentoDiarioJobKey)
        .WithIdentity("rendimentoDiarioJobKey", "aplicaRendimentoGroup")
        .StartNow()
        .WithCronSchedule(configurationSection.Diario));


    x.AddTrigger(x => x
        .ForJob(rendimentoComErroJobKey)
        .WithIdentity("rendimentoComErroJobKey", "aplicaRendimentoGroup")
        .StartNow()
        .WithCronSchedule(configurationSection.Erro));
});

builder.Services.AddSingleton<IDbConnection>(x => new SqlConnection(builder.Configuration.GetConnectionString("DBRENDAFIXA")));

builder.Services.AddQuartzHostedService(x => { x.WaitForJobsToComplete = true; });

var host = builder.Build();
host.Run();


class ConfiguraCronJob
{
    public string Diario { get; set; } = string.Empty;
    public string Erro { get; set; } = string.Empty;
}