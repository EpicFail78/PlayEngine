using System;
using System.Windows.Forms;

using PlayEngine.Helpers;

namespace PlayEngine {
   internal static class Program {
      [STAThread]
      private static void Main() {
         Settings.mInstance = Settings.loadSettings();

         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);
         Application.Run(new Forms.MainForm());
      }
   }
}

