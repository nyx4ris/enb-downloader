using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ENBInstall {
    public partial class BrowserForm : Form {
        public BrowserForm() {
            InitializeComponent();
        }

        private async void BrowserForm_Load(object sender, EventArgs e) {
            await browser.EnsureCoreWebView2Async();
            browser.CoreWebView2.Navigate("http://enbdev.com");
        }
    }
}
