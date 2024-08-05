using Tgs.ServiceUtility;

namespace Tgs.GameInterface.CRSports.Core.ApplicationServices;

internal class FundInGetService : IGetService<FundInQuery, FundResult>
{
	private readonly IGameApiClient _client;

	public FundInGetService(
		IGameApiClient client)
	{
		_client = client ?? throw new ArgumentNullException(nameof(client));
	}

	public async ValueTask<FundResult> GetAsync(FundInQuery query, CancellationToken cancellationToken = default)
	{
		var resp = await _client.WithdrawAsync(
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
