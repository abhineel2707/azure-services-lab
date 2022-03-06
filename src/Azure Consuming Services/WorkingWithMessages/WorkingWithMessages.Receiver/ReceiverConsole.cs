using Azure.Messaging.ServiceBus;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WorkingWithMessages.MessageEntities;

namespace WorkingWithMessages.Receiver
{
    class ReceiverConsole
    {
        private static string receiverConnectionString = "";
        private static string queueName = "appqueue";
        static async Task Main(string[] args)
        {
            WriteLine("Receiver Console", ConsoleColor.White);
            Console.ReadLine();

            //await ReceiveAndProcessText();

            await ReceiveAndProcessPizzaOrder(5);
        }

        private static async Task ReceiveAndProcessText()
        {
            ServiceBusClient serviceBusClient = new ServiceBusClient(receiverConnectionString);
            ServiceBusReceiver receiver = serviceBusClient.CreateReceiver(queueName, new ServiceBusReceiverOptions()
            {
                ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete // ServiceBusReceiveMode.ReceiveAndDelete
            });

            ServiceBusReceivedMessage message = await receiver.ReceiveMessageAsync();

            WriteLine($"Received: {message.Body.ToString()}", ConsoleColor.Green);

            // Calling DisposeAsync is required to ensure that
            // network resources and other unmanaged objects are properly cleaned up.
            await receiver.DisposeAsync();
            await serviceBusClient.DisposeAsync();
        }

        private static async Task ReceiveAndProcessPizzaOrder(int threads)
        {
            WriteLine("ReceiveAndProcessPizzaOrder", ConsoleColor.Cyan);

            ServiceBusClient serviceBusClient = new ServiceBusClient(receiverConnectionString);
            ServiceBusProcessor processor = serviceBusClient.CreateProcessor(queueName, new ServiceBusProcessorOptions()
            {
                ReceiveMode = ServiceBusReceiveMode.PeekLock,
                AutoCompleteMessages = true,
                MaxConcurrentCalls = threads,
                MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(10)
            });

            try
            {
                // add the handler
                processor.ProcessMessageAsync += ProcessPizzaOrderAsync;
                processor.ProcessErrorAsync += ErrorHandler;

                // start processing async
                await processor.StartProcessingAsync();

                WriteLine($"Receiving, hit enter to exit", ConsoleColor.White);
                Console.ReadLine();
                await processor.StopProcessingAsync();
            }
            finally
            {
                await processor.DisposeAsync();
                await serviceBusClient.DisposeAsync();
            }
        }

        private static async Task ProcessPizzaOrderAsync(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();

            var pizzaOrder = JsonSerializer.Deserialize<PizzaOrder>(body);

            CookPizza(pizzaOrder);

            await args.CompleteMessageAsync(args.Message);
        }

        private static void CookPizza(PizzaOrder pizzaOrder)
        {
            WriteLine($"Cooking {pizzaOrder.Type} pizza for {pizzaOrder.CustomerName}.", ConsoleColor.Yellow);
            Thread.Sleep(5000);
            WriteLine($"\t{pizzaOrder.Type} pizza for {pizzaOrder.CustomerName} is ready.", ConsoleColor.Green);
        }

        private static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            WriteLine(args.Exception.ToString(), ConsoleColor.Red);
            return Task.CompletedTask;
        }
        private static void WriteLine(string text, ConsoleColor color)
        {
            var tempColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = tempColor;
        }

        private static void Write(string text, ConsoleColor color)
        {
            var tempColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = tempColor;
        }
    }
}
