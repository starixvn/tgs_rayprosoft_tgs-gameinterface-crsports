using Tgs.GameInterface.CRSports.Core.Models;

namespace Tgs.GameInterface.CRSports.Core;

public interface IGameApiClient
{
	public ValueTask<string> AGLoginAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, CancellationToken cancellationToken);

	public ValueTask CreateMemberAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, string memname, string currency, CancellationToken cancellationToken);

	public ValueTask<ChkMemberBalanceResp> ChkMemberBalanceAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, string memname, CancellationToken cancellationToken);

	public ValueTask<MoneyDataResp> DepositAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, string memname, string payno, decimal amount, CancellationToken cancellationToken);

	public ValueTask<MoneyDataResp> WithdrawAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, string memname, string payno, decimal amount, CancellationToken cancellationToken);

	public ValueTask<bool> ChkTransInfoAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, string memname, string payno, CancellationToken cancellationToken);

	public ValueTask<LaunchGameResp> LaunchGameAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, string memname, string currency, string machine, string langx, string remoteip, CancellationToken cancellationToken);

	public IAsyncEnumerable<WagerDataResp> ALLWagerAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, DateTimeOffset dateStart, DateTimeOffset dateEnd, int settle, int page, CancellationToken cancellationToken);

	public ValueTask KickOutMemAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, string memname, CancellationToken cancellationToken);

	public ValueTask KickOutAllMemAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, CancellationToken cancellationToken);

	public IAsyncEnumerable<MemOnlineResp> GetMemOnlineListAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, CancellationToken cancellationToken);
}
