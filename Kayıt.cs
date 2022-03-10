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
    public partial class Kayıt : Form
    {
        public Kayıt()
        {
            InitializeComponent();
        }

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);
        private void Kayıt_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        // SQL bağlantımızı yapıyoruz.
        SqlConnection baglanti = new SqlConnection("Data Source=KEKW\\SQLEXPRESS;Initial Catalog=OtelRezervasyon;Integrated Security=True");

        // Giriş formuna geçiş için butonumuzun click eventini ayarlıyoruz.
        private void btnKayıtKapat_Click(object sender, EventArgs e)
        {
            new Giris().Show();
            this.Close();
        }

        // TC kimlik numarasının sistemde kayıtlı olup olmadığını kontrol etmek için metod tanımlıyoruz.
        public static bool durum1,durum2;
        void uniquetc()
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("select *from Uyeler where TC=@TC", baglanti);
            komut.Parameters.AddWithValue("@TC", txtTc.Text);
            SqlDataReader read = komut.ExecuteReader();
            
            if (read.Read())
            {
                durum1 = false;
            }
            else
            {
                durum1 = true;
            }
            baglanti.Close();
        }

        // Kullanıcı adının sistemde kayıtlı olup olmadığını kontrol etmek için metod tanımlıyoruz.
        void uniqueusername()
        {
            baglanti.Open();
            SqlCommand komut = new SqlCommand("select *from Uyeler where KullanıcıAdı=@KullanıcıAdı", baglanti);
            komut.Parameters.AddWithValue("@KullanıcıAdı", txtKayıtKullanıcıAdı.Text);
            SqlDataReader read = komut.ExecuteReader();

            if (read.Read())
            {
                durum2 = false;
            }
            else
            {
                durum2 = true;
            }
            baglanti.Close();
        }

        // Kayıt işlemi için click event i tanımlıyoruz.
        private void btnKayıt_Click(object sender, EventArgs e)
        {
            // Boş alan olup olmadığını sorguluyoruz.
            if (txtTc.Text == "" || txtAd.Text == "" || txtSoyAd.Text == "" || comboBox1.Text == "" || txtEposta.Text == "" || txtTelNo.Text == "" || txtAdres.Text == "" || txtKayıtKullanıcıAdı.Text == "" || txtKayıtSifre.Text == "")
            {
                MessageBox.Show(" Please enter a value into all boxes.");
            }
            // Eğer boş alan yoksa kullanıcı adı ve TC no metodlarını çağırıp sorguluyoruz.
            else
            {
                uniquetc();
                uniqueusername();
                if (durum1 == true && durum2 == true)
                {
                    baglanti.Open();
                    SqlCommand komut = new SqlCommand("insert into Uyeler(TC,Ad,Soyad,Cinsiyet,Telefon,Eposta,Adres,KullanıcıAdı,Sifre) values (@TC,@Ad,@Soyad,@Cinsiyet,@Telefon,@Eposta,@Adres,@KullanıcıAdı,@Sifre)", baglanti);
                    string tckimlik;
                    try
                    {
                        // TC kimlik numarasının gerçek bir TC kimlik no olup olmadığını sorguluyoruz
                        tckimlik = txtTc.Text;
                        int index = 0;
                        int toplam = 0;
                        foreach (char n in tckimlik)
                        {
                            if (index < 10)
                            {
                                toplam += Convert.ToInt32(char.ToString(n));
                            }
                            index++;
                        }
                        // Eğer gerçek TC no ise kayıt işlemini yapıyoruz.
                        if (toplam % 10 == Convert.ToInt32(tckimlik[10].ToString()))
                        {
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

                            MessageBox.Show(" Your registration is complete. Plese sign in.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            new Giris().Show();
                        }
                        else
                        {
                            // Gerçek bir TC no girilmedi ise uyarı yapıyoruz.
                            MessageBox.Show(" The TCIDNo must be unique. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txtTc.Text = "";
                            txtTc.Focus();
                        }
                    }
                    catch
                    {
                        // TC kimlik no 11 haneli girilmezse uyarı yapıyoruz.
                        MessageBox.Show(" The TCIDNo must be 11 digits, please correct it.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txtTc.Text = "";
                        txtTc.Focus();
                    }
                    baglanti.Close();
                }
                else
                {
                    // Eğer kullanıcı adı alındıysa uyarı yapıyoruz.
                    if (durum1 == true && durum2 == false)
                    {
                        MessageBox.Show(" This Username already taken, please change Username", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtKayıtKullanıcıAdı.Text = "";
                        txtKayıtKullanıcıAdı.Focus();
                    }
                    // Eğer TC no kayıtlı ise uyarı yapıyoruz.
                    else if (durum1 == false && durum2 == true)
                    {
                        MessageBox.Show(" This TCIDNO already have an account, please sign in.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtTc.Text = "";
                        txtTc.Focus();
                    }

                    else if (durum1 == false && durum2 == false)
                    {
                        MessageBox.Show(" This user already have an account. Please sign in.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtTc.Text = "";
                        this.Close();
                        new Giris().Show();
                    }
                }
            }
        }
    }
}
