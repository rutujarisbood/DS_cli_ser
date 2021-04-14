// Name: Rutuja Sanjay Risbood
// UTA ID: 1001843943

// https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.control.checkforillegalcrossthreadcalls?view=net-5.0

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientUI
{
    public partial class ClientUI : Form
    {
        public ClientUI()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
        }

        public void Log(String message)
        {
            textBox1.Text += message + Environment.NewLine;
        }

        public void Append(String message)
        {
            lexicon_textBox.Text += message + Environment.NewLine;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                if (!string.IsNullOrEmpty(textBox2.Text))
                    Client.Client.Run((String message) => { Log(message); }, textBox2.Text);
            });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var fd = new System.Windows.Forms.OpenFileDialog();
            if (fd.ShowDialog() == DialogResult.OK)
            {
                Client.Client.SendFile(fd.FileName, textBox2.Text);
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {//exitClient
            Client.Client.exitClient(textBox2.Text);
            Environment.Exit(0);
        }

        private void Add_button_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Add_textbox.Text))
            {
                //var wordToAdd = Add_textbox.Text;
                lexicon_textBox.Clear();
                Client.Client.addWordToQueue((String message) => { Append(message); },Add_textbox.Text);
                Add_textbox.Clear();
            }
        }

        //public void clearLexicon_textBox()
        //{
        //    lexicon_textBox.Clear();
        //}

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
