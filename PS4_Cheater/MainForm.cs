using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PS4_Cheater.Utils;

namespace PS4_Cheater {
   public partial class MainForm : Form {
      private enum ScanStatus {
         FirstScan,
         DidScan,
         Scanning
      }
      public static class ResultsColumnIndex {
         public static readonly Int32 Address = 0;
         public static readonly Int32 Section = 1;
         public static readonly Int32 Value = 2;
      }
      public static class SavedResultsColumnIndex {
         public static readonly Int32 Freeze = 0;
         public static readonly Int32 Description = 1;
         public static readonly Int32 Address = 2;
         public static readonly Int32 Section = 3;
         public static readonly Int32 Type = 4;
         public static readonly Int32 Value = 5;
      }
      public static class ScanTypeOptions {
         public static readonly List<String> listSearch_FirstScan = new List<String>()
         {
            "Exact value",
            "Bigger than...",
            "Smaller than...",
            "Value between...",
            "Unknown initial value"
         };
         public static readonly List<String> listSearch_NextScan = new List<String>()
         {
            "Exact value",
            "Increased value",
            "Increased value by...",
            "Decreased value",
            "Decreased value by...",
            "Bigger than...",
            "Smaller than...",
            "Value between...",
            "Changed value",
            "Unchanged value"
         };
         public static readonly List<String> listSearchExactOnly = new List<String>()
         {
            "Exact value"
         };

         public static MemoryHelper.CompareType getCompareTypeFromString(String str) {
            switch (str) {
               case "Exact value":
                  return MemoryHelper.CompareType.ExactValue;
               case "Fuzzy value":
                  return MemoryHelper.CompareType.FuzzyValue;
               case "Increased value":
                  return MemoryHelper.CompareType.IncreasedValue;
               case "Increased value by...":
                  return MemoryHelper.CompareType.IncreasedValueBy;
               case "Decreased value":
                  return MemoryHelper.CompareType.DecreasedValue;
               case "Decreased value by...":
                  return MemoryHelper.CompareType.DecreasedValueBy;
               case "Bigger than...":
                  return MemoryHelper.CompareType.BiggerThan;
               case "Smaller than...":
                  return MemoryHelper.CompareType.SmallerThan;
               case "Value between...":
                  return MemoryHelper.CompareType.BetweenValues;
               case "Changed value":
                  return MemoryHelper.CompareType.ChangedValue;
               case "Unchanged value":
                  return MemoryHelper.CompareType.UnchangedValue;
               case "Unknown initial value":
                  return MemoryHelper.CompareType.UnknownInitialValue;
               default:
                  return MemoryHelper.CompareType.None;
            }
         }
         public static MemoryHelper.ValueType getValueTypeFromString(String str) {
            switch (str) {
               case "Byte":
                  return MemoryHelper.ValueType.UInt8;
               case "2 Bytes":
                  return MemoryHelper.ValueType.UInt16;
               case "4 Bytes":
                  return MemoryHelper.ValueType.UInt32;
               case "8 Bytes":
                  return MemoryHelper.ValueType.UInt64;
               case "Array of bytes":
                  return MemoryHelper.ValueType.ArrayOfBytes;
               default:
                  return (MemoryHelper.ValueType)Enum.Parse(typeof(MemoryHelper.ValueType), str);
            }
         }
      }
      private void setControlEnabled(Control[] arrControls, Boolean isEnabled) {
         foreach (Control cntrl in arrControls) {
            cntrl.Enabled = isEnabled;
         }
      }

