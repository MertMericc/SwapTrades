using Business.Abstract;
using Entities.Dto.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SwapApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;
        private string Token { get { return Request.Headers["Authorization"].ToString().Replace("Bearer ", ""); } }

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("getallusers")]
        public IActionResult GetAllUsers()
        {
            var users = _userService.GetList();
            return Ok(users);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("getbyid")]
        public IActionResult GetById(int id)
        {
            var users = _userService.GetById(id);
            return Ok(users);
        }

        [Authorize(Roles = "Member,Admin")]
        [HttpGet]
        [Route("getmydetails")]
        public IActionResult GetMyDetails()
        {
            var users = _userService.GetDetails(Token);
            return Ok(users);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("updateuser")]
        public IActionResult UpdateUser(UserUpdateDto dto)
        {
            var user = _userService.Update(dto);
            return Ok(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("deleteuser")]
        public IActionResult DeleteUser(int id)
        {
            var user = _userService.Delete(id);
            return Ok(user);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("getuserhistory")]
        public IActionResult GetUserHistory(int id)
        {
            var userHistory = _userService.GetUserHistoryById(id);
            return Ok(userHistory);
        }
    }
}
