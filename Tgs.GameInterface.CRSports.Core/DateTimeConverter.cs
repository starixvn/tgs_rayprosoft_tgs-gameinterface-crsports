namespace Tgs.GameInterface.GlobalTechnology.Core;

internal class DateTimeConverter
{
	private readonly TimeZoneInfo _timeZone = TimeZoneInfo.CreateCustomTimeZone(id: "eastern time", baseUtcOffset: TimeSpan.FromHours(-4), "eastern time", "eastern time");

	private readonly string _toStringformat = "yyyy-MM-dd HH:mm:ss";

	public string DatetimeConvertToString(DateTimeOffset time)
		=> TimeZoneInfo.ConvertTime(time.ToUniversalTime(), _timeZone).ToString(_toStringformat);

	public DateTimeOffset StringConvertToDatetime(string str)
		=> DateTime.TryParse(str, out var date) ? new(date, _timeZone.GetUtcOffset(date)) : DateTimeOffset.UnixEpoch;
}
