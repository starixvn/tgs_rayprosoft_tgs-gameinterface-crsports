using System.Net;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using RichardSzalay.MockHttp;
using Tgs.GameInterface.CRSports.Core;

namespace Tgs.GameInterface.CRSports.Core.UnitTest;

public class GameApiClient_ALLWagerUnitTest
{
	[Fact]
	public async Task Betting()
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
		var fakeSystemClock = Substitute.For<ISystemClock>();
		var dateTimeConverter = new DateTimeConverter();
		var fakeTokenUtensil = Substitute.For<ITokenUtensil>();
		var fakeCryptographyUtensil = Substitute.For<ICryptographyUtensil>();

		var apiUrl = new Uri("https://api.orb-6789.com/");
		var agentId = 3238;
		var secretKey = "FnzrUTi5fcsdrws7";
		var agentAccount = "RT706test1";
		var agentPassword = "aaa123";
		var dateStart = new DateTimeOffset(2024, 07, 26, 16, 30, 00, TimeSpan.FromHours(+8));
		var dateEnd = new DateTimeOffset(2024, 07, 26, 17, 00, 00, TimeSpan.FromHours(+8));
		var settle = 0;
		var page = 1;

		var timeProvider = DateTimeOffset.FromUnixTimeMilliseconds(1721985116406);
		_ = fakeSystemClock.UtcNow.Returns(timeProvider);

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
			source: Arg.Is<JsonNode>(content => content["dateStart"]!.GetValue<string>() == "2024-07-26 04:30:00" &&
				content["dateEnd"]!.GetValue<string>() == "2024-07-26 05:00:00" &&
				content["settle"]!.GetValue<int>() == 0 &&
				content["page"]!.GetValue<int>() == 1 &&
				content["token"]!.GetValue<string>() == token &&
				content["timestamp"]!.GetValue<string>() == timeProvider.ToUnixTimeMilliseconds().ToString()),
			Arg.Is(secretKey))
			.Returns(encrypt);
		var responseContent = Guid.NewGuid().ToString("N");
		var handler = new MockHttpMessageHandler();
		handler.Expect(HttpMethod.Post, new Uri(apiUrl, "/app/control_API/agents/api_doaction.php").ToString())
			.WithJsonContent<JsonObject>(content =>
				content["Request"]!.GetValue<string>() == encrypt &&
				content["Method"]!.GetValue<string>() == "ALLWager" &&
				content["AGID"]!.GetValue<string>() == agentId.ToString())
			.Respond(HttpStatusCode.OK, new StringContent(responseContent));
		var httpClient = handler.ToHttpClient();

		var decrypt = new JsonObject
		{
			["agname"] = "RT706test1",
			["wager_cashout"] = new JsonArray { },
			["method"] = "ALLWager",
			["responeid"] = "18837809387",
			["page"] = "1",
			["agid"] = "3238",
			["wager_totalpage"] = "1",
			["respcode"] = "0000",
			["wager_data"] = new JsonArray {
					new JsonObject{
						["oddsFormat"]= "0.5 / 1",
						["strong"]= "C",
						["gtype"]= "FT",
						["degold"]= "2.000",
						["settle"]= "0",
						["mid"]= "28831508",
						["wingold_d"]= "3.90",
						["orderdate"]= "2024-07-26",
						["wingold"]= "4.08",
						["gold"]= "2.000",
						["result"]= "0",
						["rtype"]= "Running Ball-Home",
						["score"]= "0:0",
						["tname_home"]= "Real Murcia",
						["gold_d"]= "1.91",
						["cashoutid"]= "0",
						["odds"]= "H",
						["wtypecode"]= "RE",
						["currency"]= "HKD",
						["degold_d"]= "1.910",
						["id"]= "438050329",
						["adddate"]= "2024-07-26 04:31:51",
						["order"]= "Real Murcia",
						["cashout_d"]= "0.000",
						["tname_away"]= "Millwall",
						["handicap"]= "2",
						["pname"]= "",
						["IP"]= "185.196.0.16",
						["league"]= "Club Friendlies",
						["ioratio"]= "1.04",
						["rtypecode"]= "REH",
						["ordertime"]= "04:30:00",
						["cashout"]= "0.000",
						["report_test"]= "",
						["wtype"]= "Running Ball",
						["resultdetail"]= "0",
						["result_score"]= "",
						["username"]= "4uf6k014jp5x"
					}
				},
			["status"] = "success",
			["timestamp"] = "1721985899034"
		};
		_ = fakeCryptographyUtensil.Decrypt(
			Arg.Is(responseContent),
			Arg.Is(secretKey))
			.Returns(decrypt);

		var fakerLogger = NullLogger<GameApiClient>.Instance;
		var apiClient = new GameApiClient(
			fakerLogger,
			fakePolicySettingsOptions,
			fakeGameInterfaceOptions,
			fakeSystemClock,
			dateTimeConverter,
			fakeTokenUtensil,
			fakeCryptographyUtensil,
			httpClient);

		var actual = await apiClient.ALLWagerAsync(
			apiUrl: apiUrl,
			agentId: agentId,
			secretKey: secretKey,
			agentAccount: agentAccount,
			agentPassword: agentPassword,
			dateStart: dateStart,
			dateEnd: dateEnd,
			settle: settle,
			page: page,
			cancellationToken: default)
			.ToArrayAsync();

		Assert.NotNull(actual);
		Assert.Equal("438050329", actual[0].Id);
		Assert.Equal("4uf6k014jp5x", actual[0].Username);
		Assert.Equal("FT", actual[0].Gtype);
		Assert.Equal(new DateTimeOffset(DateTime.Parse("2024-07-26 04:31:51"), TimeSpan.FromHours(-4)), actual[0].Adddate);
		Assert.Equal(DateTimeOffset.UnixEpoch, actual[0].Resultdate);
		Assert.Equal("0", actual[0].Settle);
		Assert.Equal("0", actual[0].Result);
		Assert.Equal(2.0m, actual[0].Gold);
		Assert.Equal(2.0m, actual[0].MembersVgold);
		Assert.Equal(4.08m, actual[0].Wingold);
		Assert.Equal("0", actual[0].CashoutId);
		Assert.NotEmpty(actual[0].AdditionalData);
	}

	[Fact]
	public async Task Settle()
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
		var fakeSystemClock = Substitute.For<ISystemClock>();
		var dateTimeConverter = new DateTimeConverter();
		var fakeTokenUtensil = Substitute.For<ITokenUtensil>();
		var fakeCryptographyUtensil = Substitute.For<ICryptographyUtensil>();

		var apiUrl = new Uri("https://api.orb-6789.com/");
		var agentId = 3238;
		var secretKey = "FnzrUTi5fcsdrws7";
		var agentAccount = "RT706test1";
		var agentPassword = "aaa123";
		var dateStart = new DateTimeOffset(2024, 07, 26, 18, 00, 00, TimeSpan.FromHours(+8));
		var dateEnd = new DateTimeOffset(2024, 07, 26, 18, 30, 00, TimeSpan.FromHours(+8));
		var settle = 1;
		var page = 1;

		var timeProvider = DateTimeOffset.FromUnixTimeMilliseconds(1722216724217);
		_ = fakeSystemClock.UtcNow.Returns(timeProvider);

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
			source: Arg.Is<JsonNode>(content => content["dateStart"]!.GetValue<string>() == "2024-07-26 06:00:00" &&
				content["dateEnd"]!.GetValue<string>() == "2024-07-26 06:30:00" &&
				content["settle"]!.GetValue<int>() == 1 &&
				content["page"]!.GetValue<int>() == 1 &&
				content["token"]!.GetValue<string>() == token &&
				content["timestamp"]!.GetValue<string>() == timeProvider.ToUnixTimeMilliseconds().ToString()),
			Arg.Is(secretKey))
			.Returns(encrypt);
		var responseContent = Guid.NewGuid().ToString("N");
		var handler = new MockHttpMessageHandler();
		handler.Expect(HttpMethod.Post, new Uri(apiUrl, "/app/control_API/agents/api_doaction.php").ToString())
			.WithJsonContent<JsonObject>(content =>
				content["Request"]!.GetValue<string>() == encrypt &&
				content["Method"]!.GetValue<string>() == "ALLWager" &&
				content["AGID"]!.GetValue<string>() == agentId.ToString())
			.Respond(HttpStatusCode.OK, new StringContent(responseContent));
		var httpClient = handler.ToHttpClient();

		var decrypt = new JsonObject
		{
			["agname"] = "RT706test1",
			["wager_cashout"] = new JsonArray { },
			["method"] = "ALLWager",
			["responeid"] = "22684385659",
			["page"] = "1",
			["agid"] = "3238",
			["wager_totalpage"] = "1",
			["respcode"] = "0000",
			["wager_data"] = new JsonArray {
					new JsonObject{
						["oddsFormat"]= "0.5 / 1",
						["strong"]= "C",
						["gtype"]= "FT",
						["degold"]= "2.000",
						["settle"]= "1",
						["mid"]= "28831508",
						["wingold_d"]= "0.95",
						["orderdate"]= "2024-07-26",
						["wingold"]= "1.00",
						["gold"]= "2.000",
						["result"]= "L",
						["rtype"]= "Running Ball-Home",
						["score"]= "0:0",
						["tname_home"]= "Real Murcia",
						["gold_d"]= "1.91",
						["cashoutid"]= "0",
						["odds"]= "H",
						["wtypecode"]= "RE",
						["currency"]= "HKD",
						["degold_d"]= "1.910",
						["id"]= "438050329",
						["adddate"]= "2024-07-26 04:31:51",
						["order"]= "Real Murcia",
						["cashout_d"]= "0.000",
						["tname_away"]= "Millwall",
						["handicap"]= "2",
						["pname"]= "",
						["IP"]= "185.196.0.16",
						["league"]= "Club Friendlies",
						["ioratio"]= "1.04",
						["rtypecode"]= "REH",
						["members_vgold"]= "1",
						["ordertime"]= "04:30:00",
						["cashout"]= "0.000",
						["report_test"]= "",
						["wtype"]= "Running Ball",
						["resultdetail"]= "LL",
						["vgold"]= "0.96",
						["resultdate"]= "2024-07-26 06:19:25",
						["result_score"]= "1:2",
						["username"]= "4uf6k014jp5x"
					}
				},
			["status"] = "success",
			["timestamp"] = "1722216724217"
		};
		_ = fakeCryptographyUtensil.Decrypt(
			Arg.Is(responseContent),
			Arg.Is(secretKey))
			.Returns(decrypt);

		var fakerLogger = NullLogger<GameApiClient>.Instance;
		var apiClient = new GameApiClient(
			fakerLogger,
			fakePolicySettingsOptions,
			fakeGameInterfaceOptions,
			fakeSystemClock,
			dateTimeConverter,
			fakeTokenUtensil,
			fakeCryptographyUtensil,
			httpClient);

		var actual = await apiClient.ALLWagerAsync(
			apiUrl: apiUrl,
			agentId: agentId,
			secretKey: secretKey,
			agentAccount: agentAccount,
			agentPassword: agentPassword,
			dateStart: dateStart,
			dateEnd: dateEnd,
			settle: settle,
			page: page,
			cancellationToken: default)
			.ToArrayAsync();

		Assert.NotNull(actual);
		Assert.Equal("438050329", actual[0].Id);
		Assert.Equal("4uf6k014jp5x", actual[0].Username);
		Assert.Equal("FT", actual[0].Gtype);
		Assert.Equal(new DateTimeOffset(DateTime.Parse("2024-07-26 04:31:51"), TimeSpan.FromHours(-4)), actual[0].Adddate);
		Assert.Equal(new DateTimeOffset(DateTime.Parse("2024-07-26 06:19:25"), TimeSpan.FromHours(-4)), actual[0].Resultdate);
		Assert.Equal("1", actual[0].Settle);
		Assert.Equal("L", actual[0].Result);
		Assert.Equal(2.0m, actual[0].Gold);
		Assert.Equal(1.0m, actual[0].MembersVgold);
		Assert.Equal(1.00m, actual[0].Wingold);
		Assert.Equal("0", actual[0].CashoutId);
		Assert.NotEmpty(actual[0].AdditionalData);
	}

	[Fact]
	public async Task Can_not_ALLWager_no_data_for_search()
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
		var fakeSystemClock = Substitute.For<ISystemClock>();
		var dateTimeConverter = new DateTimeConverter();
		var fakeTokenUtensil = Substitute.For<ITokenUtensil>();
		var fakeCryptographyUtensil = Substitute.For<ICryptographyUtensil>();

		var apiUrl = new Uri("https://api.orb-6789.com/");
		var agentId = 3238;
		var secretKey = "FnzrUTi5fcsdrws7";
		var agentAccount = "RT706test1";
		var agentPassword = "aaa123";
		var dateStart = new DateTimeOffset(2024, 07, 26, 16, 30, 00, TimeSpan.FromHours(+8));
		var dateEnd = new DateTimeOffset(2024, 07, 26, 17, 00, 00, TimeSpan.FromHours(+8));
		var settle = 1;
		var page = 1;

		var timeProvider = DateTimeOffset.FromUnixTimeMilliseconds(1721985116406);
		_ = fakeSystemClock.UtcNow.Returns(timeProvider);

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
			source: Arg.Is<JsonNode>(content => content["dateStart"]!.GetValue<string>() == "2024-07-26 04:30:00" &&
				content["dateEnd"]!.GetValue<string>() == "2024-07-26 05:00:00" &&
				content["settle"]!.GetValue<int>() == 1 &&
				content["page"]!.GetValue<int>() == 1 &&
				content["token"]!.GetValue<string>() == token &&
				content["timestamp"]!.GetValue<string>() == timeProvider.ToUnixTimeMilliseconds().ToString()),
			Arg.Is(secretKey))
			.Returns(encrypt);
		var responseContent = Guid.NewGuid().ToString("N");
		var handler = new MockHttpMessageHandler();
		handler.Expect(HttpMethod.Post, new Uri(apiUrl, "/app/control_API/agents/api_doaction.php").ToString())
			.WithJsonContent<JsonObject>(content =>
				content["Request"]!.GetValue<string>() == encrypt &&
				content["Method"]!.GetValue<string>() == "ALLWager" &&
				content["AGID"]!.GetValue<string>() == agentId.ToString())
			.Respond(HttpStatusCode.OK, new StringContent(responseContent));
		var httpClient = handler.ToHttpClient();

		var decrypt = new JsonObject
		{
			["Error"] = "can not ALLWager: no data for search",
			["responeid"] = "20837820712",
			["respcode"] = "0003",
			["timestamp"] = "1721985117325"
		};
		_ = fakeCryptographyUtensil.Decrypt(
			Arg.Is(responseContent),
			Arg.Is(secretKey))
			.Returns(decrypt);

		var fakerLogger = NullLogger<GameApiClient>.Instance;
		var apiClient = new GameApiClient(
			fakerLogger,
			fakePolicySettingsOptions,
			fakeGameInterfaceOptions,
			fakeSystemClock,
			dateTimeConverter,
			fakeTokenUtensil,
			fakeCryptographyUtensil,
			httpClient);

		var actual = await apiClient.ALLWagerAsync(
			apiUrl: apiUrl,
			agentId: agentId,
			secretKey: secretKey,
			agentAccount: agentAccount,
			agentPassword: agentPassword,
			dateStart: dateStart,
			dateEnd: dateEnd,
			settle: settle,
			page: page,
			cancellationToken: default)
			.ToArrayAsync();

		Assert.Empty(actual);
	}
}
