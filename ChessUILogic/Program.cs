using System;
using System.Threading;
using System.IO;
using ChessLogic;
using ChessConnection;

namespace ChessUI
{
    public sealed class ChessUILogic
    {
        private IChessGame game;
        private readonly IChessUI form;

        private bool isWaitingConnection;
        private bool isServerSide;
        private Thread waitingConnectionThread;

        private int prevX, prevY;

        private bool isWhitePlayer;
        private bool isMyTurn;
        private bool isGameRunning;
        private Thread waitingForPlayersTurnThread;

        public ChessUILogic(IChessUI form)
        {
            this.form = form;
            prevX = -1;
            prevY = -1;
        }

        private void WaitingConnection()
        {
            IConnection connection = null;
            try
            {
                if (isServerSide)
                {
                    connection = new TcpIPServerConnection(form.SocketInfo.Host, int.Parse(form.SocketInfo.Port));
                    for (int i = 0; i < 40; i++)
                        if (!isWaitingConnection)
                            break;
                        else if (connection.TryToOpen())
                        {
                            game = ChessGameFactory.GetInstance().GetChessGame(connection);
                            break;
                        }
                        else
                            Thread.Sleep(100);
                }
                else
                {
                    connection = new TcpIPClientConnection(form.SocketInfo.Host, int.Parse(form.SocketInfo.Port));
                    for (int i = 0; i < 2; i++)
                        if (connection.TryToOpen())
                        {
                            game = RemoteChessGameFactory.GetInstance().GetChessGame(connection);
                            break;
                        }
                        else
                            Thread.Sleep(100);
                }
            }
            catch (ThreadAbortException)
            {
                isWaitingConnection = false;
            }
            catch (Exception e) when (e is ConnectionException || e is IOException)
            {
                isWaitingConnection = false;
                form.ShowMessageBox("Fail to connect. Port is busy", "Error");
                form.Invoke((Action<bool>)form.StartStopWaitingConnBtnRename, new object[] { false });
            }
            catch (Exception e) when (e is FormatException || e is OverflowException)
            {
                isWaitingConnection = false;
                form.ShowMessageBox("Wrong host or port format", "Error");
                form.Invoke((Action<bool>)form.StartStopWaitingConnBtnRename, new object[] { false });
            }

            finally
            {
                if (isWaitingConnection)
                {
                    if (game == null)
                    {
                        form.Invoke((Action<bool>)form.StartStopWaitingConnBtnRename, new object[] { false });
                        isWaitingConnection = false;
                        form.ShowMessageBox("Fail to connect", "Error");
                    }
                    else
                        form.Invoke((ThreadStart)StartGame);
                }
                if (!isWaitingConnection)
                {
                    if (game != null)
                    {
                        try
                        {
                            game.Disconnect();
                        }
                        catch (Exception e) when (e is ConnectionException || e is IOException)
                        {

                        }
                        game = null;
                    }
                    if (connection != null)
                    {
                        connection.Close();
                        connection = null;
                    }
                }
                isWaitingConnection = false;
            }
        }

        public void StartGame()
        {
            try
            {
                isMyTurn = false;
                game.Refresh();
                form.ResetPlayingFieldBacklight();
                form.RefreshPlayingField(game.GetField(), game.IsWhitesTurn, isWhitePlayer);

                form.ShowGame();

                isGameRunning = true;
                waitingForPlayersTurnThread = new Thread(WaitingForPlayersTurn);
                waitingForPlayersTurnThread.Start();
            }
            catch (Exception e) when (e is IOException || e is ConnectionException)
            {
                form.ShowMessageBox("Fail to connect", "Error");
                StopGame(false);
            }
        }

        public void StopGame(bool isInvokedFromUI)
        {
            isGameRunning = false;
            isWaitingConnection = false;
            form.Invoke((Action)(() => form.StartStopWaitingConnBtnRename(false)));

            if (waitingForPlayersTurnThread != null && isInvokedFromUI)
            {
                waitingForPlayersTurnThread.Join();
                waitingForPlayersTurnThread = null;
            }
            if (game != null)
            {
                try
                {
                    game.Disconnect();
                }
                catch (Exception e) when (e is IOException || e is ConnectionException)
                {

                }

                game = null;
            }
        }

