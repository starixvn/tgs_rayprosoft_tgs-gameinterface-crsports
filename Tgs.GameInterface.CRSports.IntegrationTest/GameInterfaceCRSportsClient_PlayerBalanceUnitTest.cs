using System.Text.Json.Nodes;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GameInterface.CRSports.Core.ApplicationServices;
using Tgs.GameInterface.CRSports.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.CRSports.IntegrationTest;

public class GameInterfaceCRSportsClient_PlayerBalanceUnitTest
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

		var result = new PlayerBalanceResult(
			IsPlayerExist: true,
			Balance: 100m);

		var web = new GameInterfaceCRSportsApplication(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeService = Substitute.For<IGetService<PlayerBalanceQuery, PlayerBalanceResult>>();

			var query = new PlayerBalanceQuery
			{
				ApiUrl = apiUrl,
				AgentId = agentId,
				SecretKey = secretKey,
				AgentAccount = agentAccount,
				AgentPassword = agentPassword,
				PlayerCode = playerCode
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

		var actual = await sut.PlayerBalanceAsync(apiUrl, agentId, secretKey, agentAccount, agentPassword, playerCode);

		Assert.NotNull(actual);
		Assert.True(actual.IsPlayerExist);
		Assert.Equal(100.0m, actual.Balance.ToDecimal().GetValueOrDefault(0.0m));
	}
}