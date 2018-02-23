using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace VocalUtau.Formats.Demo
{
    static class Program1
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main1()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
