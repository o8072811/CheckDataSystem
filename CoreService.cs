using System;
using System.Data;
using System.Data.SQLite; // 引用剛剛裝好的套件
using System.IO;

namespace CheckDataSystem
{
    public class CoreService
    {
        // ★★★ 設定資料庫路徑 ★★★
        // 如果要共用，請改成 @"Data Source=\\Server\Share\data.db;Version=3;";
        //private string _connectionString = $"Data Source={Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.db")};Version=3;";
        private string _connectionString = "";
        public event Action<string> OnLogMessage;

        /// <summary>
        /// 初始化：如果資料庫檔案不存在，自動建立並新增 Table
        /// </summary>
        public void Initialize(string folderPath)
        {
            try
            {
                // 組合出完整的檔案路徑 (例如: Z:\Share\data.db)
                string dbPath = Path.Combine(folderPath, "data.db");

                // 設定連線字串 (加上引號防止路徑有空白報錯)
                _connectionString = $"Data Source=\"{dbPath}\";Version=3;";

                Log($"資料庫路徑設定為: {dbPath}");
                
                System.Windows.Forms.MessageBox.Show("資料庫路徑:\n" + dbPath);
                // 建立資料庫檔案 (如果不存在)
                if (!File.Exists(dbPath))
                {
                    if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
                    SQLiteConnection.CreateFile(dbPath);
                    Log("建立新的資料庫檔案 data.db");
                }

                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();
                    // 建立表格 SQL 指令：包含 條碼(主鍵) 和 狀態
                    string sql = "CREATE TABLE IF NOT EXISTS Products (Barcode TEXT PRIMARY KEY, Status TEXT)";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                Log("資料庫連線成功！");
            }
            catch (Exception ex)
            {
                Log($"初始化失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// [給同事用] 新增資料到資料庫
        /// </summary>
        public void AddData(string barcode, string status)
        {
            try
            {
                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();
                    // INSERT OR REPLACE: 如果條碼重複，就更新狀態；沒重複就新增
                    string sql = "INSERT OR REPLACE INTO Products (Barcode, Status) VALUES (@code, @status)";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
                        // 使用參數 (@) 防止 SQL Injection 攻擊，也比較安全
                        cmd.Parameters.AddWithValue("@code", barcode);
                        cmd.Parameters.AddWithValue("@status", status);
                        cmd.ExecuteNonQuery();
                    }
                }
                Log($"已錄入資料: {barcode} ({status})");
            }
            catch (Exception ex)
            {
                Log($"寫入失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// [給你用] 檢查條碼，如果有就回傳並刪除
        /// </summary>
        public ProductItem CheckAndRemove(string barcode)
        {
            ProductItem result = null;

            try
            {
                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();

                    // 1. 先查詢有沒有這筆
                    string querySql = "SELECT Status FROM Products WHERE Barcode = @code";
                    using (var cmd = new SQLiteCommand(querySql, conn))
                    {
                        cmd.Parameters.AddWithValue("@code", barcode);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string status = reader["Status"].ToString();
                                result = new ProductItem(barcode, status);
                            }
                        }
                    }

                    // 2. 如果有查到，立刻刪除
                    if (result != null)
                    {
                        string deleteSql = "DELETE FROM Products WHERE Barcode = @code";
                        using (var cmd = new SQLiteCommand(deleteSql, conn))
                        {
                            cmd.Parameters.AddWithValue("@code", barcode);
                            cmd.ExecuteNonQuery();
                        }
                        Log($"比對成功，已從資料庫刪除: {barcode}");
                       
                    }
                    else 
                    {
                        Log($"查無資料: {barcode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"查詢/刪除錯誤: {ex.Message}");
            }

            return result;
        }
        public DataTable GetAllData()
        {
            DataTable dt = new DataTable();
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT Barcode AS '條碼', Status AS '狀態' FROM Products";
                using (var adapter = new SQLiteDataAdapter(sql, conn))
                {
                    adapter.Fill(dt);
                }
            }
            return dt;
        }
        // 取得目前剩餘筆數 (方便顯示)
        public int GetCount()
        {
            try
            {
                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM Products", conn))
                    {
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch 
            { 
                return 0; 
            }
        }

        private void Log(string msg) => OnLogMessage?.Invoke(msg);
    }
}