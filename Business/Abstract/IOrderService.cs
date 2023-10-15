using Core.Entities.Concrete;
using Core.Utilities.Results;
using Entities.Concrete;
using Entities.Dto.Parity;
using Entities.Dto.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IOrderService
    {
        IDataResult<Parity> GetParityById(int id);
        IDataResult<ParityListDto> GetParityDtoById(int id);
    }
}
