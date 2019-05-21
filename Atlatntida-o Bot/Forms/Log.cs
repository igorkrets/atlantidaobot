using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Atlatntida_o_Bot.Forms
{
    public partial class Log : DockContent
    {
        public Log()
        {
            InitializeComponent();
        }

        private void TextLog_TextChanged(object sender, EventArgs e)
        {
            TextLog.SelectionStart = TextLog.Text.Length;
            TextLog.ScrollToCaret();
        }
    }
}
