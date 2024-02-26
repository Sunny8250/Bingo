using Mango.Services.ProductApi;
using Mango.Services.ProductApi.Data;
using Mango.Services.ProductApi.Extensions;
using Mango.Services.ProductApi.Models.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddHttpContextAccessor();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
WebApplicationBuilderExtension.AddSwaggerGenAuthentication(builder);

builder.Services.AddDbContext<AppDbContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("MangoProductConnectionString")));

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

WebApplicationBuilderExtension.AddAppAuthentication(builder);

builder.Services.AddScoped<ResponseDTO>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
