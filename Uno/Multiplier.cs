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
    public partial class Multiplier : Form
    {
        readonly Options options;

        private void MnuMultiply_Click(object sender, EventArgs e)
        {
            options.multiplier *= int.Parse(((ToolStripMenuItem)sender).Tag + "");
            Close();
        }
        public Multiplier(Options options)
        {
            InitializeComponent();
            this.options = options;
            foreach (ToolStripMenuItem mnu in mnuMultiplier.Items)
                mnu.Font = new Font(mnu.Font.FontFamily, mnu.Font.Size * 4);
            Size = new Size(mnuMultiplier.Items[0].Width * 5, mnuMultiplier.Height);
        }
    }
}
