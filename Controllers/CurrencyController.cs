using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Proyecto_FinalProgra1.Services;
using System.Threading.Tasks;

namespace Proyecto_FinalProgra1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        private readonly CurrencyService _currencyService;

        public CurrencyController(CurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        [HttpGet("usd-to-pen")]
        public async Task<IActionResult> GetRate()
        {
            var rate = await _currencyService.GetUsdToPenRateAsync();
            return Ok(new { rate });
        }
    }
}