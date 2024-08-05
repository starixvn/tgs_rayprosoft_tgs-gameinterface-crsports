using Tgs.ServiceUtility;

namespace Tgs.GameInterface.CRSports.Core.ApplicationServices;

internal class PlayerOnlineGetService : IGetService<PlayerOnlineQuery, bool>
{
	private readonly IGameApiClient _client;

	public PlayerOnlineGetService(
		IGameApiClient client)
	{
		_client = client ?? throw new ArgumentNullException(nameof(client));
	}

	public ValueTask<bool> GetAsync(PlayerOnlineQuery query, CancellationToken cancellationToken = default)
		=> _client.GetMemOnlineListAsync(
			apiUrl: query.ApiUrl,
			agentId: query.AgentId,
			secretKey: query.SecretKey,
			agentAccount: query.AgentAccount,
			agentPassword: query.AgentPassword,
			cancellationToken: cancellationToken)
		.AnyAsync(cancellationToken: cancellationToken);
}
