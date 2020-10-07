using System;

namespace ChessConnection
{
    public class ConnectionException : Exception
    {
        public ConnectionException(string message) : base(message)
        {

        }
    }

    public interface IConnection
    {
        bool TryToOpen();
        void Write(string str);
        string Read();
        void Close();
    }

    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
