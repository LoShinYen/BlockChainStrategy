﻿using BlockChainStrategy.Library.Enums;
using BlockChainStrategy.Library.Enums.Binance;

namespace BlockChainStrategy.Library.Models.Dto.Utility
{
    public class OrderRequest
    {
        public string Symbol { get; set; }
        public decimal Quantity { get; set; }
        public OrderSideStatus Side { get; set; }
        public OrderType Type { get; set; }
    }
}
