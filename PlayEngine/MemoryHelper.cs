using System;
using System.Globalization;
using System.Threading;

using librpc;

namespace PlayEngine {
   public class MemoryHelper {
      public enum CompareType {
         None,
         ExactValue,
         FuzzyValue,
         IncreasedValue,
         IncreasedValueBy,
         DecreasedValue,
         DecreasedValueBy,
         BiggerThan,
         SmallerThan,
         ChangedValue,
         UnchangedValue,
         BetweenValues,
         UnknownInitialValue
      }
      public enum ValueType {
         None,
         UInt8,
         UInt16,
         UInt32,
         UInt64,
         Float,
         Double,
         String,
         ArrayOfBytes
      }

      public static PS4RPC ps4 = null;
      private static Mutex mutex;
      private Int32 SelfProcessID;
      private Int32 ProcessID
      {
         get {
            if (DefaultProcessID)
               return Util.DefaultProcessID;
            return SelfProcessID;
         }
      }
      private Boolean DefaultProcessID;

      static MemoryHelper() {
         mutex = new Mutex();
      }

      public MemoryHelper(Boolean defaultProcessID, Int32 processID) {
         this.SelfProcessID = processID;
         this.DefaultProcessID = defaultProcessID;
      }

      public static Boolean Connect(String ip) {
         try {
            mutex.WaitOne();
            if (ps4 != null)
               ps4.Disconnect();
            ps4 = new PS4RPC(ip);
            ps4.Connect();
            mutex.ReleaseMutex();
            return true;
         } catch {
            mutex.ReleaseMutex();
         }
         return false;
      }

      public static Boolean Disconnect() {

         try {
            mutex.WaitOne();
            if (ps4 != null) {
               ps4.Disconnect();
            }
            mutex.ReleaseMutex();
            return true;
         } catch {
            mutex.ReleaseMutex();
         }
         return false;
      }

      public delegate String BytesToStringHandler(Byte[] value);
      public delegate Byte[] StringToBytesHandler(String value);
      public delegate Boolean ComparatorHandler(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value);

      public BytesToStringHandler BytesToString { get; set; }
      public BytesToStringHandler BytesToHexString { get; set; }
      public StringToBytesHandler StringToBytes { get; set; }
      public StringToBytesHandler HexStringToBytes { get; set; }
      public ComparatorHandler Comparer { get; set; }

      public Int32 Length { get; set; }
      public Int32 Alignment { get; set; }

      public ValueType valueType;
      public CompareType compareType;

      public Boolean ParseFirstValue { get; set; }
      public Boolean ParseSecondValue { get; set; }

      public Byte[] ReadMemory(UInt64 address, Int32 length) {
         mutex.WaitOne();
         try {
            Byte[] buf = ps4.ReadMemory(ProcessID, address, length);
            mutex.ReleaseMutex();
            return buf;
         } catch {
            mutex.ReleaseMutex();
         }
         return new Byte[length];
      }

      public void WriteMemory(UInt64 address, Byte[] data) {
         mutex.WaitOne();
         try {
            ps4.WriteMemory(ProcessID, address, data);
            mutex.ReleaseMutex();
         } catch {
            mutex.ReleaseMutex();
         }
      }

      public static ProcessList GetProcessList() {
         mutex.WaitOne();
         try {
            ProcessList processList = ps4.GetProcessList();
            mutex.ReleaseMutex();
            return processList;
         } catch {
            mutex.ReleaseMutex();
         }
         return null;
      }

      public static ProcessInfo GetProcessInfo(Int32 processID) {
         mutex.WaitOne();
         try {
            ProcessInfo processInfo = ps4.GetProcessInfo(processID);
            mutex.ReleaseMutex();
            return processInfo;
         } catch {
            mutex.ReleaseMutex();
            return null;
         }
      }

      static String hex_to_string(Byte[] bytes) {
         return BitConverter.ToString(bytes).Replace("-", "");
      }

      static String string_to_string(Byte[] value) {
         return System.Text.Encoding.Default.GetString(value);
      }

