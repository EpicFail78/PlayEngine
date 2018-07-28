using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

using PS4_Cheater.Utils;

namespace PS4_Cheater.Forms.ChildForms {
   public partial class childFrmSendPayload : Form {
      public childFrmSendPayload() {
         if (MemoryHelper.Connect(Settings.mInstance.ps4.IPAddress, Settings.mInstance.ps4.FWVersion == PS4FWVersion.v5_05))
            MessageBox.Show("Payload is already injected!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

         InitializeComponent();

         foreach (var payloadDir in Directory.GetDirectories(Path.Combine(Application.StartupPath, "Payloads")))
            cmbBoxFirmware.Items.Add(new DirectoryInfo(payloadDir).Name);
         txtBoxIPAddress.Text = Settings.mInstance.ps4.IPAddress;
         txtBoxIPPort.Text = Settings.mInstance.ps4.IPPort.ToString();
         cmbBoxFirmware.SelectedIndex = (Int32)Settings.mInstance.ps4.FWVersion;
      }

      private void btnSendPayload_Click(Object sender, EventArgs e) {
         try {
            String payloadDir = Path.Combine(Application.StartupPath, "Payloads\\" + (String)cmbBoxFirmware.SelectedItem);
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)) {
               socket.Connect(new IPEndPoint(IPAddress.Parse(txtBoxIPAddress.Text), Convert.ToInt32(txtBoxIPPort.Text)));
               socket.SendFile(Path.Combine(payloadDir, "payload.bin"));
               socket.Shutdown(SocketShutdown.Both);
               socket.Close();
               Thread.Sleep(2000);
            }
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)) {
               socket.Connect(new IPEndPoint(IPAddress.Parse(txtBoxIPAddress.Text), 9023));
               socket.SendFile(Path.Combine(payloadDir, "kpayload.elf"));
               socket.Shutdown(SocketShutdown.Both);
               socket.Close();
               Thread.Sleep(2000);
            }
            Settings.mInstance.ps4.IPAddress = txtBoxIPAddress.Text;
            Settings.mInstance.ps4.IPPort = Convert.ToInt32(txtBoxIPPort.Text);
            Settings.mInstance.ps4.FWVersion = (PS4FWVersion)cmbBoxFirmware.SelectedIndex;
            Settings.mInstance.saveToFile();

            if (MessageBox.Show("Payload successfully injected!", "Success") == DialogResult.OK)
               this.Close();
         } catch (Exception ex) {
            MessageBox.Show(ex.ToString(), "Error during sending payload!", MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
      }
   }
}
