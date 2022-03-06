using Azure;
using Azure.Messaging.ServiceBus.Administration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TopicAndSubscriptions
{
    internal class TopicAndSubscriptionsConsole
    {
        private static string ServiceBusConnectionString = @"";
        private static string TopicName = "orders";
        static async Task Main(string[] args)
        {
            Console.WriteLine("Topics And Subscriptions Console!");

            PrompAndWait("Press enter to create topics and subscriptions...");
            await CreateTopicAndSubscriptionsAsync();

            PrompAndWait("Press enter to send order messages...");
            await SendOrders();

            PrompAndWait("Press enter to receive order messages...");
            await ReceiveOrderFromAllSubcriptions();

            PrompAndWait("Topic and Subscription console complete");
        }

        static async Task CreateTopicAndSubscriptionsAsync()
        {
            var manager = new Manager(ServiceBusConnectionString);

            await manager.CreateTopicAsync(TopicName);

            await manager.CreateSubscriptionAsync(TopicName, "AllOrders");

            await manager.CreateSubscriptionWithSqlFilterAsync(TopicName, "UsaOrders", "region = 'USA'");
            await manager.CreateSubscriptionWithSqlFilterAsync(TopicName, "EuOrders", "region = 'EU'");

            await manager.CreateSubscriptionWithSqlFilterAsync(TopicName, "LargeOrders", "items > 30");
            await manager.CreateSubscriptionWithSqlFilterAsync(TopicName, "HighValueOrders", "value > 500");

            await manager.CreateSubscriptionWithSqlFilterAsync(TopicName, "LoyalityCardOrders", "loyality = true AND region = 'USA'");

            await manager.CreateSubscriptionWithCorrelationFilterAsync(TopicName, "UkOrders", "UK");
        }

        private static async Task SendOrders()
        {
            var orders = CreateTestOrders();

            var sender = new TopicSender(ServiceBusConnectionString, TopicName);

            foreach (var order in orders)
            {
                await sender.SendOrderMessage(order);
            }

            await sender.Close();
        }

        private static async Task ReceiveOrderFromAllSubcriptions()
        {
            var manager = new Manager(ServiceBusConnectionString);

            // Get all subscriptions from our topic
            AsyncPageable<SubscriptionProperties> pagedSubscriptionProperties = manager.GetSubscriptionsForTopicAsync(TopicName);

            await foreach (SubscriptionProperties subscriptionProperties in pagedSubscriptionProperties)
            {
                var processor = new SubscriptionProcessor(ServiceBusConnectionString, TopicName, subscriptionProperties.SubscriptionName);
                processor.ReceiveAndProcessOrderMessage();
                PrompAndWait($"Receiving orders from { subscriptionProperties.SubscriptionName }, press enter when complete..");
                await processor.Close();
            }
        }

        static List<Order> CreateTestOrders()
        {
            var orders = new List<Order>();

            orders.Add(new Order()
            {
                Name = "Loyal Customer",
                Value = 19.99,
                Region = "USA",
                Items = 1,
                HasLoyalityCard = true
            });
            orders.Add(new Order()
            {
                Name = "Large Order",
                Value = 49.99,
                Region = "USA",
                Items = 50,
                HasLoyalityCard = false
            });
            orders.Add(new Order()
            {
                Name = "High Value",
                Value = 749.45,
                Region = "USA",
                Items = 45,
                HasLoyalityCard = false
            });
            orders.Add(new Order()
            {
                Name = "Loyal Europe",
                Value = 49.45,
                Region = "EU",
                Items = 3,
                HasLoyalityCard = true
            });
            orders.Add(new Order()
            {
                Name = "UK Order",
                Value = 49.45,
                Region = "UK",
                Items = 3,
                HasLoyalityCard = false
            });

            return orders;
        }

        static void PrompAndWait(string text)
        {
            var temp = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(text);
            Console.ForegroundColor = temp;

            Console.ReadLine();
        }
    }
}