      static String double_to_string(Byte[] value) {
         return BitConverter.ToDouble(value, 0).ToString();
      }
      static String float_to_string(Byte[] value) {
         return BitConverter.ToSingle(value, 0).ToString();
      }
      static String ulong_to_string(Byte[] value) {
         return BitConverter.ToUInt64(value, 0).ToString();
      }
      static String uint_to_string(Byte[] value) {
         return BitConverter.ToUInt32(value, 0).ToString();
      }
      static String uint16_to_string(Byte[] value) {
         return BitConverter.ToUInt16(value, 0).ToString();
      }
      static String uchar_to_string(Byte[] value) {
         return value[0].ToString();
      }
      static String double_to_hex_string(Byte[] value) {
         return BitConverter.ToUInt64(value, 0).ToString("X16");
      }
      static String float_to_hex_string(Byte[] value) {
         return BitConverter.ToUInt32(value, 0).ToString("X8");
      }
      static String ulong_to_hex_string(Byte[] value) {
         return BitConverter.ToUInt64(value, 0).ToString("X16");
      }
      static String uint_to_hex_string(Byte[] value) {
         return BitConverter.ToUInt32(value, 0).ToString("X8");
      }
      static String uint16_to_hex_string(Byte[] value) {
         return BitConverter.ToUInt16(value, 0).ToString("X4");
      }
      static String uchar_to_hex_string(Byte[] value) {
         return value[0].ToString("X2");
      }
      static String hex_to_hex_string(Byte[] bytes) {
         return BitConverter.ToString(bytes).Replace("-", "");
      }

      static String string_to_hex_string(Byte[] value) {
         return BitConverter.ToString(value).Replace("-", "");
      }

      static Byte[] string_to_double(String value) {
         return BitConverter.GetBytes(double.Parse(value));
      }
      static Byte[] string_to_float(String value) {
         return BitConverter.GetBytes(float.Parse(value));
      }
      static Byte[] string_to_8_bytes(String value) {
         return BitConverter.GetBytes(ulong.Parse(value));
      }

      static Byte[] string_to_4_bytes(String value) {
         return BitConverter.GetBytes(uint.Parse(value));
      }

      static Byte[] string_to_2_bytes(String value) {
         return BitConverter.GetBytes(UInt16.Parse(value));
      }
      static Byte[] string_to_byte(String value) {
         return BitConverter.GetBytes(Byte.Parse(value));
      }
      public static Byte[] string_to_hex_bytes(String hexString) {
         Byte[] returnBytes = new Byte[hexString.Length / 2];
         for (Int32 i = 0; i < returnBytes.Length; i++)
            returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
         return returnBytes;
      }

      static Byte[] string_to_string_bytes(String hexString) {
         Byte[] buffer = System.Text.Encoding.Default.GetBytes(hexString);
         return buffer;
      }

      static Byte[] hex_string_to_double(String value) {
         return BitConverter.GetBytes(double.Parse(value, NumberStyles.HexNumber));
      }
      static Byte[] hex_string_to_float(String value) {
         return BitConverter.GetBytes(float.Parse(value, NumberStyles.HexNumber));
      }
      static Byte[] hex_string_to_8_bytes(String value) {
         return BitConverter.GetBytes(ulong.Parse(value, NumberStyles.HexNumber));
      }

      static Byte[] hex_string_to_4_bytes(String value) {
         return BitConverter.GetBytes(uint.Parse(value, NumberStyles.HexNumber));
      }

      static Byte[] hex_string_to_2_bytes(String value) {
         return BitConverter.GetBytes(UInt16.Parse(value, NumberStyles.HexNumber));
      }
      static Byte[] hex_string_to_byte(String value) {
         return BitConverter.GetBytes(Byte.Parse(value, NumberStyles.HexNumber));
      }

      public Byte[] GetBytesFromAddress(UInt64 address) {
         return ReadMemory(address, Length);
      }

      public void SetBytesByType(UInt64 address, Byte[] data) {
         WriteMemory(address, data);
      }

      public void CompareWithMemoryBufferNextScanner(Byte[] default_value_0, Byte[] default_value_1, Byte[] buffer,
          ResultList old_result_list, ResultList new_result_list) {
         Int32 length = Length;

         Byte[] new_value = new Byte[length];
         for (old_result_list.Begin(); !old_result_list.End(); old_result_list.Next()) {
            UInt32 address_offset = 0;
            Byte[] old_value = null;
            old_result_list.Get(ref address_offset, ref old_value);
            Buffer.BlockCopy(buffer, (Int32)address_offset, new_value, 0, length);
            if (Comparer(default_value_0, default_value_1, old_value, new_value)) {
               new_result_list.Add(address_offset, new_value);
            }
         }
      }

