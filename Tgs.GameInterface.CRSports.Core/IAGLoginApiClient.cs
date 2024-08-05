using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgs.GameInterface.CRSports.Core;
public interface IAGLoginApiClient
{
	public ValueTask<string> AGLoginAsync(Uri apiUrl, int agentId, string secretKey, string agentAccount, string agentPassword, CancellationToken cancellationToken);
}
