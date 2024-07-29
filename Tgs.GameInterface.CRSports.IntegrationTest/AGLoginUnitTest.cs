using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using StackExchange.Redis;
using Tgs.GameInterface.CRSports.Core.ApplicationServices;
using Tgs.GameInterface.CRSports.GrpcServices;
using Tgs.GameInterface.CRSports.Messages;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameInterface.CRSports.IntegrationTest;
public class AGLoginUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var apiUrl = new Uri("https://api.orb-6789.com/");
		var agentId = 3238;
		var secretKey = "FnzrUTi5fcsdrws7";
		var agentAccount = "RT706test1";
		var agentPassword = "aaa123";

		var token = Guid.NewGuid().ToString();
		var web = new GameInterfaceCRSportsApplication(builder => _ = builder.ConfigureServices(services => services.AddTransient(sp =>
		{
			var fakeMessageSender = Substitute.For<IMessageSender>();

			_ = fakeMessageSender.RequestAsync<Protos.AGLoginRequest, Protos.AGLoginReply>(
				Arg.Is("gp.gameinterface.cr.aglogin"),
				Arg.Is(new Protos.AGLoginRequest
				{
					ApiUrl = apiUrl.ToString(),
					AgentId = agentId,
					SecretKey = secretKey,
					AgentAccount = agentAccount,
					AgentPassword = agentPassword
				}),
				Arg.Any<CancellationToken>())
			.Returns(new Protos.AGLoginReply
			{
				Token = token
			});

			return fakeMessageSender;
		})));

		var sut = web.CreateClient();

		var content = JsonContent.Create(new AGLoginQuery
		{
			ApiUrl = apiUrl,
			AgentId = agentId,
			SecretKey = secretKey,
			AgentAccount = agentAccount,
			AgentPassword = agentPassword
		},
		options: new()
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			Converters ={
				new JsonStringEnumConverter()
			}
		});
		var response = await sut.PostAsync("/cr/aglogin", content);

		_ = response.EnsureSuccessStatusCode();

		var actual = JsonSerializer.Deserialize<string>(await response.Content.ReadAsStringAsync());

		Assert.NotNull(actual);
		Assert.Equal(token, actual);
	}
}
