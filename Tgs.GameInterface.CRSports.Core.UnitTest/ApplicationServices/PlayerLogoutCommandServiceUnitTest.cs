using System.Net;
using System.Text.Json.Nodes;
using NSubstitute;
using Tgs.GameInterface.CRSports.Core.ApplicationServices;
using Tgs.GameInterface.CRSports.Core.Models;

namespace Tgs.GameInterface.CRSports.Core.UnitTest.ApplicationServices;

public class PlayerLogoutCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameApiClient = Substitute.For<IGameApiClient>();

		var command = new PlayerLogoutCommand
		{
			ApiUrl = new Uri("https://api.orb-6789.com/"),
			AgentId = 3238,
			SecretKey = "FnzrUTi5fcsdrws7",
			AgentAccount = "RT706test1",
			AgentPassword = "aaa123",
			PlayerCode = "4uf6k014jp5x"
		};

		var sut = new PlayerLogoutCommandService(fakeGameApiClient);

		await sut.ExecuteAsync(
		   command: command,
		   cancellationToken: default);

		_ = fakeGameApiClient
			.Received(1)
			.KickOutMemAsync(
			apiUrl: Arg.Is(command.ApiUrl),
			agentId: Arg.Is(command.AgentId),
			secretKey: Arg.Is(command.SecretKey),
			agentAccount: Arg.Is(command.AgentAccount),
			agentPassword: Arg.Is(command.AgentPassword),
			memname: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
