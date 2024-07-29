using StackExchange.Redis;
using Tgs.GameInterface.CRSports.Protos;
using Valhalla.MessageQueue;

namespace Tgs.GameInterface.CRSports.Core;

internal class TokenUtensil(
	IDatabase database,
	TimeSpan expiry,
	IMessageSender messageSender) : ITokenUtensil
{
	private readonly SemaphoreSlim _semaphore = new(1, 1);

	public async ValueTask DeleteTokenAsync(Uri apiUrl, int agentId, CancellationToken cancellationToken = default)
	{
		var key = string.Concat(typeof(TokenUtensil).Namespace!.Replace(".", "-").ToLower(), ":", agentId);
		await database.KeyDeleteAsync(key, CommandFlags.FireAndForget).ConfigureAwait(false);
	}

	public async ValueTask<string> GetTokenAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, CancellationToken cancellationToken = default)
	{
		var key = string.Concat(typeof(TokenUtensil).Namespace!.Replace(".", "-").ToLower(), ":", agentId);
		if (!await database.KeyExistsAsync(key).ConfigureAwait(false))
		{
			await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
			try
			{
				if (!await database.KeyExistsAsync(key).ConfigureAwait(false))
				{
					var reply = await messageSender.RequestAsync<AGLoginRequest, AGLoginReply>(
						"gp.gameinterface.cr.aglogin",
						new AGLoginRequest
						{
							ApiUrl = apiUrl.ToString(),
							AgentId = agentId,
							SecretKey = secretKey,
							AgentAccount = agentAccount,
							AgentPassword = agentPassword
						},
						cancellationToken: cancellationToken)
						.ConfigureAwait(false);

					await database.StringSetAsync(
						key: key,
						value: reply.Token,
						expiry: expiry)
						.ConfigureAwait(false);

					return reply.Token;
				}
			}
			finally
			{
				_semaphore.Release();
			}
		}

		return (await database.StringGetAsync(key).ConfigureAwait(false)).ToString();
	}
}
