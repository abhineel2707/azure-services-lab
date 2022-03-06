using Azure.Messaging.ServiceBus;
using RfidCheckout.MessageEntities;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace RfidCheckout.TagReader
{
    internal class TagReaderConsole
    {
        private static string senderConnectionString = @"";
        private static string queueName = "rfidqueue";

        static async Task Main(string[] args)
        {
            Console.WriteLine("Tag Reader Console");

            ServiceBusClient serviceBusClient = new ServiceBusClient(senderConnectionString);
            ServiceBusSender sender = serviceBusClient.CreateSender(queueName);

            // Create a sample order
            RfidTag[] orderItems = new RfidTag[]
            {
                new RfidTag(){Product="Ball",Price=4.99},
                new RfidTag(){Product="Whistle",Price=1.95},
                new RfidTag(){Product="Bat",Price=12.99},
                new RfidTag(){Product="Bat",Price=12.99},
                new RfidTag(){Product="Gloves",Price=7.99},
                new RfidTag(){Product="Gloves",Price=7.99},
                new RfidTag(){Product="Cap",Price=9.99},
                new RfidTag(){Product="Cap",Price=9.99},
                new RfidTag(){Product="Shirt",Price=14.99},
                new RfidTag(){Product="Shirt",Price=14.99}
            };

            // Display order data
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Order contains {0} items.", orderItems.Length);

            Console.ForegroundColor = ConsoleColor.Yellow;
            double orderTotal = 0.0;
            foreach (RfidTag tag in orderItems)
            {
                Console.WriteLine($"{tag.Product} - {tag.Price}");
                orderTotal += tag.Price;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Order value = {orderTotal}");
            Console.WriteLine();
            Console.ResetColor();

            Console.WriteLine("Press enter to scan...");

            Console.ReadLine();

            Random random = new Random(DateTime.Now.Millisecond);

            // Send the order with random duplicate tag reads
            int sentCount = 0;
            int position = 0;

            Console.WriteLine("Reading tags...");
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Cyan;

            while (position < 10)
            {
                RfidTag tag = orderItems[position];
                var tagReadMessage = new ServiceBusMessage(JsonSerializer.Serialize<RfidTag>(tag));
                // Set the message id to enable duplicate detection
                tagReadMessage.MessageId = tag.TagId;
                tagReadMessage.ApplicationProperties.Add("StoreName", "Metro");
                await sender.SendMessageAsync(tagReadMessage);

                Console.WriteLine($"Sent: {tag.Product}\t Message Id: {tagReadMessage.MessageId}");

                var randomVal = random.NextDouble();
                if (randomVal > 0.4)
                {
                    position++;
                }
                sentCount++;

                Thread.Sleep(100);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Total tag reads {sentCount}");
            Console.WriteLine();
            Console.ResetColor();
        }
    }
}
