using Business.Abstract;
using Business.Constants;
using Business.Constants.Enums;
using Core.Extensions;
using Core.Utilities.Results;
using Core.Utilities.Security.Jwt;
using Dal.Abstract;
using Entities.Concrete;
using Entities.Dto.SellOrder;
using Microsoft.AspNetCore.Http;

namespace Business.Concrete
{
    public class SellOrderManager : ISellOrderService
    {
        private ISellOrderDal _sellOrderDal;
        private IBuyOrderDal _buyOrderDal;
        private IUserDal _userDal;
        private IOrderStatusService _orderStatusService;
        private IWalletService _walletService;
        private ICompanyWalletService _companyWalletService;
        private IOrderService _orderService;
        private ITokenHelper _tokenHelper;
        public SellOrderManager(ISellOrderDal sellOrderDal, IBuyOrderDal buyOrderDal, IOrderStatusService orderStatusService, IWalletService walletService, ICompanyWalletService companyWalletService, IUserDal userDal, IOrderService orderService, ITokenHelper tokenHelper)
        {
            _sellOrderDal = sellOrderDal;
            _buyOrderDal = buyOrderDal;
            _orderStatusService = orderStatusService;
            _walletService = walletService;
            _companyWalletService = companyWalletService;
            _userDal = userDal;
            _orderService = orderService;
            _tokenHelper = tokenHelper;
        }

        public IDataResult<SellOrderCreateDto> Add(SellOrderCreateDto dto, string token)
        {
            try
            {
                if (dto != null)
                {
                    var parity = _orderService.GetParityById(dto.ParityId).Data;
                    var sellOrder = new SellOrder()
                    {
                        UserId = _tokenHelper.GetUserClaimsFromToken(token).Id,
                        Price = dto.Price,
                        Amount = dto.Amount,
                        FeePrice = (dto.Price * dto.Amount) * parity.FeeRate,
                        ParityId = dto.ParityId,
                        StatusId = (int)OrderStatuses.IsWaiting,
                        CreatedDate = DateTime.Now,
                    };

                    var soldCoinAmountInUserWallet = _walletService.Get(ba => ba.UserId == sellOrder.UserId && ba.CryptoCurrencyId == parity.ReceivedCurrencyId).Data.Amount; //satın alım işlemini oluşturan kullanıcının sattığı coinin walletdaki miktarı

                    if (soldCoinAmountInUserWallet < sellOrder.Amount)
                    {
                        return new ErrorDataResult<SellOrderCreateDto>(Messages.NotEnoughBalance);
                    }
                    _sellOrderDal.Add(sellOrder);

                    var result = OperationOfSyncBuyOrder(parity, sellOrder); //satış emri eşleşmesi ve eşleştikten sonra gerçekleşen operasyonlar

                    if (!result.Success)
                    {
                        return new ErrorDataResult<SellOrderCreateDto>(Messages.Error);
                    }
                    return new SuccessDataResult<SellOrderCreateDto>(dto, result.Message);
                }
                return new ErrorDataResult<SellOrderCreateDto>(Messages.IncompletedEntry);
            }
            catch (Exception)
            {
                return new ErrorDataResult<SellOrderCreateDto>(Messages.UnknownError);
            }
        }

        public IResult Delete(int id)
        {
            try
            {
                var checkSellOrder = _sellOrderDal.GetById(id);
                if (checkSellOrder != null)
                {
                    checkSellOrder.StatusId = (int)OrderStatuses.IsCancelled;
                    _sellOrderDal.Update(checkSellOrder);
                    return new SuccessResult(Messages.Success);
                }
                return new ErrorResult(Messages.SellOrderNotFound);
            }
            catch (Exception)
            {
                return new ErrorResult(Messages.UnknownError);
            }
        }

        public IDataResult<List<SellOrderListDto>> GetAllSellOrders()
        {
            try
            {
                var sellOrders = _sellOrderDal.GetSellOrders();
                if (sellOrders.Count > 0)
                {
                    return new SuccessDataResult<List<SellOrderListDto>>(sellOrders, Messages.Success);
                }
                return new ErrorDataResult<List<SellOrderListDto>>(Messages.OrderListNotFound);
            }
            catch (Exception)
            {
                return new ErrorDataResult<List<SellOrderListDto>>(Messages.UnknownError);
            }
        }

