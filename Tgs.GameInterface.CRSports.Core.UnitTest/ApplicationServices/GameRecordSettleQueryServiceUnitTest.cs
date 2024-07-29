using System.Text.Json.Nodes;
using NSubstitute;
using Tgs.GameInterface.CRSports.Core.ApplicationServices;
using Tgs.GameInterface.CRSports.Core.Models;

namespace Tgs.GameInterface.CRSports.Core.UnitTest.ApplicationServices;

public class GameRecordSettleQueryServiceUnitTest
{
	[Fact]
	public async Task Lose()
	{
		var fakeGameApiClient = Substitute.For<IGameApiClient>();

		var query = new GameRecordSettleQuery
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
			Resultdate: new DateTimeOffset(DateTime.Parse("2024-07-26 06:19:25"), TimeSpan.FromHours(-4)),
			Settle: "1",
			Result: "L",
			Gold: 2.0m,
			MembersVgold: 1.0m,
			Wingold: 1.0m,
			CashoutId: "0",
			AdditionalData: JsonNode.Parse("""
			{
				"oddsFormat": "0.5 / 1",
				"strong": "C",
				"gtype": "FT",
				"degold": "2.000",
				"settle": "1",
				"mid": "28831508",
				"wingold_d": "0.95",
				"orderdate": "2024-07-26",
				"wingold": "1.00",
				"gold": "2.000",
				"result": "L",
				"rtype": "Running Ball-Home",
				"score": "0:0",
				"tname_home": "Real Murcia",
				"gold_d": "1.91",
				"cashoutid": "0",
				"odds": "H",
				"wtypecode": "RE",
				"currency": "HKD",
				"degold_d": "1.910",
				"id": "438050329",
				"adddate": "2024-07-26 04:31:51",
				"order": "Real Murcia",
				"cashout_d": "0.000",
				"tname_away": "Millwall",
				"handicap": "2",
				"pname": "",
				"IP": "185.196.0.16",
				"league": "Club Friendlies",
				"ioratio": "1.04",
				"rtypecode": "REH",
				"members_vgold": "1",
				"ordertime": "04:30:00",
				"cashout": "0.000",
				"report_test": "",
				"wtype": "Running Ball",
				"resultdetail": "LL",
				"vgold": "0.96",
				"resultdate": "2024-07-26 06:19:25",
				"result_score": "1:2",
				"username": "4uf6k014jp5x"
			}
			""")!.ToJsonString());

		_ = fakeGameApiClient.ALLWagerAsync(
			apiUrl: Arg.Is(query.ApiUrl),
			agentId: Arg.Is(query.AgentId),
			secretKey: Arg.Is(query.SecretKey),
			agentAccount: Arg.Is(query.AgentAccount),
			agentPassword: Arg.Is(query.AgentPassword),
			dateStart: Arg.Is(query.StartTime),
			dateEnd: Arg.Is(query.EndTime),
			settle: Arg.Is(1),
			page: Arg.Is(query.Page),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(new[] { resp }.ToAsyncEnumerable());

		var sut = new GameRecordSettleQueryService(fakeGameApiClient);

		var actual = await sut.QueryAsync(
			query: query,
			cancellationToken: default)
			.ToArrayAsync();

		Assert.NotNull(actual);
		Assert.Equal("4uf6k014jp5x", actual[0].PlayerCode);
		Assert.Equal("438050329", actual[0].ExternalBetId);
		Assert.Equal(new DateTimeOffset(DateTime.Parse("2024-07-26 04:31:51"), TimeSpan.FromHours(-4)), actual[0].ExternalCreatedOn);
		Assert.Equal(new DateTimeOffset(DateTime.Parse("2024-07-26 06:19:25"), TimeSpan.FromHours(-4)), actual[0].ExternalClosedOn);
		Assert.Equal(Protos.GameRecordStatus.Lose, actual[0].Status);
		Assert.False(actual[0].IsCashOut);
		Assert.Equal(2.0m, actual[0].Bet);
		Assert.Equal(1.0m, actual[0].ValidBet);
		Assert.Equal(1.0m, actual[0].Settle);
		Assert.Equal(-1.0m, actual[0].WinLoss);
		Assert.NotEmpty(actual[0].AdditionalData);
	}

