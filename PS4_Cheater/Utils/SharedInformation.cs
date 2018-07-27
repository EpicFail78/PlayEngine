using System;

namespace PS4_Cheater.Utils {
   public enum PS4Version {
      v4_05,
      v4_55,
      v5_05
   }
   public static class SharedInformation {
      public static String PS4_IPAddress = String.Empty;
      public static String PS4_IPPort = "9020";
      public static PS4Version PS4_Version = PS4Version.v5_05;

      public static Boolean FastScan = true;

      public static void loadValuesFromSettings() {
         var strPS4IPAddress = Settings.getValue<String>(Settings.SettingsKey.PS4IPAddress);
         if (!String.IsNullOrWhiteSpace(strPS4IPAddress))
            SharedInformation.PS4_IPAddress = strPS4IPAddress;
         else
            Settings.setValue<String>(Settings.SettingsKey.PS4IPAddress, SharedInformation.PS4_IPAddress);

         var strPS4IPPort = Settings.getValue<String>(Settings.SettingsKey.PS4IPPort);
         if (!String.IsNullOrWhiteSpace(strPS4IPPort))
            SharedInformation.PS4_IPPort = strPS4IPPort;
         else
            Settings.setValue<String>(Settings.SettingsKey.PS4IPPort, SharedInformation.PS4_IPPort);

         var strPS4Version = Settings.getValue<String>(Settings.SettingsKey.PS4Version);
         if (!String.IsNullOrWhiteSpace(strPS4Version))
            SharedInformation.PS4_Version = (PS4Version)Enum.Parse(typeof(PS4Version), strPS4Version);
         else
            Settings.setValue<PS4Version>(Settings.SettingsKey.PS4Version, SharedInformation.PS4_Version);
      }
   }
}
