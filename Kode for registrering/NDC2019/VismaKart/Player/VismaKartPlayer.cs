using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VismaKart.Scenes.ScoreboardScene.Models;

namespace VismaKart.Player
{
    public class VismaKartPlayer
    {
        private Task<HttpResponseMessage> _postQrCodeTask;
        private string _name;

        public string Id { get; set; }

        public string Name
        {
            get => _name;
            set =>
                _name = value?.Replace("æ", "ae")
                    .Replace("Æ", "Ae")
                    .Replace("ø", "o")
                    .Replace("Ø", "O")
                    .Replace("å", "aa")
                    .Replace("Å", "Aa")
                    .Replace("ñ", "n")
                    .Replace("è", "e")
                    .Replace("é", "e");
        }

        public int Score { get; set; }

        public int ScoreThisRound { get; set; }

        public double ScoreBoostNextRound { get; set; }

        public double MaxScorePossibleThisRound { get; set; }

        public double CurrentScoreBoost { get; set; } = 1;

        public bool HasAnswered { get; set; }

        public bool CorrectAnswer { get; set; }

        public Task<HttpResponseMessage> PostQrCodeTask
        {
            set
            {
                _postQrCodeTask = value;
                Task.Run(async () =>
                {
                    var result = await _postQrCodeTask;
                    var content = await result.Content.ReadAsStringAsync();
                    var h = JsonConvert.DeserializeObject<Highscore>(content);

                    Name = h.DisplayName;
                });
            }
        }

        public VismaKartPlayer(string userName)
        {
            Name = userName;
        }

        public void UpdateScoreThisRound()
        {
            if (CorrectAnswer)
            {
                ScoreThisRound = (int)(MaxScorePossibleThisRound * CurrentScoreBoost);
            }

            // Hvis boost er 1 og ingen svar: punish!
            if (!HasAnswered && CurrentScoreBoost < 1.01)
            {
                ScoreBoostNextRound = CurrentScoreBoost * 0.8; // 20% loss!
            }
            // Hvis boost er allerede negativ, men spilleren svarer: Rett opp til 1
            else if (!CorrectAnswer && HasAnswered && CurrentScoreBoost < 1)
            {
                ScoreBoostNextRound = 1;

            }
            // Ved korrekt svar, minst opp til 1.2 i boost. Eller mer hvis boost var høyere fra før.
            else if (CorrectAnswer && CurrentScoreBoost < 2.0)
            {
                ScoreBoostNextRound = Math.Max(1.2, CurrentScoreBoost * 1.2); // 20% increase.
            }
            else
            {
                // Feil svar :( sett boost til 1 igjen.
                ScoreBoostNextRound = 1;
            }
        }

        public void UpdateScore()
        {
            UpdateScoreThisRound();
            // Dobbel-sjekk siden det er noe feil en annen plass
            if (HasAnswered && CorrectAnswer)
            {
                Score += (int) (ScoreThisRound * CurrentScoreBoost);
            }

            CurrentScoreBoost = ScoreBoostNextRound;
        }
    }
}
