using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgs.GameInterface.CRSports.Core;
public interface ITokenUtensil
{
	public ValueTask<string> GetTokenAsync(Uri apiBaseUrl, int agentId, string secretKey, string agentAccount, string agentPassword, CancellationToken cancellationToken = default);
	public ValueTask DeleteTokenAsync(Uri apiBaseUrl, int agentId, CancellationToken cancellationToken = default);
}
