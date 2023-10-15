using Business.Abstract;
using Business.Constants;
using Business.Constants.Enums;
using Core.Entities;
using Core.Extensions;
using Core.Utilities.Results;
using Core.Utilities.Security.Jwt;
using Dal.Abstract;
using Entities.Concrete;
using Entities.Dto.BuyOrder;
using Entities.Dto.SellOrder;
using Microsoft.AspNetCore.Http;

namespace Business.Concrete
{
    public class BuyOrderManager : IBuyOrderService
    {
        private IBuyOrderDal _buyOrderDal;
        private ISellOrderDal _sellOrderDal;
        private IUserDal _userDal;
        private IWalletService _walletService;
        private ICompanyWalletService _companyWalletService;
        private IOrderStatusService _orderStatusService;
        private IOrderService _orderService;
        private ITokenHelper _tokenHelper;
        public BuyOrderManager(IBuyOrderDal buyOrderDal, ISellOrderDal sellOrderDal, IWalletService walletService, ICompanyWalletService companyWalletService, IOrderService orderService, IOrderStatusService orderStatusService, IUserDal userDal, ITokenHelper tokenHelper)
        {
            _buyOrderDal = buyOrderDal;
            _sellOrderDal = sellOrderDal;
            _walletService = walletService;
            _companyWalletService = companyWalletService;
            _orderService = orderService;
            _orderStatusService = orderStatusService;
            _userDal = userDal;
            _tokenHelper = tokenHelper;
        }


        public IDataResult<BuyOrderCreateDto> Add(BuyOrderCreateDto dto, string token)
        {

            try
            {
                if (dto != null)
                {
                    var parity = _orderService.GetParityById(dto.ParityId);
                    if (parity != null)
                    {
                        var feePrice = (dto.Price * dto.Amount) * parity.Data.FeeRate; //komisyon ücreti
                        var total = (dto.Price * dto.Amount) + feePrice; //satan kişiden çekilecek komisyonlu toplam

                        var buyOrder = new BuyOrder()
                        {
                            UserId = _tokenHelper.GetUserClaimsFromToken(token).Id,
                            Price = dto.Price,
                            Amount = dto.Amount,
                            StatusId = (int)OrderStatuses.IsWaiting,
                            ParityId = dto.ParityId,
                            CreatedDate = DateTime.Now,
                            FeePrice = feePrice,
                        };

                        var soldCoinAmountInUserWallet = _walletService.Get(ba => ba.UserId == buyOrder.UserId && ba.CryptoCurrencyId == parity.Data.SoldCurrencyId).Data.Amount; //satın alım işlemini oluşturan kullanıcının sattığı coinin walletdaki miktarı

                        if (soldCoinAmountInUserWallet < buyOrder.Price)
                        {
                            return new ErrorDataResult<BuyOrderCreateDto>(Messages.NotEnoughBalance);
                        }

                        _buyOrderDal.Add(buyOrder);

                        var result = OperationOfSyncSellOrder(parity.Data, buyOrder, dto, feePrice);

                        if (!result.Success)
                        {
                            return new ErrorDataResult<BuyOrderCreateDto>(Messages.Error);
                        }
                        return new SuccessDataResult<BuyOrderCreateDto>(dto, result.Message);
                    }
                    return new ErrorDataResult<BuyOrderCreateDto>(Messages.ParityNotFound);
                }
                return new ErrorDataResult<BuyOrderCreateDto>(Messages.IncompletedEntry);
            }
            catch (Exception)
            {
                return new ErrorDataResult<BuyOrderCreateDto>(Messages.UnknownError);
            }

        }
        public IResult Delete(int id) //Statüsü cancelled yapılacak.
        {
            try
            {
                var deletedBuyOrder = _buyOrderDal.GetById(id);

                if (deletedBuyOrder != null)
                {
                    deletedBuyOrder.StatusId = (int)OrderStatuses.IsCancelled;
                    _buyOrderDal.Update(deletedBuyOrder);
                    return new SuccessResult(Messages.Success);
                }
                return new ErrorResult(Messages.OrderNotFound);
            }
            catch (Exception)
            {
                return new ErrorResult(Messages.UnknownError);
            }
        }

