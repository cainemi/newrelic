using System.Net.Http;
using System.Threading;
using System;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using Amazon.SimpleNotificationService;
using Amazon.Runtime;
using Amazon.SimpleNotificationService.Model;

public class CustomActionFilter : ActionFilterAttribute
{
    public override async Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
    {
        var sender = new SnsMessageSender(new SnsClientFactory());

        await sender.SendMessageAsync("arn:aws:sns:us-east-1:000000000000:METER_READ_RECEIVED", @"{ ""test"" : ""true"" } ");
    }
}

public interface ISnsClientFactory
{
    IAmazonSimpleNotificationService CreateClient();
}

public class SnsClientFactory : ISnsClientFactory
{
    public IAmazonSimpleNotificationService CreateClient()
    {
        // Replace this with your LocalStack SNS endpoint, typically http://localhost:4566
        var localstackUrl = "http://localhost:4566";

        // Provide dummy credentials since LocalStack doesn't validate them
        var credentials = new BasicAWSCredentials("dummyAccessKey", "dummySecretKey");

        // Configure the SNS client to use the LocalStack endpoint
        var config = new AmazonSimpleNotificationServiceConfig
        {
            ServiceURL = localstackUrl,
            AuthenticationRegion = "us-east-1", // Set your preferred region
        };

        // Create and return an SNS client (you may need to configure the region or credentials)
        return new AmazonSimpleNotificationServiceClient(credentials, config); // Example for EU-West-1 region
    }
}

public class SnsMessageSender
{
    private readonly ISnsClientFactory _snsClientFactory;

    public SnsMessageSender(ISnsClientFactory snsClientFactory)
    {
        _snsClientFactory = snsClientFactory;
    }

    public async Task SendMessageAsync(string topicArn, string message)
    {
        var snsClient = _snsClientFactory.CreateClient();
        var request = new PublishRequest
        {
            TopicArn = topicArn,
            Message = message
        };

        try
        {
            var response = await snsClient.PublishAsync(request);
            Console.WriteLine($"Message sent with ID: {response.MessageId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send message: {ex.Message}");
        }
    }
    public void SendMessage(string topicArn, string message)
    {
        var snsClient = _snsClientFactory.CreateClient();
        var request = new PublishRequest
        {
            TopicArn = topicArn,
            Message = message
        };

        try
        {
            var response = snsClient.Publish(request);
            Console.WriteLine($"Message sent with ID: {response.MessageId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send message: {ex.Message}");
        }
    }
}
