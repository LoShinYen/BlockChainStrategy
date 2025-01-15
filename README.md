# BlockChainStrategy

這是一個基於興趣所開發的網格交易機器人專案，目的是學習與實踐網格交易的基本邏輯，並結合多層次的軟體設計模式進行應用。

---

## 專案目標

1. 實現網格交易機器人的核心功能：
   - 多頭策略。
   - 空頭策略。
   - 中性策略。
2. 優化數據存儲，計劃從 MySQL 遷移至 SQLite，提升專案的可移植性。
3. 將目前的 Console 操作介面升級為 WPF，優化使用者體驗。

---
## 目前進度

### 已完成
- **基本網格交易邏輯實現**
  - 與 WebSocket 整合，接收市場數據。
- **AutoMapper 配置完成**
  - 用於模型映射與轉型。
- **TradeExecutionService**
  - 實現核心交易邏輯，並基於策略模式進行交易執行。
- **DI 設計與實現**
  - 使用依賴注入進行模組解耦與擴展性設計。

### 開發中
- **基本網格交易邏輯實現**
  - 支援多頭策略的基本交易。

### 待完成
- **TradeExecutionService**
  - 交易完成後資料存取   
- **幣安API串接**
- **空頭與中性策略**
  - 計劃加入更多交易策略，涵蓋空頭與中性場景。
- **單元測試**
- **資料庫遷移**
  - 從 MySQL 遷移至 SQLite，增強可移植性。
- **操作介面升級**
  - 計劃將目前的 Console 介面替換為 WPF 圖形化介面。
---
## 專案結構

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
