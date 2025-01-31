using Common;

namespace API2.Helpers
{
    public class StockGenerator
    {
        private static readonly Random random = new Random();

        public static List<ProductStock> AssignRandomStock(List<int> productIds, int minStock = 0, int maxStock = 100)
        {
            var stockLevels = new List<ProductStock>();

            foreach (var productId in productIds)
            {
                var stockLevel = new ProductStock
                {
                    ProductId = productId,
                    Stock = random.Next(minStock, maxStock + 1)
                };
                stockLevels.Add(stockLevel);
            }

            return stockLevels;
        }

       
    }

}
