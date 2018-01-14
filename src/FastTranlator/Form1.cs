using System;
using System.Text;
using System.Windows.Forms;
using Tools;
using System.Configuration;
using FastTranslator.Properties;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace FastTranslator
{
    public partial class Form1 : Form
    {
        MultiClipboard _clipboard;
        DictCC _dict;
        string alterInhalt = String.Empty;
        public Form1(int lang)
        {
            InitializeComponent();
            _clipboard = new Tools.MultiClipboard(this);
            _clipboard.clipBoardChanged += new MultiClipboard.ClipBoardChangHandler(_clipboard_clipBoardChanged);
            _dict = new DictCC(lang);
            Text += " (" + _dict.Name.ToUpper() + ")";
            checkBox1_CheckedChanged(null, null);
        }

        private void JumpListeAkt()
        {
            JumpList list = JumpList.CreateJumpList();
            list.ClearAllUserTasks();
            for (int i = 0; i < _dict.Namen.Length; i++)
            {
                JumpListLink j = new JumpListLink(Application.ExecutablePath, _dict.Namen[i]);
                j.Arguments = i.ToString();
                j.IconReference = new Microsoft.WindowsAPICodePack.Shell.IconReference(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "explorer.exe"), 0);
                list.AddUserTasks(j);
            }
            list.AddUserTasks(new JumpListLink(Application.StartupPath, "open directory"));
            list.Refresh();
        }

        private void _clipboard_clipBoardChanged(object sender, ClipBoardChangEventArgs ex)
        {
            string neuerInhalt;
            if (checkBox1.Checked && (neuerInhalt = Convert.ToString(ex.ClipBoardObject.GetData(typeof(string))).Trim()) != alterInhalt && (alterInhalt = neuerInhalt) != String.Empty)
            {
                webBrowser1.Navigate(_dict.Search(neuerInhalt));
                if (textBox1.Text.Trim() != neuerInhalt) textBox1.Text = neuerInhalt;
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
                Clipboard.SetText(textBox1.Text.Trim());
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked && Clipboard.GetText() != "")
                Clipboard.SetText(Clipboard.GetText());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            JumpListeAkt();
        }
    }
}
