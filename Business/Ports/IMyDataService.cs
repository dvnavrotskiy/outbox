using OutboxService.Business.Models;

namespace OutboxService.Business.Ports;

public interface IMyDataService
{
    Task<int> SaveData(MyDataItem item, CancellationToken ct);
}