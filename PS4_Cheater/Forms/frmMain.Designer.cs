﻿namespace PS4_Cheater.Forms {
   partial class MainForm {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing) {
         if (disposing && (components != null)) {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent() {
         this.components = new System.ComponentModel.Container();
         this.uiToolStrip = new System.Windows.Forms.ToolStrip();
         this.uiToolStrip_linkFile = new System.Windows.Forms.ToolStripDropDownButton();
         this.uiToolStrip_btnLoadCheatTable = new System.Windows.Forms.ToolStripMenuItem();
         this.uiToolStrip_btnSaveCheatTable = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
         this.uiToolStrip_btnExit = new System.Windows.Forms.ToolStripMenuItem();
         this.uiToolStrip_linkPayloadAndProcess = new System.Windows.Forms.ToolStripDropDownButton();
         this.uiToolStrip_linkPayloadManager = new System.Windows.Forms.ToolStripMenuItem();
         this.uiToolStrip_PayloadManager_chkPayloadActive = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
         this.uiToolStrip_PayloadManager_btnSendPayload = new System.Windows.Forms.ToolStripMenuItem();
         this.uiToolStrip_linkProcessManager = new System.Windows.Forms.ToolStripMenuItem();
         this.uiToolStrip_ProcessManager_btnRefreshProcessList = new System.Windows.Forms.ToolStripMenuItem();
         this.uiToolStrip_ProcessManager_cmbBoxActiveProcess = new System.Windows.Forms.ToolStripComboBox();
         this.uiToolStrip_linkTools = new System.Windows.Forms.ToolStripDropDownButton();
         this.uiToolStrip_btnOpenPointerScanner = new System.Windows.Forms.ToolStripMenuItem();
         this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
         this.uiToolStrip_progressBarScanPercent = new System.Windows.Forms.ToolStripProgressBar();
         this.uiToolStrip_lblActiveProcess = new System.Windows.Forms.ToolStripLabel();
         this.splitContainerMain = new System.Windows.Forms.SplitContainer();
         this.splitContainerScanner = new System.Windows.Forms.SplitContainer();
         this.listViewResults = new System.Windows.Forms.ListView();
         this.columnHeaderAddress = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.columnHeaderSection = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.columnHeaderValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.splitContainerScanDetails = new System.Windows.Forms.SplitContainer();
         this.txtBoxSectionsFilter = new System.Windows.Forms.TextBox();
         this.label2 = new System.Windows.Forms.Label();
         this.txtBoxScanValueSecond = new System.Windows.Forms.TextBox();
         this.lblSecondValue = new System.Windows.Forms.Label();
         this.chkBoxFastScan = new System.Windows.Forms.CheckBox();
         this.label3 = new System.Windows.Forms.Label();
         this.cmbBoxValueType = new System.Windows.Forms.ComboBox();
         this.cmbBoxScanType = new System.Windows.Forms.ComboBox();
         this.txtBoxScanValue = new System.Windows.Forms.TextBox();
         this.chkBoxIsHexValue = new System.Windows.Forms.CheckBox();
         this.btnScanNext = new System.Windows.Forms.Button();
         this.btnScan = new System.Windows.Forms.Button();
         this.chkListBoxSearchSections = new System.Windows.Forms.CheckedListBox();
         this.dataGridSavedResults = new System.Windows.Forms.DataGridView();
         this.uiStatusStrip = new System.Windows.Forms.StatusStrip();
         this.uiStatusStrip_linkEntryManager = new System.Windows.Forms.ToolStripDropDownButton();
         this.uiStatusStrip_EntryManager_btnAddAddress = new System.Windows.Forms.ToolStripMenuItem();
         this.uiStatusStrip_EntryManager_btnAddPointer = new System.Windows.Forms.ToolStripMenuItem();
         this.uiStatusStrip_lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
         this.bgWorkerScanner = new System.ComponentModel.BackgroundWorker();
         this.bgWorkerResultsUpdater = new System.ComponentModel.BackgroundWorker();
         this.contextMenuChkListBox = new System.Windows.Forms.ContextMenuStrip(this.components);
         this.contextMenuChkListBox_btnSelectAll = new System.Windows.Forms.ToolStripMenuItem();
         this.dataGridSavedResults_chkBoxFreezeValue = new System.Windows.Forms.DataGridViewCheckBoxColumn();
         this.dataGridSavedResults_txtBoxDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
         this.dataGridSavedResults_txtBoxAddress = new System.Windows.Forms.DataGridViewTextBoxColumn();
         this.dataGridSavedResults_txtBoxSection = new System.Windows.Forms.DataGridViewTextBoxColumn();
         this.dataGridSavedResults_txtBoxValueType = new System.Windows.Forms.DataGridViewTextBoxColumn();
         this.dataGridSavedResults_txtBoxValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
         this.uiToolStrip.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
         this.splitContainerMain.Panel1.SuspendLayout();
         this.splitContainerMain.Panel2.SuspendLayout();
         this.splitContainerMain.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainerScanner)).BeginInit();
         this.splitContainerScanner.Panel1.SuspendLayout();
         this.splitContainerScanner.Panel2.SuspendLayout();
         this.splitContainerScanner.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainerScanDetails)).BeginInit();
         this.splitContainerScanDetails.Panel1.SuspendLayout();
         this.splitContainerScanDetails.Panel2.SuspendLayout();
         this.splitContainerScanDetails.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.dataGridSavedResults)).BeginInit();
         this.uiStatusStrip.SuspendLayout();
         this.contextMenuChkListBox.SuspendLayout();
         this.SuspendLayout();
         // 
         // uiToolStrip
         // 
         this.uiToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
         this.uiToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uiToolStrip_linkFile,
            this.uiToolStrip_linkPayloadAndProcess,
            this.uiToolStrip_linkTools,
            this.toolStripSeparator3,
            this.uiToolStrip_progressBarScanPercent,
            this.uiToolStrip_lblActiveProcess});
         this.uiToolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
         this.uiToolStrip.Location = new System.Drawing.Point(0, 0);
         this.uiToolStrip.Name = "uiToolStrip";
         this.uiToolStrip.Size = new System.Drawing.Size(484, 25);
         this.uiToolStrip.TabIndex = 0;
         // 
         // uiToolStrip_linkFile
         // 
         this.uiToolStrip_linkFile.AutoToolTip = false;
         this.uiToolStrip_linkFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
         this.uiToolStrip_linkFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uiToolStrip_btnLoadCheatTable,
            this.uiToolStrip_btnSaveCheatTable,
            this.toolStripSeparator1,
            this.uiToolStrip_btnExit});
         this.uiToolStrip_linkFile.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.uiToolStrip_linkFile.Name = "uiToolStrip_linkFile";
         this.uiToolStrip_linkFile.Size = new System.Drawing.Size(38, 22);
         this.uiToolStrip_linkFile.Text = "File";
         // 
         // uiToolStrip_btnLoadCheatTable
         // 
         this.uiToolStrip_btnLoadCheatTable.Name = "uiToolStrip_btnLoadCheatTable";
         this.uiToolStrip_btnLoadCheatTable.Size = new System.Drawing.Size(170, 22);
         this.uiToolStrip_btnLoadCheatTable.Text = "Load cheat table...";
         this.uiToolStrip_btnLoadCheatTable.Click += new System.EventHandler(this.uiButtonHandler_Click);
         // 
         // uiToolStrip_btnSaveCheatTable
         // 
         this.uiToolStrip_btnSaveCheatTable.Name = "uiToolStrip_btnSaveCheatTable";
         this.uiToolStrip_btnSaveCheatTable.Size = new System.Drawing.Size(170, 22);
         this.uiToolStrip_btnSaveCheatTable.Text = "Save cheat table...";
         this.uiToolStrip_btnSaveCheatTable.Click += new System.EventHandler(this.uiButtonHandler_Click);
         // 
         // toolStripSeparator1
         // 
         this.toolStripSeparator1.Name = "toolStripSeparator1";
         this.toolStripSeparator1.Size = new System.Drawing.Size(167, 6);
         // 
         // uiToolStrip_btnExit
         // 
         this.uiToolStrip_btnExit.Name = "uiToolStrip_btnExit";
         this.uiToolStrip_btnExit.Size = new System.Drawing.Size(170, 22);
         this.uiToolStrip_btnExit.Text = "Exit";
         this.uiToolStrip_btnExit.Click += new System.EventHandler(this.uiButtonHandler_Click);
         // 
         // uiToolStrip_linkPayloadAndProcess
         // 
         this.uiToolStrip_linkPayloadAndProcess.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
         this.uiToolStrip_linkPayloadAndProcess.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uiToolStrip_linkPayloadManager,
            this.uiToolStrip_linkProcessManager});
         this.uiToolStrip_linkPayloadAndProcess.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.uiToolStrip_linkPayloadAndProcess.Name = "uiToolStrip_linkPayloadAndProcess";
         this.uiToolStrip_linkPayloadAndProcess.Size = new System.Drawing.Size(113, 22);
         this.uiToolStrip_linkPayloadAndProcess.Text = "Payload / Process";
         // 
         // uiToolStrip_linkPayloadManager
         // 
         this.uiToolStrip_linkPayloadManager.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uiToolStrip_PayloadManager_chkPayloadActive,
            this.toolStripSeparator2,
            this.uiToolStrip_PayloadManager_btnSendPayload});
         this.uiToolStrip_linkPayloadManager.Name = "uiToolStrip_linkPayloadManager";
         this.uiToolStrip_linkPayloadManager.Size = new System.Drawing.Size(180, 22);
         this.uiToolStrip_linkPayloadManager.Text = "Payload";
         // 
         // uiToolStrip_PayloadManager_chkPayloadActive
         // 
         this.uiToolStrip_PayloadManager_chkPayloadActive.Enabled = false;
         this.uiToolStrip_PayloadManager_chkPayloadActive.Name = "uiToolStrip_PayloadManager_chkPayloadActive";
         this.uiToolStrip_PayloadManager_chkPayloadActive.Size = new System.Drawing.Size(180, 22);
         this.uiToolStrip_PayloadManager_chkPayloadActive.Text = "Payload active?";
         this.uiToolStrip_PayloadManager_chkPayloadActive.CheckedChanged += new System.EventHandler(this.uiToolStrip_PayloadManager_chkPayloadActive_CheckedChanged);
         // 
         // toolStripSeparator2
         // 
         this.toolStripSeparator2.Name = "toolStripSeparator2";
         this.toolStripSeparator2.Size = new System.Drawing.Size(177, 6);
         // 
         // uiToolStrip_PayloadManager_btnSendPayload
         // 
         this.uiToolStrip_PayloadManager_btnSendPayload.Name = "uiToolStrip_PayloadManager_btnSendPayload";
         this.uiToolStrip_PayloadManager_btnSendPayload.Size = new System.Drawing.Size(180, 22);
         this.uiToolStrip_PayloadManager_btnSendPayload.Text = "Send payload";
         this.uiToolStrip_PayloadManager_btnSendPayload.Click += new System.EventHandler(this.uiButtonHandler_Click);
         // 
         // uiToolStrip_linkProcessManager
         // 
         this.uiToolStrip_linkProcessManager.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uiToolStrip_ProcessManager_btnRefreshProcessList,
            this.uiToolStrip_ProcessManager_cmbBoxActiveProcess});
         this.uiToolStrip_linkProcessManager.Name = "uiToolStrip_linkProcessManager";
         this.uiToolStrip_linkProcessManager.Size = new System.Drawing.Size(180, 22);
         this.uiToolStrip_linkProcessManager.Text = "Process";
         // 
         // uiToolStrip_ProcessManager_btnRefreshProcessList
         // 
         this.uiToolStrip_ProcessManager_btnRefreshProcessList.Name = "uiToolStrip_ProcessManager_btnRefreshProcessList";
         this.uiToolStrip_ProcessManager_btnRefreshProcessList.Size = new System.Drawing.Size(181, 22);
         this.uiToolStrip_ProcessManager_btnRefreshProcessList.Text = "Refresh process list";
         this.uiToolStrip_ProcessManager_btnRefreshProcessList.Click += new System.EventHandler(this.uiButtonHandler_Click);
         // 
         // uiToolStrip_ProcessManager_cmbBoxActiveProcess
         // 
         this.uiToolStrip_ProcessManager_cmbBoxActiveProcess.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
         this.uiToolStrip_ProcessManager_cmbBoxActiveProcess.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
         this.uiToolStrip_ProcessManager_cmbBoxActiveProcess.MaxDropDownItems = 12;
         this.uiToolStrip_ProcessManager_cmbBoxActiveProcess.Name = "uiToolStrip_ProcessManager_cmbBoxActiveProcess";
         this.uiToolStrip_ProcessManager_cmbBoxActiveProcess.Size = new System.Drawing.Size(121, 23);
         this.uiToolStrip_ProcessManager_cmbBoxActiveProcess.Text = "Active process";
         this.uiToolStrip_ProcessManager_cmbBoxActiveProcess.SelectedIndexChanged += new System.EventHandler(this.uiToolStrip_ProcessManager_cmbBoxActiveProcess_SelectedIndexChanged);
         // 
         // uiToolStrip_linkTools
         // 
         this.uiToolStrip_linkTools.AutoToolTip = false;
         this.uiToolStrip_linkTools.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
         this.uiToolStrip_linkTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uiToolStrip_btnOpenPointerScanner});
         this.uiToolStrip_linkTools.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.uiToolStrip_linkTools.Name = "uiToolStrip_linkTools";
         this.uiToolStrip_linkTools.Size = new System.Drawing.Size(48, 22);
         this.uiToolStrip_linkTools.Text = "Tools";
         // 
         // uiToolStrip_btnOpenPointerScanner
         // 
         this.uiToolStrip_btnOpenPointerScanner.Enabled = false;
         this.uiToolStrip_btnOpenPointerScanner.Name = "uiToolStrip_btnOpenPointerScanner";
         this.uiToolStrip_btnOpenPointerScanner.Size = new System.Drawing.Size(189, 22);
         this.uiToolStrip_btnOpenPointerScanner.Text = "Open Pointer Scanner";
         this.uiToolStrip_btnOpenPointerScanner.Click += new System.EventHandler(this.uiButtonHandler_Click);
         // 
         // toolStripSeparator3
         // 
         this.toolStripSeparator3.Name = "toolStripSeparator3";
         this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
         // 
         // uiToolStrip_progressBarScanPercent
         // 
         this.uiToolStrip_progressBarScanPercent.AutoSize = false;
         this.uiToolStrip_progressBarScanPercent.Name = "uiToolStrip_progressBarScanPercent";
         this.uiToolStrip_progressBarScanPercent.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
         this.uiToolStrip_progressBarScanPercent.Size = new System.Drawing.Size(100, 17);
         // 
         // uiToolStrip_lblActiveProcess
         // 
         this.uiToolStrip_lblActiveProcess.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
         this.uiToolStrip_lblActiveProcess.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
         this.uiToolStrip_lblActiveProcess.Name = "uiToolStrip_lblActiveProcess";
         this.uiToolStrip_lblActiveProcess.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
         this.uiToolStrip_lblActiveProcess.Size = new System.Drawing.Size(86, 22);
         this.uiToolStrip_lblActiveProcess.Text = "Process: NONE";
         this.uiToolStrip_lblActiveProcess.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
         // 
         // splitContainerMain
         // 
         this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
         this.splitContainerMain.Enabled = false;
         this.splitContainerMain.Location = new System.Drawing.Point(0, 25);
         this.splitContainerMain.Name = "splitContainerMain";
         this.splitContainerMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
         // 
         // splitContainerMain.Panel1
         // 
         this.splitContainerMain.Panel1.Controls.Add(this.splitContainerScanner);
         this.splitContainerMain.Panel1MinSize = 315;
         // 
         // splitContainerMain.Panel2
         // 
         this.splitContainerMain.Panel2.Controls.Add(this.dataGridSavedResults);
         this.splitContainerMain.Panel2MinSize = 50;
         this.splitContainerMain.Size = new System.Drawing.Size(484, 404);
         this.splitContainerMain.SplitterDistance = 315;
         this.splitContainerMain.SplitterWidth = 3;
         this.splitContainerMain.TabIndex = 1;
         // 
         // splitContainerScanner
         // 
         this.splitContainerScanner.Dock = System.Windows.Forms.DockStyle.Fill;
         this.splitContainerScanner.IsSplitterFixed = true;
         this.splitContainerScanner.Location = new System.Drawing.Point(0, 0);
         this.splitContainerScanner.Name = "splitContainerScanner";
         // 
         // splitContainerScanner.Panel1
         // 
         this.splitContainerScanner.Panel1.Controls.Add(this.listViewResults);
         this.splitContainerScanner.Panel1MinSize = 150;
         // 
         // splitContainerScanner.Panel2
         // 
         this.splitContainerScanner.Panel2.Controls.Add(this.splitContainerScanDetails);
         this.splitContainerScanner.Panel2.Padding = new System.Windows.Forms.Padding(0, 0, 1, 0);
         this.splitContainerScanner.Panel2MinSize = 160;
         this.splitContainerScanner.Size = new System.Drawing.Size(484, 315);
         this.splitContainerScanner.SplitterDistance = 211;
         this.splitContainerScanner.SplitterWidth = 1;
         this.splitContainerScanner.TabIndex = 0;
         // 
         // listViewResults
         // 
         this.listViewResults.AutoArrange = false;
         this.listViewResults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderAddress,
            this.columnHeaderSection,
            this.columnHeaderValue});
         this.listViewResults.Dock = System.Windows.Forms.DockStyle.Fill;
         this.listViewResults.FullRowSelect = true;
         this.listViewResults.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
         this.listViewResults.HideSelection = false;
         this.listViewResults.Location = new System.Drawing.Point(0, 0);
         this.listViewResults.Name = "listViewResults";
         this.listViewResults.ShowItemToolTips = true;
         this.listViewResults.Size = new System.Drawing.Size(211, 315);
         this.listViewResults.TabIndex = 0;
         this.listViewResults.UseCompatibleStateImageBehavior = false;
         this.listViewResults.View = System.Windows.Forms.View.Details;
         // 
         // columnHeaderAddress
         // 
         this.columnHeaderAddress.Text = "Address";
         this.columnHeaderAddress.Width = 85;
         // 
         // columnHeaderSection
         // 
         this.columnHeaderSection.Text = "Section";
         this.columnHeaderSection.Width = 74;
         // 
         // columnHeaderValue
         // 
         this.columnHeaderValue.Text = "Value";
         this.columnHeaderValue.Width = 50;
         // 
         // splitContainerScanDetails
         // 
         this.splitContainerScanDetails.Dock = System.Windows.Forms.DockStyle.Fill;
         this.splitContainerScanDetails.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
         this.splitContainerScanDetails.IsSplitterFixed = true;
         this.splitContainerScanDetails.Location = new System.Drawing.Point(0, 0);
         this.splitContainerScanDetails.Name = "splitContainerScanDetails";
         this.splitContainerScanDetails.Orientation = System.Windows.Forms.Orientation.Horizontal;
         // 
         // splitContainerScanDetails.Panel1
         // 
         this.splitContainerScanDetails.Panel1.Controls.Add(this.txtBoxSectionsFilter);
         this.splitContainerScanDetails.Panel1.Controls.Add(this.label2);
         this.splitContainerScanDetails.Panel1.Controls.Add(this.txtBoxScanValueSecond);
         this.splitContainerScanDetails.Panel1.Controls.Add(this.lblSecondValue);
         this.splitContainerScanDetails.Panel1.Controls.Add(this.chkBoxFastScan);
         this.splitContainerScanDetails.Panel1.Controls.Add(this.label3);
         this.splitContainerScanDetails.Panel1.Controls.Add(this.cmbBoxValueType);
         this.splitContainerScanDetails.Panel1.Controls.Add(this.cmbBoxScanType);
         this.splitContainerScanDetails.Panel1.Controls.Add(this.txtBoxScanValue);
         this.splitContainerScanDetails.Panel1.Controls.Add(this.chkBoxIsHexValue);
         this.splitContainerScanDetails.Panel1.Controls.Add(this.btnScanNext);
         this.splitContainerScanDetails.Panel1.Controls.Add(this.btnScan);
         this.splitContainerScanDetails.Panel1MinSize = 163;
         // 
         // splitContainerScanDetails.Panel2
         // 
         this.splitContainerScanDetails.Panel2.Controls.Add(this.chkListBoxSearchSections);
         this.splitContainerScanDetails.Panel2.Padding = new System.Windows.Forms.Padding(0, 0, 4, 0);
         this.splitContainerScanDetails.Size = new System.Drawing.Size(271, 315);
         this.splitContainerScanDetails.SplitterDistance = 163;
         this.splitContainerScanDetails.SplitterWidth = 1;
         this.splitContainerScanDetails.TabIndex = 0;
         // 
         // txtBoxSectionsFilter
         // 
         this.txtBoxSectionsFilter.Location = new System.Drawing.Point(81, 138);
         this.txtBoxSectionsFilter.Name = "txtBoxSectionsFilter";
         this.txtBoxSectionsFilter.Size = new System.Drawing.Size(179, 23);
         this.txtBoxSectionsFilter.TabIndex = 23;
         // 
         // label2
         // 
         this.label2.AutoSize = true;
         this.label2.Location = new System.Drawing.Point(6, 141);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(69, 15);
         this.label2.TabIndex = 22;
         this.label2.Text = "Name filter:";
         // 
         // txtBoxScanValueSecond
         // 
         this.txtBoxScanValueSecond.Enabled = false;
         this.txtBoxScanValueSecond.Location = new System.Drawing.Point(73, 64);
         this.txtBoxScanValueSecond.Name = "txtBoxScanValueSecond";
         this.txtBoxScanValueSecond.Size = new System.Drawing.Size(187, 23);
         this.txtBoxScanValueSecond.TabIndex = 21;
         // 
         // lblSecondValue
         // 
         this.lblSecondValue.AutoSize = true;
         this.lblSecondValue.Enabled = false;
         this.lblSecondValue.Location = new System.Drawing.Point(6, 67);
         this.lblSecondValue.Name = "lblSecondValue";
         this.lblSecondValue.Size = new System.Drawing.Size(61, 15);
         this.lblSecondValue.TabIndex = 20;
         this.lblSecondValue.Text = "2nd value:";
         // 
         // chkBoxFastScan
         // 
         this.chkBoxFastScan.AutoSize = true;
         this.chkBoxFastScan.Location = new System.Drawing.Point(166, 10);
         this.chkBoxFastScan.Name = "chkBoxFastScan";
         this.chkBoxFastScan.Size = new System.Drawing.Size(75, 19);
         this.chkBoxFastScan.TabIndex = 19;
         this.chkBoxFastScan.Text = "Fast Scan";
         this.chkBoxFastScan.UseVisualStyleBackColor = true;
         this.chkBoxFastScan.CheckedChanged += new System.EventHandler(this.chkBoxFastScan_CheckedChanged);
         // 
         // label3
         // 
         this.label3.AutoSize = true;
         this.label3.Location = new System.Drawing.Point(6, 120);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(118, 15);
         this.label3.TabIndex = 18;
         this.label3.Text = "Sections to search in:";
         // 
         // cmbBoxValueType
         // 
         this.cmbBoxValueType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cmbBoxValueType.FormattingEnabled = true;
         this.cmbBoxValueType.Items.AddRange(new object[] {
            "Byte",
            "2 Bytes",
            "4 Bytes",
            "8 Bytes",
            "Float",
            "Double",
            "String",
            "Array of bytes"});
         this.cmbBoxValueType.Location = new System.Drawing.Point(9, 93);
         this.cmbBoxValueType.Name = "cmbBoxValueType";
         this.cmbBoxValueType.Size = new System.Drawing.Size(115, 23);
         this.cmbBoxValueType.TabIndex = 17;
         this.cmbBoxValueType.SelectedIndexChanged += new System.EventHandler(this.cmbBoxValueType_SelectedIndexChanged);
         // 
         // cmbBoxScanType
         // 
         this.cmbBoxScanType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cmbBoxScanType.FormattingEnabled = true;
         this.cmbBoxScanType.Location = new System.Drawing.Point(130, 93);
         this.cmbBoxScanType.Name = "cmbBoxScanType";
         this.cmbBoxScanType.Size = new System.Drawing.Size(130, 23);
         this.cmbBoxScanType.TabIndex = 16;
         this.cmbBoxScanType.SelectedIndexChanged += new System.EventHandler(this.cmbBoxScanType_SelectedIndexChanged);
         // 
         // txtBoxScanValue
         // 
         this.txtBoxScanValue.Location = new System.Drawing.Point(73, 35);
         this.txtBoxScanValue.Name = "txtBoxScanValue";
         this.txtBoxScanValue.Size = new System.Drawing.Size(187, 23);
         this.txtBoxScanValue.TabIndex = 13;
         // 
         // chkBoxIsHexValue
         // 
         this.chkBoxIsHexValue.AutoSize = true;
         this.chkBoxIsHexValue.Location = new System.Drawing.Point(9, 37);
         this.chkBoxIsHexValue.Name = "chkBoxIsHexValue";
         this.chkBoxIsHexValue.Size = new System.Drawing.Size(46, 19);
         this.chkBoxIsHexValue.TabIndex = 12;
         this.chkBoxIsHexValue.Text = "Hex";
         this.chkBoxIsHexValue.UseVisualStyleBackColor = true;
         // 
         // btnScanNext
         // 
         this.btnScanNext.Location = new System.Drawing.Point(87, 8);
         this.btnScanNext.Name = "btnScanNext";
         this.btnScanNext.Size = new System.Drawing.Size(75, 23);
         this.btnScanNext.TabIndex = 11;
         this.btnScanNext.Text = "Next Scan";
         this.btnScanNext.UseVisualStyleBackColor = true;
         this.btnScanNext.Click += new System.EventHandler(this.uiButtonHandler_Click);
         // 
         // btnScan
         // 
         this.btnScan.Location = new System.Drawing.Point(6, 8);
         this.btnScan.Name = "btnScan";
         this.btnScan.Size = new System.Drawing.Size(75, 23);
         this.btnScan.TabIndex = 10;
         this.btnScan.Text = "First Scan";
         this.btnScan.UseVisualStyleBackColor = true;
         this.btnScan.Click += new System.EventHandler(this.uiButtonHandler_Click);
         // 
         // chkListBoxSearchSections
         // 
         this.chkListBoxSearchSections.ContextMenuStrip = this.contextMenuChkListBox;
         this.chkListBoxSearchSections.Dock = System.Windows.Forms.DockStyle.Fill;
         this.chkListBoxSearchSections.FormattingEnabled = true;
         this.chkListBoxSearchSections.HorizontalScrollbar = true;
         this.chkListBoxSearchSections.IntegralHeight = false;
         this.chkListBoxSearchSections.Location = new System.Drawing.Point(0, 0);
         this.chkListBoxSearchSections.Name = "chkListBoxSearchSections";
         this.chkListBoxSearchSections.Size = new System.Drawing.Size(267, 151);
         this.chkListBoxSearchSections.TabIndex = 0;
         this.chkListBoxSearchSections.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.chkListBoxSearchSections_ItemCheck);
         // 
         // dataGridSavedResults
         // 
         this.dataGridSavedResults.AllowUserToAddRows = false;
         this.dataGridSavedResults.AllowUserToResizeRows = false;
         this.dataGridSavedResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
         this.dataGridSavedResults.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridSavedResults_chkBoxFreezeValue,
            this.dataGridSavedResults_txtBoxDescription,
            this.dataGridSavedResults_txtBoxAddress,
            this.dataGridSavedResults_txtBoxSection,
            this.dataGridSavedResults_txtBoxValueType,
            this.dataGridSavedResults_txtBoxValue});
         this.dataGridSavedResults.Dock = System.Windows.Forms.DockStyle.Fill;
         this.dataGridSavedResults.Location = new System.Drawing.Point(0, 0);
         this.dataGridSavedResults.Name = "dataGridSavedResults";
         this.dataGridSavedResults.RowHeadersVisible = false;
         this.dataGridSavedResults.RowTemplate.Height = 23;
         this.dataGridSavedResults.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
         this.dataGridSavedResults.Size = new System.Drawing.Size(484, 86);
         this.dataGridSavedResults.TabIndex = 51;
         this.dataGridSavedResults.TabStop = false;
         // 
         // uiStatusStrip
         // 
         this.uiStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uiStatusStrip_linkEntryManager,
            this.uiStatusStrip_lblStatus});
         this.uiStatusStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
         this.uiStatusStrip.Location = new System.Drawing.Point(0, 429);
         this.uiStatusStrip.Name = "uiStatusStrip";
         this.uiStatusStrip.Size = new System.Drawing.Size(484, 22);
         this.uiStatusStrip.TabIndex = 52;
         // 
         // uiStatusStrip_linkEntryManager
         // 
         this.uiStatusStrip_linkEntryManager.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
         this.uiStatusStrip_linkEntryManager.AutoToolTip = false;
         this.uiStatusStrip_linkEntryManager.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
         this.uiStatusStrip_linkEntryManager.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uiStatusStrip_EntryManager_btnAddAddress,
            this.uiStatusStrip_EntryManager_btnAddPointer});
         this.uiStatusStrip_linkEntryManager.ImageTransparentColor = System.Drawing.Color.Magenta;
         this.uiStatusStrip_linkEntryManager.Name = "uiStatusStrip_linkEntryManager";
         this.uiStatusStrip_linkEntryManager.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
         this.uiStatusStrip_linkEntryManager.Size = new System.Drawing.Size(133, 20);
         this.uiStatusStrip_linkEntryManager.Text = "Add entry manually...";
         this.uiStatusStrip_linkEntryManager.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
         // 
         // uiStatusStrip_EntryManager_btnAddAddress
         // 
         this.uiStatusStrip_EntryManager_btnAddAddress.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
         this.uiStatusStrip_EntryManager_btnAddAddress.Name = "uiStatusStrip_EntryManager_btnAddAddress";
         this.uiStatusStrip_EntryManager_btnAddAddress.Size = new System.Drawing.Size(116, 22);
         this.uiStatusStrip_EntryManager_btnAddAddress.Text = "Address";
         this.uiStatusStrip_EntryManager_btnAddAddress.Click += new System.EventHandler(this.uiButtonHandler_Click);
         // 
         // uiStatusStrip_EntryManager_btnAddPointer
         // 
         this.uiStatusStrip_EntryManager_btnAddPointer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
         this.uiStatusStrip_EntryManager_btnAddPointer.Name = "uiStatusStrip_EntryManager_btnAddPointer";
         this.uiStatusStrip_EntryManager_btnAddPointer.Size = new System.Drawing.Size(116, 22);
         this.uiStatusStrip_EntryManager_btnAddPointer.Text = "Pointer";
         this.uiStatusStrip_EntryManager_btnAddPointer.Click += new System.EventHandler(this.uiButtonHandler_Click);
         // 
         // uiStatusStrip_lblStatus
         // 
         this.uiStatusStrip_lblStatus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
         this.uiStatusStrip_lblStatus.Name = "uiStatusStrip_lblStatus";
         this.uiStatusStrip_lblStatus.Size = new System.Drawing.Size(59, 17);
         this.uiStatusStrip_lblStatus.Text = "Standby...";
         // 
         // bgWorkerScanner
         // 
         this.bgWorkerScanner.WorkerReportsProgress = true;
         this.bgWorkerScanner.WorkerSupportsCancellation = true;
         this.bgWorkerScanner.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgWorkerScanner_DoWork);
         this.bgWorkerScanner.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgWorkerScanner_ProgressChanged);
         this.bgWorkerScanner.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgWorkerScanner_RunWorkerCompleted);
         // 
         // bgWorkerResultsUpdater
         // 
         this.bgWorkerResultsUpdater.WorkerSupportsCancellation = true;
         this.bgWorkerResultsUpdater.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgWorkerResultsUpdater_DoWork);
         // 
         // contextMenuChkListBox
         // 
         this.contextMenuChkListBox.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contextMenuChkListBox_btnSelectAll});
         this.contextMenuChkListBox.Name = "contextMenuChkListBox";
         this.contextMenuChkListBox.Size = new System.Drawing.Size(121, 26);
         // 
         // contextMenuChkListBox_btnSelectAll
         // 
         this.contextMenuChkListBox_btnSelectAll.CheckOnClick = true;
         this.contextMenuChkListBox_btnSelectAll.Name = "contextMenuChkListBox_btnSelectAll";
         this.contextMenuChkListBox_btnSelectAll.Size = new System.Drawing.Size(120, 22);
         this.contextMenuChkListBox_btnSelectAll.Text = "Select all";
         this.contextMenuChkListBox_btnSelectAll.Click += new System.EventHandler(this.uiButtonHandler_Click);
         // 
         // dataGridSavedResults_chkBoxFreezeValue
         // 
         this.dataGridSavedResults_chkBoxFreezeValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
         this.dataGridSavedResults_chkBoxFreezeValue.HeaderText = "Freeze";
         this.dataGridSavedResults_chkBoxFreezeValue.Name = "dataGridSavedResults_chkBoxFreezeValue";
         this.dataGridSavedResults_chkBoxFreezeValue.Width = 46;
         // 
         // dataGridSavedResults_txtBoxDescription
         // 
         this.dataGridSavedResults_txtBoxDescription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
         this.dataGridSavedResults_txtBoxDescription.FillWeight = 50F;
         this.dataGridSavedResults_txtBoxDescription.HeaderText = "Description";
         this.dataGridSavedResults_txtBoxDescription.MinimumWidth = 20;
         this.dataGridSavedResults_txtBoxDescription.Name = "dataGridSavedResults_txtBoxDescription";
         this.dataGridSavedResults_txtBoxDescription.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
         this.dataGridSavedResults_txtBoxDescription.Width = 73;
         // 
         // dataGridSavedResults_txtBoxAddress
         // 
         this.dataGridSavedResults_txtBoxAddress.HeaderText = "Address";
         this.dataGridSavedResults_txtBoxAddress.Name = "dataGridSavedResults_txtBoxAddress";
         this.dataGridSavedResults_txtBoxAddress.ReadOnly = true;
         this.dataGridSavedResults_txtBoxAddress.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
         this.dataGridSavedResults_txtBoxAddress.Width = 91;
         // 
         // dataGridSavedResults_txtBoxSection
         // 
         this.dataGridSavedResults_txtBoxSection.HeaderText = "Section";
         this.dataGridSavedResults_txtBoxSection.Name = "dataGridSavedResults_txtBoxSection";
         this.dataGridSavedResults_txtBoxSection.ReadOnly = true;
         this.dataGridSavedResults_txtBoxSection.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
         this.dataGridSavedResults_txtBoxSection.Width = 90;
         // 
         // dataGridSavedResults_txtBoxValueType
         // 
         this.dataGridSavedResults_txtBoxValueType.HeaderText = "Type";
         this.dataGridSavedResults_txtBoxValueType.Name = "dataGridSavedResults_txtBoxValueType";
         this.dataGridSavedResults_txtBoxValueType.ReadOnly = true;
         this.dataGridSavedResults_txtBoxValueType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
         this.dataGridSavedResults_txtBoxValueType.Width = 91;
         // 
         // dataGridSavedResults_txtBoxValue
         // 
         this.dataGridSavedResults_txtBoxValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
         this.dataGridSavedResults_txtBoxValue.HeaderText = "Value";
         this.dataGridSavedResults_txtBoxValue.Name = "dataGridSavedResults_txtBoxValue";
         this.dataGridSavedResults_txtBoxValue.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
         // 
         // MainForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
         this.AutoSize = true;
         this.BackColor = System.Drawing.SystemColors.Control;
         this.ClientSize = new System.Drawing.Size(484, 451);
         this.Controls.Add(this.splitContainerMain);
         this.Controls.Add(this.uiToolStrip);
         this.Controls.Add(this.uiStatusStrip);
         this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.ForeColor = System.Drawing.Color.Black;
         this.MinimumSize = new System.Drawing.Size(500, 490);
         this.Name = "MainForm";
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
         this.Text = "PS4 Cheater";
         this.Load += new System.EventHandler(this.MainForm_Load);
         this.uiToolStrip.ResumeLayout(false);
         this.uiToolStrip.PerformLayout();
         this.splitContainerMain.Panel1.ResumeLayout(false);
         this.splitContainerMain.Panel2.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
         this.splitContainerMain.ResumeLayout(false);
         this.splitContainerScanner.Panel1.ResumeLayout(false);
         this.splitContainerScanner.Panel2.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.splitContainerScanner)).EndInit();
         this.splitContainerScanner.ResumeLayout(false);
         this.splitContainerScanDetails.Panel1.ResumeLayout(false);
         this.splitContainerScanDetails.Panel1.PerformLayout();
         this.splitContainerScanDetails.Panel2.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.splitContainerScanDetails)).EndInit();
         this.splitContainerScanDetails.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.dataGridSavedResults)).EndInit();
         this.uiStatusStrip.ResumeLayout(false);
         this.uiStatusStrip.PerformLayout();
         this.contextMenuChkListBox.ResumeLayout(false);
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.ToolStrip uiToolStrip;
      private System.Windows.Forms.SplitContainer splitContainerMain;
      private System.Windows.Forms.SplitContainer splitContainerScanner;
      private System.Windows.Forms.ListView listViewResults;
      private System.Windows.Forms.ColumnHeader columnHeaderAddress;
      private System.Windows.Forms.ColumnHeader columnHeaderSection;
      private System.Windows.Forms.DataGridView dataGridSavedResults;
      private System.Windows.Forms.ToolStripDropDownButton uiToolStrip_linkFile;
      private System.Windows.Forms.ToolStripDropDownButton uiToolStrip_linkPayloadAndProcess;
      private System.Windows.Forms.ToolStripDropDownButton uiToolStrip_linkTools;
      private System.Windows.Forms.ColumnHeader columnHeaderValue;
      private System.Windows.Forms.SplitContainer splitContainerScanDetails;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.ComboBox cmbBoxValueType;
      private System.Windows.Forms.ComboBox cmbBoxScanType;
      private System.Windows.Forms.TextBox txtBoxScanValue;
      private System.Windows.Forms.CheckBox chkBoxIsHexValue;
      private System.Windows.Forms.Button btnScanNext;
      private System.Windows.Forms.Button btnScan;
      private System.Windows.Forms.CheckedListBox chkListBoxSearchSections;
      private System.Windows.Forms.ToolStripMenuItem uiToolStrip_btnLoadCheatTable;
      private System.Windows.Forms.ToolStripMenuItem uiToolStrip_btnSaveCheatTable;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
      private System.Windows.Forms.ToolStripMenuItem uiToolStrip_btnExit;
      private System.Windows.Forms.ToolStripMenuItem uiToolStrip_linkPayloadManager;
      private System.Windows.Forms.ToolStripMenuItem uiToolStrip_PayloadManager_chkPayloadActive;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
      private System.Windows.Forms.ToolStripMenuItem uiToolStrip_PayloadManager_btnSendPayload;
      private System.Windows.Forms.ToolStripMenuItem uiToolStrip_linkProcessManager;
      private System.Windows.Forms.ToolStripMenuItem uiToolStrip_ProcessManager_btnRefreshProcessList;
      private System.Windows.Forms.ToolStripComboBox uiToolStrip_ProcessManager_cmbBoxActiveProcess;
      private System.Windows.Forms.ToolStripMenuItem uiToolStrip_btnOpenPointerScanner;
      private System.Windows.Forms.StatusStrip uiStatusStrip;
      private System.Windows.Forms.ToolStripLabel uiToolStrip_lblActiveProcess;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
      private System.Windows.Forms.ToolStripProgressBar uiToolStrip_progressBarScanPercent;
      private System.Windows.Forms.ToolStripDropDownButton uiStatusStrip_linkEntryManager;
      private System.Windows.Forms.ToolStripMenuItem uiStatusStrip_EntryManager_btnAddAddress;
      private System.Windows.Forms.ToolStripMenuItem uiStatusStrip_EntryManager_btnAddPointer;
      private System.Windows.Forms.ToolStripStatusLabel uiStatusStrip_lblStatus;
      private System.ComponentModel.BackgroundWorker bgWorkerScanner;
      private System.Windows.Forms.CheckBox chkBoxFastScan;
      private System.ComponentModel.BackgroundWorker bgWorkerResultsUpdater;
      private System.Windows.Forms.TextBox txtBoxScanValueSecond;
      private System.Windows.Forms.Label lblSecondValue;
      private System.Windows.Forms.TextBox txtBoxSectionsFilter;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.ContextMenuStrip contextMenuChkListBox;
      private System.Windows.Forms.ToolStripMenuItem contextMenuChkListBox_btnSelectAll;
      private System.Windows.Forms.DataGridViewCheckBoxColumn dataGridSavedResults_chkBoxFreezeValue;
      private System.Windows.Forms.DataGridViewTextBoxColumn dataGridSavedResults_txtBoxDescription;
      private System.Windows.Forms.DataGridViewTextBoxColumn dataGridSavedResults_txtBoxAddress;
      private System.Windows.Forms.DataGridViewTextBoxColumn dataGridSavedResults_txtBoxSection;
      private System.Windows.Forms.DataGridViewTextBoxColumn dataGridSavedResults_txtBoxValueType;
      private System.Windows.Forms.DataGridViewTextBoxColumn dataGridSavedResults_txtBoxValue;
   }
}