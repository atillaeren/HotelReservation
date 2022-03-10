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
    public partial class Profil : Form
    {
        public Profil()
        {
            InitializeComponent();
        }

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

        private void Profil_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        // SQL bağlantımızı yapıyoruz.
        SqlConnection baglanti = new SqlConnection("Data Source=KEKW\\SQLEXPRESS;Initial Catalog=OtelRezervasyon;Integrated Security=True");


        private void Profil_Load(object sender, EventArgs e)
        {
            // Kullanıcı bilgilerini "userid" değişkenimiz ile veritabanından çağırıyoruz.
            btnUpdate.Focus();
            baglanti.Open();
            SqlCommand komut = new SqlCommand("select *from Uyeler where KullanıcıAdı like '%" + Giris.userid + "%'", baglanti);
            SqlDataReader read = komut.ExecuteReader();
            while (read.Read())
            {
                txtTc.Text = read[0].ToString();
                txtAd.Text = read[1].ToString();
                txtSoyAd.Text = read[2].ToString();
                comboBox1.Text = read[3].ToString();
                txtTelNo.Text = read[4].ToString();
                txtEposta.Text = read[5].ToString();
                txtAdres.Text = read[6].ToString();
                txtKayıtKullanıcıAdı.Text = read[7].ToString();
                txtKayıtSifre.Text = read[8].ToString();
            }
            baglanti.Close();

            // Checkbox işlevini kullanıcıya bildirmek için checkbox üzerine gelindiğinde mesaj veriyoruz.
            ToolTip Aciklama = new ToolTip();
            Aciklama.SetToolTip(checkBox1, "Check to update information.");

            // Form açıldığında değişiklik yapılamaması için ilgili yerleri kapatıyoruz.
            btnUpdate.Enabled = false;
            txtTc.Enabled = false;
            txtAd.Enabled = false;
            txtSoyAd.Enabled = false;
            txtTelNo.Enabled = false;
            txtEposta.Enabled = false;
            txtAdres.Enabled = false;
            comboBox1.Enabled = false;
            txtKayıtKullanıcıAdı.Enabled = false;
            txtKayıtSifre.Enabled = false;
        }

        // Kullanıcı, bilgilerini değiştirebilmesi için checkbox eventimizi ayarlıyoruz.
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                btnUpdate.Enabled = true;
                txtTc.Enabled = false;
                txtAd.Enabled = true;
                txtSoyAd.Enabled = true;
                txtTelNo.Enabled = true;
                txtEposta.Enabled = true;
                txtAdres.Enabled = true;
                comboBox1.Enabled = true;
                txtKayıtKullanıcıAdı.Enabled = false;
                txtKayıtSifre.Enabled = true;
            }
            else
            {
                btnUpdate.Enabled = false;
                txtTc.Enabled = false;
                txtAd.Enabled = false;
                txtSoyAd.Enabled = false;
                txtTelNo.Enabled = false;
                txtEposta.Enabled = false;
                txtAdres.Enabled = false;
                comboBox1.Enabled = false;
                txtKayıtKullanıcıAdı.Enabled = false;
                txtKayıtSifre.Enabled = false;
            }
        }

        // Formu kapatmak için kod.
        private void btnProfilKapat_Click(object sender, EventArgs e)
        {
            new AnaSayfa().Show();
            this.Close();
        }

        // Bilgi güncelleme butonumuzu ayarlıyoruz.
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            // Textlerin boş olup olmadığını sorguluyoruz. Eğer boş var ise hata verdiriyoruz.
            if (txtAd.Text == "" || txtSoyAd.Text == "" || comboBox1.Text == "" || txtEposta.Text == "" || txtTelNo.Text == "" || txtAdres.Text == "" || txtKayıtSifre.Text == "")
            {
                MessageBox.Show(" Please enter a value into all boxes.");
            }
            else
            {
                // Eğer tüm alanlar dolu ise üye tablosunu güncelleme işlemini yapıyoruz.
                baglanti.Open();
                SqlCommand komut = new SqlCommand("update Uyeler set TC=@TC,Ad=@Ad,Soyad=@Soyad,Cinsiyet=@Cinsiyet,Telefon=@Telefon,Eposta=@Eposta,Adres=@Adres,KullanıcıAdı=@KullanıcıAdı,Sifre=@Sifre where TC=@TC", baglanti);
                komut.Parameters.AddWithValue("@TC", txtTc.Text);
                komut.Parameters.AddWithValue("@Ad", txtAd.Text);
                komut.Parameters.AddWithValue("@Soyad", txtSoyAd.Text);
                komut.Parameters.AddWithValue("@Cinsiyet", comboBox1.Text);
                komut.Parameters.AddWithValue("@Telefon", txtTelNo.Text);
                komut.Parameters.AddWithValue("@Eposta", txtEposta.Text);
                komut.Parameters.AddWithValue("@Adres", txtAdres.Text);
                komut.Parameters.AddWithValue("@KullanıcıAdı", txtKayıtKullanıcıAdı.Text);
                komut.Parameters.AddWithValue("@Sifre", txtKayıtSifre.Text);
                komut.ExecuteNonQuery();
                baglanti.Close();

                // Rezervasyon tablosunda da değişikliklerimizi yapıyoruz.
                baglanti.Open();
                SqlCommand komut1 = new SqlCommand("update Rezervasyon set TC=@TC,Ad=@Ad,Soyad=@Soyad,Telefon=@Telefon,Adres=@Adres where TC=@TC", baglanti);
                komut1.Parameters.AddWithValue("@TC", txtTc.Text);
                komut1.Parameters.AddWithValue("@Ad", txtAd.Text);
                komut1.Parameters.AddWithValue("@Soyad", txtSoyAd.Text);
                komut1.Parameters.AddWithValue("@Telefon", txtTelNo.Text);
                komut1.Parameters.AddWithValue("@Adres", txtAdres.Text);
                komut1.ExecuteNonQuery();
                baglanti.Close();
                checkBox1.Checked = false;
                MessageBox.Show("Your informations has been updated.");
            }
        }
    }
}
