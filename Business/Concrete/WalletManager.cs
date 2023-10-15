using Business.Abstract;
using Business.Constants;
using Core.Extensions;
using Core.Utilities.Results;
using Core.Utilities.Security.Jwt;
using Dal.Abstract;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class WalletManager : IWalletService
    {

        private IWalletDal _walletDal;
        private ITokenHelper _tokenHelper;

        public WalletManager(IWalletDal walletDal, ITokenHelper tokenHelper)
        {
            _walletDal = walletDal;
            _tokenHelper = tokenHelper;
        }

        public IDataResult<Wallet> Create(Wallet wallet)
        {
            try
            {
                //Diğer managerlara da bu kontrolü ekle
                var result = _walletDal.GetById(wallet.Id);

                if (result == null)
                {

                    _walletDal.Add(wallet);
                    //_walletDal.SaveChanges();

                    return new SuccessDataResult<Wallet>(wallet, Messages.Success);
                }
                return new ErrorDataResult<Wallet>(Messages.WalletAlreadyExists);

            }
            catch (Exception)
            {
                return new ErrorDataResult<Wallet>(Messages.UnknownError);
            }
        }

        public IResult Delete(int id)
        {
            try
            {
                var wallet = _walletDal.GetById(id);
                if (wallet != null)
                {
                    wallet.Status = false;
                    _walletDal.Update(wallet);
                    return new SuccessResult(Messages.Success);
                }
                return new ErrorResult(Messages.WalletCouldNotBeDeleted);

            }
            catch (Exception)
            {
                return new ErrorResult(Messages.UnknownError);
            }
        }

        public IDataResult<List<Wallet>> GetUserWalletList(string token)
        {
            try
            {
                var walletList = _walletDal.GetList(x => x.UserId == _tokenHelper.GetUserClaimsFromToken(token).Id && x.Status == true);
                if (walletList.Count > 0)
                {
                    return new SuccessDataResult<List<Wallet>>(walletList.ToList(), Messages.Success);
                }
                return new ErrorDataResult<List<Wallet>>(Messages.WalletListNotFound);

            }
            catch (Exception)
            {
                return new ErrorDataResult<List<Wallet>>(Messages.UnknownError);
            }
        }

        public IDataResult<Wallet> Get(Expression<Func<Wallet, bool>> filter)
        {
            try
            {
                var result = _walletDal.Get(filter);
                if (result != null)
                {
                    return new SuccessDataResult<Wallet>(result, Messages.Success);
                }
                return new ErrorDataResult<Wallet>(Messages.WalletNotFound);
            }
            catch (Exception)
            {
                return new ErrorDataResult<Wallet>(Messages.UnknownError);
            }
        }

      

       

        public IDataResult<Wallet> Update(Wallet wallet)
        {
            try
            {
                if (wallet != null)
                {
                    _walletDal.Update(wallet);
                    _walletDal.SaveChanges();
                    return new SuccessDataResult<Wallet>(wallet, Messages.Success);
                }
                return new ErrorDataResult<Wallet>(Messages.WalletNotFound);
            }
            catch (Exception)
            {
                return new ErrorDataResult<Wallet>(Messages.UnknownError);
            }
        }
  
        public IDataResult<List<Wallet>> GetWalletsByUserId(int id)
        {
            try
            {
                var userWalletList = _walletDal.GetList(x => x.UserId == id && x.Status).ToList();
                if (userWalletList.Count > 0)
                {
                    return new SuccessDataResult<List<Wallet>>(userWalletList, Messages.Success);
                }
                return new ErrorDataResult<List<Wallet>>(Messages.WalletListNotFound);
            }
            catch (Exception)
            {
                return new ErrorDataResult<List<Wallet>>(Messages.UnknownError);
            }
        }
    }
}
