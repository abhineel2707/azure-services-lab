using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureFunctions_StorageQueue
{
    public class Order
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
