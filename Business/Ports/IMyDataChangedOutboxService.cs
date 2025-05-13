namespace OutboxService.Business.Ports;

public interface IMyDataChangedOutboxService
{
    Task ProceedOutboxBatch(CancellationToken ct);
}