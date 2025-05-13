using OutboxService.Business.Models;

namespace OutboxService.Business.Ports;

public interface IMyDataRepo
{
    Task<int> SaveMyDataItem(MyDataItem item, CancellationToken ct);
}