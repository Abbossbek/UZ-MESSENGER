using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using System.Management;
using System.Diagnostics;

using System.Net.Sockets;


namespace UzMessenager
{
    class Network
    {
        public string GetLocalIp()
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.Name == Choose_Adapter.Adapter)
                {
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            return ip.Address.ToString();
                        }
                    }
                }
            }
            return "127.0.0.1";
        }

        public void GetAllOnlineIpAddresses(DataGridView dgv, ProgressBar progBar, Label lbl)
        {
            try
            {
                Process cmd = new Process();
                cmd.StartInfo.FileName = "cmd.exe";
                cmd.StartInfo.RedirectStandardInput = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = false;
                cmd.Start();

                cmd.StandardInput.WriteLine("arp -a");
                cmd.StandardInput.Flush();
                cmd.StandardInput.Close();

                string s = cmd.StandardOutput.ReadToEnd();
                s = s.Substring(s.IndexOf(GetLocalIp()));
                s = s.Substring(0, s.IndexOf("\r\n\r\n"));
                s = s.Remove(0, s.IndexOf('\n') + 1);
                s = s.Remove(0, s.IndexOf('\n') + 1);
                s = s.Remove(s.LastIndexOf('\n'));

                string[] str = s.Split('\n');
                progBar.Maximum = str.Length;

                for (int i = 0; i < str.Length; i++)
                {
                    progBar.Value = i + 1;
                    str[i] = str[i].Substring(2, 16);
                    str[i] = str[i].Remove(str[i].IndexOf(' '));
                    lbl.Text = "Scanning " + str[i];
                    try
                    {
                        dgv.Rows.Add(str[i], Dns.GetHostEntry(IPAddress.Parse(str[i])).HostName);
                    }
                    catch
                    {
                        dgv.Rows.Add(str[i], "Unknown");
                    }
                }
                progBar.Value = 0;
                lbl.Text = "Scanning finished!";
            }catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        
        public void query(string host, ref Label lblStatus)
        {
            string temp = null;
            string[] _searchClass = { "Win32_ComputerSystem", "Win32_OperatingSystem", "Win32_BaseBoard", "Win32_BIOS" };
            string[] param = { "UserName", "Caption", "Product", "Description" };

            for (int i = 0; i <= _searchClass.Length - 1; i++)
            {
                lblStatus.Text = "Getting information.";
                try
                {
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher("\\\\" + host + "\\root\\CIMV2", "SELECT *FROM " + _searchClass[i]);
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        lblStatus.Text = "Getting information. .";

                        temp += obj.GetPropertyValue(param[i]).ToString() + "\n";
                        if (i == _searchClass.Length - 1)
                        {
                            lblStatus.Text = "Done!";
                            MessageBox.Show(temp, "Hostinfo: " + host, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                        }
                        lblStatus.Text = "Getting information. . .";
                    }
                }
                catch (Exception ex) { MessageBox.Show("Error in WMI query.\n\n" + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); break; }
            }
        }
    }
}
