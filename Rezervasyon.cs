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
    public partial class Rezervasyon : Form
    {
        public Rezervasyon()
        {
            InitializeComponent();
        }
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

        private void Rezervasyon_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        // SQL bağlantımızı yapıyoruz.
        SqlConnection baglanti = new SqlConnection("Data Source=KEKW\\SQLEXPRESS;Initial Catalog=OtelRezervasyon;Integrated Security=True");

        // Anasayfa formuna geçiş için butonumuzun click eventini ayarlıyoruz.
        private void btnKayıtKapat_Click(object sender, EventArgs e)
        {
            new AnaSayfa().Show();
            this.Close();
        }

        private void Rezervasyon_Load(object sender, EventArgs e)
        {
            // Datetime picker larımızı ayarlıyoruz.
            dtmBitis.Value = dtmBaslangic.Value.AddDays(1);
            dtmBaslangic.Format = DateTimePickerFormat.Custom;
            dtmBaslangic.CustomFormat = "yyyy MM dd";
            dtmBitis.Format = DateTimePickerFormat.Custom;
            dtmBitis.CustomFormat = "yyyy MM dd";

            // Rezervasyon yapılacak odanın müsaitliğini kontrol için 3 adet listbox tanımladık. Bu listboxların görünürlüğünü kapatıyoruz.
            listBox1.Visible = false;
            listBox2.Visible = false;
            listBox3.Visible = false;
            lblFiyat.Visible = false;
            
            // Rezervasyon yapacak kullanıcının kişisel bilgilerini otomatik doldurmak için veritabanından çekiyoruz.
            baglanti.Open();
            SqlCommand komut = new SqlCommand("select *from Uyeler where KullanıcıAdı like '%" + Giris.userid + "%'",baglanti);
            SqlDataReader read = komut.ExecuteReader();
            while (read.Read())
            {
                txtTc.Text = read[0].ToString();
                txtAd.Text = read[1].ToString();
                txtSoyAd.Text = read[2].ToString();
                txtTelNo.Text = read[4].ToString();
                txtEposta.Text = read[5].ToString();
                txtAdres.Text = read[6].ToString();
            }
            baglanti.Close();

            // Oda sınıflarını combobox a koyabilmek için veritabanından çekiyoruz.
            baglanti.Open();
            SqlCommand komut1 = new SqlCommand("select *from Siniflar", baglanti);
            komut1.CommandType = CommandType.Text;
            SqlDataReader read1 = komut1.ExecuteReader();
            while (read1.Read())
            {
                cmbClass.Items.Add(read1["Class"]);
            }
            baglanti.Close();
        }

        // Rezervasyon başlangıç tarihi değişikliğinde, hata oluşmaması için bitiş tarihimizi de değiştiriyoruz.
        private void dtmBaslangic_ValueChanged(object sender, EventArgs e)
        {
            dtmBitis.Value = dtmBaslangic.Value.AddDays(1);
        }

        // Rezervasyon tarihi seçiminde hata olup olmadığını sorguluyoruz.
        private void dtmBitis_ValueChanged(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            listBox3.Items.Clear();
            txtFiyat.Text = "";
            lblDurum.Text = "";
            lblDurum.BackColor = Color.Transparent;
            cmbOdaNo.Text = "";
            cmbClass.Text = "";
            DateTime girisTarih = Convert.ToDateTime(dtmBaslangic.Value);
            DateTime bitisTarih = Convert.ToDateTime(dtmBitis.Value.AddDays(1));

            // Rezervasyon başlangıç ve bitiş tarihinin aynı olmaması ve geçmiş tarihli olmaması için sorguluyoruz.
            if (dtmBaslangic.Value < DateTime.Today || dtmBitis.Value < DateTime.Today || dtmBitis.Value <= dtmBaslangic.Value)
            {
                MessageBox.Show("Please correct booking date.");
                dtmBaslangic.Value = DateTime.Today;
                dtmBitis.Value = dtmBaslangic.Value.AddDays(1);
            }

            // Rezervasyon tarihleri arasındaki her günü müsaitlik sorgusu için listboxımıza atıyoruz.
            for (; girisTarih <= bitisTarih; girisTarih = girisTarih.AddDays(1))
            {
                string tarihal = girisTarih.Date.ToString("yyyy MM dd");
                listBox1.Items.Add(tarihal + "\n");
            }
        }

        // Oda sınıfı seçildiğinde oda numaralarını combobox a atabilmek için veritabanından çekiyoruz.
        private void cmbClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Oda sınıfının tekrar değişmesi durumunda önceki seçeneğin bilgilerini siliyoruz.
            txtFiyat.Text = "";
            lblDurum.Text = "";
            lblDurum.BackColor = Color.Transparent;
            for (int i = 0; i < cmbClass.Items.Count; i++)
            {
                if (cmbClass.SelectedIndex == i)
                {
                    // Seçilen sınıfa ait odaları ilgili combobox ta listeliyoruz.
                    cmbOdaNo.Items.Clear();
                    cmbOdaNo.Text = "";
                    baglanti.Open();
                    SqlCommand komut = new SqlCommand("select *from Odalar where Sinif like '" + cmbClass.Text + "'", baglanti);
                    komut.CommandType = CommandType.Text;
                    SqlDataReader read = komut.ExecuteReader();
                    while (read.Read())
                    {
                        cmbOdaNo.Items.Add(read["OdaNo"]);
                    }
                    baglanti.Close();
                }
            }
        }

        // Oda müsaitlik sorgusu yaparken veritabanındaki rezervasyon tarihlerini ikinci listboxa atmak için değişkenlerimizi tanımlıyoruz.
        DateTime grsTarih;
        DateTime cksTarih;
        // Fiyat için değişkenimizi tanımlıyoruz.
        int toplamfiyat;
        private void cmbOdaNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            listBox3.Items.Clear();
            baglanti.Open();
            // Veritabanındaki rezervasyon tarihlerini çekiyoruz.
            SqlCommand komut = new SqlCommand("select GirisTarih,CikisTarih from Rezervasyon where OdaNo like '" + cmbOdaNo.Text + "'", baglanti);
            komut.CommandType = CommandType.Text;
            SqlDataReader read = komut.ExecuteReader();
            while (read.Read())
            {
                // Bu tarihleri ikinci listboxa aktarıyoruz.
                grsTarih = Convert.ToDateTime(read["GirisTarih"]);
                cksTarih = Convert.ToDateTime(read["CikisTarih"]);
                for (; grsTarih <= cksTarih; grsTarih = grsTarih.AddDays(1))
                {
                    string tarihAl = grsTarih.Date.ToString("yyyy MM dd");
                    listBox2.Items.Add(tarihAl + "\n");
                }
            }
            baglanti.Close();

            // Üçüncü listboxta ilk iki listbox arasında çakışan tarih olup olmadığını kontrol için bu listboxtaki değerleri listelere atıyoruz.
            List<string> M = new List<string>();
            List<string> N = new List<string>();
            for (int i = 0; i <= listBox1.Items.Count - 1; i++)
            {
                M.Add(listBox1.Items[i].ToString());
            }
            for (int i = 0; i <= listBox2.Items.Count - 1; i++)
            {
                N.Add(listBox2.Items[i].ToString());
            }

            // Üçüncü listboxa çakışan günleri aktarıyoruz.
            var kesisim = M.Intersect(N);
            listBox3.Items.Clear();
            foreach (var satir in kesisim)
            {
                listBox3.Items.Add(satir);
            }

            // Eğer çakışan gün yoksa..
            if (listBox3.Items.Count == 0)
            {
                // Müsaitlik labelımızın değerini ve rengini ayarlıyoruz.
                lblDurum.Text = "AVAILABLE";
                lblDurum.BackColor = Color.SpringGreen;

                // Veritabanımızdaki odalar tablosundan oda numarısının fiyatını label a atıyoruz.
                baglanti.Open();
                SqlCommand komut1 = new SqlCommand("select *from Odalar where OdaNo like '" + cmbOdaNo.Text + "'", baglanti);
                komut1.CommandType = CommandType.Text;
                SqlDataReader read1 = komut1.ExecuteReader();
                while (read1.Read())
                {
                    lblFiyat.Text = (string)read1["Fiyat"];
                }
                baglanti.Close();

                // Fiyat hesaplamamızı yapıp fiyat textimize atıyoruz.
                toplamfiyat = (dtmBitis.Value.DayOfYear - dtmBaslangic.Value.DayOfYear) * int.Parse(lblFiyat.Text);
                txtFiyat.Text = toplamfiyat.ToString();
            }

            // Çakışan gün varsa müsaitlik labelımızın değerini ve rengini ayarlıyoruz.
            else
            {
                lblDurum.Text = "NOT AVAILABLE";
                lblDurum.BackColor = Color.OrangeRed;
            }
        }

        // Rezervasyon yapabilmek için butonumuzun click eventini ayarlıyoruz.
        private void btnKayıt_Click(object sender, EventArgs e)
        {
            // Eğer oda müsait ise rezervasyon işlemini gerçekleştiriyoruz, bilgilendirme mesajı veriyoruz ve ilgili değerleri temizliyoruz.
            if (lblDurum.BackColor == Color.SpringGreen)
            {
                // Veritabanı bağlantımızı sağlayıp rezervasyon bilgilerini veritabanına aktarıyoruz.
                baglanti.Open();
                SqlCommand komut = new SqlCommand("insert into Rezervasyon(TC,Ad,Soyad,Telefon,Adres,OdaSinif,OdaNo,GirisTarih,CikisTarih,Fiyat,Eposta) values (@TC,@Ad,@Soyad,@Telefon,@Adres,@OdaSinif,@OdaNo,@GirisTarih,@CikisTarih,@Fiyat,@Eposta)", baglanti);
                komut.Parameters.AddWithValue("@TC", txtTc.Text);
                komut.Parameters.AddWithValue("@Ad", txtAd.Text);
                komut.Parameters.AddWithValue("@Soyad", txtSoyAd.Text);
                komut.Parameters.AddWithValue("@Telefon", txtTelNo.Text);
                komut.Parameters.AddWithValue("@Adres", txtAdres.Text);
                komut.Parameters.AddWithValue("@OdaSinif", cmbClass.Text);
                komut.Parameters.AddWithValue("@OdaNo", cmbOdaNo.Text);
                komut.Parameters.AddWithValue("@GirisTarih", Convert.ToDateTime(dtmBaslangic.Text));
                komut.Parameters.AddWithValue("@CikisTarih", Convert.ToDateTime(dtmBitis.Text));
                komut.Parameters.AddWithValue("@Fiyat", txtFiyat.Text);
                komut.Parameters.AddWithValue("@Eposta", txtEposta.Text);
                komut.ExecuteNonQuery();
                baglanti.Close();

                // Kaydın başarılı olduğuna dair kullanıcıya bilgilendirme mesajı veriyoruz.
                MessageBox.Show("Your reservation has been saved successfully.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                cmbClass.Text = "";
                cmbOdaNo.Text = "";
                dtmBaslangic.Value = DateTime.Today;
                dtmBitis.Value = dtmBaslangic.Value.AddDays(1);

                // Rezervasyon tamamlandıktan sonra ilgili bölümleri temizliyoruz.
                txtFiyat.Text = "";
                listBox1.Items.Clear();
                listBox2.Items.Clear();
                listBox3.Items.Clear();
                lblDurum.Text = "";
                lblDurum.BackColor = Color.Transparent;
            }
            
            // Comboboxların boş olması durumunda kullanıcıya uyarı mesajı veriyoruz.
            else if (cmbClass.Text == "" || cmbOdaNo.Text == "" )
            {
                MessageBox.Show("Please fill all the parts.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Odanın müsait olmaması durumunda, kullanıcı rezervasyon yapmaya çalışırsa kullanıcıya uyarı mesajı veriyoruz. 
            else
            {
                MessageBox.Show("The room you selected is not available \nPlease choose another room or date.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }
    }
}
