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
    public partial class SifreYenile : Form
    {
        public SifreYenile()
        {
            InitializeComponent();
        }

        // Form hareketliliği için kodlar.
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

        classgiris sifreyolla = new classgiris();

        // Şifre yollamak için metodumuzu çağırıyoruz.
        private void btnSifreYenile_Click(object sender, EventArgs e)
        {
            sifreyolla.sifre(txtSifreUsername,txtSifreEposta);
            txtSifreUsername.Text = "";
            txtSifreEposta.Text = "";
        }

        // Giriş formuna geçiş için butonumuzun click eventini ayarlıyoruz.
        private void btnSifreKapat_Click(object sender, EventArgs e)
        {
            new Giris().Show();
            this.Hide();
        }

        // Form hareketliliği için mousedown event i kullanıyoruz.
        private void SifreYenile_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }
    }
}
