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
using System.Diagnostics;
using System.Data.SqlClient;


namespace OtelRezervasyon
{
    public partial class Contact : Form
    {
        public Contact()
        {
            InitializeComponent();
        }
        
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

        private void Contact_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        // SQL bağlantımızı yapıyoruz.
        SqlConnection baglanti = new SqlConnection("Data Source=KEKW\\SQLEXPRESS;Initial Catalog=OtelRezervasyon;Integrated Security=True");

        // Anasayfa formuna geçiş için butonumuzun click eventini ayarlıyoruz.
        private void btnContactKapat_Click(object sender, EventArgs e)
        {
            new AnaSayfa().Show();
            this.Close();
        }

        // Kullanıcının panelimize yüklediğimiz harita görseli yerine gerçek haritaya bakabilmesi için panelin çift tık eventini ayarlıyoruz.
        private void panel1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Kullanıcının çift tıklaması durumunda kullanıcının varsayılan tarayıcısında haritayı açıyoruz.
            string strUrl = "https://www.google.com/maps/search/?api=1&query=marmara+universitesi+teknoloji+fakultesi";
            Process proc = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo(strUrl);
            proc.StartInfo = startInfo;
            proc.Start();
        }

        // Kullanıcının öneri ve şikayeti için oluşturulan formdaki bilgileri veri tabanına aktarılabilmesi için ilgili butonun click eventini ayarlıyoruz.
        private void button1_Click(object sender, EventArgs e)
        {
            // Kullanıcı mesajını kullanıcı adı ile birlikte veri tabanındaki ilgili tabloya aktarıyoruz.
            baglanti.Open();
            SqlCommand komut = new SqlCommand("insert into ContactUs(KullanıcıAdı,Konu,Mesaj) values (@KullanıcıAdı,@Konu,@Mesaj)", baglanti);
            komut.Parameters.AddWithValue("@KullanıcıAdı", Giris.userid.ToString());
            komut.Parameters.AddWithValue("@Konu", txtKonu.Text);
            komut.Parameters.AddWithValue("@Mesaj", txtMesaj.Text);
            komut.ExecuteNonQuery();
            baglanti.Close();
            txtKonu.Text = "";
            txtMesaj.Text = "";
            // İşlemin başarılı olması durumunda kullanıcıya teşekkür mesajı veriyoruz.
            MessageBox.Show(" Thank you for supporting us. ", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            txtKonu.Text = "";
            txtMesaj.Text = "";
        }
    }
}
