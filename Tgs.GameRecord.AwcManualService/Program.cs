using Microsoft.Extensions.Options;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Tgs.GameRecord.AwcManualService;
using Tgs.GameRecord.AwcService.Core;
using Tgs.MessageQueue.MongoDB.Configuration;
using MongoMessageQueue = MongoDB.Messaging.MessageQueue;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("/config/appsettings.json", true)
    .AddEnvironmentVariables();

builder.Logging
    .AddOpenTelemetry(options =>
    {
        options.IncludeFormattedMessage = true;
        options.ParseStateValues = true;
        _ = options.AddFluentdForwardExporter(fluentOptions =>
        {
            var nodeIp = builder.Configuration.GetValue<string>("NODE_IP");
            fluentOptions.Host = nodeIp;
            fluentOptions.Tag = builder.Configuration.GetValue<string>("OTEL_SERVICE_NAME");
            fluentOptions.UseMessagePack();
        });
    });

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();

builder.Services.AddGameRecordAwcService((options, sp) =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    options.ConnectionString = configuration.GetConnectionString("Redis")!;
});

builder.Services.AddAwcServiceCore((options, sp) =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    configuration.GetSection("GameRecordInfo").Bind(options);
},
(options, sp) =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    options.BaseUrl = configuration.GetValue<Uri>("GameRecord:GameInterfaceAwcServiceBaseUrl");
},
(options, sp) =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    options.BaseUrl = configuration.GetValue<Uri>("GameRecord:GameCommandClientBaseUrl");
},
(options, sp) =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    options.BaseUrl = configuration.GetValue<Uri>("GameRecord:GameProvidersBaseUrl");
},
(sp) => sp.GetRequiredService<RedisConnectionManager>().Database);

var gameRecordInfo = builder.Configuration.GetSection("GameRecordInfo").Get<GameRecordInfoOptions>();

builder.Services
    .AddMessageQueue()
    .AddMongoMessageQueue(config =>
    {
        _ = config
        .DefineMessageQueue(
            gameRecordInfo.MongoDefine,
            sp => new MongoMessageQueueBuilderOptions
            {
                MessageQueue = MongoMessageQueue.Default,
                ConnectionString = sp.GetRequiredService<IConfiguration>().GetConnectionString("MongoMessaging")!
            });
    })
     .AddNatsMessageQueue(natsConfig =>
     {
         _ = natsConfig
         .ConfigQueueOptions((options, sp) =>
         {
             var configuration = sp.GetRequiredService<IConfiguration>();
             options.Url = configuration.GetConnectionString("Nats")!;
         })
         .AddProcessor<ManualMessageHandler>("gp.gamerecord.manual.awc", "gp.gamerecord.manual");
     })
     .ClearAllExchanges()
     .AddMongoGlobPatternExchange($"{gameRecordInfo.MongoGlobPatternPrefix}.aelotto", gameRecordInfo.MongoDefine, $"{gameRecordInfo.MongoGlobPatternPrefix}.aelotto")
     .AddMongoGlobPatternExchange($"{gameRecordInfo.MongoGlobPatternPrefix}.aws", gameRecordInfo.MongoDefine, $"{gameRecordInfo.MongoGlobPatternPrefix}.aws")
     .AddMongoGlobPatternExchange($"{gameRecordInfo.MongoGlobPatternPrefix}.cockfight", gameRecordInfo.MongoDefine, $"{gameRecordInfo.MongoGlobPatternPrefix}.cockfight")
     .AddMongoGlobPatternExchange($"{gameRecordInfo.MongoGlobPatternPrefix}.facai", gameRecordInfo.MongoDefine, $"{gameRecordInfo.MongoGlobPatternPrefix}.facai")
     .AddMongoGlobPatternExchange($"{gameRecordInfo.MongoGlobPatternPrefix}.fs", gameRecordInfo.MongoDefine, $"{gameRecordInfo.MongoGlobPatternPrefix}.fs")
     .AddMongoGlobPatternExchange($"{gameRecordInfo.MongoGlobPatternPrefix}.km", gameRecordInfo.MongoDefine, $"{gameRecordInfo.MongoGlobPatternPrefix}.km")
     .AddMongoGlobPatternExchange($"{gameRecordInfo.MongoGlobPatternPrefix}.ludo", gameRecordInfo.MongoDefine, $"{gameRecordInfo.MongoGlobPatternPrefix}.ludo")
     .AddMongoGlobPatternExchange($"{gameRecordInfo.MongoGlobPatternPrefix}.pgs", gameRecordInfo.MongoDefine, $"{gameRecordInfo.MongoGlobPatternPrefix}.pgs")
     .AddMongoGlobPatternExchange($"{gameRecordInfo.MongoGlobPatternPrefix}.rcb", gameRecordInfo.MongoDefine, $"{gameRecordInfo.MongoGlobPatternPrefix}.rcb")
     .AddMongoGlobPatternExchange($"{gameRecordInfo.MongoGlobPatternPrefix}.sg", gameRecordInfo.MongoDefine, $"{gameRecordInfo.MongoGlobPatternPrefix}.sg")
     .AddMongoGlobPatternExchange($"{gameRecordInfo.MongoGlobPatternPrefix}.spade", gameRecordInfo.MongoDefine, $"{gameRecordInfo.MongoGlobPatternPrefix}.spade")
     .AddMongoGlobPatternExchange($"{gameRecordInfo.MongoGlobPatternPrefix}.venus", gameRecordInfo.MongoDefine, $"{gameRecordInfo.MongoGlobPatternPrefix}.venus")
     .AddMongoGlobPatternExchange($"{gameRecordInfo.MongoGlobPatternPrefix}.yl", gameRecordInfo.MongoDefine, $"{gameRecordInfo.MongoGlobPatternPrefix}.yl");

builder.Services
     .AddSingleton<IConfigureOptions<OtlpExporterOptions>>(
        sp => new ConfigureNamedOptions<OtlpExporterOptions>(
            Options.DefaultName,
            exporterOptions =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var nodeIp = config.GetValue<string>("NODE_IP");

                if (!string.IsNullOrWhiteSpace(nodeIp))
                    exporterOptions.Endpoint = new Uri($"http://{nodeIp}:4317");

                config.GetSection("Opentelemetry:Tracing").Bind(exporterOptions);
            }))
    .AddOpenTelemetryTracing(traceBuilder => traceBuilder
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddEnvironmentVariableDetector())
        .AddSource("Tgs.*")
        .AddSource("TGS.*")
        .AddGrpcClientInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddProfileViewExporter()
        .AddOtlpExporter())
    .AddProfileViewer()
    .Configure<AspNetCoreInstrumentationOptions>(options =>
    {
        options.Filter = httpContext =>
        {
            return !httpContext.Request.Path.HasValue
                || !httpContext.Request.Path.StartsWithSegments("/swagger")
                && !httpContext.Request.Path.StartsWithSegments("/healthz")
                && !httpContext.Request.Path.StartsWithSegments("/profiler/")
                && !httpContext.Request.Path.StartsWithSegments("/hangfire")
                && !httpContext.Request.Path.StartsWithSegments("/metrics")
                   && !new[] { "html", "ico", "png", "jpg" }.Any(
                       ext => httpContext.Request.Path.Value.EndsWith($".{ext}"));
        };
    });

builder.Services
    .AddHealthChecks();

var app = builder.Build();

_ = app.UseSwagger();
_ = app.UseSwaggerUI();

_ = app.UseAuthorization();

_ = app.MapControllers();

app.MapProfileViewer();

app.MapHealthChecks("/healthz");

app.Run();
