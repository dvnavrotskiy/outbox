using OutboxService.Business.Models;
using OutboxService.Business.Ports;

namespace OutboxService.Business;

public sealed class MyDataService (IMyDataRepo repo) : IMyDataService
{
    public async Task<int> SaveData(MyDataItem item, CancellationToken ct)
    {
        // Спорный вопрос о том, надо ли давать бизнес-ядру
        // знания о транзакционности и рычаги управления записью в Outbox
        return await repo.SaveMyDataItem(item, ct);
        // Но без них эта часть выглядит лаконичной
    }
}