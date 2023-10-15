using Core.DataAccess.EntityFramework;
using Core.Entities.Concrete;
using Dal.Abstract;
using Dal.Concrete.Context;
using Entities.Dto.OperationClaim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal.Concrete.EntityFramework
{
    public class EfUserDal : EfEntityRepositoryBase<User, SwapDbContext>, IUserDal
    {
        public List<OperationClaim> GetClaims(User user)
        {
            using(var context = new SwapDbContext())
            {
                var result = from operationClaim in context.OperationClaims
                             join userOperationClaim in context.UserOperationClaims
                             on operationClaim.Id equals userOperationClaim.OperationClaimId
                             where userOperationClaim.UserId == user.Id
                             select new OperationClaim {Id = operationClaim.Id, Name = operationClaim.Name };

                return result.ToList();
            }
        }
    }
}
