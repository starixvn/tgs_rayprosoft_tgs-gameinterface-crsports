using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgs.GameInterface.CRSports.Core.Models;
public record ChkMemberBalanceResp(
	bool IsPlayerExist,
	decimal Balance);