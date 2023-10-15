using Business.Abstract;
using Business.Constants;
using Core.Entities.Concrete;
using Core.Utilities.Business.Abstract;
using Core.Utilities.Results;
using Dal.Abstract;
using Entities.Concrete;
using Entities.Dto.BuyOrder;
using Entities.Dto.MarketHistory;
using Entities.Dto.SellOrder;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class MarketHistoryManagerBahadir : IMarketHistoryServiceBahadir
    {
        private IMarketHistoryDal _marketHistoryDal;
        private readonly IUriService _uriService;


        public MarketHistoryManagerBahadir(IMarketHistoryDal marketHistoryDal, IUriService uriService)
        {
            _marketHistoryDal = marketHistoryDal;
            _uriService = uriService;
        }

        public IDataResult<PagingHistoryDto> GetMarketHistoryList(int pageSize, int pageNumber, string route, string type, int? parityId, bool? IsCancelled, bool? IsWaiting, bool? IsCompleted, string userName, string parityName)
        {
            try
            {
                if (pageNumber <= 0)
                {
                    return new ErrorDataResult<PagingHistoryDto>(Messages.PageNumberWasOutOfBounds);
                }
                var skip = pageNumber == 1 ? 0 : (pageNumber - 1) * pageSize;
                var data = _marketHistoryDal.GetList(skip, pageSize, type, parityId, IsCancelled, IsWaiting, IsCompleted, userName, parityName);

                int total = data.Count;
                int maxPageNumber;
                if ((total % pageSize) == 0)
                {
                    maxPageNumber = (total / pageSize);
                }
                else
                {
                    maxPageNumber = total / pageSize + 1;
                }

                //Eğer max pageden fazla veya 0 ve 0'dan düşük bir değer girerse error
                if (pageNumber > maxPageNumber)
                {
                    return new ErrorDataResult<PagingHistoryDto>(Messages.PageNumberWasOutOfBounds);
                }

                var pagedReponse = SummaryPagination.CreatePagedResponse(pageSize, pageNumber, _uriService, route, maxPageNumber, data.Data.Count, total);

                var pagingHistory = new PagingHistoryDto
                {
                    Data = data.Data,
                    SummaryPagination = pagedReponse,
                };


                return new SuccessDataResult<PagingHistoryDto>(pagingHistory, Messages.Success);
            }
            catch (Exception)
            {
                return new ErrorDataResult<PagingHistoryDto>(Messages.UnknownError);
            }
        }
    }
}
