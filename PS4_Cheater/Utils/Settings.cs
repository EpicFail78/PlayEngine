using System;
using System.IO;
using System.Xml.Serialization;

namespace PS4_Cheater.Utils {
   public enum PS4FWVersion {
      v4_05 = 0,
      v4_55 = 1,
      v5_05 = 2
   }
   [Serializable]
   [XmlRoot("PS4")]
   public class PS4Settings {
      [XmlElement]
      public String IPAddress = String.Empty;
      [XmlElement]
      public Int32 IPPort = 9020;
      [XmlElement]
      public PS4FWVersion FWVersion = PS4FWVersion.v5_05;
   }
   [Serializable]
   [XmlRoot("Settings")]
   [XmlInclude(typeof(PS4Settings))]
   public class Settings {
      [XmlIgnore]
      public static Settings mInstance = null;
      [XmlIgnore]
      public static XmlSerializer serializer = null;

      [XmlElement("PS4")]
      public PS4Settings ps4 = new PS4Settings();
      [XmlElement("FastScan")]
      public Boolean fastScanEnabled = true;

      public static Settings loadSettings(String settingsFilePath = "Settings.xml") {
         if (serializer == null)
            serializer = new XmlSerializer(typeof(Settings));

         Settings settings = null;
         if (!System.IO.File.Exists(settingsFilePath)) {
            settings = new Settings();
            settings.saveToFile();
            return settings;
         }

         using (StreamReader reader = new StreamReader(settingsFilePath))
            settings = (Settings)serializer.Deserialize(reader);
         return settings;
      }
      public Boolean saveToFile(String settingsFilePath = "Settings.xml") {
         if (serializer == null)
            serializer = new XmlSerializer(typeof(Settings));

         try {
            using (StreamWriter writer = new StreamWriter(settingsFilePath))
               serializer.Serialize(writer, this);
            return true;
         } catch (Exception) {
            return false;
         }
      }
   }
}
