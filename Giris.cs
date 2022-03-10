using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Data.SqlClient;



namespace OtelRezervasyon
{
    public partial class Giris : Form
    {
        
        public Giris()
        {
            InitializeComponent();
        }
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

        private void Giris_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        // Kullancı adı textboxına odaklama kodu.
        private void Giris_Load(object sender, EventArgs e)
        {
            txtKullanıcıAdı.Focus();
        }

        // Kaıyt formuna geçiş.
        private void btnKayıt_Click(object sender, EventArgs e)
        {
            new Kayıt().Show();
            this.Hide();
        }

        // Şifre yenileme formuna geçiş.
        private void btnSifreYenile_Click(object sender, EventArgs e)
        {
            new SifreYenile().Show();
            this.Hide();
        }

        // Uygulamayı kapatmak için butonumuzun click eventini ayarlıyoruz.
        private void btnCikis_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // SQL bağlantımızı yapıyoruz.
        SqlConnection baglanti = new SqlConnection("Data Source=KEKW\\SQLEXPRESS;Initial Catalog=OtelRezervasyon;Integrated Security=True");
        classgiris sınıf = new classgiris();

        // Kullanıcı adı ve TC no yu diğer formlarda kullanabilmek için değişkenlerimizi tanımlıyoruz.
        public static string userid,usertc;

        private void btnGiris_Click(object sender, EventArgs e)
        {
            // Veritabanından kullanıcı adı ve şifre sorgusu yapıyoruz. Eğer doğru ise sisteme giriş yapılıyor.
            baglanti.Open();
            SqlCommand komut = new SqlCommand("select *from Uyeler where KullanıcıAdı like '" + txtKullanıcıAdı.Text + "'", baglanti);
            SqlDataReader read = komut.ExecuteReader();

            // Anasayfada kullanıcı adını gösterebilmek için kullanıcı adını değişkenimize atıyoruz.
            userid = txtKullanıcıAdı.Text;

            if (read.Read() == true)
            {
                if (txtSifre.Text == read["Sifre"].ToString())
                {
                    this.Hide();
                }
            }
            baglanti.Close();
            sınıf.kullanıcı(txtKullanıcıAdı, txtSifre);

            // Rezervasyon formunda kişisel bilgileri otomatik doldurabilmek için TC no yu değişkenimize atıyoruz.
            baglanti.Open();
            SqlCommand komut1 = new SqlCommand("select TC from Uyeler where KullanıcıAdı like '" + txtKullanıcıAdı.Text + "'", baglanti);
            SqlDataReader read1 = komut1.ExecuteReader();
            if (read1.Read() == true)
            {
                usertc = read1["TC"].ToString();
            }
            baglanti.Close();
        }

        // Girilen şifreyi gizlemek için kod.
        private void txtSifre_Enter(object sender, EventArgs e)
        {
            txtSifre.UseSystemPasswordChar = true;
        }

        // Enter tuşu ile giriş yapabilmek için kod.
        private void txtSifre_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnGiris_Click(this, new EventArgs());
            }
        }

        // Şifre kısmında gizliliği kaldırabilmek için checkbox ımızı ayarlıyoruz.
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.CheckState == CheckState.Checked)
                txtSifre.UseSystemPasswordChar = false;
            
            else if (checkBox1.CheckState == CheckState.Unchecked)
                txtSifre.UseSystemPasswordChar = true;
        }
    }
}
