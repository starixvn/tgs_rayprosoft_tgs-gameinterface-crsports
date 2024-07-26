using System.ComponentModel;

namespace Tgs.GameInterface.CRSports.Core.ApplicationServices;
public record FundInQuery
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
	/// The player code.
	/// </summary>
	[DefaultValue(typeof(string), "4uf6k014jp5x")]
	public string PlayerCode { get; set; } = null!;

	/// <summary>
	/// The order identifier.
	/// </summary>
	[DefaultValue(typeof(string), "29CDE229-AE62-49BA-B03B-2EB7288C0058")]
	public string OrderId { get; set; } = null!;

	/// <summary>
	/// The amount.
	/// </summary>
	[DefaultValue(typeof(decimal), "10.0")]
	public decimal Amount { get; set; }
}
