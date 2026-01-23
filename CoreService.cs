﻿using System;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace CheckDataSystem
{
    public class CoreService
    {
        private string _connectionString = "";

        // 用事件通知外部發生了什麼事 (Log)，而不是自己跳視窗
        public event Action<string> OnLogMessage;

        /// <summary>
        /// 初始化資料庫
        /// </summary>
        public void Initialize(string folderPath)
        {
            try
            {
                string dbPath = Path.Combine(folderPath, "data.db");
                _connectionString = $"Data Source=\"{dbPath}\";Version=3;";

                Log($"資料庫路徑設定為: {dbPath}");

                if (!File.Exists(dbPath))
                {
                    if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
                    SQLiteConnection.CreateFile(dbPath);
                    Log("建立新的資料庫檔案 data.db");
                }

                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();
                    // 確保 Table 存在
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
                throw; // 把錯誤往外丟，讓 Form1 決定要不要關閉程式
            }
        }

        /// <summary>
        /// 新增資料 (INSERT OR REPLACE)
        /// </summary>
        public void AddData(string barcode, string status)
        {
            try
            {
                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();
                    string sql = "INSERT OR REPLACE INTO Products (Barcode, Status) VALUES (@code, @status)";
                    using (var cmd = new SQLiteCommand(sql, conn))
                    {
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
                throw; // 選擇性：看是否要讓 UI 知道失敗
            }
        }

        /// <summary>
        /// 檢查並刪除 (Check and Remove)
        /// </summary>
        public ProductItem CheckAndRemove(string barcode)
        {
            ProductItem result = null;
            try
            {
                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();

                    // 1. 查詢
                    string querySql = "SELECT Status FROM Products WHERE Barcode = @code";
                    using (var cmd = new SQLiteCommand(querySql, conn))
                    {
                        cmd.Parameters.AddWithValue("@code", barcode);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                result = new ProductItem(barcode, reader["Status"].ToString());
                            }
                        }
                    }

                    // 2. 刪除 (如果有查到的話)
                    if (result != null)
                    {
                        string deleteSql = "DELETE FROM Products WHERE Barcode = @code";
                        using (var cmd = new SQLiteCommand(deleteSql, conn))
                        {
                            cmd.Parameters.AddWithValue("@code", barcode);
                            cmd.ExecuteNonQuery();
                        }
                        Log($"比對成功，已刪除: {barcode}");
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
            try
            {
                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();
                    string sql = "SELECT Barcode AS '條碼', Status AS '狀態' FROM Products";
                    using (var adapter = new SQLiteDataAdapter(sql, conn))
                    {
                        adapter.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"讀取列表失敗: {ex.Message}");
            }
            return dt;
        }

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

        /// <summary>
        /// 清空所有資料
        /// </summary>
        public void ClearAllData()
        {
            try
            {
                using (var conn = new SQLiteConnection(_connectionString))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand("DELETE FROM Products", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                Log("已手動清空所有資料");
            }
            catch (Exception ex)
            {
                Log($"清空失敗: {ex.Message}");
            }
        }

        private void Log(string msg) => OnLogMessage?.Invoke(msg);
    }
}