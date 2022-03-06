using Azure.Messaging.ServiceBus;
using System;
using System.Threading.Tasks;

namespace ErrorHandling.DeadLetterReceiver
{
    internal class DeadletterReceiver
    {
        private static string receiverConnectionString = @"";
        private static string queueName = "appqueue";
        static async Task Main(string[] args)
        {
            WriteLine("Deadletter Receiver Console! Press enter to receive", ConsoleColor.White);
            Console.ReadLine();

            ServiceBusClient serviceBusClient = new ServiceBusClient(receiverConnectionString);

            ServiceBusProcessor processor = serviceBusClient.CreateProcessor(queueName, new ServiceBusProcessorOptions()
            {
                SubQueue = SubQueue.DeadLetter,
                AutoCompleteMessages = true,
                MaxConcurrentCalls = 1,
            });

            try
            {
                processor.ProcessMessageAsync += MessageHandler;
                processor.ProcessErrorAsync += ErrorHandler;

                await processor.StartProcessingAsync();

                WriteLine($"Receiving, hit enter to exit", ConsoleColor.White);
                Console.ReadLine();
            }
            finally
            {
                await processor.DisposeAsync();
                await serviceBusClient.DisposeAsync();
            }

        }

        static async Task MessageHandler(ProcessMessageEventArgs args)
        {
            WriteLine($"Received dead letter message: {args.Message.ContentType}", ConsoleColor.Cyan);
            WriteLine($"\t Dead letter reason: {args.Message.DeadLetterReason}", ConsoleColor.Green);
            WriteLine($"\t Dead letter error description: {args.Message.DeadLetterErrorDescription}", ConsoleColor.Green);

            await args.CompleteMessageAsync(args.Message);
        }

        static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            WriteLine($"Exception: {args.Exception.Message}", ConsoleColor.Yellow);
            return Task.CompletedTask;
        }

        public static void WriteLine(string text, ConsoleColor color)
        {
            var tempColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = tempColor;
        }
    }
}
