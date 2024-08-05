using Tgs.ServiceUtility;

namespace Tgs.GameInterface.CRSports.Core.ApplicationServices;

internal class AGLoginGetService : IGetService<AGLoginQuery, string>
{
	private readonly IAGLoginApiClient _client;

	public AGLoginGetService(
		IAGLoginApiClient client)
	{
		_client = client ?? throw new ArgumentNullException(nameof(client));
	}

	public ValueTask<string> GetAsync(AGLoginQuery query, CancellationToken cancellationToken = default)
		=> _client.AGLoginAsync(
			apiUrl: query.ApiUrl,
			agentId: query.AgentId,
			secretKey: query.SecretKey,
			agentAccount: query.AgentAccount,
			agentPassword: query.AgentPassword,
			cancellationToken: cancellationToken);
}
