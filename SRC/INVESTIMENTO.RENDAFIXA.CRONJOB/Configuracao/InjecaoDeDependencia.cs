using INVESTIMENTO.RENDAFIXA.CRONJOB.CronJob;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Consulta;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.BancoDeDados.Manipula;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Financeiro.Servico;
using INVESTIMENTO.RENDAFIXA.DOMAIN.Imposto.BancoDeDados.Consulta;
using INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE;
using INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Financeiro.BancoDeDados.Consulta;
using INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Financeiro.BancoDeDados.Manipula;
using INVESTIMENTO.RENDAFIXA.INFRASTRUCTURE.Imposto.BancoDeDados.Consulta;
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
        InvestimentoRendaFixaCronJob investimentoRendaFixaCronJob;
        if (builder.Environment.IsProduction())
        {
            var criptografado = builder.Configuration.GetSection("INVESTIMENTO.RENDAFIXA.CRONJOB").Get<string>() ?? throw new InvalidOperationException("INVESTIMENTO.RENDAFIXA.CRONJOB");

            const int nativeTagSizeByte = 16;
            var keyBytes = Convert.FromBase64String("UXGoyfisUfj+1C0k+iK+kgaDnNeB7kWoPmDm0pALKCs=");
            var nonceBytes = Convert.FromBase64String("XWtpeXD4XCnCqNoy");
            var tagBytes = Convert.FromBase64String("NpW13DlIyb+6nB+GjQdLbw==");
            var cipherBytes = Convert.FromBase64String(criptografado);
            var plainBytes = new byte[cipherBytes.Length];

            using var aes = new AesGcm(keyBytes, nativeTagSizeByte);

            aes.Decrypt(nonceBytes, cipherBytes, tagBytes, plainBytes);

            investimentoRendaFixaCronJob = System.Text.Json.JsonSerializer.Deserialize<InvestimentoRendaFixaCronJob>(Encoding.UTF8.GetString(plainBytes)) ?? throw new CryptographicException("ERRO AO DESCRIPTOGRAFAR OS PARÂMETROS INICIAS DA APLICAÇÃO!");
        }
        else
            investimentoRendaFixaCronJob = builder.Configuration.GetSection("INVESTIMENTO.RENDAFIXA.CRONJOB").Get<InvestimentoRendaFixaCronJob>() ?? throw new InvalidCastException("ERRO AO CONVERTER OS PARÂMETROS INICIAIS DA APLICAÇÃO!");

        ConfiguraDbConnection(builder, investimentoRendaFixaCronJob.ConnectionString.DBRENDAFIXA);
        ConfiguraBancoDeDados(builder.Services);
        ConfiguraQuartz(builder, investimentoRendaFixaCronJob.ConfiguraCronJobAplicaRendimento);
        ConfiguraServicoCronJob(builder.Services);

        builder.Services.AddSingleton<IUsuarioInvestimentoRendaFixaCronJob>(x =>
        {
            return investimentoRendaFixaCronJob;
        });
        builder.Services.AddLogging(b => b.AddConsole());
    }

    private static void ConfiguraDbConnection(IHostApplicationBuilder builder, string connectionString)
    {
        builder.Services.AddTransient<IDbConnection>(x => new SqlConnection(connectionString));
    }

    private static void ConfiguraBancoDeDados(IServiceCollection service)
    {
        //Consulta
        service.AddSingleton<IServicoQueListaConfiguracaoImposto, ServicoQueListaConfiguracaoImposto>();
        service.AddSingleton<IServicoQueListaInvestimento, ServicoQueListaInvestimento>();
        service.AddSingleton<IServicoQueObtemAPosicaoDoInvestimento, ServicoQueObtemAPosicaoDoInvestimento>();

        //Manipula
        service.AddSingleton<IServicoQueAdicionaOuAtualizaPosicaoImpostoInvestimento, ServicoQueAdicionaOuAtualizaPosicaoImpostoInvestimento>();
        service.AddSingleton<IServicoQueAdicionaOuAtualizaPosicaoInvestimento, ServicoQueAdicionaOuAtualizaPosicaoInvestimento>();
        service.AddSingleton<IServicoQueAtualizaInvestimento, ServicoQueAtualizaInvestimento>();
    }

    private static void ConfiguraQuartz(IHostApplicationBuilder builder, ConfiguraCronJobAplicaRendimento configuraCronJobAplicaRendimento)
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

            x.AddTrigger(x => x
                .ForJob(rendimentoDiarioJobKey)
                .WithIdentity("rendimentoDiarioJobKey", "aplicaRendimentoGroup")
                .StartNow()
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(200).RepeatForever()));

            x.AddTrigger(x => x
                .ForJob(rendimentoComErroJobKey)
                .WithIdentity("rendimentoComErroJobKey", "aplicaRendimentoGroup")
                .StartNow()
                .WithCronSchedule(configuraCronJobAplicaRendimento.Erro));
        });
        builder.Services.AddQuartzHostedService(x => { x.WaitForJobsToComplete = true; });
    }

    private static void ConfiguraServicoCronJob(IServiceCollection service)
    {
        service.AddSingleton<AplicaORendimentoNaPosicaoDeHoje>();
    }
}