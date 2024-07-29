using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgs.GameInterface.CRSports.Core.ApplicationServices;
public record GameUrlQuery
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
	/// The currency
	/// </summary>
	[DefaultValue(typeof(string), "HKD")]
	public string Currency { get; set; } = null!;

	/// <summary>
	/// The is mobile
	/// </summary>
	[DefaultValue(typeof(bool), "false")]
	public bool IsMobile { get; set; }

	/// <summary>
	/// The language
	/// </summary>
	[DefaultValue(typeof(string), "zh-tw")]
	public string Language { get; set; } = null!;

	/// <summary>
	/// The ip address
	/// </summary>
	[DefaultValue(typeof(string), "127.0.0.1")]
	public string IpAddress { get; set; } = null!;
}