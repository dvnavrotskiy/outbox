namespace OutboxService.Infrastructure.Database.Models;

public record MyDataSaveResult
{
    public int Id { get; init; }
    public DateTime Timestamp { get; init; }
}