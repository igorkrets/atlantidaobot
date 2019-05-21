using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Atlatntida_o_Bot.Libs;

namespace Atlatntida_o_Bot.Forms.Debug
{
    public partial class ShowHtml : Form
    {
        public ShowHtml()
        {
            InitializeComponent();
        }

        public ShowHtml(string url)
        {
            InitializeComponent();
            urlBox.Text = url;
            unescape.Text = Requests.GetResponseHtml(url);
        }

        private void urlBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    string text = Requests.GetResponseHtml(urlBox.Text, splitText.Checked);
                    if (unescapeCheck.Checked)
                    {
                        text = System.Text.RegularExpressions.Regex.Unescape(text);
                    }
                    unescape.Text = text;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
