using OutboxService.Business.Ports;

namespace OutboxService.Business;

// Этот сервис может быть отнесен не к бизнес-слою, а к слою приложения,
// т.к. никакой бизнесовой нагрузки в данном виде не несет
public sealed class MyDataChangedOutboxService(IOutboxRepo repo, ILogger<MyDataChangedOutboxService> logger) : IMyDataChangedOutboxService
{
    public async Task ProceedOutboxBatch(CancellationToken ct)
    {
        var messages = await repo.GetMessages(ct);

        foreach (var message in messages)
        {
            // Вот тут шлем в шину, дергаем сторонний Web API etc.
            logger.LogInformation($"Processing message {message}");
        }

        await repo.ReleaseMessages(messages.Select(m => m.Id).ToList(), ct);
    }
}