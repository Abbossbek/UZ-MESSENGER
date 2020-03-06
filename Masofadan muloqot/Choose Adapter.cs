using System;
using System.Windows.Forms;

using System.Net.NetworkInformation;

namespace UzMessenager
{
    public partial class Choose_Adapter : Form
    {
        public static string Adapter { get; set; }
        public Choose_Adapter()
        {
            InitializeComponent();
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus == OperationalStatus.Up)
                {
                    listBox1.Items.Add(ni.Name);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Adapter = listBox1.SelectedItem.ToString();
                Form1 f = new Form1();
                f.ShowDialog();
                this.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
