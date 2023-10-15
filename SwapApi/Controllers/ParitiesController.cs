using Business.Abstract;
using Entities.Dto.Parity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SwapApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParitiesController : ControllerBase
    {
        private IParityService _parityManager;

        
        public ParitiesController(IParityService parityManager)
        {
            _parityManager = parityManager;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("addparity")]
        public IActionResult Add(ParityCreateDto dto)
        {
            var result = _parityManager.Add(dto);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("updateparity")]
        public IActionResult Update(ParityUpdateDto dto)
        {
            var result = _parityManager.Update(dto);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("deleteparity")]
        public IActionResult Delete(int id)
        {
            var result = _parityManager.Delete(id);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("getallparities")]
        public IActionResult GetList()
        {
            var result = _parityManager.GetList();
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("getparitiesbyid")]
        public IActionResult GetById(int id)
        {
            var result = _parityManager.GetById(id);
            return Ok(result);
        }
    }
}
