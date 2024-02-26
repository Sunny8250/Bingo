using Mango.Gateway.Solution.Extensions;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Values;

var builder = WebApplication.CreateBuilder(args);
builder.AddAppAuthentication();
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true); //Add json file to the configuration
builder.Services.AddOcelot(builder.Configuration); //register ocelot and append .json to the service

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.UseOcelot().GetAwaiter().GetResult(); //Asynchronous

app.Run();
