using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace NetworkScanner
{
    public partial class MainForm : MaterialForm
    {
        public MainForm()
        {
            InitializeComponent();

            // MaterialSkin tasarım ayarları
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Blue500, Primary.Blue700, Primary.Blue200, Accent.LightBlue200, TextShade.WHITE);

            // MaterialListBox daha büyük olsun
            materialListBox.Size = new System.Drawing.Size(600, 600);
        }

        private async void btnScan_Click(object sender, EventArgs e)
        {
            materialListBox.Items.Clear();
            string localIPBase = GetLocalIPAddressBase();

            if (localIPBase == null)
            {
                MessageBox.Show("Ağ bağlantısı bulunamadı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<Task> scanTasks = new List<Task>();

            for (int i = 1; i <= 254; i++)
            {
                string ip = $"{localIPBase}.{i}";
                scanTasks.Add(Task.Run(() => ScanIP(ip)));
            }

            await Task.WhenAll(scanTasks);
        }

        private void ScanIP(string ip)
        {
            if (PingHost(ip))
            {
                string deviceType = GetDeviceType(ip);
                Invoke(new Action(() =>
                {
                    materialListBox.Items.Add(new MaterialListBoxItem($"{ip} - {deviceType}"));
                }));
            }
        }

        private bool PingHost(string ip)
        {
            try
            {
                using (Ping ping = new Ping())
                {
                    PingReply reply = ping.Send(ip, 100);
                    return reply.Status == IPStatus.Success;
                }
            }
            catch
            {
                return false;
            }
        }

        private string GetDeviceType(string ip)
        {
            try
            {
                // MAC adresini alıp cihaza göre sınıflandırma yapalım
                string macAddress = GetMacAddress(ip);
                if (macAddress != null)
                {
                    if (macAddress.StartsWith("00:1A:79") || macAddress.StartsWith("00:1D:D5")) // Örnek MAC
                        return "Smart TV 📺";
                    if (macAddress.StartsWith("A4:50:46") || macAddress.StartsWith("AC:5F:3E")) // Örnek MAC
                        return "Telefon 📱";
                }

                // Windows cihazı mı kontrol edelim
                ManagementScope scope = new ManagementScope($"\\\\{ip}\\root\\cimv2");
                scope.Connect();

                ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_ComputerSystem");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query);
                ManagementObjectCollection queryCollection = searcher.Get();

                foreach (ManagementObject m in queryCollection)
                {
                    string type = m["PCSystemType"]?.ToString();
                    return type switch
                    {
                        "1" => "Desktop 🖥",
                        "2" => "Laptop 💻",
                        "3" => "Server 🖲",
                        _ => "Bilinmeyen Cihaz"
                    };
                }
            }
            catch
            {
                return "Ağ Cihazı 🌐 (Router, Switch, Printer?)";
            }

            return "Bilinmeyen Cihaz";
        }

        private string GetMacAddress(string ip)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "arp";
                process.StartInfo.Arguments = "-a " + ip;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                string[] lines = output.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in lines)
                {
                    if (line.Contains(ip))
                    {
                        string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length >= 2)
                        {
                            return parts[1].ToUpper(); // MAC adresi
                        }
                    }
                }
            }
            catch
            {
                return null;
            }

            return null;
        }

        private string GetLocalIPAddressBase()
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus == OperationalStatus.Up &&
                    ni.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                {
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            string ipBase = ip.Address.ToString();
                            return ipBase.Substring(0, ipBase.LastIndexOf('.'));
                        }
                    }
                }
            }
            return null;
        }

        private void btnShutdown_Click(object sender, EventArgs e)
        {
            if (materialListBox.SelectedItem == null)
            {
                MessageBox.Show("Lütfen kapatılacak bir cihaz seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedEntry = materialListBox.SelectedItem.Text;
            string ip = selectedEntry.Split(' ')[0]; // IP'yi al

            DialogResult result = MessageBox.Show($"{ip} adresindeki bilgisayarı kapatmak istediğinize emin misiniz?",
                "PC Kapatma Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                ShutdownRemotePC(ip);
            }
        }

        private void ShutdownRemotePC(string ip)
        {
            try
            {
                Process.Start("shutdown", $"/s /m \\\\{ip} /t 10 /f");
                MessageBox.Show($"{ip} kapatma komutu gönderildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bilgisayarı kapatma başarısız oldu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
