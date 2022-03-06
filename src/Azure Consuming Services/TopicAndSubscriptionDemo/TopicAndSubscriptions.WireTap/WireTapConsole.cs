using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TopicAndSubscriptions.WireTap
{
    internal class WireTapConsole
    {
        private static string ServiceBusConnectionString = @"";
        private static string TopicName = "wiretap";
        static async Task Main(string[] args)
        {
            Console.WriteLine("Wire Tap Console");
            Console.WriteLine("Press enter to activate wire tap...");
            Console.ReadLine();

            string subscriptionName = $"wiretap-{Guid.NewGuid()}";

            ServiceBusAdministrationClient serviceBusAdministrationClient = new ServiceBusAdministrationClient(ServiceBusConnectionString);
            await serviceBusAdministrationClient.CreateTopicAsync(TopicName);
            await serviceBusAdministrationClient.CreateSubscriptionAsync(TopicName, subscriptionName);

            ServiceBusClient serviceBusClient = new ServiceBusClient(ServiceBusConnectionString);
            ServiceBusSender sender = serviceBusClient.CreateSender(TopicName);
            ServiceBusMessage message = new ServiceBusMessage("Inspect this message");
            message.Subject = "Wire tap Message";
            message.ContentType = "application/json";

            await sender.SendMessageAsync(message);

            ServiceBusProcessor processor = serviceBusClient.CreateProcessor(TopicName, subscriptionName, new ServiceBusProcessorOptions()
            {
                //MaxConcurrentCalls = 1,
                AutoCompleteMessages = false
            });

            processor.ProcessMessageAsync += InspectMessageAsync;
            processor.ProcessErrorAsync += ErrorHandler;

            await processor.StartProcessingAsync();
            Console.WriteLine("Wait for a minute and then press any key to end the processing");
            Console.ReadKey();
        }

        private static async Task InspectMessageAsync(ProcessMessageEventArgs args)
        {
            Console.WriteLine($"Received message...");
            ServiceBusReceivedMessage message = args.Message;

            Console.WriteLine("Properties...");
            Console.WriteLine($"    ContentType             - { message.ContentType }");
            Console.WriteLine($"    CorrelationId           - { message.CorrelationId }");
            Console.WriteLine($"    ExpiresAt               - { message.ExpiresAt }");
            Console.WriteLine($"    Label                   - { message.Subject }");
            Console.WriteLine($"    MessageId               - { message.MessageId }");
            Console.WriteLine($"    PartitionKey            - { message.PartitionKey }");
            Console.WriteLine($"    ReplyTo                 - { message.ReplyTo }");
            Console.WriteLine($"    ReplyToSessionId        - { message.ReplyToSessionId }");
            Console.WriteLine($"    ScheduledEnqueueTimeUtc - { message.ScheduledEnqueueTime }");
            Console.WriteLine($"    SessionId               - { message.SessionId }");
            Console.WriteLine($"    TimeToLive              - { message.TimeToLive }");
            Console.WriteLine($"    To                      - { message.To }");

            Console.WriteLine("ApplicationProperties");
            foreach (var property in message.ApplicationProperties)
            {
                Console.WriteLine($"    { property.Key } - { property.Value }");
            }

            Console.WriteLine("Body");
            Console.WriteLine($"{ Encoding.UTF8.GetString(message.Body) }");
            Console.WriteLine();
            await args.CompleteMessageAsync(message);
        }

        private static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine($"Error Occured {args.Exception.ToString()}");
            return Task.CompletedTask;
        }
    }
}
