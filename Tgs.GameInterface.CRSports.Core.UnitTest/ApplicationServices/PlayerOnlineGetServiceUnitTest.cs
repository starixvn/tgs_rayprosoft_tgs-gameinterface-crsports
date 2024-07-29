using NSubstitute;
using Tgs.GameInterface.CRSports.Core.ApplicationServices;
using Tgs.GameInterface.CRSports.Core.Models;

namespace Tgs.GameInterface.CRSports.Core.UnitTest.ApplicationServices;

public class PlayerOnlineGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameApiClient = Substitute.For<IGameApiClient>();

		var query = new PlayerOnlineQuery
		{
			ApiUrl = new Uri("https://api.orb-6789.com/"),
			AgentId = 3238,
			SecretKey = "FnzrUTi5fcsdrws7",
			AgentAccount = "RT706test1",
			AgentPassword = "aaa123",
			PlayerCode = "4uf6k014jp5x"
		};

		var resp = new MemOnlineResp(
			UserUsername: "4uf6k014jp5x",
			UserCurrency: "HKD",
			UserGold: 99.0m);

		_ = fakeGameApiClient.GetMemOnlineListAsync(
			apiUrl: Arg.Is(query.ApiUrl),
			agentId: Arg.Is(query.AgentId),
			secretKey: Arg.Is(query.SecretKey),
			agentAccount: Arg.Is(query.AgentAccount),
			agentPassword: Arg.Is(query.AgentPassword),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(new[] { resp }.ToAsyncEnumerable());

		var sut = new PlayerOnlineGetService(fakeGameApiClient);

		var actual = await sut.GetAsync(
			query: query,
			cancellationToken: default);

		Assert.True(actual);
	}
}
