namespace SSMTP.Core;

public class Email
{
	public Dictionary<string, string> Headers { get; set; }

	public string Body { get; set; }

	public Email()
	{
		Headers = new Dictionary<string, string>();
		Body = "";
	}

	public static Email FromSmtpMessage(string message)
	{
		var email = new Email()
		{
			Body = message
		};

		return email;
	}
}
