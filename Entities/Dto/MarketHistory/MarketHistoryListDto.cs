using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dto.MarketHistory
{
    public class MarketHistoryListDto : IDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal FeePrice { get; set; }
        public string ParityName { get; set; }
        public string StatusName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? EndedDate { get; set; }
        public string Type { get; set; }
    }
}
