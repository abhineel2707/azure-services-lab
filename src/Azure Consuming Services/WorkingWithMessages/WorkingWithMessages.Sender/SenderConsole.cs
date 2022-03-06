using Azure.Messaging.ServiceBus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;
using WorkingWithMessages.MessageEntities;

namespace WorkingWithMessages.Sender
{
    class SenderConsole
    {
        private static string senderConnectionString = "";
        private static string queueName = "appqueue";
        static async Task Main(string[] args)
        {
            WriteLine("Sender Console - Hit Enter", ConsoleColor.White);
            Console.ReadLine();

            //await SendTextString("This is a sample text message");

            //await SendPizzaOrderAsync();

            //await SendPizzaOrderAsListAsync();

            await SendPizzaOrderListAsBatchAsync();
        }

        static async Task SendTextString(string text)
        {
            WriteLine("SendTextStringAsMessageAsync", ConsoleColor.Cyan);

            // create service bus client and sender
            ServiceBusClient serviceBusClient = new ServiceBusClient(senderConnectionString);
            ServiceBusSender sender = serviceBusClient.CreateSender(queueName);

            Write("Sending...", ConsoleColor.Green);

            ServiceBusMessage message = new ServiceBusMessage(text);
            message.ContentType = "application/json";

            await sender.SendMessageAsync(message);

            // Calling DisposeAsync is required to ensure that
            // network resources and other unmanaged objects are properly cleaned up.
            await sender.DisposeAsync();
            await serviceBusClient.DisposeAsync();

        }

        static async Task SendPizzaOrderAsync()
        {
            WriteLine("SendPizzaOrderAsync", ConsoleColor.Cyan);

            var order = new PizzaOrder()
            {
                CustomerName = "John Doe",
                Type = "Hawaiian",
                Size = "Large"
            };

            // Serialize the object
            var jsonPizzaOrder = JsonSerializer.Serialize<PizzaOrder>(order);

            // Create the message
            ServiceBusMessage message = new ServiceBusMessage(jsonPizzaOrder)
            {
                Subject = "PizzaOrder",
                ContentType = "application/json"
            };

            ServiceBusClient serviceBusClient = new ServiceBusClient(senderConnectionString);
            ServiceBusSender sender = serviceBusClient.CreateSender(queueName);
            WriteLine("Sending order...", ConsoleColor.Green);

            await sender.SendMessageAsync(message);

            WriteLine("Done!", ConsoleColor.Green);

        }

        static async Task SendPizzaOrderAsListAsync()
        {
            WriteLine("SendPizzaOrderAsListAsync", ConsoleColor.Green);

            var pizzaOrders = GetPizzaOrders();

            ServiceBusClient serviceBusClient = new ServiceBusClient(senderConnectionString);
            ServiceBusSender sender = serviceBusClient.CreateSender(queueName);

            try
            {
                WriteLine("Sending...", ConsoleColor.Yellow);

                var watch = Stopwatch.StartNew();

                foreach (var pizzaOrder in pizzaOrders)
                {
                    var jsonPizzaOrder = JsonSerializer.Serialize<PizzaOrder>(pizzaOrder);
                    ServiceBusMessage message = new ServiceBusMessage(jsonPizzaOrder)
                    {
                        Subject = "Pizza Order",
                        ContentType = "application/json"
                    };

                    await sender.SendMessageAsync(message);
                }
                await sender.CloseAsync();

                WriteLine($"Sent {pizzaOrders.Count} orders! Time: {watch.ElapsedMilliseconds} ms, i.e. {pizzaOrders.Count / watch.Elapsed.TotalSeconds} messages per second", ConsoleColor.Green);
                Console.WriteLine();
            }
            finally
            {
                await serviceBusClient.DisposeAsync();
            }
        }

        static async Task SendPizzaOrderListAsBatchAsync()
        {
            WriteLine("SendPizzaOrderListAsBatchAsync", ConsoleColor.Green);

            var pizzaOrders = GetPizzaOrders();

            ServiceBusClient serviceBusClient = new ServiceBusClient(senderConnectionString);
            ServiceBusSender sender = serviceBusClient.CreateSender(queueName);

            using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

            foreach (var pizzaOrder in pizzaOrders)
            {
                ServiceBusMessage message = new ServiceBusMessage(JsonSerializer.Serialize(pizzaOrder))
                {
                    Subject = "Pizza Order",
                    ContentType = "application/json"
                };
                if (!messageBatch.TryAddMessage(message))
                {
                    throw new Exception($"The message {message} is too large to fit in the batch.");
                }
            }


            try
            {
                WriteLine("Sending...", ConsoleColor.Yellow);

                var watch = Stopwatch.StartNew();

                await sender.SendMessagesAsync(messageBatch);

                await sender.CloseAsync();

                WriteLine($"Sent {pizzaOrders.Count} orders! Time: {watch.ElapsedMilliseconds} ms, i.e. {pizzaOrders.Count / watch.Elapsed.TotalSeconds} messages per second", ConsoleColor.Green);
                Console.WriteLine();
            }
            finally
            {
                await serviceBusClient.DisposeAsync();
            }
        }

        static IList<PizzaOrder> GetPizzaOrders()
        {
            List<PizzaOrder> pizzaOrders = new List<PizzaOrder>();

            string[] names = new string[] { "Jon", "Ron", "Steve" };
            string[] pizzas = new string[] { "Farmhouse", "Veggie Delight", "Peppy Paneer", "indi Tandoori" };

            for (int pizza = 0; pizza < pizzas.Length; pizza++)
            {
                for (int name = 0; name < names.Length; name++)
                {
                    PizzaOrder pizzaOrder = new PizzaOrder()
                    {
                        CustomerName = names[name],
                        Type = pizzas[pizza],
                        Size = "Large"
                    };

                    pizzaOrders.Add(pizzaOrder);
                }
            }

            return pizzaOrders;
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
