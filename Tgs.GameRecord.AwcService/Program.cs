using System.Net.Mime;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Tgs.GameRecord.AwcService;
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

builder.Services
    .AddRouting(options => options.LowercaseUrls = true)
    .AddControllers(options =>
    {
        options.Filters.Add(new ProducesAttribute(MediaTypeNames.Application.Json));
        options.Filters.Add(new ConsumesAttribute(MediaTypeNames.Application.Json));
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFileNames = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.xml");
    foreach (var xmlFileName in xmlFileNames) c.IncludeXmlComments(xmlFileName);
});

builder.Services.AddAwcServiceCore(
    (options, sp) =>
    {
        var configuration = sp.GetRequiredService<IConfiguration>();
        options.ConnectionString = configuration.GetConnectionString("Redis")!;
    },
    (options, sp) =>
    {
        var configuration = sp.GetRequiredService<IConfiguration>();
        configuration.GetSection("GameRecordInfo").Bind(options);
    },
    (options, sp) =>
    {
        var configuration = sp.GetRequiredService<IConfiguration>();
        configuration.GetSection("MongoSetting").Bind(options);
    },
    (options, sp) =>
    {
        var configuration = sp.GetRequiredService<IConfiguration>();
        options.BaseUrl = configuration.GetValue<Uri>("GameRecord:GameInterfaceBaseUrl");
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
    });

var mongoSetting = builder.Configuration.GetSection("MongoSetting").Get<MongoSettingOptions>();
var natsSetting = builder.Configuration.GetSection("NatsSetting").Get<NatsSettingOptions>();

builder.Services
    .AddMessageQueue()
    .AddMongoMessageQueue(config =>
    {
        _ = config
        .DefineMessageQueue(
            mongoSetting.ManagerName,
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
         .AddHandler<ScheduleMessageHandler>(
             subject: natsSetting.Subject,
             group: natsSetting.Group);
     })
     .ClearAllExchanges()
     .AddMongoGlobPatternExchange($"{mongoSetting.GlobPattern}.aelotto", mongoSetting.ManagerName, $"{mongoSetting.QueueName}.aelotto")
     .AddMongoGlobPatternExchange($"{mongoSetting.GlobPattern}.aws", mongoSetting.ManagerName, $"{mongoSetting.QueueName}.aws")
     .AddMongoGlobPatternExchange($"{mongoSetting.GlobPattern}.cockfight", mongoSetting.ManagerName, $"{mongoSetting.QueueName}.cockfight")
     .AddMongoGlobPatternExchange($"{mongoSetting.GlobPattern}.facai", mongoSetting.ManagerName, $"{mongoSetting.QueueName}.facai")
     .AddMongoGlobPatternExchange($"{mongoSetting.GlobPattern}.fs", mongoSetting.ManagerName, $"{mongoSetting.QueueName}.fs")
     .AddMongoGlobPatternExchange($"{mongoSetting.GlobPattern}.km", mongoSetting.ManagerName, $"{mongoSetting.QueueName}.km")
     .AddMongoGlobPatternExchange($"{mongoSetting.GlobPattern}.ludo", mongoSetting.ManagerName, $"{mongoSetting.QueueName}.ludo")
     .AddMongoGlobPatternExchange($"{mongoSetting.GlobPattern}.pgs", mongoSetting.ManagerName, $"{mongoSetting.QueueName}.pgs")
     .AddMongoGlobPatternExchange($"{mongoSetting.GlobPattern}.rcb", mongoSetting.ManagerName, $"{mongoSetting.QueueName}.rcb")
     .AddMongoGlobPatternExchange($"{mongoSetting.GlobPattern}.sg", mongoSetting.ManagerName, $"{mongoSetting.QueueName}.sg")
     .AddMongoGlobPatternExchange($"{mongoSetting.GlobPattern}.spade", mongoSetting.ManagerName, $"{mongoSetting.QueueName}.spade")
     .AddMongoGlobPatternExchange($"{mongoSetting.GlobPattern}.venus", mongoSetting.ManagerName, $"{mongoSetting.QueueName}.venus")
     .AddMongoGlobPatternExchange($"{mongoSetting.GlobPattern}.yl", mongoSetting.ManagerName, $"{mongoSetting.QueueName}.yl");

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

_ = app.UseGameRecordRegister();

app.Run();
