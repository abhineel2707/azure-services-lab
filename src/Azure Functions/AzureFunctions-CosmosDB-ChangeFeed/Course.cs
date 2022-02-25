using System;
using System.Collections.Generic;
using System.Text;

namespace AzureFunctions_CosmosDB_ChangeFeed
{
    public class Course
    {
        public string id { get; set; }
        public string courseid { get; set; }
        public string coursename { get; set; }
        public decimal rating { get; set; }
    }
}
