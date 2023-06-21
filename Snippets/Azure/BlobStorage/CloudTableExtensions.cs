using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using ginsjuan.Models;

namespace ginsjuan.Extensions
{
    public static class CloudTableExtensions
    {
        /// <summary>
        /// checks if a session is locked in a Cloud Storage Table and returns a boolean value indicating the lock status.
        /// </summary>
        /// <param name="cloudTable"></param>
        /// <param name="sessionId">Session Id to check</param>
        /// <returns>If the session is locked return true, otherwise return false</returns>
        public static async Task<bool> IsSessionLockedAsync(this CloudTable cloudTable, string sessionId)
        {
            try
            {
                //Check first if the session is already locked

                TableQuery<LeaseEntity> query = new TableQuery<LeaseEntity>()
                    .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, sessionId));


                var queryResult = cloudTable.ExecuteQuery<LeaseEntity>(query);

                if (queryResult.Any())
                {

                    var existingLease = queryResult.First();
                    //Check if the session is already locked by another process or thread

                    return existingLease.isLocked;
                }

            }
            catch (Exception ex)
            {
                // If the cloudTable is already leased by another instance, the lease acquisition will fail
                return false;
            }

            return false;
        }


        /// <summary>
        /// Acquires a session lock for a given session ID in an Azure Storage Table. If the lock already exists, it checks if the lock has expired or is still active. 
        /// If the lock doesn't exist, it creates a new lock. 
        /// Returns a boolean indicating the lock acquisition result and the associated lease entity.
        /// </summary>
        /// <param name="cloudTable"></param>
        /// <param name="sessionId">Session Id used as a seed for this lock</param>
        /// <returns>Returns a boolean indicating the lock acquisition result and the associated lease entity.</returns>
        public static async Task<(bool result, LeaseEntity lease)> AcquireSessionLockAsync(this CloudTable cloudTable, string sessionId)
        {
            try
            {
                //Check first if the session is already locked

                TableQuery<LeaseEntity> query = new TableQuery<LeaseEntity>()
                    .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, sessionId));


                //If the lease already existed and should be updated
                TimeSpan leaseDuration = TimeSpan.FromSeconds(80);

                #if DEBUG //Use larger time if debugging code
                        leaseDuration = TimeSpan.FromSeconds(280);
                #endif

                var queryResult = cloudTable.ExecuteQuery<LeaseEntity>(query);

                if (queryResult.Any())
                {

                    var existingLease = queryResult.First();
                    //Check if the session is already locked by another process or thread

                    if (existingLease != null && existingLease.isLocked)
                    {
                        //If the lease is locked but the lock expired then release the lock
                        if (DateTime.UtcNow >= existingLease.leaseExpiration)
                        {
                             
                            (var wasReleased, existingLease) = await ReleaseSessionLockAsync(cloudTable, existingLease).ConfigureAwait(false);

                            //If the lock was succesfully released then obtain a new lease
                            if (wasReleased)
                            {
                                //Reaquire a new lease by updating the table 
                                return await UpdateOrInsertLeaseAsync(existingLease, leaseDuration, cloudTable).ConfigureAwait(false);
                            }

                        }

                        //if the lease still locked then return false
                        return (false, existingLease);
                    }
                    else
                    {

                        //Reaquire a new lease by updating the table
                        return await UpdateOrInsertLeaseAsync(existingLease, leaseDuration, cloudTable).ConfigureAwait(false);
                    }

                }
                else
                {
                    //If the lock doesn't exist then create it when a new lock
                    return await UpdateOrInsertLeaseAsync(new LeaseEntity(sessionId) {}, leaseDuration, cloudTable).ConfigureAwait(false);
                    
                }

            }
            catch (Exception ex)
            {
                // If the cloudTable is already leased by another instance, the lease acquisition will fail
                return (false, null);
            }
     
        }

        /// <summary>
        /// Releases a session lock for a given lease entity in an Azure Storage Table. 
        /// It checks if the lock exists and updates the lease entity to mark the lock as released. 
        /// Returns a boolean indicating the lock release result and the updated lease entity.
        /// </summary>
        /// <param name="cloudTable"></param>
        /// <param name="lease">Session Id to be released</param>
        /// <returns>Returns a boolean indicating the lock release result and the updated lease entity.</returns>
        public static async Task<(bool result, LeaseEntity lease)> ReleaseSessionLockAsync(this CloudTable cloudTable, LeaseEntity lease)
        {
            try
            {
                //If the lease already existed and should be updated
                TimeSpan leaseDuration = TimeSpan.FromSeconds(0);

                //Check first if the session is already locked
                TableQuery<LeaseEntity> query = new TableQuery<LeaseEntity>()
                  .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, lease.PartitionKey));


                var existingLease = cloudTable.ExecuteQuery<LeaseEntity>(query);

                if (existingLease.Any() && DateTime.UtcNow >= lease.leaseExpiration)
                {
                    //Reaquire a new lease by updating the table
                    return await UpdateOrInsertLeaseAsync(existingLease.First(), leaseDuration, cloudTable, false).ConfigureAwait(false);
                }

            }
            catch (Exception ex)
            {
                // If the cloudTable is already leased by another instance, the lease acquisition will fail
                return (false, null);
            }

            return (false, null);
        }

        /// <summary>
        /// Executes an insert or update operation on an Azure Storage Table using the provided lease entity.
        /// It performs the operation asynchronously and returns a tuple containing the updated lease entity 
        /// and the HTTP result code indicating the outcome of the operation.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="lease">Object to be inserted or updated</param>
        /// <returns>returns a tuple containing the updated lease entity 
        /// and the HTTP result code indicating the outcome of the operation.</returns>
        private static async Task<(LeaseEntity lease, int httpResult)> ExecuteInsertOrUpdateAsync(CloudTable table, LeaseEntity lease)
        {
            TableOperation insertOrUpdate = TableOperation.InsertOrReplace(lease);
            TableResult result = await table.ExecuteAsync(insertOrUpdate).ConfigureAwait(true);

            return (result.Result as LeaseEntity, result.HttpStatusCode);
        }


        /// <summary>
        /// Updates or Inserts a new entry in the CloudTable
        /// </summary>
        /// <param name="leaseEntity">The entity lease to be updated or created</param>
        /// <param name="leaseDuration">The duration of this lease, if the current time is greater than this value, then the lease will be released</param>
        /// <param name="cloudTable">The cloud table to update</param>
        /// <param name="lockLease">Boolean value, representing if the lease should be locked or not, by default is true</param>
        /// <returns></returns>
        internal static async Task<(bool result, LeaseEntity lease)> UpdateOrInsertLeaseAsync(LeaseEntity leaseEntity, TimeSpan leaseDuration, CloudTable cloudTable, bool lockLease = true) 
        {
            leaseEntity.leaseExpiration = leaseDuration.Seconds <= 0 ? DateTime.MaxValue : DateTime.UtcNow.AddMinutes(leaseDuration.Minutes).AddSeconds(leaseDuration.Seconds);
            leaseEntity.isLocked = lockLease;
            leaseEntity.executionStamp = DateTime.MaxValue;

            //Perform the table operation to add the new lock
            (LeaseEntity lease, int result) = await ExecuteInsertOrUpdateAsync(cloudTable, leaseEntity).ConfigureAwait(false);

            //If the operation was successful then return the lease
            if (result.ToString().StartsWith("20"))
            {
                //Lock set successfully
                return (true, lease);
            }

            return (false, null);
        }
    }
}
