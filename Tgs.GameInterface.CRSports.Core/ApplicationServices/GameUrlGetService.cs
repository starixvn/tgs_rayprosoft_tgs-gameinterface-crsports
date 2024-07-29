using Tgs.ServiceUtility;

namespace Tgs.GameInterface.CRSports.Core.ApplicationServices;

internal class GameUrlGetService(
	IGameApiClient client) : IGetService<GameUrlQuery, GameUrlResult>
{
	public async ValueTask<GameUrlResult> GetAsync(GameUrlQuery query, CancellationToken cancellationToken = default)
	{
		var resp = await client.LaunchGameAsync(
			apiUrl: query.ApiUrl,
			agentId: query.AgentId,
			secretKey: query.SecretKey,
			agentAccount: query.AgentAccount,
			agentPassword: query.AgentPassword,
			memname: query.PlayerCode,
			currency: query.Currency,
			machine: query.IsMobile ? "MOBILE" : "PC",
			langx: query.Language,
			remoteip: query.IpAddress,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new GameUrlResult(
			IsPlayerExist: resp.IsPlayerExist,
			GameUrl: resp.GameUrl);
	}
}
