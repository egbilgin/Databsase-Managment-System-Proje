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
using Npgsql;
using System.Runtime.InteropServices;

namespace vtys_proje
{
    public partial class SekreterPanel : Form
    {
        string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=esmacanimbenimCR7ilker ;Database=hastane_randevu;";
        public SekreterPanel()
        {
            InitializeComponent();
        }
        public string sekreterSifre;
        private void label9_Click(object sender, EventArgs e)
        {

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

        private void SekreterPanel_Load(object sender, EventArgs e)
        {
            loadBranslar();
            loadDoktor();
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    // Giriş yapan sekreterin bilgilerini almak için sorgu
                    string query = @"
                SELECT k.""tcNo"", k.""ad"", k.""soyad""
                FROM ""sekreter"" s
                INNER JOIN ""kisi"" k ON s.""SekreterID"" = k.""KisiID""
                WHERE s.""sekreterSifre"" = @sekreterSifre";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        // Sekreterin giriş yaptığı şifreyi parametre olarak ekle
                        cmd.Parameters.AddWithValue("@sekreterSifre", sekreterSifre);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Sekreterin TC, ad ve soyad bilgilerini label'lara yazdır
                                lblSekreterTC.Text = reader["tcNo"].ToString();
                                lblSekreterAD.Text = $"{reader["ad"]} {reader["soyad"]}";
                            }
                            else
                            {
                                MessageBox.Show("Sekreter bilgileri bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //branşları datagride aktarma
            NpgsqlConnection conn2 = new NpgsqlConnection(connectionString);
            DataTable dt1 = new DataTable();
            NpgsqlDataAdapter da = new NpgsqlDataAdapter("select * from poliklinik ", conn2);
            da.Fill(dt1);
            dataGridView2.DataSource = dt1;

            //soktorları aktrma
            NpgsqlConnection conn3 = new NpgsqlConnection(connectionString);
            DataTable dt2 = new DataTable();
            NpgsqlDataAdapter da2 = new NpgsqlDataAdapter("SELECT \r\n    k.\"KisiID\", \r\n    k.\"tcNo\", \r\n    k.\"ad\", \r\n    k.\"soyad\", \r\n      d.\"DoktorID\", \r\n   d.\"PoliklinikID\"\r\nFROM \r\n    \"kisi\" k\r\nINNER JOIN \r\n    \"doktor\" d \r\nON \r\n    k.\"KisiID\" = d.\"DoktorID\";\r\n ", conn3);
            da2.Fill(dt2);
            dataGridView1.DataSource = dt2;



        }

        public void loadBranslar()
        {
            string query = "Select * from \"poliklinik\" ";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                connection.Open();
                NpgsqlDataReader reader = command.ExecuteReader();

                DataTable dataTable = new DataTable();
                dataTable.Load(reader);

                cmbBrans.DisplayMember = "adi";
                cmbBrans.ValueMember = "PoliklinikID";
                cmbBrans.DataSource = dataTable;

                connection.Close();
            }
        }

        public void loadDoktor()
        {
            string query = "Select * from \"kisi\" where \"kisi\".\"doktorMu\"=@p1";

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                connection.Open();
                command.Parameters.AddWithValue("@p1", true);
                NpgsqlDataReader reader = command.ExecuteReader();

                DataTable dataTable = new DataTable();
                dataTable.Load(reader);

                cmbDoktor.DisplayMember = "ad";
                cmbDoktor.ValueMember = "KisiID";
                cmbDoktor.DataSource = dataTable;

                connection.Close();
            }
        }
        private void cmbBrans_SelectedIndexChanged(object sender, EventArgs e)
        {
            //cmbDoktor.Items.Clear();A

            //NpgsqlCommand komut1 = new NpgsqlCommand("select1");
        }

        private void mskTarih_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            // Veritabanı bağlantısı oluştur
            var connection = new NpgsqlConnection(connectionString);

            try
            {
                connection.Open();

                // HastaTC'ye göre kisi tablosundan ID değerini al
                string selectSorgu = "SELECT \"KisiID\" FROM \"kisi\" WHERE \"tcNo\" = @tc";
                var selectKomut = new NpgsqlCommand(selectSorgu, connection);
                selectKomut.Parameters.AddWithValue("@tc", mskHastaTC.Text);

                // ID'yi al
                object hastaIdObj = selectKomut.ExecuteScalar();

                if (hastaIdObj == null)
                {
                    MessageBox.Show("Girilen TC kimlik numarasına ait hasta bulunamadı!");
                    return;
                }

                int hastaID = Convert.ToInt32(hastaIdObj);

                // Randevu ekleme sorgusu
                string insertSorgu = "INSERT INTO \"randevu\" (\"HastaID\", \"DoktorID\", \"PoliklinikID\", \"tarih\", \"saat\", \"aciklama\",\"durum\") VALUES (@p1, @p2, @p3, @p4, @p5, @p6,@p7)";
                var insertKomut = new NpgsqlCommand(insertSorgu, connection);

                insertKomut.Parameters.AddWithValue("@p1", hastaID); // Bulunan HastaID'yi kullan
                insertKomut.Parameters.AddWithValue("@p2", cmbDoktor.SelectedValue ?? DBNull.Value);
                insertKomut.Parameters.AddWithValue("@p3", cmbBrans.SelectedValue ?? DBNull.Value);
                insertKomut.Parameters.AddWithValue("@p4", dateTimePicker1.Value);
                insertKomut.Parameters.AddWithValue("@p5", maskedTextBox_saat.Text);
                insertKomut.Parameters.AddWithValue("@p6", rtxtAcıklama.Text);
                insertKomut.Parameters.AddWithValue("@p7", true);

                insertKomut.ExecuteNonQuery();
                MessageBox.Show("Randevu kaydedildi.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Bir hata oluştu: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            RandevuIslem frm = new RandevuIslem();
            frm.ShowDialog();   
            this.Hide();
        }

       
    }
}




