using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessLogic
{
    public interface ICloneableFigure
    {
        object Clone(ChessPlayingField curPlField);
    }

    public abstract class ChessFigure : ICloneableFigure
    {
        public ChessPlayingField CurPlField { get; private set; }

        public List<ChessMove> AvailableMoves { get; protected set; }

        private ChessFigure()
        {

        }
        public ChessFigure(ChessPlayingField curPlField)
        {
            this.CurPlField = curPlField;
        }

        public abstract void CalculateAvailableMoves(int Y, int X);

        public bool CheckMove(ChessMove move)
        {
            if (GameLogic.GetInstance().IsWhitesTurn == IsWhite())
                foreach (var curMove in AvailableMoves)
                {
                    Console.Write("{0},{1},{2},{3} ", curMove.StartX, curMove.StartY, curMove.EndX, curMove.EndY);
                    Console.Write("{0},{1},{2},{3} ", move.StartX, move.StartY, move.EndX, move.EndY);
                    Console.WriteLine(curMove == move);

                    if (curMove == move)
                        return true;
                }
            return false;
        }

        public abstract bool IsWhite();

        public bool IsSameColourAs(ChessFigure figure)
        {
            return (IsWhite() == figure.IsWhite());
        }

        public abstract FigureType GetFigureType();

        public abstract bool IsKing();

        public virtual object Clone(ChessPlayingField curPlField)
        {
            object cloneFigure = Activator.CreateInstance(GetType());
            (cloneFigure as ChessFigure).CurPlField = curPlField;
            (cloneFigure as ChessFigure).AvailableMoves = AvailableMoves.ToList<ChessMove>();
            return cloneFigure;
        }
    }

    public abstract class ChessPawn : ChessFigure
    {
        public ChessPawn(ChessPlayingField curPlField) : base(curPlField)
        {

        }

        protected bool CanMove(int X, int Y)
        {
            return ChessPlayingField.CheckRanges(Y, X) && CurPlField.CheckEmpty(Y, X);
        }

        protected bool CanCapture(int Y, int X)
        {
            return ChessPlayingField.CheckRanges(Y, X) &&
                    !CurPlField.CheckEmpty(Y, X) && !IsSameColourAs(CurPlField[Y, X]);
        }

        public override FigureType GetFigureType() => FigureType.Pawn;

        public override bool IsKing() => false;
    }
    public sealed class ChessWhitePawn : ChessPawn
    {
        public ChessWhitePawn() : base(null)
        {

        }

        public ChessWhitePawn(ChessPlayingField curPlField) : base(curPlField)
        {

        }

        public override void CalculateAvailableMoves(int Y, int X)
        {
            var result = new List<ChessMove>();

            if (CanMove(X, Y + 1))
            {
                result.Add(new ChessMove(X, Y, X, Y + 1));
                if (Y == 1 && CanMove(X, Y + 2))
                {
                    result.Add(new ChessMove(X, Y, X, Y + 2));
                }
            }
            if (CanCapture(Y + 1, X + 1))
                result.Add(new ChessMove(X, Y, X + 1, Y + 1));

            if (CanCapture(Y + 1, X - 1))
                result.Add(new ChessMove(X, Y, X - 1, Y + 1));

            AvailableMoves = result;
        }

        public override bool IsWhite() => true;
    }

    public sealed class ChessBlackPawn : ChessPawn
    {
        public ChessBlackPawn() : base(null)
        {

        }

        public ChessBlackPawn(ChessPlayingField curPlField) : base(curPlField)
        {

        }

        public override void CalculateAvailableMoves(int Y, int X)
        {
            var result = new List<ChessMove>();

            if (CanMove(X, Y - 1))
            {
                result.Add(new ChessMove(X, Y, X, Y - 1));
                if (Y == 6 && CanMove(X, Y - 2))
                {
                    result.Add(new ChessMove(X, Y, X, Y - 2));
                }
            }
            if (CanCapture(Y - 1, X + 1))
                result.Add(new ChessMove(X, Y, X + 1, Y - 1));

            if (CanCapture(Y - 1, X - 1))
                result.Add(new ChessMove(X, Y, X - 1, Y - 1));

            AvailableMoves = result;
        }

        public override bool IsWhite() => false;
    }

    public abstract class ChessNotPawnFigure : ChessFigure
    {
        private bool isWhite;

        public ChessNotPawnFigure(ChessPlayingField curPlField, bool isWhite) : base(curPlField)
        {
            this.isWhite = isWhite;
        }

        public override bool IsWhite() => isWhite;

        public override object Clone(ChessPlayingField curPlField)
        {
            object cloneFigure = base.Clone(curPlField);
            (cloneFigure as ChessNotPawnFigure).isWhite = isWhite;
            return cloneFigure;
        }
    }

    public abstract class ChessNotLinearMovingFigure : ChessNotPawnFigure
    {
        public ChessNotLinearMovingFigure(ChessPlayingField curPlField, bool isWhite) : base(curPlField, isWhite)
        {

        }

        protected bool CanMoveAndCapture(int X, int Y)
        {
            if (!ChessPlayingField.CheckRanges(X, Y))
                return false;
            return CurPlField.CheckEmpty(Y, X) || !IsSameColourAs(CurPlField[Y, X]);
        }
    }

    public sealed class ChessRook : ChessNotPawnFigure
    {
        public ChessRook() : base(null, false)
        {

        }

        public ChessRook(ChessPlayingField curPlField, bool isWhite) : base(curPlField, isWhite)
        {

        }

        public override void CalculateAvailableMoves(int Y, int X)
        {
            var result = new List<ChessMove>();

            for (int i = X - 1; i >= 0; i--)
                if (CurPlField.CheckEmpty(Y, i))
                    result.Add(new ChessMove(X, Y, i, Y));
                else
                {
                    if (!IsSameColourAs(CurPlField[Y, i]))
                        result.Add(new ChessMove(X, Y, i, Y));
                    break;
                }

            for (int i = X + 1; i <= 7; i++)
                if (CurPlField.CheckEmpty(Y, i))
                    result.Add(new ChessMove(X, Y, i, Y));
                else
                {
                    if (!IsSameColourAs(CurPlField[Y, i]))
                        result.Add(new ChessMove(X, Y, i, Y));
                    break;
                }

            for (int i = Y - 1; i >= 0; i--)
                if (CurPlField.CheckEmpty(i, X))
                    result.Add(new ChessMove(X, Y, X, i));
                else
                {
                    if (!IsSameColourAs(CurPlField[i, X]))
                        result.Add(new ChessMove(X, Y, X, i));
                    break;
                }

            for (int i = Y + 1; i <= 7; i++)
                if (CurPlField.CheckEmpty(i, X))
                    result.Add(new ChessMove(X, Y, X, i));
                else
                {
                    if (!IsSameColourAs(CurPlField[i, X]))
                        result.Add(new ChessMove(X, Y, X, i));
                    break;
                }

            AvailableMoves = result;
        }

        public override FigureType GetFigureType() => FigureType.Rook;

        public override bool IsKing() => false;
    }

    public sealed class ChessKnight : ChessNotLinearMovingFigure
    {
        public ChessKnight() : base(null, false)
        {

        }

        public ChessKnight(ChessPlayingField curPlField, bool isWhite) : base(curPlField, isWhite)
        {

        }

        public override void CalculateAvailableMoves(int Y, int X)
        {
            var result = new List<ChessMove>();

            if (CanMoveAndCapture(X + 1, Y + 2))
                result.Add(new ChessMove(X, Y, X + 1, Y + 2));
            if (CanMoveAndCapture(X + 1, Y - 2))
                result.Add(new ChessMove(X, Y, X + 1, Y - 2));
            if (CanMoveAndCapture(X - 1, Y + 2))
                result.Add(new ChessMove(X, Y, X - 1, Y + 2));
            if (CanMoveAndCapture(X - 1, Y - 2))
                result.Add(new ChessMove(X, Y, X - 1, Y - 2));
            if (CanMoveAndCapture(X + 2, Y + 1))
                result.Add(new ChessMove(X, Y, X + 2, Y + 1));
            if (CanMoveAndCapture(X + 2, Y - 1))
                result.Add(new ChessMove(X, Y, X + 2, Y - 1));
            if (CanMoveAndCapture(X - 2, Y + 1))
                result.Add(new ChessMove(X, Y, X - 2, Y + 1));
            if (CanMoveAndCapture(X - 2, Y - 1))
                result.Add(new ChessMove(X, Y, X - 2, Y - 1));

            AvailableMoves = result;
        }

        public override FigureType GetFigureType() => FigureType.Knight;

        public override bool IsKing() => false;
    }

    public sealed class ChessBishop : ChessNotPawnFigure
    {
        public ChessBishop() : base(null, false)
        {

        }

        public ChessBishop(ChessPlayingField curPlField, bool isWhite) : base(curPlField, isWhite)
        {

        }

        public override void CalculateAvailableMoves(int Y, int X)
        {
            var result = new List<ChessMove>();

            for (int i = 1; (X - i >= 0) && (Y - i >= 0); i++)
                if (CurPlField.CheckEmpty(Y - i, X - i))
                    result.Add(new ChessMove(X, Y, X - i, Y - i));
                else
                {
                    if (!IsSameColourAs(CurPlField[Y - i, X - i]))
                        result.Add(new ChessMove(X, Y, X - i, Y - i));
                    break;
                }

            for (int i = 1; (X + i <= 7) && (Y + i <= 7); i++)
                if (CurPlField.CheckEmpty(Y + i, X + i))
                    result.Add(new ChessMove(X, Y, X + i, Y + i));
                else
                {
                    if (!IsSameColourAs(CurPlField[Y + i, X + i]))
                        result.Add(new ChessMove(X, Y, X + i, Y + i));
                    break;
                }

            for (int i = 1; (X + i <= 7) && (Y - i >= 0); i++)
                if (CurPlField.CheckEmpty(Y - i, X + i))
                    result.Add(new ChessMove(X, Y, X + i, Y - i));
                else
                {
                    if (!IsSameColourAs(CurPlField[Y - i, X + i]))
                        result.Add(new ChessMove(X, Y, X + i, Y - i));
                    break;
                }

            for (int i = 1; (X - i >= 0) && (Y + i <= 7); i++)
                if (CurPlField.CheckEmpty(Y + i, X - i))
                    result.Add(new ChessMove(X, Y, X - i, Y + i));
                else
                {
                    if (!IsSameColourAs(CurPlField[Y + i, X - i]))
                        result.Add(new ChessMove(X, Y, X - i, Y + i));
                    break;
                }

            AvailableMoves = result;
        }

        public override FigureType GetFigureType() => FigureType.Bishop;

        public override bool IsKing() => false;
    }

    public sealed class ChessQueen : ChessNotPawnFigure
    {
        public ChessQueen() : base(null, false)
        {

        }

        public ChessQueen(ChessPlayingField curPlField, bool isWhite) : base(curPlField, isWhite)
        {

        }

        public override void CalculateAvailableMoves(int Y, int X)
        {
            var result = new List<ChessMove>();

            for (int i = X - 1; i >= 0; i--)
                if (CurPlField.CheckEmpty(Y, i))
                    result.Add(new ChessMove(X, Y, i, Y));
                else
                {
                    if (!IsSameColourAs(CurPlField[Y, i]))
                        result.Add(new ChessMove(X, Y, i, Y));
                    break;
                }

            for (int i = X + 1; i <= 7; i++)
                if (CurPlField.CheckEmpty(Y, i))
                    result.Add(new ChessMove(X, Y, i, Y));
                else
                {
                    if (!IsSameColourAs(CurPlField[Y, i]))
                        result.Add(new ChessMove(X, Y, i, Y));
                    break;
                }

            for (int i = Y - 1; i >= 0; i--)
                if (CurPlField.CheckEmpty(i, X))
                    result.Add(new ChessMove(X, Y, X, i));
                else
                {
                    if (!IsSameColourAs(CurPlField[i, X]))
                        result.Add(new ChessMove(X, Y, X, i));
                    break;
                }

            for (int i = Y + 1; i <= 7; i++)
                if (CurPlField.CheckEmpty(i, X))
                    result.Add(new ChessMove(X, Y, X, i));
                else
                {
                    if (!IsSameColourAs(CurPlField[i, X]))
                        result.Add(new ChessMove(X, Y, X, i));
                    break;
                }

            for (int i = 1; (X - i >= 0) && (Y - i >= 0); i++)
                if (CurPlField.CheckEmpty(Y - i, X - i))
                    result.Add(new ChessMove(X, Y, X - i, Y - i));
                else
                {
                    if (!IsSameColourAs(CurPlField[Y - i, X - i]))
                        result.Add(new ChessMove(X, Y, X - i, Y - i));
                    break;
                }

            for (int i = 1; (X + i <= 7) && (Y + i <= 7); i++)
                if (CurPlField.CheckEmpty(Y + i, X + i))
                    result.Add(new ChessMove(X, Y, X + i, Y + i));
                else
                {
                    if (!IsSameColourAs(CurPlField[Y + i, X + i]))
                        result.Add(new ChessMove(X, Y, X + i, Y + i));
                    break;
                }

            for (int i = 1; (X + i <= 7) && (Y - i >= 0); i++)
                if (CurPlField.CheckEmpty(Y - i, X + i))
                    result.Add(new ChessMove(X, Y, X + i, Y - i));
                else
                {
                    if (!IsSameColourAs(CurPlField[Y - i, X + i]))
                        result.Add(new ChessMove(X, Y, X + i, Y - i));
                    break;
                }

            for (int i = 1; (X - i >= 0) && (Y + i <= 7); i++)
                if (CurPlField.CheckEmpty(Y + i, X - i))
                    result.Add(new ChessMove(X, Y, X - i, Y + i));
                else
                {
                    if (!IsSameColourAs(CurPlField[Y + i, X - i]))
                        result.Add(new ChessMove(X, Y, X - i, Y + i));
                    break;
                }

            AvailableMoves = result;
        }

        public override FigureType GetFigureType() => FigureType.Queen;

        public override bool IsKing() => false;
    }

    public sealed class ChessKing : ChessNotLinearMovingFigure
    {
        public bool IsChecked
        {
            get; private set;
        }

        public bool IsCheckmated
        {
            get; set;
        }

        public ChessKing() : base(null, false)
        {

        }

        public ChessKing(ChessPlayingField curPlField, bool isWhite) : base(curPlField, isWhite)
        {

        }

        public override void CalculateAvailableMoves(int Y, int X)
        {
            var result = new List<ChessMove>();

            for (int i = -1; i <= 1; i++)
                for (int j = -1; j <= 1; j++)
                    if (i != 0 || j != 0)
                        if (CanMoveAndCapture(X + i, Y + j))
                            result.Add(new ChessMove(X, Y, X + i, Y + j));
            AvailableMoves = result;
        }

        public override FigureType GetFigureType() => FigureType.King;

        public override bool IsKing() => true;

        public void TryCheckBy(ChessFigure figure, int Y, int X)
        {
            foreach (var move in figure.AvailableMoves)
                if (move.EndX == X && move.EndY == Y)
                    IsChecked = true;
        }

        public void ResetCheck()
        {
            IsChecked = false;
        }

        public override object Clone(ChessPlayingField curPlField)
        {
            object cloneFigure = base.Clone(curPlField);
            (cloneFigure as ChessKing).IsChecked = IsChecked;
            (cloneFigure as ChessKing).IsCheckmated = IsCheckmated;
            return cloneFigure;
        }
    }

    public sealed class ChessPlayingField : ICloneable
    {
        private readonly ChessFigure[,] field;
        public ChessPlayingField()
        {
            field = new ChessFigure[8, 8] {
           {new ChessRook(this,true), new ChessKnight(this,true), new ChessBishop(this,true), new ChessKing(this,true), new ChessQueen(this,true), new ChessBishop(this,true), new ChessKnight(this,true), new ChessRook(this,true)},
           {new ChessWhitePawn(this), new ChessWhitePawn(this), new ChessWhitePawn(this), new ChessWhitePawn(this), new ChessWhitePawn(this), new ChessWhitePawn(this), new ChessWhitePawn(this), new ChessWhitePawn(this)},
           {null, null, null, null, null, null, null, null },
           {null, null, null, null, null, null, null, null },
            {null, null, null, null, null, null, null, null },
            {null, null, null, null, null, null, null, null },
           {new ChessBlackPawn(this), new ChessBlackPawn(this), new ChessBlackPawn(this), new ChessBlackPawn(this), new ChessBlackPawn(this), new ChessBlackPawn(this), new ChessBlackPawn(this), new ChessBlackPawn(this)},
           {new ChessRook(this,false), new ChessKnight(this,false), new ChessBishop(this,false), new ChessKing(this,false), new ChessQueen(this,false), new ChessBishop(this,false), new ChessKnight(this,false), new ChessRook(this,false)}
                       };
        }

        public ChessFigure this[int i, int j]
        {
            get
            {
                return field[i, j];
            }
            set
            {
                field[i, j] = value;
            }
        }

        public bool CheckEmpty(int Y, int X)
        {
            return (field[Y, X] == null);
        }

        public static bool CheckRanges(int i, int j)
        {
            if (i < 0 || j < 0 || i > 7 || j > 7)
                return false;
            return true;
        }

        public object Clone()
        {
            var plField = new ChessPlayingField();
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    if (CheckEmpty(i, j))
                        plField[i, j] = null;
                    else
                        plField[i, j] = field[i, j].Clone(plField) as ChessFigure;
            return plField;
        }
    }

    public sealed class GameLogic
    {
        private int whiteKingX, whiteKingY, blackKingX, blackKingY;
        private ChessKing WhiteKing => plField[whiteKingY, whiteKingX] as ChessKing;
        private ChessKing BlackKing => plField[blackKingY, blackKingX] as ChessKing;

        private ChessKing CurrentKing => IsWhitesTurn ? WhiteKing : BlackKing;
        private ChessKing OppositeKing => IsWhitesTurn ? BlackKing : WhiteKing;

        private ChessPlayingField plField;
        public bool IsWhitesTurn
        {
            get; private set;
        }

        private static GameLogic instance;

        private GameLogic()
        {
            plField = new ChessPlayingField();

            whiteKingX = 3;
            whiteKingY = 0;
            blackKingX = 3;
            blackKingY = 7;

            IsWhitesTurn = true;

            CalculateAvailableMoves();
        }

        public static GameLogic GetInstance()
        {
            return instance;
        }

        public static GameLogic CreateNewGame()
        {
            instance = new GameLogic();
            return instance;
        }

        public Figure[,] GetField()
        {
            var result = new Figure[8, 8];
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    if (plField.CheckEmpty(i, j))
                        result[i, j] = null;
                    else
                        result[i, j] = new Figure(plField[i, j].GetFigureType(), plField[i, j].IsWhite());

            return result;
        }

        public bool IsCheckmate()
        {
            return CurrentKing.IsCheckmated;
        }

        public void CalculateAvailableMoves()
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    if (plField[i, j] != null)
                        plField[i, j].CalculateAvailableMoves(i, j);
        }

        public void TryCheckKings()
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    if (!plField.CheckEmpty(i, j))
                    {
                        WhiteKing.TryCheckBy(plField[i, j], whiteKingY, whiteKingX);
                        BlackKing.TryCheckBy(plField[i, j], blackKingY, blackKingX);
                    }

        }

        public void TryToCheckmateOppositeKing()
        {
            if (!OppositeKing.IsChecked)
                throw new ArgumentException("Must be checked", "ChessKing king");

            bool kingCanBeUnchecked = false;

            for (int i = 0; i < 8 && !kingCanBeUnchecked; i++)
                for (int j = 0; j < 8 && !kingCanBeUnchecked; j++)
                    if (!plField.CheckEmpty(i, j) && OppositeKing.IsSameColourAs(plField[i, j]))
                        foreach (var move in plField[i, j].AvailableMoves)
                        {
                            ChessFigure capturedFigure = plField[move.EndY, move.EndX];

                            MakeOrCancelAMove(move);

                            if (!OppositeKing.IsChecked)
                            {
                                Console.WriteLine("Uncheck: {0}, {1}, {2}, {3}", move.StartY, move.StartX, move.EndY, move.EndX);
                                kingCanBeUnchecked = true;
                                MakeOrCancelAMove(move.Reverse(), capturedFigure);
                                break;
                            }

                            Console.WriteLine("----------------------------------------");
                            MakeOrCancelAMove(move.Reverse(), capturedFigure);

                        }

            OppositeKing.IsCheckmated = !kingCanBeUnchecked;

        }

        public void ResetKingsChecks()
        {
            WhiteKing.ResetCheck();
            BlackKing.ResetCheck();
        }

        private void MakeOrCancelAMove(ChessMove move, ChessFigure capturedFigure = null)
        {
            if (plField[move.StartY, move.StartX].IsKing())
                if (plField[move.StartY, move.StartX].IsWhite())
                {
                    whiteKingX = move.EndX;
                    whiteKingY = move.EndY;
                }
                else
                {
                    blackKingX = move.EndX;
                    blackKingY = move.EndY;
                }

            plField[move.EndY, move.EndX] = plField[move.StartY, move.StartX];
            plField[move.StartY, move.StartX] = capturedFigure;

            ResetKingsChecks();
            CalculateAvailableMoves();
            TryCheckKings();
            Console.WriteLine("White king is in check: {0}", WhiteKing.IsChecked);
            Console.WriteLine("Black king is in check: {0}", BlackKing.IsChecked);

        }

        public void CheckAvailableMoves()
        {
            ChessPlayingField tempPlField = plField.Clone() as ChessPlayingField;
            int tempWhiteKingX = whiteKingX, tempWhiteKingY = whiteKingY;
            int tempBlackKingX = blackKingX, tempBlackKingY = blackKingY;

            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    if (!plField.CheckEmpty(i, j))
                    {
                        foreach (var move in plField[i, j].AvailableMoves)
                            if (plField.CheckEmpty(move.EndY, move.EndX) || !plField[move.EndY, move.EndX].IsKing())
                            {
                                ChessFigure capturedFigure = plField[move.EndY, move.EndX];

                                MakeOrCancelAMove(move);

                                Console.WriteLine("----------------------------------------");

                                if (OppositeKing.IsChecked)
                                    tempPlField[i, j].AvailableMoves.Remove(move);

                                MakeOrCancelAMove(move.Reverse(), capturedFigure);

                            }
                    }
            plField = tempPlField;
            whiteKingX = tempWhiteKingX;
            whiteKingY = tempWhiteKingY;
            blackKingX = tempBlackKingX;
            blackKingY = tempBlackKingY;

        }

        public bool CheckAndMakeAMove(ChessMove move)
        {
            if (plField.CheckEmpty(move.StartY, move.StartX) || !plField[move.StartY, move.StartX].CheckMove(move))
                return false;

            ChessFigure capturedFigure = plField[move.EndY, move.EndX];

            MakeOrCancelAMove(move);

            if (IsWhitesTurn ? WhiteKing.IsChecked : BlackKing.IsChecked)
            {

                Console.WriteLine("----------------------------------------");
                MakeOrCancelAMove(move.Reverse(), capturedFigure);
                return false;
            }
            else
            {
                if (OppositeKing.IsChecked)
                    TryToCheckmateOppositeKing();
                CheckAvailableMoves();
                IsWhitesTurn = !IsWhitesTurn;

                return true;
            }
        }

        public List<ChessMove> GetAvailableMoves(int x, int y)
        {
            return plField[y, x].AvailableMoves;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
