using Azure.Messaging.ServiceBus;
using RfidCheckout.MessageEntities;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace RfidCheckout.Checkout
{
    internal class CheckoutConsole
    {
        private static string receiverConnectionString = @"";
        private static string queueName = "rfidqueue";

        static int ReceivedCount = 0;
        static double BillTotal = 0.0;
        static bool UseMessageSession = false;
        static async Task Main(string[] args)
        {
            Console.WriteLine("Checkout Console");

            ServiceBusClient serviceBusClient = new ServiceBusClient(receiverConnectionString);
            ServiceBusProcessor processor = serviceBusClient.CreateProcessor(queueName, new ServiceBusProcessorOptions()
            {
                ReceiveMode = ServiceBusReceiveMode.PeekLock,
                MaxConcurrentCalls = 1
            });
            if (!UseMessageSession)
            {
                processor.ProcessMessageAsync += MessageHandler;
                processor.ProcessErrorAsync += ErrorHandler;

                await processor.StartProcessingAsync();

                Console.WriteLine("Receiving tag read messages...");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.ReadLine();

                await processor.StopProcessingAsync();
                await processor.CloseAsync();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Bill customer {BillTotal} for {ReceivedCount} items");
                Console.ResetColor();
            }
        }
        static async Task MessageHandler(ProcessMessageEventArgs args)
        {
            var rfidTag = JsonSerializer.Deserialize<RfidTag>(args.Message.Body.ToString());
            Console.WriteLine($"{rfidTag.ToString()}");

            foreach (var key in args.Message.ApplicationProperties.Keys)
            {
                Console.WriteLine($"Key: {key}\t Value: {args.Message.ApplicationProperties[key].ToString()}");
            }

            ReceivedCount++;
            BillTotal += rfidTag.Price;

            await args.CompleteMessageAsync(args.Message);
        }

        static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(args.Exception.ToString());
            Console.ResetColor();
            return Task.CompletedTask;
        }
    }
}