      public void CompareWithMemoryBufferNewScanner(Byte[] default_value_0, Byte[] default_value_1, Byte[] buffer,
          ResultList new_result_list, UInt32 base_address) {
         Int32 alignment = Alignment;
         Int32 length = Length;

         Byte[] new_value = new Byte[length];
         Byte[] dummy_value = new Byte[length];
         for (Int32 i = 0; i + length < buffer.LongLength; i += alignment) {
            Buffer.BlockCopy(buffer, i, new_value, 0, length);
            if (Comparer(default_value_0, default_value_1, dummy_value, new_value)) {
               new_result_list.Add((UInt32)i + base_address, new_value);
            }
         }
      }

      public void CompareWithMemoryBufferPointerScanner(ProcessManager processManager, Byte[] buffer,
          PointerList pointerList, UInt64 base_address) {
         Byte[] address_buf = new Byte[8];
         for (Int32 i = 0; i + 8 < buffer.LongLength; i += 8) {
            Buffer.BlockCopy(buffer, i, address_buf, 0, 8);
            UInt64 address = BitConverter.ToUInt64(address_buf, 0);
            Int32 sectionID = processManager.MappedSectionList.GetMappedSectionID(address);
            if (sectionID != -1) {
               Pointer pointer = new Pointer(base_address + (UInt64)i, address);
               pointerList.Add(pointer);
            }
         }
      }

      public Boolean scan_type_equal_hex(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         if (default_value_0.Length != new_value.Length) {
            throw new ArgumentException("Length!!!");
         }

         for (Int32 i = 0; i < default_value_0.Length; ++i) {
            if (default_value_0[i] != new_value[i]) {
               return false;
            }
         }

         return true;
      }

      Boolean scan_type_equal_string(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         if (default_value_0.Length != new_value.Length) {
            throw new ArgumentException("Length!!!");
         }

         for (Int32 i = 0; i < default_value_0.Length; ++i) {
            if (default_value_0[i] != new_value[i]) {
               return false;
            }
         }
         return true;
      }

      Boolean scan_type_pointer_ulong(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToUInt64(new_value, 0) != 0 ? true : false;
      }

      Boolean scan_type_any_ulong(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToUInt64(new_value, 0) != 0 ? true : false;
      }

      Boolean scan_type_between_ulong(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToUInt64(new_value, 0) <= BitConverter.ToUInt64(default_value_1, 0) &&
             BitConverter.ToUInt64(new_value, 0) >= BitConverter.ToUInt64(default_value_0, 0);
      }

      Boolean scan_type_increased_ulong(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToUInt64(new_value, 0) > BitConverter.ToUInt64(old_value, 0);
      }
      Boolean scan_type_increased_by_ulong(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToUInt64(new_value, 0) ==
             BitConverter.ToUInt64(old_value, 0) + BitConverter.ToUInt64(default_value_0, 0);
      }
      Boolean scan_type_bigger_ulong(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToUInt64(new_value, 0) > BitConverter.ToUInt64(default_value_0, 0);
      }
      Boolean scan_type_decreased_ulong(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToUInt64(new_value, 0) < BitConverter.ToUInt64(old_value, 0);
      }

      Boolean scan_type_decreased_by_ulong(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToUInt64(new_value, 0) ==
             BitConverter.ToUInt64(old_value, 0) - BitConverter.ToUInt64(default_value_0, 0);
      }

      Boolean scan_type_less_ulong(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToUInt64(new_value, 0) < BitConverter.ToUInt64(default_value_0, 0);
      }
      Boolean scan_type_unchanged_ulong(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToUInt64(old_value, 0) == BitConverter.ToUInt64(new_value, 0);
      }
      Boolean scan_type_equal_ulong(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToUInt64(default_value_0, 0) == BitConverter.ToUInt64(new_value, 0);
      }
      Boolean scan_type_changed_ulong(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToUInt64(old_value, 0) != BitConverter.ToUInt64(new_value, 0);
      }
      Boolean scan_type_not_ulong(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToUInt64(default_value_0, 0) != BitConverter.ToUInt64(new_value, 0);
      }


