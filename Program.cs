namespace Server
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    class Program
    {
        static List<TcpClient> clients = new List<TcpClient>();

        static async Task Main()
        {
            TcpListener server = new TcpListener(IPAddress.Any, 5000);
            server.Start();

            while (true)
            {
                var client = await server.AcceptTcpClientAsync();
                clients.Add(client);

                Console.WriteLine("+");

                _ = HandleClient(client);
            }
        }

        static async Task HandleClient(TcpClient client)
        {
            var stream = client.GetStream();
            byte[] buffer = new byte[1024];

            try
            {
                while (true)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                    if (bytesRead == 0)
                        break;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    Console.WriteLine(message);

                    await Broadcast(message);
                }
            }
            catch { }

            clients.Remove(client);
            client.Close();
        }

        static async Task Broadcast(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);

            foreach (var client in clients)
            {
                try
                {
                    await client.GetStream().WriteAsync(data, 0, data.Length);
                }
                catch { }
            }
        }
    }
}
