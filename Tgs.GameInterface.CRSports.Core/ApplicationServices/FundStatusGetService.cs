using Tgs.ServiceUtility;

namespace Tgs.GameInterface.CRSports.Core.ApplicationServices;

internal class FundStatusGetService(IGameApiClient apiClient) : IGetService<FundStatusQuery, bool>
{
	public ValueTask<bool> GetAsync(FundStatusQuery query, CancellationToken cancellationToken = default)
		=> apiClient.ChkTransInfoAsync(
			apiBaseUrl: query.ApiUrl,
			agentId: query.AgentId,
			secretKey: query.SecretKey,
			agentAccount: query.AgentAccount,
			agentPassword: query.AgentPassword,
			memname: query.PlayerCode,
			payno: query.OrderId,
			cancellationToken: cancellationToken);
}
