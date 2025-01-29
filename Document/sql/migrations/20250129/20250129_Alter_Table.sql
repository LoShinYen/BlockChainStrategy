use crypto_platform;
-- 刪除 `grid_trade_robot_orders` 表的 `trade_action` 欄位
ALTER TABLE grid_trade_robot_orders
DROP COLUMN trade_action;

-- 在 `grid_trade_robot_order_histories` 表新增 `trade_action` 欄位
ALTER TABLE grid_trade_robot_order_histories
ADD COLUMN trade_action ENUM('Buy', 'Sell') NOT NULL;