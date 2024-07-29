using System.Diagnostics;
using Tgs.GameInterface.CRSports;

namespace Tgs.GameInterface.CRSports;

public class DiagnosticsInterceptorOptions
{
	public ActivitySource InterceptorActivitySource { get; set; } = new ActivitySource(typeof(DiagnosticsInterceptorOptions).FullName!);
}
