using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System;
using System.Text;

namespace QueueServiceDemo
{
    internal class Program
    {
        private static string connectionString = "DefaultEndpointsProtocol=https;AccountName=abhineelstorage01;AccountKey=mouu30yNH4aFALzGv2ajtSYRbBJIoahDwoxE0s8OJ3FGOs3tjb/9Pdv1RAxrGrRli62E2y+PTr65mrK/tiLKQQ==;EndpointSuffix=core.windows.net";
        private static string queueName = "appqueue";
        static void Main(string[] args)
        {
            QueueClient queueClient = new QueueClient(connectionString, queueName);

            queueClient.CreateIfNotExists();

            #region Send Message

            string msg, tempMsg;
            for (int i = 0; i < 5; i++)
            {
                tempMsg = $"This is test message {i + 1}";
                var textBytes = Encoding.UTF8.GetBytes(tempMsg);
                msg = Convert.ToBase64String(textBytes);
                queueClient.SendMessage(msg);
            }

            Console.WriteLine("All the messages have been sent");

            #endregion

            #region Peek Messages

            // Peek operation will not remove messages from the queue
            PeekedMessage[] peekedMessage = queueClient.PeekMessages(2);
            foreach (var message in peekedMessage)
            {
                Console.WriteLine($"Message Id is : {message.MessageId}");
                Console.WriteLine($"Message inserted on : {message.InsertedOn}");
                Console.WriteLine($"Message body is : {message.Body.ToString()}");
            }

            #endregion

            #region Receive and Delete message from queue

            QueueMessage queueMessage = queueClient.ReceiveMessage();

            Console.WriteLine(queueMessage.Body.ToString());

            queueClient.DeleteMessage(queueMessage.MessageId, queueMessage.PopReceipt);

            Console.WriteLine("Message deleted");

            #endregion

            Console.ReadKey();
        }
    }
}
