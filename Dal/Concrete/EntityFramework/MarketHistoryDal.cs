using Dal.Abstract;
using Dal.Concrete.Context;
using Entities.Concrete;
using Entities.Dto.MarketHistory;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Dal.Concrete.EntityFramework
{
    public class MarketHistoryDal : IMarketHistoryDal
    {
        public MarketHistoryDataListDto GetList(int skip, int take, string type, int? parityId, bool? IsCancelled, bool? IsWaiting, bool? IsCompleted, string userName, string parityName)
        {
            using (var context = new SwapDbContext())
            {
                var result = (from sellOrder in context.SellOrders
                              join status in context.Statuses on sellOrder.StatusId equals status.Id
                              join parity in context.Parities on sellOrder.ParityId equals parity.Id
                              join crypto1 in context.CryptoCurrencies on parity.ReceivedCurrencyId equals crypto1.Id
                              join crypto2 in context.CryptoCurrencies on parity.SoldCurrencyId equals crypto2.Id
                              join user in context.Users on sellOrder.UserId equals user.Id
                              select new MarketHistoryListDto
                              {
                                  Id = sellOrder.Id,
                                  UserName = (user.Name + " " + user.Surname),
                                  Price = sellOrder.Price,
                                  Amount = sellOrder.Amount,
                                  FeePrice = sellOrder.FeePrice,
                                  TotalPrice = (sellOrder.Price * sellOrder.Amount) - sellOrder.FeePrice,
                                  ParityName = (crypto1.CurrencyShortName + "/" + crypto2.CurrencyShortName),
                                  StatusName = (status.StatusName),
                                  CreatedDate = sellOrder.CreatedDate,
                                  EndedDate = sellOrder.SoldDate,
                                  Type = "Sell"
                              }).
                             Concat(from buyOrder in context.BuyOrders
                                    join status in context.Statuses on buyOrder.StatusId equals status.Id
                                    join parity in context.Parities on buyOrder.ParityId equals parity.Id
                                    join crypto1 in context.CryptoCurrencies on parity.ReceivedCurrencyId equals crypto1.Id
                                    join crypto2 in context.CryptoCurrencies on parity.SoldCurrencyId equals crypto2.Id
                                    join user in context.Users on buyOrder.UserId equals user.Id
                                    select new MarketHistoryListDto
                                    {
                                        Id = buyOrder.Id,
                                        UserName = (user.Name + " " + user.Surname),
                                        Price = buyOrder.Price,
                                        Amount = buyOrder.Amount,
                                        FeePrice = buyOrder.FeePrice,
                                        TotalPrice = (buyOrder.Price * buyOrder.Amount) + buyOrder.FeePrice,
                                        ParityName = (crypto1.CurrencyShortName + "/" + crypto2.CurrencyShortName),
                                        StatusName = (status.StatusName),
                                        CreatedDate = buyOrder.CreatedDate,
                                        EndedDate = buyOrder.BoughtDate,
                                        Type = "Buy"
                                    }
                                       ).ToList();
                #region Status
                var statusList = new List<MarketHistoryListDto>();
                if (IsCancelled == true)
                {
                    var statusName = (from status in context.Statuses where status.Id == 1 select status.StatusName).ToList();
                    statusList.AddRange(result.Where(x => x.StatusName == statusName.FirstOrDefault()).ToList());
                }
                if (IsWaiting == true)
                {
                    var statusName = (from status in context.Statuses where status.Id == 2 select status.StatusName).ToList();
                    statusList.AddRange(result.Where(x => x.StatusName == statusName.FirstOrDefault()).ToList());
                }
                if (IsCompleted == true)
                {
                    var statusName = (from status in context.Statuses where status.Id == 3 select status.StatusName).ToList();
                    statusList.AddRange(result.Where(x => x.StatusName == statusName.FirstOrDefault()).ToList());
                }
                result = result.Intersect(statusList).ToList();
                #endregion
                #region Parity
                if (parityId != null)
                {
                    var parityNamee = (from parity in context.Parities
                                      where parity.Id == parityId
                                      join crypto1 in context.CryptoCurrencies on parity.ReceivedCurrencyId equals crypto1.Id
                                      join crypto2 in context.CryptoCurrencies on parity.SoldCurrencyId equals crypto2.Id
                                      select (crypto1.CurrencyShortName + "/" + crypto2.CurrencyShortName)).ToList();
                    result = result.Where(x => x.ParityName == parityNamee.FirstOrDefault()).ToList();
                }
                if(parityName != null)
                {
                    result = result.Where(x => x.ParityName.ToLower().Contains(parityName.ToLower())).ToList();
                }
                #endregion
                #region Type
                if (type != null)
                {
                    result = result.Where(x => x.Type.ToLower() == type.ToLower()).ToList();
                }
                #endregion
                #region UserName
                if(userName != null)
                {
                    result = result.Where(x => x.UserName.ToLower().Contains(userName.ToLower())).ToList();
                }
                #endregion
                return new MarketHistoryDataListDto
                {
                    Data = result.OrderByDescending(x => x.StatusName == "IsCompleted").ThenByDescending(x => x.CreatedDate).Skip(skip).Take(take).ToList(),
                    Count = result.Count,
                };
            }
        }
    }
}
