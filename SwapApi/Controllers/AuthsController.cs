using Business.Abstract;
using Entities.Dto.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SwapApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthsController : ControllerBase
    {
        private IAuthService _authService;

        public AuthsController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("register")]
        public ActionResult Register(UserForRegisterDto userForRegisterDto)
        {
            var userExists = _authService.GetByMail(userForRegisterDto.Email);

            if (userExists.Data != null)
            {
                return BadRequest(userExists);
            }

            var registerResult = _authService.Register(userForRegisterDto, userForRegisterDto.Password);
            var result = _authService.CreateAccessToken(registerResult.Data);
            if (!result.Success)
            {
                return BadRequest(result.Message);
            }
            return Ok(result.Message);
        }


        [HttpPost("login")]
        public ActionResult Login(UserForLoginDto userForLoginDto)
        {
            var userToLogin = _authService.Login(userForLoginDto);

            var result = _authService.CreateAccessToken(userToLogin.Data);
            if (!result.Success)
            {
               return BadRequest(userToLogin.Message);
            }
            return Ok(result.Data);
        }


    }
}
