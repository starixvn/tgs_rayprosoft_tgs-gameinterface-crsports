using NSubstitute;
using StackExchange.Redis;

namespace Tgs.GameInterface.CRSports.Core.UnitTest;

public class TokenUtensilUnitTest
{
	[Fact]
	public async Task KeyExists_Is_True()
	{
		var fakeDatabase = Substitute.For<IDatabase>();
		var expiry = TimeSpan.FromSeconds(1);
		var fakeAGLoginApiClient = Substitute.For<IAGLoginApiClient>();

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
			fakeAGLoginApiClient);

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
		var fakeAGLoginApiClient = Substitute.For<IAGLoginApiClient>();

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

		_ = fakeAGLoginApiClient.AGLoginAsync(
			apiUrl: Arg.Is(apiUrl),
			agentId: Arg.Is(agentId),
			secretKey: Arg.Is(secretKey),
			agentAccount: Arg.Is(agentAccount),
			agentPassword: Arg.Is(agentPassword),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(token);

		var sut = new TokenUtensil(
			fakeDatabase,
			expiry,
			fakeAGLoginApiClient);

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
		var fakeAGLoginApiClient = Substitute.For<IAGLoginApiClient>();

		var apiUrl = new Uri("https://api.orb-6789.com/");
		var agentId = 3238;

		var sut = new TokenUtensil(
			fakeDatabase,
			expiry,
			fakeAGLoginApiClient);

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
