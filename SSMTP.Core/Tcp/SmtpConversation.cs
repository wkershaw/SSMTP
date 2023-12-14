namespace SSMTP.Core.Tcp;

/// <summary>
/// A list of the possible commands sent from the client
/// that this server can handle
/// </summary>
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

internal class SmtpConversation
{
	private Commands _previousCommand;
	string _sender;
	private List<string> _recipients;
	private string _messageContent;

	public SmtpConversation()
	{
		_previousCommand = Commands.CONNECTION_OPENED;
		_sender = "";
		_recipients = new List<string>();
		_messageContent = "";
	}

	public string HandleCommand(string commandMessage)
	{
		Commands command = ParseCommand(commandMessage);

		// Use the previous command to det
		string response = (_previousCommand, command) switch
		{
			(Commands.CONNECTION_OPENED, Commands.EHLO) => HandleEhloCommand(),
			(Commands.EHLO, Commands.MAIL_FROM) => HandleMailFromCommand(commandMessage),
			(Commands.MAIL_FROM, Commands.RCPT_TO) => HandleRcptToCommand(commandMessage),
			(Commands.RCPT_TO, Commands.RCPT_TO) => HandleRcptToCommand(commandMessage),
			(Commands.RCPT_TO, Commands.DATA) => HandleDataCommand(),
			(Commands.DATA, Commands.MAIL_CONTENT) => HandleMailContent(commandMessage),
			(Commands.MAIL_CONTENT, Commands.MAIL_FROM) => HandleMailFromCommand(commandMessage),
			(Commands.MAIL_CONTENT, Commands.QUIT) => HandleQuitCommand(),
			_ => "Unexpected command"
		};

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

	private Commands ParseCommand(string command)
	{
		if (_previousCommand == Commands.DATA)
		{
			// If the previous command was DATA, we expect
			// the next message to be the mail content
			return Commands.MAIL_CONTENT;
		}

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
		_previousCommand = Commands.EHLO;
		return "250 Hello";
	}

	private string HandleMailFromCommand(string command)
	{
		// Command string is in format: MAIL FROM:<sender@domain.com>
		string sender = command.Replace("\r\n", "");        // Remove command line endings
		sender = sender.Substring(11, sender.Length - 12);  // Pull out the sender email address

		_sender = sender;

		_previousCommand = Commands.MAIL_FROM;
		return "250 Ok";
	}

	private string HandleRcptToCommand(string command)
	{
		// Command string is in format: RCPT TO:<recipient@domain.com>
		string recipient = command.Replace("\r\n", "");             // Remove command line endings
		recipient = recipient.Substring(9, recipient.Length - 10);  // Pull out the sender email address

		_recipients.Add(recipient);

		_previousCommand = Commands.RCPT_TO;
		return "250 Ok";
	}

	private string HandleDataCommand()
	{
		_previousCommand = Commands.DATA;
		return "354 End data with <CR><LF>.<CR><LF>";
	}

	private string HandleMailContent(string commandMessage)
	{
		_messageContent += commandMessage;

		if (commandMessage.EndsWith("\r\n.\r\n"))
		{
			_previousCommand = Commands.MAIL_CONTENT;
			return "250 Ok";
		}

		return "";
	}

	private string HandleQuitCommand()
	{
		_previousCommand = Commands.QUIT;
		return "221 Bye";
	}
}
