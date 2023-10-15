using Entities.Dto.BuyOrder;
using Entities.Dto.SellOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dto.CryptoCurrency
{
    public class CryptoCurrencyHistoryDto
    {
        public string CryptoName { get; set; }
        public List<SellOrderListDto> SellOrderListDtos { get; set; }
        public List<BuyOrderListDto> BuyOrderListDto { get; set; }
    }
}
