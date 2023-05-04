using RS.UtilityLib.WinFormCommon.UINotifier;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RS.UtilityLib.WinFormCommon
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
            UIStartForm.ShowForm();
            Application.Run(new FormDemo1());
            Application.Run(new FormUIControlTest());
        }
    }
}