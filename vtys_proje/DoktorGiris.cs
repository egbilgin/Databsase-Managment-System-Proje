using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vtys_proje
{
    public partial class DoktorGiris : Form
    {
        public string doktorSifre;
        string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=esmacanimbenimCR7ilker ;Database=hastane_randevu;";
        public DoktorGiris()
        {
            InitializeComponent();
        }

        private void anaMenuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Main main = new Main();
            main.Show();

            // Mevcut formu kapat
            this.Close();
        }

        private void çıkışToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
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
                SELECT d.""DoktorID""
                FROM ""doktor"" d
                INNER JOIN ""kisi"" k ON d.""DoktorID"" = k.""KisiID""
                WHERE k.""tcNo"" = @tcNo AND d.""doktorSifre"" = @sifre";

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
                            Doktor_panel doktorPanel = new Doktor_panel
                            {
                                doktorSifre = txtSifre.Text.Trim() // Girişte kullanılan şifre

                            };
                            doktorPanel.doktorTc = mskTc.Text;
                            doktorPanel.Show();
                            this.Hide();

                            doktorPanel.Show();
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
    }
}
