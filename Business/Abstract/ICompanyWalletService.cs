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
    public interface ICompanyWalletService
    {
        IDataResult<CompanyWallet> Add(CompanyWallet wallet);
        IDataResult<CompanyWallet> Update(CompanyWallet wallet);
        IDataResult<CompanyWallet> Get(Expression<Func<CompanyWallet, bool>> filter);
    }
}
