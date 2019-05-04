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
            this.mnuColor = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRadioBox = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRightClick = new System.Windows.Forms.ToolStripMenuItem();
            this.separatorCx = new System.Windows.Forms.ToolStripSeparator();
            this.mnuSaveGame = new System.Windows.Forms.ToolStripMenuItem();
            this.itmQuit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuChat = new System.Windows.Forms.ToolStripMenuItem();
            this.itmHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuContent = new System.Windows.Forms.ToolStripMenuItem();
            this.separatorHa = new System.Windows.Forms.ToolStripSeparator();
            this.itmAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlCtrl = new System.Windows.Forms.Panel();
            this.btnJumpin = new System.Windows.Forms.Button();
            this.rdoUno = new System.Windows.Forms.RadioButton();
            this.btnChallenge = new System.Windows.Forms.Button();
            this.btnDraw = new System.Windows.Forms.Button();
            this.btnPlay = new System.Windows.Forms.Button();
            this.timPlayersToCenter = new System.Windows.Forms.Timer(this.components);
            this.timPileToCenter = new System.Windows.Forms.Timer(this.components);
            this.lblDrawMark = new System.Windows.Forms.Label();
            this.lblDraw = new System.Windows.Forms.Label();
            this.lblAction = new System.Windows.Forms.Label();
            this.timTurn = new System.Windows.Forms.Timer(this.components);
            this.timUno = new System.Windows.Forms.Timer(this.components);
            this.timChallenge = new System.Windows.Forms.Timer(this.components);
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.lblWatch = new System.Windows.Forms.Label();
            this.hPlayer = new System.Windows.Forms.HScrollBar();
            this.pnlPlayer = new System.Windows.Forms.Panel();
            this.timWatch = new System.Windows.Forms.Timer(this.components);
            this.pnlMovingCards = new System.Windows.Forms.Panel();
            this.mnuGame.SuspendLayout();
            this.pnlCtrl.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblPile
            // 
            this.lblPile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPile.Font = new System.Drawing.Font("GulimChe", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPile.Location = new System.Drawing.Point(0, 30);
            this.lblPile.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPile.Name = "lblPile";
            this.lblPile.Size = new System.Drawing.Size(90, 120);
            this.lblPile.TabIndex = 0;
            this.lblPile.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblPile.BackColorChanged += new System.EventHandler(this.Control_BackColorChanged);
            this.lblPile.Click += new System.EventHandler(this.LblPile_Click);
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
            this.itmHelp});
            this.mnuGame.Location = new System.Drawing.Point(0, 0);
            this.mnuGame.Name = "mnuGame";
            this.mnuGame.Padding = new System.Windows.Forms.Padding(5, 3, 0, 3);
            this.mnuGame.Size = new System.Drawing.Size(890, 24);
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
            this.mnuColor,
            this.separatorCx,
            this.mnuSaveGame,
            this.itmQuit});
            this.itmGame.Name = "itmGame";
            this.itmGame.Size = new System.Drawing.Size(67, 18);
            this.itmGame.Text = "游戏 (&G)";
            // 
            // mnuNew
            // 
            this.mnuNew.Name = "mnuNew";
            this.mnuNew.Size = new System.Drawing.Size(170, 22);
            this.mnuNew.Text = "开局 (&N)";
            this.mnuNew.Click += new System.EventHandler(this.MnuNew_Click);
            // 
            // separatorNc
            // 
            this.separatorNc.Name = "separatorNc";
            this.separatorNc.Size = new System.Drawing.Size(167, 6);
            // 
            // mnuRank
            // 
            this.mnuRank.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuByColor,
            this.mnuByNumber});
            this.mnuRank.Name = "mnuRank";
            this.mnuRank.Size = new System.Drawing.Size(170, 22);
            this.mnuRank.Text = "排序 (&R)";
            // 
            // mnuByColor
            // 
            this.mnuByColor.Checked = true;
            this.mnuByColor.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mnuByColor.Name = "mnuByColor";
            this.mnuByColor.Size = new System.Drawing.Size(109, 22);
            this.mnuByColor.Text = "按颜色";
            this.mnuByColor.Click += new System.EventHandler(this.MnuRank_Click);
            // 
            // mnuByNumber
            // 
            this.mnuByNumber.Name = "mnuByNumber";
            this.mnuByNumber.Size = new System.Drawing.Size(109, 22);
            this.mnuByNumber.Text = "按数字";
            this.mnuByNumber.Click += new System.EventHandler(this.MnuRank_Click);
            // 
            // mnuScrollBar
            // 
            this.mnuScrollBar.Checked = true;
            this.mnuScrollBar.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mnuScrollBar.Name = "mnuScrollBar";
            this.mnuScrollBar.Size = new System.Drawing.Size(170, 22);
            this.mnuScrollBar.Text = "多手持牌";
            this.mnuScrollBar.Click += new System.EventHandler(this.MnuScrollBar_Click);
            // 
            // mnuToolTip
            // 
            this.mnuToolTip.Checked = true;
            this.mnuToolTip.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mnuToolTip.Name = "mnuToolTip";
            this.mnuToolTip.Size = new System.Drawing.Size(170, 22);
            this.mnuToolTip.Text = "显示工具提示 (&T)";
            this.mnuToolTip.Click += new System.EventHandler(this.MnuToolTip_Click);
            // 
            // mnuColor
            // 
            this.mnuColor.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuRadioBox,
            this.mnuRightClick});
            this.mnuColor.Name = "mnuColor";
            this.mnuColor.Size = new System.Drawing.Size(170, 22);
            this.mnuColor.Text = "变色 (&C)";
            // 
            // mnuRadioBox
            // 
            this.mnuRadioBox.Checked = true;
            this.mnuRadioBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mnuRadioBox.Name = "mnuRadioBox";
            this.mnuRadioBox.Size = new System.Drawing.Size(136, 22);
            this.mnuRadioBox.Text = "使用单选框";
            this.mnuRadioBox.Click += new System.EventHandler(this.MnuColor_Click);
            // 
            // mnuRightClick
            // 
            this.mnuRightClick.Name = "mnuRightClick";
            this.mnuRightClick.Size = new System.Drawing.Size(136, 22);
            this.mnuRightClick.Text = "右击 [出牌]";
            this.mnuRightClick.Click += new System.EventHandler(this.MnuColor_Click);
            // 
            // separatorCx
            // 
            this.separatorCx.Name = "separatorCx";
            this.separatorCx.Size = new System.Drawing.Size(167, 6);
            // 
            // mnuSaveGame
            // 
            this.mnuSaveGame.Enabled = false;
            this.mnuSaveGame.Name = "mnuSaveGame";
            this.mnuSaveGame.Size = new System.Drawing.Size(170, 22);
            this.mnuSaveGame.Text = "储存 (&S)";
            this.mnuSaveGame.Click += new System.EventHandler(this.MnuSaveGame_Click);
            // 
            // itmQuit
            // 
            this.itmQuit.Name = "itmQuit";
            this.itmQuit.Size = new System.Drawing.Size(170, 22);
            this.itmQuit.Text = "退出 (&X)";
            this.itmQuit.Click += new System.EventHandler(this.ItmQuit_Click);
            // 
            // mnuChat
            // 
            this.mnuChat.Name = "mnuChat";
            this.mnuChat.Size = new System.Drawing.Size(65, 18);
            this.mnuChat.Text = "聊天 (&T)";
            this.mnuChat.Click += new System.EventHandler(this.MnuChat_Click);
            // 
            // itmHelp
            // 
            this.itmHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuContent,
            this.separatorHa,
            this.itmAbout});
            this.itmHelp.Name = "itmHelp";
            this.itmHelp.Size = new System.Drawing.Size(67, 18);
            this.itmHelp.Text = "帮助 (&H)";
            // 
            // mnuContent
            // 
            this.mnuContent.Name = "mnuContent";
            this.mnuContent.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.mnuContent.Size = new System.Drawing.Size(139, 22);
            this.mnuContent.Text = "目彔 (&C)";
            this.mnuContent.Click += new System.EventHandler(this.MnuContent_Click);
            // 
            // separatorHa
            // 
            this.separatorHa.Name = "separatorHa";
            this.separatorHa.Size = new System.Drawing.Size(136, 6);
            // 
            // itmAbout
            // 
            this.itmAbout.Name = "itmAbout";
            this.itmAbout.Size = new System.Drawing.Size(139, 22);
            this.itmAbout.Text = "关于 (&A)";
            this.itmAbout.Click += new System.EventHandler(this.ItmAbout_Click);
            // 
            // pnlCtrl
            // 
            this.pnlCtrl.Controls.Add(this.btnJumpin);
            this.pnlCtrl.Controls.Add(this.rdoUno);
            this.pnlCtrl.Controls.Add(this.btnChallenge);
            this.pnlCtrl.Controls.Add(this.btnDraw);
            this.pnlCtrl.Controls.Add(this.btnPlay);
            this.pnlCtrl.Location = new System.Drawing.Point(0, 153);
            this.pnlCtrl.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pnlCtrl.Name = "pnlCtrl";
            this.pnlCtrl.Size = new System.Drawing.Size(399, 36);
            this.pnlCtrl.TabIndex = 4;
            this.pnlCtrl.Visible = false;
            // 
            // btnJumpin
            // 
            this.btnJumpin.FlatAppearance.BorderSize = 0;
            this.btnJumpin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnJumpin.Location = new System.Drawing.Point(253, 0);
            this.btnJumpin.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnJumpin.Name = "btnJumpin";
            this.btnJumpin.Size = new System.Drawing.Size(76, 36);
            this.btnJumpin.TabIndex = 4;
            this.btnJumpin.Text = "点断";
            this.btnJumpin.UseVisualStyleBackColor = true;
            this.btnJumpin.Visible = false;
            this.btnJumpin.BackColorChanged += new System.EventHandler(this.Control_BackColorChanged);
            this.btnJumpin.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
            // 
            // rdoUno
            // 
            this.rdoUno.Appearance = System.Windows.Forms.Appearance.Button;
            this.rdoUno.FlatAppearance.BorderSize = 0;
            this.rdoUno.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rdoUno.Location = new System.Drawing.Point(337, 0);
            this.rdoUno.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.rdoUno.Name = "rdoUno";
            this.rdoUno.Size = new System.Drawing.Size(62, 36);
            this.rdoUno.TabIndex = 3;
            this.rdoUno.TabStop = true;
            this.rdoUno.Text = "UNO";
            this.rdoUno.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rdoUno.UseVisualStyleBackColor = true;
            this.rdoUno.BackColorChanged += new System.EventHandler(this.Control_BackColorChanged);
            this.rdoUno.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
            // 
            // btnChallenge
            // 
            this.btnChallenge.FlatAppearance.BorderSize = 0;
            this.btnChallenge.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnChallenge.Location = new System.Drawing.Point(169, 0);
            this.btnChallenge.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnChallenge.Name = "btnChallenge";
            this.btnChallenge.Size = new System.Drawing.Size(76, 36);
            this.btnChallenge.TabIndex = 2;
            this.btnChallenge.Text = "检举";
            this.btnChallenge.UseVisualStyleBackColor = true;
            this.btnChallenge.Visible = false;
            this.btnChallenge.BackColorChanged += new System.EventHandler(this.Control_BackColorChanged);
            this.btnChallenge.Click += new System.EventHandler(this.BtnChallenge_Click);
            this.btnChallenge.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
            // 
            // btnDraw
            // 
            this.btnDraw.FlatAppearance.BorderSize = 0;
            this.btnDraw.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDraw.Location = new System.Drawing.Point(84, 0);
            this.btnDraw.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnDraw.Name = "btnDraw";
            this.btnDraw.Size = new System.Drawing.Size(77, 36);
            this.btnDraw.TabIndex = 1;
            this.btnDraw.Text = "摸牌";
            this.btnDraw.UseVisualStyleBackColor = true;
            this.btnDraw.BackColorChanged += new System.EventHandler(this.Control_BackColorChanged);
            this.btnDraw.Click += new System.EventHandler(this.BtnDraw_Click);
            this.btnDraw.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
            // 
            // btnPlay
            // 
            this.btnPlay.FlatAppearance.BorderSize = 0;
            this.btnPlay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPlay.Location = new System.Drawing.Point(0, 0);
            this.btnPlay.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(76, 36);
            this.btnPlay.TabIndex = 0;
            this.btnPlay.Text = "出牌";
            this.btnPlay.UseVisualStyleBackColor = false;
            this.btnPlay.Visible = false;
            this.btnPlay.BackColorChanged += new System.EventHandler(this.Control_BackColorChanged);
            this.btnPlay.Click += new System.EventHandler(this.BtnPlay_Click);
            this.btnPlay.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
            this.btnPlay.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BtnPlay_MouseDown);
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
            this.lblDrawMark.Font = new System.Drawing.Font("GulimChe", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDrawMark.Location = new System.Drawing.Point(90, 30);
            this.lblDrawMark.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDrawMark.Name = "lblDrawMark";
            this.lblDrawMark.Size = new System.Drawing.Size(26, 27);
            this.lblDrawMark.TabIndex = 5;
            this.lblDrawMark.Text = "+";
            this.lblDrawMark.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblDrawMark.BackColorChanged += new System.EventHandler(this.Control_BackColorChanged);
            // 
            // lblDraw
            // 
            this.lblDraw.AutoSize = true;
            this.lblDraw.Font = new System.Drawing.Font("GulimChe", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDraw.Location = new System.Drawing.Point(116, 30);
            this.lblDraw.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDraw.Name = "lblDraw";
            this.lblDraw.Size = new System.Drawing.Size(26, 27);
            this.lblDraw.TabIndex = 6;
            this.lblDraw.Text = "0";
            this.lblDraw.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblDraw.BackColorChanged += new System.EventHandler(this.Control_BackColorChanged);
            // 
            // lblAction
            // 
            this.lblAction.AutoSize = true;
            this.lblAction.Location = new System.Drawing.Point(-5, 489);
            this.lblAction.Name = "lblAction";
            this.lblAction.Size = new System.Drawing.Size(0, 27);
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
            // timChallenge
            // 
            this.timChallenge.Interval = 1000;
            this.timChallenge.Tick += new System.EventHandler(this.TimChallenge_Tick);
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
            this.lblWatch.Font = new System.Drawing.Font("GulimChe", 10F);
            this.lblWatch.Location = new System.Drawing.Point(820, 30);
            this.lblWatch.Name = "lblWatch";
            this.lblWatch.Size = new System.Drawing.Size(70, 14);
            this.lblWatch.TabIndex = 11;
            this.lblWatch.Text = "0°0′0″";
            this.toolTip.SetToolTip(this.lblWatch, "游戏时长");
            this.lblWatch.Visible = false;
            this.lblWatch.BackColorChanged += new System.EventHandler(this.Control_BackColorChanged);
            this.lblWatch.Resize += new System.EventHandler(this.LblWatch_Resize);
            // 
            // hPlayer
            // 
            this.hPlayer.Location = new System.Drawing.Point(0, 195);
            this.hPlayer.Name = "hPlayer";
            this.hPlayer.Size = new System.Drawing.Size(80, 14);
            this.hPlayer.TabIndex = 9;
            this.hPlayer.Visible = false;
            this.hPlayer.Scroll += new System.Windows.Forms.ScrollEventHandler(this.HPlayer_Scroll);
            this.hPlayer.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
            // 
            // pnlPlayer
            // 
            this.pnlPlayer.Font = new System.Drawing.Font("MS Gothic", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlPlayer.Location = new System.Drawing.Point(0, 195);
            this.pnlPlayer.Name = "pnlPlayer";
            this.pnlPlayer.Size = new System.Drawing.Size(90, 120);
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
            this.pnlMovingCards.Location = new System.Drawing.Point(97, 60);
            this.pnlMovingCards.Name = "pnlMovingCards";
            this.pnlMovingCards.Size = new System.Drawing.Size(64, 90);
            this.pnlMovingCards.TabIndex = 12;
            // 
            // Uno
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 27F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(890, 600);
            this.Controls.Add(this.pnlMovingCards);
            this.Controls.Add(this.lblWatch);
            this.Controls.Add(this.hPlayer);
            this.Controls.Add(this.pnlPlayer);
            this.Controls.Add(this.lblAction);
            this.Controls.Add(this.lblDraw);
            this.Controls.Add(this.lblDrawMark);
            this.Controls.Add(this.pnlCtrl);
            this.Controls.Add(this.mnuGame);
            this.Controls.Add(this.lblPile);
            this.Font = new System.Drawing.Font("MingLiU", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainMenuStrip = this.mnuGame;
            this.Margin = new System.Windows.Forms.Padding(8, 6, 8, 6);
            this.Name = "Uno";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Tag = "0";
            this.Text = "Uno";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Uno_FormClosing);
            this.Load += new System.EventHandler(this.Uno_Load);
            this.Click += new System.EventHandler(this.Uno_Click);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Control_KeyDown);
            this.Resize += new System.EventHandler(this.Uno_Resize);
            this.mnuGame.ResumeLayout(false);
            this.mnuGame.PerformLayout();
            this.pnlCtrl.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblPile;
		private System.Windows.Forms.Timer timPileToPlayers;
		private System.Windows.Forms.MenuStrip mnuGame;
		private System.Windows.Forms.ToolStripMenuItem itmGame;
		private System.Windows.Forms.ToolStripMenuItem itmQuit;
		private System.Windows.Forms.Panel pnlCtrl;
		private System.Windows.Forms.Button btnPlay;
		private System.Windows.Forms.Button btnChallenge;
		private System.Windows.Forms.Button btnDraw;
		private System.Windows.Forms.RadioButton rdoUno;
		private System.Windows.Forms.Timer timPlayersToCenter;
		private System.Windows.Forms.ToolStripMenuItem itmHelp;
		private System.Windows.Forms.ToolStripMenuItem itmAbout;
        private System.Windows.Forms.Timer timPileToCenter;
		private System.Windows.Forms.Label lblDrawMark;
		private System.Windows.Forms.Label lblDraw;
        private System.Windows.Forms.Label lblAction;
        private System.Windows.Forms.Timer timTurn;
        private System.Windows.Forms.Timer timUno;
        private System.Windows.Forms.Timer timChallenge;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Button btnJumpin;
        private System.Windows.Forms.HScrollBar hPlayer;
        private System.Windows.Forms.Panel pnlPlayer;
        private System.Windows.Forms.ToolStripMenuItem mnuChat;
        private System.Windows.Forms.ToolStripMenuItem mnuRank;
        public System.Windows.Forms.ToolStripMenuItem mnuByColor;
        public System.Windows.Forms.ToolStripMenuItem mnuByNumber;
        private System.Windows.Forms.ToolStripMenuItem mnuColor;
        public System.Windows.Forms.ToolStripMenuItem mnuRadioBox;
        public System.Windows.Forms.ToolStripMenuItem mnuRightClick;
        private System.Windows.Forms.ToolStripMenuItem mnuNew;
        private System.Windows.Forms.ToolStripSeparator separatorNc;
        private System.Windows.Forms.ToolStripSeparator separatorCx;
        private System.Windows.Forms.ToolStripMenuItem mnuContent;
        private System.Windows.Forms.ToolStripSeparator separatorHa;
        private System.Windows.Forms.ToolStripMenuItem mnuToolTip;
        private System.Windows.Forms.Timer timWatch;
        private System.Windows.Forms.Label lblWatch;
        private System.Windows.Forms.ToolStripMenuItem mnuScrollBar;
        private System.Windows.Forms.ToolStripMenuItem mnuSaveGame;
        private System.Windows.Forms.Panel pnlMovingCards;
    }
}

