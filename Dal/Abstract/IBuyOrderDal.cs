using Core.DataAccess;
using Entities.Concrete;
using Entities.Dto.BuyOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal.Abstract
{
    public interface IBuyOrderDal : IEntityRepository<BuyOrder>
    {
        List<BuyOrderListDto> GetBuyOrders();
        List<BuyOrderListDto> GetBuyOrdersByUserId(int id);
    }
}
