using VismaKart.Player;

namespace VismaKart.GlobalStuff
{
    public static class GlobalGameState
    {
        public static GameState CurrentState { get; set; }

        public static VismaKartPlayer PlayerOne = new VismaKartPlayer("Player 1");
        public static VismaKartPlayer PlayerTwo = new VismaKartPlayer("Player 2");
    }
}
