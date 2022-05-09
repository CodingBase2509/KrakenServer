namespace KrakenServer;

public class Program
{
    public async static Task Main(string[] args)
    {
        var Server = new Server("127.0.0.1", 9001);

        await Server.Listen();
    }
}