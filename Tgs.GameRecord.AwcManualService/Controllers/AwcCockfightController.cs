using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.AwcManualService.ViewModels;
using Tgs.GameRecord.AwcService.Core.ManualWorkerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.AwcManualService.Controllers;

[ApiController]
[Route("awc_cockfight")]
public class AwcCockfightController : ControllerBase
{
    [HttpPost]
    public ValueTask ExecuteAsync(
        [FromServices] ICommandService<AwcCockfightCommand> service,
        GameRecordQueryViewModel model)
        => service.ExecuteAsync(new AwcCockfightCommand(
            GameRecordApiUrl: model.GameRecordApiUrl,
            GameProviderAgentId: model.GameProviderAgentId,
            GameProviderCert: model.GameProviderCert,
            StartTime: model.StartTime,
            EndTime: model.EndTime));
}
