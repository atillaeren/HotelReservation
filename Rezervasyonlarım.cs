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
    public partial class Rezervasyonlarım : Form
    {
        public Rezervasyonlarım()
        {
            InitializeComponent();
        }
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

        private void Rezervasyonlarım_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        // SQL bağlantımızı yapıyoruz.
        SqlConnection baglanti = new SqlConnection("Data Source=KEKW\\SQLEXPRESS;Initial Catalog=OtelRezervasyon;Integrated Security=True");

        // Veritabanından aldığımız rezervasyon bilgilerini listeleyebilmek için dataset tanımlıyoruz.
        DataSet dtst = new DataSet();

        // Anasayfa formuna geçiş için butonumuzun click eventini ayarlıyoruz
        private void btnMyResKapat_Click(object sender, EventArgs e)
        {
            new AnaSayfa().Show();
            this.Close();
        }

        // Kullanıcının rezervasyon kaydını veritabanından aldığımız metodumuzu oluşturuyoruz. 
        private void Rezervasyonliste()
        {
            baglanti.Open();
            // Veritabanından kullanıcının TC numarasına göre bilgileri çekiyoruz.
            SqlDataAdapter adptr = new SqlDataAdapter("select RezervasyonID,Ad,Soyad,OdaSinif,OdaNo,GirisTarih,CikisTarih,Fiyat from Rezervasyon where TC like '%" + Giris.usertc + "%'",baglanti);

            // Bilgileri datagridview a akarabilmek için datasetimize alıyoruz.
            adptr.Fill(dtst,"Rezervasyon");

            // Bilgileri listelemek için datagridview a aktarıyoruz.
            dataGridView1.DataSource = dtst.Tables["Rezervasyon"];
            baglanti.Close();
        }

        private void Rezervasyonlarım_Load(object sender, EventArgs e)
        {
            // Form açıldığında rezervasyonların listelenmesi için formun load eventinde rezervasyon listeleme metodumuzu çağırıyoruz.
            Rezervasyonliste();
        }

        // Rezervasyon iptali için ilgili butonumuzun click eventini ayarlıyoruz.
        private void button1_Click(object sender, EventArgs e)
        {
            // Kullanıcı iptal için tıkladığında hatalı tıklamaları önlemek için uyarı mesajı veriyoruz.
            DialogResult dialog;
            dialog = MessageBox.Show("Do you really want to cancel your reservation?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialog == DialogResult.Yes)
            {
                // İptal etmek isterse veritabanına bağlanıp rezervasyonu siliyoruz.
                baglanti.Open();
                SqlCommand komut = new SqlCommand("delete from Rezervasyon where RezervasyonID=@RezervasyonID", baglanti);
                komut.Parameters.AddWithValue("@RezervasyonID", dataGridView1.CurrentRow.Cells["RezervasyonID"].Value.ToString());
                komut.ExecuteNonQuery();
                baglanti.Close();

                // İptal edildiğine dair kullanıcıyı bilgilendiriyoruz.
                MessageBox.Show("The reservation has been cancelled.");

                // Yeniden bilgileri almadan önce dataseti temizliyoruz.
                dtst.Tables["Rezervasyon"].Clear();

                // İptal işleminden sonra güncel bilgileri tekrar çağırıyoruz.
                Rezervasyonliste();
            }
        }
    }
}
