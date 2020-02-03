namespace Uno {
	partial class Uno {
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 清理所有正在使用的资源。
		/// </summary>
		/// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows 窗体设计器生成的代码

		/// <summary>
		/// 设计器支持所需的方法 - 不要
		/// 使用代码编辑器修改此方法的内容。
		/// </summary>
		private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.lblPile = new System.Windows.Forms.Label();
            this.mnuCheating = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuCheat = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuColor = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuNumber = new System.Windows.Forms.ToolStripMenuItem();
            this.timPileToPlayers = new System.Windows.Forms.Timer(this.components);
            this.mnuGame = new System.Windows.Forms.MenuStrip();
            this.itmGame = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuNew = new System.Windows.Forms.ToolStripMenuItem();
            this.separatorNc = new System.Windows.Forms.ToolStripSeparator();
            this.mnuRank = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuByColor = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuByNumber = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuScrollBar = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuToolTip = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAppearance = new System.Windows.Forms.ToolStripMenuItem();
            this.separatorCx = new System.Windows.Forms.ToolStripSeparator();
            this.mnuSaveGame = new System.Windows.Forms.ToolStripMenuItem();
            this.itmQuit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuChat = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAuto = new System.Windows.Forms.ToolStripMenuItem();
            this.itmHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuContent = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCanPlay = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuControl = new System.Windows.Forms.ToolStripMenuItem();
            this.separatorHa = new System.Windows.Forms.ToolStripSeparator();
            this.itmAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuStop = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPlayPause = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuForward = new System.Windows.Forms.ToolStripMenuItem();
            this.timPlayersToCenter = new System.Windows.Forms.Timer(this.components);
            this.timPileToCenter = new System.Windows.Forms.Timer(this.components);
            this.lblDrawMark = new System.Windows.Forms.Label();
            this.lblDraw = new System.Windows.Forms.Label();
            this.lblAction = new System.Windows.Forms.Label();
            this.timTurn = new System.Windows.Forms.Timer(this.components);
            this.timUno = new System.Windows.Forms.Timer(this.components);
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.lblWatch = new System.Windows.Forms.Label();
            this.hPlayer = new System.Windows.Forms.HScrollBar();
            this.pnlPlayer = new System.Windows.Forms.Panel();
            this.timWatch = new System.Windows.Forms.Timer(this.components);
            this.pnlMovingCards = new System.Windows.Forms.Panel();
            this.timThinking = new System.Windows.Forms.Timer(this.components);
            this.rdoUno = new System.Windows.Forms.RadioButton();
            this.mnuCheating.SuspendLayout();
            this.mnuGame.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblPile
            // 
            this.lblPile.BackColor = System.Drawing.Color.White;
            this.lblPile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPile.Font = new System.Drawing.Font("微軟正黑體 Light", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPile.ForeColor = System.Drawing.Color.White;
            this.lblPile.Location = new System.Drawing.Point(0, 33);
            this.lblPile.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPile.Name = "lblPile";
            this.lblPile.Size = new System.Drawing.Size(90, 120);
            this.lblPile.TabIndex = 0;
            this.lblPile.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblPile.Click += new System.EventHandler(this.LblPile_Click);
            this.lblPile.DoubleClick += new System.EventHandler(this.LblPile_DoubleClick);
            // 
            // mnuCheating
            // 
            this.mnuCheating.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuCheat});
            this.mnuCheating.Name = "mnuCheating";
            this.mnuCheating.Size = new System.Drawing.Size(101, 26);
            this.mnuCheating.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.MnuCheating_Closed);
            // 
            // mnuCheat
            // 
            this.mnuCheat.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuColor,
            this.mnuNumber});
            this.mnuCheat.Name = "mnuCheat";
            this.mnuCheat.Size = new System.Drawing.Size(100, 22);
            this.mnuCheat.Text = "出千";
            this.mnuCheat.Click += new System.EventHandler(this.MnuCheat_Click);
            // 
            // mnuColor
            // 
            this.mnuColor.Name = "mnuColor";
            this.mnuColor.Size = new System.Drawing.Size(100, 22);
            this.mnuColor.Text = "顏色";
            this.mnuColor.Visible = false;
            // 
            // mnuNumber
            // 
            this.mnuNumber.Name = "mnuNumber";
            this.mnuNumber.Size = new System.Drawing.Size(100, 22);
            this.mnuNumber.Text = "數字";
            this.mnuNumber.Visible = false;
            // 
            // timPileToPlayers
            // 
            this.timPileToPlayers.Interval = 10;
            this.timPileToPlayers.Tick += new System.EventHandler(this.TimPileToPlayers_Tick);
            // 
            // mnuGame
            // 
            this.mnuGame.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itmGame,
            this.mnuChat,
            this.mnuAuto,
            this.itmHelp,
            this.mnuStop,
            this.mnuPlayPause,
            this.mnuForward});
            this.mnuGame.Location = new System.Drawing.Point(0, 0);
            this.mnuGame.Name = "mnuGame";
            this.mnuGame.Padding = new System.Windows.Forms.Padding(5, 3, 0, 3);
            this.mnuGame.Size = new System.Drawing.Size(890, 27);
            this.mnuGame.TabIndex = 2;
            // 
            // itmGame
            // 
            this.itmGame.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuNew,
            this.separatorNc,
            this.mnuRank,
            this.mnuScrollBar,
            this.mnuToolTip,
            this.mnuAppearance,
            this.separatorCx,
            this.mnuSaveGame,
            this.itmQuit});
            this.itmGame.Name = "itmGame";
            this.itmGame.Size = new System.Drawing.Size(65, 21);
            this.itmGame.Text = "游戏 (&G)";
            // 
            // mnuNew
            // 
            this.mnuNew.Name = "mnuNew";
            this.mnuNew.Size = new System.Drawing.Size(167, 22);
            this.mnuNew.Text = "开局 (&N)";
            this.mnuNew.Click += new System.EventHandler(this.MnuNew_Click);
            // 
            // separatorNc
            // 
            this.separatorNc.Name = "separatorNc";
            this.separatorNc.Size = new System.Drawing.Size(164, 6);
            // 
            // mnuRank
            // 
            this.mnuRank.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuByColor,
            this.mnuByNumber});
            this.mnuRank.Name = "mnuRank";
            this.mnuRank.Size = new System.Drawing.Size(167, 22);
            this.mnuRank.Text = "排序 (&R)";
            // 
            // mnuByColor
            // 
            this.mnuByColor.Checked = true;
            this.mnuByColor.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mnuByColor.Name = "mnuByColor";
            this.mnuByColor.Size = new System.Drawing.Size(112, 22);
            this.mnuByColor.Text = "按颜色";
            this.mnuByColor.Click += new System.EventHandler(this.MnuRank_Click);
            // 
            // mnuByNumber
            // 
            this.mnuByNumber.Name = "mnuByNumber";
            this.mnuByNumber.Size = new System.Drawing.Size(112, 22);
            this.mnuByNumber.Text = "按数字";
            this.mnuByNumber.Click += new System.EventHandler(this.MnuRank_Click);
            // 
            // mnuScrollBar
            // 
            this.mnuScrollBar.Checked = true;
            this.mnuScrollBar.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mnuScrollBar.Name = "mnuScrollBar";
            this.mnuScrollBar.Size = new System.Drawing.Size(167, 22);
            this.mnuScrollBar.Text = "多手持牌";
            this.mnuScrollBar.Click += new System.EventHandler(this.MnuScrollBar_Click);
            // 
            // mnuToolTip
            // 
            this.mnuToolTip.Checked = true;
            this.mnuToolTip.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mnuToolTip.Name = "mnuToolTip";
            this.mnuToolTip.Size = new System.Drawing.Size(167, 22);
            this.mnuToolTip.Text = "显示工具提示 (&T)";
            this.mnuToolTip.Click += new System.EventHandler(this.MnuToolTip_Click);
            // 
            // mnuAppearance
            // 
            this.mnuAppearance.Checked = true;
            this.mnuAppearance.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mnuAppearance.Name = "mnuAppearance";
            this.mnuAppearance.Size = new System.Drawing.Size(167, 22);
            this.mnuAppearance.Text = "外观 (&A)";
            this.mnuAppearance.Click += new System.EventHandler(this.MnuAppearance_Click);
            // 
            // separatorCx
            // 
            this.separatorCx.Name = "separatorCx";
            this.separatorCx.Size = new System.Drawing.Size(164, 6);
            // 
            // mnuSaveGame
            // 
            this.mnuSaveGame.Enabled = false;
            this.mnuSaveGame.Name = "mnuSaveGame";
            this.mnuSaveGame.Size = new System.Drawing.Size(167, 22);
            this.mnuSaveGame.Text = "储存 (&S)";
            this.mnuSaveGame.Click += new System.EventHandler(this.MnuSaveGame_Click);
            // 
            // itmQuit
            // 
            this.itmQuit.Name = "itmQuit";
            this.itmQuit.Size = new System.Drawing.Size(167, 22);
            this.itmQuit.Text = "退出 (&X)";
            this.itmQuit.Click += new System.EventHandler(this.ItmQuit_Click);
            // 
            // mnuChat
            // 
            this.mnuChat.Name = "mnuChat";
            this.mnuChat.Size = new System.Drawing.Size(63, 21);
            this.mnuChat.Text = "聊天 (&T)";
            this.mnuChat.Click += new System.EventHandler(this.MnuChat_Click);
            // 
            // mnuAuto
            // 
            this.mnuAuto.Name = "mnuAuto";
            this.mnuAuto.Size = new System.Drawing.Size(74, 21);
            this.mnuAuto.Text = "☐託管 (&A)";
            this.mnuAuto.Click += new System.EventHandler(this.MnuAuto_Click);
            // 
            // itmHelp
            // 
            this.itmHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuContent,
            this.mnuCanPlay,
            this.mnuControl,
            this.separatorHa,
            this.itmAbout});
            this.itmHelp.Name = "itmHelp";
            this.itmHelp.Size = new System.Drawing.Size(65, 21);
            this.itmHelp.Text = "帮助 (&H)";
            // 
            // mnuContent
            // 
            this.mnuContent.Name = "mnuContent";
            this.mnuContent.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.mnuContent.Size = new System.Drawing.Size(141, 22);
            this.mnuContent.Text = "目彔 (&C)";
            this.mnuContent.Click += new System.EventHandler(this.MnuContent_Click);
            // 
            // mnuCanPlay
            // 
            this.mnuCanPlay.Name = "mnuCanPlay";
            this.mnuCanPlay.Size = new System.Drawing.Size(141, 22);
            this.mnuCanPlay.Text = "出牌規則";
            this.mnuCanPlay.Click += new System.EventHandler(this.MnuCanPlay_Click);
            // 
            // mnuControl
            // 
            this.mnuControl.Name = "mnuControl";
            this.mnuControl.Size = new System.Drawing.Size(141, 22);
            this.mnuControl.Text = "按鍵說明";
            this.mnuControl.Click += new System.EventHandler(this.MnuControl_Click);
            // 
            // separatorHa
            // 
            this.separatorHa.Name = "separatorHa";
            this.separatorHa.Size = new System.Drawing.Size(138, 6);
            // 
            // itmAbout
            // 
            this.itmAbout.Name = "itmAbout";
            this.itmAbout.Size = new System.Drawing.Size(141, 22);
            this.itmAbout.Text = "关于 (&A)";
            this.itmAbout.Click += new System.EventHandler(this.ItmAbout_Click);
            // 
            // mnuStop
            // 
            this.mnuStop.Name = "mnuStop";
            this.mnuStop.Size = new System.Drawing.Size(71, 21);
            this.mnuStop.Text = "⏹停止 (&S)";
            this.mnuStop.Visible = false;
            this.mnuStop.Click += new System.EventHandler(this.MnuStop_Click);
            // 
            // mnuPlayPause
            // 
            this.mnuPlayPause.Name = "mnuPlayPause";
            this.mnuPlayPause.Size = new System.Drawing.Size(72, 21);
            this.mnuPlayPause.Text = "⏸暫停 (&P)";
            this.mnuPlayPause.Click += new System.EventHandler(this.MnuPlayPause_Click);
            // 
            // mnuForward
            // 
            this.mnuForward.Name = "mnuForward";
            this.mnuForward.Size = new System.Drawing.Size(71, 21);
            this.mnuForward.Text = "⏩快進 (&F)";
            this.mnuForward.Visible = false;
            this.mnuForward.Click += new System.EventHandler(this.MnuForward_Click);
            // 
            // timPlayersToCenter
            // 
            this.timPlayersToCenter.Interval = 10;
            this.timPlayersToCenter.Tag = "";
            this.timPlayersToCenter.Tick += new System.EventHandler(this.TimPlayersToCenter_Tick);
            // 
            // timPileToCenter
            // 
            this.timPileToCenter.Interval = 10;
            this.timPileToCenter.Tick += new System.EventHandler(this.TimPileToCenter_Tick);
            // 
            // lblDrawMark
            // 
            this.lblDrawMark.AutoSize = true;
            this.lblDrawMark.Font = new System.Drawing.Font("微軟正黑體 Light", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDrawMark.Location = new System.Drawing.Point(90, 33);
            this.lblDrawMark.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDrawMark.Name = "lblDrawMark";
            this.lblDrawMark.Size = new System.Drawing.Size(34, 34);
            this.lblDrawMark.TabIndex = 5;
            this.lblDrawMark.Text = "+";
            this.lblDrawMark.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblDrawMark.BackColorChanged += new System.EventHandler(this.Control_BackColorChanged);
            // 
            // lblDraw
            // 
            this.lblDraw.AutoSize = true;
            this.lblDraw.Font = new System.Drawing.Font("微軟正黑體 Light", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDraw.Location = new System.Drawing.Point(116, 33);
            this.lblDraw.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDraw.Name = "lblDraw";
            this.lblDraw.Size = new System.Drawing.Size(30, 34);
            this.lblDraw.TabIndex = 6;
            this.lblDraw.Text = "0";
            this.lblDraw.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblDraw.BackColorChanged += new System.EventHandler(this.Control_BackColorChanged);
            // 
            // lblAction
            // 
            this.lblAction.AutoSize = true;
            this.lblAction.Location = new System.Drawing.Point(-5, 536);
            this.lblAction.Name = "lblAction";
            this.lblAction.Size = new System.Drawing.Size(0, 34);
            this.lblAction.TabIndex = 7;
            this.lblAction.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblAction.BackColorChanged += new System.EventHandler(this.Control_BackColorChanged);
            // 
            // timTurn
            // 
            this.timTurn.Interval = 1000;
            this.timTurn.Tag = "4";
            this.timTurn.Tick += new System.EventHandler(this.TimTurn_Tick);
            // 
            // timUno
            // 
            this.timUno.Interval = 1000;
            this.timUno.Tick += new System.EventHandler(this.TimUno_Tick);
            // 
            // toolTip
            // 
            this.toolTip.AutomaticDelay = 0;
            this.toolTip.AutoPopDelay = 5000;
            this.toolTip.InitialDelay = 0;
            this.toolTip.ReshowDelay = 0;
            this.toolTip.ShowAlways = true;
            this.toolTip.ToolTipTitle = " ";
            this.toolTip.Popup += new System.Windows.Forms.PopupEventHandler(this.ToolTip_Popup);
            // 
            // lblWatch
            // 
            this.lblWatch.AutoSize = true;
            this.lblWatch.Font = new System.Drawing.Font("微軟正黑體 Light", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWatch.Location = new System.Drawing.Point(820, 33);
            this.lblWatch.Name = "lblWatch";
            this.lblWatch.Size = new System.Drawing.Size(43, 17);
            this.lblWatch.TabIndex = 11;
            this.lblWatch.Text = "0°0′0″";
            this.toolTip.SetToolTip(this.lblWatch, "游戏时长");
            this.lblWatch.Visible = false;
            this.lblWatch.BackColorChanged += new System.EventHandler(this.Control_BackColorChanged);
            this.lblWatch.Resize += new System.EventHandler(this.LblWatch_Resize);
            // 
            // hPlayer
            // 
            this.hPlayer.Location = new System.Drawing.Point(0, 214);
            this.hPlayer.Name = "hPlayer";
            this.hPlayer.Size = new System.Drawing.Size(80, 14);
            this.hPlayer.TabIndex = 9;
            this.hPlayer.Visible = false;
            this.hPlayer.Scroll += new System.Windows.Forms.ScrollEventHandler(this.HPlayer_Scroll);
            this.hPlayer.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
            // 
            // pnlPlayer
            // 
            this.pnlPlayer.Location = new System.Drawing.Point(0, 214);
            this.pnlPlayer.Name = "pnlPlayer";
            this.pnlPlayer.Size = new System.Drawing.Size(90, 132);
            this.pnlPlayer.TabIndex = 10;
            this.pnlPlayer.BackColorChanged += new System.EventHandler(this.Control_BackColorChanged);
            // 
            // timWatch
            // 
            this.timWatch.Interval = 1000;
            this.timWatch.Tick += new System.EventHandler(this.TimWatch_Tick);
            // 
            // pnlMovingCards
            // 
            this.pnlMovingCards.AutoSize = true;
            this.pnlMovingCards.Location = new System.Drawing.Point(97, 66);
            this.pnlMovingCards.Name = "pnlMovingCards";
            this.pnlMovingCards.Size = new System.Drawing.Size(64, 99);
            this.pnlMovingCards.TabIndex = 12;
            // 
            // timThinking
            // 
            this.timThinking.Interval = 1000;
            this.timThinking.Tag = "4";
            this.timThinking.Tick += new System.EventHandler(this.TimThinking_Tick);
            // 
            // rdoUno
            // 
            this.rdoUno.Appearance = System.Windows.Forms.Appearance.Button;
            this.rdoUno.AutoSize = true;
            this.rdoUno.FlatAppearance.BorderSize = 0;
            this.rdoUno.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rdoUno.Font = new System.Drawing.Font("微軟正黑體 Light", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoUno.Location = new System.Drawing.Point(0, 167);
            this.rdoUno.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.rdoUno.Name = "rdoUno";
            this.rdoUno.Size = new System.Drawing.Size(87, 44);
            this.rdoUno.TabIndex = 13;
            this.rdoUno.Text = "UNO";
            this.rdoUno.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rdoUno.UseVisualStyleBackColor = true;
            this.rdoUno.Visible = false;
            this.rdoUno.BackColorChanged += new System.EventHandler(this.Control_BackColorChanged);
            // 
            // Uno
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 34F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(890, 658);
            this.Controls.Add(this.rdoUno);
            this.Controls.Add(this.pnlMovingCards);
            this.Controls.Add(this.lblWatch);
            this.Controls.Add(this.hPlayer);
            this.Controls.Add(this.pnlPlayer);
            this.Controls.Add(this.lblAction);
            this.Controls.Add(this.lblDraw);
            this.Controls.Add(this.lblDrawMark);
            this.Controls.Add(this.mnuGame);
            this.Controls.Add(this.lblPile);
            this.Font = new System.Drawing.Font("微軟正黑體", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainMenuStrip = this.mnuGame;
            this.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.Name = "Uno";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Tag = "0";
            this.Text = "Uno";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Uno_FormClosing);
            this.Load += new System.EventHandler(this.Uno_Load);
            this.Click += new System.EventHandler(this.Uno_Click);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
            this.Resize += new System.EventHandler(this.Uno_Resize);
            this.mnuCheating.ResumeLayout(false);
            this.mnuGame.ResumeLayout(false);
            this.mnuGame.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblPile;
		private System.Windows.Forms.Timer timPileToPlayers;
		private System.Windows.Forms.MenuStrip mnuGame;
		private System.Windows.Forms.ToolStripMenuItem itmGame;
		private System.Windows.Forms.ToolStripMenuItem itmQuit;
		private System.Windows.Forms.Timer timPlayersToCenter;
		private System.Windows.Forms.ToolStripMenuItem itmHelp;
		private System.Windows.Forms.ToolStripMenuItem itmAbout;
        private System.Windows.Forms.Timer timPileToCenter;
		private System.Windows.Forms.Label lblDrawMark;
		private System.Windows.Forms.Label lblDraw;
        private System.Windows.Forms.Label lblAction;
        private System.Windows.Forms.Timer timTurn;
        private System.Windows.Forms.Timer timUno;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.HScrollBar hPlayer;
        private System.Windows.Forms.Panel pnlPlayer;
        private System.Windows.Forms.ToolStripMenuItem mnuChat;
        private System.Windows.Forms.ToolStripMenuItem mnuRank;
        public System.Windows.Forms.ToolStripMenuItem mnuByColor;
        public System.Windows.Forms.ToolStripMenuItem mnuByNumber;
        private System.Windows.Forms.ToolStripMenuItem mnuNew;
        private System.Windows.Forms.ToolStripSeparator separatorNc;
        private System.Windows.Forms.ToolStripSeparator separatorCx;
        private System.Windows.Forms.ToolStripMenuItem mnuContent;
        private System.Windows.Forms.ToolStripMenuItem mnuToolTip;
        private System.Windows.Forms.Timer timWatch;
        private System.Windows.Forms.Label lblWatch;
        private System.Windows.Forms.ToolStripMenuItem mnuScrollBar;
        private System.Windows.Forms.ToolStripMenuItem mnuSaveGame;
        private System.Windows.Forms.Panel pnlMovingCards;
        private System.Windows.Forms.ToolStripMenuItem mnuAppearance;
        private System.Windows.Forms.Timer timThinking;
        private System.Windows.Forms.RadioButton rdoUno;
        private System.Windows.Forms.ContextMenuStrip mnuCheating;
        private System.Windows.Forms.ToolStripMenuItem mnuCheat;
        private System.Windows.Forms.ToolStripMenuItem mnuColor;
        private System.Windows.Forms.ToolStripMenuItem mnuNumber;
        private System.Windows.Forms.ToolStripMenuItem mnuAuto;
        private System.Windows.Forms.ToolStripMenuItem mnuCanPlay;
        private System.Windows.Forms.ToolStripMenuItem mnuControl;
        private System.Windows.Forms.ToolStripSeparator separatorHa;
        private System.Windows.Forms.ToolStripMenuItem mnuForward;
        private System.Windows.Forms.ToolStripMenuItem mnuStop;
        private System.Windows.Forms.ToolStripMenuItem mnuPlayPause;
    }
}

