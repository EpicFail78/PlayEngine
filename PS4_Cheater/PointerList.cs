using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using librpc;

namespace PS4_Cheater
{

    [StructLayoutAttribute(LayoutKind.Sequential, Pack = 1)]
    public struct Pointer
    {
        public UInt64 Address;
        public UInt64 PointerValue;

        public Pointer(UInt64 Address, UInt64 PointerValue)
        {
            this.Address = Address;
            this.PointerValue = PointerValue;
        }
    }

    public class PointerPath
    {
        public List<UInt64> pointerPath = new List<UInt64>();

        public void AddRange(List<UInt64> pointerList)
        {
            pointerPath.AddRange(pointerList);
        }

        public void Add(UInt64 pointer)
        {
            pointerPath.Add(pointer);
        }

        public UInt64 this[Int32 index]
        {
            get
            {
                return pointerPath[index];
            }
        }

        public Int32 Count { get { return pointerPath.Count; } }
    }

   public class PointerResult {
      public Int32 baseSectionID;
      public UInt64 baseOffset;
      public Int64[] offsets;

      public PointerResult() { }
      public PointerResult(Int32 BaseSectionID, UInt64 BaseOffset, List<Int64> Offsets) {
         this.baseSectionID = BaseSectionID;
         this.baseOffset = BaseOffset;
         this.offsets = new Int64[Offsets.Count];
         for (Int32 i = 0; i < this.offsets.Length; ++i) {
            this.offsets[i] = Offsets[this.offsets.Length - 1 - i];
         }

      }

      public UInt64 GetBaseAddress(MappedSectionList mappedSectionList) {
         if (baseSectionID >= mappedSectionList.Count)
            return 0;

         MappedSection section = mappedSectionList[baseSectionID];
         return section.Start + baseOffset;
      }
   }

   public class PointerList
    {
        private List<Pointer> pointer_list_order_by_address;
        private List<Pointer> pointer_list_order_by_pointer_value;

        public Boolean Stop { get; set; }

        public PointerList()
        {
            pointer_list_order_by_address = new List<Pointer>();
            pointer_list_order_by_pointer_value = new List<Pointer>();
        }

        public delegate void NewPathGeneratedHandler(PointerList pointerList, List<Int64> path_offset, List<Pointer> path_address);

        public event NewPathGeneratedHandler NewPathGeneratedEvent;

        class ComparerByAddress : IComparer<Pointer>
        {
            public Int32 Compare(Pointer x, Pointer y)
            {
                if (x.Address == y.Address)
                {
                    return 0;
                }
                else if (x.Address < y.Address)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
        }

        class ComparerByPointerValue : IComparer<Pointer>
        {
            public Int32 Compare(Pointer x, Pointer y)
            {
                if (x.PointerValue == y.PointerValue)
                {
                    if (x.Address == y.Address)
                    {
                        return 0;
                    }
                    else if (x.Address < y.Address)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }
                }
                else if (x.PointerValue < y.PointerValue)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
        }

        public void Add(Pointer pointer)
        {
            pointer_list_order_by_address.Add(pointer);
            pointer_list_order_by_pointer_value.Add(pointer);
        }

        public void Clear()
        {
            pointer_list_order_by_address.Clear();
            pointer_list_order_by_pointer_value.Clear();
        }

        private static Int32 BinarySearchByAddress(List<Pointer> pointerList, Int32 low, Int32 high, UInt64 address)
        {
         Int32 mid = (low + high) / 2;
            if (low > high)
            {
                return -1;
            }
            else
            {
                if (pointerList[mid].Address == address)
                    return mid;
                else if (pointerList[mid].Address > address)
                {
                    if (mid - 1 >= 0 && pointerList[mid - 1].Address <= address)
                    {
                        return mid - 1;
                    }
                    return BinarySearchByAddress(pointerList, low, mid - 1, address);
                }
                else
                {
                    if (mid + 1 < pointerList.Count && pointerList[mid + 1].Address >= address)
                    {
                        return mid + 1;
                    }
                    return BinarySearchByAddress(pointerList, mid + 1, high, address);
                }
            }
        }

        private static Int32 BinarySearchByValue(List<Pointer> pointerList, UInt64 pointerValue)
        {
         Int32 low = 0;
         Int32 high = pointerList.Count - 1;
         Int32 middle;

            while (low <= high)
            {
                middle = (low + high) / 2;
                if (pointerValue > pointerList[middle].PointerValue)
                {
                    low = middle + 1;
                }
                else if (pointerValue < pointerList[middle].PointerValue)
                {
                    high = middle - 1;
                }
                else
                {
                    return middle;
                }
            }

            return -1;
        }

        public UInt64 GetTailAddress(PointerResult pointerResult, MappedSectionList mappedSectionList)
        {
         UInt64 tailAddress = pointerResult.GetBaseAddress(mappedSectionList);

            if (pointerResult.offsets.Length > 0)
            {
            Int32 j = 0;
                Pointer pointer = new Pointer();
            Int32 index = GetPointerByAddress(tailAddress, ref pointer);
                if (index < 0) return 0;
                tailAddress = pointer.PointerValue;
                for (j = 0; j < pointerResult.offsets.Length - 1; ++j)
                {
                    index = GetPointerByAddress((UInt64)((Int64)tailAddress + pointerResult.offsets[j]), ref pointer);
                    if (index < 0) return 0;
                    tailAddress = pointer.PointerValue;
                }

                tailAddress = (UInt64)((Int64)tailAddress + pointerResult.offsets[j]);
            }

            return tailAddress;
        }

