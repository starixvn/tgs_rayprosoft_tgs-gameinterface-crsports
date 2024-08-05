using Tgs.ServiceUtility;

namespace Tgs.GameInterface.CRSports.Core.ApplicationServices;

internal class PlayerCreateCommandService : ICommandService<PlayerCreateCommand>
{
	private readonly IGameApiClient _client;

	public PlayerCreateCommandService(
		IGameApiClient client)
	{
		_client = client ?? throw new ArgumentNullException(nameof(client));
	}

	public ValueTask ExecuteAsync(PlayerCreateCommand command, CancellationToken cancellationToken = default)
		=> _client.CreateMemberAsync(
			apiUrl: command.ApiUrl,
			agentId: command.AgentId,
			secretKey: command.SecretKey,
			agentAccount: command.AgentAccount,
			agentPassword: command.AgentPassword,
			memname: command.PlayerCode,
			currency: command.Currency,
			cancellationToken: cancellationToken);
}
