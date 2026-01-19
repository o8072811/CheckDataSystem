using System;
using System.IO;

namespace CheckDataSystem
{
    public static class LogService
    {
        /// <summary>
        /// 寫入 Log 到指定的資料夾
        /// </summary>
        public static void WriteToFile(string folderPath, string message)
        {
            if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath)) return;

            string fileName = $"Log_{DateTime.Now:yyyyMMdd}.txt";
            string fullPath = Path.Combine(folderPath, fileName);
            string logContent = $"[{DateTime.Now:HH:mm:ss}] {message}\r\n";

            try
            {
                // 使用 Append 模式，且允許共用寫入 (FileShare.ReadWrite)
                // 這裡簡化了原本的讀取比對邏輯，因為 Log 檔通常只管寫入，讀取交給介面
                File.AppendAllText(fullPath, logContent);
            }
            catch
            {
                // 寫入 Log 失敗通常只能忽略，不然會無限迴圈
            }
        }

        /// <summary>
        /// 讀取今日 Log 內容 (用於顯示)
        /// </summary>
        public static string ReadTodayLog(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath)) return "";

            string fileName = $"Log_{DateTime.Now:yyyyMMdd}.txt";
            string fullPath = Path.Combine(folderPath, fileName);

            if (!File.Exists(fullPath)) return "";

            try
            {
                using (var fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var sr = new StreamReader(fs))
                {
                    return sr.ReadToEnd();
                }
            }
            catch
            {
                return "";
            }
        }
    }
}