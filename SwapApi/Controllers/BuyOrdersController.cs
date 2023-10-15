using Business.Abstract;
using Entities.Dto.BuyOrder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SwapApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuyOrdersController : ControllerBase
    {
        private IBuyOrderService _buyOrderService;
        private string Token { get { return Request.Headers["Authorization"].ToString().Replace("Bearer ", ""); } }

        public BuyOrdersController(IBuyOrderService buyOrderService)
        {
            _buyOrderService = buyOrderService;
        }

        [Authorize(Roles = "Member,Admin")]
        [HttpPost("addbuyorder")]
        public IActionResult AddBuyOrder(BuyOrderCreateDto dto)
        {
            var result = _buyOrderService.Add(dto, Token);
            return Ok(result);
        }

        [Authorize(Roles = "Member,Admin")]
        [HttpPost("deletebuyorder")]
        public IActionResult DeleteBuyOrder(int id)
        {
            var result = _buyOrderService.Delete(id);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,Member")]
        [HttpGet("getallbuyorder")]
        public IActionResult GetAllBuyOrder()
        {
            var result = _buyOrderService.GetAllBuyOrder();
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("getbuyorderbyuserid")]
        public IActionResult GetBuyOrderByUserId(int userId)
        {
            var result = _buyOrderService.GetBuyOrderByUserId(userId);
            return Ok(result);
        }

        [Authorize(Roles = "Member,Admin")]
        [HttpGet("getuserbuyhistory")]
        public IActionResult GetUserBuyOrderHistory()
        {
            var result = _buyOrderService.GetUserBuyHistory(Token);
            return Ok(result);
        }
    }
}
