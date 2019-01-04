using FlysEngine.Desktop;
using FlysEngine.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lottery
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using (var window = new SpriteWindow())
            {
                RenderLoop.Run(window, () => window.Render(1, 0));
            }
        }
    }
}
