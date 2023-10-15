using Core.DataAccess.EntityFramework;
using Dal.Abstract;
using Dal.Concrete.Context;
using Entities.Concrete;
using Entities.Dto.SellOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal.Concrete.EntityFramework
{
    public class EfSellOrderDal : EfEntityRepositoryBase<SellOrder, SwapDbContext>, ISellOrderDal
    {
        public List<SellOrderListDto> GetSellOrders()
        {
            using (var context = new SwapDbContext())
            {
                var result = (from sellOrder in context.SellOrders
                              join status in context.Statuses on sellOrder.StatusId equals status.Id
                              join parity in context.Parities on sellOrder.ParityId equals parity.Id
                              join crypto1 in context.CryptoCurrencies on parity.ReceivedCurrencyId equals crypto1.Id
                              join crypto2 in context.CryptoCurrencies on parity.SoldCurrencyId equals crypto2.Id
                              join user in context.Users on sellOrder.UserId equals user.Id
                              select new SellOrderListDto
                              {
                                  Id = sellOrder.Id,
                                  Price = sellOrder.Price,
                                  UserName = user.Username,
                                  Amount = sellOrder.Amount,
                                  FeePrice = sellOrder.FeePrice,
                                  TotalPrice = (sellOrder.Price * sellOrder.Amount) + sellOrder.FeePrice,
                                  ParityName = (crypto1.CurrencyShortName + "/" + crypto2.CurrencyShortName),
                                  StatusName = (status.StatusName),
                                  CreatedDate = sellOrder.CreatedDate,
                                  SoldDate = sellOrder.SoldDate,
                              }).OrderByDescending(x => x.StatusName == "IsCompleted").ThenByDescending(x => x.CreatedDate).ToList();

                return result;
            }

        }


        public List<SellOrderListDto> GetSellOrdersByUserId(int id)
        {
            using (var context = new SwapDbContext())
            {
                var result = (from sellOrder in context.SellOrders
                              join status in context.Statuses on sellOrder.StatusId equals status.Id
                              join parity in context.Parities on sellOrder.ParityId equals parity.Id
                              join crypto1 in context.CryptoCurrencies on parity.ReceivedCurrencyId equals crypto1.Id
                              join crypto2 in context.CryptoCurrencies on parity.SoldCurrencyId equals crypto2.Id
                              join user in context.Users on sellOrder.UserId equals user.Id
                              where user.Id == id
                              select new SellOrderListDto
                              {
                                  Id = sellOrder.Id,
                                  Price = sellOrder.Price,
                                  UserName = user.Username,
                                  Amount = sellOrder.Amount,
                                  FeePrice = sellOrder.FeePrice,
                                  TotalPrice = (sellOrder.Price * sellOrder.Amount) + sellOrder.FeePrice,
                                  ParityName = (crypto1.CurrencyShortName + "/" + crypto2.CurrencyShortName),
                                  StatusName = (status.StatusName),
                                  CreatedDate = sellOrder.CreatedDate,
                                  SoldDate = sellOrder.SoldDate,
                              }).OrderByDescending(x => x.StatusName == "IsCompleted").ThenByDescending(x => x.CreatedDate).ToList();

                return result;
            }
        }

    }
}
