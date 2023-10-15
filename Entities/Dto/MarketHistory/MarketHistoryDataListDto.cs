using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dto.MarketHistory
{
    public class MarketHistoryDataListDto : IDto
    {
        public List<MarketHistoryListDto> Data { get; set; }
        public int Count { get; set; }
    }
}
