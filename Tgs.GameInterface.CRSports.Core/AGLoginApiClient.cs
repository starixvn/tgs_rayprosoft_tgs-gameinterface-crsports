using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Authentication;

using Microsoft.Extensions.Logging;
using Tgs.GameInterface.CRSports.Core.Exceptions;
using Tgs.GameInterface.CRSports.Core.Extensions;

namespace Tgs.GameInterface.CRSports.Core;

internal class AGLoginApiClient : IAGLoginApiClient
{
	private readonly ILogger<AGLoginApiClient> _logger;
	private readonly ISystemClock _systemClock;
	private readonly ICryptographyUtensil _cryptographyUtensil;
	private readonly HttpClient _httpClient;

	public AGLoginApiClient(
	ILogger<AGLoginApiClient> logger,
	ISystemClock systemClock,
	ICryptographyUtensil cryptographyUtensil,
	HttpClient httpClient)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
		_cryptographyUtensil = cryptographyUtensil ?? throw new ArgumentNullException(nameof(cryptographyUtensil));
		_httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
	}

	public async ValueTask<string> AGLoginAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, CancellationToken cancellationToken)
	{
		var parameters = new JsonObject
		{
			["username"] = agentAccount ?? throw new ArgumentNullException(nameof(agentAccount)),
			["password"] = agentPassword ?? throw new ArgumentNullException(nameof(agentPassword))
		};

		apiUrl = new Uri(apiUrl, "/app/control_API/agents/api_doaction.php");

		parameters["timestamp"] = _systemClock.UtcNow.ToUnixTimeMilliseconds().ToString();
		var request = _cryptographyUtensil.Encrypt(parameters, secretKey);
		var jsonContent = JsonContent.Create(
			inputValue: new
			{
				Request = request,
				Method = "AGLogin",
				AGID = agentId.ToString()
			},
			options: new JsonSerializerOptions
			{
				//不使用預設JsonNamingPolicy.CamelCase，保留原來參數名稱
				PropertyNamingPolicy = null
			});

		var sw = Stopwatch.StartNew();

		var httpResponseMessage = await _httpClient.PostAsync(
			requestUri: apiUrl,
			content: jsonContent,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		sw.Stop();

		var responseMessage = default(string);

		using (var sr = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync(cancellationToken).Result))
		{
			responseMessage = await sr.ReadToEndAsync().ConfigureAwait(false) ?? string.Empty;
		}

		JsonNode? jsonNode;

		try
		{
			jsonNode = _cryptographyUtensil.Decrypt(responseMessage, secretKey);

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
			Prefix = nameof(AGLoginAsync),
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
		_logger.LogInformation("{apiLogEvent}", JsonSerializer.Serialize(apiLogEvent));

		httpResponseMessage.EnsureSuccessStatusCode();

		return jsonNode is null
			? throw new ArgumentNullException(nameof(jsonNode))
			: !string.Equals(jsonNode["respcode"]!.ToString(), "0000")
				? throw new GameInterfaceException(jsonNode["respcode"]!.ToString())
				: jsonNode["token"]!.GetValue<string>();
	}
}
