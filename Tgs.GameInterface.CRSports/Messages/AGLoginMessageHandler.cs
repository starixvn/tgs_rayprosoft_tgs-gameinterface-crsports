using Tgs.GameInterface.CRSports.Core.ApplicationServices;
using Tgs.GameInterface.CRSports.Protos;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameInterface.CRSports.Messages;

internal class AGLoginMessageHandler(
	ILogger<AGLoginMessageHandler> logger,
	IGetService<AGLoginQuery, string> service) : IMessageProcessor<AGLoginRequest, AGLoginReply>
{
	public async ValueTask<AGLoginReply> HandleAsync(string subject, AGLoginRequest data, IEnumerable<MessageHeaderValue>? headerValues, CancellationToken cancellationToken = default)
	{
		logger.LogInformation("{logInformation}",
			new
			{
				LogOn = DateTime.Now.ToString("o"),
				Subject = subject,
				Mesaage = new
				{
					data.ApiUrl,
					data.AgentId,
					data.SecretKey,
					data.AgentAccount,
					data.AgentPassword
				}
			});

		var token = await service.GetAsync(
			query: new AGLoginQuery
			{
				ApiUrl = new Uri(data.ApiUrl),
				AgentId = data.AgentId,
				SecretKey = data.SecretKey,
				AgentAccount = data.AgentAccount,
				AgentPassword = data.AgentPassword
			},
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new AGLoginReply
		{
			Token = token
		};
	}
}
