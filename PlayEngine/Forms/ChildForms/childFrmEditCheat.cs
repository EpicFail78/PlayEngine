using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

using PlayEngine.Helpers;

namespace PlayEngine.Forms.ChildForms {
   public partial class childFrmEditCheat : Form {
      public childFrmEditCheat(String description, String sectionName, UInt32 sectionAddressOffset, MemoryHelper.ValueType valueType, String value) {
         InitializeComponent();

         txtBoxDescription.Text = description;
         //
         txtBoxSectionAddressOffset.Text = sectionAddressOffset.ToString("X");
         cmbBoxValueType.Items.AddRange(Enum.GetNames(typeof(MemoryHelper.ValueType));
         cmbBoxValueType.SelectedValue = value;
         txtBoxValue.Text = value;
      }
   }
}
