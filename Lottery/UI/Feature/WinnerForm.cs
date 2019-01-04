using Lottery2019.Images;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Lottery2019.UI.Feature
{
    [ComVisible(true)]
    public partial class WinnerForm : Form
    {
        private PrizeDto prize;
        private List<Person> winners;

        public WinnerForm(PrizeDto prize, List<Person> winners)
        {
            this.prize = prize;
            this.winners = winners;
            InitializeComponent();
            InitBrowser("./Resources/Html/Winner.html");
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

        public void BrowserLoaded()
        {
            var shit = browser.Document.InvokeScript("Init", new object[]
            {
                Json(prize), 
                Json(winners)
            });

            string Json(object o)
            {
                return JsonConvert.SerializeObject(o, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });
            }
        }

        public static void Show(PrizeDto prize, List<Person> winners)
        {
            using (var form = new WinnerForm(prize, winners))
            {
                form.ShowDialog();
            }
        }
    }
}
