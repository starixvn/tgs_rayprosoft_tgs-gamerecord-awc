using Microsoft.Extensions.Options;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Tgs.GameRecord.AwcService;
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
    options.BaseUrl = configuration.GetValue<Uri>("GameRecord:GameProvidersQueryServerBaseUrl");
},
(options, sp) =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    options.BaseUrl = configuration.GetValue<Uri>("GameRecord:GameProvidersBaseUrl");
});

builder.Services
    .AddMessageQueue()
    .AddMongoMessageQueue(config =>
    {
        _ = config
        .DefineMessageQueue(
            "gp.gamerecord.database",
            sp => new MongoMessageQueueBuilderOptions
            {
                MessageQueue = MongoMessageQueue.Default,
                ConnectionString = sp.GetRequiredService<IConfiguration>().GetConnectionString("MongoMessaging")
            });
    })
     .AddNatsMessageQueue(natsConfig =>
     {
         _ = natsConfig
         .ConfigQueueOptions((options, sp) =>
         {
             var configuration = sp.GetRequiredService<IConfiguration>();
             options.Url = configuration.GetConnectionString("Nats");
         })
         .AddProcessor<ManualMessageHandler>("gp.gamerecord.manual.awc", "gp.gamerecord.manual");
     })
     .ClearAllExchanges()
     .AddMongoGlobPatternExchange("tgs.gp.gamerecord.database.round.awc.aelotto", "gp.gamerecord.database", "tgs.gp.gamerecord.database.round.awc.aelotto")
     .AddMongoGlobPatternExchange("tgs.gp.gamerecord.database.round.awc.aws", "gp.gamerecord.database", "tgs.gp.gamerecord.database.round.awc.aws")
     .AddMongoGlobPatternExchange("tgs.gp.gamerecord.database.round.awc.cockfight", "gp.gamerecord.database", "tgs.gp.gamerecord.database.round.awc.cockfight")
     .AddMongoGlobPatternExchange("tgs.gp.gamerecord.database.round.awc.facai", "gp.gamerecord.database", "tgs.gp.gamerecord.database.round.awc.facai")
     .AddMongoGlobPatternExchange("tgs.gp.gamerecord.database.round.awc.fs", "gp.gamerecord.database", "tgs.gp.gamerecord.database.round.awc.fs")
     .AddMongoGlobPatternExchange("tgs.gp.gamerecord.database.round.awc.km", "gp.gamerecord.database", "tgs.gp.gamerecord.database.round.awc.km")
     .AddMongoGlobPatternExchange("tgs.gp.gamerecord.database.round.awc.ludo", "gp.gamerecord.database", "tgs.gp.gamerecord.database.round.awc.ludo")
     .AddMongoGlobPatternExchange("tgs.gp.gamerecord.database.round.awc.pgs", "gp.gamerecord.database", "tgs.gp.gamerecord.database.round.awc.pgs")
     .AddMongoGlobPatternExchange("tgs.gp.gamerecord.database.round.awc.rcb", "gp.gamerecord.database", "tgs.gp.gamerecord.database.round.awc.rcb")
     .AddMongoGlobPatternExchange("tgs.gp.gamerecord.database.round.awc.spade", "gp.gamerecord.database", "tgs.gp.gamerecord.database.round.awc.spade")
     .AddMongoGlobPatternExchange("tgs.gp.gamerecord.database.round.awc.venus", "gp.gamerecord.database", "tgs.gp.gamerecord.database.round.awc.venus")
     .AddMongoGlobPatternExchange("tgs.gp.gamerecord.database.round.awc.yl", "gp.gamerecord.database", "tgs.gp.gamerecord.database.round.awc.yl");

builder.Services
     .AddSingleton<IConfigureOptions<OtlpExporterOptions>>(
        sp => new ConfigureNamedOptions<OtlpExporterOptions>(
            Microsoft.Extensions.Options.Options.DefaultName,
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
                && !new[] { "html", "ico", "png", "jpg" }.Any(
                    ext => httpContext.Request.Path.Value.EndsWith($".{ext}"));
        };
    });

builder.Services
    .AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

_ = app.UseSwagger();
_ = app.UseSwaggerUI();

_ = app.UseAuthorization();

_ = app.MapControllers();

app.MapProfileViewer();

app.MapHealthChecks("/healthz");

app.Run();
