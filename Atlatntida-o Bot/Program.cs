using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Awesomium.Core;

namespace Atlatntida_o_Bot
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            if (!WebCore.IsRunning)
                WebCore.Initialize(new WebConfig() { UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36" });
            Application.Run(new MainForm());
        }
    }
}
