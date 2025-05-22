using Dapper;
using Newtonsoft.Json;
using Npgsql;
using OutboxService.Business.Models;
using OutboxService.Business.Ports;
using OutboxService.Infrastructure.Database.Models;

namespace OutboxService.Infrastructure.Database;

public class MyDataRepo(string connectionString) : IMyDataRepo
{
    public async Task<int> SaveMyDataItem(MyDataItem item, CancellationToken ct)
    {
        // Это слишком умный метод, в рамках примера нормально,
        // но в реальной жизни лучше разнести его на два уровня:
        // один будет управлять логикой транзакции,
        // другой непосредственно выполнять запросы
        await using var con = new NpgsqlConnection(connectionString);
        await con.OpenAsync(ct);
        await using var tran = await con.BeginTransactionAsync(ct);

        // Создадим новую запись или обновим старую
        var saveResult = (item.Id == 0)
            ? await con.QueryFirstAsync<MyDataSaveResult>(
                "INSERT INTO MyDataTable (Message, Timestamp) VALUES (@Message, current_timestamp) RETURNING Id, Timestamp",
                new { item.Message },
                tran
            )
            : await con.QueryFirstAsync<MyDataSaveResult>(
                "UPDATE MyDataTable SET Message = @Message, Timestamp = current_timestamp WHERE Id = @Id RETURNING Id, Timestamp",
                new { item.Id, item.Message },
                tran
            );
        
        // Обновляем состояние модели идентификатором и штампом времени от СУБД
        item.Id = saveResult.Id;
        item.Timestamp = saveResult.Timestamp;

        // В данном примере сериализация/десериализация оставлена на этом уровне,
        // но при разнесении на два уровня имеет смысл управлять ей там же, где и логикой транзакции.
        // Замечание справедливо и для OutboxRepo
        var data = JsonConvert.SerializeObject(item);
        await con.ExecuteAsync("INSERT INTO MyDataChangedOutbox (Data) VALUES (@Data)",
            new { Data = data },
            tran
        );
        
        await tran.CommitAsync(ct);
        return item.Id;
    }
}