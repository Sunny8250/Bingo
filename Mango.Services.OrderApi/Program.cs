using Mango.MessageBus;
using Mango.Services.OrderApi.Data;
using Mango.Services.OrderApi.Extensions;
using Mango.Services.OrderApi.Utility;
using Microsoft.EntityFrameworkCore;
using Mango.Services.OrderApi.Models.DTO;
using Mango.Services.OrderApi.Services.IService;
using Mango.Services.OrderApi;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplicationBuilderExtension.AddSwaggerGenAuthentication(builder);

builder.Services.AddHttpClient("Product", x =>
{
    x.BaseAddress = new Uri(builder.Configuration["ServiceUrls:ProductApiBaseUrl"]);
}).AddHttpMessageHandler<BackendApiAuthHttpClientHandler>();

builder.Services.AddDbContext<AppDbContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("MangoOrderConnectionString")));

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

WebApplicationBuilderExtension.AddAppAuthentication(builder);

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ResponseDTO>();
builder.Services.AddScoped<IProductService, Mango.Services.OrderApi.Services.ProductService>();//conflict b/w ProdService for orderApi and Stripe
builder.Services.AddScoped<BackendApiAuthHttpClientHandler>();
builder.Services.AddScoped<IMessageBus, MessageBus>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
