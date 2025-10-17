using INVESTIMENTO.RENDAFIXA.CRONJOB.CronJob;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Configuracao;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Consulta;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Manipula;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.Servico;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Imposto.BancoDeDados.Consulta;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Juridico.BancoDeDados.Consulta;
using INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Financeiro.BancoDeDados.Manipula;
using INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Imposto.BancoDeDados.Consulta;
using INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Juridico.BancoDeDados.Consulta;
using Quartz;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace INVESTIMENTO.RENDAFIXA.CRONJOB.Configuracao;

public static class InjecaoDeDependencia
{
    public static void AdicionaInjecaoDeDependencia(IHostApplicationBuilder builder)
    {
        InvestimentoRendaFixaWorkerService investimentoRendaFixaWorkerService;

        if (!builder.Environment.IsProduction())
            investimentoRendaFixaWorkerService = builder.Configuration.GetSection("INVESTIMENTO.RENDAFIXA.WORKERSERVICE").Get<InvestimentoRendaFixaWorkerService>() ?? throw new InvalidCastException("ERRO AO CONVERTER OS PARÂMETROS INICIAIS DA APLICAÇÃO!");
        else
        {
            var criptografado = builder.Configuration.GetSection("INVESTIMENTO.RENDAFIXA.WORKERSERVICE").Get<string>() ?? throw new InvalidOperationException("INVESTIMENTO.RENDAFIXA.WORKERSERVICE");

            const int nativeTagSizeByte = 16;
            var keyBytes = Convert.FromBase64String("UXGoyfisUfj+1C0k+iK+kgaDnNeB7kWoPmDm0pALKCs=");
            var nonceBytes = Convert.FromBase64String("XWtpeXD4XCnCqNoy");
            var tagBytes = Convert.FromBase64String("NpW13DlIyb+6nB+GjQdLbw==");
            var cipherBytes = Convert.FromBase64String(criptografado);
            var plainBytes = new byte[cipherBytes.Length];

            using var aes = new AesGcm(keyBytes, nativeTagSizeByte);

            aes.Decrypt(nonceBytes, cipherBytes, tagBytes, plainBytes);

            investimentoRendaFixaWorkerService = System.Text.Json.JsonSerializer.Deserialize<InvestimentoRendaFixaWorkerService>(Encoding.UTF8.GetString(plainBytes)) ?? throw new CryptographicException("ERRO AO DESCRIPTOGRAFAR OS PARÂMETROS INICIAS DA APLICAÇÃO!");
        }

        ConfiguraDbConnection(builder.Services, investimentoRendaFixaWorkerService.ConnectionString.DBRENDAFIXA);
        ConfiguraBancoDeDados(builder.Services);
        ConfiguraQuartzAdicionaRendimento(builder, investimentoRendaFixaWorkerService.CronJobAdicionaRendimento);
        ConfiguraQuartzResgataLiquidado(builder, investimentoRendaFixaWorkerService.CronJobResgataLiquidado);
        ConfiguraServicoCronJob(builder.Services);

        builder.Services.AddSingleton<IInvestimentoRendaFixaWorkerService>(x =>
        {
            return investimentoRendaFixaWorkerService;
        });
        builder.Services.AddLogging(b => b.AddConsole());
    }

    private static void ConfiguraDbConnection(IServiceCollection services, string connectionString)
    {
        services.AddTransient<IDbConnection>(x => new SqlConnection(connectionString));
    }

    private static void ConfiguraBancoDeDados(IServiceCollection service)
    {
        //Consulta
        service.AddSingleton<IServicoQueConsultaBloqueioInvestimento, ServicoQueConsultaBloqueioInvestimento>();
        service.AddSingleton<IServicoQueConsultaInvestimento, ServicoQueConsultaInvestimento>();
        service.AddSingleton<IServicoQueConsultaPosicaoDoInvestimento, ServicoQueConsultaPosicaoDoInvestimento>();
        service.AddSingleton<IServicoQueConsultaConfiguracaoImposto, ServicoQueConsultaConfiguracaoImposto>();

        //Manipula
        service.AddSingleton<IServicoQueManipulaInvestimento, ServicoQueManipulaInvestimento>();
        service.AddSingleton<IServicoQueManipulaPosicaoInvestimento, ServicoQueManipulaPosicaoInvestimento>();
        service.AddSingleton<IServicoQueManipulaPosicaoImpostoInvestimento, ServicoQueManipulaPosicaoImpostoInvestimento>();
        service.AddSingleton<IServicoQueManipulaResgate, ServicoQueManipulaResgate>();
    }

