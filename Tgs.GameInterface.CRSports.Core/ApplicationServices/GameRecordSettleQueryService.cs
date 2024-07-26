using System.Runtime.CompilerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.CRSports.Core.ApplicationServices;
internal class GameRecordSettleQueryService(
	IGameApiClient client) : IQueryService<GameRecordSettleQuery, GameRecordResult>
{
	public async IAsyncEnumerable<GameRecordResult> QueryAsync(GameRecordSettleQuery query, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var resps = client.ALLWagerAsync(
			apiBaseUrl: query.ApiUrl,
			agentId: query.AgentId,
			secretKey: query.SecretKey,
			agentAccount: query.AgentAccount,
			agentPassword: query.AgentPassword,
			dateStart: query.StartTime,
			dateEnd: query.EndTime,
			settle: 1,
			page: query.Page,
			cancellationToken: cancellationToken);

		await foreach (var result in resps)
		{

			var isCashOut = !result.CashoutId.Equals("0", StringComparison.OrdinalIgnoreCase);

			yield return new GameRecordResult(
				PlayerCode: result.Username,
				ExternalBetId: result.Id,
				ExternalGameId: result.Gtype,
				ExternalCreatedOn: result.Adddate,
				ExternalClosedOn: result.Resultdate,
				IsClosed: true,
				IsCashOut: isCashOut,
				Bet: result.Gold,
				ValidBet: result.MembersVgold,
				Settle: result.Wingold,
				WinLoss: result.Wingold - result.Gold,
				AdditionalData: result.AdditionalData);
		}
	}
}
