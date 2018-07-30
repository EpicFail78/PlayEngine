using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Sockets;
using System.Threading;
using System.Linq;
using System.Windows.Forms;

using PlayEngine.Helpers;
using PlayEngine.Helpers.CheatManager;
using System.Runtime.InteropServices;

namespace PlayEngine.Forms {
   public partial class MainForm : Form {
      private enum ScanStatus {
         FirstScan,
         DidScan,
         Scanning
      }
      public static class ResultsColumnIndex {
         public static readonly Int32 iAddress = 0;
         public static readonly Int32 iSection = 1;
         public static readonly Int32 iValue = 2;
      }
      public static class SavedResultsColumnIndex {
         public static readonly Int32 iFreeze = 0;
         public static readonly Int32 iDescription = 1;
         public static readonly Int32 iAddress = 2;
         public static readonly Int32 iSection = 3;
         public static readonly Int32 iType = 4;
         public static readonly Int32 iValue = 5;
      }
      public static class ScanTypeOptions {
         public static readonly List<String> listSearch_FirstScan = new List<String>()
         {
            "Exact value",
            "Fuzzy value (float or double only)",
            "Bigger than...",
            "Smaller than...",
            "Value between...",
            "Unknown initial value"
         };
         public static readonly List<String> listSearch_NextScan = new List<String>()
         {
            "Exact value",
            "Fuzzy value (float or double only)",
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

         public static Memory.CompareType getCompareTypeFromString(String str) {
            switch (str) {
               case "Exact value":
                  return Memory.CompareType.ExactValue;
               case "Fuzzy value (float or double only)":
                  return Memory.CompareType.FuzzyValue;
               case "Increased value":
                  return Memory.CompareType.IncreasedValue;
               case "Increased value by...":
                  return Memory.CompareType.IncreasedValueBy;
               case "Decreased value":
                  return Memory.CompareType.DecreasedValue;
               case "Decreased value by...":
                  return Memory.CompareType.DecreasedValueBy;
               case "Bigger than...":
                  return Memory.CompareType.BiggerThan;
               case "Smaller than...":
                  return Memory.CompareType.SmallerThan;
               case "Value between...":
                  return Memory.CompareType.BetweenValues;
               case "Changed value":
                  return Memory.CompareType.ChangedValue;
               case "Unchanged value":
                  return Memory.CompareType.UnchangedValue;
               case "Unknown initial value":
                  return Memory.CompareType.UnknownInitialValue;
               default:
                  return Memory.CompareType.None;
            }
         }
         public static Type getValueTypeFromString(String str) {
            switch (str) {
               case "Byte":
                  return typeof(Byte);
               case "2 Bytes":
                  return typeof(UInt16);
               case "4 Bytes":
                  return typeof(UInt32);
               case "8 Bytes":
                  return typeof(UInt64);
               case "Float":
                  return typeof(Single);
               case "Double":
                  return typeof(Double);
               case "String":
                  return typeof(String);
               case "Array of bytes":
                  return typeof(Byte[]);
            }
            return null;
         }
      }
      private void setControlEnabled(Control[] arrControls, Boolean isEnabled) {
         foreach (Control cntrl in arrControls) {
            cntrl.Invoke(new Action(() => cntrl.Enabled = isEnabled));
         }
      }

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
                  setControlEnabled(new Control[] { btnScan, chkBoxIsHexValue, cmbBoxScanType, cmbBoxValueType, chkListViewSearchSections, listViewResults, txtBoxSectionsFilter }, true);
                  setControlEnabled(new Control[] { btnScanNext }, false);
                  this.Invoke(new Action(() => uiToolStrip_linkPayloadAndProcess.Enabled = true));

                  String strBackupSelectedItem = (String)cmbBoxScanType.SelectedItem;
                  cmbBoxScanType.Invoke(new Action(() => cmbBoxScanType.DataSource = ScanTypeOptions.listSearch_FirstScan));
                  cmbBoxScanType.Invoke(new Action(() => cmbBoxScanType.SelectedItem = strBackupSelectedItem));

