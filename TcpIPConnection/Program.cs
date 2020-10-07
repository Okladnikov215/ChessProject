using System.Text;
using System.Net.Sockets;

namespace ChessConnection
{
    public sealed class TcpIPServerConnection : IConnection
    {
        private readonly TcpListener server;
        private TcpClient client;

        public TcpIPServerConnection(string hostname, int port)
        {
            try
            {
                System.Net.IPAddress ipAddress = System.Net.IPAddress.Parse(hostname);
                server = new TcpListener(ipAddress, port);
                server.Start();
            }
            catch(SocketException e)
            {
                throw new ConnectionException(e.Message); 
            }
        }

        public bool TryToOpen()
        {
            if (server.Pending())
            {
                client = server.AcceptTcpClient();
                return true;
            }

            return false;
        }

        public void Write(string str)
        {
            byte[] buf = Encoding.UTF8.GetBytes(str);
            client.GetStream().Write(buf, 0, buf.Length);
        }

        public string Read()
        {
            byte[] buf = new byte[10];
            string data = "";

            NetworkStream stream = client.GetStream();

            int i;

            while (stream.DataAvailable)
            {
                i = stream.Read(buf, 0, buf.Length);
                data += Encoding.UTF8.GetString(buf, 0, i);
            }

            return data;
        }

        public void Close()
        {
            if (client != null)
                client.Close();
            server.Stop();
        }
    }

    public sealed class TcpIPClientConnection : IConnection
    {
        private TcpClient client;
        private readonly string hostname;
        private readonly int port;

        public TcpIPClientConnection(string hostname, int port)
        {
            this.hostname = hostname;
            this.port = port;
        }

        public bool TryToOpen()
        {
            try
            {
                client = new TcpClient(hostname, port);
                return true;
            }
            catch(SocketException)
            {
                return false;
            }
        }

        public void Write(string str)
        {
            byte[] buf = Encoding.UTF8.GetBytes(str);
            client.GetStream().Write(buf, 0, buf.Length);
        }

        public string Read()
        {
            byte[] buf = new byte[10];
            string data = "";

            NetworkStream stream = client.GetStream();

            int i;

            while (stream.DataAvailable)
            {
                i = stream.Read(buf, 0, buf.Length);
                data += Encoding.UTF8.GetString(buf, 0, i);
            }

            return data;
        }

        public void Close()
        {
            if (client != null)
            client.Close();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
