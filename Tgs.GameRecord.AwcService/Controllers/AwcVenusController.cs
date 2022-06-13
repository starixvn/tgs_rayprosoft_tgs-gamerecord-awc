using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.AwcService.Core.ManualWorkerServices;
using Tgs.GameRecord.AwcService.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.AwcService.Controllers
{
    [ApiController]
    [Route("awc_venus")]
    public class AwcVenusController : ControllerBase
    {
        [HttpPost]
        public ValueTask ExecuteAsync(
            [FromServices] ICommandService<AwcVenusCommand> service,
            GameRecordQueryViewModel model)
            => service.ExecuteAsync(new AwcVenusCommand(
                GameRecordApiUrl: model.GameRecordApiUrl,
                GameProviderAgentId: model.GameProviderAgentId,
                GameProviderCert: model.GameProviderCert,
                StartTime: model.StartTime,
                EndTime: model.EndTime));
    }
}
