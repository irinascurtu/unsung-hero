using API2.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace API2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StocksController : ControllerBase
    {
        private readonly IStocksService stocksService;

        public StocksController(IStocksService stocksService)
        {
            this.stocksService = stocksService;
        }

        [HttpGet]

        public async Task<IActionResult> GetProductsStocks([FromQuery] List<int> productIds)
        {
            Random random = new Random();

            // Generate a random number between 0 and 100
            //int number = random.Next(0, 101);
            //if (number % 2 == 0)
            //{
            //    throw new ArgumentOutOfRangeException("BOOM! A failure occurred");
            //}

            var products = stocksService.GetStock(productIds);
            return Ok(products);
        }

    }
}
