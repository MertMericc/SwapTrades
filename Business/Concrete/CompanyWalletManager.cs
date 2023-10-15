using Business.Abstract;
using Business.Constants;
using Core.Utilities.Results;
using Dal.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class CompanyWalletManager : ICompanyWalletService
    {
        private ICompanyWalletDal _companyWalletDal;

        public CompanyWalletManager(ICompanyWalletDal companyWalletDal)
        {
            _companyWalletDal = companyWalletDal;
        }

        public IDataResult<CompanyWallet> Add(CompanyWallet wallet)
        {
            try
            {
                if (wallet != null)
                {
                    _companyWalletDal.Add(wallet);

                    return new SuccessDataResult<CompanyWallet>(wallet, Messages.Success);
                }
                return new ErrorDataResult<CompanyWallet>(Messages.WalletCouldNotBeAdded);

            }
            catch (Exception)
            {
                return new ErrorDataResult<CompanyWallet>(Messages.UnknownError);
            }
        }

        public IDataResult<CompanyWallet> Update(CompanyWallet wallet)
        {
            try
            {
                if (wallet != null)
                {
                    _companyWalletDal.Update(wallet);

                    return new SuccessDataResult<CompanyWallet>(wallet, Messages.Success);
                }
                return new ErrorDataResult<CompanyWallet>(Messages.Error);
            }
            catch (Exception)
            {
                return new ErrorDataResult<CompanyWallet>(Messages.UnknownError);
            }
        }

        public IDataResult<CompanyWallet> Get(Expression<Func<CompanyWallet,bool>> filter)
        {
            try
            {
                var wallet = _companyWalletDal.Get(filter);
                if(wallet != null)
                {
                    return new SuccessDataResult<CompanyWallet>(wallet, Messages.Success);
                }
                return new ErrorDataResult<CompanyWallet>(Messages.WalletNotFound);
            }
            catch (Exception)
            {
                return new ErrorDataResult<CompanyWallet>(Messages.UnknownError);
            }
        }
    }
}
