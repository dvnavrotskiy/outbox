namespace OutboxService.Controllers.Contracts;

public record SaveMessageRequest
{
    public int? Id { get; init; }
    public required string Message { get; init; }
}