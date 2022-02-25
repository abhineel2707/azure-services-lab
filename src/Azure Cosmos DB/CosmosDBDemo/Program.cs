using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CosmosDBDemo
{
    internal class Program
    {
        private static string connectionString = "";
        private static string databaseName = "appdb";
        private static string containerName = "course";
        private static string partitionKey = "/courseid";
        static async Task Main(string[] args)
        {
            //await InsertData();

            //await QueryData();

            //await UpdateData();

            //await DeleteData();

            // await WorkingWithStoredProcedure();

            //await AddItemUsingStoredProcedure();

            //await AddingPropertiesUsingPreTrigger();

            await WorkingWithCompositeKeys();

            Console.ReadKey();
        }

        private static async Task InsertData()
        {
            CosmosClient cosmosClient = new CosmosClient(connectionString, new CosmosClientOptions()
            {
                AllowBulkExecution = true
            });

            await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName);

            Database database = cosmosClient.GetDatabase(databaseName);

            await database.CreateContainerIfNotExistsAsync(containerName, partitionKey);

            Console.WriteLine("Database and continer have been created.");

            Container container = cosmosClient.GetContainer(databaseName, containerName);

            #region Add item to container

            Course course = new Course()
            {
                id = "1",
                courseid = "C00010",
                coursename = "Az-204",
                rating = 4.5m
            };


            await container.CreateItemAsync<Course>(course, new PartitionKey(course.courseid));

            Console.WriteLine("Course inserted successfully.");

            #endregion

            #region Bulk Insert

            List<Course> courses = new List<Course>()
            {
                new Course(){ id="1", courseid="Course0001", coursename="AZ-204 Developing Azure Solutions", rating=4.5m },
                new Course(){ id="2", courseid="Course0002", coursename="AZ-303 Architecting Azure Solutions", rating=4.6m },
                new Course(){ id="3", courseid="Course0003", coursename="DP-203 Azure Data Engineer", rating=4.7m },
                new Course(){ id="4", courseid="Course0004", coursename="AZ-900 Azure Fundamentals", rating=4.6m },
                new Course(){ id="5", courseid="Course0005", coursename="AZ-104 Azure Administrator", rating=4.5m }
            };

            List<Task> tasks = new List<Task>();

            foreach (var item in courses)
            {
                tasks.Add(container.CreateItemAsync<Course>(item, new PartitionKey(item.courseid)));
            }

            await Task.WhenAll(tasks);

            #endregion
        }

        private static async Task QueryData()
        {
            CosmosClient client = new CosmosClient(connectionString);

            Container container = client.GetContainer(databaseName, containerName);

            string query = "SELECT * FROM c WHERE c.courseid='Course0002'";

            QueryDefinition queryDefinition = new QueryDefinition(query);

            FeedIterator<Course> feedIterator = container.GetItemQueryIterator<Course>(queryDefinition);

            while (feedIterator.HasMoreResults)
            {
                FeedResponse<Course> response = await feedIterator.ReadNextAsync();

                foreach (Course course in response)
                {
                    Console.WriteLine($"Id is {course.id}");
                    Console.WriteLine($"Courseid is {course.courseid}");
                    Console.WriteLine($"Course name is {course.coursename}");
                    Console.WriteLine($"Course rating is {course.rating}");
                }
            }
        }

        private static async Task UpdateData()
        {
            CosmosClient cosmosClient = new CosmosClient(connectionString);

            Container container = cosmosClient.GetContainer(databaseName, containerName);

            string id = "2";
            string partitionKey = "Course0002";

            ItemResponse<Course> response = await container.ReadItemAsync<Course>(id, new PartitionKey(partitionKey));

            Course course = response.Resource;

            course.rating = 4.8m;

            await container.ReplaceItemAsync<Course>(course, id, new PartitionKey(partitionKey));

            Console.WriteLine("Item has been updated");
        }

        private static async Task DeleteData()
        {
            CosmosClient cosmosClient = new CosmosClient(connectionString);

            Container container = cosmosClient.GetContainer(databaseName, containerName);

            string id = "2";
            string partitionKey = "Course0002";

            await container.DeleteItemAsync<Course>(id, new PartitionKey(partitionKey));

            Console.WriteLine("Item has been deleted");
        }

        private static async Task WorkingWithStoredProcedure()
        {
            CosmosClient cosmosClient = new CosmosClient(connectionString);

            Container container = cosmosClient.GetContainer(databaseName, containerName);

            string result = await container.Scripts.ExecuteStoredProcedureAsync<string>("demo", new PartitionKey(String.Empty), null);

            Console.WriteLine(result);
        }

        private static async Task AddItemUsingStoredProcedure()
        {
            CosmosClient client = new CosmosClient(connectionString);

            Container container = client.GetContainer(databaseName, containerName);

            dynamic[] items = new dynamic[]
            {
                new {id="6", courseid="Course0006", coursename="AZ-500 Azure Security", rating=4.4m}
            };

            string output = await container.Scripts.ExecuteStoredProcedureAsync<string>("Additem", new PartitionKey("Course0006"), new[] { items });

            Console.WriteLine("Item added successfully");
        }

        private static async Task AddingPropertiesUsingPreTrigger()
        {
            CosmosClient cosmosClient = new CosmosClient(connectionString);

            Container container = cosmosClient.GetContainer(databaseName, containerName);

            Course course = new Course()
            {
                id = "7",
                courseid = "Course0007",
                coursename = "SC-900 Azure Security Fundamentals",
                rating = 4.7m
            };

            ItemResponse<Course> response = await container.CreateItemAsync<Course>(course, new PartitionKey(course.courseid), new ItemRequestOptions { PreTriggers = new List<string> { "Addtimestamp" } });

            Console.WriteLine("Item is created successfully");
        }

        private static async Task WorkingWithCompositeKeys()
        {
            string newContainerName = "newcourse";
            string newPartitionKey = "/coursename";

            CosmosClient client = new CosmosClient(connectionString);

            Database database = client.GetDatabase(databaseName);

            await database.CreateContainerIfNotExistsAsync(newContainerName, newPartitionKey);

            Container container = client.GetContainer(databaseName, newContainerName);

            List<Course> courses = new List<Course>()
            {
                new Course(){ id = "1",courseid = "C00010",coursename = "AZ-204",rating = 4.5m },
                new Course(){ id = "2",courseid = "C00011",coursename = "AZ-303",rating = 4.6m },
                new Course(){ id = "3",courseid = "C00012",coursename = "AZ-304",rating = 4.5m },
                new Course(){ id = "1",courseid = "C00010",coursename = "AZ-204",rating = 4.5m },

            }
        }
    }
}