      ProcessManager processManager;
      MemoryHelper memoryHelper;
      CheatList cheatList;
      private List<Int32> scanResults;
      private ScanStatus _curScanStatus = ScanStatus.FirstScan;
      private ScanStatus curScanStatus
      {
         get {
            return _curScanStatus;
         }
         set {
            _curScanStatus = value;
            switch (value) {
               case ScanStatus.FirstScan: {
                  setControlEnabled(new Control[] { btnScan, chkBoxIsHexValue, chkBoxFastScan, cmbBoxScanType, cmbBoxValueType, chkListBoxSearchSections }, true);
                  setControlEnabled(new Control[] { btnScanNext }, false);
                  uiToolStrip_linkPayloadAndProcess.Enabled = true;

                  String strBackupSelectedItem = (String)cmbBoxScanType.SelectedItem;
                  cmbBoxScanType.DataSource = ScanTypeOptions.listSearch_FirstScan;
                  cmbBoxScanType.SelectedItem = strBackupSelectedItem;

                  btnScan.Invoke(new Action(() => btnScan.Text = "First Scan"));
                  this.Invoke(new Action(() => uiStatusStrip_lblStatus.Text = "Standby..."));
               }
               break;
               case ScanStatus.DidScan: {
                  setControlEnabled(new Control[] { btnScan, btnScanNext, chkBoxIsHexValue, chkBoxFastScan, cmbBoxScanType, cmbBoxValueType, chkListBoxSearchSections }, true);
                  uiToolStrip_linkPayloadAndProcess.Enabled = true;

                  String strBackupSelectedItem = (String)cmbBoxScanType.SelectedItem;
                  cmbBoxScanType.DataSource = ScanTypeOptions.listSearch_NextScan;
                  cmbBoxScanType.SelectedItem = strBackupSelectedItem;

                  btnScan.Invoke(new Action(() => btnScan.Text = "New Scan"));
                  this.Invoke(new Action(() => uiStatusStrip_lblStatus.Text = String.Format("{0} results", scanResults.Count)));
               }
               break;
               case ScanStatus.Scanning: {
                  setControlEnabled(new Control[] { btnScan }, true);
                  setControlEnabled(new Control[] { btnScanNext, chkBoxIsHexValue, chkBoxFastScan, cmbBoxScanType, cmbBoxValueType, chkListBoxSearchSections }, false);
                  uiToolStrip_linkPayloadAndProcess.Enabled = false;

                  btnScan.Invoke(new Action(() => btnScan.Text = "Stop"));
                  this.Invoke(new Action(() => uiStatusStrip_lblStatus.Text = "Scanning..."));
               }
               break;
            }
         }
      }

      public MainForm() {
         processManager = new ProcessManager();
         memoryHelper = new MemoryHelper(true, 0);
         cheatList = new CheatList();

         InitializeComponent();
         this.Text = String.Format("PS4 Cheater v{0}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
         cmbBoxValueType.SelectedIndex = 2; // 4 Bytes
      }

      private void btnScan_OnClick() {
         try {
            switch (curScanStatus) {
               case ScanStatus.FirstScan:
                  if (MessageBox.Show(String.Format("Search size: {0}KB.\r\nContinue?", processManager.MappedSectionList.TotalMemorySize / 1024), "Scan", MessageBoxButtons.YesNo) != DialogResult.Yes)
                     return;

                  memoryHelper.InitMemoryHandler(
                     ScanTypeOptions.getValueTypeFromString((String)cmbBoxValueType.SelectedItem),
                     ScanTypeOptions.getCompareTypeFromString((String)cmbBoxScanType.SelectedItem),
                     chkBoxFastScan.Checked, txtBoxValue.Text.Length);

                  bgWorkerScanner.RunWorkerAsync();
                  curScanStatus = ScanStatus.Scanning;
                  break;
               case ScanStatus.DidScan:
                  listViewResults.Items.Clear();
                  processManager.MappedSectionList.ClearResultList();
                  break;
               case ScanStatus.Scanning:
                  bgWorkerScanner.CancelAsync();
                  break;
            }
         } catch (Exception ex) {
            MessageBox.Show(ex.ToString());
         }
      }
      private void btnScanNext_OnClick() {

      }
      private void uiButtonHandler_Click(Object sender, EventArgs e) {
         String btnName = sender.GetType() == typeof(Button) ? (sender as Button).Name : (sender as ToolStripMenuItem).Name;
         switch (btnName) {
            case "btnScan":
               btnScan_OnClick();
               break;
            case "btnScanNext":
               btnScanNext_OnClick();
               break;
         }
      }

      private void cmbBoxValueType_SelectedIndexChanged(Object sender, EventArgs e) {
         var newIndex = (sender as ComboBox).SelectedIndex;
         switch (newIndex) {
            case 0: // Byte
            case 1: // 2 Bytes
            case 2: // 4 Bytes
            case 3: // 8 Bytes
            case 4: // Float
            case 5: // Double
               if (curScanStatus == ScanStatus.FirstScan)
                  cmbBoxScanType.DataSource = ScanTypeOptions.listSearch_FirstScan;
               else
                  cmbBoxScanType.DataSource = ScanTypeOptions.listSearch_NextScan;
               if (newIndex == 4 || newIndex == 5)
                  ((List<String>)cmbBoxScanType.DataSource).Insert(1, "Fuzzy value");
               break;
            case 6: // String
            case 7: // Array of bytes
               cmbBoxScanType.DataSource = ScanTypeOptions.listSearchExactOnly;
               break;
         }
      }

      private void chkBoxFastScan_CheckedChanged(Object sender, EventArgs e) {
         SharedInformation.FastScan = (sender as CheckBox).Checked;
      }
   }
}
