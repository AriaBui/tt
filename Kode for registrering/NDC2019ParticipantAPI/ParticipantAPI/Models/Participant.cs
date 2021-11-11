using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using ParticipantAPI.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParticipantAPI.Models
{
    public class Participant : TableEntity
    {
        public Participant()
        {

        }

        public Participant(string attendeeId, bool assentGDPR) : this(attendeeId)
        {
            AssentGDPR = assentGDPR;
        }

        [JsonConstructor]
        public Participant(string attendeeId)
        {
            Timestamp = DateTime.Now;
            RowKey = attendeeId;
            AttendeeId = attendeeId;
            PartitionKey = AzureTableSettings.PARTITION_KEY;
            HasTriedUpdate = false;
        }

        [JsonProperty("Attendee no.")]
        public string AttendeeId { get; set; }

        [JsonProperty("First Name")]
        public string FirstName { get; set; }

        [JsonProperty("Surname")]
        public string Surname { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("Mobile Phone")]
        public string PhoneNumber { get; set; }

        [JsonProperty("Job Title")]
        public string JobTite { get; set; }

        [JsonProperty("Company")]
        public string Company { get; set; }

        [JsonProperty("Work Address 1")]
        public string WorkAddress1 { get; set; }

        [JsonProperty("Work Address 2")]
        public string WorkAddress2 { get; set; }

        [JsonProperty("Work City")]
        public string WorkCity { get; set; }

        [JsonProperty("Work Postcode")]
        public string WorkPostcode { get; set; }

        [JsonProperty("Work Country")]
        public string WorkCountry { get; set; }

        public long Score { get; set; }

        public bool AssentGDPR{ get; set; }

        public bool HasTriedUpdate { get; set; }
    }
}
