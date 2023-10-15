using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dto.SellOrder
{
    public class SellOrderCreateDto
    {
        public decimal Price { get; set; }//satışa konulan fiyat
        public decimal Amount { get; set; }
        public int ParityId { get; set; }
    }
}
