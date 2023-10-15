using Business.Abstract;
using Business.Constants;
using Core.Utilities.Results;
using Dal.Abstract;
using Entities.Concrete;
using Entities.Dto.CryptoCurrency;
using System.Linq.Expressions;

namespace Business.Concrete
{
    public class CryptoCurrencyManager : ICryptoCurrencyService
    {
        private ICryptoCurrencyDal _cryptoCurrenciesDal;
        private ISellOrderService _sellOrderService;
        private IBuyOrderService _buyOrderService;
        private IUserDal _userDal;
        private IWalletService _walletService;
        private ICompanyWalletService _companyWalletService;
        public CryptoCurrencyManager(ICryptoCurrencyDal cryptoCurrenciesDal, ISellOrderService sellOrderService, IBuyOrderService buyOrderService, IUserDal userDal, IWalletService walletService, ICompanyWalletService companyWalletService)
        {
            _cryptoCurrenciesDal = cryptoCurrenciesDal;

            _sellOrderService = sellOrderService;
            _buyOrderService = buyOrderService;
            _userDal = userDal;
            _walletService = walletService;
            _companyWalletService = companyWalletService;
        }


        public IDataResult<List<CryptoListDto>> GetListByFilter(Expression<Func<CryptoCurrency, bool>> filter = null)
        {
            try
            {
                var currencyList = _cryptoCurrenciesDal.GetList(filter);

                if (currencyList == null)
                {
                    return new ErrorDataResult<List<CryptoListDto>>(Messages.CryptoListNotFound);
                }
                var dto = new List<CryptoListDto>();

                foreach (var item in currencyList)
                {
                    dto.Add(new CryptoListDto()
                    {
                        Id = item.Id,
                        CurrencyName = item.CurrencyName,
                        CurrencyShortName = item.CurrencyShortName,
                        Status = item.Status
                    });
                }
                return new SuccessDataResult<List<CryptoListDto>>(dto, Messages.CryptoListNotFound);

            }
            catch (Exception)
            {
                return new ErrorDataResult<List<CryptoListDto>>(Messages.UnknownError);

            }
        }



        public IDataResult<CryptoCurrencyHistoryDto> GetHistoryOfCrypto(string cryptoDtoName)
        {
            try
            {
                if (cryptoDtoName != null)
                {

                    var sellOrdersByCrypto = _sellOrderService.GetAllSellOrders().Data.Where(x => x.ParityName.Contains(cryptoDtoName)).ToList();

                    if (sellOrdersByCrypto.Count <= 0)
                    {
                        return new ErrorDataResult<CryptoCurrencyHistoryDto>(Messages.OrderListNotFound);
                    }

                    var buyOrdersByCrypto = _buyOrderService.GetAllBuyOrder().Data.Where(x => x.ParityName.Contains(cryptoDtoName)).ToList();

                    if (buyOrdersByCrypto.Count <= 0)
                    {
                        return new ErrorDataResult<CryptoCurrencyHistoryDto>(Messages.OrderListNotFound);
                    }


                    var dto = new CryptoCurrencyHistoryDto()
                    {
                        CryptoName = cryptoDtoName,
                        SellOrderListDtos = sellOrdersByCrypto,
                        BuyOrderListDto = buyOrdersByCrypto
                    };

                    return new SuccessDataResult<CryptoCurrencyHistoryDto>(dto, Messages.Success);

                }
                return new ErrorDataResult<CryptoCurrencyHistoryDto>(Messages.CryptoNotFound);

            }
            catch (Exception)
            {

                return new ErrorDataResult<CryptoCurrencyHistoryDto>(Messages.UnknownError);
            }

        }


        public IResult AddWalletToUser(int cryptoCurrencyId)
        {

            var users = _userDal.GetList();

            if (users.Count > 0)
            {
                foreach (var item in users)
                {
                    _walletService.Create(new Wallet
                    {
                        UserId = item.Id,
                        CryptoCurrencyId = cryptoCurrencyId,
                        Amount = 0
                    });
                }
                return new SuccessResult(Messages.Success);
            }
            return new ErrorResult(Messages.Error);
        }


        public IResult AddWalletToCompany(int cryptoCurrencyId)
        {

            _companyWalletService.Add(new CompanyWallet
            {
                CryptoCurrencyId = cryptoCurrencyId,
                Amount = 0
            });

            return new SuccessResult(Messages.Success);

        }


        public IDataResult<CryptoCurrency> Add(CryptoAddDto cryptoDto)
        {
            try
            {
                if (cryptoDto != null)
                {
                    var crypto = new CryptoCurrency()
                    {
                        CurrencyName = cryptoDto.CurrencyName,
                        CurrencyShortName = cryptoDto.CurrencyShortName,
                        Status = cryptoDto.Status,
                    };
                    _cryptoCurrenciesDal.Add(crypto);

                    var resultUser = AddWalletToUser(crypto.Id);
                    var resultCompany = AddWalletToCompany(crypto.Id);

                    if (resultCompany.Success && resultUser.Success)
                    {
                        return new SuccessDataResult<CryptoCurrency>(crypto, Messages.Success);
                    }
                    return new ErrorDataResult<CryptoCurrency>(Messages.WalletCouldNotBeAdded);
                }
                return new ErrorDataResult<CryptoCurrency>(Messages.IncompletedEntry);

            }
            catch (Exception)
            {
                return new ErrorDataResult<CryptoCurrency>(Messages.Error);
            }
        }

