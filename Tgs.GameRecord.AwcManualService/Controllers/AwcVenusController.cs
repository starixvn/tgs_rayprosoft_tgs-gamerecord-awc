using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.AwcManualService.ViewModels;
using Tgs.GameRecord.AwcService.Core.ManualWorkerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.AwcManualService.Controllers
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
