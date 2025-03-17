namespace NetworkScanner
{
    partial class MainForm
    {
        private MaterialSkin.Controls.MaterialButton btnScan;
        private MaterialSkin.Controls.MaterialButton btnShutdown;
        private MaterialSkin.Controls.MaterialListBox materialListBox;
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnScan = new MaterialSkin.Controls.MaterialButton();
            this.btnShutdown = new MaterialSkin.Controls.MaterialButton();
            this.materialListBox = new MaterialSkin.Controls.MaterialListBox();
            this.SuspendLayout();

            // 
            // btnScan (Ağı Tara Butonu)
            // 
            this.btnScan.AutoSize = false;
            this.btnScan.Size = new System.Drawing.Size(160, 60);
            this.btnScan.Location = new System.Drawing.Point(620, 200);
            this.btnScan.Text = "🔍 Ağı Tara";
            this.btnScan.UseAccentColor = true;
            this.btnScan.Click += new System.EventHandler(this.btnScan_Click);

            // 
            // btnShutdown (PC Kapatma Butonu)
            // 
            this.btnShutdown.AutoSize = false;
            this.btnShutdown.Size = new System.Drawing.Size(160, 60);
            this.btnShutdown.Location = new System.Drawing.Point(620, 300);
            this.btnShutdown.Text = "🛑 Seçili PC'yi Kapat";
            this.btnShutdown.UseAccentColor = true;
            this.btnShutdown.Click += new System.EventHandler(this.btnShutdown_Click);

            // 
            // materialListBox (IP Listesi) - Daha Aşağı Kaydırıldı
            // 
            this.materialListBox.Location = new System.Drawing.Point(20, 100); // 50px aşağı çekildi
            this.materialListBox.Size = new System.Drawing.Size(600, 600);
            this.materialListBox.BackColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this.materialListBox.ForeColor = System.Drawing.Color.White;

            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(800, 800);
            this.Controls.Add(this.btnScan);
            this.Controls.Add(this.btnShutdown);
            this.Controls.Add(this.materialListBox);
            this.Text = "⚡ Ağ Tarayıcı & PC Kapatıcı";
            this.ResumeLayout(false);
        }
    }
}
