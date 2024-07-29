using System.Text.Json.Nodes;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GameInterface.CRSports.Core.ApplicationServices;
using Tgs.GameInterface.CRSports.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.CRSports.IntegrationTest;

public class GameInterfaceCRSportsClient_PlayerCreateUnitTest
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
		var currency = "HKD";

		var fakeService = Substitute.For<ICommandService<PlayerCreateCommand>>();

		var web = new GameInterfaceCRSportsApplication(builder => _ = builder.ConfigureServices(services => _ = services.AddTransient(_ => fakeService)));

		var sut = new Client.GameInterfaceCRSportsClient(new Protos.GameInterfaceCRSports.GameInterfaceCRSportsClient(
			GrpcChannel.ForAddress(
				address: web.Server.BaseAddress,
				channelOptions: new GrpcChannelOptions { HttpHandler = web.Server.CreateHandler() })));

		await sut.PlayerCreateAsync(apiUrl, agentId, secretKey, agentAccount, agentPassword, playerCode, currency);

		_ = fakeService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Is(new PlayerCreateCommand
			{
				ApiUrl = apiUrl,
				AgentId = agentId,
				SecretKey = secretKey,
				AgentAccount = agentAccount,
				AgentPassword = agentPassword,
				PlayerCode = playerCode,
				Currency = currency
			}),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}