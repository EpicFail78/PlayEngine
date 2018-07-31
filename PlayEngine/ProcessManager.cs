using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using librpc;
using PlayEngine.Helpers;

namespace PlayEngine {
   public class ResultList {
      private const Int32 buffer_size = 4096 * 16;
      private List<Byte[]> buffer_list = new List<Byte[]>();

      private Int32 buffer_tag_offset = 0;
      private Int32 buffer_tag_elem_count = 0;
      private Int32 buffer_id = 0;

      private Int32 count = 0;
      private Int32 iterator = 0;
      private Int32 element_size = 0;
      private Int32 element_alignment = 1;

      private const Int32 OFFSET_SIZE = 4;
      private const Int32 BIT_MAP_SIZE = 8;

      public ResultList(Int32 element_size, Int32 element_alignment) {
         buffer_list.Add(new Byte[buffer_size]);
         this.element_size = element_size;
         this.element_alignment = element_alignment;
      }

      private Int32 bit_count(UInt64 data, Int32 end) {
         Int32 sum = 0;
         for (Int32 i = 0; i <= end; ++i) {
            if ((data & (1ul << i)) != 0) {
               ++sum;
            }
         }
         return sum;
      }

      private Int32 bit_position(UInt64 data, Int32 pos) {
         Int32 sum = 0;
         for (Int32 i = 0; i <= 63; ++i) {
            if ((data & (1ul << i)) != 0) {
               if (sum == pos) {
                  return i;
               }
               ++sum;
            }
         }
         return -1;
      }

      public void Add(UInt32 memoryAddressOffset, Byte[] memoryValue) {
         if (memoryValue.Length != element_size) {
            throw new Exception("Invalid address!");
         }

         Byte[] dense_buffer = buffer_list[buffer_id];

         UInt32 tag_address_offset_base = BitConverter.ToUInt32(dense_buffer, buffer_tag_offset);
         UInt64 bit_map = BitConverter.ToUInt64(dense_buffer, buffer_tag_offset + OFFSET_SIZE);

         if (tag_address_offset_base > memoryAddressOffset) {
            throw new Exception("Invalid address!");
         }

         if (bit_map == 0) {
            tag_address_offset_base = memoryAddressOffset;
            Buffer.BlockCopy(BitConverter.GetBytes(memoryAddressOffset), 0, dense_buffer, buffer_tag_offset, OFFSET_SIZE);
         }

         Int32 offset_in_bit_map = (Int32)(memoryAddressOffset - tag_address_offset_base) / element_alignment;
         if (offset_in_bit_map < 64) {
            dense_buffer[buffer_tag_offset + OFFSET_SIZE + offset_in_bit_map / 8] |= (Byte)(1 << (offset_in_bit_map % 8)); //bit map
            Buffer.BlockCopy(memoryValue, 0, dense_buffer, buffer_tag_offset + OFFSET_SIZE + BIT_MAP_SIZE + element_size * buffer_tag_elem_count, element_size);//value
            ++buffer_tag_elem_count;
         } else {
            buffer_tag_offset += OFFSET_SIZE + BIT_MAP_SIZE + element_size * buffer_tag_elem_count;

            //Alloc new page
            if (buffer_tag_offset + OFFSET_SIZE + BIT_MAP_SIZE + element_size * 64 >= buffer_size) {
               buffer_list.Add(new Byte[buffer_size]);
               ++buffer_id;
               buffer_tag_offset = 0;
               buffer_tag_elem_count = 0;
               dense_buffer = buffer_list[buffer_id];
            }

            Buffer.BlockCopy(BitConverter.GetBytes(memoryAddressOffset), 0, dense_buffer, buffer_tag_offset, OFFSET_SIZE); //tag address base
            dense_buffer[buffer_tag_offset + OFFSET_SIZE] = (Byte)1; //bit map
            Buffer.BlockCopy(memoryValue, 0, dense_buffer, buffer_tag_offset + OFFSET_SIZE + BIT_MAP_SIZE, element_size); //value
            buffer_tag_elem_count = 1;
         }
         count++;
      }

      public void Clear() {
         count = 0;
         buffer_tag_offset = 0;
         buffer_tag_elem_count = 0;
         buffer_id = 0;
         buffer_list.Clear();
         buffer_list.Add(new Byte[buffer_size]);
      }

