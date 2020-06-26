namespace Uno
{
    partial class Multiplier
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mnuMultiplier = new System.Windows.Forms.MenuStrip();
            this.mnuMultiply1 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMultiply2 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMultiply4 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMultiply8 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMultiplier.SuspendLayout();
            this.SuspendLayout();
            // 
            // mnuMultiplier
            // 
            this.mnuMultiplier.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mnuMultiplier.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuMultiply1,
            this.mnuMultiply2,
            this.mnuMultiply4,
            this.mnuMultiply8});
            this.mnuMultiplier.Location = new System.Drawing.Point(0, 0);
            this.mnuMultiplier.Name = "mnuMultiplier";
            this.mnuMultiplier.Padding = new System.Windows.Forms.Padding(7, 3, 0, 3);
            this.mnuMultiplier.Size = new System.Drawing.Size(292, 26);
            this.mnuMultiplier.TabIndex = 1;
            // 
            // mnuMultiply1
            // 
            this.mnuMultiply1.Name = "mnuMultiply1";
            this.mnuMultiply1.Size = new System.Drawing.Size(56, 21);
            this.mnuMultiply1.Tag = "1";
            this.mnuMultiply1.Text = "不加倍";
            this.mnuMultiply1.Click += new System.EventHandler(this.MnuMultiply_Click);
            // 
            // mnuMultiply2
            // 
            this.mnuMultiply2.Name = "mnuMultiply2";
            this.mnuMultiply2.Size = new System.Drawing.Size(44, 21);
            this.mnuMultiply2.Tag = "2";
            this.mnuMultiply2.Text = "加倍";
            this.mnuMultiply2.Click += new System.EventHandler(this.MnuMultiply_Click);
            // 
            // mnuMultiply4
            // 
            this.mnuMultiply4.Name = "mnuMultiply4";
            this.mnuMultiply4.Size = new System.Drawing.Size(43, 21);
            this.mnuMultiply4.Tag = "4";
            this.mnuMultiply4.Text = "4 倍";
            this.mnuMultiply4.Click += new System.EventHandler(this.MnuMultiply_Click);
            // 
            // mnuMultiply8
            // 
            this.mnuMultiply8.Name = "mnuMultiply8";
            this.mnuMultiply8.Size = new System.Drawing.Size(43, 21);
            this.mnuMultiply8.Tag = "8";
            this.mnuMultiply8.Text = "8 倍";
            this.mnuMultiply8.Click += new System.EventHandler(this.MnuMultiply_Click);
            // 
            // Multiplier
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 277);
            this.ControlBox = false;
            this.Controls.Add(this.mnuMultiplier);
            this.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Multiplier";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.mnuMultiplier.ResumeLayout(false);
            this.mnuMultiplier.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.MenuStrip mnuMultiplier;
        private System.Windows.Forms.ToolStripMenuItem mnuMultiply1;
        private System.Windows.Forms.ToolStripMenuItem mnuMultiply2;
        private System.Windows.Forms.ToolStripMenuItem mnuMultiply4;
        private System.Windows.Forms.ToolStripMenuItem mnuMultiply8;
    }
}