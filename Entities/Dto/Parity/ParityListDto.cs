using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dto.Parity
{
    public class ParityListDto
    {
        public int Id { get; set; }
        public string ReceivedCurrenyName { get; set; }
        public string SoldCurrencyName { get; set; }
        public bool IsActiveParity { get; set; }
        public decimal FeeRate { get; set; }
    }
}