        public IDataResult<List<BuyOrderListDto>> GetAllBuyOrder()
        {
            try
            {
                var buyOrderList = _buyOrderDal.GetBuyOrders();

                if (buyOrderList != null)
                {
                    return new SuccessDataResult<List<BuyOrderListDto>>(buyOrderList, Messages.Success);
                }
                return new ErrorDataResult<List<BuyOrderListDto>>(Messages.OrderListNotFound);
            }
            catch (Exception)
            {
                return new ErrorDataResult<List<BuyOrderListDto>>(Messages.UnknownError);
            }

        }

        public IDataResult<List<BuyOrderListDto>> GetBuyOrderByUserId(int userId)
        {
            try
            {
                var buyOrderList = _buyOrderDal.GetBuyOrdersByUserId(userId);

                if (buyOrderList != null)
                {
                    return new SuccessDataResult<List<BuyOrderListDto>>(buyOrderList, Messages.Success);
                }
                return new ErrorDataResult<List<BuyOrderListDto>>(Messages.OrderListNotFound);
            }
            catch (Exception)
            {
                return new ErrorDataResult<List<BuyOrderListDto>>(Messages.UnknownError);
            }

        }

        public IDataResult<List<BuyOrderListDto>> GetUserBuyHistory(string token)
        {
            try
            {
                var buyOrders = _buyOrderDal.GetList(x => x.UserId == _tokenHelper.GetUserClaimsFromToken(token).Id);
                if (buyOrders.Count > 0)
                {
                    var dto = new List<BuyOrderListDto>();

                    foreach (var item in buyOrders)
                    {
                        var user = _userDal.GetById(item.UserId);

                        var parity = _orderService.GetParityDtoById(item.ParityId);

                        var totalPrice = (item.Price * item.Amount) + item.FeePrice;

                        dto.Add(new BuyOrderListDto()
                        {
                            Id = item.Id,
                            UserName = (user.Name + " " + user.Surname).ToString(),
                            Price = item.Price,
                            Amount = item.Amount,
                            FeePrice = item.FeePrice,
                            TotalPrice = totalPrice,
                            ParityName = $"{parity.Data.ReceivedCurrenyName}/{parity.Data.SoldCurrencyName}",
                            StatusName = _orderStatusService.GetById(item.StatusId).Data.StatusName,
                            CreatedDate = item.CreatedDate,
                            BoughtDate = item.BoughtDate
                        }) ;
                    }
                    return new SuccessDataResult<List<BuyOrderListDto>>(dto, Messages.Success);
                }
                return new ErrorDataResult<List<BuyOrderListDto>>(Messages.OrderListNotFound);

            }
            catch (Exception)
            {
                return new ErrorDataResult<List<BuyOrderListDto>>(Messages.UnknownError);
            }
        }
        #region Helper
        private IResult OperationOfSyncSellOrder(Parity parity, BuyOrder buyOrder, BuyOrderCreateDto dto, decimal feePrice)
        {
            //Statüsü is waiting olan, pariteleri, amount ve priceları aynı olan sell orderları getiriyoruz.

            #region Seller Al Sat Durumu
            var sellOrders = _sellOrderDal.GetList(x => x.StatusId == (int)OrderStatuses.IsWaiting && buyOrder.Amount == x.Amount && buyOrder.Price == x.Price && buyOrder.ParityId == x.ParityId);


            if (sellOrders.Count == 0)
            {
                return new SuccessDataResult<BuyOrderCreateDto>(dto, "Havuza gönderiliyor, eşleşen satış emri bulunamadı");

            }
            sellOrders = sellOrders.OrderBy(x => x.CreatedDate).ToList();
            var matchedSellOrder = sellOrders.First();

            var counterCurrencyId = parity.SoldCurrencyId; //TRY Id'si
            var baseCurrencyId = parity.ReceivedCurrencyId;

            var sellerUserTRYWallet = _walletService.Get(x => x.CryptoCurrencyId == counterCurrencyId && x.UserId == matchedSellOrder.UserId); //Satıcının TRY cüzdanı
            var sellerUserBTCWallet = _walletService.Get(x => x.CryptoCurrencyId == baseCurrencyId && x.UserId == matchedSellOrder.UserId);//Satıcının BTC cüzdanı

            var sellerTRYAmount = sellerUserTRYWallet.Data.Amount; //Satıcının TRY Amountu
            var sellerBTCAmount = sellerUserBTCWallet.Data.Amount; //Satıcının BTC Amountu

            if (sellerBTCAmount < buyOrder.Amount) //Eğer satıcıdan çekilecek miktardan az amount var ise hata
            {
                return new ErrorDataResult<BuyOrderCreateDto>(Messages.NotEnoughBalance);
            }

            //sellerın sattığı cryptoyu wallettaki amount'dan düş (satıcı TRY kazanıp BTC kaybedecek)

            sellerBTCAmount -= buyOrder.Amount;

            //sellerın aldığı cryptoyu wallettaki amount'a ekle(satıcı TRY kazanıp BTC kaybedecek)

            sellerTRYAmount = (sellerTRYAmount + (buyOrder.Amount * buyOrder.Price) - feePrice);

            sellerUserTRYWallet.Data.Amount = sellerTRYAmount;
            sellerUserBTCWallet.Data.Amount = sellerBTCAmount;

            _walletService.Update(sellerUserTRYWallet.Data);
            _walletService.Update(sellerUserBTCWallet.Data);
            #endregion

            #region Buyer Al Sat Durumu
            var buyerUserBTCWallet = _walletService.Get(x => x.UserId == buyOrder.UserId && x.CryptoCurrencyId == baseCurrencyId);
            var buyerUserTRYWallet = _walletService.Get(x => x.UserId == buyOrder.UserId && x.CryptoCurrencyId == counterCurrencyId);

            var buyerBTCAmount = buyerUserBTCWallet.Data.Amount;
            var buyerTRYAmount = buyerUserTRYWallet.Data.Amount;


            //buyerın aldığı cryptoyu BTC amount'una ekle

            buyerBTCAmount += buyOrder.Amount;
            buyerTRYAmount -= (buyOrder.Amount * buyOrder.Price) + feePrice;

            //buyerın sattığı cryptoyu BTC amount'una ekle

            buyerUserBTCWallet.Data.Amount = buyerBTCAmount;
            buyerUserTRYWallet.Data.Amount = buyerTRYAmount;

            _walletService.Update(buyerUserBTCWallet.Data);
            _walletService.Update(buyerUserTRYWallet.Data);

            //orderların statülerini completed yap

            matchedSellOrder.StatusId = (int)OrderStatuses.IsCompleted;
            buyOrder.StatusId = (int)OrderStatuses.IsCompleted;


            var date = DateTime.Now;
            buyOrder.SellerId = matchedSellOrder.UserId;
            buyOrder.BoughtDate = date;

            _buyOrderDal.Update(buyOrder);

            #endregion

            matchedSellOrder.BuyerId = buyOrder.UserId;
            matchedSellOrder.SoldDate = date;
            _sellOrderDal.Update(matchedSellOrder);


            var soldCurrencyCompanyWallet = _companyWalletService.Get(x => x.CryptoCurrencyId == parity.SoldCurrencyId);
            soldCurrencyCompanyWallet.Data.Amount += feePrice * 2;
            _companyWalletService.Update(soldCurrencyCompanyWallet.Data);

            return new SuccessDataResult<BuyOrderCreateDto>(dto, Messages.Success);
        }
        #endregion
    }
}
