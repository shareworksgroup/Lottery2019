using FlysEngine.Desktop;
using Lottery2019.Images;
using Lottery2019.UI.Details;
using System;

namespace Lottery2019
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            FarseerPhysics.Settings.VelocityThreshold = 0;
            ImageUtil.ProcessImages();

            IEUtil.UsingLatestIE();
            using (var form = new Form1())
            {
                RenderLoop.Run(form, () => form.Render(1, 0));
            }
        }
    }
}