        public void CheckmateStop(bool isInvokedFromUI)
        {
            form.ShowCheckmateDlg(game.IsWhitesTurn);
            StopGame(isInvokedFromUI);
        }

        public void WaitingForPlayersTurn()
        {
            while (isGameRunning)
            {
                try
                {

                    game.Refresh();

                    if (!isMyTurn)
                    {
                        if (game.IsWhitesTurn == isWhitePlayer)
                        {
                            form.RefreshPlayingField(game.GetField(), game.IsWhitesTurn, isWhitePlayer);

                            if (game.IsCheckmate())
                            {
                                form.Invoke((ThreadStart)(() => CheckmateStop(false)));
                                return;
                            }

                            isMyTurn = true;
                        }
                    }

                    if (!game.IsCheckmate() && game.IsServerStops())
                    {
                        StopGame(false);
                        form.Invoke((ThreadStart)(() => form.ShowMenu()));
                        form.ShowMessageBox("Second player interrupted the game", "Chess Game");
                    }

                    Thread.Sleep(50);
                }
                catch (Exception e) when (e is ConnectionException || e is IOException)
                {
                    form.Invoke((Action)(() => StopGame(false)));
                    form.Invoke((ThreadStart)(() => form.ShowMenu()));
                    form.ShowMessageBox("Second player interrupted the game", "Chess Game");
                    return;
                }
            }
        }

        public void StartStopWaitingConnection()
        {
            if (isWaitingConnection)
            {
                isWaitingConnection = false;
                if (waitingConnectionThread != null)
                    if (isServerSide)
                        waitingConnectionThread.Join();
                    else
                        waitingConnectionThread.Abort();
                form.StartStopWaitingConnBtnRename(false);
            }
            else
            {
                isWaitingConnection = true;
                waitingConnectionThread = new Thread(WaitingConnection);
                waitingConnectionThread.Start();
                form.StartStopWaitingConnBtnRename(true);
            }

        }

        public void ReturnToMenu()
        {
            StopGame(true);

            if (isWaitingConnection)
            {
                waitingConnectionThread.Abort();
                isWaitingConnection = false;
            }

            form.ShowMenu();
        }

        public void MakeSquareActive(int x, int y)
        {
            prevX = x;
            prevY = y;
            form.MakeSquareActive(x, y, game.GetAvailableMoves(x, y));
        }

        public void StartOrMakeAMove(int y, int x)
        {
            try
            {
                form.ResetPlayingFieldBacklight();
                if (isGameRunning)
                    if (isMyTurn && !game.IsCheckmate())
                    {
                        if (prevX == -1 || prevY == -1)
                        {
                            if (game.GetField()[y, x] != null && game.GetField()[y, x].IsWhite == game.IsWhitesTurn)
                                MakeSquareActive(x, y);
                        }
                        else if (game.GetField()[y, x] == null || game.GetField()[y, x].IsWhite != game.IsWhitesTurn)
                        {
                            if (game.CheckAndMakeAMove(new ChessMove(prevX, prevY, x, y)))
                            {
                                game.Refresh();
                                isMyTurn = false;
                            }

                            prevX = -1;
                            prevY = -1;

                            form.RefreshPlayingField(game.GetField(), game.IsWhitesTurn, isWhitePlayer);

                            if (game.IsCheckmate())
                                CheckmateStop(true);
                        }
                        else
                            MakeSquareActive(x, y);
                    }
            }
            catch (Exception e) when (e is IOException || e is ConnectionException)
            {
                StopGame(true);
            }
        }

        public void SetupConnection(bool isServerSide)
        {
            this.isServerSide = isServerSide;
            isWhitePlayer = !isServerSide;
            form.ShowConnectionMenu(isServerSide);
            isWaitingConnection = false;
            form.StartStopWaitingConnBtnRename(false);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