      Boolean scan_type_any_uint(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToUInt32(new_value, 0) != 0 ? true : false;
      }

      Boolean scan_type_between_uint(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToUInt32(new_value, 0) <= BitConverter.ToUInt32(default_value_1, 0) &&
             BitConverter.ToUInt32(new_value, 0) >= BitConverter.ToUInt32(default_value_0, 0);
      }

      Boolean scan_type_increased_uint(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToUInt32(new_value, 0) > BitConverter.ToUInt32(old_value, 0);
      }
      Boolean scan_type_increased_by_uint(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToUInt32(new_value, 0) ==
             BitConverter.ToUInt32(old_value, 0) + BitConverter.ToUInt32(default_value_0, 0);
      }
      Boolean scan_type_bigger_uint(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToUInt32(new_value, 0) > BitConverter.ToUInt32(default_value_0, 0);
      }
      Boolean scan_type_decreased_uint(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToUInt32(new_value, 0) < BitConverter.ToUInt32(old_value, 0);
      }

      Boolean scan_type_decreased_by_uint(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToUInt32(new_value, 0) ==
             BitConverter.ToUInt32(old_value, 0) - BitConverter.ToUInt32(default_value_0, 0);
      }

      Boolean scan_type_less_uint(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToUInt32(new_value, 0) < BitConverter.ToUInt32(default_value_0, 0);
      }
      Boolean scan_type_unchanged_uint(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToUInt32(old_value, 0) == BitConverter.ToUInt32(new_value, 0);
      }
      Boolean scan_type_equal_uint(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToUInt32(default_value_0, 0) == BitConverter.ToUInt32(new_value, 0);
      }
      Boolean scan_type_changed_uint(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToUInt32(old_value, 0) != BitConverter.ToUInt32(new_value, 0);
      }
      Boolean scan_type_not_uint(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToUInt32(default_value_0, 0) != BitConverter.ToUInt32(new_value, 0);
      }

      Boolean scan_type_any_uint16(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToUInt16(new_value, 0) != 0 ? true : false;
      }

      Boolean scan_type_between_uint16(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToUInt16(new_value, 0) <= BitConverter.ToUInt16(default_value_1, 0) &&
             BitConverter.ToUInt16(new_value, 0) >= BitConverter.ToUInt16(default_value_0, 0);
      }

      Boolean scan_type_increased_uint16(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToUInt16(new_value, 0) > BitConverter.ToUInt16(old_value, 0);
      }
      Boolean scan_type_increased_by_uint16(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToUInt16(new_value, 0) ==
             BitConverter.ToUInt16(old_value, 0) + BitConverter.ToUInt16(default_value_0, 0);
      }
      Boolean scan_type_bigger_uint16(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToUInt16(new_value, 0) > BitConverter.ToUInt16(default_value_0, 0);
      }
      Boolean scan_type_decreased_uint16(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToUInt16(new_value, 0) < BitConverter.ToUInt16(old_value, 0);
      }

      Boolean scan_type_decreased_by_uint16(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToUInt16(new_value, 0) ==
             BitConverter.ToUInt16(old_value, 0) - BitConverter.ToUInt16(default_value_0, 0);
      }

      Boolean scan_type_less_uint16(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToUInt16(new_value, 0) < BitConverter.ToUInt16(default_value_0, 0);
      }
      Boolean scan_type_unchanged_uint16(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToUInt16(old_value, 0) == BitConverter.ToUInt16(new_value, 0);
      }
      Boolean scan_type_equal_uint16(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToUInt16(default_value_0, 0) == BitConverter.ToUInt16(new_value, 0);
      }
      Boolean scan_type_changed_uint16(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToUInt16(old_value, 0) != BitConverter.ToUInt16(new_value, 0);
      }
      Boolean scan_type_not_uint16(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToUInt16(default_value_0, 0) != BitConverter.ToUInt16(new_value, 0);
      }

      Boolean scan_type_any_uint8(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return new_value[0] != 0 ? true : false;
      }

      Boolean scan_type_between_uint8(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return new_value[0] <= default_value_1[0] &&
             new_value[0] >= default_value_0[0];
      }

