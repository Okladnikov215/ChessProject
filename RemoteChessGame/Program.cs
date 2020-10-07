using System.Collections.Generic;
using ChessLogic;

namespace ChessConnection
{
    public sealed class RemoteChessGame : IChessGame
    {
        private readonly ChessClient chessClient;

        public GameAndServerState GameAndServerState
        {
            get; private set;
        }

        public bool IsWhitesTurn
        {
            get
            {
                return GameAndServerState.IsWhitesTurn;
            }
        }

        public RemoteChessGame(ChessClient chessClient)
        {
            this.chessClient = chessClient;
        }

        public Figure[,] GetField()
        {
            return GameAndServerState.Field;
        }

        public bool IsCheckmate()
        {
            return GameAndServerState.IsCheckmate;
        }

        public bool IsServerStops()
        {
            return GameAndServerState.IsServerStops;
        }

        public bool CheckAndMakeAMove(ChessMove move)
        {
            return (chessClient.InvokeMethod<bool?>("CheckAndMakeAMove", new object[] { move })).Value;
        }

        public List<ChessMove> GetAvailableMoves(int x, int y)
        {
            return chessClient.InvokeMethod<List<ChessMove>>("GetAvailableMoves", new object[] { x, y });
        }

        public void Disconnect()
        {
            chessClient.InvokeMethod("Disconnect", null);
            chessClient.CloseConnection();
        }

        public void Refresh()
        {
            GameAndServerState = chessClient.InvokeMethod<GameAndServerState>("GameAndServerState", null);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
