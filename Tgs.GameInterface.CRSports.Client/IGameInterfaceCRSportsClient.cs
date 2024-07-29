using Tgs.GameInterface.CRSports.Protos;

namespace Tgs.GameInterface.CRSports.Client;

public interface IGameInterfaceCRSportsClient
{
	ValueTask<string> CheckingServiceAsync(string machineName, CancellationToken cancellationToken = default);

	ValueTask<FundReply> FundInAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, string playerCode, string orderId, decimal amount, CancellationToken cancellationToken = default);

	ValueTask<FundReply> FundOutAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, string playerCode, string orderId, decimal amount, CancellationToken cancellationToken = default);

	ValueTask<bool> FundStatusAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, string playerCode, string orderId, CancellationToken cancellationToken = default);

	IAsyncEnumerable<GameRecordInfo> GameRecordBettingAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, DateTimeOffset startTime, DateTimeOffset endTime, int page = 1, CancellationToken cancellationToken = default);

	IAsyncEnumerable<GameRecordInfo> GameRecordSettleAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, DateTimeOffset startTime, DateTimeOffset endTime, int page = 1, CancellationToken cancellationToken = default);

	ValueTask<GameUrlReply> GameUrlAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, string playerCode, string currency, string language, bool isMobile, string ipAddress, CancellationToken cancellationToken = default);

	ValueTask<PlayerBalanceReply> PlayerBalanceAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, string playerCode, CancellationToken cancellationToken = default);

	ValueTask PlayerCreateAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, string playerCode, string currency, CancellationToken cancellationToken = default);

	ValueTask PlayerLogoutAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, string playerCode, CancellationToken cancellationToken = default);

	ValueTask<bool> PlayerOnlineAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, string playerCode, CancellationToken cancellationToken = default);
}
