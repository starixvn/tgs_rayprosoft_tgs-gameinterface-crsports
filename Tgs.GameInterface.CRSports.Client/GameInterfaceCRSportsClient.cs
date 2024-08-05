using System.Net;
using System.Runtime.CompilerServices;
using Google.Protobuf.WellKnownTypes;
using Tgs.GameInterface.CRSports.Protos;

namespace Tgs.GameInterface.CRSports.Client;

internal sealed class GameInterfaceCRSportsClient : IGameInterfaceCRSportsClient
{
	private readonly GameInterfaceCRSports.GameInterfaceCRSportsClient _client;

	public GameInterfaceCRSportsClient(
		GameInterfaceCRSports.GameInterfaceCRSportsClient client)
	{
		_client = client ?? throw new ArgumentNullException(nameof(client));
	}

	public async ValueTask<string> CheckingServiceAsync(string machineName, CancellationToken cancellationToken = default)
		=> (await _client.CheckingServiceAsync(
			request: new CheckingServiceRequest
			{
				MachineName = machineName,
			},
			cancellationToken: cancellationToken)
		.ConfigureAwait(false)).MachineName;
	public async ValueTask<FundReply> FundInAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, string playerCode, string orderId, decimal amount, CancellationToken cancellationToken = default)
		=> await _client.FundInAsync(
			request: new FundRequest
			{
				ApiUrl = apiUrl.ToString(),
				AgentId = agentId,
				SecretKey = secretKey,
				AgentAccount = agentAccount,
				AgentPassword = agentPassword,
				PlayerCode = playerCode,
				OrderId = orderId,
				Amount = amount
			},
			cancellationToken: cancellationToken)
		.ConfigureAwait(false);
	public async ValueTask<FundReply> FundOutAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, string playerCode, string orderId, decimal amount, CancellationToken cancellationToken = default)
		=> await _client.FundOutAsync(
			request: new FundRequest
			{
				ApiUrl = apiUrl.ToString(),
				AgentId = agentId,
				SecretKey = secretKey,
				AgentAccount = agentAccount,
				AgentPassword = agentPassword,
				PlayerCode = playerCode,
				OrderId = orderId,
				Amount = amount
			},
			cancellationToken: cancellationToken)
		.ConfigureAwait(false);
	public async ValueTask<bool> FundStatusAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, string playerCode, string orderId, CancellationToken cancellationToken = default)
		=> (await _client.FundStatusAsync(
			request: new FundStatusRequest
			{
				ApiUrl = apiUrl.ToString(),
				AgentId = agentId,
				SecretKey = secretKey,
				AgentAccount = agentAccount,
				AgentPassword = agentPassword,
				PlayerCode = playerCode,
				OrderId = orderId
			},
			cancellationToken: cancellationToken)
		.ConfigureAwait(false)).Status;
	public async IAsyncEnumerable<GameRecordInfo> GameRecordBettingAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, DateTimeOffset startTime, DateTimeOffset endTime, int page = 1, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var reply = await _client.GameRecordBettingAsync(
			request: new GameRecordRequest
			{
				ApiUrl = apiUrl.ToString(),
				AgentId = agentId,
				SecretKey = secretKey,
				AgentAccount = agentAccount,
				AgentPassword = agentPassword,
				StartTime = startTime.ToTimestamp(),
				EndTime = endTime.ToTimestamp(),
				Page = page
			},
			cancellationToken: cancellationToken)
		   .ConfigureAwait(false);

		foreach (var info in reply.Items)
			yield return info;
	}
	public async IAsyncEnumerable<GameRecordInfo> GameRecordSettleAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, DateTimeOffset startTime, DateTimeOffset endTime, int page = 1, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var reply = await _client.GameRecordSettleAsync(
			request: new GameRecordRequest
			{
				ApiUrl = apiUrl.ToString(),
				AgentId = agentId,
				SecretKey = secretKey,
				AgentAccount = agentAccount,
				AgentPassword = agentPassword,
				StartTime = startTime.ToTimestamp(),
				EndTime = endTime.ToTimestamp(),
				Page = page
			},
			cancellationToken: cancellationToken)
		   .ConfigureAwait(false);

		foreach (var info in reply.Items)
			yield return info;
	}
	public async ValueTask<GameUrlReply> GameUrlAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, string playerCode, string currency, string language, bool isMobile, string ipAddress, CancellationToken cancellationToken = default)
		=> await _client.GameUrlAsync(
			request: new GameUrlRequest
			{
				ApiUrl = apiUrl.ToString(),
				AgentId = agentId,
				SecretKey = secretKey,
				AgentAccount = agentAccount,
				AgentPassword = agentPassword,
				PlayerCode = playerCode,
				Currency = currency,
				Language = language,
				IpAddress = ipAddress,
				IsMobile = isMobile
			},
			cancellationToken: cancellationToken)
		.ConfigureAwait(false);
	public async ValueTask<PlayerBalanceReply> PlayerBalanceAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, string playerCode, CancellationToken cancellationToken = default)
		=> await _client.PlayerBalanceAsync(
			request: new PlayerBalanceRequest
			{
				ApiUrl = apiUrl.ToString(),
				AgentId = agentId,
				SecretKey = secretKey,
				AgentAccount = agentAccount,
				AgentPassword = agentPassword,
				PlayerCode = playerCode
			},
			cancellationToken: cancellationToken)
		.ConfigureAwait(false);
	public async ValueTask PlayerCreateAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, string playerCode, string currency, CancellationToken cancellationToken = default)
		=> _ = await _client.PlayerCreateAsync(
			request: new PlayerCreateRequest
			{
				ApiUrl = apiUrl.ToString(),
				AgentId = agentId,
				SecretKey = secretKey,
				AgentAccount = agentAccount,
				AgentPassword = agentPassword,
				PlayerCode = playerCode,
				Currency = currency
			},
			cancellationToken: cancellationToken)
		.ConfigureAwait(false);
	public async ValueTask PlayerLogoutAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, string playerCode, CancellationToken cancellationToken = default)
		=> _ = await _client.PlayerLogoutAsync(
			request: new PlayerLogoutRequest
			{
				ApiUrl = apiUrl.ToString(),
				AgentId = agentId,
				SecretKey = secretKey,
				AgentAccount = agentAccount,
				AgentPassword = agentPassword,
				PlayerCode = playerCode
			},
			cancellationToken: cancellationToken)
		.ConfigureAwait(false);
	public async ValueTask<bool> PlayerOnlineAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, string playerCode, CancellationToken cancellationToken = default)
		=> (await _client.PlayerOnlineAsync(
			request: new PlayerOnlineRequest
			{
				ApiUrl = apiUrl.ToString(),
				AgentId = agentId,
				SecretKey = secretKey,
				AgentAccount = agentAccount,
				AgentPassword = agentPassword,
				PlayerCode = playerCode
			},
			cancellationToken: cancellationToken)
		.ConfigureAwait(false)).Online;
}