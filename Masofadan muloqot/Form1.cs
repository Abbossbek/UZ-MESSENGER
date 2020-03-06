using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace UzMessenager
{
    public partial class Form1 : Form
    {
        Socket sck;
        EndPoint epLocal, epRemote;
        List<string> Informations = new List<string>();
        Thread myThread=null;
        Network net=new Network();

        public Form1()
        {
            InitializeComponent();
            sck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            textLocalIp.Text = net.GetLocalIp();

            label6.Text = "Press Scan to find online friends";
            Control.CheckForIllegalCrossThreadCalls = false;
            
            dgvOnlineFriends.Columns[0].Width = dgvOnlineFriends.Width/2-5;
            dgvOnlineFriends.Columns[1].Width = dgvOnlineFriends.Width / 2;

            textMessage.Location = new Point(15, 15);
            textMessage.Width = panel3.Width - 120;

            
        }
        
        private void MessageCallBack(IAsyncResult aResult)
        {
            try
            {
                int size = sck.EndReceiveFrom(aResult, ref epRemote);
                if (size > 0)
                {
                    byte[] receivedData = new byte[1500];

                    receivedData = (byte[])aResult.AsyncState;

                    ASCIIEncoding eEncoding = new ASCIIEncoding();
                    string receivedMessage = eEncoding.GetString(receivedData);
                    richTextBox1.AppendText("\nFriend: " + receivedMessage);

                    byte[] buffer = new byte[1500];
                    sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);

                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

            try
            {
                epLocal = new IPEndPoint(IPAddress.Parse(textLocalIp.Text), Convert.ToInt32(textLocalPort.Text));
                sck.Bind(epLocal);

                epRemote = new IPEndPoint(IPAddress.Parse(txtFriendIp.Text), Convert.ToInt32(textFriendsPort.Text));

                sck.Connect(epRemote);

                byte[] buffer = new byte[1500];
                sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
                

                button1.Text = "Connected";
                button1.Enabled = false;
                button2.Enabled = true;
                textMessage.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                net = new Network();
                textLocalIp.Text = net.GetLocalIp();
                Informations.Clear();
                dgvOnlineFriends.Rows.Clear();
                
                button1.Text = "Connect";
                button1.Enabled = true;
                myThread=new Thread(()=> net.GetAllOnlineIpAddresses(dgvOnlineFriends, progressBar1, label6));
                myThread.Start();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

       

        private void button2_Click_1(object sender, EventArgs e)
        {
            try
            {
                System.Text.ASCIIEncoding enc = new ASCIIEncoding();
                byte[] msg = new byte[1500];
                msg = enc.GetBytes(textMessage.Text);

                sck.Send(msg);

                richTextBox1.AppendText("\nYou: " + textMessage.Text);
                textMessage.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void dgvOnlineFriends_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            txtFriendIp.Text = dgvOnlineFriends.CurrentCell.Value.ToString();
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            textMessage.Location = new Point(15, 15);
            textMessage.Width = panel3.Width - 120;
            dgvOnlineFriends.Height = btnExit.Location.Y - dgvOnlineFriends.Location.Y-20;
        }

        
        
    }
}
