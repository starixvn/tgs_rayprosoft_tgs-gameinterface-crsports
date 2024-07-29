using System.Text.Json.Nodes;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GameInterface.CRSports.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.CRSports.IntegrationTest;

public class GameInterfaceCRSportsClient_FundOutUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var apiUrl = new Uri("https://api.orb-6789.com/");
		var agentId = 3238;
		var secretKey = "FnzrUTi5fcsdrws7";
		var agentAccount = "RT706test1";
		var agentPassword = "aaa123";
		var playerCode = "4uf6k014jp5x";
		var orderId = "E366D434-F19E-4706-8FB9-96DFB21F2EF5";
		var amount = 10.00m;

		var result = new FundResult(
			OrderId: "E366D434-F19E-4706-8FB9-96DFB21F2EF5",
			ExternalOrderId: "264549283",
			Resp: JsonNode.Parse("""
			{
				"method": "Deposit",
				"responeid": "23681717965",
				"respcode": "0000",
				"moneydata": {
					"gold": "199.0",
					"payno": "E366D434-F19E-4706-8FB9-96DFB21F2EF5",
					"pay_gold": "99.0",
					"currency": "HKD",
					"pay_way": "1",
					"recid": "264549283",
					"username": "4uf6k014jp5x"
				},
				"status": "success",
				"timestamp": "1721978380338"
			}
			""")!.ToJsonString());

		var web = new GameInterfaceCRSportsApplication(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeService = Substitute.For<IGetService<FundOutQuery, FundResult>>();

			var query = new FundOutQuery
			{
				ApiUrl = apiUrl,
				AgentId = agentId,
				SecretKey = secretKey,
				AgentAccount = agentAccount,
				AgentPassword = agentPassword,
				PlayerCode = playerCode,
				OrderId = orderId,
				Amount = amount
			};

			_ = fakeService.GetAsync(
				query: Arg.Is(query),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(result);

			_ = services.AddTransient(_ => fakeService);
		}));

		var sut = new Client.GameInterfaceCRSportsClient(new Protos.GameInterfaceCRSports.GameInterfaceCRSportsClient(
			GrpcChannel.ForAddress(
				address: web.Server.BaseAddress,
				channelOptions: new GrpcChannelOptions { HttpHandler = web.Server.CreateHandler() })));

		var actual = await sut.FundOutAsync(apiUrl, agentId, secretKey, agentAccount, agentPassword, playerCode, orderId, amount);

		Assert.NotNull(actual);
		Assert.Equal(result.OrderId, actual.OrderId);
		Assert.Equal(result.ExternalOrderId, actual.ExternalOrderId);
		Assert.NotEmpty(actual.Resp);
	}
}
