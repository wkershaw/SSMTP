using System.Net;
using System.Net.Sockets;

int port = 2025;
IPAddress localAddr = IPAddress.Parse("127.0.0.1");
var server = new TcpListener(localAddr, port);

static void InnerLoop(TcpListener server)
{
    Console.WriteLine("Waiting for a connection... ");
    using TcpClient client = server.AcceptTcpClient();
    Console.WriteLine("Connected!");

    var stream = new SmtpStream(client.GetStream());

    // Write opening message
    string opener = "220 localhost:2025";
    stream.Write(opener);
    
    // Read response
    var response = stream.ReadNext();
}




















try
{
    // Start listening for client requests.
    server.Start();

    // Enter the listening loop.
    while(true)
    {
       InnerLoop(server);
    }
}
catch(SocketException e)
{
    Console.WriteLine("SocketException: {0}", e);
}
finally
{
    server?.Stop();
}

Console.WriteLine("\nHit enter to continue...");
Console.Read();