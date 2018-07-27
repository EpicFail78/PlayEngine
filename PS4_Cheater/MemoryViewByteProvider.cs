using Be.Windows.Forms;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;

namespace PS4_Cheater
{
    public class MemoryViewByteProvider : IByteProvider
    {
        private Boolean _hasChanges;
        private ByteCollection _bytes;
        public List<Int32> change_list { get; set; }

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event EventHandler Changed;

        [field: CompilerGenerated, DebuggerBrowsable(0)]
        public event EventHandler LengthChanged;

        public MemoryViewByteProvider(Byte[] data) : this(new ByteCollection(data))
        {
            change_list = new List<Int32>();
        }

        public MemoryViewByteProvider(ByteCollection bytes)
        {
            this._bytes = bytes;
        }

        public void ApplyChanges()
        {
            this._hasChanges = false;
        }

        public void DeleteBytes(Int64 index, Int64 length)
        {

        }

        public Boolean HasChanges() =>
            this._hasChanges;

        public void InsertBytes(Int64 index, Byte[] bs)
        {

        }

        private void OnChanged(EventArgs e)
        {
            this._hasChanges = true;
            if (this.Changed != null)
            {
                this.Changed(this, e);
            }
        }

        private void OnLengthChanged(EventArgs e)
        {
            if (this.LengthChanged != null)
            {
                this.LengthChanged(this, e);
            }
        }

        public Byte ReadByte(Int64 index) =>
            this._bytes[(Int32)index];

        public Boolean SupportsDeleteBytes() =>
            false;

        public Boolean SupportsInsertBytes() =>
            false;

        public Boolean SupportsWriteByte() =>
            true;

        public void WriteByte(Int64 index, Byte value)
        {
            this._bytes[(Int32)index] = value;
            this.change_list.Add((Int32)index);
            this.OnChanged(EventArgs.Empty);
        }

        public ByteCollection Bytes =>
            this._bytes;

        public Int64 Length =>
            ((Int64)this._bytes.Count);

        public Int64 Offset =>
            0L;
    }
}
