using Azure.Messaging.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TopicAndSubscriptions
{
    internal class SubscriptionProcessor
    {
        private ServiceBusProcessor processor;
        public SubscriptionProcessor(string connectionString, string topicName, string subscriptionName)
        {
            ServiceBusClient client = new ServiceBusClient(connectionString);
            processor = client.CreateProcessor(topicName, subscriptionName, new ServiceBusProcessorOptions()
            {
                ReceiveMode = ServiceBusReceiveMode.PeekLock,
                MaxConcurrentCalls = 1,
                AutoCompleteMessages = false
            });
        }

        public void ReceiveAndProcessOrderMessage()
        {
            WriteLine("ReceiveAndProcessOrderMessage", ConsoleColor.Cyan);

            processor.ProcessMessageAsync += ProcessOrderAsync;
            processor.ProcessErrorAsync += ErrorHandler;

            processor.StartProcessingAsync();

        }

        private async Task ProcessOrderAsync(ProcessMessageEventArgs args)
        {
            var orderJson = JsonSerializer.Deserialize<Order>(args.Message.Body.ToString());

            WriteLine(orderJson.ToString(), ConsoleColor.DarkBlue);

            await args.CompleteMessageAsync(args.Message);
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            WriteLine($"Message handler encountered an exception {args.Exception.ToString()}", ConsoleColor.Red);
            return Task.CompletedTask;
        }
        public async Task Close()
        {
            await processor.CloseAsync();
        }
        private static void WriteLine(string text, ConsoleColor color)
        {
            var tempColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = tempColor;
        }
    }
}
