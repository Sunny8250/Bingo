using Azure.Messaging.ServiceBus;
using Mango.Services.EmailApi.Message;
using Mango.Services.EmailApi.Models.DTO;
using Mango.Services.EmailApi.Services;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.EmailApi.Messaging
{
    public class AzureServiceBusConsumer: IAzureServiceBusConsumer
    {
        private readonly IConfiguration _configuration;
        private readonly string emailCartQueue;
        private readonly string emailAuthQueue;
        private readonly string orderCreatedTopic;
        private readonly string orderCreatedEmailSubscription;
        private readonly string serviceBusConnectionstring;
        private ServiceBusProcessor emailCartProcessor;
        private ServiceBusProcessor emailAuthProcessor;
        private ServiceBusProcessor emailProcessor;
        private readonly EmailService _emailService;
        public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
        {
            _configuration = configuration;
            serviceBusConnectionstring = configuration.GetValue<string>("ServiceBusConnectionString");
            emailCartQueue = configuration.GetValue<string>("TopicAndQueueName:EmailShoppingCartQueue");
            emailAuthQueue = configuration.GetValue<string>("TopicAndQueueName:EmailAuthQueue");
            orderCreatedTopic = configuration.GetValue<string>("TopicAndQueueName:OrderCreatedTopic");
            orderCreatedEmailSubscription = configuration.GetValue<string>("TopicAndQueueName:OrderCreatedEmailSubscription");
            _emailService = emailService;

            var client = new ServiceBusClient(serviceBusConnectionstring);
            //Creates instance can be used to process messages using event handler that are set in the processor 
            //as per event and delegate we can say that here CreateProcessor is the instance of the publisher event class
            //where subscriber is subscribing by assigning with subscriber handler
            emailCartProcessor = client.CreateProcessor(emailCartQueue);
            emailAuthProcessor = client.CreateProcessor(emailAuthQueue);
            emailProcessor = client.CreateProcessor(orderCreatedTopic, orderCreatedEmailSubscription);
        }

        public async Task Start()
        {
            emailCartProcessor.ProcessMessageAsync += OnEmailCartRequestReceived; //(event handler/ subscriber handler subscribed to event)
            emailCartProcessor.ProcessErrorAsync += ErrorHandler;
            await emailCartProcessor.StartProcessingAsync();

            emailAuthProcessor.ProcessMessageAsync += OnRegistrationRequestReceived;
            emailAuthProcessor.ProcessErrorAsync += ErrorHandler;
            await emailAuthProcessor.StartProcessingAsync();

            emailProcessor.ProcessMessageAsync += OnNewOrderPlacedEmailRequestReceived;
            emailProcessor.ProcessErrorAsync += ErrorHandler;
			await emailProcessor.StartProcessingAsync();
		}

        public async Task Stop()
        {
            await emailCartProcessor.StopProcessingAsync();
            await emailCartProcessor.DisposeAsync();

            await emailAuthProcessor.StopProcessingAsync();
            await emailAuthProcessor.DisposeAsync();

			await emailProcessor.StopProcessingAsync();
			await emailProcessor.DisposeAsync();
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
		private async Task OnNewOrderPlacedEmailRequestReceived(ProcessMessageEventArgs args)
		{
			var message = args.Message;
			var body = Encoding.UTF8.GetString(message.Body);
			OrderConfirmation orderConfirmation = JsonConvert.DeserializeObject<OrderConfirmation>(body);
			try
			{
				await _emailService.EmailAndLogOrderPlaced(orderConfirmation);
				await args.CompleteMessageAsync(message); // Tell the queue that message has been proccessed now can be deleted.
			}
			catch (Exception ex)
			{
				throw;
			}
		}
	}
}