      public void Get(ref UInt32 memoryAddressOffset, ref Byte[] memoryValue) {

         Byte[] dense_buffer = buffer_list[buffer_id];
         memoryValue = new Byte[element_size];

         UInt32 offset_base = BitConverter.ToUInt32(dense_buffer, buffer_tag_offset);
         UInt64 bit_map = BitConverter.ToUInt64(dense_buffer, buffer_tag_offset + OFFSET_SIZE);
         memoryAddressOffset = (UInt32)(bit_position(bit_map, buffer_tag_elem_count) * element_alignment) + offset_base;
         Buffer.BlockCopy(dense_buffer, buffer_tag_offset + OFFSET_SIZE + BIT_MAP_SIZE + element_size * buffer_tag_elem_count, memoryValue, 0, element_size);
      }

      public void Set(Byte[] memoryValue) {
         Byte[] dense_buffer = buffer_list[buffer_id];
         Buffer.BlockCopy(memoryValue, 0, dense_buffer, buffer_tag_offset + OFFSET_SIZE + BIT_MAP_SIZE + element_size * buffer_tag_elem_count, element_size);
      }

      public void Begin() {
         iterator = 0;
         buffer_tag_offset = 0;
         buffer_tag_elem_count = 0;
         buffer_id = 0;
      }

      public void Next() {
         ++iterator;

         Byte[] dense_buffer = buffer_list[buffer_id];
         UInt32 base_offset = BitConverter.ToUInt32(dense_buffer, buffer_tag_offset);
         UInt64 bit_map = BitConverter.ToUInt64(dense_buffer, buffer_tag_offset + 4);
         ++buffer_tag_elem_count;

         if (bit_count(bit_map, 63) <= buffer_tag_elem_count) {
            buffer_tag_offset += OFFSET_SIZE + BIT_MAP_SIZE + element_size * buffer_tag_elem_count;
            if (buffer_tag_offset + OFFSET_SIZE + BIT_MAP_SIZE + element_size * 64 >= buffer_size) {
               ++buffer_id;
               buffer_tag_offset = 0;
               buffer_tag_elem_count = 0;
            } else {
               buffer_tag_elem_count = 0;
            }
         }
      }

      public Boolean End() {
         return (iterator == count);
      }

      public Int32 Count { get { return count; } }
   }

   public class MappedSection {
      public UInt64 Start { get; set; }
      public Int32 Length { get; set; }
      public String Name { get; set; }
      public Boolean Check { set; get; }
      public UInt32 Prot { get; set; }

      public ResultList ResultList { get; set; }


      public MappedSection() {
         ResultList = null;
      }
   }

   public class MappedSectionList {
      public UInt64 TotalMemorySize { get; set; }

      private List<MappedSection> mapped_section_list = new List<MappedSection>();

      public MappedSectionList() {

      }

      public MappedSection this[Int32 index]
      {
         get {
            return mapped_section_list[index];
         }
      }

      private Int32 FindSectionID(UInt64 address) {
         Int32 low = 0;
         Int32 high = mapped_section_list.Count - 1;
         Int32 middle;

         while (low <= high) {
            middle = (low + high) / 2;
            if (address >= mapped_section_list[middle].Start + (UInt64)(mapped_section_list[middle].Length)) {
               low = middle + 1;   //查找数组后部分  
            } else if (address < mapped_section_list[middle].Start) {
               high = middle - 1;  //查找数组前半部分  
            } else {
               return middle;  //找到用户要查找的数字，返回下标  
            }
         }

         return -1;
      }



      public Int32 GetMappedSectionID(UInt64 address) {
         UInt64 start = 0;
         UInt64 end = 0;

         if (mapped_section_list.Count > 0) {
            start = mapped_section_list[0].Start;
            end = mapped_section_list[mapped_section_list.Count - 1].Start + (UInt64)mapped_section_list[mapped_section_list.Count - 1].Length;
         }

         if (start > address || end < address) {
            return -1;
         }

         return FindSectionID(address);
      }

      public MappedSection GetMappedSection(UInt64 address) {
         Int32 sectionID = GetMappedSectionID(address);
         if (sectionID < 0) {
            return null;
         }
         return mapped_section_list[sectionID];
      }

