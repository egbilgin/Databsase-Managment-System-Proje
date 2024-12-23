using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Npgsql;

namespace vtys_proje
{


    public partial class Hasta_panel : Form

    {

        string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=esmacanimbenimCR7ilker;Database=hastane_randevu;";
        public string tel;

        public Hasta_panel()
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
            this.Close();
        }

        private void Hasta_panel_Load(object sender, EventArgs e)
        {
            lblTel.Text = tel;

            // Ad ve Soyad Bilgisini Getir
            GetAdSoyad();

            // Randevu Geçmişini Yükle
            LoadRandevuGecmisi();

            // Branşları Çekme
            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            conn.Open();
            NpgsqlCommand komut2= new NpgsqlCommand("Select adi from poliklinik", conn);
           
            NpgsqlDataReader dr2 = komut2.ExecuteReader();
            while (dr2.Read())
            {
                cmbBrans.Items.Add(dr2[0]);
            }
            conn.Close();
        }

        private void GetAdSoyad()
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT k.\"ad\", k.\"soyad\" " +
                                   "FROM \"kisi\" k " +
                                   "JOIN \"hasta\" h ON k.\"KisiID\" = h.\"HastaID\" " +
                                   "WHERE h.\"telNo\" = @p1";

                    using (NpgsqlCommand command = new NpgsqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@p1", tel);
                        using (NpgsqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lblAD.Text = $"{reader["ad"]} {reader["soyad"]}";
                            }
                            else
                            {
                                lblAD.Text = "Ad Soyad Bulunamadı";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ad Soyad bilgisi alınırken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadRandevuGecmisi()
        {
            try
            {
                using (NpgsqlConnection conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM \"randevu\" WHERE \"HastaID\" = (SELECT \"HastaID\" FROM \"hasta\" WHERE \"telNo\" = @p1)";

                    using (NpgsqlDataAdapter da = new NpgsqlDataAdapter(query, conn))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@p1", tel);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dataGridView1.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Randevu geçmişi yüklenirken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbBrans_SelectedIndexChanged(object sender, EventArgs e)
        {

            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            conn.Open();
            cmbDoktor.Items.Clear();
            NpgsqlCommand cmd = new NpgsqlCommand("SELECT k.\"ad\", k.\"soyad\"\r\nFROM \"kisi\" k\r\nJOIN \"doktor\" d ON k.\"KisiID\" = d.\"DoktorID\"\r\nWHERE d.\"brans\" = @brans;\r\n ", conn);
            cmd.Parameters.AddWithValue("@brans", cmbBrans.Text);
            NpgsqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                cmbDoktor.Items.Add(dr[0] + " " + dr[1]);
            }
            conn.Close();

        }

        private void cmbDoktor_SelectedIndexChanged(object sender, EventArgs e)
        {
            //DataTable dt = new DataTable();

            //RANDEVU ALMA BUTONU SORULACAK!!!! 
          //  NpgsqlDataAdapter da= new NpgsqlDataAdapter("select * from randevu where ");



            //NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            //conn.Open();
            //cmbDoktor.Items.Clear();
            //NpgsqlCommand cmd = new NpgsqlCommand("SELECT k.\"ad\", k.\"soyad\"\r\nFROM \"kisi\" k\r\nJOIN \"doktor\" d ON k.\"KisiID\" = d.\"DoktorID\"\r\nWHERE d.\"brans\" = @brans;\r\n ", conn);
            //cmd.Parameters.AddWithValue("@brans", cmbDoktor.Text);
            //NpgsqlDataReader dr = cmd.ExecuteReader();
            //while (dr.Read())
            //{
            //    cmbDoktor.Items.Add(dr[0] + " " + dr[1]);
            //}
            //conn.Close();

           

        }

        private void lnkBilgiGüncelle_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            HastabilgiDuzenle fr = new HastabilgiDuzenle();
            fr.telNo= lblTel.Text;
            fr.Show();
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void btnRandevuAl_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Randevunuz başarıyla oluşturuldu.", "Başarı", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
