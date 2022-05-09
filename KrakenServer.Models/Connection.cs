using System;
using System.Net.Sockets;
using System.Net;

namespace KrakenServer.Models
{
	public class Connection: IAsyncDisposable
	{
		TcpListener _listener { get; }

		List<User> _activeUser { get; }

		public Connection(string loclAddr, int port, ref List<User> activeUser)
		{
			_listener = new TcpListener(IPAddress.Parse(loclAddr), port);
			_activeUser = activeUser;
		}

		public async Task Listen()
		{
			_listener.Start();
			Console.WriteLine($"Listener is setup on");
			var _client = await _listener.AcceptTcpClientAsync();
			
			_activeUser.Add(new User(_client, ""));

			Listen();
			return;
		}

        public ValueTask DisposeAsync()
        {
			_listener.Stop();
			return new ValueTask();
        }
    }
}

