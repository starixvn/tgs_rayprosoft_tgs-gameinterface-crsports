using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgs.GameInterface.CRSports.Core;
public interface ITokenUtensil
{
	public ValueTask<string> GetTokenAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, CancellationToken cancellationToken = default);
	public ValueTask DeleteTokenAsync(Uri apiUrl, int agentId, CancellationToken cancellationToken = default);
}