        public IDataResult<List<SellOrderListDto>> GetSellOrdersByUserId(int userId)
        {
            try
            {
                var sellOrders = _sellOrderDal.GetSellOrdersByUserId(userId);
                if (sellOrders.Count > 0)
                {
                    return new SuccessDataResult<List<SellOrderListDto>>(sellOrders, Messages.Success);
                }
                return new ErrorDataResult<List<SellOrderListDto>>(Messages.OrderListNotFound);
            }
            catch (Exception)
            {
                return new ErrorDataResult<List<SellOrderListDto>>(Messages.UnknownError);
            }
        }

        public IDataResult<List<SellOrderListDto>> GetUserSellHistory(string token)
        {
            try
            {
                var sellOrders = _sellOrderDal.GetList(so => so.UserId == _tokenHelper.GetUserClaimsFromToken(token).Id);
                if (sellOrders.Count > 0)
                {
                    var dto = new List<SellOrderListDto>();
                    foreach (var sellOrder in sellOrders)
                    {
                        var user = _userDal.GetById(sellOrder.UserId);
                        var parity = _orderService.GetParityDtoById(sellOrder.ParityId);
                        dto.Add(new SellOrderListDto
                        {
                            Id = sellOrder.Id,
                            UserName = $"{user.Name} {user.Surname}",
                            Price = sellOrder.Price,
                            Amount = sellOrder.Amount,
                            FeePrice = sellOrder.FeePrice,
                            TotalPrice = (sellOrder.Price * sellOrder.Amount) + sellOrder.FeePrice,
                            ParityName = $"{parity.Data.ReceivedCurrenyName}/{parity.Data.SoldCurrencyName}",
                            StatusName = _orderStatusService.GetById(sellOrder.StatusId).Data.StatusName,
                            CreatedDate = sellOrder.CreatedDate,
                            SoldDate = sellOrder.SoldDate
                        });
                    }
                    return new SuccessDataResult<List<SellOrderListDto>>(dto, Messages.Success);
                }
                return new ErrorDataResult<List<SellOrderListDto>>(Messages.OrderListNotFound);
            }
            catch (Exception)
            {
                return new ErrorDataResult<List<SellOrderListDto>>(Messages.UnknownError);
            }
        }

