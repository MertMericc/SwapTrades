using Core.Entities;
using Core.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dto.MarketHistory
{
    public class PagingHistoryDto : IDto
    {
        public IEnumerable<MarketHistoryListDto> Data { get; set; }
        public SummaryPagination SummaryPagination { get; set; }
    }
}
