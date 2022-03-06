using Azure.Messaging.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TopicAndSubscriptions
{
    internal class TopicSender
    {
        private ServiceBusSender sender;
        public TopicSender(string connectionString, string topicName)
        {
            ServiceBusClient client = new ServiceBusClient(connectionString);
            sender = client.CreateSender(topicName);
        }

        public async Task SendOrderMessage(Order order)
        {
            Console.WriteLine($"{order.ToString()}");

            var orderJson = JsonSerializer.Serialize<Order>(order);

            ServiceBusMessage message = new ServiceBusMessage(orderJson);

            // Promote properties
            message.ApplicationProperties.Add("region", order.Region);
            message.ApplicationProperties.Add("items", order.Items);
            message.ApplicationProperties.Add("value", order.Value);
            message.ApplicationProperties.Add("loyality", order.HasLoyalityCard);

            // Set the correlation id
            message.CorrelationId = order.Region;

            await sender.SendMessageAsync(message);
        }

        public async Task Close()
        {
            await sender.CloseAsync();
        }
    }
}
