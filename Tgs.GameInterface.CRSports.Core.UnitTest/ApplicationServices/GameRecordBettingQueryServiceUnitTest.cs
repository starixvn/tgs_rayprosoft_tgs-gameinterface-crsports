using System.Text.Json.Nodes;
using NSubstitute;
using Tgs.GameInterface.CRSports.Core.ApplicationServices;
using Tgs.GameInterface.CRSports.Core.Models;

namespace Tgs.GameInterface.CRSports.Core.UnitTest.ApplicationServices;

public class GameRecordBettingQueryServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameApiClient = Substitute.For<IGameApiClient>();

		var query = new GameRecordBettingQuery
		{
			ApiUrl = new Uri("https://api.orb-6789.com/"),
			AgentId = 3238,
			SecretKey = "FnzrUTi5fcsdrws7",
			AgentAccount = "RT706test1",
			AgentPassword = "aaa123",
			StartTime = DateTimeOffset.Parse("2024-07-26T16:30:00+08:00"),
			EndTime = DateTimeOffset.Parse("2024-07-26T17:00:00+08:00"),
			Page = 1
		};

		var resp = new WagerDataResp(
			Id: "438050329",
			Username: "4uf6k014jp5x",
			Gtype: "FT",
			Adddate: new DateTimeOffset(DateTime.Parse("2024-07-26 04:31:51"), TimeSpan.FromHours(-4)),
			Resultdate: DateTimeOffset.UnixEpoch,
			Settle: "0",
			Result: "0",
			Gold: 2.0m,
			MembersVgold: 2.0m,
			Wingold: 4.08m,
			CashoutId: "0",
			AdditionalData: new JsonObject
			{
				["oddsFormat"] = "0.5 / 1",
				["strong"] = "C",
				["gtype"] = "FT",
				["degold"] = "2.000",
				["settle"] = "0",
				["mid"] = "28831508",
				["wingold_d"] = "3.90",
				["orderdate"] = "2024-07-26",
				["wingold"] = "4.08",
				["gold"] = "2.000",
				["result"] = "0",
				["rtype"] = "Running Ball-Home",
				["score"] = "0:0",
				["tname_home"] = "Real Murcia",
				["gold_d"] = "1.91",
				["cashoutid"] = "0",
				["odds"] = "H",
				["wtypecode"] = "RE",
				["currency"] = "HKD",
				["degold_d"] = "1.910",
				["id"] = "438050329",
				["adddate"] = "2024-07-26 04:31:51",
				["order"] = "Real Murcia",
				["cashout_d"] = "0.000",
				["tname_away"] = "Millwall",
				["handicap"] = "2",
				["pname"] = "",
				["IP"] = "185.196.0.16",
				["league"] = "Club Friendlies",
				["ioratio"] = "1.04",
				["rtypecode"] = "REH",
				["ordertime"] = "04:30:00",
				["cashout"] = "0.000",
				["report_test"] = "",
				["wtype"] = "Running Ball",
				["resultdetail"] = "0",
				["result_score"] = "",
				["username"] = "4uf6k014jp5x"
			}.ToJsonString());

		_ = fakeGameApiClient.ALLWagerAsync(
			apiUrl: Arg.Is(query.ApiUrl),
			agentId: Arg.Is(query.AgentId),
			secretKey: Arg.Is(query.SecretKey),
			agentAccount: Arg.Is(query.AgentAccount),
			agentPassword: Arg.Is(query.AgentPassword),
			dateStart: Arg.Is(query.StartTime),
			dateEnd: Arg.Is(query.EndTime),
			settle: Arg.Is(0),
			page: Arg.Is(query.Page),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(new[] { resp }.ToAsyncEnumerable());

		var sut = new GameRecordBettingQueryService(fakeGameApiClient);

		var actual = await sut.QueryAsync(
			query: query,
			cancellationToken: default)
			.ToArrayAsync();

		Assert.NotNull(actual);
		Assert.Equal("4uf6k014jp5x", actual[0].PlayerCode);
		Assert.Equal("438050329", actual[0].ExternalBetId);
		Assert.Equal(new DateTimeOffset(DateTime.Parse("2024-07-26 04:31:51"), TimeSpan.FromHours(-4)), actual[0].ExternalCreatedOn);
		Assert.Equal(DateTimeOffset.UnixEpoch, actual[0].ExternalClosedOn);
		Assert.Equal(Protos.GameRecordStatus.Running, actual[0].Status);
		Assert.False(actual[0].IsCashOut);
		Assert.Equal(2.0m, actual[0].Bet);
		Assert.Equal(2.0m, actual[0].ValidBet);
		Assert.Equal(0.0m, actual[0].Settle);
		Assert.Equal(0.0m, actual[0].WinLoss);
		Assert.NotEmpty(actual[0].AdditionalData);
	}
}
