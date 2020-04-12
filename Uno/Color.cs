using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Uno
{
    public partial class FrmColor : Form
    {
        public FrmColor(byte[] colorList)
        {
            InitializeComponent();
            for (byte c = 0; c < 7; c++)
            {
                mnuColor.Items.Add("  ");
                mnuColor.Items[c].Click += new EventHandler(MnuColor_Click);
                mnuColor.Items[c].Enabled = false;
                mnuColor.Items[c].Font = new Font(mnuColor.Items[c].Font.FontFamily, mnuColor.Items[c].Font.Size * 4);
                mnuColor.Items[c].Tag = c;
                if (!colorList.Contains(c))
                    mnuColor.Items[c].Visible = false;
            }
            Size = new Size(mnuColor.Items[0].Width * (colorList.Length + 1), mnuColor.Height);
        }

        private void FrmColor_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.NumPad0:
                case Keys.NumPad1:
                case Keys.NumPad2:
                case Keys.NumPad3:
                case Keys.NumPad4:
                case Keys.NumPad5:
                    byte c = (byte)(e.KeyCode - Keys.NumPad0);
                    if (mnuColor.Items[c].Enabled)
                    {
                        Tag = c;
                        Close();
                    }
                    break;
            }
        }

        private void MnuColor_Click(object sender, EventArgs e)
        {
            Tag = ((ToolStripMenuItem)sender).Tag;
            Close();
        }
    }
}
