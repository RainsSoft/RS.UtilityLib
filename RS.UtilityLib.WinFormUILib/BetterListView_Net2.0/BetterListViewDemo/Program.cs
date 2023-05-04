using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WinRemoteDesktop;

namespace BetterListViewDemo
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            Application.Run(new FormMain());
        }
    }
}