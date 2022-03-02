using System;
using System.Collections.Generic;
using System.Text;

namespace AzureRedisDemo
{
    public class Order
    {
        public string OrderID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
