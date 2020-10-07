using System.Collections.Generic;
using ChessConnection;

namespace ChessLogic
{
    public sealed class ChessGame : IChessGame
    {
        private GameLogic game;
        private bool isServerStops;
        private readonly ChessServer chessServer;

        public bool IsWhitesTurn
        {
            get
            {
                return game.IsWhitesTurn;
            }
        }

        private static ChessGame instance;

        private ChessGame(ChessServer chessServer)
        {
            this.chessServer = chessServer;

            DisconnectedPlayersCount = 0;
        }

        public static ChessGame CreateNewGame(ChessServer chessServer)
        {
            instance = new ChessGame(chessServer)
            {
                game = GameLogic.CreateNewGame()
            };
            return instance;
        }

        public Figure[,] GetField()
        {
            return game.GetField();
        }

        public bool IsCheckmate()
        {
            return game.IsCheckmate();
        }

        public bool CheckAndMakeAMove(ChessMove move)
        {
            return game.CheckAndMakeAMove(move);
        }

        public List<ChessMove> GetAvailableMoves(int x, int y)
        {
            return game.GetAvailableMoves(x, y);
        }

        public bool IsServerStops()
        {
            return isServerStops;
        }

        public int DisconnectedPlayersCount { get; private set; }

        public void Disconnect()
        {
            if (DisconnectedPlayersCount >= 1)
                chessServer.Stop();

            DisconnectedPlayersCount++;
            isServerStops = true;
        }

        public GameAndServerState GameAndServerState
            => new GameAndServerState(IsWhitesTurn, GetField(),
                IsCheckmate(), IsServerStops());

        public void Refresh()
        {

        }
    }

    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
