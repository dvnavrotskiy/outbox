namespace OutboxService.Infrastructure.Database.Models;

public record MyDataChangedRecord
{
    public int Id { get; init; }
    public required string Data { get; init; }
    public DateTime Created { get; init; }
};