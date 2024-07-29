using System.Net;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using RichardSzalay.MockHttp;
using Tgs.GameInterface.CRSports.Core;

namespace Tgs.GameInterface.CRSports.Core.UnitTest;

public class GameApiClient_DepositUnitTest
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
		var payno = "E366D434-F19E-4706-8FB9-96DFB21F2EF5";
		var amount = 99.00m;

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
				content["payno"]!.GetValue<string>() == payno &&
				content["amount"]!.GetValue<decimal>() == amount &&
				content["token"]!.GetValue<string>() == token &&
				content["timestamp"]!.GetValue<string>() == timeProvider.ToUnixTimeMilliseconds().ToString()),
			Arg.Is(secretKey))
			.Returns(encrypt);
		var responseContent = Guid.NewGuid().ToString("N");
		var handler = new MockHttpMessageHandler();
		handler.Expect(HttpMethod.Post, new Uri(apiUrl, "/app/control_API/agents/api_doaction.php").ToString())
			.WithJsonContent<JsonObject>(content =>
				content["Request"]!.GetValue<string>() == encrypt &&
				content["Method"]!.GetValue<string>() == "Deposit" &&
				content["AGID"]!.GetValue<string>() == agentId.ToString())
			.Respond(HttpStatusCode.OK, new StringContent(responseContent));
		var httpClient = handler.ToHttpClient();

		var decrypt = JsonNode.Parse("""
			{
				"method": "Deposit",
				"responeid": "23681717965",
				"respcode": "0000",
				"moneydata": {
					"gold": "199.0",
					"payno": "E366D434-F19E-4706-8FB9-96DFB21F2EF5",
					"pay_gold": "99.0",
					"currency": "HKD",
					"pay_way": "1",
					"recid": "264549283",
					"username": "4uf6k014jp5x"
				},
				"status": "success",
				"timestamp": "1721978380338"
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

		var actual = await apiClient.DepositAsync(
			apiUrl: apiUrl,
			agentId: agentId,
			secretKey: secretKey,
			agentAccount: agentAccount,
			agentPassword: agentPassword,
			memname: memname,
			payno: payno,
			amount: amount,
			cancellationToken: default)
			.ConfigureAwait(true);

		Assert.NotNull(actual);
		Assert.Equal(199.0m, actual.Gold);
		Assert.Equal(payno, actual.Payno);
		Assert.Equal("264549283", actual.Recid);
		Assert.NotEmpty(actual.Resp);
	}
}
