using System;
using System.Drawing;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace CheckDataSystem
{
    public partial class Form1 : Form
    {
        private CoreService _service;

        public Form1()
        {
            InitializeComponent();

            _service = new CoreService();
            _service.OnLogMessage += UpdateLogUi;

            // 綁定事件
            this.Load += Form1_Load;

            // 綁定按鈕 (請確認設計畫面的按鈕名稱與這裡一致)
            btnCheck.Click += BtnCheck_Click;
            btnAdd.Click += BtnAdd_Click;
            btnRefresh.Click += (s, e) => RefreshGrid();
            txtInputCheck.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnCheck_Click(s, e); };
            txtInputAdd.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnAdd_Click(s, e); };

            // --- 檢核區 (你用的) ---
            btnCheck.Click += BtnCheck_Click;
            // 假設檢核區的輸入框叫 txtInputCheck
            txtInputCheck.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnCheck_Click(s, e); };

            // --- 錄入區 (同事用的) ---
            // 假設你在 Tab2 加了按鈕 btnAddData 和輸入框 txtInputAdd
            // 請確認你在設計畫面有這些元件，並綁定以下事件
            btnAdd.Click += BtnAdd_Click;
            txtInputAdd.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnAdd_Click(s, e); };
        }

        // [檢核區] 你的功能：比對並刪除
        private void BtnCheck_Click(object sender, EventArgs e)
        {
            string input = txtInputCheck.Text.Trim(); // 這裡用你的檢核輸入框
            if (string.IsNullOrEmpty(input)) return;

            // 直接呼叫資料庫做「查詢 + 刪除」原子操作
            ProductItem result = _service.CheckAndRemove(input);

            if (result != null)
            {
                lblResult.Text = result.Status;
                lblResult.ForeColor = (result.Status == "OK") ? Color.Green : Color.Red;
            }
            else
            {
                lblResult.Text = "查無資料";
                lblResult.ForeColor = Color.Gray;
            }

            //txtInputCheck.SelectAll();
            txtInputCheck.Clear();
            UpdateCount();
            RefreshGrid();
        }

        // [錄入區] 同事的功能：新增資料
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            int Total = _service.GetCount();
            if (Total < 50)
            {
                // 這裡假設你有個輸入框給同事用，叫 txtInputAdd
                string input = txtInputAdd.Text.Trim();
                //if (string.IsNullOrEmpty(input)) return;

                // 為了簡化，這裡先用隨機產生模擬同事錄入 (你可以改成真實輸入)
                // _service.AddData(input, "OK"); 

                // 模擬：產生一筆隨機的
                for (int i = 0; i < 10; i++)
                {

                    Random rnd = new Random();
                    string code = "A" + rnd.Next(1000, 9999);
                    string status = rnd.Next(100) < 80 ? "OK" : "NG";

                    _service.AddData(code, status); // 寫入資料庫

                    UpdateCount();
                }
            }
        }
        //private void RefreshGrid()
        //{
        //    dgvData.DataSource = _service.GetAllData();
        //    dgvData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        //    lblTotalCount.Text = $"目前庫存: {_service.GetCount()} 筆";
        //}
        private void RefreshGrid()
        {
            // 為了防止閃爍太嚴重，我們可以記錄原本選取的行 (進階優化)
            // 但最簡單的做法就是直接重讀：

            try
            {
                // 讀取資料
                System.Data.DataTable dt = _service.GetAllData();

                // 為了避免干擾使用者操作，如果資料筆數沒變，其實可以不用重繪 (選用優化)
                // 這裡我們簡單暴力，直接更新：
                dgvData.DataSource = dt;

                // 更新標題數量
                UpdateCount();

            }
            catch
            {
                // 如果剛好同事在寫入，可能會發生鎖定，這裡不做事直接略過，等下一次 Tick
            }
        }
        
        private void UpdateCount()
        {
            // 可以在標題列顯示目前還有幾筆資料
            this.Text = $"檢核系統 - 目前庫存: {_service.GetCount()} 筆";
        }

        private void UpdateLogUi(string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => UpdateLogUi(message)));
                return;
            }
            string time = DateTime.Now.ToString("HH:mm:ss");
            rtbLog.AppendText($"[{time}] {message}\r\n");
            rtbLog.SelectionStart = rtbLog.Text.Length;
            rtbLog.ScrollToCaret();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 1. 取得目前設定的路徑
            string savedPath = Properties.Settings.Default.DbFolderPath;

            if (string.IsNullOrEmpty(savedPath) || !System.IO.Directory.Exists(savedPath))
            {
                // 使用 FolderBrowserDialog 選擇資料夾
                using (var dialog = new FolderBrowserDialog())
                {
                    dialog.Description = "請選擇資料庫 (data.db) 要存放的共用資料夾";

                    // 如果使用者按了確定
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        savedPath = dialog.SelectedPath;

                        // ★ 儲存設定，下次就不會再問了 ★
                        Properties.Settings.Default.DbFolderPath = savedPath;
                        Properties.Settings.Default.Save();
                    }
                    else
                    {
                        // 如果使用者按取消，程式無法執行，直接關閉
                        MessageBox.Show("未選擇路徑，程式將關閉。");
                        this.Close();
                        return;
                    }
                }
            }
            // 3. 把路徑傳給 Service 進行初始化
            try
            {
                _service.Initialize(savedPath);
            }
            catch
            {
                MessageBox.Show("資料庫連線失敗，請檢查路徑權限。");
                // 如果失敗，可以考慮清空設定讓使用者下次重選
                Properties.Settings.Default.DbFolderPath = "";
                Properties.Settings.Default.Save();
                this.Close();
                return;
            }

            Hide(); // 先把主視窗藏起來，不然會看到背後有一個空的視窗
            using (LoginForm login = new LoginForm())
            {
                // ShowDialog 會卡住程式，直到使用者關閉那個小視窗

                if (login.ShowDialog() == DialogResult.OK)
                {
                    // 使用者選好了，根據選擇來移除不需要的分頁
                    string role = login.SelectedRole;
                    if (role == "Up")
                    {
                        // 如果是檢核員 -> 移除「錄入區」
                        // 假設你的 TabControl 叫 tabControl1，錄入分頁叫 tabAdd
                        tabControl1.TabPages.Remove(tabCheck);
                        this.Text += " - [身分: 上游]";
                    }
                    else if (role == "Down")
                    {
                        // 如果是錄入員 -> 移除「檢核區」
                        // 假設檢核分頁叫 tabCheck
                        tabControl1.TabPages.Remove(tabAdd);
                        this.Text += " - [身分: 下游]";
                    }
                    // 兩個人都保留「總覽區 (tabView)」，方便查看
                    Show(); // 選擇完畢，顯示主視窗 
                }
                else
                {
                    // 如果使用者直接按 X 關閉選擇視窗，代表不想玩了 -> 關閉整個程式
                    this.Close();
                }
            }
            AutoAppendLog("系統已啟動，準備錄入資料...");
            RefreshGrid();
        }

        private void dgvData_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {

        }

        private void tabView_Click(object sender, EventArgs e)
        {

        }

        private void tsmi_SetFolderPath_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.Description = "請選擇配方儲存位置";

                // 打開時，自動跳到上次設定的路徑，方便使用者
                dialog.SelectedPath = Properties.Settings.Default.DbFolderPath;

                // 如果使用者按下了 OK
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // 更新記憶體中的變數
                    Properties.Settings.Default.DbFolderPath = dialog.SelectedPath;

                    // ★關鍵：永久儲存設定 (下次開機還會在)
                    Properties.Settings.Default.Save();

                    MessageBox.Show($"路徑已變更為：\n{dialog.SelectedPath}");
                }
            }
        }

        private void tmrAutoRefresh_Tick(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabView)
            {
                RefreshGrid();
            }
        }
        private void AutoAppendLog(string Message)
        {
            string folderPath = Properties.Settings.Default.DbFolderPath;
            // 取得你的路徑 (假設你上次已經設定好 Properties.Settings.Default.UserFolderPath)
            // 檔名規則要跟寫入時一樣 (例如每天一個檔)


            string fileName = $"Log_{DateTime.Now:yyyyMMdd}.txt";
            string fullPath = Path.Combine(folderPath, fileName);
            // 檢查檔案是否存在
            if (!File.Exists(fullPath))
            {
                using (var fs = File.Create(fullPath))
                {
                    // 可選：立即釋放
                }
            }
            try
            {
                // ★關鍵技術：使用 FileStream 開啟，並設定 FileShare.ReadWrite
                // 這樣就算對方正在寫入，你也可以讀取，不會報錯！
                using (var fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var sr = new StreamReader(fs))
                {
                    string content = sr.ReadToEnd();

                    // 優化：只有內容不一樣時才更新 (避免畫面一直閃爍)
                    if (rtbLog.Text != content)
                    {
                        rtbLog.Text = content;

                        // 自動捲動到最下面，讓你看見最新的
                        rtbLog.SelectionStart = rtbLog.Text.Length;
                        rtbLog.ScrollToCaret();
                    }
                }
            }
            catch (Exception)
            {
                // 如果真的讀取失敗(極低機率)，就略過這次，等下一秒再試
            }
            // 4. 寫入內容
            string logContent = $"[{DateTime.Now:HH:mm:ss}] {Message}\r\n";
            try
            {
                File.AppendAllText(fullPath, logContent);

                // 寫完順便刷新畫面，自己也會看到
                RefreshGrid();
            }
            catch (Exception ex)
            {
                // 錯誤處理 (選用)
                 MessageBox.Show("寫入失敗: " + ex.Message);
            }
        }

    }
}


