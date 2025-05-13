namespace OutboxService.Business.Models;

public record MyDataChangedMessage
{
    public int Id { get; init; }
    public MyDataItem? Item { get; init; }
    public DateTime Created { get; init; }
};