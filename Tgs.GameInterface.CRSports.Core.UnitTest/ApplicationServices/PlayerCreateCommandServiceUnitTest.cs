using System.Net;
using System.Text.Json.Nodes;
using NSubstitute;
using Tgs.GameInterface.CRSports.Core.ApplicationServices;
using Tgs.GameInterface.CRSports.Core.Models;

namespace Tgs.GameInterface.CRSports.Core.UnitTest.ApplicationServices;
public class PlayerCreateCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameApiClient = Substitute.For<IGameApiClient>();

		var command = new PlayerCreateCommand
		{
			ApiUrl = new Uri("https://api.orb-6789.com/"),
			AgentId = 3238,
			SecretKey = "FnzrUTi5fcsdrws7",
			AgentAccount = "RT706test1",
			AgentPassword = "aaa123",
			PlayerCode = "4uf6k014jp5x",
			Currency = "HKD"
		};

		var sut = new PlayerCreateCommandService(fakeGameApiClient);

		await sut.ExecuteAsync(
		   command: command,
		   cancellationToken: default);

		_ = fakeGameApiClient
			.Received(1)
			.CreateMemberAsync(
			apiUrl: Arg.Is(command.ApiUrl),
			agentId: Arg.Is(command.AgentId),
			secretKey: Arg.Is(command.SecretKey),
			agentAccount: Arg.Is(command.AgentAccount),
			agentPassword: Arg.Is(command.AgentPassword),
			memname: Arg.Is(command.PlayerCode),
			currency: Arg.Is(command.Currency),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