    private static void ConfiguraQuartzAdicionaRendimento(IHostApplicationBuilder builder, CronJobAdicionaRendimento cronJobAplicaRendimento)
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

            x.AddJob<CronJobConsultaEAdicionaRendimentoDiario>(x => x.WithIdentity(rendimentoDiarioJobKey));
            x.AddJob<CronJobConsultaEAplicaRendimentoComErro>(x => x.WithIdentity(rendimentoComErroJobKey));

            if (builder.Environment.IsDevelopment())
            {
                x.AddTrigger(x => x
                    .ForJob(rendimentoDiarioJobKey)
                    .WithIdentity("rendimentoDiarioJobKey", "aplicaRendimentoGroup")
                    .StartNow()
                     //.WithCronSchedule(configuraCronJobAplicaRendimento.Diario));
                     .WithSimpleSchedule(x => x.WithIntervalInSeconds(200).RepeatForever()));
                
                x.AddTrigger(x => x
                    .ForJob(rendimentoComErroJobKey)
                    .WithIdentity("rendimentoComErroJobKey", "aplicaRendimentoGroup")
                    .StartNow()
                    //.WithCronSchedule(configuraCronJobAplicaRendimento.Erro));
                    .WithSimpleSchedule(x => x.WithIntervalInSeconds(400).RepeatForever()));
            }
            else
            {
                x.AddTrigger(x => x
                    .ForJob(rendimentoDiarioJobKey)
                    .WithIdentity("rendimentoDiarioJobKey", "aplicaRendimentoGroup")
                    .StartNow()
                    .WithCronSchedule(cronJobAplicaRendimento.Diario));

                x.AddTrigger(x => x
                    .ForJob(rendimentoComErroJobKey)
                    .WithIdentity("rendimentoComErroJobKey", "aplicaRendimentoGroup")
                    .StartNow()
                    .WithCronSchedule(cronJobAplicaRendimento.Erro));
            }
        });
        builder.Services.AddQuartzHostedService(x => { x.WaitForJobsToComplete = true; });
    }

    private static void ConfiguraQuartzResgataLiquidado(IHostApplicationBuilder builder, CronJobResgataLiquidado cronJobResgataLiquidado)
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

            var resgateLiquidadoJobKey = new JobKey("resgateLiquidadoJobKey", "resgataLiquidadoGroup");

            x.AddJob<CronJobConsultaEResgataInvestimentoLiquidado>(x => x.WithIdentity(resgateLiquidadoJobKey));

            if (builder.Environment.IsDevelopment())
            {
                x.AddTrigger(x => x
                    .ForJob(resgateLiquidadoJobKey)
                    .WithIdentity("resgateLiquidadoJobKey", "resgataLiquidadoGroup")
                    .StartNow()
                    //.WithCronSchedule(cronJobResgataLiquidado.Diario));
                    .WithSimpleSchedule(x => x.WithIntervalInSeconds(300).RepeatForever()));
            }
            else
            {
                x.AddTrigger(x => x
                    .ForJob(resgateLiquidadoJobKey)
                    .WithIdentity("resgateLiquidadoJobKey", "resgataLiquidadoGroup")
                    .StartNow()
                    .WithCronSchedule(cronJobResgataLiquidado.Diario));
            }
        });
        builder.Services.AddQuartzHostedService(x => { x.WaitForJobsToComplete = true; });
    }

    private static void ConfiguraServicoCronJob(IServiceCollection service)
    {
        service.AddSingleton<AdicionaORendimentoNaPosicaoDeHoje>();
        service.AddSingleton<AdicionaOResgateNoInvestimentoLiquidado>();
    }
}