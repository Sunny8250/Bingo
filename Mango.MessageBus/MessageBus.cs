using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mango.MessageBus
{
    public class MessageBus : IMessageBus
    {
        //It is the address to the Azure ServiceBus connection path
        private string serviceBusConnectionString = "Endpoint=sb://bingoweb.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=ohBEEdfU9fKqH+5TBhqYPw6pAMVr0FKxM+ASbJdHIzs=";
        public async Task PublishMessage(object message, string topic_queue_name)
        {
            await using(var client = new ServiceBusClient(serviceBusConnectionString))
            {
                ServiceBusSender serviceBusSender = client.CreateSender(topic_queue_name);
                var jsonMessage = JsonConvert.SerializeObject(message);
                ServiceBusMessage serviceBusMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
                {
                    CorrelationId = Guid.NewGuid().ToString(),
                };
                await serviceBusSender.SendMessageAsync(serviceBusMessage);
                await client.DisposeAsync();
            }
        }
    }
}
