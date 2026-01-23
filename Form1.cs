using System;
using System.Drawing;
using System.Windows.Forms;

namespace CheckDataSystem
{
        public partial class Form1 : Form
    {
        private CoreService _service;
        private NetworkService _network; // 新增網路服務
        private string _dbFolderPath = ""; // 記錄目前資料夾路徑

        public Form1()
        {
            InitializeComponent();
            _service = new CoreService();
            _network = new NetworkService(); // 初始化

            // 訂閱 Service 的 Log 事件，顯示到介面並寫檔
            _service.OnLogMessage += HandleLogMessage;
            _network.OnLog += HandleLogMessage; // 訂閱網路 Log
            _network.OnMessageReceived += HandleNetworkMessage; // 訂閱收到訊息事件

            // 事件綁定
            this.Load += Form1_Load;
            btnCheck.Click += BtnCheck_Click;
            btnAdd.Click += BtnAdd_Click;
            btnRefresh.Click += (s, e) => RefreshGrid();
            btnCancel.Click += BtnCancel_Click;

            // 支援 Enter 鍵
            txtInputCheck.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnCheck_Click(s, e); };
            txtInputAdd.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnAdd_Click(s, e); };

            // 定時更新
            tmrAutoRefresh.Interval = 2000;
            tmrAutoRefresh.Tick += tmrAutoRefresh_Tick;
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeSystemFlow();
        }

        /// <summary>
        /// 系統初始化流程：路徑 -> 連線 -> 登入 -> 設定介面
        /// </summary>
        private void InitializeSystemFlow()
        {
            // 1. 取得或設定路徑
            _dbFolderPath = Properties.Settings.Default.DbFolderPath;
            if (string.IsNullOrEmpty(_dbFolderPath) || !System.IO.Directory.Exists(_dbFolderPath))
            {
                if (!SelectAndSaveDbPath())
                {
                    MessageBox.Show("未設定路徑，程式即將關閉。");
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
                Properties.Settings.Default.DbFolderPath = ""; // 清除錯誤路徑
                Properties.Settings.Default.Save();
                this.Close();
                return;
            }

            // 3. 登入對話框
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
            if (role == "Up") // 上游端 (隱藏查詢功能)
            {
                tabControl1.TabPages.Remove(tabCheck);
                this.Text += " - [角色: 上游/錄入]";
                
                // 上游端：嘗試連線到伺服器 (這裡範例預設連線本機，實際可跳出輸入框)
                string serverIp = Microsoft.VisualBasic.Interaction.InputBox("請輸入下游(伺服器) IP 位址：", "連線設定", "127.0.0.1");
                if (!string.IsNullOrEmpty(serverIp))
                {
                    _network.ConnectToServer(serverIp, 5000);
                }
            }
            else if (role == "Down") // 下游端 (隱藏錄入功能)
            {
                tabControl1.TabPages.Remove(tabAdd);
                this.Text += " - [角色: 下游/查詢]";
                
                // 下游端：啟動伺服器
                _network.StartServer(5000);
            }
        }

        // [查詢端]
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

        // [錄入端]
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            string input = txtInputAdd.Text.Trim();
            //if (string.IsNullOrEmpty(input)) return;

            // 限制最大數量，避免濫用 (範例)
            int total = _service.GetCount();
            if (total >= 100)
            {
                MessageBox.Show("庫存過多，請先清除資料。");
                return;
            }
            
            // 模擬批次錄入10筆data
            Random rnd = new Random();
            for (int i = 0; i < 10; i++)
            {
                string code = "A" + rnd.Next(1000, 9999);
                string status = rnd.Next(100) < 80 ? "OK" : "NG";

                _service.AddData(code, status); // 寫入資料庫

            }

            txtInputAdd.Clear();
            txtInputAdd.Focus();
            UpdateCount();
            RefreshGrid();
        }

        // 處理收到的網路訊息 (通常在下游端觸發)
        private void HandleNetworkMessage(string msg)
        {
            // 因為是從網路執行緒回來，需 Invoke 到 UI 執行緒
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => HandleNetworkMessage(msg)));
                return;
            }

            // 解析訊息
            var parts = msg.Split('|');
            if (parts.Length == 3 && parts[0] == "ADD")
            {
                string code = parts[1];
                string status = parts[2];

                // 寫入本地資料庫
                _service.AddData(code, status);
                
                // 順便更新介面
                RefreshGrid();
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("確定要清空所有資料嗎？此動作無法復原。", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                _service.ClearAllData();
                UpdateCount();
                RefreshGrid();
            }
        }

        private void RefreshGrid()
        {
            // 只有在「資料庫總覽」分頁時才更新 Grid，節省效能
            //if (tabControl1.SelectedTab == tabView)
            //{
                var dt = _service.GetAllData();
                dgvData.DataSource = dt;

                // 讀取最新 Log 檔內容
                string fileLogs = LogService.ReadTodayLog(_dbFolderPath);
                // 1. 判斷使用者是否在最底部 (捲軸是否在最後面)
                bool userIsAtBottom = rtbLog.SelectionStart == rtbLog.Text.Length;

                // 2. 更新內容
                rtbLog.Text = fileLogs;

                // 3. 如果原本就在最下方時，才自動捲動
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

            // 2. 更新 UI (因為有 Timer 會自動讀取檔案更新，這裡可以選擇直接 Append，
            //    為了避免衝突，還是可以保留)
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => HandleLogMessage(message)));
                return;
            }

            // 簡單 Append 到 UI，讓使用者第一時間看到
            // 完整內容會由 Timer 從檔案讀回覆蓋
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
            // 定時更新 Grid 與 Log 畫面
            RefreshGrid();
        }

        // 這些自動產生的空函式如果沒用可以刪除
        private void tabView_Click(object sender, EventArgs e) { }
        private void dgvData_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void btnRefresh_Click(object sender, EventArgs e) { }
    }
}
