using System;
using System.Threading;
using System.Windows.Forms;

namespace RohimToolBox {
  static class Program {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main() {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

      Application.ThreadException += Application_ThreadException;
      Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

      Application.Run(new frmMain());
    }

    private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e) {
      string msg = $"Message:{e.Exception.Message}\r\n" +
        $"Type:{e.Exception.GetType()}\r\n" +
        $"{e.Exception.StackTrace}";

      MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
  }
}