                  btnScan.Invoke(new Action(() => btnScan.Text = "First Scan"));
                  this.Invoke(new Action(() => uiStatusStrip_lblStatus.Text = "Standby..."));
               }
               break;
               case ScanStatus.DidScan: {
                  setControlEnabled(new Control[] { btnScan, btnScanNext, chkBoxIsHexValue, cmbBoxScanType, listViewResults }, true);
                  setControlEnabled(new Control[] { cmbBoxValueType, chkListViewSearchSections, txtBoxSectionsFilter }, false);
                  this.Invoke(new Action(() => uiToolStrip_linkPayloadAndProcess.Enabled = true));

                  String strBackupSelectedItem = (String)cmbBoxScanType.SelectedItem;
                  cmbBoxScanType.Invoke(new Action(() => cmbBoxScanType.DataSource = ScanTypeOptions.listSearch_NextScan));
                  cmbBoxScanType.Invoke(new Action(() => cmbBoxScanType.SelectedItem = strBackupSelectedItem));

                  btnScan.Invoke(new Action(() => btnScan.Text = "New Scan"));
               }
               break;
               case ScanStatus.Scanning: {
                  setControlEnabled(new Control[] { btnScan }, true);
                  setControlEnabled(new Control[] { btnScanNext, chkBoxIsHexValue, cmbBoxScanType, cmbBoxValueType, chkListViewSearchSections, listViewResults, txtBoxSectionsFilter }, false);
                  this.Invoke(new Action(() => uiToolStrip_linkPayloadAndProcess.Enabled = false));

                  btnScan.Invoke(new Action(() => btnScan.Text = "Stop"));
                  this.Invoke(new Action(() => uiStatusStrip_lblStatus.Text = "Scanning..."));
               }
               break;
            }
         }
      }

      private librpc.ProcessInfo processInfo = null;
      private Memory.CompareType scanCompareType = Memory.CompareType.ExactValue;
      private Type scanValueType = typeof(UInt32);

      public MainForm() {
         InitializeComponent();
         this.Text = String.Format("PS4 Cheater v{0}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
         cmbBoxValueType.SelectedIndex = 2; // 4 Bytes

         using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)) {
            try {
               IAsyncResult result = socket.BeginConnect(Settings.mInstance.ps4.IPAddress, librpc.PS4RPC.RPC_PORT, null, null);
               result.AsyncWaitHandle.WaitOne(1000);
               if (socket.Connected)
                  uiToolStrip_PayloadManager_chkPayloadActive.Checked = Memory.initPS4RPC(Settings.mInstance.ps4.IPAddress);

               socket.Shutdown(SocketShutdown.Both);
               socket.Close();
            } catch (Exception) { }
         }
         bgWorkerResultsUpdater.RunWorkerAsync();
      }
      private void MainForm_Load(Object sender, EventArgs e) {
         if (uiToolStrip_PayloadManager_chkPayloadActive.Checked)
            btnRefreshProcessList_OnClick();
      }

      #region Functions
      public void addResult(String description, String sectionName, UInt32 sectionAddressOffset) {
         UInt64 runtimeAddress = 0;
         for (int i = 0; i < ProcessManager.mInstance.MappedSectionList.Count; i++) {
            if (ProcessManager.mInstance.MappedSectionList.GetSectionName(i).Contains(sectionName, StringComparison.OrdinalIgnoreCase)) {
               runtimeAddress = ProcessManager.mInstance.MappedSectionList[i].Start + sectionAddressOffset;
               break;
            }
         }
         var runtimeValue = Memory.read(processInfo.pid, runtimeAddress, scanValueType);

         DataGridViewRow row = dataGridSavedResults.Rows[dataGridSavedResults.Rows.Add()];
         row.Cells[SavedResultsColumnIndex.iDescription].Value = "No description";
         row.Cells[SavedResultsColumnIndex.iAddress].Value = runtimeAddress;
         row.Cells[SavedResultsColumnIndex.iSection].Value = sectionName;
         row.Cells[SavedResultsColumnIndex.iType].Value = (String)cmbBoxValueType.SelectedItem;
         row.Cells[SavedResultsColumnIndex.iValue].Value = runtimeValue;

         CheatInformation cheatInformation = new CheatInformation();
         cheatInformation.sectionAddressOffset = sectionAddressOffset;
         row.Tag = cheatInformation;
      }
      #endregion

      #region Buttons
      private void btnScan_OnClick() {
         try {
            switch (curScanStatus) {
               case ScanStatus.FirstScan:
                  if (MessageBox.Show(String.Format("Search size: {0}KB. Continue?", ProcessManager.mInstance.MappedSectionList.TotalMemorySize / 1024), "Scan", MessageBoxButtons.YesNo) != DialogResult.Yes)
                     return;

                  bgWorkerScanner.RunWorkerAsync(new Object[3] { txtBoxScanValue.Text, txtBoxScanValueSecond.Text, chkBoxIsHexValue.Checked });
                  break;
               case ScanStatus.DidScan:
                  listViewResults.Items.Clear();
                  ProcessManager.mInstance.MappedSectionList.ClearResultList();
                  curScanStatus = ScanStatus.FirstScan;
                  break;
               case ScanStatus.Scanning:
                  bgWorkerScanner.CancelAsync();
                  break;
            }
         } catch (Exception ex) {
            MessageBox.Show(ex.ToString(), "btnScan");
         }
      }
      private void btnScanNext_OnClick() {
         try {
            bgWorkerScanner.RunWorkerAsync(new Object[3] { txtBoxScanValue.Text, txtBoxScanValueSecond.Text, chkBoxIsHexValue.Checked });
         } catch (Exception ex) {
            MessageBox.Show(ex.ToString(), "btnScanNext");
         }
      }
      #region uiToolStrip_linkPayloadAndProcess
      private void btnSendPayload_OnClick() {
         if (new Forms.ChildForms.childFrmSendPayload().ShowDialog() == DialogResult.OK) {
            if (Memory.initPS4RPC(Settings.mInstance.ps4.IPAddress)) {
               uiToolStrip_PayloadManager_chkPayloadActive.Checked = true;
               btnRefreshProcessList_OnClick();
            }
         }
      }
      private void btnRefreshProcessList_OnClick() {
         try {
            uiToolStrip_ProcessManager_cmbBoxActiveProcess.Items.Clear();
            foreach (librpc.Process process in Memory.ps4RPC.GetProcessList().processes)
               uiToolStrip_ProcessManager_cmbBoxActiveProcess.Items.Add(process.name);
            uiToolStrip_ProcessManager_cmbBoxActiveProcess.SelectedIndex = 0;
         } catch (Exception ex) {
            MessageBox.Show(ex.ToString(), "Error during getting process list", MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
      }
      #endregion
      #region uiStatusStrip_linkSavedResults
      private void btnAddAddress_OnClick() {
         // TODO: UI to add entries manually
      }
      #endregion
      #region contextMenuChkListBox
      private void contextMenuChkListBox_btnSelectAll_OnClick() {
         foreach (ListViewItem item in chkListViewSearchSections.Items)
            item.Checked = contextMenuChkListBox_btnSelectAll.Checked;
      }
      #endregion
      private void uiButtonHandler_Click(Object sender, EventArgs e) {
         String btnName = sender.GetType() == typeof(Button) ? (sender as Button).Name : (sender as ToolStripMenuItem).Name;
         switch (btnName) {
            case "btnScan":
               btnScan_OnClick();
               break;
            case "btnScanNext":
               btnScanNext_OnClick();
               break;
            #region uiToolStrip_linkFile
            case "uiToolStrip_btnExit":
               Application.Exit();
               break;
            #endregion
            #region uiToolStrip_linkPayloadAndProcess
            case "uiToolStrip_PayloadManager_btnSendPayload":
               btnSendPayload_OnClick();
               break;
            case "uiToolStrip_ProcessManager_btnRefreshProcessList":
               btnRefreshProcessList_OnClick();
               break;
            #endregion
            #region uiStatusStrip_linkSavedResuls
            case "uiStatusStrip_SavedResults_btnAddAddress":
               btnAddAddress_OnClick();
               break;
            #endregion
            #region contextMenuChkListBox
            case "contextMenuChkListBox_btnSelectAll":
               contextMenuChkListBox_btnSelectAll_OnClick();
               break;
               #endregion
         }
      }
      #endregion

      private void cmbBoxValueType_SelectedIndexChanged(Object sender, EventArgs e) {
         var newIndex = cmbBoxValueType.SelectedIndex;
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
               break;
            case 6: // String
            case 7: // Array of bytes
               cmbBoxScanType.DataSource = ScanTypeOptions.listSearchExactOnly;
               break;
         }
         scanValueType = ScanTypeOptions.getValueTypeFromString((String)cmbBoxValueType.SelectedItem);
      }
      private void cmbBoxScanType_SelectedIndexChanged(Object sender, EventArgs e) {
         var newCompareType = ScanTypeOptions.getCompareTypeFromString((String)((sender as ComboBox).SelectedItem));
         scanCompareType = newCompareType;
         lblSecondValue.Enabled = txtBoxScanValueSecond.Enabled = newCompareType == Memory.CompareType.BetweenValues;
      }

      private void uiToolStrip_PayloadManager_chkPayloadActive_CheckedChanged(Object sender, EventArgs e) {
         Boolean isLoaded = uiToolStrip_PayloadManager_chkPayloadActive.Checked;
         splitContainerMain.Enabled = uiToolStrip_linkProcessManager.Enabled = isLoaded;
      }
      private void uiToolStrip_ProcessManager_cmbBoxActiveProcess_SelectedIndexChanged(Object sender, EventArgs e) {
         try {
            var comboBox = sender as ToolStripComboBox;
            if (comboBox.SelectedIndex < 1) {
               comboBox.SelectedIndex = 1;
               return;
            }
            String selectedProcessName = (String)uiToolStrip_ProcessManager_cmbBoxActiveProcess.SelectedItem;
            curScanStatus = ScanStatus.FirstScan;
            chkListViewSearchSections.Items.Clear();

            processInfo = ProcessManager.mInstance.GetProcessInfo(selectedProcessName);
            ProcessManager.mInstance.MappedSectionList.InitMemorySectionList(processInfo);

            for (int i = 0; i < ProcessManager.mInstance.MappedSectionList.Count; i++)
               chkListViewSearchSections.Items.Add(ProcessManager.mInstance.MappedSectionList.GetSectionName(i));
            uiToolStrip_lblActiveProcess.Text = String.Format("Process: {0}", selectedProcessName);
            //uiToolStrip_btnOpenPointerScanner.Enabled = true;
         } catch (Exception exception) {
            MessageBox.Show(exception.ToString());
         }
      }

      private void txtBoxSectionsFilter_TextChanged(Object sender, EventArgs e) {
         Int32 listViewIndex = 0;
         chkListViewSearchSections.Items.Clear();

         for (int i = 0; i < ProcessManager.mInstance.MappedSectionList.Count; i++) {
            String sectionName = ProcessManager.mInstance.MappedSectionList.GetSectionName(i);
            if (sectionName.Contains(txtBoxSectionsFilter.Text, StringComparison.OrdinalIgnoreCase)) {
               chkListViewSearchSections.Items.Add(sectionName);
               chkListViewSearchSections.Items[listViewIndex++].Checked = ProcessManager.mInstance.MappedSectionList[i].Check;
            }
         }
      }
      private void chkListBoxSearchSections_ItemCheck(Object sender, ItemCheckEventArgs e) {
         for (int i = 0; i < ProcessManager.mInstance.MappedSectionList.Count; i++) {
            String sectionName = ProcessManager.mInstance.MappedSectionList.GetSectionName(i);
            if (chkListViewSearchSections.Items[e.Index].Text == sectionName) {
               ProcessManager.mInstance.MappedSectionList.SectionCheck(i, e.NewValue == CheckState.Checked);
               break;
            }
         }
      }

      private void listViewResults_SaveSelectedEntries() {
         foreach (ListViewItem selectedEntry in listViewResults.SelectedItems) {
            String sectionName = selectedEntry.SubItems[1].Text;
            UInt32 sectionAddressOffset = (UInt32)selectedEntry.Tag;
            addResult("No description", sectionName, sectionAddressOffset);
         }
      }
      private void listViewResults_DoubleClick(Object sender, EventArgs e) {
         listViewResults_SaveSelectedEntries();
      }
      private void listViewResults_KeyDown(Object sender, KeyEventArgs e) {
         if (e.KeyCode == Keys.Enter)
            listViewResults_SaveSelectedEntries();
      }

      private void dataGridSavedResults_CellDoubleClick(Object sender, DataGridViewCellEventArgs e) {
         var cells = dataGridSavedResults.Rows[e.RowIndex].Cells;
         var cheatInformation = (CheatInformation)dataGridSavedResults.Rows[e.RowIndex].Tag;
         var frmEditInstance = new ChildForms.childFrmEditCheat(
            (String)cells[SavedResultsColumnIndex.iDescription].Value,
            (String)cells[SavedResultsColumnIndex.iSection].Value,
            cheatInformation.sectionAddressOffset,
            (String)cells[SavedResultsColumnIndex.iType].Value,
            cells[SavedResultsColumnIndex.iValue].Value.ToString()
         );
         if (frmEditInstance.ShowDialog() == DialogResult.OK) {
            var returnInformation = frmEditInstance.returnInformation;
            cells[SavedResultsColumnIndex.iDescription].Value = returnInformation.description;
            cells[SavedResultsColumnIndex.iSection].Value = returnInformation.sectionName;
            cheatInformation.sectionAddressOffset = returnInformation.sectionAddressOffset;
            cells[SavedResultsColumnIndex.iType].Value = returnInformation.valueType;
            cells[SavedResultsColumnIndex.iValue].Value = returnInformation.value;

            UInt64 runtimeAddress = 0;
            for (Int32 i = 0; i < ProcessManager.mInstance.MappedSectionList.Count; i++) {
               if (ProcessManager.mInstance.MappedSectionList.GetSectionName(i).Contains(returnInformation.sectionName, StringComparison.OrdinalIgnoreCase)) {
                  runtimeAddress = ProcessManager.mInstance.MappedSectionList[i].Start + returnInformation.sectionAddressOffset;
                  break;
               }
            }
            Memory.write(processInfo.pid, runtimeAddress, Convert.ChangeType(returnInformation.value, ScanTypeOptions.getValueTypeFromString(returnInformation.valueType)));
         }
      }

      #region Background Workers
      #region bgWorkerScanner
      private unsafe void bgWorkerScanner_DoWork(Object sender, DoWorkEventArgs e) {
         var oldScanStatus = curScanStatus;
         curScanStatus = ScanStatus.Scanning;
         UInt64 processedMemoryRange = 0;
         UInt64 totalMemoryRange = ProcessManager.mInstance.MappedSectionList.TotalMemorySize + 1;
         String[] scanValues = new String[2] { (String)((Object[])e.Argument)[0], (String)((Object[])e.Argument)[1] };
         bgWorkerScanner.ReportProgress(0);

         for (int section_idx = 0; section_idx < ProcessManager.mInstance.MappedSectionList.Count; section_idx++) {
            if (bgWorkerScanner.CancellationPending) {
               e.Cancel = true;
               break;
            }

            MappedSection mappedSection = ProcessManager.mInstance.MappedSectionList[section_idx];
            if (mappedSection.Check) {
               // Per section scan limits
               UInt64 maxResultCount = 1000, curResultCount = 0;
               var results = Memory.search(
                  Memory.readByteArray(processInfo.pid, mappedSection.Start, mappedSection.Length),
                  scanValues[0],
                  scanValueType,
                  scanCompareType,
                  new object[2] { scanValues[0], scanValues[1] }
               );
               foreach (UInt32 sectionAddressOffset in results) {
                  if (curResultCount > maxResultCount)
                     break;
                  if (listViewResults.Items.Count == 15)
                     listViewResults.Invoke(new Action(() => listViewResults.BeginUpdate()));
                  UInt64 runtimeAddress = mappedSection.Start + sectionAddressOffset;

                  ListViewItem listViewItem = new ListViewItem();
                  // Section Offset
                  listViewItem.Tag = sectionAddressOffset;
                  // Address
                  listViewItem.Text = runtimeAddress.ToString("X");
                  // Section
                  listViewItem.SubItems.Add(ProcessManager.mInstance.MappedSectionList.GetSectionName(section_idx));
                  // Value
                  listViewItem.SubItems.Add(Memory.read(processInfo.pid, runtimeAddress, scanValueType).ToString());

                  curResultCount++;
                  listViewResults.Invoke(new Action(() => listViewResults.Items.Add(listViewItem)));
                  if (bgWorkerScanner.CancellationPending)
                     break;
               }
               processedMemoryRange += (UInt64)mappedSection.Length;
               listViewResults.Invoke(new Action(() => uiStatusStrip_lblStatus.Text = String.Format("{0} results", listViewResults.Items.Count)));
            }
            bgWorkerScanner.ReportProgress((Int32)(
               ((float)processedMemoryRange / (float)totalMemoryRange)
               * 100));
         }
         bgWorkerScanner.ReportProgress(100);
      }
      private void bgWorkerScanner_ProgressChanged(Object sender, ProgressChangedEventArgs e) {
         uiToolStrip_progressBarScanPercent.Value = e.ProgressPercentage;
      }
      private void bgWorkerScanner_RunWorkerCompleted(Object sender, RunWorkerCompletedEventArgs e) {
         listViewResults.EndUpdate();
         curScanStatus = ScanStatus.DidScan;
         if (e.Error != null)
            uiStatusStrip_lblStatus.Text = e.Error.Message;
      }
      #endregion
      #region bgWorkerResultsUpdater
      private void bgWorkerResultsUpdater_DoWork(Object sender, DoWorkEventArgs e) {
         Thread.Sleep(1000);
         //foreach (DataGridViewRow row in dataGridSavedResults.Rows) {
         //   if (bgWorkerResultsUpdater.CancellationPending) {
         //      e.Cancel = true;
         //      return;
         //   }
         //   var runtimeValue = Memory.read(
         //      processInfo.pid,
         //      (UInt64)row.Cells[SavedResultsColumnIndex.iAddress].Value,
         //      (Type)row.Cells[SavedResultsColumnIndex.iType].Value
         //   );
         //   dataGridSavedResults.Invoke(new Action(() => dataGridSavedResults.Rows[row.Index].Cells[SavedResultsColumnIndex.iValue].Value = runtimeValue));
         //}
      }
      #endregion

      #endregion
   }
}
