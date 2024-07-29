using System.Net;
using System.Text.Json.Nodes;
using NSubstitute;
using Tgs.GameInterface.CRSports.Core.ApplicationServices;
using Tgs.GameInterface.CRSports.Core.Models;

namespace Tgs.GameInterface.CRSports.Core.UnitTest.ApplicationServices;
public class GameUrlGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameApiClient = Substitute.For<IGameApiClient>();

		var query = new GameUrlQuery
		{
			ApiUrl = new Uri("https://api.orb-6789.com/"),
			AgentId = 3238,
			SecretKey = "FnzrUTi5fcsdrws7",
			AgentAccount = "RT706test1",
			AgentPassword = "aaa123",
			PlayerCode = "4uf6k014jp5x",
			Currency = "HKD",
			IsMobile = false,
			Language = "zh-tw",
			IpAddress = "127.0.0.1"
		};

		var resp = new LaunchGameResp(
			IsPlayerExist: true,
			GameUrl: "http://axm375.rake23678.com/");

		_ = fakeGameApiClient.LaunchGameAsync(
			apiUrl: Arg.Is(query.ApiUrl),
			agentId: Arg.Is(query.AgentId),
			secretKey: Arg.Is(query.SecretKey),
			agentAccount: Arg.Is(query.AgentAccount),
			agentPassword: Arg.Is(query.AgentPassword),
			memname: Arg.Is(query.PlayerCode),
			currency: Arg.Is(query.Currency),
			machine: Arg.Is("PC"),
			langx: Arg.Is(query.Language),
			remoteip: Arg.Is(query.IpAddress),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(resp);

		var sut = new GameUrlGetService(fakeGameApiClient);

		var actual = await sut.GetAsync(
			query: query,
			cancellationToken: default);

		Assert.NotNull(actual);
		Assert.True(actual.IsPlayerExist);
		Assert.Equal("http://axm375.rake23678.com/", actual.GameUrl);
	}
}