        public IResult Delete(int cryptoCurrencyId)
        {
            try
            {
                var deletedCrypto = _cryptoCurrenciesDal.Get(x => x.Id == cryptoCurrencyId);
                if (deletedCrypto != null)
                {
                    deletedCrypto.Status = false;
                    _cryptoCurrenciesDal.Update(deletedCrypto);
                    return new SuccessResult(Messages.Success);
                }
                return new ErrorResult(Messages.UserNotFound);

            }
            catch (Exception)
            {
                return new ErrorResult(Messages.UnknownError);
            }

        }

        public IDataResult<CryptoCurrency> GetById(int cryptoCurrencyId)
        {
            try
            {
                var result = _cryptoCurrenciesDal.Get(filter: p => p.Id == cryptoCurrencyId);

                if (result != null)
                {
                    return new SuccessDataResult<CryptoCurrency>(result, Messages.Success);
                }

                return new ErrorDataResult<CryptoCurrency>(Messages.CryptoNotFound);
            }
            catch (Exception)
            {
                return new ErrorDataResult<CryptoCurrency>(Messages.UnknownError);
            }
        }
        public IDataResult<List<CryptoListDto>> GetActiveList()
        {
            try
            {
                var datas = _cryptoCurrenciesDal.GetList(x => x.Status);

                if (datas == null)
                {
                    return new ErrorDataResult<List<CryptoListDto>>(Messages.CryptoListNotFound);
                }

                var listDto = new List<CryptoListDto>();
                foreach (var item in datas)
                {
                    listDto.Add(new CryptoListDto()
                    {
                        Id = item.Id,
                        CurrencyName = item.CurrencyName,
                        CurrencyShortName = item.CurrencyShortName,
                        Status = item.Status
                    });
                }
                return new SuccessDataResult<List<CryptoListDto>>(listDto);
            }
            catch (Exception)
            {
                return new ErrorDataResult<List<CryptoListDto>>(Messages.UnknownError);
            }
        }
        public IDataResult<List<CryptoListDto>> GetList()
        {
            try
            {
                var datas = _cryptoCurrenciesDal.GetList();

                if (datas == null)
                {
                    return new ErrorDataResult<List<CryptoListDto>>(Messages.CryptoListNotFound);
                }

                var listDto = new List<CryptoListDto>();
                foreach (var item in datas)
                {
                    listDto.Add(new CryptoListDto()
                    {
                        Id = item.Id,
                        CurrencyName = item.CurrencyName,
                        CurrencyShortName = item.CurrencyShortName,
                        Status = item.Status
                    });
                }
                return new SuccessDataResult<List<CryptoListDto>>(listDto);
            }
            catch (Exception)
            {
                return new ErrorDataResult<List<CryptoListDto>>(Messages.UnknownError);
            }

        }

        public IDataResult<CryptoCurrency> SetStatus(int id)
        {
            try
            {
                var cryptoToSetStatus = _cryptoCurrenciesDal.Get(x => x.Id == id);
                if (cryptoToSetStatus != null)
                {
                    cryptoToSetStatus.Status = !cryptoToSetStatus.Status;

                    _cryptoCurrenciesDal.Update(cryptoToSetStatus);

                    return new SuccessDataResult<CryptoCurrency>(cryptoToSetStatus, Messages.Success);
                }
                return new ErrorDataResult<CryptoCurrency>(Messages.CryptoNotFound);

            }
            catch (Exception)
            {
                return new ErrorDataResult<CryptoCurrency>(Messages.UnknownError);
            }
        }

        public IDataResult<CryptoCurrency> Update(CryptoUpdateDto cryptoCurrencyUpdateDto)
        {
            try
            {
                if (cryptoCurrencyUpdateDto == null)
                {
                    return new ErrorDataResult<CryptoCurrency>(Messages.IncompletedEntry);

                }
                var crypto = new CryptoCurrency()
                {
                    Id = cryptoCurrencyUpdateDto.Id,
                    CurrencyName = cryptoCurrencyUpdateDto.CurrencyName,
                    Status = cryptoCurrencyUpdateDto.Status,
                    CurrencyShortName = cryptoCurrencyUpdateDto.CurrencyShortName,
                };

                _cryptoCurrenciesDal.Update(crypto);
                return new SuccessDataResult<CryptoCurrency>(crypto, Messages.Success);

            }
            catch (Exception)
            {
                return new ErrorDataResult<CryptoCurrency>(Messages.UnknownError);
            }

        }
    }
}
