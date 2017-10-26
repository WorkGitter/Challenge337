using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Challenge337
{
    public partial class Form1 : Form
    {
        private Bitmap _srcImage = null;

        public Form1()
        {
            InitializeComponent();
        }

        // Open an image and display to the picturebox
        private void button1_MouseClick(object sender, MouseEventArgs e)
        {
            OpenFileDialog filedialog = new OpenFileDialog();
            filedialog.Filter = "PNG images|*.png";
            if (filedialog.ShowDialog() != DialogResult.OK)
                return;

            // load image and display to 
            try
            {
                _srcImage?.Dispose();
                _srcImage = (Bitmap)Image.FromFile(filedialog.FileName);
            }
            catch (OutOfMemoryException)
            {
                MessageBox.Show("Image not supported", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch(FileNotFoundException)
            {
                return;
            }
            catch(ArgumentException)
            {
                return;
            }

            // draw image to screen
            using (Graphics graphics = pictureBox.CreateGraphics())
            {
                graphics.DrawImage(_srcImage, 0, 0);
            }
        }

        // Descramble
        // Line up the red pixels
        //
        private void button2_MouseClick(object sender, MouseEventArgs e)
        {
            int imagewidth = _srcImage.Width;
            int imageheight = _srcImage.Height;
            Color keycolour = Color.FromArgb(255, 0, 0);

            using (Graphics graphics = pictureBox.CreateGraphics())
            using (Bitmap finalimage = new Bitmap(imagewidth, imageheight))
            using( Pen pen = new Pen(Color.Red))
            {
                // clear output
                graphics.FillRectangle(new SolidBrush(Color.White), pictureBox.ClientRectangle);

                for (int y = 0; y < imageheight; y++)
                {
                    // find the x-index of the start of our key colour
                    int p = 0;
                    for (; p < imagewidth; p++)
                    {
                        if (_srcImage.GetPixel(p, y) == keycolour)
                            break;
                    }

                    // we should now have the position of our key colour (red).
                    // Check that we have found it
                    Debug.Assert(p < imagewidth);
                    for (int x = p; x < (p + imagewidth); x++)
                    {
                        finalimage.SetPixel((x - p), y, _srcImage.GetPixel((x % imagewidth), y));
                    }
                    graphics.DrawLine(pen, 0, y, imagewidth - 1, y);
                }

                graphics.DrawImage(finalimage, 0, 0);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e) => _srcImage?.Dispose();
    }
}
