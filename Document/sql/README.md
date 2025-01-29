# SQL 文件說明

## 更新紀錄
- **更新日期** :   2025-01-29
- **描述**： 
  - 異動 `grid_trade_robot_orders` 和 `grid_trade_robot_order_histories`欄位
- **更新日期** ：  2025-01-25
- **描述**：
  - 新增 `grid_trade_robots` 表的欄位 `exchange` 資料型別 `enum('Binance', 'Bybit')  default Binance`。
- **更新日期** ：  2025-01-17
- **描述**：
  - 新增 `grid_trade_robots` 表的欄位 `amount_usdt` 資料型別 `decimal(12,4)`。
- **更新日期** ：  2025-01-15
- **描述**：
  - 修改 `grid_trade_robots` 表的欄位 `leverage`。
  - 將欄位名稱改為 `laverage`。


## 文件清單
- **Migrations**：
  - 歷次DB異動資料。
- **Schema**：
  - 完整DB Schema 可直接 mysql Import。


# Database Schema

[點擊查看資料庫結構圖](https://drive.google.com/file/d/1LHCKwMRXESlKlj9vBS7O2z9XtvmNZpa9/view?usp=sharing)

以下是目前的資料庫結構圖：

![Database Schema](schema.png)

DB First 指令 :
Scaffold-DbContext "server=127.0.0.1;port=3306;database=crypto_platform;user=root;password=" Pomelo.EntityFrameworkCore.MySql -o ./Models/Context -f -Context "CryptoPlatformDbContext" -NoOnConfiguring