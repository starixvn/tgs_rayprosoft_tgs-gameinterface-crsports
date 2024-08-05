using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Authentication;

using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using RichardSzalay.MockHttp;
using Tgs.GameInterface.CRSports.Core.Exceptions;

namespace Tgs.GameInterface.CRSports.Core.UnitTest;

public class AGLoginApiClient_AGLoginUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeSystemClock = Substitute.For<ISystemClock>();
		var fakeCryptographyUtensil = Substitute.For<ICryptographyUtensil>();

		var apiUrl = new Uri("https://api.orb-6789.com/");
		var agentId = 3238;
		var secretKey = "FnzrUTi5fcsdrws7";
		var agentAccount = "RT706test1";
		var agentPassword = "aaa123";

		var timeProvider = DateTimeOffset.FromUnixTimeMilliseconds(1721971001210);
		_ = fakeSystemClock.UtcNow.Returns(timeProvider);

		var encrypt = Guid.NewGuid().ToString("N");
		_ = fakeCryptographyUtensil.Encrypt(
			source: Arg.Is<JsonNode>(content => content["username"]!.GetValue<string>() == agentAccount &&
				content["password"]!.GetValue<string>() == agentPassword &&
				content["timestamp"]!.GetValue<string>() == timeProvider.ToUnixTimeMilliseconds().ToString()),
			Arg.Is(secretKey))
			.Returns(encrypt);
		var responseContent = Guid.NewGuid().ToString("N");
		var handler = new MockHttpMessageHandler();
		handler.Expect(HttpMethod.Post, new Uri(apiUrl, "/app/control_API/agents/api_doaction.php").ToString())
			.WithJsonContent<JsonObject>(content =>
				content["Request"]!.GetValue<string>() == encrypt &&
				content["Method"]!.GetValue<string>() == "AGLogin" &&
				content["AGID"]!.GetValue<string>() == agentId.ToString())
			.Respond(HttpStatusCode.OK, new StringContent(responseContent));
		var httpClient = handler.ToHttpClient();

		var decrypt = new JsonObject
		{
			["method"] = "AGLogin",
			["responeid"] = "18837756570",
			["respcode"] = "0000",
			["aid"] = "3238",
			["username"] = "RT706test1",
			["token"] = "2bb12b1am1bdeld343a2xw",
			["status"] = "success",
			["timestamp"] = "1721981249639"
		};
		_ = fakeCryptographyUtensil.Decrypt(
			Arg.Is(responseContent),
			Arg.Is(secretKey))
			.Returns(decrypt);

		var fakerLogger = NullLogger<AGLoginApiClient>.Instance;
		var apiClient = new AGLoginApiClient(
			fakerLogger,
			fakeSystemClock,
			fakeCryptographyUtensil,
			httpClient);

		var actual = await apiClient.AGLoginAsync(
			 apiUrl: apiUrl,
			 agentId: agentId,
			 secretKey: secretKey,
			 agentAccount: agentAccount,
			 agentPassword: agentPassword,
			 cancellationToken: default)
			.ConfigureAwait(true);

		Assert.NotNull(actual);
		Assert.Equal("2bb12b1am1bdeld343a2xw", actual);
	}

	[Fact]
	public async Task We_are_currently_undergoing_system_maintenance()
	{
		var fakeSystemClock = Substitute.For<ISystemClock>();
		var fakeCryptographyUtensil = Substitute.For<ICryptographyUtensil>();

		var apiUrl = new Uri("https://api.orb-6789.com/");
		var agentId = 3238;
		var secretKey = "FnzrUTi5fcsdrws7";
		var agentAccount = "RT706test1";
		var agentPassword = "aaa123";

		var timeProvider = DateTimeOffset.FromUnixTimeMilliseconds(1721974886257);
		_ = fakeSystemClock.UtcNow.Returns(timeProvider);

		var encrypt = Guid.NewGuid().ToString("N");
		_ = fakeCryptographyUtensil.Encrypt(
			source: Arg.Is<JsonNode>(content => content["username"]!.GetValue<string>() == agentAccount &&
				content["password"]!.GetValue<string>() == agentPassword &&
				content["timestamp"]!.GetValue<string>() == timeProvider.ToUnixTimeMilliseconds().ToString()),
			Arg.Is(secretKey))
			.Returns(encrypt);
		var responseContent = Guid.NewGuid().ToString("N");
		var handler = new MockHttpMessageHandler();
		handler.Expect(HttpMethod.Post, new Uri(apiUrl, "/app/control_API/agents/api_doaction.php").ToString())
			.WithJsonContent<JsonObject>(content =>
				content["Request"]!.GetValue<string>() == encrypt &&
				content["Method"]!.GetValue<string>() == "AGLogin" &&
				content["AGID"]!.GetValue<string>() == agentId.ToString())
			.Respond(HttpStatusCode.OK, JsonContent.Create(new JsonObject
			{
				["Error"] = "We are currently undergoing system maintenance.",
				["respcode"] = "0016",
				["timestamp"] = "1721974886257"
			}));
		var httpClient = handler.ToHttpClient();

		var fakerLogger = NullLogger<AGLoginApiClient>.Instance;
		var apiClient = new AGLoginApiClient(
			fakerLogger,
			fakeSystemClock,
			fakeCryptographyUtensil,
			httpClient);

		_ = await Assert.ThrowsAsync<GameInterfaceException>(
			() => apiClient.AGLoginAsync(
				apiUrl: apiUrl,
				agentId: agentId,
				secretKey: secretKey,
				agentAccount: agentAccount,
				agentPassword: agentPassword,
				cancellationToken: default)
			.AsTask())
			.ConfigureAwait(true);
	}
}
