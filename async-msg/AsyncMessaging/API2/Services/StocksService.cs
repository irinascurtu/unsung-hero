using API2.Helpers;
using Common;

namespace API2.Services
{
    public class StocksService : IStocksService
    {

        public StocksService()
        {

        }

        public List<ProductStock> GetStock(List<int> productIds)
        {
            return StockGenerator.AssignRandomStock(productIds);
        }
    }

}
