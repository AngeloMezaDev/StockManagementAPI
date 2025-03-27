using TransactionService.API.Middleware;
using TransactionService.Application.Interfaces;
using TransactionService.Application.Services;
using TransactionService.Domain.Interfaces;
using TransactionService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Register HTTP Client for Product Service
builder.Services.AddHttpClient<IProductServiceClient, ProductServiceClient>(client =>
{
    // Base URL will be loaded from configuration
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Register dependencies
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ITransactionService, TransactionService.Application.Services.TransactionService>();


var app = builder.Build();
app.UseMiddleware<ErrorHandlingMiddleware>();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.UseAuthorization();
app.MapControllers();

app.Run();