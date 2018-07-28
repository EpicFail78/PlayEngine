using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;
using PS4_Cheater.Utils;

namespace PS4_Cheater.Forms {
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

         public static MemoryHelper.CompareType getCompareTypeFromString(String str) {
            switch (str) {
               case "Exact value":
                  return MemoryHelper.CompareType.ExactValue;
               case "Fuzzy value (float or double only)":
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
            cntrl.Invoke(new Action(() => cntrl.Enabled = isEnabled));
         }
      }

      ProcessManager processManager;
      MemoryHelper memoryHelper;
      CheatList cheatList;
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
                  setControlEnabled(new Control[] { btnScan, chkBoxIsHexValue, chkBoxFastScan, cmbBoxScanType, cmbBoxValueType, chkListBoxSearchSections, listViewResults, txtBoxSectionsFilter }, true);
                  setControlEnabled(new Control[] { btnScanNext }, false);
                  this.Invoke(new Action(() => uiToolStrip_linkPayloadAndProcess.Enabled = true));

                  String strBackupSelectedItem = (String)cmbBoxScanType.SelectedItem;
                  cmbBoxScanType.Invoke(new Action(() => cmbBoxScanType.DataSource = ScanTypeOptions.listSearch_FirstScan));
                  cmbBoxScanType.Invoke(new Action(() => cmbBoxScanType.SelectedItem = strBackupSelectedItem));

