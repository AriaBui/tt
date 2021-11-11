using System;
using System.Linq;
using Newtonsoft.Json;

namespace VismaKart.Scenes.ScoreboardScene.Models
{
    public class Highscore
    {
        public string DisplayName
        {
            get
            {
                var nameToShow = "Anonymous";
                if (!string.IsNullOrWhiteSpace(FirstName))
                {
                    var firstName = FirstName;

                    const int maxLengthFirstname = 20;
                    if (firstName.Length > maxLengthFirstname)
                    {
                        firstName = firstName.Remove(maxLengthFirstname - 3);
                        firstName += "...";
                    }

                    nameToShow = firstName;
                }


                if (!string.IsNullOrWhiteSpace(Surname))
                {
                    nameToShow += $" {Surname.FirstOrDefault().ToString().ToUpper()}.";
                }

                return nameToShow;
            }
        } // Computed

        public string Attendeeno { get; set; }

        [JsonProperty("First Name")]
        public string FirstName { get; set; }

        public string Surname { get; set; }
        public string Email { get; set; }
        public string MobilePhone { get; set; }
        public string JobTitle { get; set; }
        public string Company { get; set; }
        public string WorkAddress1 { get; set; }
        public string WorkAddress2 { get; set; }
        public string WorkCity { get; set; }
        public string WorkPostcode { get; set; }
        public string WorkCountry { get; set; }
        public int score { get; set; }
        public bool assentGDPR { get; set; }
        public string partitionKey { get; set; }
        public string rowKey { get; set; }
        public DateTime timestamp { get; set; }
        public string eTag { get; set; }
    }

}