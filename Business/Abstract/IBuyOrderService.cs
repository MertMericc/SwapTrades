using Core.Utilities.Results;
using Entities.Concrete;
using Entities.Dto.BuyOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IBuyOrderService
    {
        IDataResult<BuyOrderCreateDto> Add(BuyOrderCreateDto dto, string token);
        IResult Delete(int id);
        IDataResult<List<BuyOrderListDto>> GetBuyOrderByUserId(int userId);
        IDataResult<List<BuyOrderListDto>> GetUserBuyHistory(string token);
        IDataResult<List<BuyOrderListDto>> GetAllBuyOrder();

    }
}
