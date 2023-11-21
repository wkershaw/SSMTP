using System.Net.Sockets;
using System.Text;

namespace SSMTP.Server
{
	internal class SmtpStream
	{
		private ILogger _logger;
		private NetworkStream _networkStream;

		public SmtpStream(
			ILogger logger,
			NetworkStream networkStream)
		{
			_logger = logger;
			_networkStream = networkStream;
		}

		public async Task WriteAsync(string message, CancellationToken cancellationToken)
		{
			_logger.LogDebug("Sending message: '{message}'", message);

			string toSend = message;
			if (!message.EndsWith("\r\n"))
			{
				toSend = message + "\r\n";
			}

			await _networkStream.WriteAsync(Encoding.ASCII.GetBytes(toSend), cancellationToken);
		}

		public async Task<string> ReadNextAsync(CancellationToken cancellationToken)
		{
			byte[] bytes = new byte[64 * 1024];
			int bytesReadCount = await _networkStream.ReadAsync(bytes, cancellationToken);

			// Read message
			string message = Encoding.ASCII.GetString(bytes, 0, bytesReadCount);

			// Strip line endings
			message = message.Replace("\r\n", "");

			_logger.LogDebug("Reading message: '{message}'", message);
			return message;
		}
	}
}
