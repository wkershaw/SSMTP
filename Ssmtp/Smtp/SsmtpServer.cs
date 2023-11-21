using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Ssmtp.Utilities;

namespace Ssmtp.Smtp;

public class SsmtpServer
{
    private readonly TcpListener _tcpListener;

    public SsmtpServer(string address, int port)
    {
        IPAddress localAddress = IPAddress.Parse(address);
        _tcpListener = new TcpListener(localAddress, port);
    }

    public void Start(CancellationToken cancellationToken)
    {
        try
        {
            // Start listening for client requests.
            _tcpListener.Start();

            // Enter the listening loop.
            while (!cancellationToken.IsCancellationRequested)
            {
                InnerLoop();
            }
        }
        catch (SocketException e)
        {
            Logger.LogError("An error occurred during inner loop", e);
        }
        finally
        {
            _tcpListener.Stop();
        }
    }

    private void InnerLoop()
    {
        Logger.LogDebug("Waiting for a connection... ");
        using TcpClient client = _tcpListener.AcceptTcpClient();
        Logger.LogDebug("Connected!");

        var stream = new SmtpStream(client.GetStream());

        // Write opening message
        string opener = "220 localhost:2025";
        stream.Write(opener);

        var conversation = new Conversation();

        // Handle all client commands
        while (client.Connected)
        {
            try
            {
                var command = stream.ReadNext();
                var response = conversation.HandleCommand(command);
                stream.Write(response);
            }
            catch (Exception e)
            {
                Logger.LogError($"Unable to handle command", e);
            }
        }

        Logger.LogDebug("Email Sent!");
        Logger.LogDebug(conversation.ToString());
    }
}
