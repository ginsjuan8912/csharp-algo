using Microsoft.Azure.Cosmos.Table;
using System;

namespace ginsjuan.Models
{
    public class LeaseEntity : TableEntity
    {
        public LeaseEntity() { }
        public LeaseEntity(string sessionId)
        {
            PartitionKey = sessionId;
            RowKey = sessionId;
        }

        public bool isLocked { get; set; }
        public DateTime executionStamp { get; set; }
        public DateTime leaseExpiration { get; set; }

        public override string ToString()
        {
            return $"Lock [SessionId={PartitionKey}, isLocked={isLocked}, leaseExpiration={leaseExpiration}]";
        }
    }
}
