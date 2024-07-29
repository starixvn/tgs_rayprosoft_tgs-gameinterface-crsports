using System.Diagnostics;
using Tgs.GameInterface.CRSports.Client;

namespace Tgs.GameInterface.CRSports.Client;

public class DiagnosticsInterceptorOptions
{
	public ActivitySource InterceptorActivitySource { get; set; } = new ActivitySource(typeof(DiagnosticsInterceptorOptions).FullName!);
}
