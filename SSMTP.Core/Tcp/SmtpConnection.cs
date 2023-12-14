using Microsoft.Extensions.Logging;
using System.Net.Sockets;

namespace SSMTP.Core.Tcp
{
	public class SmtpConnection : IDisposable
    {
		private bool _disposed = false;

		private readonly ILogger _logger;
		private readonly TcpClient _tcpClient;
        private readonly SmtpStream _smtpStream;

		public event EventHandler<EventArgs>? OnClosed;

        public SmtpConnection(ILogger logger, TcpClient tcpClient)
        {
			_logger = logger;
            _tcpClient = tcpClient;

            var networkStream = tcpClient.GetStream();
            _smtpStream = new SmtpStream(networkStream);
        }

		public async Task HandleAsync(CancellationToken stoppingToken)
		{
			// Write opening message
			string opener = $"220 127.0.0.1:2025";
			await _smtpStream.WriteAsync(opener, stoppingToken);
			_logger.LogDebug("Opener sent");

			var conversation = new SmtpConversation();

			// Handle all client commands
			while (_tcpClient.Connected)
			{
				try
				{
					while(true)
					{
						var command = await _smtpStream.ReadNextAsync(stoppingToken);
						_logger.LogDebug("Command recieved '{command}'", command);
					
						var response = conversation.HandleCommand(command);
						
						if (response == "")
						{
							continue;
						}
						
						if (response == "221 Bye")
						{
							break;
						}

						await _smtpStream.WriteAsync(response, stoppingToken);
						_logger.LogDebug("Response sent '{response}'", response);
					}
				}
				catch (IOException)
				{
					_logger.LogDebug("Connection closed by client");
				}
				catch (Exception e)
				{
					_logger.LogError(e, "Unable to handle command");
				}
				finally
				{
					Close();
				}
			}
		}
		
		public void Close()
		{
			_tcpClient.Close();
			OnClosed?.Invoke(this, EventArgs.Empty);
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
					_smtpStream?.Dispose();
					_tcpClient?.Dispose();
				}
				_disposed = true;
			}
		}
    }
}
