using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace AzureFunctions_StorageQueue
{
    public static class QueueMessage
    {
        [FunctionName("GetMessages")]
        [return: Table("Orders", Connection = "storage-connectionstring")]
        public static Order Run([QueueTrigger("appqueue", Connection = "storage-connectionstring")] JObject myQueueItem, ILogger log)
        {
            Order order = new Order();
            order.PartitionKey = myQueueItem["Category"].ToString();
            order.RowKey = myQueueItem["OrderID"].ToString();
            order.Quantity = Convert.ToInt32(myQueueItem["Quantity"]);
            order.UnitPrice = Convert.ToDecimal(myQueueItem["UnitPrice"]);

            return order;
        }
    }
}