        private List<Pointer> GetPointerListByValue(UInt64 pointerValue)
        {
            List<Pointer> pointerList = new List<Pointer>();
         Int32 index = BinarySearchByValue(pointer_list_order_by_pointer_value, pointerValue);

            if (index < 0) return pointerList;

         Int32 start = index;
            for (; start >= 0; --start)
            {
                if (pointer_list_order_by_pointer_value[start].PointerValue != pointerValue)
                {
                    break;
                }
            }

         Boolean find = false;
            for (Int32 i = start; i < pointer_list_order_by_pointer_value.Count; ++i)
            {
                if (pointer_list_order_by_pointer_value[i].PointerValue == pointerValue)
                {
                    find = true;
                    pointerList.Add(pointer_list_order_by_pointer_value[i]);
                }
                else
                {
                    if (find) break;
                }
            }

            return pointerList;
        }

        private Int32 GetPointerByAddress(UInt64 address, ref Pointer pointer)
        {
         Int32 index = BinarySearchByAddress(pointer_list_order_by_address, 0, pointer_list_order_by_address.Count - 1, address);
            if (index < 0) return index;

            pointer = pointer_list_order_by_address[index];
            return index;
        }

        private void PointerFinder(List<Int64> path_offset, List<Pointer> path_address,
            UInt64 address, List<Int32> range, Int32 level)
        {

            if (Stop)
            {
                return;
            }
            
            if (level < range.Count)
            {

                Pointer position = new Pointer();
            Int32 index = GetPointerByAddress(address, ref position);
            Int32 counter = 0;

                for (Int32 i = index; i >= 0; i--)
                {
                    if (Stop)
                    {
                        break;
                    }

                    if ((Int64)pointer_list_order_by_address[i].Address + range[level] < (Int64)address)
                    {
                        break;
                    }

                    List<Pointer> pointerList = GetPointerListByValue(pointer_list_order_by_address[i].Address);

                    if (pointerList.Count > 0)
                    {
                        path_offset.Add((Int64)(address - pointer_list_order_by_address[i].Address));
                        const Int32 max_pointer_count =  15;
                  Int32 cur_pointer_counter = 0;

                  Boolean in_new_level = false;

                        for (Int32 j = 0; j < pointerList.Count; ++j)
                        {
                     Boolean in_stack = false;
                            for (Int32 k = 0; k < path_address.Count; ++k)
                            {
                                if (path_address[k].PointerValue == pointerList[j].PointerValue ||
                                    path_address[k].Address == pointerList[j].Address)
                                {
                                    in_stack = true;
                                    break;
                                }
                            }
                            if (in_stack)
                            {
                                continue;
                            }

                            in_new_level = true;
                            if (cur_pointer_counter >= max_pointer_count) break;

                            ++cur_pointer_counter;

                            path_address.Add(pointerList[j]);
                            PointerFinder(path_offset, path_address, pointerList[j].Address, range, level + 1);
                            path_address.RemoveAt(path_address.Count - 1);
                        }

                        path_offset.RemoveAt(path_offset.Count - 1);

                        if (counter >= 1)
                        {
                            break;
                        }

                        if (in_new_level) ++counter;
                    }
                }
            }

            if (Stop)
            {
                return;
            }

            NewPathGeneratedEvent?.Invoke(this, path_offset, path_address);
        }

        public void Save()
        {
         String ADDRESS_NAME = "D:\\name.txt";

         String[] lines = new String[pointer_list_order_by_address.Count];

            for (Int32 i = 0; i < pointer_list_order_by_address.Count; ++i)
            {
                lines[i] = pointer_list_order_by_address[i].Address.ToString() + " " + pointer_list_order_by_address[i].PointerValue.ToString();
            }
            File.WriteAllLines(ADDRESS_NAME, lines);
        }

        public void Load()
        {
         String ADDRESS_NAME = "D:\\name.txt";

         String[] lines = File.ReadAllLines(ADDRESS_NAME);

            for (Int32 i = 0; i < lines.Length; ++i)
            {
            String[] elems = lines[i].Split(new Char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            UInt64 Address = UInt64.Parse(elems[0]);
            UInt64 PointerValue = UInt64.Parse(elems[1]);

                Pointer pointer = new Pointer(Address, PointerValue);
                pointer_list_order_by_address.Add(pointer);
                pointer_list_order_by_pointer_value.Add(pointer);
            }

            pointer_list_order_by_pointer_value.Sort(new ComparerByPointerValue());

        }

        public void Init()
        {
            pointer_list_order_by_address.Sort(new ComparerByAddress());
            pointer_list_order_by_pointer_value.Sort(new ComparerByPointerValue());
        }

        public void FindPointerList(UInt64 address, List<Int32> range)
        {
            List<Int64> path_offset = new List<Int64>();
            List<Pointer> path_address = new List<Pointer>();
         Boolean changed = true;
            PointerFinder(path_offset, path_address, address, range, 0);
        }

        public Int32 Count { get { return pointer_list_order_by_address.Count; } }
    }

}
