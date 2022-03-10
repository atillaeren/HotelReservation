using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Runtime.InteropServices;

namespace OtelRezervasyon
{
    public partial class AnaSayfa : Form
    {
        public AnaSayfa()
        {
            InitializeComponent();
        }
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

        private void AnaSayfa_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        // SQL bağlantımızı yapıyoruz.
        SqlConnection baglanti = new SqlConnection("Data Source=KEKW\\SQLEXPRESS;Initial Catalog=OtelRezervasyon;Integrated Security=True");

        // Anasayfadan çıkış yaparken kullanıcıya soruyoruz.
        private void btnKayıtKapat_Click(object sender, EventArgs e)
        {
            DialogResult dialog;
            dialog = MessageBox.Show("Do you want to log out?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialog == DialogResult.Yes)
            {
                new Giris().Show();
                this.Close();
            }
        }

        // Rezervasyon formuna geçiş için butonumuzun click eventini ayarlıyoruz.
        private void btnrezervasyon_Click(object sender, EventArgs e)
        {
            new Rezervasyon().Show();
            this.Hide();
        }

        private void AnaSayfa_Load(object sender, EventArgs e)
        {
            // Anasayfada kullanıcı adı labelına kaydettiğimiz kullanıcı adı bilgisini atıyoruz.
            baglanti.Open();
            SqlCommand komut = new SqlCommand("select *from Uyeler where KullanıcıAdı like '%" + Giris.userid + "%'", baglanti);
            SqlDataReader read = komut.ExecuteReader();
            label1.Text = "Welcome\n" + Giris.userid;
            baglanti.Close();

            // Eğer eski tarihli rezervasyonlar var ise veritabanından siliyoruz.
            baglanti.Open();
            SqlCommand komut1 = new SqlCommand("delete from Rezervasyon where CikisTarih<GETDATE()", baglanti);
            komut1.ExecuteNonQuery();
            baglanti.Close();
        }

        // İlk galeri formuna geçiş için butonumuzun click eventini ayarlıyoruz.
        private void btnGaleri_Click(object sender, EventArgs e)
        {
            new OdaKV().Show();
            this.Hide();
        }

        // İletişim formuna geçiş için butonumuzun click eventini ayarlıyoruz.
        private void btnUlas_Click(object sender, EventArgs e)
        {
            new Contact().Show();
            this.Hide();
        }

        // Rezervasyonlarım formuna geçiş için butonumuzun click eventini ayarlıyoruz.
        private void btnMyRes_Click(object sender, EventArgs e)
        {
            new Rezervasyonlarım().Show();
            this.Hide();
        }

        // Profil formuna geçiş için butonumuzun click eventini ayarlıyoruz.
        private void button1_Click(object sender, EventArgs e)
        {
            new Profil().Show();
            this.Close();
        }
    }
}