      Boolean scan_type_increased_uint8(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return new_value[0] > old_value[0];
      }
      Boolean scan_type_increased_by_uint8(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return new_value[0] ==
             old_value[0] + default_value_0[0];
      }
      Boolean scan_type_bigger_uint8(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return new_value[0] > default_value_0[0];
      }
      Boolean scan_type_decreased_uint8(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return new_value[0] < old_value[0];
      }

      Boolean scan_type_decreased_by_uint8(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return new_value[0] ==
             old_value[0] - default_value_0[0];
      }

      Boolean scan_type_less_uint8(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return new_value[0] < default_value_0[0];
      }
      Boolean scan_type_unchanged_uint8(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return old_value[0] == new_value[0];
      }
      Boolean scan_type_equal_uint8(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return default_value_0[0] == new_value[0];
      }
      Boolean scan_type_changed_uint8(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return old_value[0] != new_value[0];
      }

      Boolean scan_type_not_uint8(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return default_value_0[0] != new_value[0];
      }
      Boolean scan_type_any_double(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToDouble(new_value, 0) != 0 ? true : false;
      }
      Boolean scan_type_fuzzy_equal_double(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return Math.Abs(BitConverter.ToDouble(default_value_0, 0) -
             BitConverter.ToDouble(new_value, 0)) < 1;
      }
      Boolean scan_type_between_double(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToDouble(new_value, 0) <= BitConverter.ToDouble(default_value_1, 0) &&
             BitConverter.ToDouble(new_value, 0) >= BitConverter.ToDouble(default_value_0, 0);
      }
      Boolean scan_type_increased_double(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToDouble(new_value, 0) > BitConverter.ToDouble(old_value, 0);
      }
      Boolean scan_type_increased_by_double(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return Math.Abs(BitConverter.ToDouble(new_value, 0) -
             (BitConverter.ToDouble(default_value_0, 0) + BitConverter.ToDouble(old_value, 0))) < 0.0001;
      }
      Boolean scan_type_bigger_double(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToDouble(new_value, 0) > BitConverter.ToDouble(default_value_0, 0);
      }
      Boolean scan_type_decreased_double(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToDouble(new_value, 0) < BitConverter.ToDouble(old_value, 0);
      }
      Boolean scan_type_decreased_by_double(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return Math.Abs(BitConverter.ToDouble(new_value, 0) -
             (BitConverter.ToDouble(old_value, 0) - BitConverter.ToDouble(default_value_0, 0))) < 0.0001;
      }
      Boolean scan_type_less_double(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToDouble(new_value, 0) < BitConverter.ToDouble(default_value_0, 0);
      }
      Boolean scan_type_unchanged_double(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return Math.Abs(BitConverter.ToDouble(old_value, 0) -
             BitConverter.ToDouble(new_value, 0)) < 0.0001;
      }
      Boolean scan_type_equal_double(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return Math.Abs(BitConverter.ToDouble(default_value_0, 0) -
             BitConverter.ToDouble(new_value, 0)) < 0.0001;
      }
      Boolean scan_type_changed_double(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return !scan_type_unchanged_double(default_value_0, default_value_1, old_value, new_value);
      }
      Boolean scan_type_not_double(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return !scan_type_equal_double(default_value_0, default_value_1, old_value, new_value);
      }

      Boolean scan_type_any_float(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToSingle(new_value, 0) != 0 ? true : false;
      }
      Boolean scan_type_fuzzy_equal_float(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return Math.Abs(BitConverter.ToSingle(default_value_0, 0) -
             BitConverter.ToSingle(new_value, 0)) < 1;
      }
      Boolean scan_type_between_float(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToSingle(new_value, 0) <= BitConverter.ToSingle(default_value_1, 0) &&
             BitConverter.ToSingle(new_value, 0) >= BitConverter.ToSingle(default_value_0, 0);
      }
      Boolean scan_type_increased_float(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToSingle(new_value, 0) > BitConverter.ToSingle(old_value, 0);
      }
      Boolean scan_type_increased_by_float(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return Math.Abs(BitConverter.ToSingle(new_value, 0) -
             (BitConverter.ToSingle(default_value_0, 0) + BitConverter.ToSingle(old_value, 0))) < 0.0001;
      }
      Boolean scan_type_bigger_float(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToSingle(new_value, 0) > BitConverter.ToSingle(default_value_0, 0);
      }
      Boolean scan_type_decreased_float(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToSingle(new_value, 0) < BitConverter.ToSingle(old_value, 0);
      }
      Boolean scan_type_decreased_by_float(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return Math.Abs(BitConverter.ToSingle(new_value, 0) -
             (BitConverter.ToSingle(old_value, 0) - BitConverter.ToSingle(default_value_0, 0))) < 0.0001;
      }
      Boolean scan_type_less_float(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return BitConverter.ToSingle(new_value, 0) < BitConverter.ToSingle(default_value_0, 0);
      }
      Boolean scan_type_unchanged_float(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return Math.Abs(BitConverter.ToSingle(old_value, 0) -
             BitConverter.ToSingle(new_value, 0)) < 0.0001;
      }

