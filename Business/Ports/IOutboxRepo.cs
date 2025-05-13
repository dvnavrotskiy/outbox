using OutboxService.Business.Models;

namespace OutboxService.Business.Ports;

public interface IOutboxRepo
{
    Task<List<MyDataChangedMessage>> GetMessages(CancellationToken ct);
    Task ReleaseMessages(IList<int> ids, CancellationToken ct);
}