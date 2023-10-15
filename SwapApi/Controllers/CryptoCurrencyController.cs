using Business.Abstract;
using Entities.Dto.CryptoCurrency;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SwapApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CryptoCurrencyController : ControllerBase
    {
        private ICryptoCurrencyService _currencyService;

        public CryptoCurrencyController(ICryptoCurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("List")]
        public IActionResult GetList()
        {
            var result = _currencyService.GetList();
            return Ok(result);
        }

        [Authorize(Roles = "Member,Admin")]
        [HttpGet("getactivelist")]
        public IActionResult GetActivesList()
        {
            var result = _currencyService.GetActiveList();
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("getbyid")]
        public IActionResult GetById(int id)
        {
            var result = _currencyService.GetById(id);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("addCrypto")]
        public IActionResult Create(CryptoAddDto crypto)
        {
            var addedCrypto = _currencyService.Add(crypto);
            return Ok(addedCrypto);

        }

        [Authorize(Roles = "Admin")]
        [HttpPost("updateCrypto")]
        public IActionResult Update(CryptoUpdateDto dto)
        {
            var result = _currencyService.Update(dto);
            return Ok(result);
            //HTTP Status 204(No Content)
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("deleteCrypto")]
        public IActionResult Delete(int id)
        {
            var result = _currencyService.Delete(id);
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("historyofcurrency")]
        public IActionResult GetCryptoSellBuyHistory(string cryptoName)
        {
            var result = _currencyService.GetHistoryOfCrypto(cryptoName);
            return Ok(result);
        }
    }
}
