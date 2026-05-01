using Polly;
using Polly.Extensions.Http;
using RetryPatternServiceA.Api;
using System.Diagnostics;
using System.Net;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddSwaggerGen();
        builder.Services.AddHttpClient<ProductService>(options =>
        {
            options.BaseAddress = new Uri("http://localhost:5002");
        })
        .AddPolicyHandler(GetRetryPolicy())
        .AddPolicyHandler(GetCircuitBreakerPolicy())
        .AddPolicyHandler(GetAdvanceCircuitBreakerPolicy());


        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            //app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();

        }

        // Configure the HTTP request pipeline.

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }


    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions.HandleTransientHttpError().OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound).WaitAndRetryAsync(5, retryAttempt =>
        {
            Debug.WriteLine($"Retry Count :{retryAttempt}");
            return TimeSpan.FromSeconds(10);
        }, onRetryAsync: onRetryAsync);
    }

    private static Task onRetryAsync(DelegateResult<HttpResponseMessage> arg1, TimeSpan arg2)
    {
        Debug.WriteLine($"Request is made again:{arg2.TotalMilliseconds}");

        return Task.CompletedTask;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions.HandleTransientHttpError().CircuitBreakerAsync(3, TimeSpan.FromSeconds(10), onBreak: (arg1, arg2) =>
        {
            Debug.WriteLine("Circuit is open");
        }, onReset: () =>
        {
            Debug.WriteLine("Circuit is closed");
        }, onHalfOpen: () =>
        {
            Debug.WriteLine("Circuit is half-open");
        });
    }

    private static IAsyncPolicy<HttpResponseMessage> GetAdvanceCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions.HandleTransientHttpError().AdvancedCircuitBreakerAsync(
            0.5, TimeSpan.FromSeconds(30), 30, TimeSpan.FromSeconds(30), onBreak: (arg1, arg2) =>
        {
            Debug.WriteLine("Circuit Breaker Status => On Break");
        }, onReset: () =>
        {
            Debug.WriteLine("Circuit Breaker Status => On Reset");
        }, onHalfOpen: () =>
        {
            Debug.WriteLine("Circuit Breaker Status => On Half Open");
        });
    }
}