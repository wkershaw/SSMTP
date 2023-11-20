using System.Net.Sockets;
using System.Text;

internal class SmtpStream
{
    private NetworkStream _networkStream;

    public SmtpStream(NetworkStream networkStream)
    {
        _networkStream = networkStream;
    }

    public void Write(string message)
    {
        string toSend = message;
        if(!message.EndsWith("\r\n"))
        {
            toSend = message + "\r\n";
        }

        Logger.LogDebug($"Message sent: '{toSend}'");
        _networkStream.Write(Encoding.ASCII.GetBytes(toSend));
    }

    public string ReadNext()
    {
        Byte[] bytes = new Byte[256];
        int bytesReadCount = _networkStream.Read(bytes, 0, bytes.Length);
        
        // Read message
        string message = Encoding.ASCII.GetString(bytes, 0, bytesReadCount);

        // Strip line endings
        message = message.Replace("\r\n", "");

        Logger.LogDebug($"Message recieved: '{message}'");
        return message;
    }
}