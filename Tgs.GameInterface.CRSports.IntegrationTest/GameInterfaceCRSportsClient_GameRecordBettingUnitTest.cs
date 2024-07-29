using System.Text.Json.Nodes;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GameInterface.CRSports.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.CRSports.IntegrationTest;

public class GameInterfaceCRSportsClient_GameRecordBettingUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var apiUrl = new Uri("https://api.orb-6789.com/");
		var agentId = 3238;
		var secretKey = "FnzrUTi5fcsdrws7";
		var agentAccount = "RT706test1";
		var agentPassword = "aaa123";
		var startTime = DateTimeOffset.Parse("2024-07-26T16:30:00+08:00");
		var endTime = DateTimeOffset.Parse("2024-07-26T17:00:00+08:00");
		var page = 1;

		var result = new GameRecordResult(
			PlayerCode: "4uf6k014jp5x",
			ExternalBetId: "438050329",
			ExternalCreatedOn: new DateTimeOffset(DateTime.Parse("2024-07-26 04:31:51"), TimeSpan.FromHours(-4)),
			ExternalClosedOn: DateTimeOffset.UnixEpoch,
			Status: Protos.GameRecordStatus.Running,
			IsCashOut: false,
			Bet: 2.0m,
			ValidBet: 2.0m,
			Settle: 0.0m,
			WinLoss: 0.0m,
			AdditionalData: JsonNode.Parse("""
			{
				"oddsFormat": "0.5 / 1",
				"strong": "C",
				"gtype": "FT",
				"degold": "2.000",
				"settle": "0",
				"mid": "28831508",
				"wingold_d": "3.90",
				"orderdate": "2024-07-26",
				"wingold": "4.08",
				"gold": "2.000",
				"result": "0",
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
				"ordertime": "04:30:00",
				"cashout": "0.000",
				"report_test": "",
				"wtype": "Running Ball",
				"resultdetail": "0",
				"result_score": "",
				"username": "4uf6k014jp5x"
			}
			""")!.ToJsonString());

		var web = new GameInterfaceCRSportsApplication(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeService = Substitute.For<IQueryService<GameRecordBettingQuery, GameRecordResult>>();

			var query = new GameRecordBettingQuery
			{
				ApiUrl = apiUrl,
				AgentId = agentId,
				SecretKey = secretKey,
				AgentAccount = agentAccount,
				AgentPassword = agentPassword,
				StartTime = startTime,
				EndTime = endTime,
				Page = page
			};

			_ = fakeService.QueryAsync(
				query: Arg.Is(query),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(new[] { result }.ToAsyncEnumerable());

			_ = services.AddTransient(_ => fakeService);
		}));

		var sut = new Client.GameInterfaceCRSportsClient(new Protos.GameInterfaceCRSports.GameInterfaceCRSportsClient(
			GrpcChannel.ForAddress(
				address: web.Server.BaseAddress,
				channelOptions: new GrpcChannelOptions { HttpHandler = web.Server.CreateHandler() })));

		var actual = await sut.GameRecordBettingAsync(apiUrl, agentId, secretKey, agentAccount, agentPassword, startTime, endTime, page).ToArrayAsync();

		Assert.NotNull(actual);
		Assert.Equal("4uf6k014jp5x", actual[0].PlayerCode);
		Assert.Equal("438050329", actual[0].ExternalBetId);
		Assert.Equal(new DateTimeOffset(DateTime.Parse("2024-07-26 04:31:51"), TimeSpan.FromHours(-4)), actual[0].ExternalCreatedOn.ToDateTimeOffset());
		Assert.Equal(DateTimeOffset.UnixEpoch, actual[0].ExternalClosedOn.ToDateTimeOffset());
		Assert.Equal(Protos.GameRecordStatus.Running, actual[0].Status);
		Assert.False(actual[0].IsCashOut);
		Assert.Equal(2.0m, actual[0].Bet.ToDecimal());
		Assert.Equal(2.0m, actual[0].ValidBet.ToDecimal());
		Assert.Equal(0.0m, actual[0].Settle.ToDecimal());
		Assert.Equal(0.0m, actual[0].WinLoss.ToDecimal());
		Assert.NotEmpty(actual[0].AdditionalData);
	}
}
