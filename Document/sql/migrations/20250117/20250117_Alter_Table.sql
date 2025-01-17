use crypto_platform;
ALTER TABLE grid_trade_robots
ADD COLUMN amount_usdt decimal(12,4) COMMENT 'USDT金額'