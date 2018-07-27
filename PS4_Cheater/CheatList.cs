using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PS4_Cheater {
   public enum CheatType {
      DATA_TYPE,
      SIMPLE_POINTER_TYPE,
      None,
   }

   public enum CheatOperatorType {
      DATA_TYPE,
      OFFSET_TYPE,
      ADDRESS_TYPE,
      SIMPLE_POINTER_TYPE,
      POINTER_TYPE,
      ARITHMETIC_TYPE,
   }

   public enum ToStringType {
      DATA_TYPE,
      ADDRESS_TYPE,
      ARITHMETIC_TYPE,
   }

   public class CheatOperator {
      public CheatOperator(MemoryHelper.ValueType valueType, ProcessManager processManager) {
         ProcessManager = processManager;
         this.valueType = valueType;
      }

      private MemoryHelper.ValueType _valueType;

      protected MemoryHelper MemoryHelper = new MemoryHelper(true, 0);

      public ProcessManager ProcessManager { get; set; }

      public MemoryHelper.ValueType valueType
      {
         get {
            return _valueType;
         }
         set {
            _valueType = value;
            MemoryHelper.InitMemoryHandler(valueType, MemoryHelper.CompareType.None, false);
         }
      }
      public CheatOperatorType CheatOperatorType { get; set; }

      public virtual Byte[] Get(Int32 idx = 0) { return null; }

      public virtual Byte[] GetRuntime() { return null; }

      public virtual void Set(CheatOperator SourceCheatOperator, Int32 idx = 0) { }

      public virtual void SetRuntime(CheatOperator SourceCheatOperator, Int32 idx = 0) { }

      public virtual Int32 GetSectionID() { return -1; }

      public virtual Boolean Parse(String[] cheat_elements, ref Int32 start_idx, Boolean simple_format) { return false; }

      public virtual String ToString(Boolean simple) { return null; }

      public virtual String Dump(Boolean simpleFormat) { return null; }

      public virtual String Display() { return null; }
   }

   public class DataCheatOperator : CheatOperator {
      private const Int32 DATA_TYPE = 0;
      private const Int32 DATA = 1;

      private Byte[] data;

      public DataCheatOperator(String data, MemoryHelper.ValueType valueType, ProcessManager processManager)
          : base(valueType, processManager) {
         this.data = MemoryHelper.StringToBytes(data);
         CheatOperatorType = CheatOperatorType.DATA_TYPE;
      }

      public DataCheatOperator(Byte[] data, MemoryHelper.ValueType valueType, ProcessManager processManager)
          : base(valueType, processManager) {
         this.data = data;
         CheatOperatorType = CheatOperatorType.DATA_TYPE;
      }

      public DataCheatOperator(ProcessManager processManager)
          : base(MemoryHelper.ValueType.None, processManager) {
         CheatOperatorType = CheatOperatorType.DATA_TYPE;
      }

      public override Byte[] Get(Int32 idx = 0) { return data; }

      public override Byte[] GetRuntime() { return data; }

      public override void Set(CheatOperator SourceCheatOperator, Int32 idx = 0) {
         data = new Byte[MemoryHelper.Length];
         Buffer.BlockCopy(SourceCheatOperator.Get(), 0, data, 0, MemoryHelper.Length);
      }

      public override void SetRuntime(CheatOperator SourceCheatOperator, Int32 idx = 0) {
         data = new Byte[MemoryHelper.Length];
         Buffer.BlockCopy(SourceCheatOperator.GetRuntime(), 0, data, 0, MemoryHelper.Length);
      }

      public void Set(String data) {
         this.data = MemoryHelper.StringToBytes(data);
      }

      public void Set(Byte[] data) {
         this.data = data;
      }

      public override String ToString(Boolean simple) {
         return MemoryHelper.BytesToString(data);
      }

      public override String Display() {
         return MemoryHelper.BytesToString(data);
      }

      public override Boolean Parse(String[] cheat_elements, ref Int32 start_idx, Boolean simple_format) {
         valueType = (MemoryHelper.ValueType)Enum.Parse(typeof(MemoryHelper.ValueType), cheat_elements[start_idx + DATA_TYPE]);
         data = MemoryHelper.StringToBytes(cheat_elements[start_idx + DATA]);
         start_idx += 2;
         return true;
      }

      public override String Dump(Boolean simpleFormat) {
         String save_buf = "";
         save_buf += valueType.ToString() + "|";
         save_buf += MemoryHelper.BytesToString(data) + "|";
         return save_buf;
      }
   }

   public class OffsetCheatOperator : CheatOperator {
      public Int64 Offset { get; set; }

      public OffsetCheatOperator(Int64 offset, MemoryHelper.ValueType valueType, ProcessManager processManager)
          : base(valueType, processManager) {
         this.Offset = offset;
         CheatOperatorType = CheatOperatorType.OFFSET_TYPE;
      }

      public OffsetCheatOperator(ProcessManager processManager)
          : base(MemoryHelper.ValueType.None, processManager) {
         CheatOperatorType = CheatOperatorType.OFFSET_TYPE;
      }

      public override Byte[] Get(Int32 idx = 0) { return BitConverter.GetBytes(Offset); }

      public override Byte[] GetRuntime() { return BitConverter.GetBytes(Offset); }

      public override void Set(CheatOperator SourceCheatOperator, Int32 idx = 0) {
         Offset = BitConverter.ToInt64(SourceCheatOperator.Get(), 0);
      }

      public override void SetRuntime(CheatOperator SourceCheatOperator, Int32 idx = 0) {
         Offset = BitConverter.ToInt64(SourceCheatOperator.Get(), 0);
      }

      public void Set(Int64 offset) {
         this.Offset = offset;
      }

      public override String ToString(Boolean simple) {
         return Offset.ToString("X16");
      }

      public override String Display() {
         return Offset.ToString("X16");
      }

      public override Boolean Parse(String[] cheat_elements, ref Int32 start_idx, Boolean simple_format) {
         Offset = Int64.Parse(cheat_elements[start_idx], NumberStyles.HexNumber);
         start_idx += 1;
         return true;
      }

      public override String Dump(Boolean simpleFormat) {
         String save_buf = "";
         save_buf += "+";
         save_buf += Offset.ToString("X");
         return save_buf;
      }
   }

   public class AddressCheatOperator : CheatOperator {
      private const Int32 SECTION_ID = 0;
      private const Int32 ADDRESS_OFFSET = 1;

      public UInt64 Address { get; set; }

      public AddressCheatOperator(UInt64 Address, ProcessManager processManager)
          : base(MemoryHelper.ValueType.UInt64, processManager) {
         this.Address = Address;
         CheatOperatorType = CheatOperatorType.ADDRESS_TYPE;
      }

      public AddressCheatOperator(ProcessManager processManager)
          : base(MemoryHelper.ValueType.UInt64, processManager) {
         CheatOperatorType = CheatOperatorType.ADDRESS_TYPE;
      }

      public override Byte[] Get(Int32 idx = 0) {
         return BitConverter.GetBytes(Address);
      }

      public override Byte[] GetRuntime() {
         return MemoryHelper.ReadMemory(Address, MemoryHelper.Length);
      }

      public override Int32 GetSectionID() {
         return ProcessManager.MappedSectionList.GetMappedSectionID(Address);
      }

      public override void Set(CheatOperator SourceCheatOperator, Int32 idx = 0) {
         Address = BitConverter.ToUInt64(SourceCheatOperator.Get(), 0);
      }

      public override void SetRuntime(CheatOperator SourceCheatOperator, Int32 idx = 0) {
         MemoryHelper.WriteMemory(Address, SourceCheatOperator.GetRuntime());
      }

      public String DumpOldFormat() {
         String save_buf = "";

         Int32 sectionID = ProcessManager.MappedSectionList.GetMappedSectionID(Address);
         MappedSection mappedSection = ProcessManager.MappedSectionList[sectionID];
         save_buf += sectionID + "|";
         save_buf += String.Format("{0:X}", Address - mappedSection.Start) + "|";
         return save_buf;
      }

      public override String Dump(Boolean simpleFormat) {
         String save_buf = "";

         Int32 sectionID = ProcessManager.MappedSectionList.GetMappedSectionID(Address);
         MappedSection mappedSection = ProcessManager.MappedSectionList[sectionID];
         save_buf += String.Format("@{0:X}", Address) + "_";
         save_buf += sectionID + "_";
         save_buf += String.Format("{0:X}", Address - mappedSection.Start);
         return save_buf;
      }

      public override Boolean Parse(String[] cheat_elements, ref Int32 start_idx, Boolean simple_format) {
         if (simple_format) {
            String address = cheat_elements[start_idx++];
            String[] address_elements = address.Split('_');

            Int32 sectionID = int.Parse(address_elements[1]);
            if (sectionID >= ProcessManager.MappedSectionList.Count || sectionID < 0) {
               return false;
            }

            UInt64 addressOffset = ulong.Parse(address_elements[2], NumberStyles.HexNumber);

            Address = addressOffset + ProcessManager.MappedSectionList[sectionID].Start;
         }
         return false;
      }

      public Boolean ParseOldFormat(String[] cheat_elements, ref Int32 start_idx) {
         Int32 sectionID = int.Parse(cheat_elements[start_idx + SECTION_ID]);
         if (sectionID >= ProcessManager.MappedSectionList.Count || sectionID < 0) {
            return false;
         }

         UInt64 addressOffset = ulong.Parse(cheat_elements[start_idx + ADDRESS_OFFSET], NumberStyles.HexNumber);

         Address = addressOffset + ProcessManager.MappedSectionList[sectionID].Start;

         start_idx += 2;
         return true;
      }

      public override String Display() {
         return Address.ToString("X");
      }

      public override String ToString() {
         return Address.ToString("X");
      }
   }

   public class SimplePointerCheatOperator : CheatOperator {
      private AddressCheatOperator Address { get; set; }
      private List<OffsetCheatOperator> Offsets { get; set; }

      public SimplePointerCheatOperator(AddressCheatOperator Address, List<OffsetCheatOperator> Offsets, MemoryHelper.ValueType valueType, ProcessManager processManager)
          : base(valueType, processManager) {
         this.Address = Address;
         this.Offsets = Offsets;
         CheatOperatorType = CheatOperatorType.SIMPLE_POINTER_TYPE;
      }

      public SimplePointerCheatOperator(ProcessManager processManager)
          : base(MemoryHelper.ValueType.None, processManager) {
         Address = new AddressCheatOperator(ProcessManager);
         Offsets = new List<OffsetCheatOperator>();

         CheatOperatorType = CheatOperatorType.SIMPLE_POINTER_TYPE;
      }

      public override Byte[] Get(Int32 idx = 0) {
         return Address.Get();
      }

      public override Byte[] GetRuntime() {
         return MemoryHelper.ReadMemory(GetAddress(), MemoryHelper.Length);
      }

      public override Int32 GetSectionID() {
         return ProcessManager.MappedSectionList.GetMappedSectionID(GetAddress());
      }

      public override void Set(CheatOperator SourceCheatOperator, Int32 idx = 0) {
         throw new Exception("Pointer Set!!");
      }

      private UInt64 GetAddress() {
         UInt64 address = BitConverter.ToUInt64(Address.GetRuntime(), 0);
         Int32 i = 0;
         for (; i < Offsets.Count - 1; ++i) {
            Byte[] new_address = MemoryHelper.ReadMemory((UInt64)((Int64)address + Offsets[i].Offset), 8);
            address = BitConverter.ToUInt64(new_address, 0);
         }

         if (i < Offsets.Count) {
            address += (UInt64)Offsets[i].Offset;
         }

         return address;
      }

      public override void SetRuntime(CheatOperator SourceCheatOperator, Int32 idx = 0) {
         Byte[] buf = new Byte[MemoryHelper.Length];
         Buffer.BlockCopy(SourceCheatOperator.GetRuntime(), 0, buf, 0, MemoryHelper.Length);

         MemoryHelper.WriteMemory(GetAddress(), buf);
      }

      public override Boolean Parse(String[] cheat_elements, ref Int32 start_idx, Boolean simple_format) {
         valueType = (MemoryHelper.ValueType)Enum.Parse(typeof(MemoryHelper.ValueType), cheat_elements[start_idx + 0]);
         String pointer_str = cheat_elements[start_idx + 1];
         Int32 pointer_idx = 0;
         String[] pointer_list = pointer_str.Split('+');

         Address.Parse(pointer_list, ref pointer_idx, simple_format);

         for (Int32 i = 1; i < pointer_list.Length; ++i) {
            OffsetCheatOperator offset = new OffsetCheatOperator(ProcessManager);
            offset.Parse(pointer_list, ref pointer_idx, simple_format);
            Offsets.Add(offset);
         }

         start_idx += 2;

         return true;
      }

      public override String Display() {
         return "p->" + GetAddress().ToString("X");
      }

      public override String Dump(Boolean simpleFormat) {
         String dump_buf = "";

         dump_buf += valueType.ToString() + "|";
         dump_buf += Address.Dump(simpleFormat);
         for (Int32 i = 0; i < Offsets.Count; ++i) {
            dump_buf += Offsets[i].Dump(simpleFormat);
         }
         return dump_buf;
      }
   }

   public enum ArithmeticType {
      ADD_TYPE,
      SUB_TYPE,
      MUL_TYPE,
      DIV_TYPE,
   }

   public class BinaryArithmeticCheatOperator : CheatOperator {
      public CheatOperator Left { get; set; }
      public CheatOperator Right { get; set; }

      private ArithmeticType ArithmeticType { get; set; }

      public BinaryArithmeticCheatOperator(CheatOperator left, CheatOperator right, ArithmeticType ArithmeticType,
          ProcessManager processManager)
          : base(left.valueType, processManager) {
         Left = left;
         Right = right;
         this.ArithmeticType = ArithmeticType;
         CheatOperatorType = CheatOperatorType.ARITHMETIC_TYPE;
      }

      public override Byte[] Get(Int32 idx) {
         if (idx == 0) return Left.Get();
         return Right.Get();
      }

      public Byte[] GetRuntime(Int32 idx) {
         Byte[] left_buf = new Byte[MemoryHelper.Length];
         Buffer.BlockCopy(Left.Get(), 0, left_buf, 0, MemoryHelper.Length);
         Byte[] right_buf = new Byte[MemoryHelper.Length];
         Buffer.BlockCopy(Right.Get(), 0, right_buf, 0, MemoryHelper.Length);
         UInt64 left = BitConverter.ToUInt64(left_buf, 0);
         UInt64 right = BitConverter.ToUInt64(right_buf, 0);
         UInt64 result = 0;

         switch (ArithmeticType) {
            case ArithmeticType.ADD_TYPE:
               result = left + right;
               break;
            case ArithmeticType.SUB_TYPE:
               result = left - right;
               break;
            case ArithmeticType.MUL_TYPE:
               result = left * right;
               break;
            case ArithmeticType.DIV_TYPE:
               result = left / right;
               break;
            default:
               throw new Exception("ArithmeticType!!!");
         }
         return MemoryHelper.StringToBytes(result.ToString());
      }

      public override void Set(CheatOperator SourceCheatOperator, Int32 idx = 0) {
         throw new Exception("Set BinaryArithmeticCheatOperator");
      }

      public override void SetRuntime(CheatOperator SourceCheatOperator, Int32 idx = 0) {
         throw new Exception("SetRuntime BinaryArithmeticCheatOperator");
      }

      public override Boolean Parse(String[] cheat_elements, ref Int32 start_idx, Boolean simple_format) {
         if (Left.Parse(cheat_elements, ref start_idx, simple_format)) {
            return false;
         }

         switch (cheat_elements[start_idx]) {
            case "+":
               ArithmeticType = ArithmeticType.ADD_TYPE;
               break;
            case "-":
               ArithmeticType = ArithmeticType.SUB_TYPE;
               break;
            case "*":
               ArithmeticType = ArithmeticType.MUL_TYPE;
               break;
            case "/":
               ArithmeticType = ArithmeticType.DIV_TYPE;
               break;
            default:
               throw new Exception("ArithmeticType parse!!!");
         }
         ++start_idx;

         if (Right.Parse(cheat_elements, ref start_idx, simple_format)) {
            return false;
         }

         return true;
      }

      public override String Display() {
         return "";
      }

      public override String Dump(Boolean simpleFormat) {
         return Left.Dump(simpleFormat) + Right.Dump(simpleFormat);
      }
   }

   public class Cheat {

      public CheatType CheatType { get; set; }

      protected ProcessManager ProcessManager;

      public String Description { get; set; }

      public Boolean Lock { get; set; }

      public Boolean AllowLock { get; set; }

      public virtual Boolean Parse(String[] cheat_elements) {
         return false;
      }

      public Cheat(ProcessManager ProcessManager) {
         this.ProcessManager = ProcessManager;
      }

      protected CheatOperator Source { get; set; }
      protected CheatOperator Destination { get; set; }

      public CheatOperator GetSource() {
         return Source;
      }

      public CheatOperator GetDestination() {
         return Destination;
      }
   }

   public class DataCheat : Cheat {
      private const Int32 CHEAT_CODE_DATA_TYPE_FLAG = 5;
      private const Int32 CHEAT_CODE_DATA_TYPE_DESCRIPTION = 6;

      private const Int32 CHEAT_CODE_DATA_TYPE_ELEMENT_COUNT = CHEAT_CODE_DATA_TYPE_DESCRIPTION + 1;

      public DataCheat(DataCheatOperator source, AddressCheatOperator dest, Boolean lock_, String description, ProcessManager processManager)
          : base(processManager) {
         CheatType = CheatType.DATA_TYPE;
         AllowLock = true;
         Source = source;
         Destination = dest;
         Lock = lock_;
         Description = description;
      }

      public DataCheat(ProcessManager ProcessManager) :
          base(ProcessManager) {
         Source = new DataCheatOperator(ProcessManager);
         Destination = new AddressCheatOperator(ProcessManager);
         CheatType = CheatType.DATA_TYPE;
         AllowLock = true;
      }

      public override Boolean Parse(String[] cheat_elements) {
         if (cheat_elements.Length < CHEAT_CODE_DATA_TYPE_ELEMENT_COUNT) {
            return false;
         }

         Int32 start_idx = 1;
         AddressCheatOperator addressCheatOperator = (AddressCheatOperator)Destination;
         if (!(addressCheatOperator.ParseOldFormat(cheat_elements, ref start_idx))) {
            return false;
         }

         if (!Source.Parse(cheat_elements, ref start_idx, true)) {
            return false;
         }

         UInt64 flag = ulong.Parse(cheat_elements[CHEAT_CODE_DATA_TYPE_FLAG], NumberStyles.HexNumber);

         Lock = flag == 1 ? true : false;

         Description = cheat_elements[CHEAT_CODE_DATA_TYPE_DESCRIPTION];

         Destination.valueType = Source.valueType;

         return true;
      }

      public override String ToString() {
         String save_buf = "";
         save_buf += "data|";
         save_buf += ((AddressCheatOperator)Destination).DumpOldFormat();
         save_buf += Source.Dump(true);
         save_buf += (Lock ? "1" : "0") + "|";
         save_buf += Description + "|";
         save_buf += Destination.ToString() + "\n";
         return save_buf;
      }
   }


   public class SimplePointerCheat : Cheat {
      public SimplePointerCheat(ProcessManager ProcessManager)
          : base(ProcessManager) {
         CheatType = CheatType.SIMPLE_POINTER_TYPE;
         AllowLock = true;
      }

      public SimplePointerCheat(CheatOperator source, CheatOperator dest, Boolean lock_, String description, ProcessManager processManager)
          : base(processManager) {
         CheatType = CheatType.DATA_TYPE;
         AllowLock = true;
         Source = source;
         Destination = dest;
         Lock = lock_;
         Description = description;
      }

      public override Boolean Parse(String[] cheat_elements) {
         Int32 start_idx = 1;

         if (cheat_elements[start_idx] == "address") {
            Destination = new AddressCheatOperator(ProcessManager);
         } else if (cheat_elements[start_idx] == "pointer") {
            Destination = new SimplePointerCheatOperator(ProcessManager);
         }

         ++start_idx;
         Destination.Parse(cheat_elements, ref start_idx, true);

         if (cheat_elements[start_idx] == "data") {
            Source = new DataCheatOperator(ProcessManager);
         } else if (cheat_elements[start_idx] == "pointer") {
            Source = new SimplePointerCheatOperator(ProcessManager);
         }

         ++start_idx;
         Source.Parse(cheat_elements, ref start_idx, true);

         UInt64 flag = ulong.Parse(cheat_elements[start_idx], NumberStyles.HexNumber);

         Lock = flag == 1 ? true : false;

         Description = cheat_elements[start_idx + 1];

         return true;
      }

      public override String ToString() {
         String save_buf = "";
         save_buf += "simple pointer|";
         save_buf += "pointer|";
         save_buf += Destination.Dump(true) + "|";
         save_buf += "data|";
         save_buf += Source.Dump(true);
         save_buf += (Lock ? "1" : "0") + "|";
         save_buf += Description + "|";
         save_buf += "\n";
         return save_buf;
      }
   }

   class CheatList {
      private List<Cheat> cheat_list;

      private const Int32 CHEAT_CODE_HEADER_VERSION = 0;
      private const Int32 CHEAT_CODE_HEADER_PROCESS_NAME = 1;
      private const Int32 CHEAT_CODE_HEADER_PROCESS_ID   = 2;
      private const Int32 CHEAT_CODE_HEADER_PROCESS_VER  = 3;

      private const Int32 CHEAT_CODE_HEADER_ELEMENT_COUNT = CHEAT_CODE_HEADER_PROCESS_NAME + 1;

      private const Int32 CHEAT_CODE_TYPE = 0;
      public CheatList() {
         cheat_list = new List<Cheat>();
      }

      public void Add(Cheat cheat) {
         cheat_list.Add(cheat);
      }

      public void RemoveAt(Int32 idx) {
         cheat_list.RemoveAt(idx);
      }

      public Boolean Exist(Cheat cheat) {
         return false;
      }

      public Boolean Exist(UInt64 destAddress) {
         return false;
      }

      public Boolean LoadFile(String path, ProcessManager processManager, ComboBox comboBox) {
         String[] cheats = File.ReadAllLines(path);

         if (cheats.Length < 2) {
            return false;
         }

         String header = cheats[0];
         String[] header_items = header.Split('|');

         if (header_items.Length < CHEAT_CODE_HEADER_ELEMENT_COUNT) {
            return false;
         }

         String[] version = (header_items[CHEAT_CODE_HEADER_VERSION]).Split('.');

         UInt64 major_version = 0;
         UInt64 secondary_version = 0;

         ulong.TryParse(version[0], out major_version);
         if (version.Length > 1) {
            ulong.TryParse(version[1], out secondary_version);
         }

         if (major_version > CONSTANT.MAJOR_VERSION || (major_version == CONSTANT.MAJOR_VERSION && secondary_version > CONSTANT.SECONDARY_VERSION)) {
            return false;
         }

         String process_name = header_items[CHEAT_CODE_HEADER_PROCESS_NAME];
         if (process_name != (String)comboBox.SelectedItem) {
            comboBox.SelectedItem = process_name;
         }

         if (process_name != (String)comboBox.SelectedItem) {
            MessageBox.Show("Invalid process or refresh processes first.");
            return false;
         }

         String game_id = "";
         String game_ver = "";

         if (header_items.Length > CHEAT_CODE_HEADER_PROCESS_ID) {
            game_id = header_items[CHEAT_CODE_HEADER_PROCESS_ID];
            game_id = game_id.Substring(3);
         }

         if (header_items.Length > CHEAT_CODE_HEADER_PROCESS_VER) {
            game_ver = header_items[CHEAT_CODE_HEADER_PROCESS_VER];
            game_ver = game_ver.Substring(4);
         }

         if (game_id != "" && game_ver != "") {
            GameInfo gameInfo = new GameInfo();
            if (gameInfo.GameID != game_id) {
               if (MessageBox.Show("Your Game ID(" + gameInfo.GameID + ") is different with cheat file(" + game_id + "), still load?",
                   "Invalid game ID", MessageBoxButtons.YesNo) != DialogResult.Yes) {
                  return false;
               }
            }

            if (gameInfo.Version != game_ver) {
               if (MessageBox.Show("Your game version(" + gameInfo.Version + ") is different with cheat file(" + game_ver + "), still load?",
                   "Invalid game version", MessageBoxButtons.YesNo) != DialogResult.Yes) {
                  return false;
               }
            }
         }

         for (Int32 i = 1; i < cheats.Length; ++i) {
            String cheat_tuple = cheats[i];
            String[] cheat_elements = cheat_tuple.Split(new String[] { "|" }, StringSplitOptions.None);

            if (cheat_elements.Length == 0) {
               continue;
            }

            if (cheat_elements[CHEAT_CODE_TYPE] == "data") {
               DataCheat cheat = new DataCheat(processManager);
               if (!cheat.Parse(cheat_elements)) {
                  MessageBox.Show("Invaid cheat code:" + cheat_tuple);
                  continue;
               }

               cheat_list.Add(cheat);
            } else if (cheat_elements[CHEAT_CODE_TYPE] == "simple pointer") {

               SimplePointerCheat cheat = new SimplePointerCheat(processManager);
               if (!cheat.Parse(cheat_elements))
                  continue;
               cheat_list.Add(cheat);
            } else {
               MessageBox.Show("Invaid cheat code:" + cheat_tuple);
               continue;
            }
         }
         return true;
      }

      public void SaveFile(String path, String prcessName, ProcessManager processManager) {
         GameInfo gameInfo = new GameInfo();
         String save_buf = CONSTANT.MAJOR_VERSION + "."
                + CONSTANT.SECONDARY_VERSION
                + "|" + prcessName
                + "|ID:" + gameInfo.GameID
                + "|VER:" + gameInfo.Version
                + "|FM:" + Util.Version
                + "\n";

         for (Int32 i = 0; i < cheat_list.Count; ++i) {
            save_buf += cheat_list[i].ToString();
         }

         StreamWriter myStream = new StreamWriter(path);
         myStream.Write(save_buf);
         myStream.Close();
      }

      public Cheat this[Int32 index]
      {
         get {
            return cheat_list[index];
         }
         set {
            cheat_list[index] = value;
         }
      }

      public void Clear() {
         cheat_list.Clear();
      }

      public Int32 Count { get { return cheat_list.Count; } }
   }
}
