using System.Net;
using System.Net.Sockets;

namespace SSMTP.Server
{
	public class Server : BackgroundService
	{
		private readonly TcpListener _tcpListener;
		private readonly ILogger _logger;

		public Server(
			ILogger<Server> logger) : base()
		{
			string address = "127.0.0.1";
			int port = 2025;

			_logger = logger;
			
			IPAddress localAddress = IPAddress.Parse(address);
			_tcpListener = new TcpListener(localAddress, port);
		}

		protected async override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			try
			{
				_logger.LogInformation("SSMTP server starting");
				_tcpListener.Start();

				// Enter the listening loop.
				while (!stoppingToken.IsCancellationRequested)
				{
					await InnerLoop(stoppingToken);
				}
			}
			catch (SocketException e)
			{
				_logger.LogError(e, "An error occurred during inner loop");
			}
			finally
			{
				_logger.LogInformation("SSMTP server stopping");
				_tcpListener.Stop();
			}
		}

		private async Task InnerLoop(CancellationToken stoppingToken)
		{
			using TcpClient client = await _tcpListener.AcceptTcpClientAsync();
			_logger.LogDebug("Connected!");

			var stream = new SmtpStream(_logger, client.GetStream());

			// Write opening message
			string opener = "220 localhost:2025";
			await stream.WriteAsync(opener, stoppingToken);

			var conversation = new Conversation();

			// Handle all client commands
			while (client.Connected)
			{
				try
				{
					var command = await stream.ReadNextAsync(stoppingToken);
					var response = conversation.HandleCommand(command);
					await stream.WriteAsync(response, stoppingToken);
				}
				catch (Exception e)
				{
					_logger.LogError(e, "Unable to handle command");
				}
			}

			_logger.LogDebug("Email Sent!");
			_logger.LogDebug(conversation.ToString());
		}
	}
}
