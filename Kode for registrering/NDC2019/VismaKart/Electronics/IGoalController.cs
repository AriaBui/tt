using System.Threading.Tasks;

namespace VismaKart.Electronics
{
    public interface IGoalController
    {
        Task<bool> Setup();
        bool Player1IsInGoal();
        bool Player2IsInGoal();
    }
}
