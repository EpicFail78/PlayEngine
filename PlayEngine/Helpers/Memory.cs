using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

using librpc;

namespace PlayEngine.Helpers {
   public class Memory {
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
      public static class CompareUtil {
         private static Single oldSearchValue = 0.0f;

         public static Boolean compareByteArray(Byte[] arr1, Byte[] arr2, CompareType compareType) {
            return false;
         }
         public static Boolean compare<T>(T searchValue, T memoryValueToCompare, CompareType compareType, Object[] extraParams = null) {
            Single _searchValue = (Single)Convert.ChangeType(searchValue, typeof(Single));
            Single _memoryValueToCompare = (Single)Convert.ChangeType(memoryValueToCompare, typeof(Single));
            CompareUtil.oldSearchValue = _searchValue;
            switch (compareType) {
               case CompareType.ExactValue:
                  return _searchValue == _memoryValueToCompare;
               case CompareType.FuzzyValue:
                  return Math.Abs(_searchValue - _memoryValueToCompare) < 1.0f;
               case CompareType.IncreasedValue:
                  return _memoryValueToCompare < oldSearchValue;
               case CompareType.IncreasedValueBy:
                  return _memoryValueToCompare == oldSearchValue - (Single)extraParams[0];
               case CompareType.DecreasedValue:
                  return _memoryValueToCompare > oldSearchValue;
               case CompareType.DecreasedValueBy:
                  return _memoryValueToCompare == oldSearchValue + (Single)extraParams[0];
               case CompareType.BiggerThan:
                  return _searchValue < _memoryValueToCompare;
               case CompareType.SmallerThan:
                  return _searchValue > _memoryValueToCompare;
               case CompareType.ChangedValue:
                  return _memoryValueToCompare != oldSearchValue;
               case CompareType.UnchangedValue:
                  return _memoryValueToCompare == oldSearchValue;
               case CompareType.BetweenValues:
                  Single betweenVal0 = (Single)Convert.ChangeType(extraParams[0], typeof(Single));
                  Single betweenVal1 = (Single)Convert.ChangeType(extraParams[1], typeof(Single));
                  return (_memoryValueToCompare >= betweenVal0) && (_memoryValueToCompare <= betweenVal1);
               case CompareType.None:
               case CompareType.UnknownInitialValue:
               default:
                  return true;
            }
         }
      }

      public static PS4RPC ps4RPC = null;
      public static Boolean initPS4RPC(String ipAddress) {
         Mutex mutex = new Mutex();
         try {
            mutex.WaitOne();
            if (ps4RPC != null)
               ps4RPC.Disconnect();
            ps4RPC = new PS4RPC(ipAddress);
            ps4RPC.Connect();
         } catch {
         } finally {
            mutex.ReleaseMutex();
         }
         return ps4RPC != null;
      }

      public static Byte[] readByteArray(Int32 procId, UInt64 address, Int32 size) {
         Byte[] returnBuf = null;
         try {
            returnBuf = ps4RPC.ReadMemory(procId, address, size);
         } catch (Exception ex) {
            Console.WriteLine("ERROR", String.Format(
                "Error during ReadByteArray:\r\nAddress: {0}, Size: {1}\r\n{2}",
                address.ToString("X"), size, ex.ToString()));
         }
         return returnBuf ?? new Byte[1];
      }
      public static String readString(Int32 procId, UInt64 address) {
         try {
            return ps4RPC.ReadString(procId, address);
         } catch (Exception ex) {
            Console.WriteLine("ERROR", string.Format(
                "Error during ReadString:\r\nEncoding: {0}, Address: {1}\r\n{2}",
                "UTF8", address.ToString("X"), ex.ToString()));
            return string.Empty;
         }
      }
      public static Object read(Int32 procId, UInt64 address, Type valueType) {
         return readByteArray(procId, address, valueType == typeof(Boolean) ? 1 : Marshal.SizeOf(valueType))
             .getObject(valueType);
      }

      public static void writeByteArray(Int32 procId, UInt64 address, Byte[] bytes) {
         try {
            ps4RPC.WriteMemory(procId, address, bytes);
         } catch (Exception ex) {
            Console.WriteLine("ERROR", String.Format(
                "Error during WriteByteArray:\r\nAddress: {0}, bytes.Length: {1}\r\n{2}",
                   address.ToString("X"), bytes.Length, ex.ToString()));
         }
      }
      public static void writeString(Int32 procId, UInt64 address, String str) {
         try {
            ps4RPC.WriteString(procId, address, str);
         } catch (Exception ex) {
            Console.WriteLine("ERROR", string.Format(
               "Error during WriteString:\r\nEncoding: {0}, Address: {1}, String: {2}\r\n{3}",
                "UTF8", address.ToString("X"), str, ex.ToString()));
         }
      }
      public static void write<T>(Int32 procId, UInt64 address, T value) {
         writeByteArray(procId, address, value.getBytes());
      }

      public static List<UInt32> search(Byte[] searchBuffer, Object searchObject, Type searchObjectType, CompareType compareType, Object[] extraParams = null) {
         List<UInt32> listResults = new List<UInt32>();
         Int32 objectTypeSize = Marshal.SizeOf(searchObjectType);
         Int32 endOffset = searchBuffer.Length - objectTypeSize;
         for (Int32 index = 0; index < endOffset; index += objectTypeSize) {
            if (Memory.CompareUtil.compare(searchObject, searchBuffer.Skip(index).Take(objectTypeSize).ToArray().getObject(searchObjectType), compareType, extraParams))
               listResults.Add((UInt32)index);
            
         }

         return listResults;
      }
   }
}
