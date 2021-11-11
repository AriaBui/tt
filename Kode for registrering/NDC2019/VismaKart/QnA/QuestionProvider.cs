using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using VismaKart.Utils;

namespace VismaKart.QnA
{
    public class QuestionProvider
    {
        private List<Question> _questions;
        private int _nextQuestion;

        public QuestionProvider()
        {
            ReadQuestionFile();
            VerifyQuestionsAndAnswers();
            _questions.Shuffle();
        }

        public Question GetNextQuestion()
        {
            if (_nextQuestion >= _questions.Count) _nextQuestion = 0;

            var nextQuestion = _questions[_nextQuestion];
            _nextQuestion++;
            return nextQuestion;
        }

        private void VerifyQuestionsAndAnswers()
        {
            var i = 1;
            foreach (var question in _questions)
            {
                if (string.IsNullOrWhiteSpace(question.Text))
                {
                    throw new Exception($"Spørsmål nummer {i} har ikke tekst.");
                }

                if (question.Answers == null)
                {
                    throw new Exception($"Spørsmål nummer {i} har ikke svar.");
                }

                if (question.Answers.Count < 4)
                {
                    throw new Exception($"Spørsmål nummer {i} har ikke 4 svar. (bare {question.Answers.Count})");
                }

                if (!question.Answers.Any(a => a.Correct))
                {
                    throw new Exception($"Spørsmål nummer {i} har ikke noen svar som er markert som korrekt.");
                }

                i++;
            }
        }

        private void ReadQuestionFile()
        {
            var client = HttpClientFactory.Create();

            string text;
            try
            {
                text = client
                    .GetStringAsync("https://raw.githubusercontent.com/VismaConsulting/QnA/master/qna.json")
                    .GetAwaiter()
                    .GetResult();
            }
            catch
            {
                text = File.ReadAllText("questions_large.json");
            }

            _questions = JsonConvert.DeserializeObject<List<Question>>(text);
        }
    }
}
