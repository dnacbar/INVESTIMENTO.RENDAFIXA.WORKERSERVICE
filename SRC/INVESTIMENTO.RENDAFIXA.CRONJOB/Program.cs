using INVESTIMENTO.RENDAFIXA.CRONJOB.CronJob;
using Quartz;

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
    var rendimentoDataAtualJobKey = new JobKey("rendimentoDataAtualJobKey", "aplicaRendimentoGroup");
    var rendimentoComErroJobKey = new JobKey("rendimentoComErroJobKey", "aplicaRendimentoGroup");

    x.AddJob<ServicoCronJobQueConsultaEAplicaRendimentoDiario>(x => x.WithIdentity(rendimentoDiarioJobKey));
    x.AddJob<ServicoCronJobQueConsultaEAplicaRendimentoSomenteNaDataAtual>(x => x.WithIdentity(rendimentoDataAtualJobKey));
    x.AddJob<ServicoCronJobQueConsultaEAplicaRendimentoComErro>(x => x.WithIdentity(rendimentoComErroJobKey));

    var verificaSeEstaNoAmbienteDeProducao = builder.Environment.IsProduction();

    var configurationSection = new ConfiguraCronJobAplicaRendimento();

    builder.Configuration.GetSection("ConfiguraCronJobAplicaRendimento").Bind(configurationSection);

    x.AddTrigger(x => x
        .ForJob(rendimentoDiarioJobKey)
        .WithIdentity("rendimentoDiarioJobKey", "aplicaRendimentoGroup")
        .StartNow()
        .WithCronSchedule(configurationSection.Diario));

    x.AddTrigger(x => x
        .ForJob(rendimentoDataAtualJobKey)
        .WithIdentity("rendimentoDataAtualJobKey", "aplicaRendimentoGroup")
        .StartNow()
        .WithCronSchedule(configurationSection.DataAtual));

    x.AddTrigger(x => x
        .ForJob(rendimentoComErroJobKey)
        .WithIdentity("rendimentoComErroJobKey", "aplicaRendimentoGroup")
        .StartNow()
        .WithCronSchedule(configurationSection.Erro));
});

builder.Services.AddQuartzHostedService(x => { x.WaitForJobsToComplete = true; });

var host = builder.Build();
host.Run();


class ConfiguraCronJobAplicaRendimento
{
    public string Diario { get; set; } = string.Empty;
    public string DataAtual { get; set; } = string.Empty;
    public string Erro { get; set; } = string.Empty;
}