using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dto.BuyOrder
{
    public class BuyOrderCreateDto
    {
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public int ParityId { get; set; }
    }
}
