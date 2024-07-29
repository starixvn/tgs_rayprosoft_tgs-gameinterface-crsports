using NSubstitute;
using Tgs.GameInterface.CRSports.Core.ApplicationServices;

namespace Tgs.GameInterface.CRSports.Core.UnitTest.ApplicationServices;

public class AGLoginGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameApiClient = Substitute.For<IGameApiClient>();

		var query = new AGLoginQuery
		{
			ApiUrl = new Uri("https://api.orb-6789.com/"),
			AgentId = 3238,
			SecretKey = "FnzrUTi5fcsdrws7",
			AgentAccount = "RT706test1",
			AgentPassword = "aaa123"
		};

		var token = Guid.NewGuid().ToString();

		_ = fakeGameApiClient.AGLoginAsync(
			apiUrl: Arg.Is(query.ApiUrl),
			agentId: Arg.Is(query.AgentId),
			secretKey: Arg.Is(query.SecretKey),
			agentAccount: Arg.Is(query.AgentAccount),
			agentPassword: Arg.Is(query.AgentPassword),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(token);

		var sut = new AGLoginGetService(fakeGameApiClient);

		var actual = await sut.GetAsync(
			query: query,
			cancellationToken: default);

		Assert.NotNull(actual);
		Assert.Equal(token, actual);
	}
}
