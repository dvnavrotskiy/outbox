using OutboxService.Business;
using OutboxService.Business.Ports;
using OutboxService.Controllers;
using OutboxService.Infrastructure.Database;
using OutboxService.Infrastructure.Database.Migrations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

var cs = builder.Configuration.GetConnectionString("pgConnection"); 
if (string.IsNullOrWhiteSpace(cs))
    throw new Exception("ConnectionStrings: pgConnection must be defined");

builder.Services
    .AddSingleton<IMyDataRepo>(new MyDataRepo(cs))
    .AddSingleton<IOutboxRepo>(new OutboxRepo(cs))

    .AddSingleton<IMyDataService, MyDataService>()
    .AddSingleton<IMyDataChangedOutboxService, MyDataChangedOutboxService>()

    .AddHostedService<OutboxPoller>();

var app = builder.Build();

MigrationRunner.Up(app.Configuration);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();


