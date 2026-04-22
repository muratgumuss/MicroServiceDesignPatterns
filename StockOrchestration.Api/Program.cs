using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedOrchestration;
using StockOrchestration.Api.Consumers;
using StockOrchestration.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderCreatedEventConsumer>();
    x.AddConsumer<StockRollBackMessageConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration.GetConnectionString("RabbitMq"));
        cfg.ReceiveEndpoint(RabbitMQSettingsConst.StockOrderCreatedEventQueueName, e =>
        {
            e.ConfigureConsumer<OrderCreatedEventConsumer>(context);
        });
        cfg.ReceiveEndpoint(RabbitMQSettingsConst.StockRollBackMessageQueueName, e =>
        {
            e.ConfigureConsumer<StockRollBackMessageConsumer>(context);
        });
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseInMemoryDatabase("StockDb");
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Stocks.AddRange(new List<StockOrchestration.Api.Models.Stock>
    {
        new() { Id = 1, ProductId = 1, Count = 100 },
        new StockOrchestration.Api.Models.Stock { Id = 2, ProductId = 2, Count = 200 },
        new StockOrchestration.Api.Models.Stock { Id = 3, ProductId = 3, Count = 300 }
    });
    dbContext.SaveChanges();
}

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
