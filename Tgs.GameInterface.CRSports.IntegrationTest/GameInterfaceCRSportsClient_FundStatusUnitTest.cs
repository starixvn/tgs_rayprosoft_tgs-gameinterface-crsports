using System.Text.Json.Nodes;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GameInterface.CRSports.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.CRSports.IntegrationTest;

public class GameInterfaceCRSportsClient_FundStatusUnitTest
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

		var web = new GameInterfaceCRSportsApplication(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeService = Substitute.For<IGetService<FundStatusQuery, bool>>();

			var query = new FundStatusQuery
			{
				ApiUrl = apiUrl,
				AgentId = agentId,
				SecretKey = secretKey,
				AgentAccount = agentAccount,
				AgentPassword = agentPassword,
				PlayerCode = playerCode,
				OrderId = orderId
			};

			_ = fakeService.GetAsync(
				query: Arg.Is(query),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

			_ = services.AddTransient(_ => fakeService);
		}));

		var sut = new Client.GameInterfaceCRSportsClient(new Protos.GameInterfaceCRSports.GameInterfaceCRSportsClient(
			GrpcChannel.ForAddress(
				address: web.Server.BaseAddress,
				channelOptions: new GrpcChannelOptions { HttpHandler = web.Server.CreateHandler() })));

		var actual = await sut.FundStatusAsync(apiUrl, agentId, secretKey, agentAccount, agentPassword, playerCode, orderId);

		Assert.True(actual);
	}
}