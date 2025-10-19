using System.Text;
using Azure.Messaging.ServiceBus;
using Mango.Services.RewardAPI.Message;
using Mango.Services.RewardAPI.Services;
using Newtonsoft.Json;

namespace Mango.Services.RewardAPI.Messaging;

public class AzureServiceBusConsumer : IAzureServiceBusConsumer
{
    private readonly string serviceBusConnectionString;
    private readonly string orderCreatedTopic;
    private readonly string OrderCreatedRewardsSubscription;
    private readonly IConfiguration _configuration;
    private readonly RewardsService _rewardService;

    private ServiceBusProcessor _rewardsProcessor;

    public AzureServiceBusConsumer(IConfiguration configuration, RewardsService rewardsService)
    {
        _rewardService = rewardsService;
        _configuration = configuration;
        serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
        orderCreatedTopic = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
        OrderCreatedRewardsSubscription = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreated_Rewards_Subscription");

        var client = new ServiceBusClient(serviceBusConnectionString);
        _rewardsProcessor = client.CreateProcessor(orderCreatedTopic, OrderCreatedRewardsSubscription);
    }

    public async Task Start()
    {
        _rewardsProcessor.ProcessMessageAsync += OnNewOrderRequestReceived;
        _rewardsProcessor.ProcessErrorAsync += ErrorHandler;

        await _rewardsProcessor.StartProcessingAsync();
    }
    public async Task Stop()
    {
        await _rewardsProcessor.StopProcessingAsync();
        await _rewardsProcessor.DisposeAsync();
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine(args.Exception.ToString());
        return Task.CompletedTask;
    }
    private async Task OnNewOrderRequestReceived(ProcessMessageEventArgs args)
    {
        // receive message
        var message = args.Message;
        var body = Encoding.UTF8.GetString(message.Body);

        RewardsMessage objMessage = JsonConvert.DeserializeObject<RewardsMessage>(body);
        try
        {
            // TODO - TRY TO LOG EMAIL
            await _rewardService.UpdateRewards(objMessage);
            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
