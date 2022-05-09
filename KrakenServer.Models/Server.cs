using System;
namespace KrakenServer.Models
{

	public class Server
	{
		public Connection Connection { get; set; }

		public List<User> ActiveUser { get; set; }

		public Dictionary<User, string> UserDict { get; set; }

		public string HostIp { get; private set; } = "127.0.0.1";

		public int Port { get; private set; } = 9001;


		public Server(string hostIp, int port)
        {

        }

    }
}

