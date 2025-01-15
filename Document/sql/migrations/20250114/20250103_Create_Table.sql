create database crypto_platform;

use crypto_platform;
CREATE TABLE grid_trade_robots (
    grid_trade_robot_id INT AUTO_INCREMENT PRIMARY KEY COMMENT '機器人ID',
    status ENUM('Open', 'Running', 'Cancel') NOT NULL COMMENT '狀態',
    symbol VARCHAR(20) NOT NULL COMMENT '交易標的',
    position_side ENUM('Long', 'Short', 'All') NOT NULL COMMENT '持倉方向',
    grid_count INT NOT NULL COMMENT '網格數量',
    leverage INT DEFAULT 1 COMMENT '槓桿倍數',
    max_price DECIMAL(13,6) NOT NULL COMMENT '金額上限',
    min_price DECIMAL(13,6) NOT NULL COMMENT '金額下限',
    encrypted_api_key TEXT NOT NULL COMMENT 'API Key',
    encrypted_api_secret TEXT NOT NULL COMMENT 'API Secret',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP COMMENT '建立時間',
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新時間'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='網格交易機器人';

CREATE TABLE grid_trade_robot_details (
    grid_trade_robot_detail_id INT AUTO_INCREMENT PRIMARY KEY COMMENT '機器人詳細ID',
    grid_trade_robot_id INT NOT NULL COMMENT '機器人ID',
    grid_infos JSON NOT NULL COMMENT '網格詳細資訊',
    current_position_count INT NOT NULL COMMENT '現有倉位檔數',
    avg_price DECIMAL(13,6) NOT NULL COMMENT '平均價格',
    holding_amount DECIMAL(12,6) NOT NULL COMMENT '持倉數量',
    FOREIGN KEY (grid_trade_robot_id) REFERENCES grid_trade_robots(grid_trade_robot_id)
        ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='網格交易機器人詳細';

CREATE TABLE grid_trade_robot_orders (
    grid_trade_robot_order_id INT AUTO_INCREMENT PRIMARY KEY COMMENT '訂單ID',
    grid_trade_robot_id INT NOT NULL COMMENT '機器人ID',
    status ENUM('Running', 'Finish') NOT NULL COMMENT '訂單狀態',
    trade_amount DECIMAL(12,6) NOT NULL COMMENT '交易數量',
    trade_action ENUM('buy', 'sell') NOT NULL COMMENT '交易動作',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP COMMENT '建立時間',
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新時間',
    FOREIGN KEY (grid_trade_robot_id) REFERENCES grid_trade_robots(grid_trade_robot_id)
        ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='網格交易機器人訂單';

CREATE TABLE grid_trade_robot_order_histories (
    grid_trade_robot_order_history_id INT AUTO_INCREMENT PRIMARY KEY COMMENT '訂單歷史ID',
    grid_trade_robot_order_id INT NOT NULL COMMENT '訂單ID',
    price DECIMAL(13,6) NOT NULL COMMENT '價格',
    trade_amount DECIMAL(12,6) NOT NULL COMMENT '交易數量',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP COMMENT '建立時間',
    FOREIGN KEY (grid_trade_robot_order_id) REFERENCES grid_trade_robot_orders(grid_trade_robot_order_id)
        ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='網格交易機器人訂單歷史';
