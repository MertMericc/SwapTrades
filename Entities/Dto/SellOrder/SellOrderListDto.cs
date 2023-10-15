using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dto.SellOrder
{
    public class SellOrderListDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public decimal Price { get; set; }//satışa konulan fiyat
        public decimal Amount { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal FeePrice { get; set; }
        public string ParityName { get; set; }
        public string StatusName { get; set; }
        public DateTime CreatedDate { get; set; } //sell oluşma tarihi
        public DateTime? SoldDate { get; set; }
    }
}
