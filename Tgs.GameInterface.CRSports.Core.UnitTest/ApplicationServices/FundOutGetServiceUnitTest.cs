using System.Text.Json.Nodes;
using NSubstitute;
using Tgs.GameInterface.CRSports.Core.ApplicationServices;
using Tgs.GameInterface.CRSports.Core.Models;

namespace Tgs.GameInterface.CRSports.Core.UnitTest.ApplicationServices;

public class FundOutGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameApiClient = Substitute.For<IGameApiClient>();

		var query = new FundOutQuery
		{
			ApiUrl = new Uri("https://api.orb-6789.com/"),
			AgentId = 3238,
			SecretKey = "FnzrUTi5fcsdrws7",
			AgentAccount = "RT706test1",
			AgentPassword = "aaa123",
			PlayerCode = "4uf6k014jp5x",
			OrderId = "E366D434-F19E-4706-8FB9-96DFB21F2EF5",
			Amount = 99.00m
		};

		var resp = new MoneyDataResp(
			Gold: 189.0m,
			Payno: "E366D434-F19E-4706-8FB9-96DFB21F2EF5",
			Recid: "264549283",
			Resp: JsonNode.Parse("""
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
			""")!.ToJsonString());

		_ = fakeGameApiClient.DepositAsync(
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

		var sut = new FundOutGetService(fakeGameApiClient);

		var actual = await sut.GetAsync(
			query: query,
			cancellationToken: default);

		Assert.NotNull(actual);
		Assert.Equal(query.OrderId, actual.OrderId);
		Assert.Equal("264549283", actual.ExternalOrderId);
		Assert.NotEmpty(actual.Resp);
	}
}
