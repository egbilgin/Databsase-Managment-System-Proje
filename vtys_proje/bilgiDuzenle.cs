using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace vtys_proje
{
    public partial class HastabilgiDuzenle : Form
    {
        public string telNo;

        string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=esmacanimbenimCR7ilker ;Database=hastane_randevu;";
        public HastabilgiDuzenle()
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

      

       
        private void HastabilgiDuzenle_Load(object sender, EventArgs e)
        {
            mskTel.Text= telNo;
            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            conn.Open();
            //string telNo = Hasta_giris.mskTel.Text.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");

            // Telefon numarasına göre hem kisi hem de hasta bilgilerini alacak sorgu
            string query = @"
SELECT k.""ad"", k.""soyad"", k.""tcNo"", h.""cinsiyet"", h.""dogumTarihi""
FROM ""kisi"" k
INNER JOIN ""hasta"" h ON k.""KisiID"" = h.""HastaID""
WHERE h.""telNo"" = @telNo";  // telNo'yu hasta tablosundan alıyoruz

            NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@telNo", mskTel.Text); // Maskeli Tel No

            NpgsqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                // Kisi tablosu verileri
                txtAd.Text = dr["ad"].ToString();
                txtSoyad.Text = dr["soyad"].ToString();
                mskTc.Text = dr["tcNo"].ToString();

                // Hasta tablosu verileri
                cmbCinsiyet.SelectedItem = dr["cinsiyet"].ToString();
                dtpTarih.Value = Convert.ToDateTime(dr["dogumTarihi"]);
            }
            else
            {
                MessageBox.Show("Bu telefon numarasına ait bir kayıt bulunamadı.");
            }
            dr.Close();
            conn.Close();

            try
            {
                using (NpgsqlConnection conn5 = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    // Telefon numarasına göre KisiID'yi al
                    string query2 = @"SELECT ""KisiID"" FROM ""hasta"" WHERE ""telNo"" = @telNo";
                    using (NpgsqlCommand cmd2 = new NpgsqlCommand(query, conn5))
                    {
                        cmd.Parameters.AddWithValue("@telNo", mskTel.Text.Trim());
                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            KisiID = Convert.ToInt32(result); // KisiID'yi global değişkene ata
                        }
                        else
                        {
                            MessageBox.Show("Bu telefon numarasına ait bir kayıt bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }



            //NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            //mskTel.Text = telNo;
            //conn.Open();   

            //NpgsqlCommand cmd = new NpgsqlCommand("select * from kisi where \"tcNo\"=@p1 ", conn);
            //cmd.Parameters.AddWithValue("@p1", mskTc.Text);
            //NpgsqlDataReader dr = cmd.ExecuteReader();
            //while (dr.Read())
            //{
            //    txtAd.Text= dr[1].ToString();
            //    txtSoyad.Text= dr[2].ToString();
            //    mskTc.Text= dr[3].ToString();
            //}

            //NpgsqlCommand cmd2 = new NpgsqlCommand("select * from hasta where tcNo=@p1 ", conn);
            //cmd.Parameters.AddWithValue("@p1", mskTc.Text);
            //NpgsqlDataReader dr2 = cmd.ExecuteReader();
            //while (dr.Read())
            //{
            //    txtAd.Text = dr2[1].ToString();
            //    txtSoyad.Text = dr2[2].ToString();
            //    mskTc.Text = dr2[3].ToString();
            //}

        }

        public int KisiID;
        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            try
            {

                //bölüm11
                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();

                    // Kisi tablosunu güncelleme sorgusu
                    string updateKisiQuery = @"
                UPDATE ""kisi""
                SET ""ad"" = @ad, 
                    ""soyad"" = @soyad,
                    ""tcNo"" = @tcNo
                WHERE ""KisiID"" = @kisiID";

                    // Hasta tablosunu güncelleme sorgusu
                    string updateHastaQuery = @"
                UPDATE ""hasta""
                SET ""cinsiyet"" = @cinsiyet, 
                    ""dogumTarihi"" = @dogumTarihi
                WHERE ""HastaID"" = @kisiID";

                    using (NpgsqlCommand cmdKisi = new NpgsqlCommand(updateKisiQuery, conn))
                    using (NpgsqlCommand cmdHasta = new NpgsqlCommand(updateHastaQuery, conn))
                    {
                        cmdKisi.Parameters.AddWithValue("@ad", txtAd.Text.Trim());
                        cmdKisi.Parameters.AddWithValue("@soyad", txtSoyad.Text.Trim());
                        cmdKisi.Parameters.AddWithValue("@tcNo", mskTc.Text.Trim());
                        cmdKisi.Parameters.AddWithValue("@kisiID", KisiID);

                        cmdHasta.Parameters.AddWithValue("@cinsiyet", cmbCinsiyet.Text.Trim());
                        cmdHasta.Parameters.AddWithValue("@dogumTarihi", dtpTarih.Value);
                        cmdHasta.Parameters.AddWithValue("@kisiID", KisiID);


                        // Sorguları çalıştır
                        cmdKisi.ExecuteNonQuery();
                        cmdHasta.ExecuteNonQuery();
                    }

                    MessageBox.Show("Bilgiler başarıyla güncellendi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Güncelleme sırasında bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
