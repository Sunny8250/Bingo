using Mango.Services.RewardApi.Services;
using Mango.Services.RewardApi.Data;
using Mango.Services.RewardApi.Extensions;
using Mango.Services.RewardApi.Messaging;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MangoRewardConnectionString")));

var optionBuilder = new DbContextOptionsBuilder<AppDbContext>();
optionBuilder.UseSqlServer(builder.Configuration.GetConnectionString("MangoRewardConnectionString"));
builder.Services.AddSingleton(new RewardService(optionBuilder.Options));

builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();

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

app.UseAzureBusServiceConsumer();

app.Run();
