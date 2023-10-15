using Business.Abstract;
using Business.Constants;
using Core.Utilities.Results;
using Dal.Abstract;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class OrderStatusManager : IOrderStatusService
    {
        private readonly IOrderStatusDal _orderStatusDal;

        public OrderStatusManager(IOrderStatusDal orderStatusDal)
        {
            _orderStatusDal = orderStatusDal;
        }

        public IDataResult<Status> GetById(int id)
        {
            try
            {
                var status = _orderStatusDal.GetById(id);

                if (status != null)
                {
                    return new SuccessDataResult<Status>(status, Messages.Success);
                }
                return new ErrorDataResult<Status>(Messages.StatusNotFound);

            }
            catch (Exception)
            {
                return new ErrorDataResult<Status>(Messages.UnknownError);

            }
        }
    }
}
