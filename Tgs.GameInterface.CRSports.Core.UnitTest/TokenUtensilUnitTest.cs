using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using RichardSzalay.MockHttp;
using StackExchange.Redis;
using Tgs.GameInterface.CRSports.Protos;
using Valhalla.MessageQueue;

namespace Tgs.GameInterface.CRSports.Core.UnitTest;
public class TokenUtensilUnitTest
{
	[Fact]
	public async Task KeyExists_Is_True()
	{

		var fakeDatabase = Substitute.For<IDatabase>();
		var expiry = TimeSpan.FromSeconds(1);
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var apiUrl = new Uri("https://api.orb-6789.com/");
		var agentId = 3238;
		var secretKey = "FnzrUTi5fcsdrws7";
		var agentAccount = "RT706test1";
		var agentPassword = "aaa123";

		var key = string.Concat(typeof(TokenUtensil).Namespace!.Replace(".", "-").ToLower(), ":", agentId);

		_ = fakeDatabase.KeyExistsAsync(
			key: Arg.Is<RedisKey>(key))
			.Returns(true);

		var token = Guid.NewGuid().ToString();
		_ = fakeDatabase.StringGetAsync(
			key: Arg.Is<RedisKey>(key))
			.Returns(token);

		var sut = new TokenUtensil(
			fakeDatabase,
			expiry,
			fakeMessageSender);

		var actual = await sut.GetTokenAsync(
			 apiUrl: apiUrl,
			 agentId: agentId,
			 secretKey: secretKey,
			 agentAccount: agentAccount,
			 agentPassword: agentPassword,
			 cancellationToken: default)
			.ConfigureAwait(true);

		Assert.NotNull(actual);
		Assert.Equal(token, actual);
	}

	[Fact]
	public async Task KeyExists_Is_False()
	{

		var fakeDatabase = Substitute.For<IDatabase>();
		var expiry = TimeSpan.FromSeconds(1);
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var apiUrl = new Uri("https://api.orb-6789.com/");
		var agentId = 3238;
		var secretKey = "FnzrUTi5fcsdrws7";
		var agentAccount = "RT706test1";
		var agentPassword = "aaa123";

		var key = string.Concat(typeof(TokenUtensil).Namespace!.Replace(".", "-").ToLower(), ":", agentId);

		_ = fakeDatabase.KeyExistsAsync(
			key: Arg.Is<RedisKey>(key))
			.Returns(false);

		var token = Guid.NewGuid().ToString();

		_ = fakeMessageSender.RequestAsync<AGLoginRequest, AGLoginReply>(
			subject: Arg.Is("gp.gameinterface.cr.aglogin"),
			data: Arg.Is(new AGLoginRequest
			{
				ApiUrl = apiUrl.ToString(),
				AgentId = agentId,
				SecretKey = secretKey,
				AgentAccount = agentAccount,
				AgentPassword = agentPassword
			}),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(new AGLoginReply
			{
				Token = token
			});

		var sut = new TokenUtensil(
			fakeDatabase,
			expiry,
			fakeMessageSender);

		var actual = await sut.GetTokenAsync(
			 apiUrl: apiUrl,
			 agentId: agentId,
			 secretKey: secretKey,
			 agentAccount: agentAccount,
			 agentPassword: agentPassword,
			 cancellationToken: default)
			.ConfigureAwait(true);

		Assert.NotNull(actual);
		Assert.Equal(token, actual);

		_ = fakeDatabase
			.Received(1)
			.StringSetAsync(
			key: Arg.Is<RedisKey>(key),
			value: Arg.Is<RedisValue>(token),
			expiry: expiry);

		_ = fakeDatabase
			.Received(0)
			.StringGetAsync(
			key: Arg.Is<RedisKey>(key));
	}

	[Fact]
	public async Task Delete_Token()
	{

		var fakeDatabase = Substitute.For<IDatabase>();
		var expiry = TimeSpan.FromSeconds(1);
		var fakeMessageSender = Substitute.For<IMessageSender>();

		var apiUrl = new Uri("https://api.orb-6789.com/");
		var agentId = 3238;

		var sut = new TokenUtensil(
			fakeDatabase,
			expiry,
			fakeMessageSender);

		await sut.DeleteTokenAsync(
			apiUrl: apiUrl,
			agentId: agentId,
			cancellationToken: default)
		   .ConfigureAwait(true);

		var key = string.Concat(typeof(TokenUtensil).Namespace!.Replace(".", "-").ToLower(), ":", agentId);

		_ = fakeDatabase
			.Received(1)
			.KeyDeleteAsync(
			key: Arg.Is<RedisKey>(key),
			flags: Arg.Is(CommandFlags.FireAndForget));
	}
}
