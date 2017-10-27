/************************************************************************************************************
 * Coding challenge 337 from Reddit
 * 
 *
    [2017-10-26] Challenge #337 [Intermediate] Scrambled images
    #Description
    For this challenge you will get a couple of images containing
    a secret word, you will have to unscramble the images to be able to read the words.  
    To unscramble the images you will have to line up all non-gray scale pixels on each "row" of the image.

    #Formal Inputs &amp; Outputs
    You get a [scrambled](http://i.imgur.com/rMYBq14.png) image,
    which you will have to unscramble to get the [original](http://i.imgur.com/wKaiHpv.png) image.

    ###Input description
    Challenge 1:  [input](http://i.imgur.com/F4SlYMn.png)  
    Challenge 2: [input](http://i.imgur.com/ycDwgXA.png)  
    Challenge 3: [input](http://i.imgur.com/hg9iVXA.png)

    ###Output description
    You should post the correct images or words.

    #Notes/Hints
    The colored pixels are red (#FF0000, rgb(255, 0, 0)) 

    #Bonus
    Bonus: [input](http://i.imgur.com/HLc1UHv.png)

    This image is scrambled both horizontally and vertically.  
    The colored pixels are a gradient from green to red ((255, 0, _), (254, 1, _), ..., (1, 254, _), (0, 255, _)).

    #Finally
    Have a good challenge idea?
    Consider submitting it to /r/dailyprogrammer_ideas
    https://www.reddit.com/r/dailyprogrammer/comments/78twyd/20171026_challenge_337_intermediate_scrambled/
 *
 *************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Challenge337
{
    public partial class Form1 : Form
    {
        private Bitmap _srcImage = null;        // Bitmap to hold loaded image

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

            // draw image to screen
            using (Graphics graphics = pictureBox.CreateGraphics())
            {
                graphics.DrawImage(_srcImage, 0, 0);
            }
        }

        // Descramble 1
        // Line up the red pixels
        //
        // This only works on red pixel scrambled images
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
                    /*Debug.Assert(p < imagewidth);*/
                    if(p >= imagewidth)
                    {
                        MessageBox.Show("Unable to descramble\nTry the other button!");
                        return;
                    }

                    for (int x = p; x < (p + imagewidth); x++)
                    {
                        finalimage.SetPixel((x - p), y, _srcImage.GetPixel((x % imagewidth), y));
                    }
                    graphics.DrawLine(pen, 0, y, imagewidth - 1, y);
                }

                graphics.DrawImage(finalimage, 0, 0);
            }
        }

        //
        private bool IsGreyscale(Color c) => ((c.R == c.G) && (c.R == c.B));

        // Full Solution
        // Works for images scrambled by a non-grey key colour.
        // We then sort the image lines by R-G scale.
        private void button4_Click(object sender, EventArgs e)
        {
            int imagewidth = _srcImage.Width;
            int imageheight = _srcImage.Height;

            using (Graphics graphics = pictureBox.CreateGraphics())
            using (Bitmap finalimage = new Bitmap(imagewidth, imageheight))
            using (Bitmap sortedimage = new Bitmap(imagewidth, imageheight))
            using (Pen pen = new Pen(Color.OrangeRed))
            using (Pen hpen = new Pen(Color.Crimson))
            {
                // clear output
                graphics.FillRectangle(new SolidBrush(Color.White), pictureBox.ClientRectangle);

                for (int y = 0; y < imageheight; y++)
                {
                    // find the x-index of the start of our key colour
                    int p = 0;
                    for (; p < imagewidth; p++)
                    {
                        // check for non-grey pixel
                        if (IsGreyscale(_srcImage.GetPixel(p, y)))
                            continue;

                        if ((p + 1 >= imagewidth) || (p + 2 >= imagewidth))
                            break;

                        if (!IsGreyscale(_srcImage.GetPixel(p + 1, y)) && !IsGreyscale(_srcImage.GetPixel(p + 2, y)))
                            break;
                    }

                    // we should now have the position of our key colour (red).
                    // Check that we have found it
                    /*Debug.Assert(p < imagewidth);*/

                    if (p >= imagewidth)
                    {
                        MessageBox.Show("Unable to descramble!");
                        return;
                    }

                    for (int x = p; x < (p + imagewidth); x++)
                    {
                        finalimage.SetPixel((x - p), y, _srcImage.GetPixel((x % imagewidth), y));
                    }
                    graphics.DrawLine(pen, 0, y, imagewidth - 1, y);
                }

                graphics.DrawImage(finalimage, 0, 0);

                // sort by key colour red -> green
                // ((255, 0, _), (254, 1, _), ..., (1, 254, _), (0, 255, _)).
                var lines = new List<KeyValuePair<Color, int>>();
                for (int yA = 0; yA < imageheight; yA++)
                    lines.Add( new KeyValuePair<Color, int>(finalimage.GetPixel(0, yA), yA) );

                int sY = 0;
                foreach (var line in lines.OrderBy(key => key.Key.R).ThenBy(key => key.Key.G))
                {
                    for (int x = 0; x < imagewidth; x++)
                    {
                        sortedimage.SetPixel(x, sY, finalimage.GetPixel(x, line.Value));
                    }
                    sY++;

                    graphics.DrawLine(hpen, 0, sY, imagewidth - 1, sY);
                }

                graphics.DrawImage(sortedimage, 0, 0);
            } // using

        }

        // save results
        private void button3_Click(object sender, EventArgs e)
        {
            /***** TODO ***
            SaveFileDialog filedialog = new SaveFileDialog();
            filedialog.Filter = "PNG Format|*png";
            if(filedialog.ShowDialog() == DialogResult.OK)
            {
                pictureBox.Image.Save(filedialog.FileName, ImageFormat.Png);
            }*/
        }

        // clean up when we are closing
        private void Form1_FormClosing(object sender, FormClosingEventArgs e) => _srcImage?.Dispose();
    }
}
