using Business.Abstract;
using Business.Constants;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Dal.Abstract;
using Entities.Concrete;
using Entities.Dto.Parity;
using Entities.Dto.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class OrderManager : IOrderService
    {
        private IParityService _parityService;

        public OrderManager(IParityService parityService, IOrderStatusService orderStatusService)
        {
            _parityService = parityService;
        }

        public IDataResult<Parity> GetParityById(int id)
        {
            try
            {
                var parity = _parityService.GetById(id).Data;
                if (parity != null)
                {
                    return new SuccessDataResult<Parity>(parity, Messages.Success);
                }
                return new ErrorDataResult<Parity>(Messages.ParityNotFound);
            }
            catch (Exception)
            {
                return new ErrorDataResult<Parity>(Messages.UnknownError);
            }
        }

        public IDataResult<ParityListDto> GetParityDtoById(int id)
        {
            try
            {
                var parity = _parityService.GetDtoById(id).Data;
                if (parity != null)
                {
                    return new SuccessDataResult<ParityListDto>(parity, Messages.Success);
                }
                return new ErrorDataResult<ParityListDto>(Messages.ParityNotFound);
            }
            catch (Exception)
            {
                return new ErrorDataResult<ParityListDto>(Messages.UnknownError);
            }
        }
    }
}
