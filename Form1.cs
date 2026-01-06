using System;
using System.Drawing;
using System.Windows.Forms;

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

            this.Load += (s, e) =>
            {
                _service.Initialize();
                UpdateCount(); // 顯示目前庫存
            };

            // --- 檢核區 (你用的) ---
            btnCheck.Click += BtnCheck_Click;
            // 假設檢核區的輸入框叫 txtInputCheck
            txtInput.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnCheck_Click(s, e); };

            // --- 錄入區 (同事用的) ---
            // 假設你在 Tab2 加了按鈕 btnAddData 和輸入框 txtInputAdd
            // 請確認你在設計畫面有這些元件，並綁定以下事件
             btnAddData.Click += BtnAdd_Click; 
             btnAddData.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnAdd_Click(s, e); };
        }

        // [檢核區] 你的功能：比對並刪除
        private void BtnCheck_Click(object sender, EventArgs e)
        {
            string input = txtInput.Text.Trim(); // 這裡用你的檢核輸入框
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

            txtInput.SelectAll();
            UpdateCount();
        }

        // [錄入區] 同事的功能：新增資料
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            // 這裡假設你有個輸入框給同事用，叫 txtInputAdd
            // string input = txtInputAdd.Text.Trim(); 
            // if (string.IsNullOrEmpty(input)) return;

            // 為了簡化，這裡先用隨機產生模擬同事錄入 (你可以改成真實輸入)
            // _service.AddData(input, "OK"); 

            // 模擬：產生一筆隨機的
            Random rnd = new Random();
            string code = "A" + rnd.Next(1000, 9999);
            string status = rnd.Next(100) < 80 ? "OK" : "NG";

            _service.AddData(code, status); // 寫入資料庫

            UpdateCount();
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
    }
}