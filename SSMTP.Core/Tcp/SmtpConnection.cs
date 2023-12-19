using Microsoft.Extensions.Logging;
using System.Net.Sockets;

namespace SSMTP.Core.Tcp
{
	public class SmtpConnection : IDisposable
    {
		private bool _disposed = false;

		private readonly ILoggerFactory _loggerFactory;
		private readonly ILogger _logger;
		
		private readonly TcpClient _tcpClient;
        private readonly SmtpStream _smtpStream;

		public event EventHandler<EventArgs>? OnClosed;

        public SmtpConnection(ILoggerFactory loggerFactory, TcpClient tcpClient)
        {
			_loggerFactory = loggerFactory;
			_logger = loggerFactory.CreateLogger<SmtpConnection>();
        
			_tcpClient = tcpClient;

            var networkStream = tcpClient.GetStream();
            _smtpStream = new SmtpStream(loggerFactory.CreateLogger<SmtpStream>(), networkStream);
        }

		public async Task HandleAsync(CancellationToken stoppingToken)
		{
			// Write opening message
			string opener = $"220 127.0.0.1:2025";
			await _smtpStream.WriteAsync(opener, stoppingToken);
			_logger.LogDebug("Opener sent");

			var conversation = new SmtpConversation();
			conversation.EmailHandled += (e, email) =>
			{
				_logger.LogInformation("Email handled: '{email}'", email);
			};

			try
			{
				while(_tcpClient.Connected)
				{
					var command = await _smtpStream.ReadNextAsync(stoppingToken);		
					var response = conversation.HandleCommand(command);
						
					if (response == "")
					{
						continue;
					}
						
					await _smtpStream.WriteAsync(response, stoppingToken);
						
					if (response == "221 Bye")
					{
						break;
					}
				}
			}
			catch (IOException)
			{
				_logger.LogInformation("Connection closed by client");
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
		
		public void Close()
		{
			_logger.LogInformation("Closing SMTP connection");
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
