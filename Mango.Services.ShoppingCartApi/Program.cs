using Mango.MessageBus;
using Mango.Services.ProductApi.Data;
using Mango.Services.ShoppingCartApi;
using Mango.Services.ShoppingCartApi.Extensions;
using Mango.Services.ShoppingCartApi.Models.DTO;
using Mango.Services.ShoppingCartApi.Services;
using Mango.Services.ShoppingCartApi.Services.IService;
using Mango.Services.ShoppingCartApi.Utility;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
WebApplicationBuilderExtension.AddSwaggerGenAuthentication(builder);

builder.Services.AddHttpClient("Product", x =>
{
    x.BaseAddress = new Uri(builder.Configuration["ServiceUrls:ProductApiBaseUrl"]);
}).AddHttpMessageHandler<BackendApiAuthHttpClientHandler>();
builder.Services.AddHttpClient("Coupon", x =>
{
    x.BaseAddress = new Uri(builder.Configuration["ServiceUrls:CouponApiBaseUrl"]);
}).AddHttpMessageHandler<BackendApiAuthHttpClientHandler>();

builder.Services.AddDbContext<AppDbContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("MangoShoppingCartConnectionString")));

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddHttpContextAccessor();

WebApplicationBuilderExtension.AddAppAuthentication(builder);

builder.Services.AddScoped<ResponseDTO>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<BackendApiAuthHttpClientHandler>();
builder.Services.AddScoped<IMessageBus, MessageBus>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
