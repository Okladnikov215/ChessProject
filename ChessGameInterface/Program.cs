using System;
using System.Collections.Generic;

namespace ChessLogic
{
    public sealed class ChessMove
    {
        private int _startX, _startY, _endX, _endY;

        public ChessMove(int startX, int startY, int endX, int endY)
        {
            this.StartX = startX;
            this.StartY = startY;
            this.EndX = endX;
            this.EndY = endY;
        }

        public int StartX
        {
            get
            {
                return _startX;
            }

            private set
            {
                if (value > 7 && value < 0)
                    throw new ArgumentOutOfRangeException("X", "X and Y coordinates of ChessFigure must be in range from 0 to 7");
                _startX = value;
            }
        }

        public int StartY
        {
            get
            {
                return _startY;
            }

            private set
            {
                if (value > 7 && value < 0)
                    throw new ArgumentOutOfRangeException("X", "X and Y coordinates of ChessFigure must be in range from 0 to 7");
                _startY = value;
            }
        }

        public int EndX
        {
            get
            {
                return _endX;
            }

            private set
            {
                if (value > 7 && value < 0)
                    throw new ArgumentOutOfRangeException("X", "X and Y coordinates of ChessFigure must be in range from 0 to 7");
                _endX = value;
            }
        }

        public int EndY
        {
            get
            {
                return _endY;
            }

            private set
            {
                if (value > 7 && value < 0)
                    throw new ArgumentOutOfRangeException("X", "X and Y coordinates of ChessFigure must be in range from 0 to 7");
                _endY = value;
            }
        }

        public ChessMove Reverse()
        {
            return new ChessMove(EndX, EndY, StartX, StartY);
        }

        public static bool operator ==(ChessMove move1, ChessMove move2)
        {
            if (move1.StartX == move2.StartX && move1.StartY == move2.StartY && move1.EndX == move2.EndX && move1.EndY == move2.EndY)
                return true;
            return false;
        }

        public static bool operator !=(ChessMove move1, ChessMove move2)
        {
            if (move1 == move2)
                return false;
            return true;
        }

        public override bool Equals(Object O)
        {
            if (O is ChessMove move)
                return (this == move);
            return false;
        }

        public override int GetHashCode() => 1;

    }

    public enum FigureType
    {
        Pawn,
        Rook,
        Knight,
        Bishop,
        Queen,
        King
    }

    public sealed class Figure
    {
        public Figure(FigureType FigType, bool IsWhite)
        {
            this.FigType = FigType;
            this.IsWhite = IsWhite;
        }
        public FigureType FigType
        {
            get;
            private set;
        }
        public bool IsWhite
        {
            get;
            private set;
        }
    }

    public sealed class GameAndServerState
    {
        public bool IsWhitesTurn { get; }
        public Figure[,] Field { get; }
        public bool IsCheckmate { get; }
        public bool IsServerStops { get; }

        public GameAndServerState(bool IsWhitesTurn, Figure[,] Field,
            bool IsCheckmate, bool IsServerStops)
        {
            this.IsWhitesTurn = IsWhitesTurn;
            this.Field = Field;
            this.IsCheckmate = IsCheckmate;
            this.IsServerStops = IsServerStops;
        }
    }

    public interface IChessGame
    {
        bool IsWhitesTurn { get; }
        Figure[,] GetField();
        bool CheckAndMakeAMove(ChessMove move);
        bool IsCheckmate();
        List<ChessMove> GetAvailableMoves(int x, int y);
        bool IsServerStops();
        void Disconnect();
        void Refresh();
        GameAndServerState GameAndServerState{ get; }
    }

    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
