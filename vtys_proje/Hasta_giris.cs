using System;
using System.Windows.Forms;
using Npgsql;

namespace vtys_proje
{
    public partial class Hasta_giris : Form
    {
        string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=esmacanimbenimCR7ilker;Database=hastane_randevu;";

        public Hasta_giris()
        {
            InitializeComponent();
        }

        // Ana Menüye Dönüş
        private void anaMenuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Main main = new Main();
            main.Show();
            this.Close();
        }

        // Çıkış İşlemi
        private void çıkışToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Kayıt Formuna Geçiş
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            hasta_kayit frm = new hasta_kayit();
            frm.Show();
        }

        // Hasta Giriş Kontrolü
        private void btnGirisYap_Click(object sender, EventArgs e)
        {
            // Kullanıcı Girişi Kontrol
            if (string.IsNullOrWhiteSpace(mskTel.Text) || string.IsNullOrWhiteSpace(txtSifre.Text))
            {
                MessageBox.Show("Lütfen telefon numarası ve şifre alanlarını doldurun!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Veritabanı Bağlantısı
                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "SELECT COUNT(*) FROM hasta WHERE \"telNo\" = @p1 AND \"HastaSifre\" = @p2";
                    using (NpgsqlCommand command = new NpgsqlCommand(query, conn))
                    {
                        // Parametreler
                        command.Parameters.AddWithValue("@p1", mskTel.Text.Trim());
                        command.Parameters.AddWithValue("@p2", txtSifre.Text.Trim());

                        // Sorgu Sonucu
                        int result = Convert.ToInt32(command.ExecuteScalar());

                        if (result > 0)
                        {
                            // Hasta Paneline Geçiş
                            Hasta_panel form = new Hasta_panel
                            {
                                tel = mskTel.Text.Trim() // Telefon numarasını aktar
                                
                            };
                            form.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Hatalı telefon numarası veya şifre!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Kullanıcıya Gösterilecek Hata
                MessageBox.Show($"Giriş sırasında bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Hasta_giris_Load(object sender, EventArgs e)
        {

        }
    }
}
