using ENBInstall.Properties;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using System.Security.Policy;
using System.Security.Cryptography;
using System.Net;

namespace ENBInstall {
    public struct LinkList {
        public Link[] links;
    }
    public struct Link {
        public string name;
        public string url;
    }
    internal class ENB {

        private static readonly IntPtr HWND_MESSAGE = new IntPtr(-3);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr CreateWindowEx(
            uint dwExStyle, string lpClassName, string lpWindowName, uint dwStyle, int x, int y,
            int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern Boolean DestroyWindow(IntPtr hWnd);

        private const string BaseUrl = "http://enbdev.com/";

        private Dictionary<string, Dictionary<string, string>> _linkCache =
            new Dictionary<string, Dictionary<string, string>>();

        private static bool BrowserLoading = false;
        private static string DownloadLink = "";

        private Dictionary<string, Dictionary<string, string>> games =
            new Dictionary<string, Dictionary<string, string>>();

        private CoreWebView2Environment environment;
        BrowserForm form = new BrowserForm();
        private WebView2 browser {
            get {
                return form.browser;
            }
        }

        private async Task<Dictionary<string, string>> GetLinks(string url) {
            if (_linkCache.TryGetValue(url, out var cached)) {
                return cached;
            }

            while (!form.Visible || BrowserLoading) {
                Debug.WriteLine($"[{url}] Sleeping...");
                Thread.Sleep(100);
            }
            BrowserLoading = true;

            form.Invoke((Action)async delegate {
                Debug.WriteLine($"[{url}] Loading WebView...");
                await browser.EnsureCoreWebView2Async();
                browser.CoreWebView2.DefaultDownloadDialogCornerAlignment =
                    CoreWebView2DefaultDownloadDialogCornerAlignment.TopLeft;
                browser.CoreWebView2.DefaultDownloadDialogMargin = new Point(0, 0);

                Debug.WriteLine($"[{url}] Fetching...");
                browser.CoreWebView2.Navigate($"{BaseUrl}{url}");
                browser.CoreWebView2.DOMContentLoaded += async delegate {
                    var page = browser.CoreWebView2.Source.Substring(BaseUrl.Length);
                    if (_linkCache.TryGetValue(url, out _)) return;
                    var result = await browser.CoreWebView2.ExecuteScriptAsync("Array.from(document.querySelectorAll(\"a[href*=mod],a[href*=zip]\")).map(e=>({[e.innerHTML]: e.href.replace(location.origin+'/', '')}))");
                    var links = new Dictionary<string, string>();

                    var doc = JsonDocument.Parse(result);
                    foreach (var prop in doc.RootElement.EnumerateArray().SelectMany(item => item.EnumerateObject())) {
                        if (prop.Value.GetString().StartsWith("http"))
                            continue;
                        links[prop.Name] = prop.Value.GetString();
                    }

                    Debug.WriteLine($"[{url}] Loaded!");
                    _linkCache[page] = links;
                    BrowserLoading = false;
                };
            });

            while (!_linkCache.ContainsKey(url)) Thread.Sleep(1);

            Debug.WriteLine($"[{url}] Done!");
            return _linkCache[url];
        }

        private bool ShouldArchive = false;

        public async void Archive() {
            ShouldArchive = true;
        }

        private async void CoreWebView2_SourceChanged(object _, CoreWebView2SourceChangedEventArgs e) {
        }

        public async void Update() {
            var formThread = new Thread(() => {
                form.ShowDialog();
            });
            formThread.SetApartmentState(ApartmentState.STA);
            formThread.Start();

            while (!form.Visible) Thread.Sleep(1);

            form.Invoke((Action)async delegate {
                await browser.EnsureCoreWebView2Async();
                browser.SourceChanged += CoreWebView2_SourceChanged;
                browser.CoreWebView2.DOMContentLoaded += CoreWebView2_DOMContentLoaded;
            });

            if (!File.Exists("versions.json")) {
                UpdateGames(await GetLinks("download.html"));
                File.WriteAllText("versions.json", JsonSerializer.Serialize(games));
            } else {
                games = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(File.ReadAllText("versions.json"));
            }
        }

        private async void CoreWebView2_DOMContentLoaded(object _, CoreWebView2DOMContentLoadedEventArgs e) {
            if (!ShouldArchive) return;

            foreach (var game in games) {
                foreach (var version in game.Value) {
                    await Task.Run(() => {
                        var t = new TaskCompletionSource<bool>();

                        form.Invoke((Action)async delegate {
                            var outPath = Path.Combine(Application.StartupPath, "Downloads", game.Key.Replace("/", "and"), version.Key + ".zip");
                            if (File.Exists(outPath)) {
                                t.TrySetResult(true);
                                return;
                            }

                            Console.WriteLine($"Trying to download {version.Value}");
                            await browser.CoreWebView2.ExecuteScriptAsync($@"var a = document.createElement('a');a.href=""{BaseUrl}{version.Value}"";a.download=""{version.Value}"";document.body.appendChild(a);a.click()");
                            browser.CoreWebView2.DownloadStarting += (sender, args) => {
                                args.ResultFilePath = outPath;
                                args.DownloadOperation.StateChanged += (o, o1) => {
                                    switch (args.DownloadOperation.State) {
                                        case CoreWebView2DownloadState.InProgress:
                                        case CoreWebView2DownloadState.Completed:
                                            t.TrySetResult(true);
                                            break;
                                        case CoreWebView2DownloadState.Interrupted:
                                            break;
                                    }
                                };
                            };
                        });

                        return t.Task;
                    });
                }
            }
        }

        public Dictionary<string, Dictionary<string, string>> Games => games;

        private async void UpdateGames(Dictionary<string, string> result) {
            foreach (var game in result) {
                UpdateVersions(game.Key, await GetLinks(game.Value));
            }
        }

        private async void UpdateVersions(string game, Dictionary<string, string> result) {
            games[game] = new Dictionary<string, string>();
            foreach (var version in result.Where(r => !r.Value.EndsWith(".zip"))) {
                DownloadVersion(game, version.Key, await GetLinks(version.Value));
            }
        }

        private async void DownloadVersion(string game, string version, Dictionary<string, string> result) {
            Console.WriteLine($"[{game}] {version}");
            var link = result.First().Value;
            games[game][version] = link;
        }
    }
}
