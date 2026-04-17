using MassTransit;
using Microsoft.EntityFrameworkCore;
using SagaOrchestrationStateMachineWorkerService;
using SagaOrchestrationStateMachineWorkerService.Models;
using SharedOrchestration;
using System.Reflection;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddMassTransit(cfg =>
{
    cfg.AddSagaStateMachine<OrderStateMachine, OrderStateInstance>().EntityFrameworkRepository(opt =>
    {
        opt.AddDbContext<DbContext, OrderStateDbContext>((provider, builder) =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            builder.UseSqlServer(configuration.GetConnectionString("SqlCon"), sqlOptions =>
            {
                //sqlOptions.MigrationsAssembly("SagaOrchestrationStateMachineWorkerService");
                sqlOptions.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
            });
        });
    });

    cfg.UsingRabbitMq((context, rabbitCfg) =>
    {
        var configuration = context.GetRequiredService<IConfiguration>();
        rabbitCfg.Host(configuration.GetConnectionString("RabbitMq"));

        rabbitCfg.ReceiveEndpoint(RabbitMQSettingsConst.OrderSaga, e =>
        {
            e.ConfigureSaga<OrderStateInstance>(context);
        });
    });

    /*Deprecated*/
    //cfg.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(configure =>
    //{
    //    var configuration = provider.GetRequiredService<IConfiguration>();
    //    configure.ReceiveEndpoint(RabbitMQSettingsConst.OrderSaga, e =>
    //    {
    //        e.ConfigureSaga<OrderStateInstance>(provider);
    //    });
    //}));

});

var host = builder.Build();
host.Run();
