using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DSOplotter
{
    public partial class Form1 : Form
    {
        Bitmap bmp;
        List<int> point = new List<int>();
        int c_t1 = 100;
        int c_t2 = 100;
        int zoom=1;
        double sampleTime;
       
        public Form1(string filename)
        {
            read_file(filename);
            InitializeComponent();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            
        }

        // read and parse all the XML
        private void read_file(String filename) {                  
            System.IO.StreamReader File = new System.IO.StreamReader(filename);
            string temp;
         
            File.ReadLine(); // <?xml version="1.0" encoding="UTF-8"?>
            File.ReadLine(); // <Document>
            File.ReadLine();// <Profile>
            File.ReadLine(); // <triggerMode>AUTO</triggerMode>
            File.ReadLine(); // <triggerKind>EdgeRising</triggerKind>
            File.ReadLine();// <triggerLevel>2.80V</triggerLevel>
            File.ReadLine(); // <triggerSensitivity>0.00mV</triggerSensitivity>
            File.ReadLine(); // <attenuation>x1</attenuation>
            File.ReadLine();// <voltageDiv>1V</voltageDiv>
            File.ReadLine(); // <timeDiv>200us</timeDiv>
            File.ReadLine();// <sampleDiv>200us</sampleDiv>
            File.ReadLine();// <firmware>V3.64</firmware>
            File.ReadLine();// <fileNumber>S004</fileNumber>
            File.ReadLine();// <triggerIndex>1400</triggerIndex>

            temp = File.ReadLine();// <sampleCount>4098</sampleCount>
            temp = temp.Substring(13, temp.Length - 27);
            int sampleCount = Int32.Parse(temp);

            temp = File.ReadLine();// <timeRange>32.78e-3</timeRange>
            temp = temp.Substring(11,temp.Length-26); // Trick remove e-3 **** Can and will probably make trouble                
            double timeRange = double.Parse(temp.Replace(".",",")) * 0.001;                       
           
            File.ReadLine();// </Profile>          

            Console.WriteLine("timeRange " + timeRange + "s");
            Console.WriteLine("sampleCount " + sampleCount);
            sampleTime = timeRange / sampleCount;
            Console.WriteLine("sampleTime " + sampleTime);
            
            // Read all the samples
            string inn = "";
            while (!File.EndOfStream)
            {
                inn = File.ReadLine();
                if (inn.Substring(0, 5).CompareTo("<val>") == 0)
                {
                    inn = inn.Replace(".", ",");
                    temp = inn.Substring(5, inn.Length - 11);
                    point.Add((int)(Double.Parse(temp) * 1000));
                }
            }

            File.Close();

            Console.WriteLine("file read");
        }


        // Create an image of the plot
        private void paint() {
            int W = pictureBox1.Size.Width;
            int H = pictureBox1.Size.Height;
            bmp = new Bitmap(W, H);
           
            Graphics g = Graphics.FromImage(bmp);

           // System.Drawing.Graphics g;
           // g = pictureBox1.CreateGraphics();
            g.Clear(System.Drawing.Color.Black);

            // find min max
            int max = 0;
            int min = 99999;
            for (int i = 0; i < point.Count; i++)
            {
                if (point[i] > max) max = point[i];
                if (point[i] < min) min = point[i];
            }

            int GND = 0;

            if (min < 0)
            {
                for (int i = 0; i < point.Count; i++)
                {
                    point[i] = point[i] + (min * -1); // We want it all to be positive
                }

                GND = (min * -1);
                max = max + (min * -1);
                min = 0;
            }       

            // scale
            double y = ((double)(pictureBox1.Size.Height) / (max));
            GND = (int)Math.Round((double)GND * y, 0);

            // draw GND
            Pen GNDPen = new Pen(System.Drawing.Color.Yellow, 1);
            g.DrawLine(GNDPen, 0, H - GND, W, H - GND);
            // GND end
            
            Pen myPen = new Pen(System.Drawing.Color.Green, 1);
            
            hScrollBar1.Maximum = point.Count - W;
            int start_point = hScrollBar1.Value;

          
            int j = 0;
            for (int i = start_point; i < W + start_point; i++)
            {
                int v1 = (int)Math.Round((double)point[i] * y, 0);
                int v2 = (int)Math.Round((double)point[i + 1] * y, 0);
                v1 = pictureBox1.Size.Height - v1;
                v2 = pictureBox1.Size.Height - v2;
                int x1 = (int)Math.Round((double)i);
                int x2 = (int)Math.Round((double)(i + 1));

                g.DrawLine(myPen, j, v1, j + zoom, v2);
                j = j + zoom;

            }
            pictureBox1.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            paint();
        }
 
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (checkBox1.Checked) c_t1 = e.X;
            if (checkBox2.Checked) c_t2 = e.X;

            label1.Text = "t1 = " + (c_t1 / zoom) * sampleTime * 1000000; //µS
            label2.Text = "t2 = " + (c_t2 / zoom) * sampleTime * 1000000; //µS
            label3.Text = "Δt = " + ((c_t2 - c_t1) / zoom) * sampleTime * 1000000;     //µS  
            pictureBox1.Refresh();
        }

        // Paint the image created on to the box
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {        
            if (bmp != null) e.Graphics.DrawImage(bmp, new Point(0, 0));

            // Draw cursors
            System.Drawing.Graphics j = e.Graphics;
 
            Pen te = new Pen(System.Drawing.Color.Yellow, 1);
            j.DrawLine(te, c_t1, 0, c_t1, pictureBox1.Size.Height);//pictureBox1.Size.Height);
            j.DrawLine(te, c_t2, 0, c_t2, pictureBox1.Size.Height);
        }

        private void numericUpDown1_Validated(object sender, EventArgs e)
        {
            zoom = (int)numericUpDown1.Value;
        }
    }
}
