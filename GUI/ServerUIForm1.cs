// Name: Rutuja Sanjay Risbood
// UTA ID: 1001843943


// https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.control.checkforillegalcrossthreadcalls?view=net-5.0


using MyServer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUI
{
    public partial class ServerUIForm1 : Form
    {

        Server sv;
        public ServerUIForm1()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        public void Log(String message)
        {
            textBox1.Text += message + Environment.NewLine;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                sv = new Server((String message) => { Log(message); });
                sv.Run();
            });
        }
    }
}
