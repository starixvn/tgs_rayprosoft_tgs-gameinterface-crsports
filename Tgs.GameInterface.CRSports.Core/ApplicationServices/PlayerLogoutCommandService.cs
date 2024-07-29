using Tgs.ServiceUtility;

namespace Tgs.GameInterface.CRSports.Core.ApplicationServices;

internal class PlayerLogoutCommandService(IGameApiClient apiClient) : ICommandService<PlayerLogoutCommand>
{
	public ValueTask ExecuteAsync(PlayerLogoutCommand command, CancellationToken cancellationToken = default)
		=> apiClient.KickOutMemAsync(
			apiUrl: command.ApiUrl,
			agentId: command.AgentId,
			secretKey: command.SecretKey,
			agentAccount: command.AgentAccount,
			agentPassword: command.AgentPassword,
			memname: command.PlayerCode,
			cancellationToken: cancellationToken);
}
