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
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
        string filename="";

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "XML files (*.XML)|*.xml|All files (*.*)|*.*";          
            dialog.Title = "Select a text file";
            if (dialog.ShowDialog() == DialogResult.OK) filename = dialog.FileName;

            if (filename == String.Empty)
            {
                Console.WriteLine("Nothing");
                return;//user didn't select a file
            }           
            
            Form1 chForm = new Form1(filename);     
            chForm.MdiParent = this;
            chForm.Text = filename;
            chForm.Show(); 
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("DSOplotter\nCopyright (c) 2013 Geir54\nhttps://github.com/geir54/DSOplotter");
        }
    }
}
