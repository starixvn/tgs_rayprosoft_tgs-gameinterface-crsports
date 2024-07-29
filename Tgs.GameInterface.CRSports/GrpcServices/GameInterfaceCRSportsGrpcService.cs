using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Tgs.GameInterface.CRSports.Core.ApplicationServices;
using Tgs.GameInterface.CRSports.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.CRSports.GrpcServices;

public class GameInterfaceCRSportsGrpcService(
	ILogger<GameInterfaceCRSportsGrpcService> logger,
	IGetService<FundInQuery, FundResult> fundInService,
	IGetService<FundOutQuery, FundResult> fundOutService,
	IGetService<FundStatusQuery, bool> fundStatusService,
	IQueryService<GameRecordBettingQuery, GameRecordResult> gameRecordBettingService,
	IQueryService<GameRecordSettleQuery, GameRecordResult> gameRecordSettleService,
	IGetService<GameUrlQuery, GameUrlResult> gameUrlService,
	IGetService<PlayerBalanceQuery, PlayerBalanceResult> playerBalanceService,
	ICommandService<PlayerCreateCommand> playerCreateService,
	ICommandService<PlayerLogoutCommand> playerLogoutService,
	IGetService<PlayerOnlineQuery, bool> playerOnlineService
	) : GameInterfaceCRSports.GameInterfaceCRSportsBase
{
	public override Task<CheckingServiceReply> CheckingService(CheckingServiceRequest request, ServerCallContext context)
	{
		logger.LogWarning("LogOn:{logOn} Message: Create a check connection.({machineName})", DateTime.Now.ToString("o"), request.MachineName);

		return Task.FromResult(new CheckingServiceReply
		{
			MachineName = Environment.MachineName,
		});
	}

	public override async Task<FundReply> FundIn(FundRequest request, ServerCallContext context)
	{
		var query = new FundInQuery
		{
			ApiUrl = new Uri(request.ApiUrl),
			AgentId = request.AgentId,
			SecretKey = request.SecretKey,
			AgentAccount = request.AgentAccount,
			AgentPassword = request.AgentPassword,
			PlayerCode = request.PlayerCode,
			OrderId = request.OrderId,
			Amount = request.Amount
		};

		var result = await fundInService.GetAsync(
			query: query,
			cancellationToken: context.CancellationToken)
			.ConfigureAwait(false);

		return new FundReply
		{
			OrderId = result.OrderId,
			ExternalOrderId = result.ExternalOrderId,
			Resp = result.Resp,
		};
	}

	public override async Task<FundReply> FundOut(FundRequest request, ServerCallContext context)
	{
		var query = new FundOutQuery
		{
			ApiUrl = new Uri(request.ApiUrl),
			AgentId = request.AgentId,
			SecretKey = request.SecretKey,
			AgentAccount = request.AgentAccount,
			AgentPassword = request.AgentPassword,
			PlayerCode = request.PlayerCode,
			OrderId = request.OrderId,
			Amount = request.Amount
		};

		var result = await fundOutService.GetAsync(
			query: query,
			cancellationToken: context.CancellationToken)
			.ConfigureAwait(false);

		return new FundReply
		{
			OrderId = result.OrderId,
			ExternalOrderId = result.ExternalOrderId,
			Resp = result.Resp,
		};
	}

	public override async Task<BaseReply> FundStatus(FundStatusRequest request, ServerCallContext context)
	{
		var query = new FundStatusQuery
		{
			ApiUrl = new Uri(request.ApiUrl),
			AgentId = request.AgentId,
			SecretKey = request.SecretKey,
			AgentAccount = request.AgentAccount,
			AgentPassword = request.AgentPassword,
			PlayerCode = request.PlayerCode,
			OrderId = request.OrderId
		};

		var result = await fundStatusService.GetAsync(
			query: query,
			cancellationToken: context.CancellationToken)
			.ConfigureAwait(false);

		return new BaseReply
		{
			Status = result
		};
	}

	public override async Task<GameRecordReply> GameRecordBetting(GameRecordRequest request, ServerCallContext context)
	{
		var query = new GameRecordBettingQuery
		{
			ApiUrl = new Uri(request.ApiUrl),
			AgentId = request.AgentId,
			SecretKey = request.SecretKey,
			AgentAccount = request.AgentAccount,
			AgentPassword = request.AgentPassword,
			StartTime = request.StartTime.ToDateTimeOffset(),
			EndTime = request.EndTime.ToDateTimeOffset(),
			Page = request.Page
		};

		var results = gameRecordBettingService.QueryAsync(
			query: query,
			cancellationToken: context.CancellationToken);

		var reply = new GameRecordReply();

		await foreach (var result in results)
		{
			reply.Items.Add(new GameRecordInfo
			{
				PlayerCode = result.PlayerCode,
				ExternalBetId = result.ExternalBetId,
				ExternalCreatedOn = result.ExternalCreatedOn.ToTimestamp(),
				ExternalClosedOn = result.ExternalClosedOn.ToTimestamp(),
				Status = result.Status,
				IsCashOut = result.IsCashOut,
				Bet = result.Bet,
				ValidBet = result.ValidBet,
				Settle = result.Settle,
				WinLoss = result.WinLoss,
				AdditionalData = result.AdditionalData
			});
		}

		return reply;
	}

	public override async Task<GameRecordReply> GameRecordSettle(GameRecordRequest request, ServerCallContext context)
	{
		var query = new GameRecordSettleQuery
		{
			ApiUrl = new Uri(request.ApiUrl),
			AgentId = request.AgentId,
			SecretKey = request.SecretKey,
			AgentAccount = request.AgentAccount,
			AgentPassword = request.AgentPassword,
			StartTime = request.StartTime.ToDateTimeOffset(),
			EndTime = request.EndTime.ToDateTimeOffset(),
			Page = request.Page
		};

		var results = gameRecordSettleService.QueryAsync(
			query: query,
			cancellationToken: context.CancellationToken);

		var reply = new GameRecordReply();

		await foreach (var result in results)
		{
			reply.Items.Add(new GameRecordInfo
			{
				PlayerCode = result.PlayerCode,
				ExternalBetId = result.ExternalBetId,
				ExternalCreatedOn = result.ExternalCreatedOn.ToTimestamp(),
				ExternalClosedOn = result.ExternalClosedOn.ToTimestamp(),
				Status = result.Status,
				IsCashOut = result.IsCashOut,
				Bet = result.Bet,
				ValidBet = result.ValidBet,
				Settle = result.Settle,
				WinLoss = result.WinLoss,
				AdditionalData = result.AdditionalData
			});
		}

		return reply;
	}

	public override async Task<GameUrlReply> GameUrl(GameUrlRequest request, ServerCallContext context)
	{
		var query = new GameUrlQuery
		{
			ApiUrl = new Uri(request.ApiUrl),
			AgentId = request.AgentId,
			SecretKey = request.SecretKey,
			AgentAccount = request.AgentAccount,
			AgentPassword = request.AgentPassword,
			PlayerCode = request.PlayerCode,
			Currency = request.Currency,
			Language = request.Language,
			IpAddress = request.IpAddress,
			IsMobile = request.IsMobile
		};

		var result = await gameUrlService.GetAsync(
			query: query,
			cancellationToken: context.CancellationToken)
			.ConfigureAwait(false);

		return new GameUrlReply
		{
			IsPlayerExist = result.IsPlayerExist,
			GameUrl = result.GameUrl
		};
	}

	public override async Task<PlayerBalanceReply> PlayerBalance(PlayerBalanceRequest request, ServerCallContext context)
	{
		var query = new PlayerBalanceQuery
		{
			ApiUrl = new Uri(request.ApiUrl),
			AgentId = request.AgentId,
			SecretKey = request.SecretKey,
			AgentAccount = request.AgentAccount,
			AgentPassword = request.AgentPassword,
			PlayerCode = request.PlayerCode
		};

		var result = await playerBalanceService.GetAsync(
			query: query,
			cancellationToken: context.CancellationToken)
			.ConfigureAwait(false);

		return new PlayerBalanceReply
		{
			IsPlayerExist = result.IsPlayerExist,
			Balance = result.Balance
		};
	}

	public override async Task<BaseReply> PlayerCreate(PlayerCreateRequest request, ServerCallContext context)
	{
		var command = new PlayerCreateCommand
		{
			ApiUrl = new Uri(request.ApiUrl),
			AgentId = request.AgentId,
			SecretKey = request.SecretKey,
			AgentAccount = request.AgentAccount,
			AgentPassword = request.AgentPassword,
			PlayerCode = request.PlayerCode,
			Currency = request.Currency
		};

		await playerCreateService.ExecuteAsync(
		   command: command,
		   cancellationToken: context.CancellationToken)
		   .ConfigureAwait(false);

		return new BaseReply
		{
			Status = true
		};
	}

	public override async Task<BaseReply> PlayerLogout(PlayerLogoutRequest request, ServerCallContext context)
	{
		var command = new PlayerLogoutCommand
		{
			ApiUrl = new Uri(request.ApiUrl),
			AgentId = request.AgentId,
			SecretKey = request.SecretKey,
			AgentAccount = request.AgentAccount,
			AgentPassword = request.AgentPassword,
			PlayerCode = request.PlayerCode
		};

		await playerLogoutService.ExecuteAsync(
		   command: command,
		   cancellationToken: context.CancellationToken)
		   .ConfigureAwait(false);

		return new BaseReply
		{
			Status = true
		};
	}

	public override async Task<PlayerOnlineReply> PlayerOnline(PlayerOnlineRequest request, ServerCallContext context)
	{
		var query = new PlayerOnlineQuery
		{
			ApiUrl = new Uri(request.ApiUrl),
			AgentId = request.AgentId,
			SecretKey = request.SecretKey,
			AgentAccount = request.AgentAccount,
			AgentPassword = request.AgentPassword,
			PlayerCode = request.PlayerCode
		};

		var result = await playerOnlineService.GetAsync(
			query: query,
			cancellationToken: context.CancellationToken)
			.ConfigureAwait(false);

		return new PlayerOnlineReply
		{
			Online = result
		};
	}
}
