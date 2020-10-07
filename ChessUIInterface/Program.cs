using System;
using System.Collections.Generic;
using ChessLogic;

namespace ChessUI
{
    public sealed class SocketInfo
    {
        public SocketInfo(string host, string port)
        {
            Host = host;
            Port = port;
        }
        public string Host { get; }
        public string Port { get; }
    }

    public interface IChessUI
    {
        SocketInfo SocketInfo { get; }
        void ShowConnectionMenu(bool isServerSide);
        void ShowMenu();
        void ShowCheckmateDlg(bool isWhitesTurn);
        void StartStopWaitingConnBtnRename(bool isStartingConn);
        void ShowGame();
        void ResetPlayingFieldBacklight();
        void RefreshPlayingField(Figure[,] field, bool isWhitesTurn, bool isWhitePlayer);
        void MakeSquareActive(int x, int y, List<ChessMove> availableMoves);
        object Invoke(Delegate method, params object[] args);
        void ShowMessageBox(string text, string caption);
    }
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
