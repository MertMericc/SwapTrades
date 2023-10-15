using Core.DataAccess.EntityFramework;
using Dal.Abstract;
using Dal.Concrete.Context;
using Entities.Concrete;
using Entities.Dto.BuyOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal.Concrete.EntityFramework
{
    public class EfBuyOrderDal : EfEntityRepositoryBase<BuyOrder, SwapDbContext>, IBuyOrderDal
    {
        public List<BuyOrderListDto> GetBuyOrders()
        {
            using (var context = new SwapDbContext())
            {
                var result = (from buyOrder in context.BuyOrders
                              join status in context.Statuses on buyOrder.StatusId equals status.Id
                              join parity in context.Parities on buyOrder.ParityId equals parity.Id
                              join crypto1 in context.CryptoCurrencies on parity.ReceivedCurrencyId equals crypto1.Id
                              join crypto2 in context.CryptoCurrencies on parity.SoldCurrencyId equals crypto2.Id
                              join user in context.Users on buyOrder.UserId equals user.Id
                              select new BuyOrderListDto
                              {
                                  Id = buyOrder.Id,
                                  Price = buyOrder.Price,
                                  UserName = user.Username,
                                  Amount = buyOrder.Amount,
                                  FeePrice = buyOrder.FeePrice,
                                  TotalPrice = (buyOrder.Price * buyOrder.Amount) + buyOrder.FeePrice,
                                  ParityName = (crypto1.CurrencyShortName + "/" + crypto2.CurrencyShortName),
                                  StatusName = (status.StatusName),
                                  CreatedDate = buyOrder.CreatedDate,
                                  BoughtDate = buyOrder.BoughtDate,
                              }).OrderByDescending(x => x.StatusName == "IsCompleted").ThenByDescending(x => x.CreatedDate).ToList();

                return result;
            }
        }

        public List<BuyOrderListDto> GetBuyOrdersByUserId(int id)
        {
            using (var context = new SwapDbContext())
            {
                var result = (from buyOrder in context.BuyOrders
                              join status in context.Statuses on buyOrder.StatusId equals status.Id
                              join parity in context.Parities on buyOrder.ParityId equals parity.Id
                              join crypto1 in context.CryptoCurrencies on parity.ReceivedCurrencyId equals crypto1.Id
                              join crypto2 in context.CryptoCurrencies on parity.SoldCurrencyId equals crypto2.Id
                              join user in context.Users on buyOrder.UserId equals user.Id where user.Id == id
                              select new BuyOrderListDto
                              {
                                  Id = buyOrder.Id,
                                  Price = buyOrder.Price,
                                  UserName = user.Username,
                                  Amount = buyOrder.Amount,
                                  FeePrice = buyOrder.FeePrice,
                                  TotalPrice = (buyOrder.Price * buyOrder.Amount) + buyOrder.FeePrice,
                                  ParityName = (crypto1.CurrencyShortName + "/" + crypto2.CurrencyShortName),
                                  StatusName = (status.StatusName),
                                  CreatedDate = buyOrder.CreatedDate,
                                  BoughtDate = buyOrder.BoughtDate,
                              }).OrderByDescending(x => x.StatusName == "IsCompleted").ThenByDescending(x => x.CreatedDate).ToList();

                return result;
            }
        }

    }
}
