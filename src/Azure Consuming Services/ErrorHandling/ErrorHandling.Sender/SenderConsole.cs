using Azure.Messaging.ServiceBus;
using System;
using System.Threading.Tasks;

namespace ErrorHandling.Sender
{
    internal class SenderConsole
    {
        private static string senderConnectionString = @"";
        private static string queueName = "appqueue";
        static async Task Main(string[] args)
        {
            WriteLine("Sender Console! Press enter to send", ConsoleColor.White);
            Console.ReadLine();

            ServiceBusClient serviceBusClient = new ServiceBusClient(senderConnectionString);
            ServiceBusSender sender = serviceBusClient.CreateSender(queueName);

            while (true)
            {
                Console.WriteLine("text/json/poison/unknown/exit?");

                var messageType = Console.ReadLine().Trim().ToLower();

                if (messageType == "exit")
                {
                    break;
                }

                switch (messageType)
                {
                    case "text":
                        {
                            var msg = GetMessage("Hello", "text/plain");
                            await sender.SendMessageAsync(msg);
                            break;
                        }
                    case "json":
                        {
                            var msg = GetMessage("{\"contact\": {\"name\": \"Abhineel\",\"twitter\": \"@abhineel\" }}", "application/json");
                            await sender.SendMessageAsync(msg);
                            break;
                        }
                    case "poison":
                        {
                            var msg = GetMessage("<contact><name>Abhineel</name><twitter>@abhineel</twitter></contact>", "application/json");
                            await sender.SendMessageAsync(msg);
                            break;
                        }
                    case "unknown":
                        {
                            var msg = GetMessage("unknown Message", "application/unknown");
                            await sender.SendMessageAsync(msg);
                            break;
                        }
                    default:
                        Console.WriteLine("What?");
                        break;

                }
            }

            await sender.CloseAsync();
        }

        private static ServiceBusMessage GetMessage(string text, string contentType)
        {
            ServiceBusMessage message = new ServiceBusMessage(text)
            {
                ContentType = contentType
            };

            return message;
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
