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

namespace OtelRezervasyon
{
    public partial class Oda12 : Form
    {
        public Oda12()
        {
            InitializeComponent();
        }

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

        private void Oda12_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        // Galerideki fotoğrafların otomatik değişmesi için tanımladığımız timer ı başlatıyoruz.
        private void Oda12_Load(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            timer1.Interval = 4000;
            timer1.Start();
        }

        // Anasayfa formuna geçiş için butonumuzun click eventini ayarlıyoruz.
        private void btn12Kapat_Click(object sender, EventArgs e)
        {
            new AnaSayfa().Show();
            this.Close();
        }

        // İlk galeri formuna geçiş için butonumuzun click eventini ayarlıyoruz.
        private void button1_Click(object sender, EventArgs e)
        {
            new OdaKV().Show();
            this.Close();
        }

        // Her galerinin hangi fotoğrafları göstereceğini ayarlamak için değişken tanımlıyoruz.
        private int imgFirst = 11;
        private int imgSecond = 16;

        // Galeride fotoğrafların otomatik gelmesi için timerımızın tick eventini ayarlıyoruz.
        private void timer1_Tick(object sender, EventArgs e)
        {
            // Galerilerin hangi fotoğrafları göstereceğini ayarlayıp fotoğrafların konumunu belirliyoruz.
            if (imgFirst == 16)
            {
                imgFirst = 11;
            }
            pbFirst.ImageLocation = string.Format(@"Images\{0}.jpg", imgFirst);
            imgFirst++;

            if (imgSecond == 21)
            {
                imgSecond = 16;
            }
            pbSecond.ImageLocation = string.Format(@"Images\{0}.jpg", imgSecond);
            imgSecond++;
        }

        
    }
}
