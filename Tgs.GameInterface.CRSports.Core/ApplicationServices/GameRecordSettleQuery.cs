using System.ComponentModel;

namespace Tgs.GameInterface.CRSports.Core.ApplicationServices;
public record GameRecordSettleQuery
{
	/// <summary>
	/// The API URL.
	/// </summary>
	[DefaultValue(typeof(Uri), "https://api.orb-6789.com/")]
	public Uri ApiUrl { get; init; } = null!;

	/// <summary>
	/// The agent identifier
	/// </summary>
	[DefaultValue(typeof(int), "3238")]
	public int AgentId { get; init; }

	/// <summary>
	/// The secret key
	/// </summary>
	[DefaultValue(typeof(string), "FnzrUTi5fcsdrws7")]
	public string SecretKey { get; init; } = null!;

	/// <summary>
	/// The agent account
	/// </summary>
	[DefaultValue(typeof(string), "RT706test1")]
	public string AgentAccount { get; init; } = null!;

	/// <summary>
	/// The agent password
	/// </summary>
	[DefaultValue(typeof(string), "aaa123")]
	public string AgentPassword { get; init; } = null!;

	/// <summary>
	/// The start time
	/// </summary>
	[DefaultValue(typeof(string), "2024-07-26T16:30:00+08:00")]
	public DateTimeOffset StartTime { get; set; }

	/// <summary>
	/// The end time
	/// </summary>
	[DefaultValue(typeof(string), "2024-07-26T17:00:00+08:00")]
	public DateTimeOffset EndTime { get; set; }
	/// <summary>
	/// The page
	/// </summary>
	/// <example>1</example>
	[DefaultValue(typeof(int), "1")]
	public int Page { get; set; } = 1;
}
