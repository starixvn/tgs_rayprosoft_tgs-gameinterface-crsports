using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgs.GameInterface.CRSports.Core;
public record PolicySettingsOptions
{
	/// <summary>
	/// The global retry count
	/// </summary>
	public int GlobalRetryCount { get; init; }
	/// <summary>
	/// The global retry wait sec
	/// </summary>
	public int GlobalRetryWaitSec { get; init; }
}
