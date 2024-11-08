using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ENBInstall {
    internal class Program {
        [STAThread]
        private static void Main(string[] args) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
            /*var enb = new ENB();
            enb.Update();
            enb.Archive();

            Console.ReadKey(true);*/
        }
    }
}
