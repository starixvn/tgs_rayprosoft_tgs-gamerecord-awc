using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.AwcService.Core.MuaualWorkerServices;
using Tgs.GameRecord.AwcService.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.AwcService.Controllers
{
    [ApiController]
    [Route("awc_spade")]
    public class AwcSpadeController : ControllerBase
    {
        [HttpPost]
        public ValueTask ExecuteAsync(
            [FromServices] ICommandService<AwcSpadeCommand> service,
            GameRecordQueryViewModel model)
            => service.ExecuteAsync(new AwcSpadeCommand(
                GameRecordApiUrl: model.GameRecordApiUrl,
                GameProviderAgentId: model.GameProviderAgentId,
                GameProviderCert: model.GameProviderCert,
                StartTime: model.StartTime,
                EndTime: model.EndTime));
    }
}
