using System.Diagnostics;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Tgs.GameInterface.CRSports.Core.Exceptions;
using Tgs.GameInterface.CRSports.Core.Extensions;
using Tgs.GameInterface.CRSports.Core.Models;
using Tgs.GameInterface.CRSports.Core;

namespace Tgs.GameInterface.CRSports.Core;

internal class GameApiClient(
	ILogger<GameApiClient> logger,
	IOptions<PolicySettingsOptions> policySettingsOptions,
	IOptions<GameInterfaceSettingsOptions> gameInterfaceOptions,
	TimeProvider timeProvider,
	DateTimeConverter dateTimeConverter,
	ITokenUtensil tokenUtensil,
	ICryptographyUtensil cryptographyUtensil,
	HttpClient httpClient) : IGameApiClient
{
	private readonly GameInterfaceSettingsOptions _gameInterfaceOptions = gameInterfaceOptions.Value;
	private readonly PolicySettingsOptions _policySettingsOptions = policySettingsOptions.Value;

	public async ValueTask<string> AGLoginAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, CancellationToken cancellationToken)
	{
		var parameters = new JsonObject
		{
			["username"] = agentAccount,
			["password"] = agentPassword
		};

		apiUrl = new Uri(apiUrl, "/app/control_API/agents/api_doaction.php");

		var jsonNode = await PosetAsync(
			apiUrl: apiUrl,
			agentId: agentId,
			secretKey: secretKey,
			method: "AGLogin",
			parameters: parameters,
			logPrefix: nameof(AGLoginAsync).ToString(),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return !string.Equals(jsonNode["respcode"]!.ToString(), "0000") ? throw new GameInterfaceException(jsonNode["respcode"]!.ToString()) : jsonNode["token"]!.GetValue<string>();
	}

	public async IAsyncEnumerable<WagerDataResp> ALLWagerAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, DateTimeOffset dateStart, DateTimeOffset dateEnd, int settle, int page, [EnumeratorCancellation] CancellationToken cancellationToken)
	{
		var parameters = new JsonObject
		{
			["dateStart"] = dateTimeConverter.DatetimeConvertToString(dateStart),
			["dateEnd"] = dateTimeConverter.DatetimeConvertToString(dateEnd),
			["page"] = page,
			["settle"] = settle,
			["langx"] = "en-us"
		};

		var jsonNode = await TokenAsync(
			apiUrl: apiUrl,
			agentId: agentId,
			secretKey: secretKey,
			agentAccount: agentAccount,
			agentPassword: agentPassword,
			method: "ALLWager",
			parameters: parameters,
			logPrefix: nameof(ALLWagerAsync).ToString(),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (!string.Equals(jsonNode["respcode"]!.ToString(), "0000", StringComparison.Ordinal))
		{
			if (!string.Equals(jsonNode["respcode"]!.ToString(), "0003", StringComparison.Ordinal))
			{
				throw new GameInterfaceException(jsonNode["respcode"]!.ToString());
			}

			yield break;
		}

		foreach (var wagerdataNode in jsonNode["wager_data"]!.AsArray())
		{
			if (wagerdataNode is JsonObject wagerdata)
			{
				var resultdate = wagerdata["resultdate"]?.GetValue<string>() ?? null;
				var membersVgold = wagerdata["members_vgold"]?.GetValue<string>() ?? wagerdata["gold"]!.GetValue<string>();

				yield return new WagerDataResp(
					 Id: wagerdata["id"]!.GetValue<string>(),
					 Username: wagerdata["username"]!.GetValue<string>(),
					 Gtype: wagerdata["gtype"]!.GetValue<string>(),
					 Adddate: dateTimeConverter.StringConvertToDatetime(wagerdata["adddate"]!.GetValue<string>()),
					 Resultdate: !string.IsNullOrEmpty(resultdate) ? dateTimeConverter.StringConvertToDatetime(resultdate) : DateTimeOffset.UnixEpoch,
					 Settle: wagerdata["settle"]!.GetValue<string>(),
					 Result: wagerdata["result"]!.GetValue<string>(),
					 Gold: decimal.Parse(wagerdata["gold"]!.GetValue<string>()),
					 MembersVgold: decimal.Parse(membersVgold),
					 Wingold: decimal.Parse(wagerdata["wingold"]!.GetValue<string>()),
					 CashoutId: wagerdata["cashoutid"]!.GetValue<string>(),
					 AdditionalData: wagerdata.ToJsonString());
			}
		}
	}

	public async ValueTask<ChkMemberBalanceResp> ChkMemberBalanceAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, string memname, CancellationToken cancellationToken)
	{
		var parameters = new JsonObject
		{
			["memname"] = memname
		};

		var jsonNode = await TokenAsync(
			apiUrl: apiUrl,
			agentId: agentId,
			secretKey: secretKey,
			agentAccount: agentAccount,
			agentPassword: agentPassword,
			method: "chkMemberBalance",
			parameters: parameters,
			logPrefix: nameof(ChkMemberBalanceAsync).ToString(),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return !string.Equals(jsonNode["respcode"]!.ToString(), "0000")
			? !string.Equals(jsonNode["respcode"]!.ToString(), "0003")
				? throw new GameInterfaceException(jsonNode["respcode"]!.ToString())
				: new ChkMemberBalanceResp(
					IsPlayerExist: false,
					Balance: 0.0m)
			: new ChkMemberBalanceResp(
				IsPlayerExist: true,
				Balance: decimal.Parse(jsonNode["balance"]!.GetValue<string>()));
	}

	public async ValueTask<bool> ChkTransInfoAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, string memname, string payno, CancellationToken cancellationToken)
	{
		var parameters = new JsonObject
		{
			["memname"] = memname,
			["transidtype"] = "1",
			["transid"] = payno
		};

		var jsonNode = await TokenAsync(
			apiUrl: apiUrl,
			agentId: agentId,
			secretKey: secretKey,
			agentAccount: agentAccount,
			agentPassword: agentPassword,
			method: "ChkTransInfo",
			parameters: parameters,
			logPrefix: nameof(ChkTransInfoAsync).ToString(),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return string.Equals(jsonNode["respcode"]!.ToString(), "0000")
			|| (!string.Equals(jsonNode["respcode"]!.ToString(), "0003")
				? throw new GameInterfaceException(jsonNode["respcode"]!.ToString())
				: false);
	}

	public async ValueTask CreateMemberAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, string memname, string currency, CancellationToken cancellationToken)
	{
		var parameters = new JsonObject
		{
			["memname"] = memname,
			["password"] = _gameInterfaceOptions.PlayerPassword,
			["currency"] = currency
		};

		var jsonNode = await TokenAsync(
			apiUrl: apiUrl,
			agentId: agentId,
			secretKey: secretKey,
			agentAccount: agentAccount,
			agentPassword: agentPassword,
			method: "CreateMember",
			parameters: parameters,
			logPrefix: nameof(CreateMemberAsync).ToString(),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (!string.Equals(jsonNode["respcode"]!.ToString(), "0000"))
		{
			if (!string.Equals(jsonNode["respcode"]!.ToString(), "0008"))
			{
				throw new GameInterfaceException(jsonNode["respcode"]!.ToString());
			}
		}
	}

	public async ValueTask<MoneyDataResp> DepositAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, string memname, string payno, decimal amount, CancellationToken cancellationToken)
	{
		var parameters = new JsonObject
		{
			["memname"] = memname,
			["amount"] = amount,
			["payno"] = payno
		};

		var jsonNode = await TokenAsync(
			apiUrl: apiUrl,
			agentId: agentId,
			secretKey: secretKey,
			agentAccount: agentAccount,
			agentPassword: agentPassword,
			method: "Deposit",
			parameters: parameters,
			logPrefix: nameof(DepositAsync).ToString(),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return !string.Equals(jsonNode["respcode"]!.ToString(), "0000")
			? throw new GameInterfaceException(jsonNode["respcode"]!.ToString())
			: new MoneyDataResp(
				 Gold: decimal.Parse(jsonNode["moneydata"]!["gold"]!.GetValue<string>()),
				 Payno: jsonNode["moneydata"]!["payno"]!.GetValue<string>(),
				 Recid: jsonNode["moneydata"]!["recid"]!.GetValue<string>(),
				 Resp: jsonNode["moneydata"]!.ToJsonString());
	}

	public async IAsyncEnumerable<MemOnlineResp> GetMemOnlineListAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, [EnumeratorCancellation] CancellationToken cancellationToken)
	{
		var parameters = new JsonObject { };

		var jsonNode = await TokenAsync(
			apiUrl: apiUrl,
			agentId: agentId,
			secretKey: secretKey,
			agentAccount: agentAccount,
			agentPassword: agentPassword,
			method: "GetMemOnlineList",
			parameters: parameters,
			logPrefix: nameof(GetMemOnlineListAsync).ToString(),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (!string.Equals(jsonNode["respcode"]!.ToString(), "0000"))
		{
			throw new GameInterfaceException(jsonNode["respcode"]!.ToString());
		}

		foreach (var userdataNode in jsonNode["userdata"]!.AsArray())
		{
			if (userdataNode is JsonObject userdata)
				yield return new MemOnlineResp(
					UserUsername: userdata["username"]!.GetValue<string>(),
					UserCurrency: userdata["currency"]!.GetValue<string>(),
					UserGold: userdata["usergold"]!.GetValue<decimal>());
		}
	}

	public async ValueTask KickOutAllMemAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, CancellationToken cancellationToken)
	{
		var parameters = new JsonObject { };

		var jsonNode = await TokenAsync(
			apiUrl: apiUrl,
			agentId: agentId,
			secretKey: secretKey,
			agentAccount: agentAccount,
			agentPassword: agentPassword,
			method: "KickOutAllMem",
			parameters: parameters,
			logPrefix: nameof(KickOutAllMemAsync).ToString(),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (!string.Equals(jsonNode["respcode"]!.ToString(), "0000"))
		{
			throw new GameInterfaceException(jsonNode["respcode"]!.ToString());
		}
	}

	public async ValueTask KickOutMemAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, string memname, CancellationToken cancellationToken)
	{
		var parameters = new JsonObject
		{
			["memname"] = memname
		};

		var jsonNode = await TokenAsync(
			apiUrl: apiUrl,
			agentId: agentId,
			secretKey: secretKey,
			agentAccount: agentAccount,
			agentPassword: agentPassword,
			method: "KickOutMem",
			parameters: parameters,
			logPrefix: nameof(KickOutMemAsync).ToString(),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (!string.Equals(jsonNode["respcode"]!.ToString(), "0000"))
		{
			throw new GameInterfaceException(jsonNode["respcode"]!.ToString());
		}
	}

	public async ValueTask<LaunchGameResp> LaunchGameAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, string memname, string currency, string machine, string langx, string remoteip, CancellationToken cancellationToken)
	{
		var parameters = new JsonObject
		{
			["memname"] = memname,
			["password"] = _gameInterfaceOptions.PlayerPassword,
			["currency"] = currency,
			["machine"] = machine,
			["langx"] = langx,
			["remoteip"] = remoteip
		};

		var jsonNode = await TokenAsync(
			apiUrl: apiUrl,
			agentId: agentId,
			secretKey: secretKey,
			agentAccount: agentAccount,
			agentPassword: agentPassword,
			method: "LaunchGame",
			parameters: parameters,
			logPrefix: nameof(LaunchGameAsync).ToString(),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return !string.Equals(jsonNode["respcode"]!.ToString(), "0000")
			? !string.Equals(jsonNode["respcode"]!.ToString(), "0003")
				? throw new GameInterfaceException(jsonNode["respcode"]!.ToString())
				: new LaunchGameResp(
					IsPlayerExist: false,
					GameUrl: string.Empty)
			: new LaunchGameResp(
				IsPlayerExist: true,
				GameUrl: jsonNode["launchgameurl"]!.GetValue<string>());
	}

	public async ValueTask<MoneyDataResp> WithdrawAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, string memname, string payno, decimal amount, CancellationToken cancellationToken)
	{
		var parameters = new JsonObject
		{
			["memname"] = memname,
			["amount"] = amount,
			["payno"] = payno
		};

		var jsonNode = await TokenAsync(
			apiUrl: apiUrl,
			agentId: agentId,
			secretKey: secretKey,
			agentAccount: agentAccount,
			agentPassword: agentPassword,
			method: "Withdraw",
			parameters: parameters,
			logPrefix: nameof(WithdrawAsync).ToString(),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return !string.Equals(jsonNode["respcode"]!.ToString(), "0000")
			? throw new GameInterfaceException(jsonNode["respcode"]!.ToString())
			: new MoneyDataResp(
				 Gold: decimal.Parse(jsonNode["moneydata"]!["gold"]!.GetValue<string>()),
				 Payno: jsonNode["moneydata"]!["payno"]!.GetValue<string>(),
				 Recid: jsonNode["moneydata"]!["recid"]!.GetValue<string>(),
				 Resp: jsonNode["moneydata"]!.ToJsonString());
	}

	private async ValueTask<JsonNode> TokenAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, string method, JsonNode parameters, string logPrefix, CancellationToken cancellationToken = default)
	{
		var token = await tokenUtensil.GetTokenAsync(
			apiUrl: apiUrl,
			agentId: agentId,
			secretKey: secretKey,
			agentAccount: agentAccount,
			agentPassword: agentPassword,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		parameters["token"] = token;
		var retryPolicy = Policy
			.HandleResult<JsonNode>(jsonNode => string.Equals(jsonNode["respcode"]!.ToString(), "0002"))
			.WaitAndRetryAsync(
					retryCount: _policySettingsOptions.GlobalRetryCount,
					retryAttempt => TimeSpan.FromSeconds(Math.Pow(_policySettingsOptions.GlobalRetryWaitSec, retryAttempt)),
					onRetry: async (response, retryTime, retryCount, context) =>
					{
						logger.LogWarning("{LogWarning}",
						new
						{
							LogOn = DateTime.Now.ToString("u"),
							RetryTime = retryTime,
							RetryCount = retryCount,
							Response = response
						});
						await tokenUtensil.DeleteTokenAsync(apiUrl, agentId, cancellationToken).ConfigureAwait(false);
						token = await tokenUtensil.GetTokenAsync(
							apiUrl: apiUrl,
							agentId: agentId,
							secretKey: secretKey,
							agentAccount: agentAccount,
							agentPassword: agentPassword,
							cancellationToken: cancellationToken)
							.ConfigureAwait(false);
						parameters["token"] = token;
					});

		return await retryPolicy.ExecuteAsync(() => PosetAsync(
			apiUrl: new Uri(apiUrl, "/app/control_API/agents/api_doaction.php"),
			agentId: agentId,
			secretKey: secretKey,
			method: method,
			parameters: parameters,
			logPrefix: logPrefix,
			cancellationToken: cancellationToken).AsTask())
			.ConfigureAwait(false);
	}

	private async ValueTask<JsonNode> PosetAsync(Uri apiUrl, int agentId, string secretKey, string method, JsonNode parameters, string logPrefix, CancellationToken cancellationToken = default)
	{
		parameters["timestamp"] = timeProvider.GetUtcNow().ToUnixTimeMilliseconds().ToString();
		var request = cryptographyUtensil.Encrypt(parameters, secretKey);
		var jsonContent = JsonContent.Create(
			inputValue: new
			{
				Request = request,
				Method = method,
				AGID = agentId.ToString()
			},
			options: new JsonSerializerOptions
			{
				//不使用預設JsonNamingPolicy.CamelCase，保留原來參數名稱
				PropertyNamingPolicy = null
			});

		var sw = Stopwatch.StartNew();

		var httpResponseMessage = await httpClient.PostAsync(
			requestUri: apiUrl,
			content: jsonContent,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		sw.Stop();

		var responseMessage = default(string);

		using (var sr = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync(cancellationToken).Result))
		{
			responseMessage = await sr.ReadToEndAsync(
				cancellationToken: cancellationToken)
				.ConfigureAwait(false) ?? string.Empty;
		}

		JsonNode? jsonNode;

		try
		{
			jsonNode = cryptographyUtensil.Decrypt(responseMessage, secretKey);

			jsonNode ??= JsonNode.Parse(responseMessage);
		}
		catch
		{
			jsonNode = null;
		}

		var apiLogEvent = new
		{
			Path = httpResponseMessage.RequestMessage?.RequestUri?.AbsoluteUri.NoLongerThan(512) ?? string.Empty,
			Method = httpResponseMessage.RequestMessage?.Method.ToString().NoLongerThan(8) ?? string.Empty,
			Prefix = logPrefix,
			ServerName = Environment.MachineName.NoLongerThan(128),
			RequestHeaders = httpResponseMessage.RequestMessage?.Headers + httpResponseMessage?.Content?.Headers?.ToString(),
			RequestParameters = parameters.ToJsonString(),
			RequestContent = (await jsonContent.ReadAsStringAsync(cancellationToken).ConfigureAwait(false)).NoLongerThan(4096),
			ResponseHeaders = httpResponseMessage!.Headers.ToString(),
			ResponseContent = responseMessage.NoLongerThan(4096),
			ResponseDecryptContent = jsonNode?.ToString().NoLongerThan(4096) ?? string.Empty,
			ResponseCode = (short)httpResponseMessage.StatusCode,
			ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
			RecordedOn = DateTimeOffset.UtcNow
		};
		logger.LogInformation("{apiLogEvent}", JsonSerializer.Serialize(apiLogEvent));

		httpResponseMessage.EnsureSuccessStatusCode();

		return jsonNode!;
	}
}
