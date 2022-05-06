using System;
using System.Text.Encodings;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace KrakenServer
{
	public class Server : IAsyncDisposable
	{
		/// <summary>
        /// The ip-adress of the host
        /// </summary>
		public string HostIp { get; private set; }

		/// <summary>
        /// the port on which the server is running on
        /// </summary>
		public int Port { get; private set; }

		/// <summary>
        /// the <see cref="TcpClient"/> which handles the connection
        /// </summary>
		TcpClient? _client { get; set; }

		/// <summary>
        /// the <see cref="NetworkStream"/> from the <see cref="_client"/>
        /// </summary>
		NetworkStream? _stream { get; set; }

		/// <summary>
        /// The <see cref="TcpListener"/> to listen for incoming connections
        /// </summary>
		TcpListener _listener { get; }

		/// <summary>
        /// the data that arraives via the network stream
        /// </summary>
		public string Data { get; set; }

		/// <summary>
        /// Indicates the Status of the Server(-operations) with a <see cref="bool"/>
        /// <see langword="false"/> indicates that an error occured
        /// </summary>
		public bool Status { get; private set; }

		/// <summary>
		/// Creates an instanz of the <see cref="Server"/> class
		/// </summary>
		/// <param name="host">The ip on which the <see cref="TcpListener"/> runs on</param>
		/// <param name="port">The port on which the <see cref="TcpListener"/> runs on</param>
		public Server(string host, int port)
		{
			HostIp = host;
			Port = port;
			Data = string.Empty;

			_listener = new TcpListener(IPAddress.Parse(HostIp), Port);
		}

		/// <summary>
        /// Setup the Listener to establish connections with incoming requests
        /// </summary>
        /// <returns></returns>
		public async Task Listen()
        {
			_listener.Start();
			Console.WriteLine($"Listener is setup on port {Port}");
			_client = await _listener.AcceptTcpClientAsync();

            if (_client.Connected)
            {
				_stream = _client.GetStream();
                Console.WriteLine($"TcpClient {((IPEndPoint)_client.Client.RemoteEndPoint!).Address} connects to the Server");
				Status = await Receive();
            }
        }

		/// <summary>
        /// Receives messages from the established connetion
        /// </summary>
        /// <returns>A <see cref="bool"/> which indicates wether the operation was successful or not</returns>
		public async Task<bool> Receive()
        {
			byte[] tempBuffer = Array.Empty<byte>();

			try
			{
				Console.WriteLine($"receiveing from {((IPEndPoint)_client!.Client.RemoteEndPoint!).Address}:");

				if (!_stream!.CanRead)
					return false;

				int received = await _stream.ReadAsync(tempBuffer);
				await _stream.FlushAsync();

				if (received > 0)
				{
					Data = Encoding.UTF8.GetString(tempBuffer, 0, tempBuffer.Length);
					Console.WriteLine(Data);
					return true;
				}
				else
					return false;
			}
			catch(Exception e)
            {
				Data = "";
                Console.WriteLine("An error occured on receiving: " + e.Message);
				return false;
            }
        }

		/// <summary>
		/// Send messages back to the established connection through the <see cref="NetworkStream"/>
		/// </summary>
		/// <param name="msg">The message which will be send</param>
		/// <returns>A <see cref="bool"/> which indicates wether the operation was successful or not</returns>
		public async Task<bool> Send(string msg)
        {
            try
            {
				byte[] temBuffer = Encoding.UTF8.GetBytes(msg);
				await _stream!.WriteAsync(temBuffer);

				return true;
            }
			catch(Exception e)
            {
                Console.WriteLine("An error occured on sending: " + e.Message);
				return false;
            }
        }

		/// <summary>
        /// Disposes The <see cref="Server"/> Object async
        /// </summary>
        /// <returns></returns>
        public async ValueTask DisposeAsync()
        {
			_stream!.Close();
			await _stream!.DisposeAsync();
			_client!.Close();
			_client!.Dispose();
			_listener.Stop();
			Status = false;
			Data = HostIp = "";
			Port = 0;
        }
    }
}

