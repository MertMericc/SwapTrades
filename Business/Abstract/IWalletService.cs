using Core.Utilities.Results;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IWalletService
    {
        IDataResult<Wallet> Create(Wallet wallet);
        IDataResult<Wallet> Update(Wallet wallet);
        //IDataResult<Wallet> GetList(Wallet wallet);
        IDataResult<List<Wallet>> GetUserWalletList(string token);
        IResult Delete(int id);
        IDataResult<Wallet> Get(Expression<Func<Wallet, bool>> filter);
        IDataResult<List<Wallet>> GetWalletsByUserId(int id);
    }
}
