using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CheckDataSystem
{
    public class NetworkService
    {
        private TcpListener _server;
        private TcpClient _client;
        private StreamReader _reader;
        private StreamWriter _writer;
        
        public event Action<string> OnMessageReceived;
        public event Action<string> OnLog;

        // 啟動伺服器 (下游端使用)
        public async void StartServer(int port)
        {
            try
            {
                _server = new TcpListener(IPAddress.Any, port);
                _server.Start();
                Log($"伺服器啟動，監聽 Port: {port}...");

                while (true)
                {
                    var client = await _server.AcceptTcpClientAsync();
                    Log("上游端已連線！");
                    _ = HandleClientAsync(client); // 處理連線，不阻塞主執行緒
                }
            }
            catch (Exception ex)
            {
                Log($"伺服器錯誤: {ex.Message}");
            }
        }

        // 連線到伺服器 (上游端使用)
        public bool ConnectToServer(string ip, int port)
        {
            try
            {
                _client = new TcpClient();
                _client.Connect(ip, port);
                var stream = _client.GetStream();
                _reader = new StreamReader(stream, Encoding.UTF8);
                _writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
                Log($"已連線到伺服器 {ip}:{port}");
                return true;
            }
            catch (Exception ex)
            {
                Log($"連線失敗: {ex.Message}");
                return false;
            }
        }

        // 傳送訊息
        public void Send(string message)
        {
            if (_client != null && _client.Connected)
            {
                try
                {
                    _writer.WriteLine(message);
                }
                catch (Exception ex)
                {
                    Log($"傳送失敗: {ex.Message}");
                }
            }
            else
            {
                Log("尚未連線，無法傳送。");
            }
        }

        // 處理接收到的訊息 (伺服器端邏輯)
        private async Task HandleClientAsync(TcpClient client)
        {
            using (client)
            using (var stream = client.GetStream())
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                try
                {
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        OnMessageReceived?.Invoke(line);
                    }
                }
                catch (Exception ex)
                {
                    Log($"連線中斷: {ex.Message}");
                }
            }
        }

        private void Log(string msg) => OnLog?.Invoke($"[TCP] {msg}");

        public void Stop()
        {
            _server?.Stop();
            _client?.Close();
        }
    }
}