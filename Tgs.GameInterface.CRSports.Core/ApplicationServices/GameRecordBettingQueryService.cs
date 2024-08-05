using System.Runtime.CompilerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.CRSports.Core.ApplicationServices;

internal class GameRecordBettingQueryService : IQueryService<GameRecordBettingQuery, GameRecordResult>
{
	private readonly IGameApiClient _client;

	public GameRecordBettingQueryService(
		IGameApiClient client)
	{
		_client = client ?? throw new ArgumentNullException(nameof(client));
	}

	public async IAsyncEnumerable<GameRecordResult> QueryAsync(GameRecordBettingQuery query, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var resps = _client.ALLWagerAsync(
			apiUrl: query.ApiUrl,
			agentId: query.AgentId,
			secretKey: query.SecretKey,
			agentAccount: query.AgentAccount,
			agentPassword: query.AgentPassword,
			dateStart: query.StartTime,
			dateEnd: query.EndTime,
			settle: 0,
			page: query.Page,
			cancellationToken: cancellationToken);

		await foreach (var result in resps)
		{
			yield return new GameRecordResult(
				PlayerCode: result.Username,
				ExternalBetId: result.Id,
				ExternalCreatedOn: result.Adddate,
				ExternalClosedOn: DateTimeOffset.UnixEpoch,
				Status: Protos.GameRecordStatus.Running,
				IsCashOut: false,
				Bet: result.Gold,
				ValidBet: result.Gold,
				Settle: 0.0m,
				WinLoss: 0.0m,
				AdditionalData: result.AdditionalData);
		}
	}
}
