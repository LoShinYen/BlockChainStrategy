using System;
using System.Collections.Generic;

namespace BlockChainStrategy.Library.Models.Context;

/// <summary>
/// 網格交易機器人訂單歷史
/// </summary>
public partial class GridTradeRobotOrderHistory
{
    /// <summary>
    /// 訂單歷史ID
    /// </summary>
    public int GridTradeRobotOrderHistoryId { get; set; }

    /// <summary>
    /// 訂單ID
    /// </summary>
    public int GridTradeRobotOrderId { get; set; }

    /// <summary>
    /// 價格
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// 交易數量
    /// </summary>
    public decimal TradeAmount { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    public virtual GridTradeRobotOrder GridTradeRobotOrder { get; set; } = null!;
}
