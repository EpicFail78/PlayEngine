using System;
using System.Windows.Forms;
using PS4_Cheater.Utils;

namespace PS4_Cheater {
   internal static class Program {
      private static void onProcessExit(Object sender, EventArgs e) {
         Settings.setValue(Settings.SettingsKey.PS4IPAddress, SharedInformation.PS4_IPAddress);
         Settings.setValue(Settings.SettingsKey.PS4IPPort, SharedInformation.PS4_IPPort);
         Settings.setValue(Settings.SettingsKey.PS4Version, SharedInformation.PS4_Version);
      }

      [STAThread]
      private static void Main() {
         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);
         Application.ApplicationExit += new EventHandler(onProcessExit);
         Application.Run(new Forms.MainForm());
      }
   }
}

