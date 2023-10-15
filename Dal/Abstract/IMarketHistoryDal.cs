using Entities.Concrete;
using Entities.Dto.MarketHistory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal.Abstract
{
    public interface IMarketHistoryDal
    {
        MarketHistoryDataListDto GetList(int skip, int take, string type, int? partiyId, bool? IsCancelled, bool? IsWaiting, bool? IsCompleted, string userName, string parityName);
    }
}
