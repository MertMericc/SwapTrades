using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dto.MarketHistory
{
    public class MarketHistoryFilterDto : IDto
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public string? UserName { get; set; }
        public string? ParityName { get; set; }
        public string? Type { get; set; }
        public int? ParityId { get; set; }
        public bool? IsCancelled { get; set; }
        public bool? IsWaiting { get; set; }
        public bool? IsCompleted { get; set; }
    }
}
