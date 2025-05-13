using OutboxService.Business.Ports;

namespace OutboxService.Controllers;

// Да, в концепциях GRASP это тоже контроллер
public sealed class OutboxPoller(IMyDataChangedOutboxService service) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                await service.ProceedOutboxBatch(ct);
                
                // Не бомбим БД запросами подряд
                await Task.Delay(1000, ct);
            }
            catch (Exception ex)
            {
                // Несмотря на повсеместное упрощение, этот try..catch я оставляю,
                // так как он имеет значение для паттерна:
                // не позволяет упасть Poller'у при единичном сбое итерации 
                Console.WriteLine(ex);
            }
        }
    }
}