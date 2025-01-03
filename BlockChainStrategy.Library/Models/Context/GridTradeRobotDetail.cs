using System;
using System.Collections.Generic;

namespace BlockChainStrategy.Library.Models.Context;

/// <summary>
/// 網格交易機器人詳細
/// </summary>
public partial class GridTradeRobotDetail
{
    /// <summary>
    /// 機器人詳細ID
    /// </summary>
    public int GridTradeRobotDetailId { get; set; }

    /// <summary>
    /// 機器人ID
    /// </summary>
    public int GridTradeRobotId { get; set; }

    /// <summary>
    /// 網格詳細資訊
    /// </summary>
    public string GridInfos { get; set; } = null!;

    /// <summary>
    /// 現有倉位檔數
    /// </summary>
    public int CurrentPositionCount { get; set; }

    /// <summary>
    /// 平均價格
    /// </summary>
    public decimal AvgPrice { get; set; }

    /// <summary>
    /// 持倉數量
    /// </summary>
    public decimal HoldingAmount { get; set; }

    public virtual GridTradeRobot GridTradeRobot { get; set; } = null!;
}
