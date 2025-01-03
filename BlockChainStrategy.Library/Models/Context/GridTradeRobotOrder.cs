using System;
using System.Collections.Generic;

namespace BlockChainStrategy.Library.Models.Context;

/// <summary>
/// 網格交易機器人訂單
/// </summary>
public partial class GridTradeRobotOrder
{
    /// <summary>
    /// 訂單ID
    /// </summary>
    public int GridTradeRobotOrderId { get; set; }

    /// <summary>
    /// 機器人ID
    /// </summary>
    public int GridTradeRobotId { get; set; }

    /// <summary>
    /// 訂單狀態
    /// </summary>
    public string Status { get; set; } = null!;

    /// <summary>
    /// 交易數量
    /// </summary>
    public decimal TradeAmount { get; set; }

    /// <summary>
    /// 交易動作
    /// </summary>
    public string TradeAction { get; set; } = null!;

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    public virtual GridTradeRobot GridTradeRobot { get; set; } = null!;

    public virtual ICollection<GridTradeRobotOrderHistory> GridTradeRobotOrderHistories { get; set; } = new List<GridTradeRobotOrderHistory>();
}
