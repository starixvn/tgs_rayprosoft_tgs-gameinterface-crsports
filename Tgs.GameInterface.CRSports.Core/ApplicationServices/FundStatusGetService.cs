using Tgs.ServiceUtility;

namespace Tgs.GameInterface.CRSports.Core.ApplicationServices;

internal class FundStatusGetService : IGetService<FundStatusQuery, bool>
{
	private readonly IGameApiClient _client;

	public FundStatusGetService(
		IGameApiClient client)
	{
		_client = client ?? throw new ArgumentNullException(nameof(client));
	}

	public ValueTask<bool> GetAsync(FundStatusQuery query, CancellationToken cancellationToken = default)
		=> _client.ChkTransInfoAsync(
			apiUrl: query.ApiUrl,
			agentId: query.AgentId,
			secretKey: query.SecretKey,
			agentAccount: query.AgentAccount,
			agentPassword: query.AgentPassword,
			memname: query.PlayerCode,
			payno: query.OrderId,
			cancellationToken: cancellationToken);
}
