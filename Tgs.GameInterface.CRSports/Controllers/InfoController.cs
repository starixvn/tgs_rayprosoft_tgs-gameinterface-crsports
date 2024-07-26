using System.Net;

using System.Net.Sockets;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tgs.GameInterface.CRSports.Core;

namespace Tgs.GameInterface.CRSports.Controllers;

[Route("[controller]")]
[ApiController]
public class InfoController(
	IConfiguration configuration) : ControllerBase
{
	/// <summary>
	/// Gets the asynchronous.
	/// </summary>
	/// <param name="_hostingEnvironment">The hosting environment.</param>
	/// <param name="gameInterfaceOptions">The game interface options.</param>
	/// <returns></returns>
	[HttpGet]
	public async Task<object> GetAsync(
		[FromServices] IWebHostEnvironment _hostingEnvironment,
		[FromServices] IOptions<GameInterfaceOptions> gameInterfaceOptions)
	{
		var hostName = Dns.GetHostName();
		var hostEntry = await Dns.GetHostEntryAsync(hostName).ConfigureAwait(false);
		var hostIp = Array.Find(hostEntry.AddressList,
			x => x.AddressFamily == AddressFamily.InterNetwork);
		return new
		{
			Data = new
			{
				Environment.MachineName,
				HostName = hostName,
				HostIp = hostIp?.ToString() ?? string.Empty,
				Environment = _hostingEnvironment.EnvironmentName,
				OsVersion = $"{Environment.OSVersion}",
				Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(),
				ProcessCount = Environment.ProcessorCount,
				ReleaseNamespace = configuration.GetValue<string>("RELEASE_NAMESPACE"),
				PodIPAddress = configuration.GetValue<string>("POD_IP"),
				GameInterfaceOptions = gameInterfaceOptions.Value
			}
		};
	}
}
