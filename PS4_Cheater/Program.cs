using System;
using System.Windows.Forms;

using PS4_Cheater.Utils;

namespace PS4_Cheater {
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

