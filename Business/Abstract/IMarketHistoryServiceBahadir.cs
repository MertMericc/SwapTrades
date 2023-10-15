using Core.Utilities.Results;
using Entities.Dto.MarketHistory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IMarketHistoryServiceBahadir
    {
        IDataResult<PagingHistoryDto> GetMarketHistoryList(int pageSize, int pageNumber, string route, string type, int? partiyId, bool? IsCancelled, bool? IsWaiting, bool? IsCompleted, string userName, string parityName);
    }
}
