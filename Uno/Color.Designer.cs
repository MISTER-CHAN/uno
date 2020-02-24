namespace Uno
{
    partial class FrmColor
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.mnuColor = new System.Windows.Forms.MenuStrip();
            this.SuspendLayout();
            // 
            // mnuColor
            // 
            this.mnuColor.Location = new System.Drawing.Point(0, 0);
            this.mnuColor.Name = "mnuColor";
            this.mnuColor.Size = new System.Drawing.Size(292, 24);
            this.mnuColor.TabIndex = 0;
            // 
            // FrmColor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 277);
            this.ControlBox = false;
            this.Controls.Add(this.mnuColor);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MainMenuStrip = this.mnuColor;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmColor";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmColor_KeyDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.MenuStrip mnuColor;


    }
}