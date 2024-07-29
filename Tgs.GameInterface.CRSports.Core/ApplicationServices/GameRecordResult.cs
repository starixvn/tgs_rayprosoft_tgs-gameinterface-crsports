namespace Tgs.GameInterface.CRSports.Core.ApplicationServices;
public record GameRecordResult(
	string PlayerCode,
	string ExternalBetId,
	DateTimeOffset ExternalCreatedOn,
	DateTimeOffset ExternalClosedOn,
	Protos.GameRecordStatus Status,
	bool IsCashOut,
	decimal Bet,
	decimal ValidBet,
	decimal Settle,
	decimal WinLoss,
	string AdditionalData);
