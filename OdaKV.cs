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
    public partial class OdaKV : Form
    {
        public OdaKV()
        {
            InitializeComponent();
        }

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

        private void OdaKV_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        // Anasayfa formuna geçiş için butonumuzun click eventini ayarlıyoruz.
        private void btnKVKapat_Click(object sender, EventArgs e)
        {
            new AnaSayfa().Show();
            this.Close();
        }

        // Galerideki fotoğrafların otomatik değişmesi için tanımladığımız timer ı başlatıyoruz.
        private void OdaKV_Load(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            timer1.Interval = 4000;
            timer1.Start();
        }

        // İkinci galeri formuna geçiş için butonumuzun click eventini ayarlıyoruz.
        private void button1_Click(object sender, EventArgs e)
        {
            new Oda12().Show();
            this.Close();
        }

        // Her galerinin hangi fotoğrafları göstereceğini ayarlamak için değişken tanımlıyoruz.
        private int imgKing = 1;
        private int imgVip = 6;

        // Galeride fotoğrafların otomatik gelmesi için timerımızın tick eventini ayarlıyoruz.
        private void timer1_Tick(object sender, EventArgs e)
        {
            // Galerilerin hangi fotoğrafları göstereceğini ayarlayıp fotoğrafların konumunu belirliyoruz.
            if (imgKing == 6)
            {
                imgKing = 1;
            }
            pbKing.ImageLocation = string.Format(@"Images\{0}.jpg", imgKing);
            imgKing++;

            if (imgVip == 11)
            {
                imgVip = 6;
            }
            pbVip.ImageLocation = string.Format(@"Images\{0}.jpg", imgVip);
            imgVip++;
        }
    }
}
