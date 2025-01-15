using System;
using System.Collections.Generic;

namespace BlockChainStrategy.Library.Models.Context;

/// <summary>
/// 網格交易機器人
/// </summary>
public partial class GridTradeRobot
{
    /// <summary>
    /// 機器人ID
    /// </summary>
    public int GridTradeRobotId { get; set; }

    /// <summary>
    /// 狀態
    /// </summary>
    public string Status { get; set; } = null!;

    /// <summary>
    /// 交易標的
    /// </summary>
    public string Symbol { get; set; } = null!;

    /// <summary>
    /// 持倉方向
    /// </summary>
    public string PositionSide { get; set; } = null!;

    /// <summary>
    /// 網格數量
    /// </summary>
    public int GridCount { get; set; }

    public int? Laverage { get; set; }

    /// <summary>
    /// 金額上限
    /// </summary>
    public decimal MaxPrice { get; set; }

    /// <summary>
    /// 金額下限
    /// </summary>
    public decimal MinPrice { get; set; }

    /// <summary>
    /// API Key
    /// </summary>
    public string EncryptedApiKey { get; set; } = null!;

    /// <summary>
    /// API Secret
    /// </summary>
    public string EncryptedApiSecret { get; set; } = null!;

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<GridTradeRobotDetail> GridTradeRobotDetails { get; set; } = new List<GridTradeRobotDetail>();

    public virtual ICollection<GridTradeRobotOrder> GridTradeRobotOrders { get; set; } = new List<GridTradeRobotOrder>();
}
