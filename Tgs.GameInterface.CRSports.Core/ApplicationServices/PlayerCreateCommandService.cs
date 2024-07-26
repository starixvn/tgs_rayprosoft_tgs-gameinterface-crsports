using Tgs.ServiceUtility;

namespace Tgs.GameInterface.CRSports.Core.ApplicationServices;

internal class PlayerCreateCommandService(IGameApiClient apiClient) : ICommandService<PlayerCreateCommand>
{
	public ValueTask ExecuteAsync(PlayerCreateCommand command, CancellationToken cancellationToken = default)
		=> apiClient.CreateMemberAsync(
			apiBaseUrl: command.ApiUrl,
			agentId: command.AgentId,
			secretKey: command.SecretKey,
			agentAccount: command.AgentAccount,
			agentPassword: command.AgentPassword,
			memname: command.PlayerCode,
			currency: command.Currency,
			cancellationToken: cancellationToken);
}
