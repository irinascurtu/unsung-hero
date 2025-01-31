using API.Infrastructure.Mappings;
using Azure.Core.Pipeline;
using Contracts.Response;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Http.Resilience;
using Orders.Data;
using Orders.Domain;
using Orders.Service;
using OrdersApi.Service.Clients;
using OrdersApi.Services;
using Polly;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

builder.Services.AddAutoMapper(typeof(OrderProfileMapping));
builder.Services.AddDbContext<OrderContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services
    .AddHttpClient<IProductStockServiceClient, ProductStockServiceClient>()
     .AddResilienceHandler("my-pipeline", builder =>
      {
          // Refer to https://www.pollydocs.org/strategies/retry.html#defaults for retry defaults
          builder.AddRetry(new HttpRetryStrategyOptions
          {
              MaxRetryAttempts = 4,
              Delay = TimeSpan.FromSeconds(2),
              BackoffType = DelayBackoffType.Exponential

          });
          //builder.AddTimeout(TimeSpan.FromSeconds(1));
      });

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    x.AddRequestClient<VerifyOrder>();
    // Step 2: Select a Transport
    x.UsingRabbitMq((context, cfg) =>
    {
        //cfg.ReceiveEndpoint("OrderCreated", e =>
        //{
        //    e.ConfigureConsumer<OrderCreatedConsumer>(context);
        //});
        var entryAssembly = Assembly.GetEntryAssembly();
        x.AddConsumers(entryAssembly);
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
    {
        serviceScope.ServiceProvider.GetService<OrderContext>().Database.EnsureCreated();
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
