using System.Text;
using Azure.Messaging.ServiceBus;
using Mango.Services.EmailAPI.Model.DTO;
using Mango.Services.EmailAPI.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Mango.Services.EmailAPI.Messaging;

public class AzureServiceBusConsumer : IAzureServiceBusConsumer
{
    private readonly string serviceBusConnectionString;
    private readonly string emailCartQueue;
    private readonly string emailUserQueue;
    private readonly IConfiguration _configuration;
    private ServiceBusProcessor _emailCartProcessor;
    private ServiceBusProcessor _emailRegisterUserProcessor;
    private readonly EmailService _emailService;

    public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
    {
        _emailService = emailService;
        _configuration = configuration;
        serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
        emailCartQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");
        emailUserQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailRegisterUserQueue");

        var client = new ServiceBusClient(serviceBusConnectionString);
        _emailCartProcessor = client.CreateProcessor(emailCartQueue);
        _emailRegisterUserProcessor = client.CreateProcessor(emailUserQueue);
    }

    public async Task Start()
    {
        _emailCartProcessor.ProcessMessageAsync += OnEmailCartRequestReceived;
        _emailCartProcessor.ProcessErrorAsync += ErrorHandler;

        _emailRegisterUserProcessor.ProcessMessageAsync += OnEmailUserRegisterRequestReceived;
        _emailRegisterUserProcessor.ProcessErrorAsync += ErrorHandler;

        await _emailCartProcessor.StartProcessingAsync();
        await _emailRegisterUserProcessor.StartProcessingAsync();
    }
    public async Task Stop()
    {
        await _emailCartProcessor.StopProcessingAsync();
        await _emailCartProcessor.DisposeAsync();

        await _emailRegisterUserProcessor.StopProcessingAsync();
        await _emailRegisterUserProcessor.DisposeAsync();
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine(args.Exception.ToString());
        return Task.CompletedTask;
    }
    private async Task OnEmailCartRequestReceived(ProcessMessageEventArgs args)
    {
        // receive message
        var message = args.Message;
        var body = Encoding.UTF8.GetString(message.Body);

        CartDTO objMessage = JsonConvert.DeserializeObject<CartDTO>(body);
        try
        {
            // TODO - TRY TO LOG EMAIL
            await _emailService.EmailCartAndLog(objMessage);
            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    private async Task OnEmailUserRegisterRequestReceived(ProcessMessageEventArgs args)
    {
        // receive message
        var message = args.Message;
        var body = Encoding.UTF8.GetString(message.Body);

        string objMessage = JsonConvert.DeserializeObject<string>(body);
        try
        {
            // TODO - TRY TO LOG EMAIL
            await _emailService.EmailRegisterUserAndLog(objMessage);
            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}
