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
    public partial class Doktor_panel : Form
    {

        string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=esmacanimbenimCR7ilker;Database=hastane_randevu;";
        public string doktorSifre;
        public string doktorTc;

        public Doktor_panel()
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
        private void RandevulariGetir()
        {
            var connection = new NpgsqlConnection(connectionString);

            try
            {
                connection.Open();

                // Doktorun randevularını getiren sorgu
                string sorgu = @"SELECT r.""RandevuID"", k.""ad"" AS ""HastaAd"", k.""soyad"" AS ""HastaSoyad"", 
                                    r.""tarih"", r.""saat"", r.""aciklama""
                             FROM ""randevu"" r
                             INNER JOIN ""kisi"" k ON r.""HastaID"" = k.""KisiID""
                             INNER JOIN ""doktor"" d ON r.""DoktorID"" = d.""DoktorID""
                             WHERE d.""tcNo"" = @doktorTcNo
                             ORDER BY r.""tarih"", r.""saat""";

                var komut = new NpgsqlCommand(sorgu, connection);
                komut.Parameters.AddWithValue("@doktorTcNo", doktorTc);

                var adapter = new NpgsqlDataAdapter(komut);

                // Sonuçları DataTable'a doldur
                var dataTable = new DataTable();
                adapter.Fill(dataTable);

                // DataGridView'e bağla
                dataGridView1.DataSource = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }
        private void Doktor_panel_Load(object sender, EventArgs e)
        {
            lblDoktorTC.Text= doktorTc;
            RandevulariGetir();
        }
    }
}
