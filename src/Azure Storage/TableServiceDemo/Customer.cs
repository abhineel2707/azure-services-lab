using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace TableServiceDemo
{
    class Customer : TableEntity
    {
        public string CustomerName { get; set; }
        public Customer()
        {

        }

        public Customer(string customerName, string city, string customerId)
        {
            PartitionKey = city;
            RowKey = customerId;
            this.CustomerName = customerName;
        }
    }
}
