namespace OutboxService.Business.Models;

public sealed record MyDataItem
{
    public int Id { get; set; }
    public required string Message { get; init; }
    public DateTime Timestamp { get; set; }
}