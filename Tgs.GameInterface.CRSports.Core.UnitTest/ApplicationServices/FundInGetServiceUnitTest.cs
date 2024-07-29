using System.Text.Json.Nodes;
using NSubstitute;
using Tgs.GameInterface.CRSports.Core.ApplicationServices;
using Tgs.GameInterface.CRSports.Core.Models;

namespace Tgs.GameInterface.CRSports.Core.UnitTest.ApplicationServices;

public class FundInGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameApiClient = Substitute.For<IGameApiClient>();

		var query = new FundInQuery
		{
			ApiUrl = new Uri("https://api.orb-6789.com/"),
			AgentId = 3238,
			SecretKey = "FnzrUTi5fcsdrws7",
			AgentAccount = "RT706test1",
			AgentPassword = "aaa123",
			PlayerCode = "4uf6k014jp5x",
			OrderId = "29CDE229-AE62-49BA-B03B-2EB7288C0058",
			Amount = 10.00m
		};

		var resp = new MoneyDataResp(
			Gold: 189.0m,
			Payno: "29CDE229-AE62-49BA-B03B-2EB7288C0058",
			Recid: "264552615",
			Resp: JsonNode.Parse("""
			{
				"method": "Withdraw",
				"responeid": "22681727334",
				"respcode": "0000",
				"moneydata": {
					"gold": "189.0",
					"payno": "29CDE229-AE62-49BA-B03B-2EB7288C0058",
					"pay_gold": "10.0",
					"currency": "HKD",
					"pay_way": "-1",
					"recid": "264552615",
					"username": "4uf6k014jp5x"
				},
				"status": "success",
				"timestamp": "1721979994842"
			}
			""")!.ToJsonString());

		_ = fakeGameApiClient.WithdrawAsync(
			apiUrl: Arg.Is(query.ApiUrl),
			agentId: Arg.Is(query.AgentId),
			secretKey: Arg.Is(query.SecretKey),
			agentAccount: Arg.Is(query.AgentAccount),
			agentPassword: Arg.Is(query.AgentPassword),
			memname: Arg.Is(query.PlayerCode),
			payno: Arg.Is(query.OrderId),
			amount: Arg.Is(query.Amount),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(resp);

		var sut = new FundInGetService(fakeGameApiClient);

		var actual = await sut.GetAsync(
			query: query,
			cancellationToken: default);

		Assert.NotNull(actual);
		Assert.Equal(query.OrderId, actual.OrderId);
		Assert.Equal("264552615", actual.ExternalOrderId);
		Assert.NotEmpty(actual.Resp);
	}
}
