using Core.Entities.Concrete;
using Entities.Dto.Claim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utilities.Security.Jwt
{
    public interface ITokenHelper
    {
        AccessToken CreateToken(User user, List<OperationClaim> operationClaims);
        ClaimListDto GetUserClaimsFromToken(string token);
    }
}
