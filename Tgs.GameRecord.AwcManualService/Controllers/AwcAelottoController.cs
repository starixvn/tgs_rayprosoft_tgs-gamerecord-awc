using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.AwcManualService.ViewModels;
using Tgs.GameRecord.AwcService.Core.ManualWorkerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.AwcManualService.Controllers;

[ApiController]
[Route("awc_aelotto")]
public class AwcAelottoController : ControllerBase
{
    [HttpPost]
    public ValueTask ExecuteAsync(
        [FromServices] ICommandService<AwcAelottoCommand> service,
        GameRecordQueryViewModel model)
        => service.ExecuteAsync(new AwcAelottoCommand(
            GameRecordApiUrl: model.GameRecordApiUrl,
            GameProviderAgentId: model.GameProviderAgentId,
            GameProviderCert: model.GameProviderCert,
            StartTime: model.StartTime,
            EndTime: model.EndTime));
}
