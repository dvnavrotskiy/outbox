using Microsoft.AspNetCore.Mvc;
using OutboxService.Business.Models;
using OutboxService.Business.Ports;
using OutboxService.Controllers.Contracts;

namespace OutboxService.Controllers;

[ApiController, Route("api/[controller]")]
public class MyDataController (IMyDataService service) : ControllerBase
{
    [HttpPost("message")]
    public async Task<IActionResult> SaveMessage([FromBody] SaveMessageRequest request, CancellationToken ct)
    {
        // Так как у нас анимичная модель, мы можем использовать ее как контракт между слоями
        var item = new MyDataItem{ Id = request.Id ?? 0, Message = request.Message };
        var result = await service.SaveData(item, ct);
        return Ok(result);
    }
}