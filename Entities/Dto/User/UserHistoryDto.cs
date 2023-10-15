using Core.Entities;
using Entities.Dto.BuyOrder;
using Entities.Dto.SellOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dto.User
{
    public class UserHistoryDto : IDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public List<SellOrderListDto> UserSellOrdersHistory{ get; set; }
        public List<BuyOrderListDto> UserBuyOrdersHistory{ get; set; }
    }
}
