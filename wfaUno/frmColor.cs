using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace wfaUno
{
    public partial class frmColor : Form
    {
        public frmColor()
        {
            InitializeComponent();
            for (int c = 0; c <= 3; c++)
            {
                mnuColor.Items.Add("  ");
                mnuColor.Items[c].Click += new EventHandler(mnuColor_Click);
                mnuColor.Items[c].Enabled = false;
                mnuColor.Items[c].Font = new Font(mnuColor.Items[c].Font.FontFamily, mnuColor.Items[c].Font.Size * 4);
                mnuColor.Items[c].Tag = c;
            }
            Size = new Size(mnuColor.Items[0].Width * 5, mnuColor.Height);
        }

        private void frmColor_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.NumPad0:
                case Keys.NumPad1:
                case Keys.NumPad2:
                case Keys.NumPad3:
                    int c = e.KeyCode - Keys.NumPad0;
                    if (!mnuColor.Items[c].Enabled)
                        break;
                    Tag = c + "";
                    Close();
                    break;
            }
        }

        private void mnuColor_Click(object sender, EventArgs e)
        {
            Tag = ((ToolStripMenuItem)sender).Tag;
            Close();
        }
    }
}
