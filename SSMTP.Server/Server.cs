﻿using SSMTP.Core.Tcp;
using System.Net;
using System.Net.Sockets;

namespace SSMTP.Server
{
	/// <summary>
	/// A server that listens to incoming TCP connections and handles SMTP
	/// requests from the client
	/// </summary>
	public class Server : BackgroundService
	{
		private static readonly IPAddress SERVER_ADDRESS = IPAddress.Parse("127.0.0.1");
		private const int SERVER_PORT = 2025;

		private readonly TcpListener _tcpListener;
		private readonly ILogger _logger;
		private readonly List<SmtpConnection> _connections;

		/// <summary>
		/// Create a new instance of the SMTP server
		/// </summary>
		/// <param name="logger">A logger for adding information to the server log</param>
		public Server(ILogger<Server> logger) : base()
		{
			_logger = logger;
			_tcpListener = new TcpListener(SERVER_ADDRESS, SERVER_PORT);
			_connections = new List<SmtpConnection>();
		}

		/// <summary>
		/// Called when the server starts. Handles the setup of the TCP listener
		/// and begins handling requests.
		/// </summary>
		/// <param name="stoppingToken">A cancellation token for stopping the server</param>
		protected async override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			try
			{
				_logger.LogInformation("SSMTP server starting");
				_tcpListener.Start();

				// Enter the listening loop.
				while (!stoppingToken.IsCancellationRequested)
				{
					await AcceptNextConnectionAsync(stoppingToken);
				}
			}
			catch (SocketException e)
			{
				_logger.LogError(e, "A socket exception occurred during inner loop");
			}
			catch (Exception e)
			{
				_logger.LogError(e, "An error occurred during the inner loop");
			}
			finally
			{
				_logger.LogInformation("SSMTP server stopping");
				_tcpListener.Stop();
			}
		}

		private async Task AcceptNextConnectionAsync(CancellationToken stoppingToken)
		{
			TcpClient client = await _tcpListener.AcceptTcpClientAsync(stoppingToken);
			_logger.LogDebug("A client has connected to the SSMTP server");

			var connection = new SmtpConnection(_logger, client);
			connection.OnClosed += (e, s) =>
			{
				_logger.LogInformation("Connection closed");
				connection.Dispose();
				_connections.Remove(connection);
			};

			_connections.Add(connection);

			_ = connection.HandleAsync(stoppingToken);
		}
	}
}
