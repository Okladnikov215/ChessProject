using ChessConnection;

namespace ChessLogic
{
    public sealed class RemoteChessGameFactory
    {
        private static RemoteChessGameFactory instance;

        private RemoteChessGameFactory()
        {
         
        }
        public IChessGame GetChessGame(IConnection connection)
        {
            return new RemoteChessGame(new ChessConnection.ChessClient(connection));
        }

        public static RemoteChessGameFactory GetInstance()
        {
            if (instance == null)
                instance = new RemoteChessGameFactory();
            return instance;
        }
    }

    public sealed class ChessGameFactory
    {
        private static ChessGameFactory instance;

        private ChessGameFactory()
        {

        }
        public IChessGame GetChessGame(IConnection connection)
        {
            ChessServer chessServer = new ChessServer(connection);
            IChessGame game = ChessGame.CreateNewGame(chessServer);
            chessServer.Game = game;
            chessServer.Start();
            return game;
        }

        public static ChessGameFactory GetInstance()
        {
            if (instance == null)
                instance = new ChessGameFactory();
            return instance;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
