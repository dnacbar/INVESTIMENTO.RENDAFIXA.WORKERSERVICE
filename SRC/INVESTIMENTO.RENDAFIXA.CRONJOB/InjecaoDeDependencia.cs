using INVESTIMENTO.RENDAFIXA.CRONJOB.CronJob;
using INVESTIMENTO.RENDAFIXA.CRONJOB.Servico;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Consulta;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Manipula;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Imposto.BancoDeDados.Consulta;
using INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Financeiro.BancoDeDados.Consulta;
using INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Financeiro.BancoDeDados.Manipula;
using INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Imposto.BancoDeDados.Consulta;
using Quartz;
using System.Data;
using System.Data.SqlClient;

namespace INVESTIMENTO.RENDAFIXA.CRONJOB;

public static class InjecaoDeDependencia
{
    public static void AdicionaInjecaoDeDependencia(IHostApplicationBuilder builder)
    {
        ConfiguraBancoDeDados(builder.Services);
        ConfiguraDbConnection(builder);
        ConfiguraQuartz(builder);
        ConfiguraServicoCronJob(builder.Services);
        builder.Services.AddLogging(b => b.AddConsole());
    }

    public static void ConfiguraBancoDeDados(IServiceCollection service)
    {
        //Consulta
        service.AddSingleton<IServicoQueListaConfiguracaoImposto, ServicoQueListaConfiguracaoImposto>();
        service.AddSingleton<IServicoQueListaInvestimentoSemBloqueio, ServicoQueListaInvestimentoSemBloqueio>();
        service.AddSingleton<IServicoQueObtemAPosicaoDoInvestimento, ServicoQueObtemAPosicaoDoInvestimento>();

        //Manipula
        service.AddSingleton<IServicoQueAdicionaOuAtualizaPosicaoImpostoInvestimento, ServicoQueAdicionaOuAtualizaPosicaoImpostoInvestimento>();
        service.AddSingleton<IServicoQueAdicionaOuAtualizaPosicaoInvestimento, ServicoQueAdicionaOuAtualizaPosicaoInvestimento>();
        service.AddSingleton<IServicoQueAtualizaInvestimento, ServicoQueAtualizaInvestimento>();   
    }

    public static void ConfiguraDbConnection(IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IDbConnection>(x => new SqlConnection(builder.Configuration.GetConnectionString("DBRENDAFIXA")));
    }

    public static void ConfiguraQuartz(IHostApplicationBuilder builder)
    {
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

            x.AddJob<CronJobAplicaRendimentoDiario>(x => x.WithIdentity(rendimentoDiarioJobKey));
            x.AddJob<CronJobConsultaEAplicaRendimentoComErro>(x => x.WithIdentity(rendimentoComErroJobKey));

            var verificaSeEstaNoAmbienteDeProducao = builder.Environment.IsProduction();

            var configurationSection = new ConfiguraCronJob();

            builder.Configuration.GetSection("ConfiguraCronJobAplicaRendimento").Bind(configurationSection);

            x.AddTrigger(x => x
                .ForJob(rendimentoDiarioJobKey)
                .WithIdentity("rendimentoDiarioJobKey", "aplicaRendimentoGroup")
                .StartNow()
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(200).RepeatForever()));
                //.WithCronSchedule(configurationSection.Diario));

            x.AddTrigger(x => x
                .ForJob(rendimentoComErroJobKey)
                .WithIdentity("rendimentoComErroJobKey", "aplicaRendimentoGroup")
                .StartNow()
                .WithCronSchedule(configurationSection.Erro));
        });
        builder.Services.AddQuartzHostedService(x => { x.WaitForJobsToComplete = true; });
    }

    public static void ConfiguraServicoCronJob(IServiceCollection service)
    {
        service.AddSingleton<AplicaORendimentoNaPosicaoDeHoje>();
        service.AddSingleton<ConsultaAConfiguracaoDoImposto>();
    }
}