      Boolean scan_type_equal_float(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return Math.Abs(BitConverter.ToSingle(default_value_0, 0) -
             BitConverter.ToSingle(new_value, 0)) < 0.0001;
      }
      Boolean scan_type_changed_float(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return !scan_type_unchanged_float(default_value_0, default_value_1, old_value, new_value);
      }

      Boolean scan_type_not_float(Byte[] default_value_0, Byte[] default_value_1, Byte[] old_value, Byte[] new_value) {
         return !scan_type_equal_float(default_value_0, default_value_1, old_value, new_value);
      }


      public void InitNextScanMemoryHandler(CompareType compareType) {
         if (valueType == ValueType.ArrayOfBytes)
            Length *= 2;
         InitMemoryHandler(valueType, compareType, Alignment != 1, Length);
      }

      public void InitMemoryHandler(ValueType valueType, CompareType compareType, Boolean is_alignment, Int32 type_length = 0) {
         this.valueType = valueType;
         this.compareType = compareType;

         switch (valueType) {
            case ValueType.Double:
               BytesToString = double_to_string;
               BytesToHexString = double_to_hex_string;
               StringToBytes = string_to_double;
               HexStringToBytes = hex_string_to_double;
               Length = sizeof(Double);
               Alignment = (is_alignment) ? 4 : 1;
               break;
            case ValueType.Float:
               BytesToString = float_to_string;
               BytesToHexString = float_to_hex_string;
               StringToBytes = string_to_float;
               HexStringToBytes = hex_string_to_float;
               Length = sizeof(Single);
               Alignment = (is_alignment) ? 4 : 1;
               break;
            case ValueType.UInt64:
               BytesToString = ulong_to_string;
               BytesToHexString = ulong_to_hex_string;
               StringToBytes = string_to_8_bytes;
               HexStringToBytes = hex_string_to_8_bytes;
               Length = sizeof(UInt64);
               Alignment = (is_alignment) ? 4 : 1;
               break;
            case ValueType.UInt32:
               BytesToString = uint_to_string;
               BytesToHexString = uint_to_hex_string;
               StringToBytes = string_to_4_bytes;
               HexStringToBytes = hex_string_to_4_bytes;
               Length = sizeof(UInt32);
               Alignment = (is_alignment) ? 4 : 1;
               break;
            case ValueType.UInt16:
               BytesToString = uint16_to_string;
               BytesToHexString = uint16_to_hex_string;
               StringToBytes = string_to_2_bytes;
               HexStringToBytes = hex_string_to_2_bytes;
               Length = sizeof(UInt16);
               Alignment = (is_alignment) ? 2 : 1;
               break;
            case ValueType.UInt8:
               BytesToString = uchar_to_string;
               BytesToHexString = uchar_to_hex_string;
               StringToBytes = string_to_byte;
               HexStringToBytes = hex_string_to_byte;
               Length = sizeof(Byte);
               Alignment = 1;
               break;
            case ValueType.ArrayOfBytes:
               BytesToString = hex_to_string;
               BytesToHexString = hex_to_hex_string;
               StringToBytes = string_to_hex_bytes;
               HexStringToBytes = null;
               Alignment = 1;
               Length = type_length / 2;
               break;
            case ValueType.String:
               BytesToString = string_to_string;
               BytesToHexString = string_to_hex_string;
               StringToBytes = string_to_string_bytes;
               HexStringToBytes = null;
               Alignment = 1;
               Length = type_length;
               break;
         }

         switch (compareType) {
            case CompareType.UnknownInitialValue:
               switch (valueType) {
                  case ValueType.Double:
                     Comparer = scan_type_any_double;
                     break;
                  case ValueType.Float:
                     Comparer = scan_type_any_float;
                     break;
                  case ValueType.UInt64:
                     Comparer = scan_type_any_ulong;
                     break;
                  case ValueType.UInt32:
                     Comparer = scan_type_any_uint;
                     break;
                  case ValueType.UInt16:
                     Comparer = scan_type_any_uint16;
                     break;
                  case ValueType.UInt8:
                     Comparer = scan_type_any_uint8;
                     break;
               }
               ParseFirstValue = false;
               ParseSecondValue = false;
               break;
            case CompareType.FuzzyValue:
               switch (valueType) {
                  case ValueType.Double:
                     Comparer = scan_type_fuzzy_equal_double;
                     break;
                  case ValueType.Float:
                     Comparer = scan_type_fuzzy_equal_float;
                     break;
               }
               ParseFirstValue = true;
               ParseSecondValue = false;
               break;
            case CompareType.ExactValue:
               switch (valueType) {
                  case ValueType.Double:
                     Comparer = scan_type_equal_double;
                     break;
                  case ValueType.Float:
                     Comparer = scan_type_equal_float;
                     break;
                  case ValueType.UInt64:
                     Comparer = scan_type_equal_ulong;
                     break;
                  case ValueType.UInt32:
                     Comparer = scan_type_equal_uint;
                     break;
                  case ValueType.UInt16:
                     Comparer = scan_type_equal_uint16;
                     break;
                  case ValueType.UInt8:
                     Comparer = scan_type_equal_uint8;
                     break;
                  case ValueType.ArrayOfBytes:
                     Comparer = scan_type_equal_hex;
                     break;
                  case ValueType.String:
                     Comparer = scan_type_equal_string;
                     break;
               }
               ParseFirstValue = true;
               ParseSecondValue = false;
               break;
            case CompareType.ChangedValue:
               switch (valueType) {
                  case ValueType.Double:
                     Comparer = scan_type_changed_double;
                     break;
                  case ValueType.Float:
                     Comparer = scan_type_changed_float;
                     break;
                  case ValueType.UInt64:
                     Comparer = scan_type_changed_ulong;
                     break;
                  case ValueType.UInt32:
                     Comparer = scan_type_changed_uint;
                     break;
                  case ValueType.UInt16:
                     Comparer = scan_type_changed_uint16;
                     break;
                  case ValueType.UInt8:
                     Comparer = scan_type_changed_uint8;
                     break;
               }
               ParseFirstValue = true;
               ParseSecondValue = false;
               break;
            case CompareType.UnchangedValue:
               switch (valueType) {
                  case ValueType.Double:
                     Comparer = scan_type_unchanged_double;
                     break;
                  case ValueType.Float:
                     Comparer = scan_type_unchanged_float;
                     break;
                  case ValueType.UInt64:
                     Comparer = scan_type_unchanged_ulong;
                     break;
                  case ValueType.UInt32:
                     Comparer = scan_type_unchanged_uint;
                     break;
                  case ValueType.UInt16:
                     Comparer = scan_type_unchanged_uint16;
                     break;
                  case ValueType.UInt8:
                     Comparer = scan_type_unchanged_uint8;
                     break;
               }
               ParseFirstValue = true;
               ParseSecondValue = false;
               break;
            case CompareType.IncreasedValue:
               switch (valueType) {
                  case ValueType.Double:
                     Comparer = scan_type_increased_double;
                     break;
                  case ValueType.Float:
                     Comparer = scan_type_increased_float;
                     break;
                  case ValueType.UInt64:
                     Comparer = scan_type_increased_ulong;
                     break;
                  case ValueType.UInt32:
                     Comparer = scan_type_increased_uint;
                     break;
                  case ValueType.UInt16:
                     Comparer = scan_type_increased_uint16;
                     break;
                  case ValueType.UInt8:
                     Comparer = scan_type_increased_uint8;
                     break;
               }
               ParseFirstValue = false;
               ParseSecondValue = false;
               break;
            case CompareType.IncreasedValueBy:
               switch (valueType) {
                  case ValueType.Double:
                     Comparer = scan_type_increased_by_double;
                     break;
                  case ValueType.Float:
                     Comparer = scan_type_increased_by_float;
                     break;
                  case ValueType.UInt64:
                     Comparer = scan_type_increased_by_ulong;
                     break;
                  case ValueType.UInt32:
                     Comparer = scan_type_increased_by_uint;
                     break;
                  case ValueType.UInt16:
                     Comparer = scan_type_increased_by_uint16;
                     break;
                  case ValueType.UInt8:
                     Comparer = scan_type_increased_by_uint8;
                     break;
               }
               ParseFirstValue = true;
               ParseSecondValue = false;
               break;
            case CompareType.DecreasedValue:
               switch (valueType) {
                  case ValueType.Double:
                     Comparer = scan_type_decreased_double;
                     break;
                  case ValueType.Float:
                     Comparer = scan_type_decreased_float;
                     break;
                  case ValueType.UInt64:
                     Comparer = scan_type_decreased_ulong;
                     break;
                  case ValueType.UInt32:
                     Comparer = scan_type_decreased_uint;
                     break;
                  case ValueType.UInt16:
                     Comparer = scan_type_decreased_uint16;
                     break;
                  case ValueType.UInt8:
                     Comparer = scan_type_decreased_uint8;
                     break;
               }
               ParseFirstValue = false;
               ParseSecondValue = false;
               break;
            case CompareType.DecreasedValueBy:
               switch (valueType) {
                  case ValueType.Double:
                     Comparer = scan_type_decreased_by_double;
                     break;
                  case ValueType.Float:
                     Comparer = scan_type_decreased_by_float;
                     break;
                  case ValueType.UInt64:
                     Comparer = scan_type_decreased_by_ulong;
                     break;
                  case ValueType.UInt32:
                     Comparer = scan_type_decreased_by_uint;
                     break;
                  case ValueType.UInt16:
                     Comparer = scan_type_decreased_by_uint16;
                     break;
                  case ValueType.UInt8:
                     Comparer = scan_type_decreased_by_uint8;
                     break;
               }
               ParseFirstValue = true;
               ParseSecondValue = false;
               break;
            case CompareType.BiggerThan:
               switch (valueType) {
                  case ValueType.Double:
                     Comparer = scan_type_bigger_double;
                     break;
                  case ValueType.Float:
                     Comparer = scan_type_bigger_float;
                     break;
                  case ValueType.UInt64:
                     Comparer = scan_type_bigger_ulong;
                     break;
                  case ValueType.UInt32:
                     Comparer = scan_type_bigger_uint;
                     break;
                  case ValueType.UInt16:
                     Comparer = scan_type_bigger_uint16;
                     break;
                  case ValueType.UInt8:
                     Comparer = scan_type_bigger_uint8;
                     break;
               }
               ParseFirstValue = true;
               ParseSecondValue = false;
               break;
            case CompareType.SmallerThan:
               switch (valueType) {
                  case ValueType.Double:
                     Comparer = scan_type_less_double;
                     break;
                  case ValueType.Float:
                     Comparer = scan_type_less_float;
                     break;
                  case ValueType.UInt64:
                     Comparer = scan_type_less_ulong;
                     break;
                  case ValueType.UInt32:
                     Comparer = scan_type_less_uint;
                     break;
                  case ValueType.UInt16:
                     Comparer = scan_type_less_uint16;
                     break;
                  case ValueType.UInt8:
                     Comparer = scan_type_less_uint8;
                     break;
               }
               ParseFirstValue = true;
               ParseSecondValue = false;
               break;
            case CompareType.BetweenValues:
               switch (valueType) {
                  case ValueType.Double:
                     Comparer = scan_type_between_double;
                     break;
                  case ValueType.Float:
                     Comparer = scan_type_between_float;
                     break;
                  case ValueType.UInt64:
                     Comparer = scan_type_between_ulong;
                     break;
                  case ValueType.UInt32:
                     Comparer = scan_type_between_uint;
                     break;
                  case ValueType.UInt16:
                     Comparer = scan_type_between_uint16;
                     break;
                  case ValueType.UInt8:
                     Comparer = scan_type_between_uint8;
                     break;
               }
               ParseFirstValue = true;
               ParseSecondValue = true;
               break;
         }
      }
   }
}
