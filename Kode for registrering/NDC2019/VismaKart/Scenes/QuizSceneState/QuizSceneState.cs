using System;
using System.Collections.Generic;
using VismaKart.QnA;

namespace VismaKart.Scenes.QuizSceneState
{
    public class QuizSceneState
    {
        public QuizState QuizState;

        public Question CurrentQuestion { get; set; }

        public List<Answer> CurrentAnswers { get; set; }

        public DateTime TimeToEnd { get; set; }

        public string SecondsLeft { get; set; }

        public int CurrentQuestionNumber { get; set; }
    }
}
