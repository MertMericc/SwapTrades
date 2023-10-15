using Business.Abstract;
using Core.Entities.Concrete;
using Core.Utilities.Business.Abstract;
using Entities.Dto.MarketHistory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SwapApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketHistoriesController : ControllerBase
    {
        private IMarketHistoryServiceBahadir _marketHistoryServiceBahadir;

        public MarketHistoriesController(IMarketHistoryServiceBahadir marketHistoryServiceBahadir)
        {
            _marketHistoryServiceBahadir = marketHistoryServiceBahadir;
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost("getmarkethistory")]
        public IActionResult GetMarketHistory(MarketHistoryFilterDto filter)
        {
            var route = Request.Path.Value;
            var result = _marketHistoryServiceBahadir.GetMarketHistoryList(filter.PageSize, filter.PageNumber, route, filter.Type, filter.ParityId, filter.IsCancelled, filter.IsWaiting, filter.IsCompleted, filter.UserName, filter.ParityName);
            return Ok(result);
        }
    }
}
