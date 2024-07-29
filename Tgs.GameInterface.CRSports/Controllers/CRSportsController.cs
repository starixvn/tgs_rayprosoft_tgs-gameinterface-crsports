using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tgs.GameInterface.CRSports.Core.ApplicationServices;
using Tgs.GameInterface.CRSports.Protos;
using Tgs.ServiceUtility;
using Valhalla.MessageQueue;

namespace Tgs.GameInterface.CRSports.Controllers;

[Route("CR")]
[ApiController]
public class CRSportsController : ControllerBase
{
	/// <summary>
	/// Ags the login v2 asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpPost("aglogin/v2", Name = "AGLoginV2")]
	public ValueTask<string> AGLoginV2Async(
		[FromServices] IGetService<AGLoginQuery, string> service,
		[FromBody] AGLoginQuery source)
		=> service.GetAsync(
			query: source,
			cancellationToken: HttpContext.RequestAborted);
	/// <summary>
	/// Ags the login asynchronous.
	/// </summary>
	/// <param name="messageSender">The message sender.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpPost("aglogin", Name = "AGLogin")]
	public async ValueTask<string> AGLoginAsync(
		[FromServices] IMessageSender messageSender,
		[FromBody] AGLoginQuery source)
	{
		var reply = await messageSender.RequestAsync<AGLoginRequest, AGLoginReply>(
			subject: "gp.gameinterface.cr.aglogin",
			data: new AGLoginRequest
			{
				ApiUrl = source.ApiUrl.ToString(),
				AgentId = source.AgentId,
				SecretKey = source.SecretKey,
				AgentAccount = source.AgentAccount,
				AgentPassword = source.AgentPassword
			},
			cancellationToken: HttpContext.RequestAborted)
			.ConfigureAwait(false);

		return reply.Token;
	}

	/// <summary>
	/// Funds the out asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="query">The query.</param>
	/// <returns></returns>
	[HttpPost("fund/out", Name = "FundOut")]
	public ValueTask<FundResult> FundOutAsync(
		[FromServices] IGetService<FundOutQuery, FundResult> service,
		[FromBody] FundOutQuery query)
		=> service.GetAsync(
			query: query,
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Funds the in asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="query">The query.</param>
	/// <returns></returns>
	[HttpPost("fund/in", Name = "FundIn")]
	public ValueTask<FundResult> FundInAsync(
		[FromServices] IGetService<FundInQuery, FundResult> service,
		[FromBody] FundInQuery query)
		=> service.GetAsync(
			query: query,
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Funds the status asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="query">The query.</param>
	/// <returns></returns>
	[HttpPost("fund/Status", Name = "FundStatus")]
	public ValueTask<bool> FundStatusAsync(
		[FromServices] IGetService<FundStatusQuery, bool> service,
		[FromBody] FundStatusQuery query)
		=> service.GetAsync(
			query: query,
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Players the create asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="command">The command.</param>
	/// <returns></returns>
	[HttpPost("player/create", Name = "PlayerCreate")]
	public ValueTask PlayerCreateAsync(
		[FromServices] ICommandService<PlayerCreateCommand> service,
		[FromBody] PlayerCreateCommand command)
		=> service.ExecuteAsync(
			command: command,
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Players the balance asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpPost("player/balance", Name = "PlayerBalance")]
	public ValueTask<PlayerBalanceResult> PlayerBalanceAsync(
		[FromServices] IGetService<PlayerBalanceQuery, PlayerBalanceResult> service,
		[FromBody] PlayerBalanceQuery source)
		=> service.GetAsync(
			query: source,
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Players the logout asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="command">The command.</param>
	/// <returns></returns>
	[HttpPost("player/logout", Name = "PlayerLogout")]
	public ValueTask PlayerLogoutAsync(
		[FromServices] ICommandService<PlayerLogoutCommand> service,
		PlayerLogoutCommand command)
		=> service.ExecuteAsync(
			command: command,
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Players the online asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="command">The command.</param>
	/// <returns></returns>
	[HttpPost("player/online", Name = "PlayerOnline")]
	public ValueTask<bool> PlayerOnlineAsync(
		[FromServices] IGetService<PlayerOnlineQuery, bool> service,
		PlayerOnlineQuery command)
		=> service.GetAsync(
			query: command,
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Games the URL asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpPost("gameurl", Name = "GameUrl")]
	public ValueTask<GameUrlResult> GameUrlAsync(
		[FromServices] IGetService<GameUrlQuery, GameUrlResult> service,
		[FromBody] GameUrlQuery source)
		=> service.GetAsync(
			query: source,
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Games the record betting asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpPost("game/record/betting", Name = "GameRecordBetting")]
	public IAsyncEnumerable<GameRecordResult> GameRecordBettingAsync(
			[FromServices] IQueryService<GameRecordBettingQuery, GameRecordResult> service,
			GameRecordBettingQuery source)
		=> service.QueryAsync(
			query: source,
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Games the record betting asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	[HttpPost("game/record/settle", Name = "GameRecordSettle")]
	public IAsyncEnumerable<GameRecordResult> GameRecordSettleAsync(
			[FromServices] IQueryService<GameRecordSettleQuery, GameRecordResult> service,
			GameRecordSettleQuery source)
		=> service.QueryAsync(
			query: source,
			cancellationToken: HttpContext.RequestAborted);
}
