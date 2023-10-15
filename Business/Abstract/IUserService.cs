using Core.Entities.Concrete;
using Core.Utilities.Results;
using Entities.Dto.OperationClaim;
using Entities.Dto.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IUserService
    {
        IDataResult<List<UserListDto>> GetList(Expression<Func<User, bool>> filter = null);
        IDataResult<UserListDto> GetDto(Expression<Func<User, bool>> filter);
        IDataResult<User> Get(Expression<Func<User, bool>> filter);
        List<OperationClaimListDto> GetRolesDto(User user);
        IDataResult<UserListDto> GetDetails(string token);
        IDataResult<User> AddWithRole(User user);
        IDataResult<User> Update(UserUpdateDto dto);
        IResult Delete(int id);
        IDataResult<UserListDto> GetById(int id);
        IDataResult<UserHistoryDto> GetUserHistoryById(int id);
        IDataResult<List<OperationClaim>> GetClaims(User user);
    }
}
