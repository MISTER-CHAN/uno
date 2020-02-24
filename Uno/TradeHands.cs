using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Uno
{
    public partial class TradeHands : Form
    {
        public TradeHands()
        {
            InitializeComponent();
            mnuTradeHands.Items.Add("  ");
            mnuTradeHands.Items[0].Visible = false;
            for (byte p = 1; p <= 3; p++)
            {
                mnuTradeHands.Items.Add("  ");
                mnuTradeHands.Items[p].Click += MnuTradeHands_Click;
                mnuTradeHands.Items[p].Enabled = false;
                mnuTradeHands.Items[p].Font = new Font(mnuTradeHands.Items[p].Font.FontFamily, mnuTradeHands.Items[p].Font.Size * 4);
                mnuTradeHands.Items[p].Tag = p;
            }
            Size = new Size(mnuTradeHands.Items[1].Width * 4, mnuTradeHands.Height);
        }

        private void MnuTradeHands_Click(object sender, EventArgs e)
        {
            Tag = ((ToolStripMenuItem)sender).Tag;
            Close();
        }

        private void TradeHands_KeyDown(object sender, KeyEventArgs e)
        {

            switch (e.KeyCode)
            {
                case Keys.Left:
                case Keys.Up:
                case Keys.Right:
                    byte p = (byte)(e.KeyCode - Keys.Left + 1);
                    if (mnuTradeHands.Items[p].Enabled)
                    {
                        Tag = p;
                        Close();
                    }
                    break;
            }
        }
    }
}
