using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class CountTableEntity: TableEntity
    {
        public CountTableEntity()
        {

        }
        public CountTableEntity(string key,int number)
        {
            PartitionKey = "CountTable";
            RowKey = key;
            NumberOfCalls = number;
        }
        public int NumberOfCalls { get; set; }
    }
}