	[Fact]
	public async Task Cancel()
	{
		var fakeGameApiClient = Substitute.For<IGameApiClient>();

		var query = new GameRecordSettleQuery
		{
			ApiUrl = new Uri("https://api.orb-6789.com/"),
			AgentId = 3238,
			SecretKey = "FnzrUTi5fcsdrws7",
			AgentAccount = "RT706test1",
			AgentPassword = "aaa123",
			StartTime = DateTimeOffset.Parse("2024-07-29T13:30:00+08:00"),
			EndTime = DateTimeOffset.Parse("2024-07-29T14:00:00+08:00"),
			Page = 1
		};

		var resp = new WagerDataResp(
			Id: "439117464",
			Username: "4uf6k014jp5x",
			Gtype: "FT",
			Adddate: new DateTimeOffset(DateTime.Parse("2024-07-29 01:33:19"), TimeSpan.FromHours(-4)),
			Resultdate: new DateTimeOffset(DateTime.Parse("2024-07-29 01:45:17"), TimeSpan.FromHours(-4)),
			Settle: "1",
			Result: "D",
			Gold: 2.0m,
			MembersVgold: 0.00m,
			Wingold: 2.00m,
			CashoutId: "0",
			AdditionalData: JsonNode.Parse("""
			{
				"oddsFormat": "2",
				"strong": "",
				"gtype": "FT",
				"degold": "2.000",
				"settle": "1",
				"mid": "28831508",
				"wingold_d": "1.91",
				"orderdate": "2024-07-29",
				"wingold": "2.00",
				"gold": "2",
				"result": "D",
				"rtype": "Over/Under-over",
				"score": "",
				"tname_home": "Barracas Central",
				"gold_d": "1.91",
				"cashoutid": "0",
				"odds": "H",
				"wtypecode": "OU",
				"currency": "HKD",
				"degold_d": "1.910",
				"id": "439117464",
				"adddate": "2024-07-29 01:33:19",
				"order": "over",
				"cashout_d": "0.000",
				"tname_away": "CA Tucuman",
				"handicap": "2",
				"pname": "",
				"IP": "185.196.0.9",
				"league": "Argentina Liga Profesional",
				"ioratio": "0.97",
				"rtypecode": "OUC",
				"members_vgold": "0.00",
				"ordertime": "14:00:00",
				"cashout": "0.000",
				"report_test": "",
				"wtype": "Over/Under",
				"resultdetail": "D",
				"vgold": "0.00",
				"resultdate": "2024-07-29 01:45:17",
				"result_score": "0:0",
				"username": "4uf6k014jp5x"
			}
			""")!.ToJsonString());

		_ = fakeGameApiClient.ALLWagerAsync(
			apiUrl: Arg.Is(query.ApiUrl),
			agentId: Arg.Is(query.AgentId),
			secretKey: Arg.Is(query.SecretKey),
			agentAccount: Arg.Is(query.AgentAccount),
			agentPassword: Arg.Is(query.AgentPassword),
			dateStart: Arg.Is(query.StartTime),
			dateEnd: Arg.Is(query.EndTime),
			settle: Arg.Is(1),
			page: Arg.Is(query.Page),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(new[] { resp }.ToAsyncEnumerable());

		var sut = new GameRecordSettleQueryService(fakeGameApiClient);

		var actual = await sut.QueryAsync(
			query: query,
			cancellationToken: default)
			.ToArrayAsync();

		Assert.NotNull(actual);
		Assert.Equal("4uf6k014jp5x", actual[0].PlayerCode);
		Assert.Equal("439117464", actual[0].ExternalBetId);
		Assert.Equal(new DateTimeOffset(DateTime.Parse("2024-07-29 01:33:19"), TimeSpan.FromHours(-4)), actual[0].ExternalCreatedOn);
		Assert.Equal(new DateTimeOffset(DateTime.Parse("2024-07-29 01:45:17"), TimeSpan.FromHours(-4)), actual[0].ExternalClosedOn);
		Assert.Equal(Protos.GameRecordStatus.Cancel, actual[0].Status);
		Assert.False(actual[0].IsCashOut);
		Assert.Equal(2.0m, actual[0].Bet);
		Assert.Equal(0.0m, actual[0].ValidBet);
		Assert.Equal(2.0m, actual[0].Settle);
		Assert.Equal(0.0m, actual[0].WinLoss);
		Assert.NotEmpty(actual[0].AdditionalData);
	}
}
