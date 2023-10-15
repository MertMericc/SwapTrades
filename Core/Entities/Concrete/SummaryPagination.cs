using Core.Utilities.Business.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Concrete
{
    public class SummaryPagination
    {

        public SummaryPagination(int pageNumber, int pageSize)
        {
            this.CurrentPage = pageNumber;
            this.CurrentRecord = pageSize;
        }


        public int CurrentRecord { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPage { get; set; }
        public int TotalRecord { get; set; }
        public Uri FirstPage { get; set; }
        public Uri LastPage { get; set; }
        public Uri NextPage { get; set; }
        public Uri PreviousPage { get; set; }


        public static SummaryPagination CreatePagedResponse(int pSize, int pNumber, IUriService uriService, string route, int totalPage, int currentRecord, int total)
        {
            var response = new SummaryPagination(pNumber, pSize);
            response.NextPage =
        pNumber > 0 && pNumber < totalPage
        ? uriService.GetPageUri(pNumber + 1, pSize, route)
        : null;
            response.PreviousPage =
        pNumber - 1 >= 1 && pNumber <= totalPage
        ? uriService.GetPageUri(pNumber - 1, pSize, route)
        : null;

            response.FirstPage = uriService.GetPageUri(1, pSize, route);
            response.LastPage = uriService.GetPageUri(totalPage, pSize , route);
            response.CurrentPage = pNumber;
            response.CurrentRecord = currentRecord;
            response.TotalPage = totalPage;
            response.TotalRecord = total;
            return response;
        }


    }
}
