using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VismaKart.Electronics
{
    class NumberOfQuestionsGoalController : IGoalController
    {
        private int _numberOfQuestions;
        private int _currentQuestionNumber;
        private int _player1Score;
        private int _player2Score;

        public NumberOfQuestionsGoalController(int numberOfQuestions)
        {
            _numberOfQuestions = numberOfQuestions;
        }

        public void updateGoalController(int currentQuestion, int player1Score, int player2Score)
        {
            _currentQuestionNumber = currentQuestion;
            _player1Score = player1Score;
            _player2Score = player2Score;
        }

        public async Task<bool> Setup()
        {
            await Task.CompletedTask;
            return true;
        }

        public bool Player1IsInGoal()
        {
            if (_currentQuestionNumber >= _numberOfQuestions)
            {
                return _player1Score >= _player2Score;
            }
            return false;
        }

        public bool Player2IsInGoal()
        {
            if (_currentQuestionNumber >= _numberOfQuestions)
            {
                return _player2Score >= _player1Score;
            }
            return false;
        }
    }
}
