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
    public partial class RandevuIslem : Form
    {
        string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=esmacanimbenimCR7ilker ;Database=hastane_randevu;";

        public RandevuIslem()
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

        public void loadRandevu()
        {
            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            DataTable dt1 = new DataTable();
            NpgsqlDataAdapter da = new NpgsqlDataAdapter("select * from randevu ", conn);
            da.Fill(dt1);
            dataGridView1.DataSource = dt1;
        }

        private void RandevuIslem_Load(object sender, EventArgs e)
        {
            loadRandevu();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowIndex = e.RowIndex;

            // Geçerli bir satır seçildi mi kontrol et
            if (rowIndex >= 0)
            {
                // DataGridView'deki hücre değerlerini al ve TextBox'lara yaz

                txtID.Text = dataGridView1.Rows[rowIndex].Cells["RandevuID"].Value.ToString();
                textBox1.Text = dataGridView1.Rows[rowIndex].Cells["HastaID"].Value.ToString();
                cmbBrans.Text = dataGridView1.Rows[rowIndex].Cells["PoliklinikID"].Value.ToString();
                dateTimePicker1.Text = dataGridView1.Rows[rowIndex].Cells["tarih"].Value.ToString();
                maskedTextBox1.Text = dataGridView1.Rows[rowIndex].Cells["saat"].Value.ToString();
                rtxtAcıklama.Text = dataGridView1.Rows[rowIndex].Cells["aciklama"].Value.ToString();
                cmbDoktor.Text= dataGridView1.Rows[rowIndex].Cells["DoktorID"].Value.ToString();
            }
        }

        private void button_guncelle_Click(object sender, EventArgs e)
        {
            // Veritabanı bağlantısını oluştur
            var connection = new NpgsqlConnection(connectionString);

            try
            {
                connection.Open();

                // Güncelleme sorgusu
                string sorgu = "UPDATE \"randevu\" " +
                               "SET \"DoktorID\" = @doktorID, \"PoliklinikID\" = @poliklinikID,\"HastaID\" = @hastaID, \"tarih\" = @tarih, \"saat\" = @saat, \"aciklama\" = @aciklama " +
                               "WHERE \"RandevuID\" = @RandevuID";

                var komut = new NpgsqlCommand(sorgu, connection);

                // Parametreleri ekle
                komut.Parameters.AddWithValue("@doktorID",Convert.ToInt32(cmbDoktor.Text));
                komut.Parameters.AddWithValue("@RandevuID", Convert.ToInt32(txtID.Text));
                komut.Parameters.AddWithValue("@poliklinikID", Convert.ToInt32(cmbBrans.Text));
                komut.Parameters.AddWithValue("@tarih", dateTimePicker1.Value);
                komut.Parameters.AddWithValue("@saat", maskedTextBox1.Text);
                komut.Parameters.AddWithValue("@aciklama", rtxtAcıklama.Text);
                komut.Parameters.AddWithValue("@hastaID", Convert.ToInt32(textBox1.Text));

                // Sorguyu çalıştır
                int rowsAffected = komut.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Kayıt başarıyla güncellendi.");
                    loadRandevu();
                }
                else
                {
                    MessageBox.Show("Kayıt güncellenemedi. HastaID bulunamadı.");
                }
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

        private void button_sil_Click(object sender, EventArgs e)
        {
            // Veritabanı bağlantısını oluştur
            var connection = new NpgsqlConnection(connectionString);

            try
            {
                // Eğer TextBox boşsa silme işlemini engelle
                if (string.IsNullOrEmpty(txtID.Text))
                {
                    MessageBox.Show("Lütfen silmek istediğiniz kaydın ID değerini girin!");
                    return;
                }

                connection.Open();

                // Silme sorgusu
                string sorgu = "DELETE FROM \"randevu\" WHERE \"RandevuID\" = @randevuID";

                var komut = new NpgsqlCommand(sorgu, connection);

                // Parametre ekle
                komut.Parameters.AddWithValue("@randevuID", Convert.ToInt32(txtID.Text));

                // Silme işlemini gerçekleştir
                int rowsAffected = komut.ExecuteNonQuery();

                // İşlem sonucunu kontrol et
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Kayıt başarıyla silindi.");
                    loadRandevu(); // Tabloyu yenileme metodu
                }
                else
                {
                    MessageBox.Show("Silme işlemi başarısız. Belirtilen ID'ye ait kayıt bulunamadı.");
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Geçersiz bir ID değeri girdiniz. Lütfen sadece sayısal bir değer girin.");
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

    }
}
