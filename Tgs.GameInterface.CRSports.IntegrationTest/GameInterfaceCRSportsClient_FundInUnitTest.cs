using System.Text.Json.Nodes;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GameInterface.CRSports.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.CRSports.IntegrationTest;

public class GameInterfaceCRSportsClient_FundInUnitTest
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
		var orderId = "29CDE229-AE62-49BA-B03B-2EB7288C0058";
		var amount = 10.00m;

		var result = new FundResult(
			OrderId: "29CDE229-AE62-49BA-B03B-2EB7288C0058",
			ExternalOrderId: "264552615",
			Resp: JsonNode.Parse("""
			{
				"method": "Withdraw",
				"responeid": "22681727334",
				"respcode": "0000",
				"moneydata": {
					"gold": "189.0",
					"payno": "29CDE229-AE62-49BA-B03B-2EB7288C0058",
					"pay_gold": "10.0",
					"currency": "HKD",
					"pay_way": "-1",
					"recid": "264552615",
					"username": "4uf6k014jp5x"
				},
				"status": "success",
				"timestamp": "1721979994842"
			}
			""")!.ToJsonString());

		var web = new GameInterfaceCRSportsApplication(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeService = Substitute.For<IGetService<FundInQuery, FundResult>>();

			var query = new FundInQuery
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

		var actual = await sut.FundInAsync(apiUrl, agentId, secretKey, agentAccount, agentPassword, playerCode, orderId, amount);

		Assert.NotNull(actual);
		Assert.Equal(result.OrderId, actual.OrderId);
		Assert.Equal(result.ExternalOrderId, actual.ExternalOrderId);
		Assert.NotEmpty(actual.Resp);
	}
}
