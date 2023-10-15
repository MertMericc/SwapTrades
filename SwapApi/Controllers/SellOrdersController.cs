using Business.Abstract;
using Entities.Dto.SellOrder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SwapApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SellOrdersController : ControllerBase
    {
        private ISellOrderService _sellOrderService;
        private string Token { get { return Request.Headers["Authorization"].ToString().Replace("Bearer ", ""); } }

        public SellOrdersController(ISellOrderService sellOrderService)
        {
            _sellOrderService = sellOrderService;
        }

        [Authorize(Roles = "Admin,Member")]
        [HttpGet]
        [Route("getsellorders")]
        public IActionResult GetAllSellOrders()
        {
            var sellOrders = _sellOrderService.GetAllSellOrders();
            return Ok(sellOrders);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("getsellordersbyid")]
        public IActionResult GetSellOrdersById(int userId)
        {
            var sellOrders = _sellOrderService.GetSellOrdersByUserId(userId);
            return Ok(sellOrders);
        }

        [Authorize(Roles = "Member,Admin")]
        [HttpGet]
        [Route("getmysellorders")]
        public IActionResult GetMySellOrders()
        {
            var sellOrders = _sellOrderService.GetUserSellHistory(Token);
            return Ok(sellOrders);
        }

        [Authorize(Roles = "Member,Admin")]
        [HttpPost]
        [Route("createsellorder")]
        public IActionResult CreateSellOrder(SellOrderCreateDto dto)
        {
            var sellOrder = _sellOrderService.Add(dto, Token);
            return Ok(sellOrder);
        }

        [Authorize(Roles = "Member,Admin")]
        [HttpPost]
        [Route("deletesellorder")]
        public IActionResult DeleteSellOrder(int id)
        {
            var sellOrder = _sellOrderService.Delete(id);
            return Ok(sellOrder);
        }
    }
}
