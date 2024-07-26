namespace Tgs.GameInterface.CRSports.Core.Models;
public record WagerDataResp(
	string Id,
	string Username,
	string Mid,
	string Gtype,
	DateTimeOffset Adddate,
	DateTimeOffset Resultdate,
	string Settle,
	string Result,
	decimal Gold,
	decimal MembersVgold,
	decimal Wingold,
	string CashoutId,
	string AdditionalData);
