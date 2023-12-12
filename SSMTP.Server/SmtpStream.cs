using System.Net.Sockets;
using System.Text;

namespace SSMTP.Server
{
	/// <summary>
	/// Represents a network stream with a client used to send and receive SMTP messages
	/// </summary>
	internal class SmtpStream
	{
		private readonly ILogger _logger;
		private readonly NetworkStream _networkStream;

		/// <summary>
		/// Create a new instance of a <see cref="SmtpStream"/>
		/// </summary>
		/// <param name="logger">A logger for writing to the server log</param>
		/// <param name="networkStream">A network stream for the connection to the client</param>
		public SmtpStream(ILogger logger, NetworkStream networkStream)
		{
			_logger = logger;
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
			_logger.LogDebug("Sending message: '{message}'", message);

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

			// Strip line endings
			//message = message.Replace("\r\n", "");

			_logger.LogDebug("Reading message: '{message}'", message);
			return message;
		}

		/// <summary>
		/// Close the SMTP stream
		/// </summary>
		public void Close()
		{
			_logger.LogDebug("Closing connection with client");
			_networkStream?.Close();
		}
	}
}
