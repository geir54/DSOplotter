using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;

namespace DSOplotter
{
    static class Program
    {       
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainForm form = new MainForm();                 
   
            Thread ct = new Thread(
            new ThreadStart(   // console thread
            delegate()
            {
                while (true)
                {
                    string command = Console.ReadLine();                  
                }
            }));

            ct.Start();  // Start console thread            

            form.Show();

            form.Activate();
            Application.Run(form);
            Environment.Exit(0);
        }
    }
}
