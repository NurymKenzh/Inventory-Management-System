using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Inventory_Management_System.Data;
using Inventory_Management_System.Middleware;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<Inventory_Management_SystemContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Inventory_Management_SystemContext") ?? throw new InvalidOperationException("Connection string 'Inventory_Management_SystemContext' not found.")));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<LoggingMiddleware>();

app.Run();
