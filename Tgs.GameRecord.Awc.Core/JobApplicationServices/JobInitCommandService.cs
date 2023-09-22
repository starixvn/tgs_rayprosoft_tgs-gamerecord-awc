using System.Runtime.InteropServices;
using System.Text.Json.Nodes;
using Hangfire;
using Hangfire.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.GameCommand.Client;
using Tgs.GameCommand.Common;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.Awc.Core.JobApplicationServices;

internal class JobInitCommandService : ICommandService<JobInitCommand>
{
    private readonly IGameCommandClient _client;
    private readonly GameRecordInfoOptions _gameRecordInfo;
    private readonly ILogger<JobInitCommandService> _logger;
    private readonly RecurringJobOptions _recurringJobOptions;

    public JobInitCommandService(
        ILogger<JobInitCommandService> logger,
        IOptions<GameRecordInfoOptions> options,
        IGameCommandClient client)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _gameRecordInfo = options.Value;
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _recurringJobOptions = new RecurringJobOptions()
        {
            TimeZone = TimeZoneInfo.FindSystemTimeZoneById(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "China Standard Time" : "Asia/Shanghai")
        };
    }

    public async ValueTask ExecuteAsync(JobInitCommand command, CancellationToken cancellationToken = default)
    {
        using (var connection = JobStorage.Current.GetConnection())
        {
            foreach (var recurringJob in connection.GetRecurringJobs())
            {
                RecurringJob.RemoveIfExists(recurringJob.Id);
            }
        }

        try
        {
            foreach (var gameProviderCode in _gameRecordInfo.GameProviderCodes)
            {
                var gameProviderConfiguration = $"{gameProviderCode}Configuration";
                var settingConfiguration = await _client
                .SettingGetConfigurationStringAsync(
                key: gameProviderConfiguration,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false)
                ?? throw new Exception($"This game product setting does not exist.({gameProviderConfiguration})");

                var settingConfigurationJson = JsonNode.Parse(settingConfiguration)
                    ?? throw new Exception($"This game product setting does not exist.({gameProviderConfiguration})");

                var gameProviderConfigs = await _client
                    .GameProviderConfigurationQueryAsync(
                    gameProviderCode: gameProviderCode,
                    cancellationToken: cancellationToken)
                    .ToArrayAsync(cancellationToken: cancellationToken)
                    .ConfigureAwait(false)
                    ?? Enumerable.Empty<GameProviderConfigInfo>();

                if (!gameProviderConfigs.Any())
                {
                    continue;
                }

                var gameProviderAgentMapper = new Dictionary<string, JobExecuteCommand>();

                foreach (var gameProviderConfig in gameProviderConfigs)
                {
                    if (string.IsNullOrWhiteSpace(gameProviderConfig.ConfigurationData) || gameProviderConfig.Status == GameProviderConfigStatus.Inactive)
                    {
                        continue;
                    }

                    var configurationDataJson = JsonNode.Parse(gameProviderConfig.ConfigurationData)
                        ?? throw new Exception($"Game Provider Configuration does not exist.({gameProviderConfig.GameProviderConfigId})");

                    var apiUrl = new Uri(configurationDataJson["ApiRecordUrl"]?.ToString()
                        ?? settingConfigurationJson["ApiRecordUrl"]?.ToString()
                        ?? throw new Exception($"Required parameters(ApiRecordUrl)"));
                    var agentId = configurationDataJson["agentId"]?.ToString()
                        ?? throw new Exception($"Required parameters(agentId)");
                    var cert = configurationDataJson["cert"]?.ToString()
                        ?? throw new Exception($"Required parameters(cert)");

                    if (gameProviderAgentMapper.ContainsKey(agentId))
                    {
                        continue;
                    }

                    var jobExecuteCommand = new JobExecuteCommand(
                        GameProviderCode: gameProviderConfig.GameProviderCode,
                        Name: gameProviderConfig.Name,
                        ApiUrl: apiUrl,
                        AgentId: agentId,
                        Cert: cert);

                    gameProviderAgentMapper.Add(agentId, jobExecuteCommand);

                    RecurringJob.AddOrUpdate<IHangfireUtensil>(
                        recurringJobId: gameProviderConfig.Name,
                        methodCall: service => service.JobExecuteAsync(jobExecuteCommand),
                        cronExpression: Cron.Minutely(),
                        options: _recurringJobOptions);

                    //初始化強制第一次處理
                    _ = BackgroundJob.Enqueue<IHangfireUtensil>(service => service.JobExecuteAsync(jobExecuteCommand));
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LogOn:{logOn} Message:遊戲廠商初始化排程發生例外", DateTime.Now.ToString("o"));
        }
        finally
        {
            _ = BackgroundJob.Schedule<IHangfireUtensil>(service => service.JobCheckAsync(), TimeSpan.FromMinutes(1));
        }
    }
}
