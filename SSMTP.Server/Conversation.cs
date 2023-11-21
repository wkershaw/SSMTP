namespace SSMTP.Server
{
	internal enum Commands
	{
		CONNECTION_OPENED,
		EHLO,
		MAIL_FROM,
		RCPT_TO,
		DATA,
		MAIL_CONTENT,
		QUIT
	}

	internal class Conversation
	{
		private Commands _previousCommand;
		string _sender;
		private List<string> _recipients;
		private string _messageContent;

		public Conversation()
		{
			_previousCommand = Commands.CONNECTION_OPENED;
			_sender = "";
			_recipients = new List<string>();
			_messageContent = "";
		}

		public string HandleCommand(string commandMessage)
		{
			if (_previousCommand == Commands.DATA)
			{
				string mailContentResponse = HandleMessageContent(commandMessage);

				if (commandMessage.EndsWith("\r\n.\r\n"))
				{
					_previousCommand = Commands.MAIL_CONTENT;
				}

				return mailContentResponse;
			}

			Commands command = ParseCommand(commandMessage);

			// Use the previous command to det
			string response = (_previousCommand, command) switch
			{
				(Commands.CONNECTION_OPENED, Commands.EHLO) => HandleEhloCommand(),
				(Commands.EHLO, Commands.MAIL_FROM) => HandleMailFromCommand(commandMessage),
				(Commands.MAIL_FROM, Commands.RCPT_TO) => HandleRcptToCommand(commandMessage),
				(Commands.RCPT_TO, Commands.RCPT_TO) => HandleRcptToCommand(commandMessage),
				(Commands.RCPT_TO, Commands.DATA) => HandleDataCommand(),
				(Commands.MAIL_CONTENT, Commands.QUIT) => HandleQuitCommand(),
				_ => "Unexpected command"
			};

			// Update the previous command
			_previousCommand = command;

			// Return the response to be sent
			return response;
		}

		public override string ToString()
		{
			return $"""
			To: {string.Join(',', _recipients)}
			From: {_sender}
			{_messageContent}
			""";
		}

		private static Commands ParseCommand(string command)
		{
			if (command.StartsWith("EHLO") ||
				command.StartsWith("HELO"))
			{
				return Commands.EHLO;
			}

			if (command.StartsWith("MAIL FROM"))
			{
				return Commands.MAIL_FROM;
			}

			if (command.StartsWith("RCPT TO"))
			{
				return Commands.RCPT_TO;
			}

			if (command.StartsWith("DATA"))
			{
				return Commands.DATA;
			}

			if (command.StartsWith("QUIT"))
			{
				return Commands.QUIT;
			}

			throw new Exception("Unrecognised command");
		}
		private string HandleEhloCommand()
		{
			return "250 Hello";
		}
		private string HandleMailFromCommand(string command)
		{
			// Command string is in format: MAIL FROM:<sender@domain.com>
			string sender = command
				.Replace("\r\n", "")                    // Remove command line endings
				.Substring(11, command.Length - 12);        // Pull out the sender email address

			_sender = sender;

			return "250 Ok";
		}
		private string HandleRcptToCommand(string command)
		{
			// Command string is in format: RCPT TO:<recipient@domain.com>
			string recipient = command
				.Replace("\r\n", "")                    // Remove command line endings
				.Substring(9, command.Length - 10);     // Pull out the sender email address

			_recipients.Add(recipient);

			return "250 Ok";
		}
		private string HandleDataCommand()
		{
			return "354 End data with <CR><LF>.<CR><LF>";
		}
		private string HandleMessageContent(string commandMessage)
		{
			_messageContent = commandMessage;
			return "250 Ok";
		}
		private string HandleQuitCommand()
		{
			return "221 Bye";
		}

	}
}
