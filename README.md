# BlockChainStrategy

這是一個基於興趣所開發的網格交易機器人專案，目的是學習與實踐網格交易的基本邏輯，並結合多層次的軟體設計模式進行應用。

---

## 專案目標

1. 實現網格交易機器人的核心功能：
   - 多頭策略。
2. 研究並實作單元測試(xUnit)。
3. 新增複數交易所
4. 優化數據存儲，計劃從 MySQL 遷移至 SQLite。
5. 網格交易機器人
   - 空頭策略。
   - 中性策略。
6. 擴充其餘交易策略

---
## 目前進度
### 已完成
- **實現網格交易機器人的核心功能：**
  - 多頭策略。
- **新增複數交易所**
  - 架構設計完成

### 開發中
- **新增複數交易所**
  - 預計新增Bybit交易所
- **研究並實作單元測試**

### 待完成
- **優化數據存儲，計劃從 MySQL 遷移至 SQLite**
- **網格交易機器人**
   - 空頭策略。
   - 中性策略。 
- **擴充其餘交易策略**

---
## 專案結構(調整中，待更新)

```plaintext
BlockChainStrategy/
├── BlockChainStrategy.Library/
│   ├── Models/          # ORM模型
│   │   ├── Extensions/  # 擴展ORM類別
│   │   └── Context/     # ORM 模型
│   ├── Helpers/         # 輔助工具類
├── Document/
│   ├── sql/
│   │   ├── migrations/  # 數據庫遷移腳本
│   │   └── schema/      # 數據庫結構快照
│   └── README.md        # 文檔概述
│ GridBotStrategy/
│   ├── Enums/                        # 枚舉類型
│   ├── Extensions/                   # 擴展方法
│   ├── Helpers/                      # 工具類與輔助方法
│   │   └── TradeStrategyFactory      # 策略工廠
│   ├── Mappings/                     # AutoMapper 配置
│   ├── Models/                       # DTO 與業務模型
│   ├── Observers/                    # 觀察者模式的實現
│   ├── Repository/                   # 資料存取層
│   ├── Services/                     # 核心服務
│   │   ├── Interface/                # 服務接口
│   │   ├── Handlers/                 # 流程調度與處理類
│   │   │   └── TradeHandler.cs       # 交易流程判定
│   │   ├── TradeExecutionService.cs  # 業務執行服務
│   │   ├── TradeOperationService.cs  # 交易基礎操作服務
|   |   └── RobotManagerService.cs    # 使用者操作服務
│   ├── Strategies/                   # 策略類
│   │   ├── ITradeStrategy.cs
│   │   ├── LongTradeStrategy.cs
│   │   ├── NeutralTradeStrategy.cs
│   │   └── ShortTradeStrategy.cs
│   └── Program.cs                 # 主程序入口
├── UnifiedWsGateway/
│   ├── Services/        # WebSocket 統一接口
│   └── Models/          # 市場數據模型
├── README.md            # 專案描述與開發指南
└── BlockChainStrategy.sln # 解決方案文件
```
---
## 注意事項
1. 本專案僅作為學習用途，並未實際部署於生產環境。  
2. 交易邏輯的實現僅供參考，不建議直接應用於真實交易。  
3. 開發者對因使用此專案所產生的任何損失不承擔責任。  
