using System;
using System.Windows.Forms;

namespace PlayEngine.Forms.ChildForms {
   public partial class childFrmEditCheat : Form {
      public class ReturnInformation {
         public String description;
         public String sectionName;
         public UInt32 sectionAddressOffset;
         public String valueType;
         public String value;
      }
      public ReturnInformation returnInformation;

      public childFrmEditCheat(String description, String sectionName, UInt32 sectionAddressOffset, String strValueType, Object value) {
         InitializeComponent();
         for (int i = 0; i < ProcessManager.mInstance.MappedSectionList.Count; i++)
            cmbBoxSection.Items.Add(ProcessManager.mInstance.MappedSectionList.GetSectionName(i));
         cmbBoxSection.SelectedItem = sectionName;
         cmbBoxValueType.SelectedItem = strValueType;

         txtBoxDescription.Text = description;
         txtBoxSectionAddressOffset.Text = sectionAddressOffset.ToString("X");
         txtBoxValue.Text = value.ToString();
      }

      private void btnApply_Click(Object sender, EventArgs e) {
         this.returnInformation = new ReturnInformation
         {
            description = txtBoxDescription.Text,
            sectionName = (String)cmbBoxSection.SelectedItem,
            sectionAddressOffset = UInt32.Parse(txtBoxSectionAddressOffset.Text, System.Globalization.NumberStyles.HexNumber),
            valueType = (String)cmbBoxValueType.SelectedItem,
            value = txtBoxValue.Text
         };
         this.DialogResult = DialogResult.OK;
         this.Close();
      }

      private void btnCancel_Click(Object sender, EventArgs e) {
         this.DialogResult = DialogResult.Cancel;
         this.Close();
      }
   }
}
