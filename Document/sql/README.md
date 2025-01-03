# Database Schema

[點擊查看資料庫結構圖](https://drive.google.com/file/d/1LHCKwMRXESlKlj9vBS7O2z9XtvmNZpa9/view?usp=sharing)

以下是目前的資料庫結構圖：

![Database Schema](schema.png)

DB First 指令 :
Scaffold-DbContext "server=127.0.0.1;port=3306;database=crypto_platform;user=root;password=" Pomelo.EntityFrameworkCore.MySql -o ./Models/Context -f -Context "CryptoPlatformDbContext" -NoOnConfiguring