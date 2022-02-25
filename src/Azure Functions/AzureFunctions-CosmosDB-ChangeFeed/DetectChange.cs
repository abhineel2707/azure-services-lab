using System;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace AzureFunctions_CosmosDB_ChangeFeed
{
    public static class DetectChange
    {
        [FunctionName("ReadChange")]
        public static void Run([CosmosDBTrigger(
            databaseName: "appdb",
            collectionName: "course",
            ConnectionStringSetting = "cosmosdb",
            LeaseCollectionName = "leases",CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> input, ILogger log)
        {
            if (input != null && input.Count > 0)
            {
                foreach (var course in input)
                {
                    log.LogInformation($"Id is {course.GetPropertyValue<string>("id")}");
                    log.LogInformation($"Course id is {course.GetPropertyValue<string>("courseid")}");
                    log.LogInformation($"Course name is {course.GetPropertyValue<string>("coursename")}");
                    log.LogInformation($"Course rating is {course.GetPropertyValue<decimal>("rating")}");
                }

                //log.LogInformation("Documents modified " + input.Count);
                //log.LogInformation("First document Id " + input[0].Id);
            }
        }
    }
}
