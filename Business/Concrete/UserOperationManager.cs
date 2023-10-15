using Business.Abstract;
using Business.Constants;
using Core.Utilities.Results;
using Dal.Abstract;
using Entities.Concrete;
using Entities.Dto.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class UserOperationManager : IUserOperationService
    {
        private ICryptoCurrencyService _cryptoCurrencyService;
        private IWalletService _walletDaService;


        public UserOperationManager(ICryptoCurrencyService cryptoCurrencyService, IWalletService walletService)
        {
            _cryptoCurrencyService = cryptoCurrencyService;
            _walletDaService = walletService;
        }

        public IResult AddCryptoCurrencyForUser(int userId)
        {
            //Wallet'ı oluşturulacak
            var cryptoCurrencies = _cryptoCurrencyService.GetListByFilter(x => x.Status == true);

            foreach (var item in cryptoCurrencies.Data)
            {
                _walletDaService.Create(new Wallet()
                {
                    UserId = userId,
                    CryptoCurrencyId = item.Id,
                    Amount = 0
                });
            }
            return new SuccessResult(Messages.Success);
        }

    }
}
