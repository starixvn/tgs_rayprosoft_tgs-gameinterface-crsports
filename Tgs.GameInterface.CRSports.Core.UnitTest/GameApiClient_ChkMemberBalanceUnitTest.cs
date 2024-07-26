using System.Net;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using RichardSzalay.MockHttp;
using Tgs.GameInterface.GlobalTechnology.Core;

namespace Tgs.GameInterface.CRSports.Core.UnitTest;

public class GameApiClient_ChkMemberBalanceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakePolicySettingsOptions = Options.Create(new PolicySettingsOptions
		{
			GlobalRetryCount = 1,
			GlobalRetryWaitSec = 1,
		});
		var fakeGameInterfaceOptions = Options.Create(new GameInterfaceOptions
		{
			PlayerPassword = "TgsPassw0rd"
		});
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var dateTimeConverter = new DateTimeConverter();
		var fakeTokenUtensil = Substitute.For<ITokenUtensil>();
		var fakeCryptographyUtensil = Substitute.For<ICryptographyUtensil>();

		var apiBaseUrl = new Uri("https://api.orb-6789.com/");
		var agentId = 3238;
		var secretKey = "FnzrUTi5fcsdrws7";
		var agentAccount = "RT706test1";
		var agentPassword = "aaa123";
		var memname = "4uf6k014jp5x";

		var timeProvider = DateTimeOffset.FromUnixTimeMilliseconds(1721971001210);
		_ = fakeTimeProvider.GetUtcNow().Returns(timeProvider);

		var token = Guid.NewGuid().ToString("N");
		_ = fakeTokenUtensil.GetTokenAsync(
			apiBaseUrl: Arg.Is(apiBaseUrl),
			agentId: Arg.Is(agentId),
			secretKey: Arg.Is(secretKey),
			agentAccount: Arg.Is(agentAccount),
			agentPassword: Arg.Is(agentPassword),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(token);

		var encrypt = Guid.NewGuid().ToString("N");
		_ = fakeCryptographyUtensil.Encrypt(
			source: Arg.Is<JsonNode>(content => content["memname"]!.GetValue<string>() == memname &&
				content["token"]!.GetValue<string>() == token &&
				content["timestamp"]!.GetValue<string>() == timeProvider.ToUnixTimeMilliseconds().ToString()),
			Arg.Is(secretKey))
			.Returns(encrypt);
		var responseContent = Guid.NewGuid().ToString("N");
		var handler = new MockHttpMessageHandler();
		handler.Expect(HttpMethod.Post, new Uri(apiBaseUrl, "/app/control_API/agents/api_doaction.php").ToString())
			.WithJsonContent<JsonObject>(content =>
				content["Request"]!.GetValue<string>() == encrypt &&
				content["Method"]!.GetValue<string>() == "chkMemberBalance" &&
				content["AGID"]!.GetValue<string>() == agentId.ToString())
			.Respond(HttpStatusCode.OK, new StringContent(responseContent));
		var httpClient = handler.ToHttpClient();

		var Decrypt = JsonNode.Parse("""
			{
				"memname": "4uf6k014jp5x",
				"balance": "0.00",
				"method": "chkMemberBalance",
				"currency": "HKD",
				"responeid": "17837574323",
				"respcode": "0000",
				"status": "success",
				"timestamp": "1721976098451"
			}
			""");
		_ = fakeCryptographyUtensil.Decrypt(
			Arg.Is(responseContent),
			Arg.Is(secretKey))
			.Returns(Decrypt);

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

		var actual = await apiClient.ChkMemberBalanceAsync(
			apiBaseUrl: apiBaseUrl,
			agentId: agentId,
			secretKey: secretKey,
			agentAccount: agentAccount,
			agentPassword: agentPassword,
			memname: memname,
			cancellationToken: default)
			.ConfigureAwait(true);

		Assert.NotNull(actual);
		Assert.True(actual.IsPlayerExist);
		Assert.Equal(0.0m, actual.Balance);
	}

	[Fact]
	public async Task No_data_found()
	{
		var fakePolicySettingsOptions = Options.Create(new PolicySettingsOptions
		{
			GlobalRetryCount = 1,
			GlobalRetryWaitSec = 1,
		});
		var fakeGameInterfaceOptions = Options.Create(new GameInterfaceOptions
		{
			PlayerPassword = "TgsPassw0rd"
		});
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var dateTimeConverter = new DateTimeConverter();
		var fakeTokenUtensil = Substitute.For<ITokenUtensil>();
		var fakeCryptographyUtensil = Substitute.For<ICryptographyUtensil>();

		var apiBaseUrl = new Uri("https://api.orb-6789.com/");
		var agentId = 3238;
		var secretKey = "FnzrUTi5fcsdrws7";
		var agentAccount = "RT706test1";
		var agentPassword = "aaa123";
		var memname = "4uf6k014jp5x";

		var timeProvider = DateTimeOffset.FromUnixTimeMilliseconds(1721971001210);
		_ = fakeTimeProvider.GetUtcNow().Returns(timeProvider);

		var token = Guid.NewGuid().ToString("N");
		_ = fakeTokenUtensil.GetTokenAsync(
			apiBaseUrl: Arg.Is(apiBaseUrl),
			agentId: Arg.Is(agentId),
			secretKey: Arg.Is(secretKey),
			agentAccount: Arg.Is(agentAccount),
			agentPassword: Arg.Is(agentPassword),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(token);

		var encrypt = Guid.NewGuid().ToString("N");
		_ = fakeCryptographyUtensil.Encrypt(
			source: Arg.Is<JsonNode>(content => content["memname"]!.GetValue<string>() == memname &&
				content["token"]!.GetValue<string>() == token &&
				content["timestamp"]!.GetValue<string>() == timeProvider.ToUnixTimeMilliseconds().ToString()),
			Arg.Is(secretKey))
			.Returns(encrypt);
		var responseContent = Guid.NewGuid().ToString("N");
		var handler = new MockHttpMessageHandler();
		handler.Expect(HttpMethod.Post, new Uri(apiBaseUrl, "/app/control_API/agents/api_doaction.php").ToString())
			.WithJsonContent<JsonObject>(content =>
				content["Request"]!.GetValue<string>() == encrypt &&
				content["Method"]!.GetValue<string>() == "chkMemberBalance" &&
				content["AGID"]!.GetValue<string>() == agentId.ToString())
			.Respond(HttpStatusCode.OK, new StringContent(responseContent));
		var httpClient = handler.ToHttpClient();

		var Decrypt = JsonNode.Parse("""
			{
				"Error": "No data found",
				"responeid": "21681597833",
				"respcode": "0003",
				"timestamp": "1721965680624"
			}
			""");
		_ = fakeCryptographyUtensil.Decrypt(
			Arg.Is(responseContent),
			Arg.Is(secretKey))
			.Returns(Decrypt);

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

		var actual = await apiClient.ChkMemberBalanceAsync(
			apiBaseUrl: apiBaseUrl,
			agentId: agentId,
			secretKey: secretKey,
			agentAccount: agentAccount,
			agentPassword: agentPassword,
			memname: memname,
			cancellationToken: default)
			.ConfigureAwait(true);

		Assert.NotNull(actual);
		Assert.False(actual.IsPlayerExist);
		Assert.Equal(0.0m, actual.Balance);
	}
}
