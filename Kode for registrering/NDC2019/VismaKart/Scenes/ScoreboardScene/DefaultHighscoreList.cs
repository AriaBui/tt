using System.Collections.Generic;
using VismaKart.Scenes.ScoreboardScene.Models;

namespace VismaKart.Scenes.ScoreboardScene
{
    public static class DefaultHighscoreList
    {
        public static List<Highscore> Default = new List<Highscore>
        {
            new Highscore { FirstName = "Nasty Nick", score = 1000 },
            new Highscore { FirstName = "Jane Honda", score = 900 },
            new Highscore { FirstName = "Sam Speed", score = 800 },
            new Highscore { FirstName = "Duke Nukem", score = 700 },
            new Highscore { FirstName = "Clint West", score = 600 },
            new Highscore { FirstName = "Matt Miller", score = 500 },
            new Highscore { FirstName = "Motor Mary", score = 400 },
            new Highscore { FirstName = "Mad Mac", score = 300 },
            new Highscore { FirstName = "Farmer Ted", score = 200 },
            new Highscore { FirstName = "Bogus Bill", score = 100 }
        };
    }
}
