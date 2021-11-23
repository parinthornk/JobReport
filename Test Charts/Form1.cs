using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test_Charts
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();


            chart1.Series["Series1"].Points.AddXY("", "45");
            chart1.Series["Series1"].Points.AddXY("", "20");
            chart1.Series["Series1"].Points.AddXY("", "35");






        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (Bitmap bmp = new Bitmap(this.Width, this.Height))
            {
                this.DrawToBitmap(bmp, new Rectangle(Point.Empty, bmp.Size));
                bmp.Save(@"C:\Users\parin\Desktop\node_modules\sample.png", ImageFormat.Png); // make sure path exists!
            }
        }
    }
}
