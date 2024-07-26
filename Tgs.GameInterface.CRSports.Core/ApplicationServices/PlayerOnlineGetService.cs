using Tgs.ServiceUtility;

namespace Tgs.GameInterface.CRSports.Core.ApplicationServices;

internal class PlayerOnlineGetService(
	IGameApiClient client) : IGetService<PlayerOnlineQuery, bool>
{
	public async ValueTask<bool> GetAsync(PlayerOnlineQuery query, CancellationToken cancellationToken = default)
	{
		var resps = await client.GetMemOnlineListAsync(
			apiBaseUrl: query.ApiUrl,
			agentId: query.AgentId,
			secretKey: query.SecretKey,
			agentAccount: query.AgentAccount,
			agentPassword: query.AgentPassword,
			cancellationToken: cancellationToken)
			.ToArrayAsync(
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return resps.Where(x => x.UserUsername == query.PlayerCode).Any();
	}
}