      public void SectionCheck(Int32 idx, Boolean _checked) {
         mapped_section_list[idx].Check = _checked;
         if (mapped_section_list[idx].Check) {
            TotalMemorySize += (UInt64)mapped_section_list[idx].Length;
         } else {
            TotalMemorySize -= (UInt64)mapped_section_list[idx].Length;
         }
      }


      public String GetSectionName(Int32 section_idx) {
         if (section_idx < 0) {
            return "sectioni wrong!";
         }
         MappedSection sectionInfo = mapped_section_list[section_idx];

         StringBuilder section_name = new StringBuilder();
         section_name.Append(sectionInfo.Name + "-");
         section_name.Append(String.Format("{0:X}", sectionInfo.Prot) + "-");
         section_name.Append(String.Format("{0:X}", sectionInfo.Start) + "-");
         section_name.Append((sectionInfo.Length / 1024).ToString() + "KB");

         return section_name.ToString();
      }

      public List<MappedSection> GetMappedSectionList(String name, Int32 prot) {
         List<MappedSection> result_list = new List<MappedSection>();
         for (Int32 idx = 0; idx < mapped_section_list.Count; ++idx) {
            if (mapped_section_list[idx].Prot == prot &&
                mapped_section_list[idx].Name.StartsWith(name)) {
               result_list.Add(mapped_section_list[idx]);
            }
         }
         return result_list;
      }

      public void InitMemorySectionList(ProcessInfo pi) {
         mapped_section_list.Clear();
         TotalMemorySize = 0;

         for (Int32 i = 0; i < pi.entries.Length; i++) {
            MemoryEntry entry = pi.entries[i];
            if ((entry.prot & 0x1) == 0x1) {
               UInt64 length = entry.end - entry.start;
               UInt64 start = entry.start;
               String name = entry.name;
               Int32 idx = 0;
               UInt64 buffer_length = 1024 * 1024 * 128;

               //Executable section
               if ((entry.prot & 0x5) == 0x5) {
                  buffer_length = length;
               }

               while (length != 0) {
                  UInt64 cur_length = buffer_length;

                  if (cur_length > length) {
                     cur_length = length;
                     length = 0;
                  } else {
                     length -= cur_length;
                  }

                  MappedSection mappedSection = new MappedSection();
                  mappedSection.Start = start;
                  mappedSection.Length = (Int32)cur_length;
                  mappedSection.Name = entry.name + "[" + idx + "]";
                  mappedSection.Check = false;
                  mappedSection.Prot = entry.prot;

                  mapped_section_list.Add(mappedSection);

                  start += cur_length;
                  ++idx;
               }
            }
         }

      }

      public UInt64 TotalResultCount() {
         UInt64 total_result_count = 0;
         for (Int32 idx = 0; idx < mapped_section_list.Count; ++idx) {
            if (mapped_section_list[idx].Check && mapped_section_list[idx].ResultList != null) {
               total_result_count += (UInt64)mapped_section_list[idx].ResultList.Count;
            }
         }
         return total_result_count;
      }

      public void ClearResultList() {
         for (Int32 idx = 0; idx < mapped_section_list.Count; ++idx) {
            if (mapped_section_list[idx].ResultList != null) {
               mapped_section_list[idx].ResultList.Clear();
            }
         }
      }

      public Int32 Count { get { return mapped_section_list.Count; } }
   }

   public class ProcessManager {
      public static ProcessManager mInstance = null;
      public MappedSectionList MappedSectionList { get; }

      public ProcessManager() {
         MappedSectionList = new MappedSectionList();
      }
      static ProcessManager() {
         mInstance = new ProcessManager();
      }

      public ProcessInfo GetProcessInfo(String process_name) {
         ProcessList processList = Memory.ps4RPC.GetProcessList();
         ProcessInfo processInfo = null;
         foreach (Process process in processList.processes) {
            if (process.name == process_name) {
               processInfo = Memory.ps4RPC.GetProcessInfo(process.pid);
               break;
            }
         }

         return processInfo;
      }

      public String GetProcessName(Int32 idx) {
         return Memory.ps4RPC.GetProcessList().processes[idx].name;
      }

   }
}
