using Be.Windows.Forms;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Configuration;
using librpc;
using System.Collections.Generic;

namespace PS4_Cheater {

   class GameInfo {
      const String GAME_INFO_4_05_PROCESS_NAME = "SceCdlgApp";
      const String GAME_INFO_4_05_SECTION_NAME = "libSceCdlgUtilServer.sprx";
      const Int32 GAME_INFO_4_05_SECTION_PROT = 3;
      const Int32 GAME_INFO_4_05_ID_OFFSET = 0XA0;
      const Int32 GAME_INFO_4_05_VERSION_OFFSET = 0XC8;

      const String GAME_INFO_4_55_PROCESS_NAME = "SceCdlgApp";
      const String GAME_INFO_4_55_SECTION_NAME = "libSceCdlgUtilServer.sprx";
      const Int32 GAME_INFO_4_55_SECTION_PROT = 3;
      const Int32 GAME_INFO_4_55_ID_OFFSET = 0XA0;
      const Int32 GAME_INFO_4_55_VERSION_OFFSET = 0XC8;

      const String GAME_INFO_5_05_PROCESS_NAME = "SceCdlgApp";
      const String GAME_INFO_5_05_SECTION_NAME = "libSceCdlgUtilServer.sprx";
      const Int32 GAME_INFO_5_05_SECTION_PROT = 3;
      const Int32 GAME_INFO_5_05_ID_OFFSET = 0XA0;
      const Int32 GAME_INFO_5_05_VERSION_OFFSET = 0XC8;

      public String GameID = "";
      public String Version = "";

      public GameInfo() {
         String process_name = "";
         String section_name = "";
         UInt64 id_offset = 0;
         UInt64 version_offset = 0;
         Int32 section_prot = 0;

         switch (Util.Version) {
            case 405:
               process_name = GAME_INFO_4_05_PROCESS_NAME;
               section_name = GAME_INFO_4_05_SECTION_NAME;
               id_offset = GAME_INFO_4_05_ID_OFFSET;
               version_offset = GAME_INFO_4_05_VERSION_OFFSET;
               section_prot = GAME_INFO_4_05_SECTION_PROT;
               break;
            case 455:
               process_name = GAME_INFO_4_55_PROCESS_NAME;
               section_name = GAME_INFO_4_55_SECTION_NAME;
               id_offset = GAME_INFO_4_55_ID_OFFSET;
               version_offset = GAME_INFO_4_55_VERSION_OFFSET;
               section_prot = GAME_INFO_4_55_SECTION_PROT;
               break;
            case 505:
               process_name = GAME_INFO_5_05_PROCESS_NAME;
               section_name = GAME_INFO_5_05_SECTION_NAME;
               id_offset = GAME_INFO_5_05_ID_OFFSET;
               version_offset = GAME_INFO_5_05_VERSION_OFFSET;
               section_prot = GAME_INFO_5_05_SECTION_PROT;
               break;
            default:
               break;
         }

         try {
            ProcessManager processManager = new ProcessManager();
            ProcessInfo processInfo = processManager.GetProcessInfo(process_name);

            MemoryHelper memoryHelper = new MemoryHelper(false, processInfo.pid);
            MappedSectionList mappedSectionList = processManager.MappedSectionList;
            mappedSectionList.InitMemorySectionList(processInfo);
            List<MappedSection> sectionList = mappedSectionList.GetMappedSectionList(section_name, section_prot);

            if (sectionList.Count != 1)
               return;

            GameID = System.Text.Encoding.Default.GetString(memoryHelper.ReadMemory(sectionList[0].Start + id_offset, 16));
            GameID = GameID.Trim(new Char[] { '\0' });
            Version = System.Text.Encoding.Default.GetString(memoryHelper.ReadMemory(sectionList[0].Start + version_offset, 16));
            Version = Version.Trim(new Char[] { '\0' });
         } catch {

         }
      }
   }

   class CONSTANT {
      public const UInt32 SAVE_FLAG_NONE = 0x0;
      public const UInt32 SAVE_FLAG_LOCK = 0x1;
      public const UInt32 SAVE_FLAG_MODIFED = 0x2;

      public const UInt32 SECTION_EXECUTABLE = 0x5;

      public static readonly UInt32 MAJOR_VERSION = (UInt32)System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Major;
      public static readonly UInt32 SECONDARY_VERSION = (UInt32)System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Minor;
      public static readonly UInt32 THIRD_VERSION = (UInt32)System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Build;
      
      public const Int32 MAX_PEEK_QUEUE = 4;
      public const Int32 PEEK_BUFFER_LENGTH = 32 * 1024 * 1024;
   }

   public class Util {
      public static Int32 DefaultProcessID = 0;
      public static Int32 SceProcessID = 0;
      public static Int32 Version = 0;
   }
}
