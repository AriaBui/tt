using Newtonsoft.Json;

namespace VismaKart.QnA
{
    public class Answer
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("correct")]
        public bool Correct { get; set; }
    }
}