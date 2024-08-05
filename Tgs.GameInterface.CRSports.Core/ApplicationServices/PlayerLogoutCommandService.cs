using Tgs.ServiceUtility;

namespace Tgs.GameInterface.CRSports.Core.ApplicationServices;

internal class PlayerLogoutCommandService : ICommandService<PlayerLogoutCommand>
{
	private readonly IGameApiClient _client;

	public PlayerLogoutCommandService(
		IGameApiClient client)
	{
		_client = client ?? throw new ArgumentNullException(nameof(client));
	}

	public ValueTask ExecuteAsync(PlayerLogoutCommand command, CancellationToken cancellationToken = default)
		=> _client.KickOutMemAsync(
			apiUrl: command.ApiUrl,
			agentId: command.AgentId,
			secretKey: command.SecretKey,
			agentAccount: command.AgentAccount,
			agentPassword: command.AgentPassword,
			memname: command.PlayerCode,
			cancellationToken: cancellationToken);
}
