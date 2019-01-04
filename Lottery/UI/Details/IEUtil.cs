using Microsoft.Win32;
using System.Diagnostics;
using System.Windows.Forms;

namespace Lottery2019.UI.Details
{
    public static class IEUtil
    {
        static IEUtil()
        {
            UsingLatestIERaw();
        }

        private static void UsingLatestIERaw()
        {
            int browserVersion, regVal;

            // get the installed IE version
            using (WebBrowser Wb = new WebBrowser())
                browserVersion = Wb.Version.Major;

            // set the appropriate IE version
            if (browserVersion >= 11)
                regVal = 11001;
            else if (browserVersion == 10)
                regVal = 10001;
            else if (browserVersion == 9)
                regVal = 9999;
            else if (browserVersion == 8)
                regVal = 8888;
            else
                regVal = 7000;

            // set the actual key
            using (RegistryKey subKey = Registry.CurrentUser.CreateSubKey(
                @"SOFTWARE\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION",
                RegistryKeyPermissionCheck.ReadWriteSubTree))
            {
                var currentPsName = Process.GetCurrentProcess().ProcessName + ".exe";
                if (subKey.GetValue(currentPsName) == null)
                {
                    subKey.SetValue(currentPsName, regVal, RegistryValueKind.DWord);
                }
            }
        }

        public static void UsingLatestIE()
        {
            // do nothing
        }
    }
}
