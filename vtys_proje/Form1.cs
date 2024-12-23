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
    public partial class Main : Form
    {
        string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=esmacanimbenimCR7ilker ;Database=hastane_randevu;";
        public Main()
        {
            InitializeComponent();
        }
                                    
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            SekreterGiris sekreterGiris = new SekreterGiris();
            sekreterGiris.Show();

            // Mevcut formu kapat
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            // Form2'yi başlat
            Hasta_giris hasta_Giris = new Hasta_giris();
            hasta_Giris.Show();

            // Mevcut formu kapat
            this.Hide();
       

    }

        private void button2_Click(object sender, EventArgs e)
        {
            DoktorGiris doktorGiris = new DoktorGiris();
            doktorGiris.Show();

            // Mevcut formu kapat
            this.Hide();
        }
    }
}
