using Dapper;
using Newtonsoft.Json;
using Npgsql;
using OutboxService.Business.Models;
using OutboxService.Business.Ports;
using OutboxService.Infrastructure.Database.Models;

namespace OutboxService.Infrastructure.Database;

public sealed class OutboxRepo(string connectionString) : IOutboxRepo
{
    public async Task<List<MyDataChangedMessage>> GetMessages(CancellationToken ct)
    {
        await using var con = new NpgsqlConnection(connectionString);
        await con.OpenAsync(ct);

        // Нам нужно получить пачку записей, у которых не вышло или не назначено время обработки
        // Мы просто внутри одной транзакции зададим записям время обработки
        // и заберем нужные данные.
        // Мы можем забирать в этом запросе только Id, а остальные данные забирать
        // отдельным запросом по мере обработки. Это несколько облегчит и ускорит данный запрос,
        // но не сильно. Логику получения данных симметрично несильно усложнит.
        var result = (await con.QueryAsync<MyDataChangedRecord>(
                @"
WITH batch AS (
	SELECT Id, Data, Created
	FROM MyDataChangedOutbox
	WHERE (OccupiedTill IS NULL OR OccupiedTill < current_timestamp)
    ORDER BY Created
	LIMIT 10
)
UPDATE MyDataChangedOutbox ob SET
	OccupiedTill = current_timestamp + interval '1 minute'
FROM batch b WHERE b.Id = ob.Id
RETURNING b.Id, b.Data, b.Created"
            ))
            // И сконвертируем результат в контракт
            .Select(
                record => new MyDataChangedMessage
                {
                    Id = record.Id,
                    Created = record.Created,
                    Item = JsonConvert.DeserializeObject<MyDataItem>(record.Data)
                }
            )
            .ToList();
        
        return result;
    }

    public async Task ReleaseMessages(IList<int> ids, CancellationToken ct)
    {
        await using var con = new NpgsqlConnection(connectionString);
        await con.OpenAsync(ct);

        // Просто удалим обработанные записи
        await con.ExecuteAsync(
            "DELETE FROM MyDataChangedOutbox WHERE Id = ANY(@Ids)",
            new { Ids = ids }
        );
    }

}