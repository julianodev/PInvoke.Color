using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace PInvoke.Color
{
    public partial class Main : Form
    {
        [DllImport("user32.dll")]
        static extern bool GetCursorPos(ref Point lpPoint);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);

        readonly Bitmap screenPixel = new Bitmap(1, 1, PixelFormat.Format32bppArgb);

        public Main()
        {
            InitializeComponent();
        }

        private async void Main_Load(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    var cursor = new Point();

                    GetCursorPos(ref cursor);

                    var c = GetColorAt(cursor);

                    this.BackColor = c;

                    Action<string> action = (color) => this.label1.Text = $"{color}";

                    this.Invoke(action, new object[] { c.Name });
                }
            });
        }

        private System.Drawing.Color GetColorAt(
            Point location)
        {
            using (var gdest = Graphics.FromImage(screenPixel))
            {
                using (var gsrc = Graphics.FromHwnd(IntPtr.Zero))
                {
                    var hSrcDC = gsrc.GetHdc();
                    var hDC = gdest.GetHdc();
                    var retval = BitBlt(
                        hDC,
                        0,
                        0,
                        1,
                        1,
                        hSrcDC,
                        location.X,
                        location.Y,
                        (int)CopyPixelOperation.SourceCopy);
                    gdest.ReleaseHdc();
                    gsrc.ReleaseHdc();
                }
            }

            return screenPixel.GetPixel(0, 0);
        }
    }
}
