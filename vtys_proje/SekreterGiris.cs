using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace vtys_proje
{
    public partial class SekreterGiris : Form
    {
        public string sekreterSifre;
        string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=esmacanimbenimCR7ilker ;Database=hastane_randevu;";
        public SekreterGiris()
        {
            InitializeComponent();
        }

        private void çıkışToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void anaMenuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Main main = new Main();
            main.Show();

            // Mevcut formu kapat
            this.Close();
        }

        
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    // Sekreterin bilgilerini kontrol eden sorgu
                    string query = @"
                SELECT s.""SekreterID""
                FROM ""sekreter"" s
                INNER JOIN ""kisi"" k ON s.""SekreterID"" = k.""KisiID""
                WHERE k.""tcNo"" = @tcNo AND s.""sekreterSifre"" = @sifre";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        // Kullanıcının girdiği değerleri parametre olarak ekle
                        cmd.Parameters.AddWithValue("@tcNo", mskTc.Text.Trim()); // TC No
                        cmd.Parameters.AddWithValue("@sifre", txtSifre.Text.Trim()); // Şifre

                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            // Giriş başarılı
                            MessageBox.Show("Giriş başarılı. Hoş geldiniz!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Yeni forma geçiş yapabilirsiniz
                            SekreterPanel sekreterPanel = new SekreterPanel
                            {
                                sekreterSifre = txtSifre.Text.Trim() // Girişte kullanılan şifre
                            };
                            sekreterPanel.Show();
                            this.Hide();

                            sekreterPanel.Show();
                            this.Hide();
                        }
                        else
                        {
                            // Giriş başarısız
                            MessageBox.Show("TC kimlik numarası veya şifre hatalı. Lütfen tekrar deneyiniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void SekreterGiris_Load(object sender, EventArgs e)
        {
           
        }
    }
}
