using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.AwcService.Core.ManualWorkerServices;
using Tgs.GameRecord.AwcService.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.AwcService.Controllers
{
    [ApiController]
    [Route("awc_ludo")]
    public class AwcLudoController : ControllerBase
    {
        [HttpPost]
        public ValueTask ExecuteAsync(
            [FromServices] ICommandService<AwcLudoCommand> service,
            GameRecordQueryViewModel model)
            => service.ExecuteAsync(new AwcLudoCommand(
                GameRecordApiUrl: model.GameRecordApiUrl,
                GameProviderAgentId: model.GameProviderAgentId,
                GameProviderCert: model.GameProviderCert,
                StartTime: model.StartTime,
                EndTime: model.EndTime));
    }
}
