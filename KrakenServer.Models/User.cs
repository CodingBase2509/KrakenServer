using System.Net.Sockets;
using System.Net;
using System.Text;

namespace KrakenServer.Models
{
    public class User
    {
        TcpClient _client { get; }

        IPAddress _ipAdress { get; }

        Socket _socket { get; }

        NetworkStream _stream { get; }

        string Name { get; set; }

        public User(TcpClient client, string name)
        {
            _client = client;
            _socket = client.Client;
            _ipAdress = ((IPEndPoint)_socket.RemoteEndPoint!).Address;
            _stream = client.GetStream();
            Name = name;
        }

        public async Task<bool> Send(string msg)
        {
            try
            {
                byte[] temBuffer = Encoding.UTF8.GetBytes(msg);
                await _stream.WriteAsync(temBuffer);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occured on sending: " + e.Message);
                return false;
            }
        }

        public async Task<string> Receive()
        {
            byte[] tempBuffer = Array.Empty<byte>();

            try
            {
                Console.WriteLine($"receiveing from {((IPEndPoint)_client!.Client.RemoteEndPoint!).Address}:");

                if (!_stream!.CanRead)
                    return string.Empty;

                int received = await _stream.ReadAsync(tempBuffer);
                await _stream.FlushAsync();

                if (received > 0)
                {
                    var data = Encoding.UTF8.GetString(tempBuffer, 0, tempBuffer.Length);
                    Console.WriteLine(data);
                    return data;
                }
                else
                    return string.Empty;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occured on receiving: " + e.Message);
                return string.Empty;
            }
        }

        /// <summary>
        /// Disposes The <see cref="Server"/> Object async
        /// </summary>
        /// <returns></returns>
        public async ValueTask DisposeAsync()
        {
            _stream.Close();
            await _stream.DisposeAsync();
            _client.Close();
            _client.Dispose();
        }

    }
}



