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
         InitializeComponent();

         foreach (var payloadDir in Directory.GetDirectories(Path.Combine(Application.StartupPath, "Payloads")))
            cmbBoxFirmware.Items.Add(new DirectoryInfo(payloadDir).Name);
         txtBoxIPAddress.Text = SharedInformation.PS4_IPAddress;
         txtBoxIPPort.Text = SharedInformation.PS4_IPPort;
         cmbBoxFirmware.SelectedIndex = (Int32)SharedInformation.PS4_Version;
      }

      private void btnSendPayload_Click(Object sender, EventArgs e) {
         try {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)) {
               String payloadDir = Path.Combine(Application.StartupPath, "Payloads\\" + (String)cmbBoxFirmware.SelectedItem);

               socket.Connect(new IPEndPoint(IPAddress.Parse(txtBoxIPAddress.Text), Convert.ToInt32(txtBoxIPPort.Text)));
               socket.SendFile(Path.Combine(payloadDir, "payload.bin"));
               socket.Shutdown(SocketShutdown.Both);
               socket.Close();
               Thread.Sleep(2000);

               socket.Connect(new IPEndPoint(IPAddress.Parse(txtBoxIPAddress.Text), 9023));
               socket.SendFile(Path.Combine(payloadDir, "kpayload.elf"));
               socket.Shutdown(SocketShutdown.Both);
               socket.Close();
               Thread.Sleep(2000);

               SharedInformation.PS4_IPAddress = txtBoxIPAddress.Text;
               SharedInformation.PS4_IPPort = txtBoxIPPort.Text;
               SharedInformation.PS4_Version = (PS4Version)cmbBoxFirmware.SelectedIndex;
            }
            if (MessageBox.Show("Payload successfully injected!", "Success") == DialogResult.OK)
               this.Close();
         } catch (Exception ex) {
            MessageBox.Show(ex.Message, "Error during sending payload", MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
      }
   }
}
