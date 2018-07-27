using System;
using System.Configuration;
using System.Windows.Forms;

namespace PS4_Cheater.Utils {
   public static class Settings {
      public static class SettingsKey {
         public static readonly String PS4IPAddress = "PS4IPAddress";
         public static readonly String PS4IPPort = "PS4IPPort";
         public static readonly String PS4Version = "PS4Version";
      }

      public static Boolean setValue<T>(String valueKey, T value) {
         try {
            var config = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
            var settings = config.AppSettings.Settings;
            if (settings[valueKey] == null)
               settings.Add(valueKey, value.ToString());
            else
               settings[valueKey].Value = value.ToString();

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
         } catch (Exception ex) {
            MessageBox.Show("Error during Utils.Settings.setValue\r\n" + ex.ToString(), "ERROR");
            return false;
         }
         return true;
      }

      public static T getValue<T>(String valueKey) {
         try {
            var settingsEntry = ConfigurationManager.AppSettings[valueKey];
            if (settingsEntry != null) {
               if (typeof(T).IsEnum)
                  return (T)Enum.Parse(typeof(T), settingsEntry);
               return (T)Convert.ChangeType(settingsEntry, typeof(T));
            }
         } catch (Exception ex) {
            MessageBox.Show("Error during Utils.Settings.getValue\r\n" + ex.ToString(), "ERROR");
         }
         return default(T);
      }
   }
}
