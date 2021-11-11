using System;

namespace ParticipantAPI.Configuration
{
    public class AzureTableSettings
    {
        private const string PARTITION_KEY_PREFIX = "NDC_";
        public static readonly string PARTITION_KEY = PARTITION_KEY_PREFIX + DateTime.Now.Year;

        public AzureTableSettings(string storageAccount,
                                     string storageKey,
                                     string tableName)
        {
            if (string.IsNullOrEmpty(storageAccount))
                throw new ArgumentNullException("StorageAccount");

            if (string.IsNullOrEmpty(storageKey))
                throw new ArgumentNullException("StorageKey");

            if (string.IsNullOrEmpty(tableName))
                throw new ArgumentNullException("TableName");

            this.StorageAccount = storageAccount;
            this.StorageKey = storageKey;
            this.TableName = tableName;
        }

        public string StorageAccount { get; }
        public string StorageKey { get; }
        public string TableName { get; }
    }
}
