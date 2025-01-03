using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BlockChainStrategy.Library.Models.Context;

public partial class CryptoPlatformDbContext : DbContext
{
    public CryptoPlatformDbContext(DbContextOptions<CryptoPlatformDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<GridTradeRobot> GridTradeRobots { get; set; }

    public virtual DbSet<GridTradeRobotDetail> GridTradeRobotDetails { get; set; }

    public virtual DbSet<GridTradeRobotOrder> GridTradeRobotOrders { get; set; }

    public virtual DbSet<GridTradeRobotOrderHistory> GridTradeRobotOrderHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<GridTradeRobot>(entity =>
        {
            entity.HasKey(e => e.GridTradeRobotId).HasName("PRIMARY");

            entity.ToTable("grid_trade_robots", tb => tb.HasComment("網格交易機器人"));

            entity.Property(e => e.GridTradeRobotId)
                .HasComment("機器人ID")
                .HasColumnType("int(11)")
                .HasColumnName("grid_trade_robot_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("建立時間")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.EncryptedApiKey)
                .HasComment("API Key")
                .HasColumnType("text")
                .HasColumnName("encrypted_api_key");
            entity.Property(e => e.EncryptedApiSecret)
                .HasComment("API Secret")
                .HasColumnType("text")
                .HasColumnName("encrypted_api_secret");
            entity.Property(e => e.GridCount)
                .HasComment("網格數量")
                .HasColumnType("int(11)")
                .HasColumnName("grid_count");
            entity.Property(e => e.Leverage)
                .HasDefaultValueSql("'1'")
                .HasComment("槓桿倍數")
                .HasColumnType("int(11)")
                .HasColumnName("leverage");
            entity.Property(e => e.MaxPrice)
                .HasPrecision(13, 6)
                .HasComment("金額上限")
                .HasColumnName("max_price");
            entity.Property(e => e.MinPrice)
                .HasPrecision(13, 6)
                .HasComment("金額下限")
                .HasColumnName("min_price");
            entity.Property(e => e.PositionSide)
                .HasComment("持倉方向")
                .HasColumnType("enum('Long','Short','All')")
                .HasColumnName("position_side");
            entity.Property(e => e.Status)
                .HasComment("狀態")
                .HasColumnType("enum('Open','Running','Cancel')")
                .HasColumnName("status");
            entity.Property(e => e.Symbol)
                .HasMaxLength(20)
                .HasComment("交易標的")
                .HasColumnName("symbol");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("更新時間")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<GridTradeRobotDetail>(entity =>
        {
            entity.HasKey(e => e.GridTradeRobotDetailId).HasName("PRIMARY");

            entity.ToTable("grid_trade_robot_details", tb => tb.HasComment("網格交易機器人詳細"));

            entity.HasIndex(e => e.GridTradeRobotId, "grid_trade_robot_id");

            entity.Property(e => e.GridTradeRobotDetailId)
                .HasComment("機器人詳細ID")
                .HasColumnType("int(11)")
                .HasColumnName("grid_trade_robot_detail_id");
            entity.Property(e => e.AvgPrice)
                .HasPrecision(13, 6)
                .HasComment("平均價格")
                .HasColumnName("avg_price");
            entity.Property(e => e.CurrentPositionCount)
                .HasComment("現有倉位檔數")
                .HasColumnType("int(11)")
                .HasColumnName("current_position_count");
            entity.Property(e => e.GridInfos)
                .HasComment("網格詳細資訊")
                .HasColumnType("json")
                .HasColumnName("grid_infos");
            entity.Property(e => e.GridTradeRobotId)
                .HasComment("機器人ID")
                .HasColumnType("int(11)")
                .HasColumnName("grid_trade_robot_id");
            entity.Property(e => e.HoldingAmount)
                .HasPrecision(12, 6)
                .HasComment("持倉數量")
                .HasColumnName("holding_amount");

            entity.HasOne(d => d.GridTradeRobot).WithMany(p => p.GridTradeRobotDetails)
                .HasForeignKey(d => d.GridTradeRobotId)
                .HasConstraintName("grid_trade_robot_details_ibfk_1");
        });

        modelBuilder.Entity<GridTradeRobotOrder>(entity =>
        {
            entity.HasKey(e => e.GridTradeRobotOrderId).HasName("PRIMARY");

            entity.ToTable("grid_trade_robot_orders", tb => tb.HasComment("網格交易機器人訂單"));

            entity.HasIndex(e => e.GridTradeRobotId, "grid_trade_robot_id");

            entity.Property(e => e.GridTradeRobotOrderId)
                .HasComment("訂單ID")
                .HasColumnType("int(11)")
                .HasColumnName("grid_trade_robot_order_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("建立時間")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.GridTradeRobotId)
                .HasComment("機器人ID")
                .HasColumnType("int(11)")
                .HasColumnName("grid_trade_robot_id");
            entity.Property(e => e.Status)
                .HasComment("訂單狀態")
                .HasColumnType("enum('Running','Finish')")
                .HasColumnName("status");
            entity.Property(e => e.TradeAction)
                .HasComment("交易動作")
                .HasColumnType("enum('buy','sell')")
                .HasColumnName("trade_action");
            entity.Property(e => e.TradeAmount)
                .HasPrecision(12, 6)
                .HasComment("交易數量")
                .HasColumnName("trade_amount");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("更新時間")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.GridTradeRobot).WithMany(p => p.GridTradeRobotOrders)
                .HasForeignKey(d => d.GridTradeRobotId)
                .HasConstraintName("grid_trade_robot_orders_ibfk_1");
        });

        modelBuilder.Entity<GridTradeRobotOrderHistory>(entity =>
        {
            entity.HasKey(e => e.GridTradeRobotOrderHistoryId).HasName("PRIMARY");

            entity.ToTable("grid_trade_robot_order_histories", tb => tb.HasComment("網格交易機器人訂單歷史"));

            entity.HasIndex(e => e.GridTradeRobotOrderId, "grid_trade_robot_order_id");

            entity.Property(e => e.GridTradeRobotOrderHistoryId)
                .HasComment("訂單歷史ID")
                .HasColumnType("int(11)")
                .HasColumnName("grid_trade_robot_order_history_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("current_timestamp()")
                .HasComment("建立時間")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.GridTradeRobotOrderId)
                .HasComment("訂單ID")
                .HasColumnType("int(11)")
                .HasColumnName("grid_trade_robot_order_id");
            entity.Property(e => e.Price)
                .HasPrecision(13, 6)
                .HasComment("價格")
                .HasColumnName("price");
            entity.Property(e => e.TradeAmount)
                .HasPrecision(12, 6)
                .HasComment("交易數量")
                .HasColumnName("trade_amount");

            entity.HasOne(d => d.GridTradeRobotOrder).WithMany(p => p.GridTradeRobotOrderHistories)
                .HasForeignKey(d => d.GridTradeRobotOrderId)
                .HasConstraintName("grid_trade_robot_order_histories_ibfk_1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
