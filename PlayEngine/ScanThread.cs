using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PlayEngine
{
    class PeekThread
    {
        private ProcessManager processManager;
        private BackgroundWorker worker;
        private List<Byte[]> buffer_queue;

        private Semaphore consumer_mutex;
        private Semaphore producer_mutex;

        private Int32 productor_idx = 0;

        public PeekThread(ProcessManager processManager, List<Byte[]> bufferQueue,
            Semaphore consumerMutex, Semaphore producerMutex)
        {
            this.processManager = processManager;
            this.buffer_queue = bufferQueue;
            this.consumer_mutex = consumerMutex;
            this.producer_mutex = producerMutex;
            this.productor_idx = 0;
        }

        public void Peek()
        {
            for (Int32 section_idx = 0; section_idx < processManager.MappedSectionList.Count; ++section_idx)
            {
                MappedSection mappedSection = processManager.MappedSectionList[section_idx];

                if (!mappedSection.Check)
                {
                    mappedSection.ResultList = null;
                    continue;
                }

            UInt64 address = mappedSection.Start;
            Int32 length = mappedSection.Length;

                while (length != 0)
                {
               Int32 cur_length = CONSTANT.PEEK_BUFFER_LENGTH;

                    if (cur_length > length)
                    {
                        cur_length = length;
                        length = 0;
                    }
                    else
                    {
                        length -= cur_length;
                    }

                    if (worker.CancellationPending) break;

                    producer_mutex.WaitOne();
                    //buffer_queue[productor_idx] = memoryHelper.ReadMemory(address, (int)cur_length);
                    productor_idx = (productor_idx + 1) % CONSTANT.MAX_PEEK_QUEUE;
                    consumer_mutex.Release();

                    address += (UInt64)cur_length;
                }
            }
        }
    }

    class ComparerThread
    {
        private ProcessManager processManager;

        private MemoryHelper memoryHelper;

        private List<Byte[]> buffer_queue;

        private Byte[] default_value_0 = null;
        private Byte[] default_value_1 = null;

        private BackgroundWorker worker;

        private Semaphore consumer_mutex;
        private Semaphore producer_mutex;
        private Mutex worker_mutex;

        private Int32 consumer_idx = 0;

        public ComparerThread(ProcessManager processManager, MemoryHelper memoryHelper, List<Byte[]> bufferQueue,
            String value_0, String value_1, BackgroundWorker worker, Semaphore consumerMutex, Semaphore producerMutex, Mutex workerMutex)
        {
            this.processManager = processManager;
            this.memoryHelper = memoryHelper;
            this.buffer_queue = bufferQueue;

            this.consumer_idx = 0;
            this.default_value_0 = memoryHelper.StringToBytes(value_0);
            this.default_value_1 = memoryHelper.StringToBytes(value_1);
            this.worker = worker;
            this.consumer_mutex = consumerMutex;
            this.producer_mutex = producerMutex;
            this.worker_mutex = workerMutex;
        }

        public void ResultListOfNewScan()
        {
         Int64 processed_memory_len = 0;
         UInt64 total_memory_size = processManager.MappedSectionList.TotalMemorySize + 1;

            for (Int32 section_idx = 0; section_idx < processManager.MappedSectionList.Count; ++section_idx)
            {
                if (worker.CancellationPending) break;
                MappedSection mappedSection = processManager.MappedSectionList[section_idx];

                if (!mappedSection.Check)
                {
                    mappedSection.ResultList = null;
                    continue;
                }

                ResultList new_result_list = new ResultList(memoryHelper.Length, memoryHelper.Alignment);

            UInt64 address = mappedSection.Start;
            UInt32 base_address_offset = 0;
            Int32 length = mappedSection.Length;

                while (length != 0)
                {
               Int32 cur_length = CONSTANT.PEEK_BUFFER_LENGTH;

                    if (cur_length > length)
                    {
                        cur_length = length;
                        length = 0;
                    }
                    else
                    {
                        length -= cur_length;
                    }

                    if (worker.CancellationPending) break;

                    consumer_mutex.WaitOne();

               Int32 element_alignment = memoryHelper.Alignment;
               Int32 element_length = memoryHelper.Length;

               Byte[] buffer = buffer_queue[consumer_idx];

                    Byte[] new_value = new Byte[element_length];
                    if (default_value_0.Length == 0)
                    {
                        for (Int32 i = 0; i + element_length < buffer.LongLength; i += element_alignment)
                        {
                            Buffer.BlockCopy(buffer, i, new_value, 0, element_length);
                            if (memoryHelper.Comparer(default_value_0, default_value_1, null, new_value))
                            {
                                new_result_list.Add((UInt32)i + base_address_offset, new_value);
                            }
                        }
                    }

                    consumer_idx = (consumer_idx + 1) % CONSTANT.MAX_PEEK_QUEUE;
                    producer_mutex.Release();

                    address += (UInt64)cur_length;
                    base_address_offset += (UInt32)cur_length;
                }

                mappedSection.ResultList = new_result_list;
                if (mappedSection.Check) processed_memory_len += mappedSection.Length;
                worker.ReportProgress((Int32)(((Single)processed_memory_len / total_memory_size) * 80));
            }
        }

        public void ResultListOfNextScan()
        {
         Int64 processed_memory_len = 0;
         UInt64 total_memory_size = processManager.MappedSectionList.TotalMemorySize + 1;

            for (Int32 section_idx = 0; section_idx < processManager.MappedSectionList.Count; ++section_idx)
            {
                if (worker.CancellationPending) break;
                MappedSection mappedSection = processManager.MappedSectionList[section_idx];

                if (!mappedSection.Check)
                {
                    mappedSection.ResultList = null;
                    continue;
                }

                ResultList new_result_list = new ResultList(memoryHelper.Length, memoryHelper.Alignment);

            UInt64 address = mappedSection.Start;
            UInt32 base_address_offset = 0;
            Int32 length = mappedSection.Length;

                ResultList old_result_list = mappedSection.ResultList;
                old_result_list.Begin();

                while (length != 0)
                {
               Int32 cur_length = CONSTANT.PEEK_BUFFER_LENGTH;

                    if (cur_length > length)
                    {
                        cur_length = length;
                        length = 0;
                    }
                    else
                    {
                        length -= cur_length;
                    }

                    if (worker.CancellationPending) break;

                    consumer_mutex.WaitOne();

               Int32 element_alignment = memoryHelper.Alignment;
               Int32 element_length = memoryHelper.Length;

               Byte[] buffer = buffer_queue[consumer_idx];
               Int32 buffer_len = buffer.Length;
                    Byte[] new_value = new Byte[element_length];

                    if (default_value_0.Length == 0)
                    {
                        for (; !old_result_list.End(); old_result_list.Next())
                        {
                     UInt32 address_offset = 0;
                            Byte[] old_value = null;
                            old_result_list.Get(ref address_offset, ref old_value);

                            if (address_offset - base_address_offset + length >= buffer_len)
                                break;

                            Buffer.BlockCopy(buffer, (Int32)(address_offset - base_address_offset), new_value, 0, length);
                            if (memoryHelper.Comparer(default_value_0, default_value_1, old_value, new_value))
                            {
                                new_result_list.Add(address_offset, new_value);
                            }
                        }
                    }

                    consumer_idx = (consumer_idx + 1) % CONSTANT.MAX_PEEK_QUEUE;
                    producer_mutex.Release();

                    address += (UInt64)cur_length;
                    base_address_offset += (UInt32)cur_length;
                }

                mappedSection.ResultList = new_result_list;
                if (mappedSection.Check) processed_memory_len += mappedSection.Length;
                worker.ReportProgress((Int32)(((Single)processed_memory_len / total_memory_size) * 80));
            }
        }
    }
}