        #region Helper
        private IResult OperationOfSyncBuyOrder(Parity parity, SellOrder sellOrder)
        {
            var buyOrders = _buyOrderDal.GetList(bo => bo.ParityId == sellOrder.ParityId && bo.Price == sellOrder.Price && bo.StatusId == (int)OrderStatuses.IsWaiting);

            if (buyOrders.Count > 0)
            {
                var firstBuyOrder = buyOrders.OrderBy(fbo => fbo.CreatedDate).First();

                #region Satan için
                //eşleşme başarılı ise satan kullanıcıdan sattığı coin düşülecek ve satın altığı coin aktarılacak. Sonra satma işleminin statusu complete olacak.
                //var walletForSeller = _walletService.GEt(sellOrder.UserId).Data; //kullanıcının walletdaki coinleri
                var soldCoinInWalletForSeller = _walletService.Get(bw => bw.UserId == sellOrder.UserId && bw.CryptoCurrencyId == parity.ReceivedCurrencyId).Data;//kullanıcının walletdaki satacağı coin
                soldCoinInWalletForSeller.Amount = soldCoinInWalletForSeller.Amount - sellOrder.Amount; //sattığı coin miktarı düşürüldü
                _walletService.Update(soldCoinInWalletForSeller);

                var boughtCoinInWalletForSeller = _walletService.Get(tw => tw.UserId == sellOrder.UserId && tw.CryptoCurrencyId == parity.SoldCurrencyId).Data; //kullanıcının walletdaki alacağı coin
                boughtCoinInWalletForSeller.Amount = boughtCoinInWalletForSeller.Amount + (sellOrder.Price - sellOrder.FeePrice); //alacağı coin miktarı arttırıldı
                _walletService.Update(boughtCoinInWalletForSeller);

                var date = DateTime.Now;
                var amount = sellOrder.Amount;
                var amountForSell = sellOrder.Amount - firstBuyOrder.Amount; //satış emrinin miktarından eşleşen alış emrinin miktarı düşürüldü.
                if(amountForSell > 0) //satış emrinin miktarı sıfırdan büyük ise düşürülen miktar açılan yeni satış emrine aktarılacak ve durumu IsCompleted olacak. bu satış emrindende miktar düşülecek durumu isWaiting olarak kalacak.
                {
                    _sellOrderDal.Add(new SellOrder //satış emrinden düşen miktar için isCompleted olarak yeni bir emri oluşturuldu.
                    {
                        UserId = sellOrder.UserId,
                        BuyerId = firstBuyOrder.UserId,
                        Price = sellOrder.Price,
                        Amount = firstBuyOrder.Amount,
                        FeePrice = sellOrder.FeePrice,
                        ParityId = sellOrder.ParityId,
                        StatusId = (int)OrderStatuses.IsCompleted,
                        CreatedDate = date,
                        SoldDate = date
                    });

                    sellOrder.Amount = sellOrder.Amount - firstBuyOrder.Amount;//miktar düşürüldü durum IsWaiting olarak devam etmekte.
                    _sellOrderDal.Update(sellOrder);
                }
                else
                {
                    sellOrder.StatusId = (int)OrderStatuses.IsCompleted;
                    sellOrder.BuyerId = firstBuyOrder.UserId;
                    sellOrder.SoldDate = date;
                    _sellOrderDal.Update(sellOrder);
                }
                #endregion

                #region Satın alan için
                //eşleşme başarılı ise satın alan kullanıcıdan sattığı coin düşülecek ve satın altığı coin aktarılacak. Sonra satın alma işleminin statusu complete olacak.s
                //var walletForBuyer = _walletService.GetUserWalletList(firstBuyOrder.UserId).Data; //satın alma işlemini oluşturan kullanıcının walletdaki coinleri
                var soldCoinInBuyerWallet = _walletService.Get(tw => tw.UserId == firstBuyOrder.UserId && tw.CryptoCurrencyId == parity.SoldCurrencyId).Data; //satın alma işlemini oluşturan kullanıcının satacağı coin
                soldCoinInBuyerWallet.Amount = soldCoinInBuyerWallet.Amount - (sellOrder.Price + sellOrder.FeePrice); //satacağı coin miktarı düşürüldü
                _walletService.Update(soldCoinInBuyerWallet);

                var boughtCoinInBuyerWallet = _walletService.Get(bw => bw.UserId == firstBuyOrder.UserId && bw.CryptoCurrencyId == parity.ReceivedCurrencyId).Data; //satın alma işlemini oluşturan kullanıcının satın alacağı coin
                boughtCoinInBuyerWallet.Amount = boughtCoinInBuyerWallet.Amount + sellOrder.Amount; //satın alacağı coin miktarı arttırıldı
                _walletService.Update(boughtCoinInBuyerWallet);

                var amountForBuy = firstBuyOrder.Amount - amount;
                if (amountForBuy > 0) //eşleşen alış emrindeki miktar sıfırdan büyük ise yeni bir alış emri oluşacak ve düşürülen miktar kadar amount olacak durumu da IsCompleted olacak. Bu alış emri de miktarı düşürülmüş bir şekilde güncellenecek ve durumu isWaiting olarak kalacak.
                {
                    _buyOrderDal.Add(new BuyOrder //düşürülen miktar kadar tamamlandı diye yeni bir emir oluşturuldu.
                    {
                        UserId = firstBuyOrder.UserId,
                        SellerId = sellOrder.UserId,
                        Price = firstBuyOrder.Price,
                        Amount = amount,
                        FeePrice = firstBuyOrder.FeePrice,
                        ParityId = firstBuyOrder.ParityId,
                        StatusId = (int)OrderStatuses.IsCompleted,
                        CreatedDate = firstBuyOrder.CreatedDate,
                        BoughtDate = date
                    });

                    firstBuyOrder.Amount = firstBuyOrder.Amount - sellOrder.Amount;//eşleşen emrin miktarı düşürüldü ve durumu isWaitin olarak devam etmekte
                    _buyOrderDal.Update(firstBuyOrder);
                }
                else
                {
                    firstBuyOrder.StatusId = (int)OrderStatuses.IsCompleted;
                    firstBuyOrder.SellerId = sellOrder.UserId;
                    firstBuyOrder.BoughtDate = date;
                    _buyOrderDal.Update(firstBuyOrder);

                }
                #endregion

                #region Şirket hesabına komisyonu aktarma
                var boughtCurrencyCompanyWallet = _companyWalletService.Get(bcw => bcw.CryptoCurrencyId == parity.SoldCurrencyId).Data;
                boughtCurrencyCompanyWallet.Amount += sellOrder.FeePrice * 2;
                _companyWalletService.Update(boughtCurrencyCompanyWallet);
                #endregion
                
                return new SuccessResult(Messages.Success);
            }

            return new SuccessResult("Eşleşen emir bulunamadı. Satış emri havuza gönderiliyor...");
        }
        #endregion
    }
}
