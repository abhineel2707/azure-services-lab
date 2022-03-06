using Azure.Messaging.ServiceBus;
using System;
using System.Threading.Tasks;

namespace ErrorHandling.Receiver
{
    internal class ReceiverConsole
    {
        private static string receiverConnectionString = @"";
        private static string queueName = "appqueue";

        private static string forwardingQueueConnectionString = @"";
        private static string forwardingQueue = "forwardingqueue";
        static async Task Main(string[] args)
        {
            WriteLine("Receiver Console! Press enter to receive", ConsoleColor.White);

            Console.ReadLine();

            ServiceBusClient serviceBusClient = new ServiceBusClient(receiverConnectionString);
            ServiceBusProcessor processor = serviceBusClient.CreateProcessor(queueName, new ServiceBusProcessorOptions()
            {
                ReceiveMode = ServiceBusReceiveMode.PeekLock,
                AutoCompleteMessages = false,
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
                await serviceBusClient.DisposeAsync();
            }
        }

        static async Task MessageHandler(ProcessMessageEventArgs args)
        {
            WriteLine($"Received: {args.Message.ContentType}", ConsoleColor.Cyan);

            switch (args.Message.ContentType)
            {
                case "text/plain":
                    await ProcessTextMessage(args);
                    break;
                default:
                    WriteLine("Inside default", ConsoleColor.DarkBlue);
                    break;
            }
        }
        private static async Task ProcessTextMessage(ProcessMessageEventArgs args)
        {
            WriteLine($"Text message: {args.Message.Body.ToString()} - Delivery Count: {args.Message.DeliveryCount}", ConsoleColor.Green);

            // Send the message to a forwarding queue and disable the forwarding queue to push this message to deadletter
            try
            {
                ServiceBusClient forwardingClient = new ServiceBusClient(forwardingQueueConnectionString);
                ServiceBusSender forwardingSender = forwardingClient.CreateSender(forwardingQueue);

                await forwardingSender.SendMessageAsync(new ServiceBusMessage(args.Message.Body.ToString()));
                await forwardingSender.CloseAsync();

            }
            catch (Exception ex)
            {
                WriteLine($"Exception : {ex.Message}", ConsoleColor.Yellow);

                // To abandon the message
                //await args.AbandonMessageAsync(args.Message);

                // To move the message to Dead Letter after 5 retries
                if (args.Message.DeliveryCount > 5)
                {
                    await args.DeadLetterMessageAsync(args.Message);
                }
                else
                {
                    // abandon the message
                    //await args.AbandonMessageAsync(args.Message);
                }
            }
        }

        private static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            WriteLine(args.Exception.ToString(), ConsoleColor.Red);
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
