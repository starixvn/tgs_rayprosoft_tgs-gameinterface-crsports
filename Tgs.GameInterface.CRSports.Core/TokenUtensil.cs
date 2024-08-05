using System.Diagnostics;
using System.Net.Http.Json;
using System.Net.Http;
using System.Text.Json;
using System;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Tgs.GameInterface.CRSports.Core.Exceptions;
using Tgs.GameInterface.CRSports.Core.Extensions;
using Tgs.GameInterface.CRSports.Protos;
using Valhalla.MessageQueue;

namespace Tgs.GameInterface.CRSports.Core;

internal class TokenUtensil : ITokenUtensil
{
	private readonly SemaphoreSlim _semaphore = new(1, 1);
	private readonly IDatabase _database;
	private readonly TimeSpan _expiry;
	private readonly IAGLoginApiClient _client;

	public TokenUtensil(
		IDatabase database,
		TimeSpan expiry,
		IAGLoginApiClient client)
	{
		_database = database ?? throw new ArgumentNullException(nameof(database));
		_expiry = expiry;
		_client = client ?? throw new ArgumentNullException(nameof(client));
	}

	public async ValueTask DeleteTokenAsync(Uri apiUrl, int agentId, CancellationToken cancellationToken = default)
	{
		var key = string.Concat(typeof(TokenUtensil).Namespace!.Replace(".", "-").ToLower(), ":", agentId);
		await _database.KeyDeleteAsync(key, CommandFlags.FireAndForget).ConfigureAwait(false);
	}

	public async ValueTask<string> GetTokenAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, CancellationToken cancellationToken = default)
	{
		var key = string.Concat(typeof(TokenUtensil).Namespace!.Replace(".", "-").ToLower(), ":", agentId);
		if (!await _database.KeyExistsAsync(key).ConfigureAwait(false))
		{
			await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
			try
			{
				if (!await _database.KeyExistsAsync(key).ConfigureAwait(false))
				{
					var token = await _client.AGLoginAsync(
						 apiUrl, agentId, secretKey, agentAccount, agentPassword, cancellationToken)
						.ConfigureAwait(false);

					await _database.StringSetAsync(
						key: key,
						value: token,
						expiry: _expiry)
						.ConfigureAwait(false);

					return token;
				}
			}
			finally
			{
				_semaphore.Release();
			}
		}

		return (await _database.StringGetAsync(key).ConfigureAwait(false)).ToString();
	}
}
