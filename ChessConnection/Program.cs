using System.Reflection;
using System.Threading;
using ChessLogic;
using JsonSerialization;

namespace ChessConnection
{
    public sealed class ChessServer
    {
        public IChessGame Game { get; set; }
        private readonly IConnection connection;
        private Thread readingThread;

        public bool IsServerRunning { get; private set; }

        public ChessServer(IConnection connection)
        {
            this.connection = connection;
        }

        public void Start()
        {
            IsServerRunning = true;
            readingThread = new Thread(ThreadProc);
            readingThread.Start();
        }

        public void Stop()
        {
            IsServerRunning = false;
        }

        public void CloseConnection()
        {
            IsServerRunning = false;
            readingThread.Join();
        }
        public void ThreadProc()
        {
            int attemptsCounter = 0;
            while (IsServerRunning)
            {
                string request;
                if ((request = connection.Read()) != "")
                {
                    attemptsCounter = 0;
                    connection.Write(ProcessRequest(request));
                }
                else if (attemptsCounter >= 30)
                    Game.Disconnect();
                else
                {
                    attemptsCounter++;
                    Thread.Sleep(50);
                }
            }
            connection.Close();
        }

        public string ProcessRequest(string request)
        {
            var jsonRequest = JsonRequest.Parse(request);
            object response = InvokeMethod(jsonRequest.MethodName, jsonRequest.DeserializedArgs);
            return JsonResponse.SerializeResponse(response);
        }

        public object InvokeMethod(string methodName, object[] args)
        {
            if (methodName == "GameAndServerState")
            {
                var gameAndServerState = Game.GetType().InvokeMember(methodName,
                    BindingFlags.InvokeMethod | BindingFlags.GetProperty, null, Game, args)
                        as GameAndServerState;
                return gameAndServerState;
            }

            return Game.GetType().InvokeMember(methodName, BindingFlags.InvokeMethod | BindingFlags.GetProperty, null, Game, args);
        }
    }

    public sealed class ChessClient
    {
        private readonly IConnection connection;
        private readonly object connectionLock;

        public ChessClient(IConnection connection)
        {
            this.connection = connection;
            connectionLock = new object();
        }

        public void CloseConnection()
        {
            connection.Close();
        }

        public void InvokeMethod(string methodName, object[] args)
        {
            lock (connectionLock)
            {
                SendRequest(JsonRequest.FormInvokationRequest(methodName, args));
                WaitingForResponse();
            }
        }

        public ReturnType InvokeMethod<ReturnType>(string methodName, object[] args)
        {
            string response;
            lock (connectionLock)
            {
                SendRequest(JsonRequest.FormInvokationRequest(methodName, args));
                response = WaitingForResponse();
            }
            return JsonResponse.DeserializeResponse<ReturnType>(response);
        }

        public void SendRequest(string request)
        {
            connection.Write(request);
        }

        public string WaitingForResponse()
        {
            string response;

            for (int i = 0; i < 30; i++)
            {
                if ((response = connection.Read()) != "")
                {
                    return response;
                }
                else
                    Thread.Sleep(50);
            }

            throw new ConnectionException("Fail to get response");
        }

    }

    class Program
    {
        static void Main(string[] args)
        {

        }
    }
}
