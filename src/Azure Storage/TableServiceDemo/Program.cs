using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;

namespace TableServiceDemo
{
    internal class Program
    {
        private static string connectionString = "";
        private static string tableName = "Customer";

        static void Main(string[] args)
        {
            InsertData();

            QueryData();

            UpdateData();

            DeleteData();
        }

        private static void InsertData()
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(connectionString);

            CloudTableClient cloudTableClient = cloudStorageAccount.CreateCloudTableClient();

            CloudTable cloudTable = cloudTableClient.GetTableReference(tableName);

            if (!cloudTable.Exists())
            {
                cloudTable.CreateIfNotExists();
                Console.WriteLine("Table has been created");
            }

            #region Insert Single Entity

            Customer customerEntity = new Customer("UserA", "Chicago", "C1");

            TableOperation tableOperation = TableOperation.Insert(customerEntity);

            TableResult result = cloudTable.Execute(tableOperation);

            Console.WriteLine("Single entity operation completed successfully.");

            #endregion

            // Entities in a batch should have same partition key.
            List<Customer> customers = new List<Customer>()
            {
                new Customer("UserB","Chicago","C2"),
                new Customer("UserC","Chicago","C3"),
                new Customer("UserD","Chicago","C4"),
            };

            TableBatchOperation batchOperation = new TableBatchOperation();
            foreach (var customer in customers)
            {
                batchOperation.Insert(customer);
            }

            TableBatchResult batchResult = cloudTable.ExecuteBatch(batchOperation);

            Console.WriteLine("Batch operation completed successfully.");

            Console.ReadLine();
        }

        private static void QueryData()
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(connectionString);

            CloudTableClient client = account.CreateCloudTableClient();

            CloudTable table = client.GetTableReference(tableName);

            string partitionKey = "Chicago";
            string rowKey = "C1";

            TableOperation operation = TableOperation.Retrieve<Customer>(partitionKey, rowKey);

            TableResult result = table.Execute(operation);

            Customer customer = result.Result as Customer;

            Console.WriteLine($"The customer name is {customer.CustomerName}");
            Console.WriteLine($"The customer city is {customer.PartitionKey}");
            Console.WriteLine($"The customer id is {customer.RowKey}");

        }

        private static void UpdateData()
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(connectionString);

            CloudTableClient tableClient = account.CreateCloudTableClient();

            CloudTable table = tableClient.GetTableReference(tableName);

            string partitionKey = "Chicago";
            string rowKey = "C2";

            Customer customer = new Customer("UserE", partitionKey, rowKey);
            TableOperation operation = TableOperation.InsertOrMerge(customer);

            TableResult result = table.Execute(operation);

            Console.WriteLine("Customer C2 updated successfully.");
        }

        private static void DeleteData()
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(connectionString);

            CloudTableClient tableClient = account.CreateCloudTableClient();

            CloudTable table = tableClient.GetTableReference(tableName);

            string partitionKey = "Chicago";
            string rowKey = "C2";

            TableOperation retrieveOp = TableOperation.Retrieve<Customer>(partitionKey, rowKey);

            TableResult result = table.Execute(retrieveOp);

            Customer customer = result.Result as Customer;

            TableOperation deleteOp = TableOperation.Delete(customer);

            TableResult deleteResult = table.Execute(deleteOp);

            Console.WriteLine("Customer C2 deleted successfully.");
        }
    }
}
