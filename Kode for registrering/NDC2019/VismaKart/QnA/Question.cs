using System.Collections.Generic;
using Newtonsoft.Json;

namespace VismaKart.QnA
{
    public class Question
    {
        [JsonProperty("q")]
        public string Text { get; set; }


        [JsonProperty("a")]
        public List<Answer> Answers { get; set; }
    }
}
