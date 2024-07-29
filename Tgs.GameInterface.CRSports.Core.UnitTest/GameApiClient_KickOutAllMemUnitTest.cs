using System.Net;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using RichardSzalay.MockHttp;
using Tgs.GameInterface.CRSports.Core;

namespace Tgs.GameInterface.CRSports.Core.UnitTest;

public class GameApiClient_KickOutAllMemUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakePolicySettingsOptions = Options.Create(new PolicySettingsOptions
		{
			GlobalRetryCount = 1,
			GlobalRetryWaitSec = 1,
		});
		var fakeGameInterfaceOptions = Options.Create(new GameInterfaceSettingsOptions
		{
			PlayerPassword = "TgsPassw0rd"
		});
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var dateTimeConverter = new DateTimeConverter();
		var fakeTokenUtensil = Substitute.For<ITokenUtensil>();
		var fakeCryptographyUtensil = Substitute.For<ICryptographyUtensil>();

		var apiUrl = new Uri("https://api.orb-6789.com/");
		var agentId = 3238;
		var secretKey = "FnzrUTi5fcsdrws7";
		var agentAccount = "RT706test1";
		var agentPassword = "aaa123";

		var timeProvider = DateTimeOffset.FromUnixTimeMilliseconds(1721971001210);
		_ = fakeTimeProvider.GetUtcNow().Returns(timeProvider);

		var token = Guid.NewGuid().ToString("N");
		_ = fakeTokenUtensil.GetTokenAsync(
			apiUrl: Arg.Is(apiUrl),
			agentId: Arg.Is(agentId),
			secretKey: Arg.Is(secretKey),
			agentAccount: Arg.Is(agentAccount),
			agentPassword: Arg.Is(agentPassword),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(token);

		var encrypt = Guid.NewGuid().ToString("N");
		_ = fakeCryptographyUtensil.Encrypt(
			source: Arg.Is<JsonNode>(content => content["token"]!.GetValue<string>() == token &&
				content["timestamp"]!.GetValue<string>() == timeProvider.ToUnixTimeMilliseconds().ToString()),
			Arg.Is(secretKey))
			.Returns(encrypt);
		var responseContent = Guid.NewGuid().ToString("N");
		var handler = new MockHttpMessageHandler();
		handler.Expect(HttpMethod.Post, new Uri(apiUrl, "/app/control_API/agents/api_doaction.php").ToString())
			.WithJsonContent<JsonObject>(content =>
				content["Request"]!.GetValue<string>() == encrypt &&
				content["Method"]!.GetValue<string>() == "KickOutAllMem" &&
				content["AGID"]!.GetValue<string>() == agentId.ToString())
			.Respond(HttpStatusCode.OK, new StringContent(responseContent));
		var httpClient = handler.ToHttpClient();

		var decrypt = JsonNode.Parse("""
			{
				"Error": "success",
				"responeid": "20837684617",
				"respcode": "0000",
				"timestamp": "1721971001595"
			}
			""");
		_ = fakeCryptographyUtensil.Decrypt(
			Arg.Is(responseContent),
			Arg.Is(secretKey))
			.Returns(decrypt);

		var fakerLogger = NullLogger<GameApiClient>.Instance;
		var apiClient = new GameApiClient(
			fakerLogger,
			fakePolicySettingsOptions,
			fakeGameInterfaceOptions,
			fakeTimeProvider,
			dateTimeConverter,
			fakeTokenUtensil,
			fakeCryptographyUtensil,
			httpClient);

		await apiClient.KickOutAllMemAsync(
			 apiUrl: apiUrl,
			 agentId: agentId,
			 secretKey: secretKey,
			 agentAccount: agentAccount,
			 agentPassword: agentPassword,
			 cancellationToken: default)
			.ConfigureAwait(true);
	}
}
