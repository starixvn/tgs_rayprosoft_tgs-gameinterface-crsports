using Tgs.ServiceUtility;

namespace Tgs.GameInterface.CRSports.Core.ApplicationServices;

internal class FundOutGetService(IGameApiClient apiClient) : IGetService<FundOutQuery, FundResult>
{
	public async ValueTask<FundResult> GetAsync(FundOutQuery query, CancellationToken cancellationToken = default)
	{
		var resp = await apiClient.DepositAsync(
			apiUrl: query.ApiUrl,
			agentId: query.AgentId,
			secretKey: query.SecretKey,
			agentAccount: query.AgentAccount,
			agentPassword: query.AgentPassword,
			memname: query.PlayerCode,
			payno: query.OrderId,
			amount: query.Amount,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new FundResult(
			OrderId: resp.Payno,
			ExternalOrderId: resp.Recid,
			Resp: resp.Resp);
	}
}
