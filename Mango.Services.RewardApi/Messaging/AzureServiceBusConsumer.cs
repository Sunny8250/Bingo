using Azure.Messaging.ServiceBus;
using Mango.Services.RewardApi.Message;
using Mango.Services.RewardApi.Services;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.RewardApi.Messaging
{
    public class AzureServiceBusConsumer: IAzureServiceBusConsumer
    {
        private readonly IConfiguration _configuration;
        private readonly string orderCreatedTopic;
        private readonly string orderCreatedRewardsSubscription;
        private readonly string serviceBusConnectionstring;
        private ServiceBusProcessor rewardProcessor;
        private readonly RewardService _rewardService;
        public AzureServiceBusConsumer(IConfiguration configuration, RewardService rewardService)
        {
            _configuration = configuration;

            serviceBusConnectionstring = configuration.GetValue<string>("ServiceBusConnectionString");

			orderCreatedTopic = configuration.GetValue<string>("TopicAndQueueName:OrderCreatedTopic");
            orderCreatedRewardsSubscription = configuration.GetValue<string>("TopicAndQueueName:OrderCreatedRewardsUpdateSubscription");

            _rewardService = rewardService;

            var client = new ServiceBusClient(serviceBusConnectionstring);
            //Creates instance can be used to process messages using event handler that are set in the processor 
            //as per event and delegate we can say that here CreateProcessor is the instance of the publisher event class
            //where subscriber is subscribing by assigning with subscriber handler
            rewardProcessor = client.CreateProcessor(orderCreatedTopic, orderCreatedRewardsSubscription); //processor will listen to subscription.
                                                                                                          //Like topic name is passed in the parameter as 1st arg
        }

        public async Task Start()
        {
            rewardProcessor.ProcessMessageAsync += OnNewOrderRewardsRequestReceived; //(event handler/ subscriber handler subscribed to event)
            rewardProcessor.ProcessErrorAsync += ErrorHandler;
            await rewardProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await rewardProcessor.StopProcessingAsync();
            await rewardProcessor.DisposeAsync();
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
        //event handler
        private async Task OnNewOrderRewardsRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);
            RewardMessage rewardMessage = JsonConvert.DeserializeObject<RewardMessage>(body);
            try
            {
                await _rewardService.UpdateRewards(rewardMessage);
                await args.CompleteMessageAsync(message); // Tell the queue that message has been proccessed now can be deleted.
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}
