using Azure.Messaging.ServiceBus;
using Mango.Services.EmailApi.Models.DTO;
using Mango.Services.EmailApi.Services;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.EmailApi.Messaging
{
    public class AzureServiceBusConsumer: IAzureServiceBusConsumer
    {
        private readonly IConfiguration _configuration;
        private readonly string _emailCartQueue;
        private readonly string _emailAuthQueue;
        private readonly string _serviceBusConnectionstring;
        private ServiceBusProcessor _emailCartProcessor;
        private ServiceBusProcessor _emailAuthProcessor;
        private readonly EmailService _emailService;
        public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
        {
            _configuration = configuration;
            _serviceBusConnectionstring = configuration.GetValue<string>("ServiceBusConnectionString");
            _emailCartQueue = configuration.GetValue<string>("TopicAndQueueName:EmailShoppingCartQueue");
            _emailAuthQueue = configuration.GetValue<string>("TopicAndQueueName:EmailAuthQueue");
            _emailService = emailService;

            var client = new ServiceBusClient(_serviceBusConnectionstring);
            //Creates instance can be used to process messages using event handler that are set in the processor 
            //as per event and delegate we can say that here CreateProcessor is the instance of the publisher event class
            //where subscriber is subscribing by assigning with subscriber handler
            _emailCartProcessor = client.CreateProcessor(_emailCartQueue);
            _emailAuthProcessor = client.CreateProcessor(_emailAuthQueue);
        }

        public async Task Start()
        {
            _emailCartProcessor.ProcessMessageAsync += OnEmailCartRequestReceived; //(event handler/ subscriber handler subscribed to event)
            _emailCartProcessor.ProcessErrorAsync += ErrorHandler;
            await _emailCartProcessor.StartProcessingAsync();

            _emailAuthProcessor.ProcessMessageAsync += OnRegistrationRequestReceived;
            _emailAuthProcessor.ProcessErrorAsync += ErrorHandler;
            await _emailAuthProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await _emailCartProcessor.StopProcessingAsync();
            await _emailCartProcessor.DisposeAsync();

            await _emailAuthProcessor.StopProcessingAsync();
            await _emailAuthProcessor.DisposeAsync();
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
        //event handler
        private async Task OnEmailCartRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            CartDTO cart = JsonConvert.DeserializeObject<CartDTO>(body);
            try
            {
                await _emailService.EmailCartAndLog(cart);
                await args.CompleteMessageAsync(message); // Tell the queue that message has been proccessed now can be deleted.
            }
            catch(Exception ex)
            {
                throw;
            }
        }
        private async Task OnRegistrationRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            string email = JsonConvert.DeserializeObject<string>(body);
            try
            {
                await _emailService.EmailUserAndLog(email);
                await args.CompleteMessageAsync(message); // Tell the queue that message has been proccessed now can be deleted.
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
