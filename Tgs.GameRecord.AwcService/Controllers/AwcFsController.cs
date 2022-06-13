using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.AwcService.ViewModels;
using Tgs.ServiceUtility;
using Tgs.GameRecord.AwcService.Core.ManualWorkerServices;

namespace Tgs.GameRecord.AwcService.Controllers
{
    [ApiController]
    [Route("awc_fs")]
    public class AwcFsController : ControllerBase
    {
        [HttpPost]
        public ValueTask ExecuteAsync(
            [FromServices] ICommandService<AwcFsCommand> service,
            GameRecordQueryViewModel model)
            => service.ExecuteAsync(new AwcFsCommand(
                GameRecordApiUrl: model.GameRecordApiUrl,
                GameProviderAgentId: model.GameProviderAgentId,
                GameProviderCert: model.GameProviderCert,
                StartTime: model.StartTime,
                EndTime: model.EndTime));
    }
}
