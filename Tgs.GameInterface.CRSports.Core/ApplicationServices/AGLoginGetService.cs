using Tgs.ServiceUtility;

namespace Tgs.GameInterface.CRSports.Core.ApplicationServices;

internal class AGLoginGetService(
	IGameApiClient client) : IGetService<AGLoginQuery, string>
{
	public ValueTask<string> GetAsync(AGLoginQuery query, CancellationToken cancellationToken = default)
		=> client.AGLoginAsync(
			apiBaseUrl: query.ApiUrl,
			agentId: query.AgentId,
			secretKey: query.SecretKey,
			agentAccount: query.AgentAccount,
			agentPassword: query.AgentPassword,
			cancellationToken: cancellationToken);
}
