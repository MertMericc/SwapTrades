using Core.DataAccess;
using Entities.Concrete;
using Entities.Dto.SellOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal.Abstract
{
    public interface ISellOrderDal : IEntityRepository<SellOrder>
    {
        List<SellOrderListDto> GetSellOrders();
        List<SellOrderListDto> GetSellOrdersByUserId(int id);
    }
}
