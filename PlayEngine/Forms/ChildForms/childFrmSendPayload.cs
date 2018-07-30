using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

using PlayEngine.Helpers;

namespace PlayEngine.Forms.ChildForms {
   public partial class childFrmSendPayload : Form {
      public childFrmSendPayload() {
         InitializeComponent();
         foreach (var payloadDir in Directory.GetDirectories(Path.Combine(Application.StartupPath, "Payloads")))
            cmbBoxPayload.Items.Add(new DirectoryInfo(payloadDir).Name);
         if (cmbBoxPayload.Items.Count == 0) {
            MessageBox.Show("No payload was found inside 'Payloads/'!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            this.DialogResult = DialogResult.Abort;
            this.Close();
         }

         txtBoxIPAddress.Text = Settings.mInstance.ps4.IPAddress;
         txtBoxIPPort.Text = Settings.mInstance.ps4.IPPort.ToString();
         if (cmbBoxPayload.Items.Count == 1) {
            cmbBoxPayload.SelectedIndex = 0;
            cmbBoxPayload.Enabled = false;
         } else {
            if (cmbBoxPayload.Items.Contains(Settings.mInstance.ps4.LastUsedPayload))
               cmbBoxPayload.SelectedItem = Settings.mInstance.ps4.LastUsedPayload;
         }
      }

      private void btnSendPayload_Click(Object sender, EventArgs e) {
         try {
            String payloadDir = Path.Combine(Application.StartupPath, "Payloads\\" + (String)cmbBoxPayload.SelectedItem);
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)) {
               IAsyncResult result = socket.BeginConnect(txtBoxIPAddress.Text, 733, null, null);
               result.AsyncWaitHandle.WaitOne(1000);
               if (!socket.Connected) {
                  socket.Close();
                  socket.Connect(txtBoxIPAddress.Text, Convert.ToInt32(txtBoxIPPort.Text));
                  socket.SendFile(Path.Combine(payloadDir, "payload.bin"));
                  socket.Shutdown(SocketShutdown.Both);
                  socket.Close();
                  MessageBox.Show("Payload successfully injected!", "Success");
               } else {
                  MessageBox.Show("Payload is already injected, connecting...", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
               }
            }
            Settings.mInstance.ps4.IPAddress = txtBoxIPAddress.Text;
            Settings.mInstance.ps4.IPPort = Convert.ToInt32(txtBoxIPPort.Text);
            Settings.mInstance.ps4.LastUsedPayload = cmbBoxPayload.SelectedText;
            Settings.mInstance.saveToFile();

            this.DialogResult = DialogResult.OK;
            this.Close();
         } catch (Exception ex) {
            MessageBox.Show(ex.ToString(), "Error during sending payload!", MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
      }
   }
}
