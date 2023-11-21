using System.Net.Sockets;
using System.Text;
using Ssmtp.Utilities;

namespace Ssmtp.Smtp;

internal class SmtpStream
{
    private NetworkStream _networkStream;

    public SmtpStream(NetworkStream networkStream)
    {
        _networkStream = networkStream;
    }

    public void Write(string message)
    {
        Logger.LogDebug($"Sending message: '{message}'");

        string toSend = message;
        if (!message.EndsWith("\r\n"))
        {
            toSend = message + "\r\n";
        }

        _networkStream.Write(Encoding.ASCII.GetBytes(toSend));
    }

    public string ReadNext()
    {
        byte[] bytes = new byte[64 * 1024];
        int bytesReadCount = _networkStream.Read(bytes, 0, bytes.Length);

        // Read message
        string message = Encoding.ASCII.GetString(bytes, 0, bytesReadCount);

        // Strip line endings
        message = message.Replace("\r\n", "");

        Logger.LogDebug($"Message received: '{message}'");
        return message;
    }
}