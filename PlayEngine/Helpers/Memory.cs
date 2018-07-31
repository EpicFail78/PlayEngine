using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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
                  return _memoryValueToCompare == oldSearchValue - _searchValue;
               case CompareType.DecreasedValue:
                  return _memoryValueToCompare > oldSearchValue;
               case CompareType.DecreasedValueBy:
                  return _memoryValueToCompare == oldSearchValue + _searchValue;
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

      public static Mutex mutex = new Mutex();
      public static PS4RPC ps4RPC = null;
      public static Boolean initPS4RPC(String ipAddress) {
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
            mutex.WaitOne();
            returnBuf = ps4RPC.ReadMemory(procId, address, size);
         } catch (Exception ex) {
            Console.WriteLine("Error during ReadByteArray:\r\nAddress: {0}, Size: {1}\r\n{2}",
                address.ToString("X"), size, ex.ToString());
         } finally {
            mutex.ReleaseMutex();
         }
         return returnBuf ?? new Byte[1];
      }
      public static String readString(Int32 procId, UInt64 address) {
         String returnStr = String.Empty;
         try {
            mutex.WaitOne();
            returnStr = ps4RPC.ReadString(procId, address);
         } catch (Exception ex) {
            Console.WriteLine("Error during ReadString:\r\nEncoding: {0}, Address: {1}\r\n{2}",
                "UTF8", address.ToString("X"), ex.ToString());
         } finally {
            mutex.ReleaseMutex();
         }
         return returnStr;
      }
      public static Object read(Int32 procId, UInt64 address, Type valueType) {
         return readByteArray(procId, address, valueType == typeof(Boolean) ? 1 : Marshal.SizeOf(valueType))
             .getObject(valueType);
      }

      public static void writeByteArray(Int32 procId, UInt64 address, Byte[] bytes) {
         try {
            mutex.WaitOne();
            ps4RPC.WriteMemory(procId, address, bytes);
         } catch (Exception ex) {
            Console.WriteLine("Error during WriteByteArray:\r\nAddress: {0}, bytes.Length: {1}\r\n{2}",
                   address.ToString("X"), bytes.Length, ex.ToString());
         } finally {
            mutex.ReleaseMutex();
         }
      }
      public static void writeString(Int32 procId, UInt64 address, String str) {
         try {
            mutex.WaitOne();
            ps4RPC.WriteString(procId, address, str);
         } catch (Exception ex) {
            Console.WriteLine("Error during WriteString:\r\nEncoding: {0}, Address: {1}, String: {2}\r\n{3}",
                "UTF8", address.ToString("X"), str, ex.ToString());
         } finally {
            mutex.ReleaseMutex();
         }
      }
      public static void write<T>(Int32 procId, UInt64 address, T value) {
         writeByteArray(procId, address, value.getBytes());
      }

      public static List<UInt32> search(Byte[] searchBuffer, Object searchObject, Type searchObjectType, CompareType compareType, Object[] extraParams = null) {
         List<UInt32> listResults = new List<UInt32>();
         if (searchObjectType == typeof(String)) {
            listResults.AddRange(search(searchBuffer, null, typeof(Byte[]), compareType, new Object[1] { Encoding.ASCII.GetBytes((String)searchObject) }));
         } else if (searchObjectType == typeof(Byte[])) {
            List<Byte> searchBytes;
            if (extraParams == null) {
               searchBytes = new List<Byte>();
               foreach (var item in ((String)searchObject).Split(' '))
                  searchBytes.Add(Convert.ToByte(item, 16));
            } else {
               searchBytes = new List<Byte>((List<Byte>)extraParams[0]);
            }

            Int32 indexEnd = searchBuffer.Length - searchBytes.Count;
            for (Int32 index = 0; index < indexEnd; index++) {
               Boolean isFound = false;
               for (Int32 j = 0; j < searchBytes.Count - 1; j++)
                  isFound = searchBuffer[index + j] == searchBytes[j];
               if (isFound)
                  listResults.Add((UInt32)index);
            }
         } else {
            Int32 objectTypeSize = Marshal.SizeOf(searchObjectType);
            Int32 endOffset = searchBuffer.Length - objectTypeSize;
            for (Int32 index = 0; index < endOffset; index += objectTypeSize) {
               if (Memory.CompareUtil.compare(searchObject, searchBuffer.Skip(index).Take(objectTypeSize).ToArray().getObject(searchObjectType), compareType, extraParams))
                  listResults.Add((UInt32)index);

            }
         }

         return listResults;
      }
   }
}
