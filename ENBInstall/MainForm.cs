using ENBInstall.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.Web.WebView2.Core;

namespace ENBInstall {
    struct Fetch {
        public string Url;
        public Action<Dictionary<string, string>> Callback;
    }

    public partial class MainForm : Form {
        private static readonly WebClient WebClient = new WebClient();
        private const string BaseUrl = "http://enbdev.com/";
        private static readonly WebView2 Browser = new WebView2();

        private Dictionary<string, Dictionary<string, string>> _linkCache =
            new Dictionary<string, Dictionary<string, string>>();

        private static bool BrowserLoading = false;
        private static string DownloadLink = "";

        private Dictionary<string, string> GetLinks(string url) {
            if (_linkCache.TryGetValue(url, out var cached))
                return cached;

            if (BrowserLoading)
                return null;
            BrowserLoading = true;
            var regex = new Regex(@"<a href=""(.*?)"">(.*?)</a>", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.ECMAScript);

            Invoke((Action)delegate {
                Browser.CoreWebView2.Navigate($"{BaseUrl}{url}");
                Browser.CoreWebView2.DOMContentLoaded += async delegate {
                    var result = await Browser.CoreWebView2.ExecuteScriptAsync("document.body.outerHTML");
                    var html = JsonSerializer.Deserialize<string>(result);
                    Debug.WriteLine(html);
                    var matches = regex.Matches(html);
                    var links = new Dictionary<string, string>();
                    for (var i = 0; i < matches.Count; i++) {
                        var match = matches[i];
                        if (!links.ContainsKey(match.Groups[2].Value))
                            links[match.Groups[2].Value] = match.Groups[1].Value;
                    }

                    Debug.WriteLine(Browser.CoreWebView2.Source);
                    _linkCache[Browser.CoreWebView2.Source.Substring(BaseUrl.Length)] = links;

                    BrowserLoading = false;
                };
            });
            return null;
        }

        public MainForm() {
            InitializeComponent();
            Browser.Visible = false;
            Browser.Size = Size;
            Browser.Location = Location;
            Controls.Add(Browser);
        }

        private async void MainForm_Load(object sender, EventArgs e) {
            await Browser.EnsureCoreWebView2Async();
            Browser.CoreWebView2.DefaultDownloadDialogCornerAlignment =
                CoreWebView2DefaultDownloadDialogCornerAlignment.TopLeft;
            Browser.CoreWebView2.DefaultDownloadDialogMargin = new Point(0, 0);

            RunFetch(new Fetch {
                Url = "download.html",
                Callback = UpdateGames
            });
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start(Resources.Repository);
        }

        private void RunFetch(Fetch fetch) {
            new Thread(() => {
                Dictionary<string, string> result;
                while ((result = GetLinks($"{fetch.Url}")) == null)
                    Thread.Sleep(100);

                Invoke((Action)delegate {
                    fetch.Callback(result);
                });
            }).Start();
        }

        private void UpdateGames(Dictionary<string, string> result) {
            cmbGame.BeginUpdate();
            foreach (var game in result.Where(game => game.Value.StartsWith("download_mod_"))) {
                cmbGame.Items.Add(game.Key);

                if (game.Key == "Fallout 4")
                    cmbGame.SelectedIndex = cmbGame.Items.Count - 1;
            }

            cmbGame.Enabled = true;
            cmbGame.EndUpdate();
        }

        private void UpdateVersions(Dictionary<string, string> result) {
            cmbVersions.BeginUpdate();
            foreach (var version in result.Where(version => version.Value.StartsWith("mod_"))) {
                cmbVersions.Items.Add(version.Key);
            }

            cmbVersions.Enabled = true;
            cmbVersions.SelectedIndex = 0;
            cmbVersions.EndUpdate();
        }

        private void SetLink(Dictionary<string, string> result) {
            DownloadLink = result.First(r => r.Key.Contains("download")).Value;
            btnInstall.Enabled = true;
        }

        private void cmbGame_SelectedIndexChanged(object sender, EventArgs e) {
            cmbVersions.Enabled = false;
            cmbVersions.Items.Clear();
            var downloadPage = _linkCache["download.html"][cmbGame.Text];

            RunFetch(new Fetch {
                Url = downloadPage,
                Callback = UpdateVersions
            });
        }

        private void cmbVersions_SelectedIndexChanged(object sender, EventArgs e) {
            btnInstall.Enabled = false;
            var downloadPage = _linkCache["download.html"][cmbGame.Text];
            var versionPage = _linkCache[downloadPage][cmbVersions.Text];

            RunFetch(new Fetch {
                Url = versionPage,
                Callback = SetLink
            });
        }

        private void btnInstall_Click(object sender, EventArgs e) {
            Browser.CoreWebView2.ExecuteScriptAsync($"location.href = \"{DownloadLink}\";");
        }
    }
}
