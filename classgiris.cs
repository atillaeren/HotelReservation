using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Net;
using System.Net.Mail;

namespace OtelRezervasyon
{
    // Giriş ve şifre işlemlerini gerçekleştirmek için class tanımladık.
    class classgiris
    {
        // SQL bağlantısını ayarlıyoruz.
        SqlConnection baglanti = new SqlConnection("Data Source=KEKW\\SQLEXPRESS;Initial Catalog=OtelRezervasyon;Integrated Security=True");
        SqlCommand komut;
        SqlDataReader read;
        AnaSayfa uyeGiris = new AnaSayfa();

        // Sisteme giriş için kullanıcı adı ve şifre sorgusunu yapıyoruz.
        public SqlDataReader kullanıcı(TextBox kullanıcıadı, TextBox sifre)
        {
            baglanti.Open();
            komut = new SqlCommand();
            komut.Connection = baglanti;

            //Girilen kullanıcı adını veritabanından sorguluyoruz. Kullanıcı adı ve şifre doğru ise giriş yapılıyor.
            komut.CommandText = "select *from Uyeler where KullanıcıAdı='" + kullanıcıadı.Text + "'";
            read = komut.ExecuteReader();

            if (read.Read() == true) 
            {
                if (sifre.Text == read["Sifre"].ToString())
                {
                    uyeGiris.Show();
                }
                else
                {
                    MessageBox.Show("Please check your password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please check the informations you entered.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            baglanti.Close();
            return read;
        }

        // Kullanıcıya şifresini hatırlatmak için yollanacak e postanın bilgilerini değişkenlerimize aktarıyoruz.
        string systememail = ""; // E posta yollayacak adres burada olacak.
        string systememailpass = ""; // E posta yollayacak adresin şifresi burada olacak.

        // Kullanıcı şifresi için değişkenimizi oluşturuyoruz..
        string usersifre,userid;

        //Şifre hatırlatma için methodumuzu tanımlıyoruz.
        public void sifre(TextBox TC, TextBox Eposta)
        {
                baglanti.Open();
                komut = new SqlCommand("select *from Uyeler where TC='" + TC.Text + "'", baglanti);
                read = komut.ExecuteReader();
                if (read.Read() == true)
                {
                //Kullanıcı şifresini değişkenimize atıyoruz.
                    usersifre = read["Sifre"].ToString();
                    userid = read["KullanıcıAdı"].ToString();
                // Eğer şifre ve eposta uyuşuyor ise kullanıcı mailine sistem tarafından şifresi yollanıyor.
                if (Eposta.Text == read["Eposta"].ToString())
                    {
                    
                    MailMessage message = new MailMessage();

                    // E postanın hangi e posta adresi tarafından gönderileceğini belirliyoruz.
                    message.From = new MailAddress(systememail);

                    // E postanın kime yollanacağını belirliyoruz.
                    message.To.Add(Eposta.Text);

                    // E postanın konusunu belirliyoruz.
                    message.Subject = "--Username and Password Reminder--";

                    // E postanın içeriğini belirliyoruz.
                    message.Body = "Your username is : " + userid + "\nYour password is : " + usersifre;
                    

                    // E postanın gönderilme metodunu belirliyoruz.
                    SmtpClient smtp = new SmtpClient();

                    // E postayı yollayacak hesabın giriş bilgilerini ekliyoruz.
                    smtp.Credentials = new NetworkCredential(systememail, systememailpass);

                    // E postayı yollayacak hesabın host ve port değerlerini ve ssl bilgisini belirliyoruz.
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    try
                    {
                        // Bilgilerin uygun olması durumunda e postayı yolluyoruz.
                        smtp.SendAsync(message, (object)message);
                        MessageBox.Show("Your username and password has been sended, please check your email.", "Password", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        // Herhangi bir hata olması durumunda kullanıcıyı uyarıyoruz.
                        MessageBox.Show(ex.Message);
                    }  
                }
                    else
                    {
                        // E posta hatalı ise verilen uyarı.
                        MessageBox.Show("The email you entered is incorrect, please check.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    // Kullanıcı adı hatalı ise verilen uyarı.
                    MessageBox.Show("The TC ID No you entered is incorrect, please check.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                baglanti.Close();
        }
    }
}
