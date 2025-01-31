
using Common;

namespace API2.Services
{
    public interface IStocksService
    {
        List<ProductStock> GetStock(List<int> productIds);
    }
}