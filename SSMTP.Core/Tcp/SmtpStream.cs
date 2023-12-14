using System.Net.Sockets;
using System.Text;

namespace SSMTP.Core.Tcp;

/// <summary>
/// Represents a network stream with a client used to send and receive SMTP messages
/// </summary>
internal class SmtpStream : IDisposable
{
    private bool _disposed = false;
    private readonly NetworkStream _networkStream;

    /// <summary>
    /// Create a new instance of a <see cref="SmtpStream"/>
    /// </summary>
    /// <param name="logger">A logger for writing to the server log</param>
    /// <param name="networkStream">A network stream for the connection to the client</param>
    public SmtpStream(NetworkStream networkStream)
    {
        _networkStream = networkStream;
    }

    /// <summary>
    /// Write a message to the SMTP stream
    /// </summary>
    /// <param name="message">The message to be sent</param>
    /// <param name="cancellationToken">A cancellation token for stopping the async task</param>
    /// <returns></returns>
    public async Task WriteAsync(string message, CancellationToken cancellationToken)
    {
        string toSend = message;
        if (!message.EndsWith("\r\n"))
        {
            // ensure the message ends with the SMTP terminator
            toSend = message + "\r\n";
        }

        await _networkStream.WriteAsync(Encoding.ASCII.GetBytes(toSend), cancellationToken);
    }

    /// <summary>
    /// Read the most recent message from the client
    /// </summary>
    /// <param name="cancellationToken">A cancellation token for stopping the async task</param>
    /// <returns>The message sent from the client</returns>
    public async Task<string> ReadNextAsync(CancellationToken cancellationToken)
    {
        byte[] bytes = new byte[64 * 1024];
        int bytesReadCount = await _networkStream.ReadAsync(bytes, cancellationToken);

        // Read message
        string message = Encoding.ASCII.GetString(bytes, 0, bytesReadCount);

        return message;
    }

    /// <summary>
    /// Close the SMTP stream
    /// </summary>
    public void Close()
    {
        _networkStream?.Close();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _networkStream?.Dispose();
            }
            _disposed = true;
        }
    }
}
