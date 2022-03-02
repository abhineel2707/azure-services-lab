using StackExchange.Redis;
using System;
using System.Text.Json;

namespace AzureRedisDemo
{
    internal class Program
    {
        private static Lazy<ConnectionMultiplexer> cacheConnection = CreateConnection();

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return cacheConnection.Value;
            }
        }
        static void Main(string[] args)
        {
            IDatabase cacheDb = Connection.GetDatabase();

            Order order = new Order()
            {
                OrderID = "O1",
                Quantity = 10,
                UnitPrice = 9.99m
            };

            cacheDb.StringSet(order.OrderID, JsonSerializer.Serialize<Order>(order));

            Order newOrder = JsonSerializer.Deserialize<Order>(cacheDb.StringGet(order.OrderID));
            Console.WriteLine($"Order Id: {newOrder.OrderID} Quantity: {newOrder.Quantity} Unit Price: {newOrder.Quantity}");
            Console.Read();
        }

        private static void WorkingWithSimpleTypes()
        {
            IDatabase cacheDb = Connection.GetDatabase();
            cacheDb.StringSet("Course Name", "AZ-204 Developing Azure Solutions");

            if (cacheDb.KeyExists("Course Name"))
            {
                string response = cacheDb.StringGet("Course Name");
                Console.WriteLine(response);
            }
            else
            {
                Console.WriteLine("The key doesn't exits");
            }

            Console.ReadLine();
        }

        private static Lazy<ConnectionMultiplexer> CreateConnection()
        {
            string connectionString = "<REDIS CONNECTION STRING>";

            return new Lazy<ConnectionMultiplexer>(() =>
            {
                return ConnectionMultiplexer.Connect(connectionString);
            });
        }
    }
}
