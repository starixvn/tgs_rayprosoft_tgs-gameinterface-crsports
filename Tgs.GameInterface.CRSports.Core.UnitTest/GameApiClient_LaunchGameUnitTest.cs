using System.Net;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using RichardSzalay.MockHttp;
using Tgs.GameInterface.CRSports.Core;

namespace Tgs.GameInterface.CRSports.Core.UnitTest;

public class GameApiClient_LaunchGameUnitTest
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
		var memname = "4uf6k014jp5x";
		var currency = "HKD";
		var machine = "MOBILE";
		var langx = "zh-tw";
		var remoteip = "127.0.0.1";

		var timeProvider = DateTimeOffset.FromUnixTimeMilliseconds(1721976666753);
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
			source: Arg.Is<JsonNode>(content => content["memname"]!.GetValue<string>() == memname &&
				content["currency"]!.GetValue<string>() == currency &&
				content["machine"]!.GetValue<string>() == machine &&
				content["langx"]!.GetValue<string>() == langx &&
				content["langx"]!.GetValue<string>() == langx &&
				content["remoteip"]!.GetValue<string>() == remoteip &&
				content["token"]!.GetValue<string>() == token &&
				content["timestamp"]!.GetValue<string>() == timeProvider.ToUnixTimeMilliseconds().ToString()),
			Arg.Is(secretKey))
			.Returns(encrypt);
		var responseContent = Guid.NewGuid().ToString("N");
		var handler = new MockHttpMessageHandler();
		handler.Expect(HttpMethod.Post, new Uri(apiUrl, "/app/control_API/agents/api_doaction.php").ToString())
			.WithJsonContent<JsonObject>(content =>
				content["Request"]!.GetValue<string>() == encrypt &&
				content["Method"]!.GetValue<string>() == "LaunchGame" &&
				content["AGID"]!.GetValue<string>() == agentId.ToString())
			.Respond(HttpStatusCode.OK, new StringContent(responseContent));
		var httpClient = handler.ToHttpClient();

		var decrypt = JsonNode.Parse("""
			{
				"memname": "4uf6k014jp5x",
				"method": "LaunchGame",
				"launchgameurl": "http://exm428.rake23678.com/app/member_api/FT_index.php?mtype=3\\u0026uid=dh4ug1nm28831508l987369\\u0026langx=zh-tw\\u0026news_mem=Y\\u0026news_api=Y\\u0026isSSL=N",
				"responeid": "23681698801",
				"respcode": "0000",
				"memToken": "dh4ug1nm28831508l987369",
				"status": "success",
				"timestamp": "1721976667716"
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

		var actual = await apiClient.LaunchGameAsync(
			apiUrl: apiUrl,
			agentId: agentId,
			secretKey: secretKey,
			agentAccount: agentAccount,
			agentPassword: agentPassword,
			memname: memname,
			currency: currency,
			machine: machine,
			langx: langx,
			remoteip: remoteip,
			cancellationToken: default)
			.ConfigureAwait(true);

		Assert.NotNull(actual);
		Assert.True(actual.IsPlayerExist);
		Assert.NotEmpty(actual.GameUrl);
	}
}
