using System.Net.Mail;

namespace EmailClient
{
	public partial class EmailClient : Form
	{
		private readonly SmtpClient _smtpClient;

		private readonly string _fromAddress = "from@example.com";
		private readonly List<string> _toAddresses = ["to@example.com"];
		private readonly string _subject = "Subject";

		public EmailClient()
		{
			InitializeComponent();

			_smtpClient = new SmtpClient("127.0.0.1")
			{
				Port = 2025,
				EnableSsl = false
			};
		}

		/// <summary>
		/// Event handler for when the 'Send' button is clicked
		/// </summary>
		private void SendButton_Click(object sender, EventArgs e)
		{
			StatusMessage.Text = "";

			try
			{
				_smtpClient.Send(
					_fromAddress,
					string.Join(',',_toAddresses),
					_subject,
					ContentTextBox.Text);
			}
			catch (Exception ex)
			{
				StatusMessage.Text = ex.Message;
				return;
			}

			StatusMessage.Text = "Success!";
		}
	}
}
