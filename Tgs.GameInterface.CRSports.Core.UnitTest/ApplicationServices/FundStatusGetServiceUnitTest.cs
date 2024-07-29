using System.Text.Json.Nodes;
using NSubstitute;
using Tgs.GameInterface.CRSports.Core.ApplicationServices;
using Tgs.GameInterface.CRSports.Core.Models;

namespace Tgs.GameInterface.CRSports.Core.UnitTest.ApplicationServices;

public class FundStatusGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameApiClient = Substitute.For<IGameApiClient>();

		var query = new FundStatusQuery
		{
			ApiUrl = new Uri("https://api.orb-6789.com/"),
			AgentId = 3238,
			SecretKey = "FnzrUTi5fcsdrws7",
			AgentAccount = "RT706test1",
			AgentPassword = "aaa123",
			PlayerCode = "4uf6k014jp5x",
			OrderId = "E366D434-F19E-4706-8FB9-96DFB21F2EF5"
		};

		_ = fakeGameApiClient.ChkTransInfoAsync(
			apiUrl: Arg.Is(query.ApiUrl),
			agentId: Arg.Is(query.AgentId),
			secretKey: Arg.Is(query.SecretKey),
			agentAccount: Arg.Is(query.AgentAccount),
			agentPassword: Arg.Is(query.AgentPassword),
			memname: Arg.Is(query.PlayerCode),
			payno: Arg.Is(query.OrderId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

		var sut = new FundStatusGetService(fakeGameApiClient);

		var actual = await sut.GetAsync(
			query: query,
			cancellationToken: default);

		Assert.True(actual);
	}
}
