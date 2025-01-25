use crypto_platform;
ALTER TABLE grid_trade_robots
ADD COLUMN exchange ENUM('Binance', 'Bybit') NOT NULL DEFAULT 'Binance';
