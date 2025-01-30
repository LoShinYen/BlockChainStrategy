using BlockChainStrategy.Library.Enums;

namespace BlockChainStrategy.Library.Models.Dto.Utility
{
    public class OrderResponse
    {
        public string ClientOrderId { get; set; } = string.Empty;
        
        public string Symbol { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public decimal Quantity { get; set; }

        public OrderSideStatus OrderSideStatus { get; set; }

        public DateTime UpdateTime { get; set; } = DateTime.UtcNow;

    }
}
