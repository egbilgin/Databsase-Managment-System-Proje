using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace vtys_proje
{
    public partial class hasta_kayit : Form
    {
        string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=esmacanimbenimCR7ilker ;Database=hastane_randevu;";
        public hasta_kayit()
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
        private void btnKayitYap_Click(object sender, EventArgs e)
        {
            var connection = new NpgsqlConnection(connectionString);

            if (string.IsNullOrWhiteSpace(txtAd.Text) ||
        string.IsNullOrWhiteSpace(txtSoyad.Text) ||
        string.IsNullOrWhiteSpace(mskTC.Text) ||
        string.IsNullOrWhiteSpace(mskTel.Text) ||
        string.IsNullOrWhiteSpace(txtSifre.Text) ||
        cmbCinsiyet.SelectedIndex == -1 ||
        dtpTarih.Value == dtpTarih.MinDate)
            {
                MessageBox.Show("Lütfen tüm alanları doldurunuz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // 1. Kişi Tablosuna Ekleme
                    string insertKisiQuery = @"INSERT INTO ""kisi"" 
                (""ad"", ""soyad"", ""tcNo"", ""hastaMi"", ""doktorMu"", ""refakatciMi"", ""sekreterMi"") 
                VALUES (@ad, @soyad, @tcNo, @hastaMi, @doktorMu, @refakatciMi, @sekreterMi)
                RETURNING ""KisiID"";";

                    int kisiID;
                    using (NpgsqlCommand cmdKisi = new NpgsqlCommand(insertKisiQuery, conn))
                    {
                        cmdKisi.Parameters.AddWithValue("@ad", txtAd.Text);
                        cmdKisi.Parameters.AddWithValue("@soyad", txtSoyad.Text);
                        cmdKisi.Parameters.AddWithValue("@tcNo", mskTC.Text);
                        cmdKisi.Parameters.AddWithValue("@hastaMi", true); // Hasta olduğu için true
                        cmdKisi.Parameters.AddWithValue("@doktorMu", false);
                        cmdKisi.Parameters.AddWithValue("@refakatciMi", false);
                        cmdKisi.Parameters.AddWithValue("@sekreterMi", false);

                        // RETURNING ile dönen KisiID değerini alıyoruz
                        kisiID = Convert.ToInt32(cmdKisi.ExecuteScalar());
                    }

                    // 2. Hasta Tablosuna Ekleme
                    string insertHastaQuery = @"INSERT INTO ""hasta"" 
                (""HastaID"", ""dogumTarihi"", ""cinsiyet"", ""telNo"", ""HastaSifre"") 
                VALUES (@HastaID, @dogumTarihi, @cinsiyet, @telNo, @HastaSifre);";

                    using (NpgsqlCommand cmdHasta = new NpgsqlCommand(insertHastaQuery, conn))
                    {
                        cmdHasta.Parameters.AddWithValue("@HastaID", kisiID);
                        cmdHasta.Parameters.AddWithValue("@dogumTarihi", dtpTarih.Value);
                        cmdHasta.Parameters.AddWithValue("@cinsiyet", cmbCinsiyet.SelectedItem.ToString());
                        cmdHasta.Parameters.AddWithValue("@telNo", mskTel.Text);
                        cmdHasta.Parameters.AddWithValue("@HastaSifre", txtSifre.Text);

                        cmdHasta.ExecuteNonQuery();
                    }

                    MessageBox.Show("Hasta başarıyla kaydedildi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void hasta_kayit_Load(object sender, EventArgs e)
        {

        }
    }
}
