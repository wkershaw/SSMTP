using Microsoft.Extensions.Logging;
using SSMTP.Core.Models;

namespace SSMTP.Core.Persistence
{
	public class EmailRepository
	{
		private readonly ILogger _logger;

		public EmailRepository(ILogger logger)
		{
			_logger = logger;
		
		}

		public async Task SaveEmail(Email email)
		{
			_logger.LogDebug("Saving email {email}", email);
			
			await Task.CompletedTask;
		}
	}
}
