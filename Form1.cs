using System;
using System.Drawing;
using System.Windows.Forms;

namespace CheckDataSystem
{
    public partial class Form1 : Form
    {
        private CoreService _service;
        private string _dbFolderPath = ""; // 快取目前的路徑

        public Form1()
        {
            InitializeComponent();
            _service = new CoreService();

            // 訂閱 Service 的 Log 事件，顯示到介面並寫檔
            _service.OnLogMessage += HandleLogMessage;

            // 事件綁定
            this.Load += Form1_Load;
            btnCheck.Click += BtnCheck_Click;
            btnAdd.Click += BtnAdd_Click;
            btnRefresh.Click += (s, e) => RefreshGrid();

            // 支援 Enter 鍵
            txtInputCheck.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnCheck_Click(s, e); };
            txtInputAdd.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnAdd_Click(s, e); };

            // 定時刷新
            tmrAutoRefresh.Interval = 2000;
            tmrAutoRefresh.Tick += tmrAutoRefresh_Tick;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeSystemFlow();
        }

        /// <summary>
        /// 系統初始化流程：選路徑 -> 連線 -> 登入 -> 設定介面
        /// </summary>
        private void InitializeSystemFlow()
        {
            // 1. 取得或設定路徑
            _dbFolderPath = Properties.Settings.Default.DbFolderPath;
            if (string.IsNullOrEmpty(_dbFolderPath) || !System.IO.Directory.Exists(_dbFolderPath))
            {
                if (!SelectAndSaveDbPath())
                {
                    MessageBox.Show("未選擇路徑，程式將關閉。");
                    this.Close();
                    return;
                }
            }

            // 2. 初始化 Service
            try
            {
                _service.Initialize(_dbFolderPath);
            }
            catch
            {
                MessageBox.Show("資料庫連線失敗，請檢查路徑權限。");
                Properties.Settings.Default.DbFolderPath = ""; // 清空錯誤路徑
                Properties.Settings.Default.Save();
                this.Close();
                return;
            }

            // 3. 登入選身份
            Hide(); // 隱藏主視窗
            using (LoginForm login = new LoginForm())
            {
                if (login.ShowDialog() == DialogResult.OK)
                {
                    ConfigureUiByRole(login.SelectedRole);
                    Show(); // 顯示主視窗
                    HandleLogMessage("系統已啟動，準備就緒。");
                    RefreshGrid();
                }
                else
                {
                    this.Close();
                }
            }
        }

        /// <summary>
        /// 選擇資料庫資料夾
        /// </summary>
        private bool SelectAndSaveDbPath()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "請選擇資料庫 (data.db) 存放的共用資料夾";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _dbFolderPath = dialog.SelectedPath;
                    Properties.Settings.Default.DbFolderPath = _dbFolderPath;
                    Properties.Settings.Default.Save();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 根據身分調整介面
        /// </summary>
        private void ConfigureUiByRole(string role)
        {
            if (role == "Up") // 檢核員 (移除錄入功能)
            {
                tabControl1.TabPages.Remove(tabCheck);
                this.Text += " - [身分: 上游/檢核]";
            }
            else if (role == "Down") // 錄入員 (移除檢核功能)
            {
                tabControl1.TabPages.Remove(tabAdd);
                this.Text += " - [身分: 下游/錄入]";
            }
        }

        // [檢核區]
        private void BtnCheck_Click(object sender, EventArgs e)
        {
            string input = txtInputCheck.Text.Trim();
            if (string.IsNullOrEmpty(input)) return;

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

            txtInputCheck.Clear();
            UpdateCount();
            RefreshGrid();
        }

        // [錄入區]
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            string input = txtInputAdd.Text.Trim();
            //if (string.IsNullOrEmpty(input)) return;

            // 限制最大筆數，防止濫用 (可選)
            int total = _service.GetCount();
            if (total >= 100)
            {
                MessageBox.Show("庫存過多，請先消化資料。");
                return;
            }
            
            //先做亂數10筆當data用
            for (int i = 0; i < 10; i++)
            {

                Random rnd = new Random();
                string code = "A" + rnd.Next(1000, 9999);
                string status = rnd.Next(100) < 80 ? "OK" : "NG";

                _service.AddData(code, status); // 寫入資料庫

            }

            txtInputAdd.Clear();
            txtInputAdd.Focus();
            UpdateCount();
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            // 只有在「總覽」分頁時才更新 Grid，節省效能
            //if (tabControl1.SelectedTab == tabView)
            //{
                var dt = _service.GetAllData();
                dgvData.DataSource = dt;

                // 讀取最新的 Log 檔內容顯示
                string fileLogs = LogService.ReadTodayLog(_dbFolderPath);
                // 1. 判斷使用者是否正在看舊資料 (游標是否在最後面)
                bool userIsAtBottom = rtbLog.SelectionStart == rtbLog.Text.Length;

                // 2. 更新內容
                rtbLog.Text = fileLogs;

                // 3. 只有原本就在最下面時，才自動捲動
                if (userIsAtBottom)
                {
                    rtbLog.SelectionStart = rtbLog.Text.Length;
                    rtbLog.ScrollToCaret();
                }
            //}
            UpdateCount();
        }

        private void UpdateCount()
        {
            int count = _service.GetCount();
            lblTotalCount.Text = $"目前庫存: {count} 筆";
            // 也可以更新視窗標題
            // this.Text = $"檢核系統 - 庫存: {count}";
        }

        // 統一處理 Log (來自 Service 或 Form)
        private void HandleLogMessage(string message)
        {
            // 1. 寫入檔案
            LogService.WriteToFile(_dbFolderPath, message);

            // 2. 更新 UI (因為有 Timer 會自動讀取檔案更新，這裡其實可以不用即時 Append，
            //    但為了反應速度，還是可以保留)
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => HandleLogMessage(message)));
                return;
            }

            // 簡單 Append 到 UI，讓使用者知道剛才發生什麼事
            // 完整內容會由 Timer 從檔案讀回來覆蓋
            string time = DateTime.Now.ToString("HH:mm:ss");
            // rtbLog.AppendText($"[{time}] {message}\r\n"); 
        }

        private void tsmi_SetFolderPath_Click(object sender, EventArgs e)
        {
            if (SelectAndSaveDbPath())
            {
                MessageBox.Show($"路徑已變更為：\n{_dbFolderPath}\n請重新啟動程式以套用。");
            }
        }

        private void tmrAutoRefresh_Tick(object sender, EventArgs e)
        {
            // 定時刷新 Grid 和 Log 畫面
            RefreshGrid();
        }

        // 這些自動生成的空事件如果沒用可以刪除
        private void tabView_Click(object sender, EventArgs e) { }
        private void dgvData_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void btnRefresh_Click(object sender, EventArgs e) { }
    }
}