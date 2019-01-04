using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Lottery2019.UI.Feature
{
    [ComVisible(true)]
    public partial class PickPrizeWindow : Form
    {
        private PrizeDto _prize;
        private readonly string[] _winnedPrizes;

        private PickPrizeWindow(string[] winnedPrizes)
        {
            _winnedPrizes = winnedPrizes;
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            InitBrowser("./resources/html/PickPrize.html");
        }

        private void InitBrowser(string url)
        {
            browser.AllowWebBrowserDrop = false;
            browser.IsWebBrowserContextMenuEnabled = false;
            browser.WebBrowserShortcutsEnabled = false;
            browser.ObjectForScripting = this;
            var absolutePath = Path.GetFullPath(url);
            browser.Navigate($"file:///{absolutePath}");
        }

        public void PrizeSelected(string prizeId)
        {
            _prize = SystemPrizeProvider.Prizes[prizeId];
            Close();
        }

        public void BrowserLoaded()
        {
            var shit = browser.Document.InvokeScript("InitPrizes", new object[] 
            {
                JsonConvert.SerializeObject(SystemPrizeProvider.Prizes, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }),
                JsonConvert.SerializeObject(_winnedPrizes)
            });
        }

        internal static PrizeDto PickPrize(string[] winnedPrizes)
        {
            using (var form = new PickPrizeWindow(winnedPrizes))
            {
                form.ShowDialog();
                return form._prize;
            }
        }
    }
}
