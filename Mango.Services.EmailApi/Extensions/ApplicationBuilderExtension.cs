using Mango.Services.EmailApi.Messaging;

namespace Mango.Services.EmailApi.Extensions
{
    public static class ApplicationBuilderExtension
    {
        private static IAzureServiceBusConsumer ServiceBusConsumer {get; set;}

        public static IApplicationBuilder UseAzureBusServiceConsumer(this IApplicationBuilder app)
        {
            ServiceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumer>();
            var hostApplicationLife = app.ApplicationServices.GetService<IHostApplicationLifetime>();

            hostApplicationLife.ApplicationStarted.Register(OnStart);
            hostApplicationLife.ApplicationStopping.Register(OnStop);

            return app;
        }

        private async static void OnStop()
        {
           await ServiceBusConsumer.Stop();
        }

        private async static void OnStart()
        {
            await ServiceBusConsumer.Start();
        }
    }
}
