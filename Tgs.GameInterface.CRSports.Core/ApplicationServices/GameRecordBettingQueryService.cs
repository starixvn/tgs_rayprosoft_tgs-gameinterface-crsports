using System.Runtime.CompilerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.CRSports.Core.ApplicationServices;
internal class GameRecordBettingQueryService(
	IGameApiClient client) : IQueryService<GameRecordBettingQuery, GameRecordResult>
{
	public async IAsyncEnumerable<GameRecordResult> QueryAsync(GameRecordBettingQuery query, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var resps = client.ALLWagerAsync(
			apiBaseUrl: query.ApiUrl,
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
			var isClosed = result.Settle.Equals("1", StringComparison.OrdinalIgnoreCase);
			var isCashOut = !result.CashoutId.Equals("0", StringComparison.OrdinalIgnoreCase);
			var externalCreatedOn = result.Adddate;
			var externalClosedOn = DateTimeOffset.UnixEpoch;
			var bet = result.Gold;
			var validBet = result.Gold;
			var settle = 0.0m;
			var winLoss = 0.0m;

			if (isClosed)
			{
				externalClosedOn = result.Resultdate;
				validBet = result.MembersVgold;
				settle = result.Wingold;
				winLoss = settle - bet;
			}

			yield return new GameRecordResult(
				PlayerCode: result.Username,
				ExternalBetId: result.Id,
				ExternalGameId: result.Gtype,
				ExternalCreatedOn: externalCreatedOn,
				ExternalClosedOn: externalClosedOn,
				IsClosed: isClosed,
				IsCashOut: isCashOut,
				Bet: bet,
				ValidBet: validBet,
				Settle: settle,
				WinLoss: winLoss,
				AdditionalData: result.AdditionalData);
		}
	}
}
