using StackExchange.Redis;
using Tgs.GameInterface.GlobalTechnology.Protos;
using Valhalla.MessageQueue;

namespace Tgs.GameInterface.CRSports.Core;

internal class TokenUtensil(
	IDatabase database,
	IMessageSender messageSender) : ITokenUtensil
{
	private readonly SemaphoreSlim _semaphore = new(1, 1);

	public async ValueTask DeleteTokenAsync(Uri apiBaseUrl, int agentId, CancellationToken cancellationToken = default)
	{
		var key = string.Concat(typeof(TokenUtensil).Namespace!.Replace(".", "-").ToLower(), ":", agentId);
		await database.KeyDeleteAsync(key, CommandFlags.FireAndForget).ConfigureAwait(false);
	}

	public async ValueTask<string> GetTokenAsync(Uri apiBaseUrl, int agentId, string secretKey, string agentAccount, string agentPassword, CancellationToken cancellationToken = default)
	{
		var key = string.Concat(typeof(TokenUtensil).Namespace!.Replace(".", "-").ToLower(), ":", agentId);
		if (!await database.KeyExistsAsync(key).ConfigureAwait(false))
		{
			await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
			try
			{
				if (!await database.KeyExistsAsync(key).ConfigureAwait(false))
				{
					var token = await messageSender.RequestAsync<AGLoginRequest, AGLoginReply>(
						"gp.gameinterface.cr.aglogin",
						new AGLoginRequest
						{
							ApiUrl = apiBaseUrl.ToString(),
							AgentId = agentId,
							SecretKey = secretKey,
							AgentAccount = agentAccount,
							AgentPassword = agentPassword
						},
						cancellationToken: cancellationToken)
						.ConfigureAwait(false);

					await database.StringSetAsync(
						key: key,
						value: token.Token,
						expiry: TimeSpan.FromMinutes(10))
						.ConfigureAwait(false);

					return token.Token;
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
