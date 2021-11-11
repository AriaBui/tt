using System.Collections.Generic;
using System.Linq;
using VismaKart.Utils;

namespace VismaKart.QnA
{
    public static class AnswerPicker
    {
        public static List<Answer> GetFourAnswersWithAtLeastOneCorrect(QnA.Question question)
        {
            question.Answers.Shuffle();
            var pickedAnswers = question.Answers.Take(4).ToList();

            if (pickedAnswers.Any(a => a.Correct)) return pickedAnswers;

            // If we have no correct answers, discard a random one (they are shuffled)
            // And insert a correct answers. Also reshuffle.
            pickedAnswers[0] = question.Answers.First(a => a.Correct);
            pickedAnswers.Shuffle();

            return pickedAnswers;
        }
    }
}