                  btnScan.Invoke(new Action(() => btnScan.Text = "First Scan"));
                  this.Invoke(new Action(() => uiStatusStrip_lblStatus.Text = "Standby..."));
                  bgWorkerResultsUpdater.CancelAsync();
               }
               break;
               case ScanStatus.DidScan: {
                  setControlEnabled(new Control[] { btnScan, btnScanNext, chkBoxIsHexValue, chkBoxFastScan, cmbBoxScanType, listViewResults }, true);
                  setControlEnabled(new Control[] { cmbBoxValueType, chkListBoxSearchSections, txtBoxSectionsFilter }, false);
                  this.Invoke(new Action(() => uiToolStrip_linkPayloadAndProcess.Enabled = true));

                  String strBackupSelectedItem = (String)cmbBoxScanType.SelectedItem;
                  cmbBoxScanType.Invoke(new Action(() => cmbBoxScanType.DataSource = ScanTypeOptions.listSearch_NextScan));
                  cmbBoxScanType.Invoke(new Action(() => cmbBoxScanType.SelectedItem = strBackupSelectedItem));

                  btnScan.Invoke(new Action(() => btnScan.Text = "New Scan"));
                  if (!bgWorkerResultsUpdater.IsBusy)
                     bgWorkerResultsUpdater.RunWorkerAsync();
               }
               break;
               case ScanStatus.Scanning: {
                  setControlEnabled(new Control[] { btnScan }, true);
                  setControlEnabled(new Control[] { btnScanNext, chkBoxIsHexValue, chkBoxFastScan, cmbBoxScanType, cmbBoxValueType, chkListBoxSearchSections, listViewResults, txtBoxSectionsFilter }, false);
                  this.Invoke(new Action(() => uiToolStrip_linkPayloadAndProcess.Enabled = false));

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

         using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)) {
            try {
               IAsyncResult result = socket.BeginConnect(Settings.mInstance.ps4.IPAddress, 9023, null, null);
               result.AsyncWaitHandle.WaitOne(1000);
               if (socket.Connected)
                  uiToolStrip_PayloadManager_chkPayloadActive.Checked = MemoryHelper.Connect(Settings.mInstance.ps4.IPAddress);

               socket.Shutdown(SocketShutdown.Both);
               socket.Close();
            } catch (Exception) { }
         }
      }
      private void MainForm_Load(Object sender, EventArgs e) {
         if (uiToolStrip_PayloadManager_chkPayloadActive.Checked)
            btnRefreshProcessList_OnClick();
      }

      #region Functions
      public void addResult() {

      }
      #endregion

      #region Buttons
      private void btnScan_OnClick() {
         try {
            switch (curScanStatus) {
               case ScanStatus.FirstScan:
                  if (MessageBox.Show(String.Format("Search size: {0}KB. Continue?", processManager.MappedSectionList.TotalMemorySize / 1024), "Scan", MessageBoxButtons.YesNo) != DialogResult.Yes)
                     return;

                  memoryHelper.InitMemoryHandler(
                     ScanTypeOptions.getValueTypeFromString((String)cmbBoxValueType.SelectedItem),
                     ScanTypeOptions.getCompareTypeFromString((String)cmbBoxScanType.SelectedItem),
                     Settings.mInstance.fastScanEnabled, txtBoxScanValue.Text.Length);

                  bgWorkerScanner.RunWorkerAsync(new Object[3] { txtBoxScanValue.Text, txtBoxScanValueSecond.Text, chkBoxIsHexValue.Checked });
                  break;
               case ScanStatus.DidScan:
                  listViewResults.Items.Clear();
                  processManager.MappedSectionList.ClearResultList();
                  curScanStatus = ScanStatus.FirstScan;
                  break;
               case ScanStatus.Scanning:
                  bgWorkerScanner.CancelAsync();
                  curScanStatus = ScanStatus.DidScan;
                  break;
            }
         } catch (Exception ex) {
            MessageBox.Show(ex.ToString(), "btnScan");
         }
      }
      private void btnScanNext_OnClick() {
         try {
            memoryHelper.InitNextScanMemoryHandler(ScanTypeOptions.getCompareTypeFromString((String)cmbBoxScanType.SelectedItem));
            bgWorkerScanner.RunWorkerAsync(new Object[3] { txtBoxScanValue.Text, txtBoxScanValueSecond.Text, chkBoxIsHexValue.Checked });
         } catch (Exception ex) {
            MessageBox.Show(ex.ToString(), "btnScanNext");
         }
      }
      #region uiToolStrip_linkPayloadAndProcess
      private void btnSendPayload_OnClick() {
         new Forms.ChildForms.childFrmSendPayload().ShowDialog();
         if (MemoryHelper.Connect(Settings.mInstance.ps4.IPAddress)) {
            uiToolStrip_PayloadManager_chkPayloadActive.Checked = true;
            btnRefreshProcessList_OnClick(true);
         }
      }
      private void btnRefreshProcessList_OnClick(Boolean suppressErrorMessage = false) {
         try {
            if (uiToolStrip_PayloadManager_chkPayloadActive.Checked) {
               uiToolStrip_ProcessManager_cmbBoxActiveProcess.Items.Clear();
               foreach (librpc.Process process in MemoryHelper.GetProcessList().processes)
                  uiToolStrip_ProcessManager_cmbBoxActiveProcess.Items.Add(process.name);
               uiToolStrip_ProcessManager_cmbBoxActiveProcess.SelectedIndex = 0;
            } else {
               if (!suppressErrorMessage)
                  MessageBox.Show("Payload is NOT injected!", "Error");
            }
         } catch (Exception ex) {
            MessageBox.Show(ex.ToString(), "Error during getting process list", MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
      }
      #endregion
      #region contextMenuChkListBox
      private void contextMenuChkListBox_btnSelectAll_OnClick() {
         for (int i = 0; i < chkListBoxSearchSections.Items.Count; i++)
            chkListBoxSearchSections.SetItemChecked(i, contextMenuChkListBox_btnSelectAll.Checked);
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
            #region contextMenuChkListBox
            case "contextMenuChkListBox_btnSelectAll":
               contextMenuChkListBox_btnSelectAll_OnClick();
               break;
               #endregion
         }
      }
      #endregion

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
               break;
            case 6: // String
            case 7: // Array of bytes
               cmbBoxScanType.DataSource = ScanTypeOptions.listSearchExactOnly;
               break;
         }
      }
      private void cmbBoxScanType_SelectedIndexChanged(Object sender, EventArgs e) {
         var newCompareType = ScanTypeOptions.getCompareTypeFromString((String)((sender as ComboBox).SelectedItem));
         lblSecondValue.Enabled = txtBoxScanValueSecond.Enabled = newCompareType == MemoryHelper.CompareType.BetweenValues;
      }
      private void uiToolStrip_ProcessManager_cmbBoxActiveProcess_SelectedIndexChanged(Object sender, EventArgs e) {
         try {
            String selectedProcessName = uiToolStrip_ProcessManager_cmbBoxActiveProcess.Text;
            curScanStatus = ScanStatus.FirstScan;
            chkListBoxSearchSections.Items.Clear();

            librpc.ProcessInfo processInfo = processManager.GetProcessInfo(selectedProcessName);
            Util.DefaultProcessID = processInfo.pid;
            processManager.MappedSectionList.InitMemorySectionList(processInfo);

            for (int i = 0; i < processManager.MappedSectionList.Count; i++)
               chkListBoxSearchSections.Items.Add(processManager.MappedSectionList.GetSectionName(i), false);
            uiToolStrip_lblActiveProcess.Text = String.Format("Process: {0}", selectedProcessName);
         } catch (Exception exception) {
            MessageBox.Show(exception.Message);
         }
      }
      private void uiToolStrip_PayloadManager_chkPayloadActive_CheckedChanged(Object sender, EventArgs e) {
         Boolean isLoaded = uiToolStrip_PayloadManager_chkPayloadActive.Checked;
         splitContainerMain.Enabled = uiToolStrip_btnOpenPointerScanner.Enabled = isLoaded;
      }
      private void chkBoxFastScan_CheckedChanged(Object sender, EventArgs e) {
         Settings.mInstance.fastScanEnabled = (sender as CheckBox).Checked;
         Settings.mInstance.saveToFile();
      }
      private void chkListBoxSearchSections_ItemCheck(Object sender, ItemCheckEventArgs e) {
         processManager.MappedSectionList.SectionCheck(e.Index, e.NewValue == CheckState.Checked);
      }
      private void listViewResults_DoubleClick(Object sender, EventArgs e) {
         var selectedItems = (sender as ListView).SelectedItems;
      }

      #region Background Workers
      #region bgWorkerScanner
      private void bgWorkerScanner_DoWork(Object sender, DoWorkEventArgs e) {
         var oldScanStatus = curScanStatus;
         curScanStatus = ScanStatus.Scanning;
         UInt64 processedMemoryRange = 0;
         UInt64 totalMemoryRange = processManager.MappedSectionList.TotalMemorySize + 1;
         String[] scanValues = new String[2] { (String)((Object[])e.Argument)[0], (String)((Object[])e.Argument)[1] };
         bgWorkerScanner.ReportProgress(0);

         for (int section_idx = 0; section_idx < processManager.MappedSectionList.Count; section_idx++) {
            if (bgWorkerScanner.CancellationPending) {
               e.Cancel = true;
               break;
            }

            MappedSection mappedSection = processManager.MappedSectionList[section_idx];
            mappedSection.UpdateResultList(processManager, memoryHelper, scanValues[0], scanValues[1], (Boolean)((Object[])e.Argument)[2], oldScanStatus == ScanStatus.FirstScan);

            if (mappedSection.Check) {
               processedMemoryRange += (UInt64)mappedSection.Length;
               ResultList resultList = mappedSection.ResultList;
               if (resultList != null) {
                  UInt64 maxResultCount = 2000, curResultCount = 0, totalResultCount = processManager.MappedSectionList.TotalResultCount();
                  for (resultList.Begin(); !resultList.End(); resultList.Next()) {
                     if (curResultCount >= maxResultCount) {
                        listViewResults.Invoke(new Action(() => uiStatusStrip_lblStatus.Text = String.Format("{0}+ results", curResultCount)));
                        break;
                     }

                     UInt32 addressSectionOffsett = 0;
                     Byte[] valueInMemory = null;
                     resultList.Get(ref addressSectionOffsett, ref valueInMemory);
                     UInt64 finalAddress = mappedSection.Start + addressSectionOffsett;

                     ListViewItem listViewItem = new ListViewItem();
                     // Section Offset
                     listViewItem.Tag = addressSectionOffsett;
                     // Address
                     listViewItem.Text = finalAddress.ToString("X");
                     // Section
                     listViewItem.SubItems.Add(processManager.MappedSectionList.GetSectionName(section_idx));
                     // Value
                     valueInMemory = memoryHelper.GetBytesFromAddress(finalAddress);
                     resultList.Set(valueInMemory);
                     listViewItem.SubItems.Add(memoryHelper.BytesToString(valueInMemory));

                     curResultCount++;
                     listViewResults.Invoke(new Action(() => listViewResults.Items.Add(listViewItem)));
                     if (bgWorkerScanner.CancellationPending)
                        break;
                  }
               }

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
         if (!e.Cancelled) {
            curScanStatus = ScanStatus.DidScan;
            if (e.Error != null)
               uiStatusStrip_lblStatus.Text = e.Error.Message;
         }
      }
      #endregion
      #region bgWorkerResultsUpdater
      private void bgWorkerResultsUpdater_DoWork(Object sender, DoWorkEventArgs e) {
         Thread.Sleep(1000);
         foreach (DataGridViewRow row in dataGridSavedResults.Rows) {
            if (bgWorkerResultsUpdater.CancellationPending) {
               e.Cancel = true;
               return;
            }
            var valueInMemory = memoryHelper.GetBytesFromAddress((UInt64)row.Cells[SavedResultsColumnIndex.Address].Value);
            var value = memoryHelper.BytesToString(valueInMemory);
            dataGridSavedResults.Invoke(new Action(() => dataGridSavedResults.Rows[row.Index].Cells[SavedResultsColumnIndex.Value].Value = value));
         }
      }
      #endregion
      #endregion

   }
}
