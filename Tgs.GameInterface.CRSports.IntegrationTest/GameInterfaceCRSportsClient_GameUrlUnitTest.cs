using System.Text.Json.Nodes;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.GameInterface.CRSports.Core.ApplicationServices;
using Tgs.GameInterface.CRSports.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.CRSports.IntegrationTest;

public class GameInterfaceCRSportsClient_GameUrlUnitTest
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
		var isMobile = false;
		var language = "zh-tw";
		var ipAddress = "127.0.0.1";

		var result = new GameUrlResult(
			IsPlayerExist: true,
			GameUrl: "http://axm375.rake23678.com/");

		var web = new GameInterfaceCRSportsApplication(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeService = Substitute.For<IGetService<GameUrlQuery, GameUrlResult>>();

			var query = new GameUrlQuery
			{
				ApiUrl = apiUrl,
				AgentId = agentId,
				SecretKey = secretKey,
				AgentAccount = agentAccount,
				AgentPassword = agentPassword,
				PlayerCode = playerCode,
				Currency = currency,
				IsMobile = isMobile,
				Language = language,
				IpAddress = ipAddress
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

		var actual = await sut.GameUrlAsync(apiUrl, agentId, secretKey, agentAccount, agentPassword, playerCode, currency, language, isMobile, ipAddress);

		Assert.NotNull(actual);
		Assert.True(actual.IsPlayerExist);
		Assert.Equal("http://axm375.rake23678.com/", actual.GameUrl);
	}
}