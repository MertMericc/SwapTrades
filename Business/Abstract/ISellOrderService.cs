using Core.Utilities.Results;
using Entities.Concrete;
using Entities.Dto.SellOrder;
using System.Linq.Expressions;

namespace Business.Abstract
{
    public interface ISellOrderService
    {
        IDataResult<SellOrderCreateDto> Add(SellOrderCreateDto dto, string token);
        IResult Delete(int id);
        IDataResult<List<SellOrderListDto>> GetUserSellHistory(string token);
        IDataResult<List<SellOrderListDto>> GetAllSellOrders();
        IDataResult<List<SellOrderListDto>> GetSellOrdersByUserId(int userId);
    }
}
