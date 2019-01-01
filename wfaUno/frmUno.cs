using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace wfaUno {
	public partial class FrmUno : Form {
		public class Card {
			public byte color = UnoColor.MAX_VALUE;
			public byte number = UnoNumber.MAX_VALUE;
		}

		public class Cards {
			public int[,] uno = new int[UnoColor.MAX_VALUE + 1, UnoNumber.MAX_VALUE + 1];
		}

		public class MovingCard {
			public byte color = 0, number = 0, player = 0;
			public bool drew = false, one = false, playing = false, quickly = false, unoDraw = false;
			public int downpour = -1, progress = 0;
		}

        public class UnoSize
        {
            public const int WIDTH = 90;
            public const int HEIGHT = 120;
        }

        public class UnoColor {
            public const byte RED = 0;
            public const byte YELLOW = 1;
            public const byte GREEN = 2;
            public const byte BLUE = 3;
            public const byte MAGENTA = 4;
            public const byte BLACK = 5;
            public const byte CYAN = 6;
            public const byte WHITE = 7;
            public const byte MAX_VALUE = 7;
        }

        public class UnoNumber {
            public const byte SKIP = 10;
            public const byte REVERSE = 11;
            public const byte DRAW_2 = 12;
            public const byte DISCARD_ALL = 13;
            public const byte TRADE_HANDS = 14;
            public const byte BLANK = 15;
            public const byte WILD = 16;
            public const byte WILD_DOWNPOUR_DRAW_1 = 17;
            public const byte WILD_DOWNPOUR_DRAW_2 = 18;
            public const byte WILD_DOWNPOUR_DRAW_4 = 19;
            public const byte WILD_DRAW_4 = 20;
            public const byte WILD_HITFIRE = 21;
            public const byte NULL = 22;
            public const byte MAX_VALUE = 22;
        }

        public class UnoNumberName
        {
            public const string SKIP = "Ø";
            public const string REVERSE = "^v";
            public const string DRAW_2 = "+2";
            public const string DISCARD_ALL = "×";
            public const string TRADE_HANDS = "<>";
            public const string WILD = "::";
            public const string WILD_DOWNPOUR_DRAW_1 = "!1";
            public const string WILD_DOWNPOUR_DRAW_2 = "!2";
            public const string WILD_DOWNPOUR_DRAW_4 = "!4";
            public const string WILD_DRAW_4 = "+4";
            public const string WILD_HITFIRE = "+?";
            public const string NULL = "   ";
        }

        bool reverse = false, isSelectingCards = false;
        Cards BoxCards = new Cards();
        Cards[] Players = new Cards[4];
        frmOptions form;
        int gametime = 0, height = 0, skip = 0;
        int[] skips = new int[4];
        Label[] lblCounts = new Label[4];
        public Label[] lblPlayers = new Label[4];
        List<CheckBox> chkPlayer = new List<CheckBox>();
        List<Label> lblCards = new List<Label>(), lblMoving = new List<Label>();
        MovingCard movingCard = new MovingCard();

        void Action(byte player, string msg)
        {
            lblAction.Text = msg;
            switch (player)
            {
                case 0: lblAction.Location = new Point(Width / 2 - lblAction.Width / 2, lblCounts[0].Top - lblAction.Height); break;
                case 1: lblAction.Location = new Point(lblPlayers[1].Left + UnoSize.WIDTH, height / 2 - lblAction.Height / 2); break;
                case 2: lblAction.Location = new Point(Width / 2 - lblAction.Width / 2, lblCounts[2].Top + lblCounts[2].Height); break;
                case 3: lblAction.Location = new Point(lblPlayers[3].Left - lblAction.Width, height / 2 - lblAction.Height / 2); break;
            }
        }

		void AddChkPlayer(int count = 1) {
			for (int i = 1; i <= count; i++) {
				int length = chkPlayer.ToArray().Length;
				chkPlayer.Add(new CheckBox());
				pnlPlayer.Controls.Add(chkPlayer[length]);
				chkPlayer[length].AutoSize = false;
				chkPlayer[length].Appearance = Appearance.Button;
				chkPlayer[length].BringToFront();
				chkPlayer[length].CheckedChanged += new EventHandler(ChkPlayer_CheckedChanged);
                chkPlayer[length].Enter += new EventHandler(ChkPlayer_Enter);
                chkPlayer[length].FlatStyle = FlatStyle.Flat;
                chkPlayer[length].Font = new Font(chkPlayer[length].Font.FontFamily, 42);
                chkPlayer[length].ForeColor = Color.White;
                chkPlayer[length].KeyDown += new KeyEventHandler(Control_KeyDown);
                chkPlayer[length].MouseDown += new MouseEventHandler(ChkPlayer_MouseDown);
                chkPlayer[length].MouseEnter += new EventHandler(ChkPlayer_MouseEnter);
                chkPlayer[length].MouseLeave += new EventHandler(ChkPlayer_MouseLeave);
                chkPlayer[length].MouseMove += new MouseEventHandler(ChkPlayer_MouseMove);
                chkPlayer[length].MouseUp += new MouseEventHandler(ChkPlayer_MouseUp);
                chkPlayer[length].MouseWheel += new MouseEventHandler(ChkPlayer_MouseWheel);
                chkPlayer[length].Size = new Size(UnoSize.WIDTH, UnoSize.HEIGHT);
                chkPlayer[length].Tag = length;
				chkPlayer[length].TextAlign = ContentAlignment.MiddleCenter;
            }
		}

        void AddDraw(int count)
        {
            lblDraw.Text = int.Parse(lblDraw.Text) + (form.mnuDoubleDraw.Checked ? 2 : 1) * count + "";
        }

		void AddLabel(List<Label> label, int count = 1) {
			for (int c = 1; c <= count; c++) {
				int length = label.ToArray().Length;
				label.Add(new Label());
                Controls.Add(label[length]);
                label[length].AutoSize = false;
                label[length].BorderStyle = BorderStyle.FixedSingle;
                label[length].BringToFront();
                label[length].Font = new Font("MS Gothic", 42);
                label[length].ForeColor = Color.White;
                label[length].Location = new Point(-UnoSize.WIDTH, -UnoSize.HEIGHT);
                label[length].MouseEnter += new EventHandler(Label_MouseEnter);
                label[length].MouseLeave += new EventHandler(Label_MouseLeave);
                label[length].Size = new Size(UnoSize.WIDTH, UnoSize.HEIGHT);
                label[length].Text = UnoNumberName.NULL;
                label[length].TextAlign = ContentAlignment.MiddleCenter;
			}
		}

		Card[] Ai(byte player) {
            byte backColor = GetColorId(BackColor);
			List<Card> cards = new List<Card>();
            int quantityColor = GetQuantityByColor(player, backColor), quantityNumber = GetQuantityByNumber(player, GetNumberId(lblCards[1].Text));
            Card bestCard = new Card();
            if (!form.mnuPlayOrDrawAll.Checked
                && lblCards[1].Text == UnoNumberName.DRAW_2 && int.Parse(lblDraw.Text) >= 2)
                {
                    if (quantityNumber > 0)
                        bestCard.number = UnoNumber.DRAW_2;
                    else
                    {
                        bestCard.color = UnoColor.BLACK;
                        bestCard.number = UnoNumber.WILD_DRAW_4;
                    }
                }
            else if (!form.mnuPlayOrDrawAll.Checked
                && lblCards[1].Text == UnoNumberName.WILD_DRAW_4 && int.Parse(lblDraw.Text) >= 4)
            {
                bestCard.color = UnoColor.BLACK;
                bestCard.number = UnoNumber.WILD_DRAW_4;
            }
            else if (!form.mnuPlayOrDrawAll.Checked
                && lblCards[1].Text == UnoNumberName.WILD_HITFIRE && int.Parse(lblDraw.Text) > 0)
            {
                bestCard.color = UnoColor.BLACK;
                bestCard.number = UnoNumber.WILD_HITFIRE;
            }
            else if (quantityColor == 0 && quantityNumber == 0)
            {
                if (Players[player].uno[UnoColor.MAGENTA, UnoNumber.BLANK] > 0)
                {
                    bestCard.color = UnoColor.MAGENTA;
                    bestCard.number = UnoNumber.BLANK;
                }
                else
                    for (byte n = UnoNumber.BLANK; n <= UnoNumber.MAX_VALUE; n++)
                        if (Players[player].uno[UnoColor.BLACK, n] > 0)
                        {
                            bestCard.color = UnoColor.BLACK;
                            bestCard.number = n;
                            break;
                        }
            }
            else if (quantityColor >= quantityNumber)
            {
                if (Players[player].uno[backColor, UnoNumber.DISCARD_ALL] > 0)
                {
                    bestCard.color = backColor;
                    bestCard.number = UnoNumber.DISCARD_ALL;
                }
                else
                    for (byte n = 0; n <= UnoNumber.BLANK; n++)
                        if (Players[player].uno[backColor, n] > 0)
                        {
                            bestCard.color = backColor;
                            bestCard.number = n;
                            break;
                        }
            }
            else bestCard.number = GetNumberId(lblCards[1].Text);
            int mostQuantity = 0, quantity = 0;
            if (bestCard.number < UnoNumber.BLANK
                || bestCard.number == UnoNumber.BLANK && Players[player].uno[UnoColor.BLACK, UnoNumber.BLANK] <= 0)
            {
                if (form.mnuPairs.Checked)
                {
                    for (byte color = UnoColor.RED; color <= UnoColor.BLUE; color++)
                        if (Players[player].uno[color, bestCard.number] > 0)
                        {
                            quantity = GetQuantityByColor(player, color);
                            if (quantity > mostQuantity)
                            {
                                mostQuantity = quantity;
                                movingCard.color = color;
                            }
                            Card card = new Card
                            {
                                color = color,
                                number = bestCard.number
                            };
                            for (byte c = 1; c <= Players[player].uno[color, card.number]; c++)
                                cards.Add(card);
                        }
                    if (bestCard.number == UnoNumber.BLANK && Players[player].uno[UnoColor.MAGENTA, UnoNumber.BLANK] > 0)
                    {
                        if (cards.ToArray().Length == 0)
                            movingCard.color = backColor;
                        Card card = new Card
                        {
                            color = UnoColor.MAGENTA,
                            number = UnoNumber.BLANK
                        };
                        for (byte c = 1; c <= Players[player].uno[UnoColor.MAGENTA, UnoNumber.BLANK]; c++)
                            cards.Add(card);
                    }
                }
                else
                {
                    if (bestCard.color == UnoColor.WHITE)
                    {
                        for (byte c = UnoColor.RED; c <= UnoColor.BLUE; c++)
                        {
                            if (Players[player].uno[c, bestCard.number] > 0)
                            {
                                quantity = GetQuantityByColor(player, c);
                                if (quantity > mostQuantity)
                                {
                                    mostQuantity = quantity;
                                    movingCard.color = c;
                                }
                            }
                        }
                        bestCard.color = movingCard.color;
                        cards.Add(bestCard);
                    }
                    else if (Players[player].uno[bestCard.color, bestCard.number] > 0)
                    {
                        movingCard.color = bestCard.color;
                        cards.Add(bestCard);
                    }
                }
                if (bestCard.number == UnoNumber.DISCARD_ALL)
                {
                    byte color = lblCards[1].Text == UnoNumberName.DISCARD_ALL ? movingCard.color : backColor;
                    for (byte n = 0; n <= UnoNumber.MAX_VALUE; n++)
                    {
                        if (n == UnoNumber.DISCARD_ALL)
                            continue;
                        for (int c = 1; c <= Players[player].uno[color, n]; c++)
                        {
                            Card card = new Card
                            {
                                color = color,
                                number = n
                            };
                            cards.Add(card);
                        }
                    }
                }
            }
            else
            {
                bestCard.color = UnoColor.BLACK;
                for (byte c = UnoColor.RED; c <= UnoColor.BLUE; c++)
                {
                    quantity = GetQuantityByColor(player, c);
                    if (quantity > mostQuantity)
                    {
                        mostQuantity = quantity;
                        movingCard.color = c;
                    }
                }
                if (form.mnuPairs.Checked)
                    for (byte c = UnoColor.RED; c <= UnoColor.BLACK; c++)
                        for (byte u = 1; u <= Players[player].uno[c, bestCard.number]; u++)
                        {
                            Card card = new Card
                            {
                                color = c,
                                number = bestCard.number
                            };
                            cards.Add(card);
                        }
                else if (Players[player].uno[UnoColor.BLACK, bestCard.number] > 0)
                    cards.Add(bestCard);
            }
			return cards.ToArray();
		}

		Card[] Box() {
			List<Card> cards = new List<Card>();
			for (byte color = 0; color <= UnoColor.MAX_VALUE; color++)
				for (byte number = 0; number <= UnoNumber.MAX_VALUE; number++)
                    for (int c = 1; c <= BoxCards.uno[color, number]; c++) {
                        Card card = new Card
                        {
                            color = color,
                            number = number
                        };
                        cards.Add(card);
				    };
			return cards.ToArray();
		}

        private void BtnChallenge_Click(object sender, EventArgs e)
        {
            pnlCtrl.Visible = false;
            Action(0, "检举失败! +2");
            AddDraw(2);
            if (!form.mnuDrawTilCanPlay.Checked) movingCard.drew = false;
            timChallenge.Enabled = true;
        }

		private void BtnDraw_Click(object sender, EventArgs e) {
			Draw(0);
		}

		private void BtnPlay_Click(object sender, EventArgs e) {
			Play(0);
		}

		private void BtnPlay_MouseDown(object sender, MouseEventArgs e) {
			if (mnuRightClick.Checked && e.Button == MouseButtons.Right) 
                switch (btnPlay.BackColor.Name) {
					case "Red": btnPlay.BackColor = Color.Yellow; break;
					case "Yellow": btnPlay.BackColor = Color.Lime; break;
					case "Lime": btnPlay.BackColor = Color.Blue; break;
					default: btnPlay.BackColor = Color.Red; break;
				}
		}

		bool CanPlay(List<Card> card, byte color) {
            if (form.mnuAttack.Checked)
            {
                byte discardColor = UnoColor.MAX_VALUE;
                if (lblCards[1].Text != UnoNumberName.DISCARD_ALL)
                    discardColor = GetColorId(BackColor);
                foreach (Card d in card)
                    if (d.number == UnoNumber.DISCARD_ALL)
                    {
                        foreach (Card c in card)
                        {
                            if (c.number != UnoNumber.DISCARD_ALL && discardColor == UnoColor.MAX_VALUE)
                                discardColor = c.color;
                            if (c.color != discardColor && c.number != UnoNumber.DISCARD_ALL)
                                goto deny;
                        }
                        goto discardAll;
                    }
                if (!form.mnuPairs.Checked)
                    if (card.ToArray().Length > 1)
                        goto deny;
            }
            foreach (Card c in card)
                if (c.number != card[0].number)
                    goto deny;
            discardAll:
            if (!form.mnuPlayOrDrawAll.Checked)
            {
                switch (lblCards[1].Text)
                {
                    case UnoNumberName.DRAW_2:
                        if (int.Parse(lblDraw.Text) >= 2)
                            if (!new Regex("^(" + UnoNumber.DRAW_2 + "|" + UnoNumber.WILD_DRAW_4 + "$)").IsMatch(card[0].number + ""))
                                goto deny;
                        break;
                    case UnoNumberName.WILD_DRAW_4:
                        if (int.Parse(lblDraw.Text) >= 4 && card[0].number != UnoNumber.WILD_DRAW_4)
                            goto deny;
                        break;
                    case UnoNumberName.WILD_HITFIRE:
                        if (int.Parse(lblDraw.Text) > 0 && card[0].number != UnoNumber.WILD_HITFIRE)
                            goto deny;
                        break;
                }
            }
            for (int c = 0; c < card.ToArray().Length; c++)
                if (card[c].color == UnoColor.BLACK)
                    goto accept;
			for (int c = 0; c < card.ToArray().Length; c++) if (card[c].color != UnoColor.MAGENTA) goto color;
            goto accept;
color:
            for (int c = 0; c < card.ToArray().Length; c++) if (card[c].color == color) goto btn;
			goto deny;
btn:        
            for (int c = 0; c < card.ToArray().Length; c++) if (new Regex("^(" + GetColorId(BackColor) + "|" + UnoColor.MAGENTA + ")$").IsMatch(card[c].color + "")) goto accept;
            if (card[0].number == GetNumberId(lblCards[1].Text))
                goto accept;
			goto deny;
accept:		
            return true;
deny:
            Action(0, "你不能这样出牌");
            return false;
		}

		private void ChkPlayer_CheckedChanged(object sender, EventArgs e) {
			int index = (int) ((CheckBox) sender).Tag;
            if (!form.mnuPairs.Checked && !form.mnuAttack.Checked && ((CheckBox)sender).Checked)
                for (int c = 0; c < chkPlayer.ToArray().Length; c++)
                    if (c != index) chkPlayer[c].Checked = false;
			if (mnuRightClick.Checked) if (chkPlayer[index].Checked && !new Regex("^Magenta|Black$").IsMatch(chkPlayer[index].BackColor.Name)) btnPlay.BackColor = chkPlayer[index].BackColor;
			for (int c = 0; c < chkPlayer.ToArray().Length; c++) if (chkPlayer[c].Checked) {
				btnPlay.Visible = true;
				return;
			}
			btnPlay.Visible = false;
		}

        void ChkPlayer_Enter(object sender, EventArgs e)
        {
            if (hPlayer.Visible)
            {
                CheckBox c = (CheckBox)sender;
                if (c.Left + pnlPlayer.Left < 0 || c.Left + c.Width + pnlPlayer.Left > Width)
                    if (c.Left >= pnlPlayer.Width - Width)
                        pnlPlayer.Left = -pnlPlayer.Width + Width;
                    else
                        pnlPlayer.Left = -c.Left;
            }
        }

        void ChkPlayer_MouseDown(object sender, MouseEventArgs e)
        {
            isSelectingCards = true;
        }

        private void ChkPlayer_MouseEnter(object sender, EventArgs e)
        {
            ((CheckBox)sender).Focus();
        }

        void ChkPlayer_MouseLeave(object sender, EventArgs e)
        {
            toolTip.Hide(this);
        }

        void ChkPlayer_MouseMove(object sender, MouseEventArgs e)
        {
            if (isSelectingCards)
            {
                if (e.X >= 0 && e.X < UnoSize.WIDTH || e.Y < chkPlayer[0].Top || e.Y >= chkPlayer[0].Top + chkPlayer[0].Height)
                    return;
                int i = (int)Math.Floor((double)e.X / (hPlayer.Visible ? UnoSize.WIDTH : Width / chkPlayer.ToArray().Length)) + int.Parse(((CheckBox)sender).Tag + "");
                if (i >= 0 && i < chkPlayer.ToArray().Length)
                    switch (e.Button)
                    {
                        case MouseButtons.Left:
                            chkPlayer[i].Checked = true;
                            ((CheckBox)sender).Checked = true;
                            break;
                        case MouseButtons.Right:
                            chkPlayer[i].Checked = false;
                            ((CheckBox)sender).Checked = false;
                            break;
                    }
            }
        }

        private void ChkPlayer_MouseUp(object sender, MouseEventArgs e)
        {
            isSelectingCards = false;
            if (e.Y < 0 && pnlCtrl.Visible)
            {
                ((CheckBox)sender).Checked = true;
                BtnPlay_Click(sender, new EventArgs());
            }
        }

        void ChkPlayer_MouseWheel(object sender, MouseEventArgs e)
        {
            if (hPlayer.Visible)
            {
                switch (Math.Sign(e.Delta))
                {
                    case -1:
                        if (hPlayer.Maximum - hPlayer.Value >= Width)
                            hPlayer.Value += Width;
                        else
                            hPlayer.Value = hPlayer.Maximum;
                        break;
                    case 1:
                        if (hPlayer.Value - hPlayer.Minimum >= Width)
                            hPlayer.Value -= Width;
                        else
                            hPlayer.Value = hPlayer.Minimum;
                        break;
                    default:
                        return;
                }
                HPlayer_Scroll(hPlayer, new ScrollEventArgs(new ScrollEventType(), hPlayer.Value));
            }
        }

        private void Control_BackColorChanged(object sender, EventArgs e)
        {
            Color backColor = ((Control)sender).BackColor;
            ((Control)sender).ForeColor = Color.FromArgb(255 - backColor.R, 255 - backColor.G, 255 - backColor.B);
        }

        void Control_KeyDown(object sender, KeyEventArgs e)
        {
            if (!pnlCtrl.Visible)
            {
                movingCard.quickly = true;
                return;
            }
            if (!form.mnuPlayer0.Checked)
                return;
            string number = "%NULL%";
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    if (btnPlay.Visible)
                        BtnPlay_Click(btnPlay, new EventArgs());
                    return;
                case Keys.Add:
                    if (btnDraw.Visible)
                        BtnDraw_Click(btnDraw, new EventArgs());
                    return;
                case Keys.Subtract:
                    rdoUno.Checked = true;
                    break;
                case Keys.Divide:
                    ChkPlayer_MouseWheel(new object(), new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 1));
                    return;
                case Keys.Multiply:
                    ChkPlayer_MouseWheel(new object(), new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, -1));
                    return;
                case Keys.T:
                case Keys.OemQuestion:
                    MnuChat_Click(mnuChat, new EventArgs());
                    return;
                case Keys.Decimal:
                    if (form.mnuPairs.Checked)
                        return;
                    string n = "%NULL%";
                    foreach (CheckBox c in chkPlayer)
                    {
                        if (c.Text == n)
                        {
                            if (c.Left + pnlPlayer.Left < 0 || c.Left + c.Width + pnlPlayer.Left > Width)
                                pnlPlayer.Left = -c.Left;
                            c.Checked = true;
                            break;
                        }
                        if (c.Checked)
                            n = c.Text;
                    }
                    return;
                case Keys.OemQuotes:
                    if (mnuRadioBox.Checked)
                        return;
                    BtnPlay_MouseDown(btnPlay, new MouseEventArgs(MouseButtons.Right, 1, 0, 0, 0));
                    return;
                case Keys.OemCloseBrackets:
                    if (btnChallenge.Visible)
                        BtnChallenge_Click(btnChallenge, new EventArgs());
                    return;
                case Keys.OemOpenBrackets:
                    return;
                case Keys.NumPad0:
                case Keys.NumPad1:
                case Keys.NumPad2:
                case Keys.NumPad3:
                case Keys.NumPad4:
                case Keys.NumPad5:
                case Keys.NumPad6:
                case Keys.NumPad7:
                case Keys.NumPad8:
                case Keys.NumPad9:
                    number = e.KeyCode - Keys.NumPad0 + "";
                    break;
                case Keys.Delete:
                    number = UnoNumberName.SKIP;
                    break;
                case Keys.End:
                    number = UnoNumberName.REVERSE;
                    break;
                case Keys.PageDown:
                    number = UnoNumberName.DRAW_2;
                    break;
                case Keys.Insert:
                    number = form.txtBlankText.Text;
                    break;
                case Keys.Home:
                    number = UnoNumberName.WILD;
                    break;
                case Keys.PageUp:
                    number = UnoNumberName.WILD_DRAW_4;
                    break;
                default:
                    return;
            }
            foreach (CheckBox c in chkPlayer)
                c.Checked = false;
            if (form.mnuPairs.Checked)
            {
                foreach (CheckBox c in chkPlayer)
                    if (c.Text == number)
                    {
                        c.Checked = true;
                        c.Focus();
                    }
            }
            else
                foreach (CheckBox c in chkPlayer)
                    if (c.Text == number)
                    {
                        if (c.Left + pnlPlayer.Left < 0 || c.Left + c.Width + pnlPlayer.Left > Width)
                            pnlPlayer.Left = -c.Left;
                        c.Checked = true;
                        c.Focus();
                        break;
                    }
        }

        void DownpourDraw(byte player, int draw)
        {
            byte ons = 0;
            foreach (Label p in lblPlayers)
                if (p.Visible)
                    ons++;
            movingCard.downpour = draw * (ons - 1);
            movingCard.player = NextPlayer(player);
            movingCard.progress = 0;
            movingCard.quickly = false;
            timBoxToPlayers.Enabled = true;
        }

        void Draw(byte player)
        {
            if (int.Parse(lblBox.Text) <= 0)
                RefillBox();
            if ((!movingCard.drew || form.mnuDrawTilCanPlay.Checked) && int.Parse(lblBox.Text) > 0)
            {
                Action(player, "摸牌");
                if (player == 0)
                {
                    pnlCtrl.Visible = false;
                    if (rdoUno.Checked)
                    {
                        AddDraw(2);
                        Action(0, "UNO? +2");
                    }
                }
                movingCard.player = player; movingCard.progress = 0; movingCard.quickly = false;
				timBoxToPlayers.Enabled = true;
            }
            else
            {
                Action(player, "过牌");
                movingCard.drew = false;
                PlayersTurn(player, false);
                PlayersTurn(NextPlayer(player), true, form.mnuDrawBeforePlaying.Checked);
			}
		}

		public FrmUno(frmOptions form) 
        {
            InitializeComponent();
            height = Height - mnuGame.Height;
            this.form = form;
            lblBox.Top = mnuGame.Height;
            if (mnuRightClick.Checked) btnPlay.BackColor = Color.Red;
            if (form.mnuJumpin.Checked) btnJumpin.Visible = true;
			for (byte i = 0; i < 4; i++) {
				lblPlayers[i] = new Label();
				lblCounts[i] = new Label();
				Controls.Add(lblPlayers[i]);
				Controls.Add(lblCounts[i]);
				lblPlayers[i].AutoSize = false;
				if (i > 0) lblPlayers[i].BorderStyle = BorderStyle.FixedSingle;
                lblPlayers[i].Click += new EventHandler(LblPlayers_Click);
                lblPlayers[i].Size = new Size(UnoSize.WIDTH, 120);
                lblPlayers[i].Tag = i;
                lblCounts[i].BackColorChanged += new EventHandler(Control_BackColorChanged);
                lblCounts[i].Font = new Font("GulimChe", lblCounts[i].Font.Size);
				lblCounts[i].TextAlign = ContentAlignment.MiddleCenter;
				lblCounts[i].Tag = i;
				lblCounts[i].Text = "0";
				lblCounts[i].TextChanged += new EventHandler(LblCounts_TextChanged);
            }
            for (int o = 0; o < lblPlayers.Length; o++)
            {
                lblPlayers[o].Visible = ((ToolStripMenuItem)form.mnuOns.DropDownItems[o]).Checked;
                lblCounts[o].Visible = ((ToolStripMenuItem)form.mnuOns.DropDownItems[o]).Checked;
            }
            lblCards.Add(new Label());
			Controls.Add(lblCards[0]);
			ResizeForm();
            int boxes = int.Parse(form.txtBoxes.Text);
			for (byte c = UnoColor.RED; c <= UnoColor.BLUE; c++)
            {
				BoxCards.uno[c, 0] = boxes;
                for (byte n = 1; n <= UnoNumber.DRAW_2; n++)
                    BoxCards.uno[c, n] = 2 * boxes;
                if (form.mnuAttack.Checked)
                {
                    BoxCards.uno[c, UnoNumber.DISCARD_ALL] = boxes;
                    // Box.uno[c, UnoNumber.TRADE_HANDS] = boxes;
                }
                if (form.mnuBlank.Checked)
                    BoxCards.uno[c, UnoNumber.BLANK] = 1 * boxes;
			}
            if (form.mnuBlank.Checked && form.mnuMagentaBlank.Checked)
                BoxCards.uno[UnoColor.MAGENTA, UnoNumber.BLANK] = 1 * boxes;
            if (form.mnuDownpourDraw.Checked && form.mnuBlackBlank.Checked)
                BoxCards.uno[UnoColor.BLACK, UnoNumber.BLANK] = 2 * boxes;
            BoxCards.uno[UnoColor.BLACK, UnoNumber.WILD] = 4 * boxes;
            if (form.mnuDownpourDraw.Checked)
            {
                BoxCards.uno[UnoColor.BLACK, UnoNumber.WILD_DOWNPOUR_DRAW_1] = boxes;
                BoxCards.uno[UnoColor.BLACK, UnoNumber.WILD_DOWNPOUR_DRAW_2] = boxes;
                BoxCards.uno[UnoColor.BLACK, UnoNumber.WILD_DOWNPOUR_DRAW_4] = boxes;
            }
            BoxCards.uno[UnoColor.BLACK, UnoNumber.WILD_DRAW_4] = 4 * boxes;
            if (form.mnuAttack.Checked)
            {
                BoxCards.uno[UnoColor.BLACK, UnoNumber.WILD_HITFIRE] = 2 * boxes;
                BoxCards.uno[UnoColor.BLACK, UnoNumber.WILD_DOWNPOUR_DRAW_1] = 2 * boxes;
            }
			lblBox.Text = Box().Length + "";
            for (byte p = 0; p < 4; p++)
                Players[p] = new Cards();
            AddLabel(lblMoving);
            lblMoving[0].BringToFront();
		}

        private void FrmUno_Click(object sender, EventArgs e) {
            movingCard.quickly = true;
        }

        private void FrmUno_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("退出游戏?", "退出", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
            {
                FormClosing -= new FormClosingEventHandler(FrmUno_FormClosing);
                Application.Exit();
            }
            else e.Cancel = true;
        }

		private void FrmUno_Load(object sender, EventArgs e) {
            for (byte p = 0; p < 4; p++)
                if (lblPlayers[p].Visible)
                {
                    movingCard.player = p;
                    goto play;
                }
            FormClosing -= new FormClosingEventHandler(FrmUno_FormClosing);
            Application.Exit();
            return;
play:
            timBoxToPlayers.Enabled = true;
		}

		private void FrmUno_Resize(object sender, EventArgs e) {
			ResizeForm();
		}

		Color GetColor(byte id) {
			switch (id) {
				case 0: return Color.Red;
				case 1: return Color.Yellow;
				case 2: return Color.Lime;
				case 3: return Color.Blue;
				case 4: return Color.Magenta;
                case 5: return Color.Black;
                case 6: return Color.Cyan;
			}
			return Color.White;
		}

		byte GetColorId(Color color) {
			switch (color.Name) {
				case "Red": return 0;
				case "Yellow": return 1;
				case "Lime": return 2;
				case "Blue": return 3;
				case "Magenta": return 4;
                case "Black": return 5;
                case "Cyan": return 6;
			}
			return 7;
		}

        string GetColorName(byte id) {
            switch (id)
            {
                case 0: return "红";
                case 1: return "黃";
                case 2: return "绿";
                case 3: return "蓝";
                case 4: return "紫";
                case 5: return "黑";
                case 6: return "靑";
            }
            return "白";
        }

        string GetNumber(byte id) {
			switch (id) {
				default: return id + "";
				case UnoNumber.SKIP: return UnoNumberName.SKIP;
                case UnoNumber.REVERSE: return UnoNumberName.REVERSE;
                case UnoNumber.DRAW_2: return UnoNumberName.DRAW_2;
                case UnoNumber.DISCARD_ALL: return UnoNumberName.DISCARD_ALL;
                case UnoNumber.TRADE_HANDS: return UnoNumberName.TRADE_HANDS;
                case UnoNumber.BLANK: return form.txtBlankText.Text;
                case UnoNumber.WILD: return UnoNumberName.WILD;
                case UnoNumber.WILD_DOWNPOUR_DRAW_1: return UnoNumberName.WILD_DOWNPOUR_DRAW_1;
                case UnoNumber.WILD_DOWNPOUR_DRAW_2: return UnoNumberName.WILD_DOWNPOUR_DRAW_2;
                case UnoNumber.WILD_DOWNPOUR_DRAW_4: return UnoNumberName.WILD_DOWNPOUR_DRAW_4;
                case UnoNumber.WILD_DRAW_4: return UnoNumberName.WILD_DRAW_4;
                case UnoNumber.WILD_HITFIRE: return UnoNumberName.WILD_HITFIRE;
                case UnoNumber.NULL: return UnoNumberName.NULL;
			}
		}

		byte GetNumberId(String number) {
            if (number == form.txtBlankText.Text) return UnoNumber.BLANK;
			switch (number) {
				default: return byte.Parse(number);
                case UnoNumberName.SKIP: return UnoNumber.SKIP;
                case UnoNumberName.REVERSE: return UnoNumber.REVERSE;
                case UnoNumberName.DRAW_2: return UnoNumber.DRAW_2;
                case UnoNumberName.DISCARD_ALL: return UnoNumber.DISCARD_ALL;
                case UnoNumberName.TRADE_HANDS: return UnoNumber.TRADE_HANDS;
                case UnoNumberName.WILD: return UnoNumber.WILD;
                case UnoNumberName.WILD_DOWNPOUR_DRAW_1: return UnoNumber.WILD_DOWNPOUR_DRAW_1;
                case UnoNumberName.WILD_DOWNPOUR_DRAW_2: return UnoNumber.WILD_DOWNPOUR_DRAW_2;
                case UnoNumberName.WILD_DOWNPOUR_DRAW_4: return UnoNumber.WILD_DOWNPOUR_DRAW_4;
                case UnoNumberName.WILD_DRAW_4: return UnoNumber.WILD_DRAW_4;
                case UnoNumberName.WILD_HITFIRE: return UnoNumber.WILD_HITFIRE;
                case UnoNumberName.NULL: return UnoNumber.NULL;
			}
		}

        string GetNumberName(byte number)
        {
            if (number == UnoNumber.BLANK) return "Blank";
            switch (number)
            {
                default: return number + "";
                case UnoNumber.SKIP: return "Skip";
                case UnoNumber.REVERSE: return "Reverse";
                case UnoNumber.DRAW_2: return "Draw Two";
                case UnoNumber.DISCARD_ALL: return "Discard All";
                case UnoNumber.TRADE_HANDS: return "Trade Hands";
                case UnoNumber.WILD: return "Wild";
                case UnoNumber.WILD_DOWNPOUR_DRAW_1: return "Wild Downpour Draw One";
                case UnoNumber.WILD_DOWNPOUR_DRAW_2: return "Wild Downpour Draw Two";
                case UnoNumber.WILD_DOWNPOUR_DRAW_4: return "Wild Downpour Draw Four";
                case UnoNumber.WILD_DRAW_4: return "Wild Draw Four";
                case UnoNumber.WILD_HITFIRE: return "Wild Hit-fire";
                case UnoNumber.NULL: return "Null";
            }
        }

		bool IsNumeric(string s) {
			return new Regex(@"^[+-]?\d+[.\d]*$").IsMatch(s);
		}

        int GetOnplayersCards(byte color, byte number)
        {
            int cards = 0;
            foreach (Cards p in Players) cards += p.uno[color, number];
            return cards;
        }

        int GetPoints(byte player)
        {
            int point = 0;
            Card[] cards = PlayersCards(player);
            foreach (Card c in cards)
                switch (c.number)
                {
                    default:
                        point += c.number;
                        break;
                    case UnoNumber.SKIP:
                    case UnoNumber.REVERSE:
                    case UnoNumber.DRAW_2:
                    case UnoNumber.DISCARD_ALL:
                    case UnoNumber.TRADE_HANDS:
                    case UnoNumber.BLANK:
                        point += 20;
                        break;
                    case UnoNumber.WILD:
                    case UnoNumber.WILD_DOWNPOUR_DRAW_1:
                    case UnoNumber.WILD_DOWNPOUR_DRAW_2:
                    case UnoNumber.WILD_DOWNPOUR_DRAW_4:
                    case UnoNumber.WILD_DRAW_4:
                        point += 50;
                        break;
                }
            return point;
        }

        int GetQuantityByColor(byte player, byte color) {
            int quantity = 0;
            for (byte q = 0; q <= UnoNumber.MAX_VALUE; q++)
                quantity += Players[player].uno[color, q];
            return quantity;
        }

        int GetQuantityByNumber(byte player, byte number) {
            int quantity = 0;
            for (byte q = 0; q <= UnoColor.MAX_VALUE; q++)
                quantity += Players[player].uno[q, number];
            return quantity;
        }

        string GetType(byte color, byte number)
        {
            if (color == UnoColor.BLACK)
                return "万能";
            else if (number <= 9)
                return "普通";
            else
                return "功能";
        }

        string GetUsage(string number)
        {
            string usage = "任意指定颜色幷且所有家罚抽 %AMOUNT% 张牌.";
            string[] usages = {"禁止下家出牌.", "反转出牌顺序.", "下家罚抽 %AMOUNT% 张牌."};
            if (number == form.txtBlankText.Text)
                return (int.Parse(form.txtBlankSkip.Text) > 0 ? usages[0] : "") + " " + (form.mnuBlankReverse.Checked ? usages[1] : "") + " " + (int.Parse(form.txtBlankDraw.Text) > 0 ? usages[2].Replace("%AMOUNT%", form.txtBlankDraw.Text) : "");
            switch (number)
            {
                case UnoNumberName.SKIP: return usages[0];
                case UnoNumberName.REVERSE: return usages[1];
                case UnoNumberName.DRAW_2: return usages[2].Replace("%AMOUNT%", "2");
                case UnoNumberName.DISCARD_ALL: return "允许玩家打出所有相同颜色的牌.";
                case UnoNumberName.TRADE_HANDS: return "所有玩家互相交換手牌.";
                case UnoNumberName.WILD: return "任意指定颜色.";
                case UnoNumberName.WILD_DOWNPOUR_DRAW_1: return usage.Replace("%AMOUNT%", "1");
                case UnoNumberName.WILD_DOWNPOUR_DRAW_2: return usage.Replace("%AMOUNT%", "2");
                case UnoNumberName.WILD_DOWNPOUR_DRAW_4: return usage.Replace("%AMOUNT%", "4");
                case UnoNumberName.WILD_DRAW_4: return "任意指定颜色幷且下家罚抽 4 张牌.";
                case UnoNumberName.WILD_HITFIRE: return "下家罚抽牌盒中的所有牌.";
                default: return "普通的 " + number + " 号牌.";
            }
        }

        private void HPlayer_Scroll(object sender, ScrollEventArgs e)
        {
            pnlPlayer.Left = -((HScrollBar)sender).Value;
        }

        private void ItmAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("作者\n" +
                "MISTER_CHAN", "关于", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

		private void ItmQuit_Click(object sender, EventArgs e) {
            Application.Exit();
		}

        private void LblBox_Click(object sender, EventArgs e)
        {
            if (form.mnuCanShowCards.Checked)
            {
                string uno = "";
                for (byte color = 0; color <= UnoColor.MAX_VALUE; color++)
                    uno += "\t" + GetColorName(color);
                for (byte number = 0; number <= UnoNumber.MAX_VALUE; number++)
                {
                    uno += "\n" + GetNumber(number);
                    for (byte color = 0; color <= UnoColor.MAX_VALUE; color++)
                        uno += "\t" + BoxCards.uno[color, number];
                }
                MessageBox.Show(uno, "Box");
            }
            else if (btnDraw.Visible)
                BtnDraw_Click(sender, e);
        }

        private void LblBox_TextChanged(object sender, EventArgs e)
        {
            if (int.Parse(((Label)sender).Text) <= 0)
            {
                bool btc = timBoxToCenter.Enabled, btp = timBoxToPlayers.Enabled;
                if (btc) timBoxToCenter.Enabled = false;
                if (btp) timBoxToPlayers.Enabled = false;
                if (MessageBox.Show("牌已用尽, 使用废牌堆的牌?", "牌已用尽", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    RefillBox();
                    if (lblBox.Text == "0")
                    {
                        DialogResult result = MessageBox.Show("废牌堆无牌!", "牌已用尽", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
                        switch (result)
                        {
                            default:
                                FormClosing -= new FormClosingEventHandler(FrmUno_FormClosing);
                                Application.Exit();
                                break;
                            case DialogResult.Retry:
                                FormClosing -= new FormClosingEventHandler(FrmUno_FormClosing);
                                Application.Restart();
                                break;
                            case DialogResult.Ignore:
                                lblDraw.Text = "0";
                                break;
                        }
                    }
                    timBoxToCenter.Enabled = btc;
                    timBoxToPlayers.Enabled = btp;
                }
                else
                    Close();
            }
        }

        private void Label_MouseEnter(object sender, EventArgs e)
        {
        }

        void Label_MouseLeave(object sender, EventArgs e)
        {
            toolTip.Hide(this);
        }

		private void LblCounts_TextChanged(object sender, EventArgs e) {
            byte index = (byte)((Label)sender).Tag;
            if (movingCard.playing)
            {
                byte player = 0, winners = 0;
                if (int.Parse(lblCounts[index].Text) <= 0)
                {
                    lblPlayers[index].Visible = false;
                    lblCounts[index].Visible = false;
                }
                if (form.mnuOneWinner.Checked)
                {
                    if (!lblPlayers[index].Visible)
                    {
                        player = index;
                        goto gameOver;
                    }
                }
                else
                {
                    for (byte p = 0; p < lblPlayers.Length; p++) if (!lblPlayers[p].Visible) winners++;
                    if (winners == 3)
                    {
                        for (byte p = 0; p < lblPlayers.Length; p++)
                        {
                            if (lblPlayers[p].Visible)
                            {
                                player = p;
                                goto gameOver;
                            }
                        }
                    }
                }
                return;
gameOver:

                timBoxToCenter.Enabled = false;
                timBoxToPlayers.Enabled = false;
                timTurn.Enabled = false;
                timPlayersToCenter.Enabled = false;
                timWatch.Enabled = false;
                FormClosing -= new FormClosingEventHandler(FrmUno_FormClosing);
                if (form.mnuOneWinner.Checked)
                {
                    List<Label> label = new List<Label>();
                    for (byte p = 1; p <= 3; p++)
                    {
                        int playerPos = label.ToArray().Length;
                        if (mnuByColor.Checked)
                            for (byte color = 0; color <= UnoColor.MAX_VALUE; color++)
                                for (byte number = 0; number <= UnoNumber.MAX_VALUE; number++)
                                    for (int c = 1; c <= Players[p].uno[color, number]; c++)
                                    {
                                        int length = label.ToArray().Length;
                                        AddLabel(label);
                                        label[length].BackColor = GetColor(color);
                                        if (new Regex("^(1|3)$").IsMatch(p + "")) label[length].Left = lblPlayers[p].Left;
                                        label[length].Text = GetNumber(number);
                                        if (p == 2) label[length].Top = lblPlayers[2].Top;
                                    }
                        else
                            for (byte number = 0; number <= UnoNumber.MAX_VALUE; number++)
                                for (byte color = 0; color <= UnoColor.MAX_VALUE; color++)
                                    for (int c = 1; c <= Players[p].uno[color, number]; c++)
                                    {
                                        int length = label.ToArray().Length;
                                        AddLabel(label);
                                        label[length].BackColor = GetColor(color);
                                        if (new Regex("^(1|3)$").IsMatch(p + "")) label[length].Left = lblPlayers[p].Left;
                                        label[length].Text = GetNumber(number);
                                        if (p == 2) label[length].Top = lblPlayers[2].Top;
                                    }
                        for (int c = playerPos; c < label.ToArray().Length; c++)
                        {
                            switch (p)
                            {
                                case 1:
                                case 3:
                                    if (height / 2 - UnoSize.HEIGHT * (label.ToArray().Length - playerPos) / 2 >= 0)
                                        label[c].Top = height / 2 - UnoSize.HEIGHT * (label.ToArray().Length - playerPos) / 2 + UnoSize.HEIGHT * (c - playerPos);
                                    else
                                        label[c].Top = height / (label.ToArray().Length - playerPos) * (c - playerPos);
                                    break;
                                case 2:
                                    if (Width / 2 - UnoSize.WIDTH * (label.ToArray().Length - playerPos) / 2 >= 0)
                                        label[c].Left = Width / 2 - UnoSize.WIDTH * (label.ToArray().Length - playerPos) / 2 + UnoSize.WIDTH * (c - playerPos);
                                    else
                                        label[c].Left = Width / (label.ToArray().Length - playerPos) * (c - playerPos);
                                    break;
                            }
                        }
                    }
                    string msg = (player == 0 ? "你" : "Player" + player) + " 赢了!\n" +
                        "\n" +
                        (form.mnuWatch.Checked ? "游戏时长\t" + lblWatch.Text + "\n" +
                        "\n" : "") +
                        "玩家\t得分";
                    for (byte p = 0; p <= 3; p++)
                        msg += "\n" + (p == 0 ? "你" : "Player" + p) + "\t" + GetPoints(p);
                    if (MessageBox.Show(msg, "结束", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Retry) goto retry;
                }
                else
                {
                    if (player != 0)
                    {
                        List<Label> label = new List<Label>();
                        if (mnuByColor.Checked)
                            for (byte color = 0; color <= UnoColor.MAX_VALUE; color++)
                                for (byte number = 0; number <= UnoNumber.MAX_VALUE; number++)
                                    for (int c = 1; c <= Players[player].uno[color, number]; c++)
                                    {
                                        int length = label.ToArray().Length;
                                        AddLabel(label);
                                        label[length].BackColor = GetColor(color);
                                        if (new Regex("^(1|3)$").IsMatch(player + "")) label[length].Left = lblPlayers[player].Left;
                                        label[length].Text = GetNumber(number);
                                        if (player == 2) label[length].Top = lblPlayers[2].Top;
                                    }
                        else
                            for (byte number = 0; number <= UnoNumber.MAX_VALUE; number++)
                                for (byte color = 0; color <= UnoColor.MAX_VALUE; color++)
                                    for (int c = 1; c <= Players[player].uno[color, number]; c++)
                                    {
                                        int length = label.ToArray().Length;
                                        AddLabel(label);
                                        label[length].BackColor = GetColor(color);
                                        if (new Regex("^(1|3)$").IsMatch(player + "")) label[length].Left = lblPlayers[player].Left;
                                        label[length].Text = GetNumber(number);
                                        if (player == 2) label[length].Top = lblPlayers[2].Top;
                                    }
                        for (int c = 0; c < label.ToArray().Length; c++)
                        {
                            switch (player)
                            {
                                case 1:
                                case 3:
                                    if (height / 2 - UnoSize.HEIGHT * label.ToArray().Length / 2 >= 0)
                                        label[c].Top = height / 2 - UnoSize.HEIGHT * label.ToArray().Length / 2 + UnoSize.HEIGHT * c;
                                    else
                                        label[c].Top = height / label.ToArray().Length * c;
                                    break;
                                case 2:
                                    if (Width / 2 - UnoSize.WIDTH * label.ToArray().Length / 2 >= 0)
                                        label[c].Left = Width / 2 - UnoSize.WIDTH * label.ToArray().Length / 2 + UnoSize.WIDTH * c;
                                    else
                                        label[c].Left = Width / label.ToArray().Length * c;
                                    break;
                            }
                        }
                    }
                    if (MessageBox.Show((player == 0 ? "你" : "Player" + player) + " 输了!\n" +
                        (form.mnuWatch.Checked ? "\n" +
                        "游戏时长\t" + lblWatch.Text : ""), "结束", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Retry) goto retry;
                }
                Application.Exit();
                return;
retry:
                Application.Restart();
            }
		}

        private void LblPlayers_Click(object sender, EventArgs e)
        {
            int index = int.Parse(((Control)sender).Tag + "");
            if (form.mnuCanShowCards.Checked)
            {
                string uno = "";
                for (byte color = 0; color <= UnoColor.MAX_VALUE; color++)
                    uno += "\t" + GetColorName(color);
                for (byte number = 0; number <= UnoNumber.MAX_VALUE; number++)
                {
                    uno += "\n" + GetNumber(number);
                    for (byte color = 0; color <= UnoColor.MAX_VALUE; color++)
                        uno += "\t" + Players[index].uno[color, number];
                }
                MessageBox.Show(uno, "Player" + index);
            }
        }

        private void LblWatch_Resize(object sender, EventArgs e)
        {
            lblWatch.Left = Width - lblWatch.Width;
        }

        private void LoadGame(string[] keys)
        {
            string[] pls = keys[0].Split(char.Parse("P"));
            string[] colors;
            for (byte p = 0; p <= 3; p++)
            {
                colors = pls[p].Split(char.Parse("C"));
                for (byte c = 0; c <= UnoColor.MAX_VALUE; c++)
                {
                    string[] numbers = colors[c].Split(char.Parse("N"));
                    for (byte n = 0; n <= UnoNumber.MAX_VALUE; n++)
                        Players[p].uno[c, n] = int.Parse(numbers[n]);
                }
                lblCounts[p].Text = PlayersCards(p).Length + "";
            }
            Rank();
            colors = keys[1].Split(char.Parse("C"));
            for (byte c = 0; c <= UnoColor.MAX_VALUE; c++)
            {
                string[] numbers = colors[c].Split(char.Parse("N"));
                for (byte n = 0; n <= UnoNumber.MAX_VALUE; n++)
                    BoxCards.uno[c, n] = int.Parse(numbers[n]);
            }
            lblBox.Text = Box().Length - 1 + "";
            BackColor = GetColor(byte.Parse(keys[2]));
            foreach (Label card in lblCards)
                card.Text = GetNumber(byte.Parse(keys[3]));
            lblCards[0].Text = UnoNumberName.NULL;
            skip = int.Parse(keys[4]);
            string[] sks = keys[5].Split(char.Parse("P"));
            for (byte sk = 0; sk <= 3; sk++)
                skips[sk] = int.Parse(sks[sk]);
            reverse = bool.Parse(keys[6]);
            lblDraw.Text = keys[7];
            gametime = int.Parse(keys[8]);
        }

        private void MnuChat_Click(object sender, EventArgs e)
        {
            string cmd = Interaction.InputBox(">", "Chat");
            int count = 0;
            cmd = cmd.Trim();
            for (; cmd.IndexOf("  ") > -1; )
                cmd = cmd.Replace("  ", " ");
            if (cmd == "") return;
            if (cmd.Substring(0, 1) == "/")
            {
                if (form.mnuCheat.Checked)
                {
                    string[] data = cmd.Split(char.Parse(" "));
                    switch (data[0].ToLower())
                    {
                        case "/boxes":
                            if (data.Length > 1)
                                if (IsNumeric(data[1]))
                                {
                                    form.txtBoxes.Text = data[1];
                                    Action(0, "已准备 " + data[1] + " 副牌");
                                }
                            break;
                        case "/clear":
                            byte fromColor = 0, fromNumber = 0, player = 0, toColor = UnoColor.MAX_VALUE, toNumber = UnoNumber.MAX_VALUE;
                            switch (data.Length)
                            {
                                case 1:
                                    break;
                                case 2:
                                    player = byte.Parse(data[1]);
                                    break;
                                case 3:
                                    player = byte.Parse(data[1]);
                                    fromColor = byte.Parse(data[2]); toColor = byte.Parse(data[2]);
                                    break;
                                default:
                                    player = byte.Parse(data[1]);
                                    fromColor = byte.Parse(data[2]); toColor = byte.Parse(data[2]);
                                    fromNumber = byte.Parse(data[3]); toNumber = byte.Parse(data[3]);
                                    break;
                            }
                            for (byte color = fromColor; color <= toColor; color++)
                                for (byte number = fromNumber; number <= toNumber; number++)
                                    Players[player].uno[color, number] = 0;
                            if (player == 0) Rank();
                            Action(0, "已淸除 Player" + player + " 的手牌");
                            lblCounts[player].Text = PlayersCards(player).Length + "";
                            break;
                        case "/currard":
                            if (data.Length > 2)
                            {
                                BackColor = GetColor(byte.Parse(data[1]));
                                foreach (Label card in lblCards)
                                    card.Text = GetNumber(byte.Parse(data[2]));
                            }
                            else if (data.Length == 2)
                                BackColor = GetColor(byte.Parse(data[1]));
                            Action(0, "你的回合被指定为 [" + GetColorName(byte.Parse(data[1])) + " " + lblCards[1].Text + "]");
                            break;
                        case "/draw":
                            if (data.Length == 2)
                                lblDraw.Text = data[1];
                            else
                            {
                                count = 1;
                                if (data.Length > 2)
                                    count = int.Parse(data[2]);
                                for (int c = 1; c <= count; c++)
                                {
                                    Card[] box = this.Box();
                                    Card rndCard = box[(int)(box.Length * Rnd())];
                                    BoxCards.uno[rndCard.color, rndCard.number]--;
                                    lblBox.Text = box.Length - 1 + "";
                                    Players[int.Parse(data[1])].uno[rndCard.color, rndCard.number]++;
                                    lblCounts[int.Parse(data[1])].Text = PlayersCards(byte.Parse(data[1])).Length + "";
                                }
                                if (data[1] == "0")
                                    Rank();
                            }
                            break;
                        case "/give":
                            if (data.Length < 4) return;
                            count = 1;
                            if (data.Length >= 5) count = Int32.Parse(data[4]);
                            Players[Byte.Parse(data[1])].uno[Byte.Parse(data[2]), Byte.Parse(data[3])] += count;
                            if (data[1] == "0")
                                Rank();
                            lblCounts[int.Parse(data[1])].Text = PlayersCards(byte.Parse(data[1])).Length + "";
                            Action(0, "已将 [" + GetColorName(byte.Parse(data[2])) + " " + GetNumber(byte.Parse(data[3])) + "] * " + count + " 给予 Player" + data[1]);
                            break;
                        case "/help":
                        case "/?":
                            string page = "1";
                            if (data.Length > 1) page = data[1];
                            string help = "/boxes <int boxes>\n" +
                                "/clear [byte player] [byte color] [byte number]\n" +
                                "/currard <byte color> [byte number]\n" +
                                "/draw [byte player] <int draw>\n" +
                                "/give <byte player> <byte color> <byte number> [int count]\n" +
                                "/help [int page]\n" +
                                "/load [string data]\n" +
                                "/me <string action>\n" +
                                "/pause [options | pause | quit | restart]\n" +
                                "/reverse [bool reverse]\n" +
                                "/save\n" +
                                "/say <string message>\n" +
                                "/skip [[byte player] <int skip>]\n" +
                                "/time true | false | pause | <int gametime> | <int h>:<int m>:<int s>\n" +
                                "/tips\n" +
                                "/uno [bool isUno]\n" +
                                "/? [int page]";
                            MessageBox.Show("=== 显示指令列表第 " + page + " 页, 共 1 页 ===\n" + (page == "1" ? help : ""), "帮助", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                        case "/load":
                            string[] keys;
                            if (data.Length > 1)
                                keys = data[1].Split(char.Parse("K"));
                            else
                                keys = Clipboard.GetText().Split(char.Parse("K"));
                            LoadGame(keys);
                            break;
                        case "/me":
                            if (data.Length > 1) Action(0, "* 你 " + cmd.Substring(cmd.IndexOf(" ") + 1));
                            break;
                        case "/pause":
                            if (data.Length == 1)
                            {
                                form.Show();
                            }
                            else
                            {
                                switch (data[1])
                                {
                                    default:
                                        form.Show();
                                        break;
                                    case "quit":
                                        FormClosing -= new FormClosingEventHandler(FrmUno_FormClosing);
                                        Application.Exit();
                                        break;
                                    case "restart":
                                        FormClosing -= new FormClosingEventHandler(FrmUno_FormClosing);
                                        Application.Restart();
                                        break;
                                }
                            }
                            break;
                        case "/reverse":
                            if (data.Length == 1)
                                reverse = !reverse;
                            else
                                reverse = bool.Parse(data[1]);
                            Action(0, "已反转出牌顺序");
                            break;
                        case "/save":
                            Clipboard.SetText(SaveGame());
                            Action(0, "游戏记彔已储存到剪贴簿");
                            break;
                        case "/say":
                            if (data.Length > 1) Action(0, "[你] " + cmd.Substring(cmd.IndexOf(" ") + 1));
                            break;
                        case "/skip":
                            switch (data.Length)
                            {
                                case 1:
                                    PlayersTurn(0, false);
                                    PlayersTurn(movingCard.player = NextPlayer(movingCard.player), true, form.mnuDrawBeforePlaying.Checked);
                                    Action(0, "跳过");
                                    break;
                                case 2:
                                    skip = int.Parse(data[1]);
                                    Action(0, "跳过 " + skip + " 个玩家");
                                    break;
                                default:
                                    skips[byte.Parse(data[1])] = int.Parse(data[2]);
                                    Action(0, "跳过 Player" + data[1] + " " + data[2] + " 次");
                                    break;
                            }
                            break;
                        case "/time":
                            switch (data[1].ToLower())
                            {
                                case "true":
                                    timWatch.Enabled = true;
                                    break;
                                case "false":
                                case "pause":
                                    timWatch.Enabled = false;
                                    break;
                                default:
                                    if (data[1].IndexOf(":") > 0)
                                    {
                                        string[] t = data[1].Split(char.Parse(":"));
                                        gametime = int.Parse(t[0]) * 3600 + int.Parse(t[1]) * 60 + int.Parse(t[2]);
                                    }
                                    else
                                        gametime = int.Parse(data[1]);
                                    break;
                            }
                            break;
                        case "/tips":
                            Card[] cards = Ai(0);
                            foreach (CheckBox chk in chkPlayer)
                            {
                                chk.Checked = false;
                                foreach (Card c in cards)
                                    if (GetColorId(chk.BackColor) == c.color && GetNumberId(chk.Text) == c.number) chk.Checked = true;
                            }
                            Action(0, "提示");
                            break;
                        case "/uno":
                            if (data.Length > 1)
                                rdoUno.Checked = bool.Parse(data[1]);
                            else
                                rdoUno.Checked = !rdoUno.Checked;
                            break;
                        default:
                            Action(0, "未知的指令, 请尝试 '/help' 来显示指令列表.");
                            break;
                    }
                }
                else Action(0, "你沒有权限使用此命令");
            }
            else Action(0, "<你> " + cmd);
        }

        private void MnuColor_Click(object sender, EventArgs e)
        {
            mnuRadioBox.Checked = false;
            mnuRightClick.Checked = false;
            ((ToolStripMenuItem)sender).Checked = true;
        }

        private void MnuContent_Click(object sender, EventArgs e)
        {
            MessageBox.Show("[0 ~ 9]\tNumpad0~9\n" +
                "[" + UnoNumberName.SKIP + "]\tDelete\n" +
                "[" + UnoNumberName.REVERSE + "]\tEnd\n" +
                "[" + UnoNumberName.DRAW_2 + "]\tPageDown\n" +
                "[" + form.txtBlankText.Text + "]\tInsert\n" +
                "[" + UnoNumberName.WILD + "]\tHome\n" +
                "[" + UnoNumberName.WILD_DRAW_4 + "]\tPageUp\n" +
                "下张牌\tNumpad.\n" +
                "上一页\tNumpad/\n" +
                "下一页\tNumpad*\n" +
                "光标移动\t←↑→↓\n" +
                "选定卡牌\tSpace\n" +
                "选择颜色\tNumpad0~3\n" +
                "切換颜色\t'\n" +
                "出牌\tEnter\n" +
                "摸牌\tNumpad+\n" +
                "检举\t]\n" +
                "点断\t[\n" +
                "喊 UNO!\tNumpad-\n" +
                "聊天\tT\n" +
                "指令\t/\n", "按键说明", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MnuNew_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("放弃本局?", "新游戏", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Cancel)
                return;
            FormClosing -= new FormClosingEventHandler(FrmUno_FormClosing);
            Application.Restart();
        }

        private void MnuRank_Click(object sender, EventArgs e)
        {
            mnuByColor.Checked = false;
            mnuByNumber.Checked = false;
            ((ToolStripMenuItem)sender).Checked = true;
            Rank();
        }

        private void MnuSaveGame_Click(object sender, EventArgs e)
        {
            Interaction.SaveSetting("UNO", "GAME", "SAVE", SaveGame());
            Action(0, "储存成功");
        }

        private void MnuScrollBar_Click(object sender, EventArgs e)
        {
            ((ToolStripMenuItem)sender).Checked = !((ToolStripMenuItem)sender).Checked;
            Rank();
        }

        private void MnuToolTip_Click(object sender, EventArgs e)
        {
            ((ToolStripMenuItem)sender).Checked = !((ToolStripMenuItem)sender).Checked;
            toolTip.Active = ((ToolStripMenuItem)sender).Checked;
        }

		byte NextPlayer(byte currentPlayer, bool reverse = false) {
            bool r = reverse;
            if (this.reverse) r = !r;
            byte next = (byte)(!r ? currentPlayer == 3 ? 0 : currentPlayer + 1 : currentPlayer == 0 ? 3 : currentPlayer - 1);
            return lblPlayers[next].Visible ? next : NextPlayer(next, reverse);
		}

        void Play(byte player)
        {
			List<Card> cards = new List<Card>();
			if (player == 0) {
                if (form.mnuAttack.Checked)
                {
                    List<Card> discardAll = new List<Card>();
                    foreach (CheckBox c in chkPlayer)
                        if (c.Checked)
                        {
                            Card card = new Card
                            {
                                color = GetColorId(c.BackColor),
                                number = GetNumberId(c.Text)
                            };
                            if (c.Text == UnoNumberName.DISCARD_ALL)
                                discardAll.Add(card);
                            else
                                cards.Add(card);
                        }
                    discardAll.AddRange(cards);
                    cards = discardAll;
                }
                else
                    foreach (CheckBox c in chkPlayer)
                        if (c.Checked)
                        {
                            Card card = new Card
                            {
                                color = GetColorId(c.BackColor),
                                number = GetNumberId(c.Text)
                            };
                            cards.Add(card);
				        }
                if (mnuRadioBox.Checked)
                {
                    if (!CanPlay(cards, cards.First().color)) return;
                }
                else if (!CanPlay(cards, GetColorId(btnPlay.BackColor))) return;
				for (int c = 0; c < cards.ToArray().Length; c++) {
					Players[0].uno[cards[c].color, cards[c].number]--;
					Controls.Remove(chkPlayer[0]);
					chkPlayer.RemoveAt(0);
				}
				for (int c = 0; c < chkPlayer.ToArray().Length; c++) if (chkPlayer[c].Checked) {
					btnPlay.Visible = true;
					goto play;
				}
				btnPlay.Visible = false;
play:   		Rank();
                btnPlay.Visible = false;
			} else {
                Card[] ais = Ai(player);
				foreach (Card card in ais) cards.Add(card);
				for (int c = 0; c < cards.ToArray().Length; c++) Players[player].uno[cards[c].color, cards[c].number]--;
			}
            if (player == 0)
                Action(0, rdoUno.Checked ? "UNO!" : "出牌");
            else
                Action(player, player != 0 && PlayersCards(player).Length == 1 ? "UNO!" : "出牌");
			PlayersTurn(player, false);
            if (cards.ToArray().Length > 0)
            {
                movingCard.player = player; movingCard.progress = 0;
                if (player == 0)
                {
                    if (mnuRadioBox.Checked)
                    {
                        frmColor color = new frmColor();
                        int colors = 0;
                        if (cards.Last().color == UnoColor.BLACK)
                        {
                            foreach (ToolStripMenuItem menu in color.mnuColor.Items)
                            {
                                menu.Enabled = true;
                                menu.BackColor = GetColor(byte.Parse(menu.Tag + ""));
                            }
                            colors = 4;
                        }
                        else
                        {
                            foreach (Card card in cards)
                                if (!new Regex("^(" + UnoColor.MAGENTA + "|" + UnoColor.BLACK + ")$").IsMatch(card.color + ""))
                                {
                                    color.mnuColor.Items[card.color].Enabled = true;
                                    color.mnuColor.Items[card.color].BackColor = GetColor(card.color);
                                }
                            foreach (ToolStripMenuItem menu in color.mnuColor.Items) if (menu.Enabled) colors++;
                        }
                        if (colors > 1)
                        {
                            color.ShowDialog();
                            movingCard.color = byte.Parse(color.Tag + "");
                        }
                        else if (cards[0].color != UnoColor.MAGENTA) movingCard.color = cards.First().color;
                    }
                    else movingCard.color = GetColorId(btnPlay.BackColor);
                }
                movingCard.number = cards[0].number;
                RemoveLabel(lblMoving);
                AddLabel(lblMoving, cards.ToArray().Length);
                for (int c = 1; c < lblMoving.ToArray().Length; c++)
                {
                    lblMoving[c].BackColor = GetColor(cards[c - 1].color == UnoColor.BLACK ? movingCard.color : cards[c - 1].color); lblMoving[c].Text = GetNumber(cards[c - 1].number);
                    lblMoving[c].BringToFront();
                }
                timPlayersToCenter.Enabled = true;
            }
            else if (player > 0) {
                Draw(player);
            }
		}

		Card[] PlayersCards(byte player) {
			List<Card> cards = new List<Card>();
			for (byte color = 0; color <= UnoColor.MAX_VALUE; color++)
				for (byte number = 0; number <= UnoNumber.MAX_VALUE; number++)
                    for (int c = 1; c <= Players[player].uno[color, number]; c++) {
                        Card card = new Card
                        {
                            color = color,
                            number = number
                        };
                        cards.Add(card);
					};
			return cards.ToArray();
		}

		void PlayersTurn(byte player, bool turn = true, bool one = false) {
            if (turn)
            {
                if (timTurn.Tag.ToString().Split(char.Parse(","))[0] == "4")
                {
                    timTurn.Tag = player + "," + one;
                    timTurn.Enabled = true;
                    return;
                }
                timTurn.Tag = "4,";
                if (skip > 0 || skips[player] > 0)
                {
                    if (form.mnuSkipPlayers.Checked)
                    {
                        Action(player, "禁手 (" + skip + ")");
                        skip--;
                    }
                    else
                    {
                        Action(player, "禁手 (" + skips[player] + ")");
                        skips[player]--;
                    }
                    PlayersTurn(player, false);
                    PlayersTurn(NextPlayer(player), true, form.mnuDrawBeforePlaying.Checked);
                    return;
                }
            }
            if (one && int.Parse(lblCounts[player].Text) > 7)
            {
                Action(player, "摸牌");
                movingCard.one = true; movingCard.player = player; movingCard.progress = 0; movingCard.quickly = false;
                if (int.Parse(lblBox.Text) <= 0)
                    RefillBox();
                if (int.Parse(lblBox.Text) > 0)
                    timBoxToPlayers.Enabled = true;
            } 
            else if (player == 0 && (skip <= 0 || skips[0] <= 0))
            {
                pnlCtrl.Visible = turn;
                mnuSaveGame.Enabled = turn;
                if (turn)
                {
                    movingCard.one = false;
                    Action(0, "你的回合");
                    btnChallenge.Visible = form.mnuChallenges.Checked && lblCards[1].Text == UnoNumberName.WILD_DRAW_4 && int.Parse(lblDraw.Text) >= 4;
                    rdoUno.Checked = false;
                }
            }
            else if (turn)
                Play(player);
		}

		void Rank() {
			RemoveChkPlayer();
			int i = 0;
            if (mnuByColor.Checked)
			    for (byte color = 0; color <= UnoColor.MAX_VALUE; color++)
                    for (byte number = 0; number <= UnoNumber.MAX_VALUE; number++)
                        for (int c = 1; c <= Players[0].uno[color, number]; c++) {
				            AddChkPlayer();
				            chkPlayer[i].BackColor = GetColor(color);
                            chkPlayer[i].Text = GetNumber(number);
                            SetUsage(chkPlayer[i]);
				            i++;
			            }
            else
                for (byte number = 0; number <= UnoNumber.MAX_VALUE; number++)
                    for (byte color = 0; color <= UnoColor.MAX_VALUE; color++)
                        for (int c = 1; c <= Players[0].uno[color, number]; c++)
                        {
                            AddChkPlayer();
                            chkPlayer[i].BackColor = GetColor(color);
                            chkPlayer[i].Text = GetNumber(number);
                            SetUsage(chkPlayer[i]);
                            i++;
                        }
            pnlPlayer.Width = chkPlayer.ToArray().Length * UnoSize.WIDTH;
            hPlayer.Visible = false;
            int width = UnoSize.WIDTH * chkPlayer.ToArray().Length;
            if (width <= Width)
            {
                pnlPlayer.Left = Width / 2 - width / 2;
                for (i = 0; i < chkPlayer.ToArray().Length; i++) chkPlayer[i].Left = UnoSize.WIDTH * i;
            }
            else if (width > Width * 2 && mnuScrollBar.Checked)
            {
                for (i = 0; i < chkPlayer.ToArray().Length; i++) chkPlayer[i].Left = UnoSize.WIDTH * i;
                hPlayer.Maximum = width - Width;
                hPlayer.Visible = true;
            }
            else
            {
                pnlPlayer.Left = 0;
                for (i = 0; i < chkPlayer.ToArray().Length; i++) chkPlayer[i].Left = Width / chkPlayer.ToArray().Length * i;
            }
		}

        void RefillBox()
        {
            int boxes = int.Parse(form.txtBoxes.Text);
            for (byte c = UnoColor.RED; c <= UnoColor.BLUE; c++)
            {
                BoxCards.uno[c, 0] = boxes - GetOnplayersCards(c, 0);
                for (byte n = 1; n <= UnoNumber.DRAW_2; n++)
                    BoxCards.uno[c, n] = 2 * boxes - GetOnplayersCards(c, n);
                if (form.mnuAttack.Checked)
                {
                    BoxCards.uno[c, UnoNumber.DISCARD_ALL] = boxes - GetOnplayersCards(c, UnoNumber.DISCARD_ALL);
                    // Box.uno[c, UnoNumber.TRADE_HANDS] = boxes - getOnplayersCards(c, UnoNumber.TRADE_HANDS);
                }
                if (form.mnuBlank.Checked)
                    BoxCards.uno[c, UnoNumber.BLANK] = boxes - GetOnplayersCards(c, UnoNumber.BLANK);
            }
            if (form.mnuBlank.Checked && form.mnuMagentaBlank.Checked)
                BoxCards.uno[UnoColor.MAGENTA, UnoNumber.BLANK] = 1 * boxes - GetOnplayersCards(UnoColor.MAGENTA, UnoNumber.BLANK);
            if (form.mnuDownpourDraw.Checked && form.mnuBlackBlank.Checked)
                BoxCards.uno[UnoColor.BLACK, UnoNumber.BLANK] = 2 * boxes - GetOnplayersCards(UnoColor.BLACK, UnoNumber.BLANK);
            BoxCards.uno[UnoColor.BLACK, UnoNumber.WILD] = 4 * boxes - GetOnplayersCards(UnoColor.BLACK, UnoNumber.WILD);
            if (form.mnuDownpourDraw.Checked)
            {
                BoxCards.uno[UnoColor.BLACK, UnoNumber.WILD_DOWNPOUR_DRAW_1] = boxes - GetOnplayersCards(UnoColor.BLACK, UnoNumber.WILD_DOWNPOUR_DRAW_1);
                BoxCards.uno[UnoColor.BLACK, UnoNumber.WILD_DOWNPOUR_DRAW_2] = boxes - GetOnplayersCards(UnoColor.BLACK, UnoNumber.WILD_DOWNPOUR_DRAW_2);
                BoxCards.uno[UnoColor.BLACK, UnoNumber.WILD_DOWNPOUR_DRAW_4] = boxes - GetOnplayersCards(UnoColor.BLACK, UnoNumber.WILD_DOWNPOUR_DRAW_4);
            }
            BoxCards.uno[UnoColor.BLACK, UnoNumber.WILD_DRAW_4] = 4 * boxes - GetOnplayersCards(UnoColor.BLACK, UnoNumber.WILD_DRAW_4);
            if (form.mnuAttack.Checked)
            {
                BoxCards.uno[UnoColor.BLACK, UnoNumber.WILD_HITFIRE] = 2 * boxes - GetOnplayersCards(UnoColor.BLACK, UnoNumber.WILD_HITFIRE);
                BoxCards.uno[UnoColor.BLACK, UnoNumber.WILD_DOWNPOUR_DRAW_1] = 2 * boxes - GetOnplayersCards(UnoColor.BLACK, UnoNumber.WILD_DOWNPOUR_DRAW_1);
            }
            lblBox.Text = Box().Length + "";
        }

		void RemoveChkPlayer() {
			while (chkPlayer.ToArray().Length > 0) {
				Controls.Remove(chkPlayer[0]);
				chkPlayer.RemoveAt(0);
			}
		}

		void RemoveLabel(List<Label> label)
        {
			while (label.ToArray().Length > 1)
            {
				Controls.Remove(label[1]);
				label.RemoveAt(1);
			}
		}

		void ResizeForm() {
            height = Height - mnuGame.Height;
            lblPlayers[0].Location = new Point(Width / 2 - UnoSize.WIDTH / 2, height - UnoSize.HEIGHT);
            lblPlayers[1].Location = new Point(0, height / 2 - UnoSize.HEIGHT / 2 + mnuGame.Height / 2);
            lblPlayers[2].Location = new Point(Width / 2 - UnoSize.WIDTH / 2, mnuGame.Height);
            lblPlayers[3].Location = new Point(Width - UnoSize.WIDTH, height / 2 - UnoSize.HEIGHT / 2 + mnuGame.Height / 2);
			for (byte i = 0; i < 4; i++)
                lblCounts[i].Location = new Point(lblPlayers[i].Left, lblPlayers[i].Top - lblCounts[i].Height);
            lblCounts[2].Top = lblPlayers[2].Top + UnoSize.HEIGHT;
            lblWatch.Left = Width - lblWatch.Width;
			pnlCtrl.Location = new Point(0, lblPlayers[0].Top - pnlCtrl.Height);
            btnPlay.Width = Width / 5;
            btnDraw.Left = Width / 5;
            btnDraw.Width = Width / 5;
            btnChallenge.Left = Width / 5 * 2;
            btnChallenge.Width = Width / 5;
            rdoUno.Left = Width / 5 * 4;
            rdoUno.Width = Width / 5;
            pnlCtrl.Width = Width;
			lblCounts[0].Top = pnlCtrl.Top - lblCounts[0].Height;
            pnlPlayer.Top = pnlCtrl.Top + pnlCtrl.Height;
            hPlayer.Top = pnlPlayer.Top;
            hPlayer.Width = Width;
            if (movingCard.playing)
                Rank();
            lblCards[0].Location = new Point(Width / 2 - UnoSize.WIDTH / 2, height / 2 - UnoSize.HEIGHT / 2);
		}

		double Rnd() {
            if (form.mnuSeed.Checked)
                return 0;
            return new Random(Guid.NewGuid().GetHashCode()).NextDouble();
		}

        string SaveGame()
        {
            string s = "";
            for (byte p = 0; p <= 3; p++)
            {
                for (byte c = 0; c <= UnoColor.MAX_VALUE; c++)
                {
                    for (byte n = 0; n <= UnoNumber.MAX_VALUE; n++)
                    {
                        s += Players[p].uno[c, n] + "N";
                    }
                    s += "C";
                }
                s += "P";
            }
            s = s.Substring(0, s.Length - 3);
            s += "K"; // 0
            for (byte c = 0; c <= UnoColor.MAX_VALUE; c++)
            {
                for (byte n = 0; n <= UnoNumber.MAX_VALUE; n++)
                {
                    s += BoxCards.uno[c, n] + "N";
                }
                s += "C";
            }
            s += "K"; // 1
            s += GetColorId(BackColor) + "K"; // 2
            s += GetNumberId(lblCards[1].Text) + "K"; // 3
            s += skip + "K"; // 4
            foreach (int sk in skips)
                s += sk + "P";
            s = s.Substring(0, s.Length - 1);
            s += "K"; // 5
            s += reverse + "K"; // 6
            s += lblDraw.Text + "K"; // 7
            s += gametime + "K"; // 8
            s += form.saveRules();
            return s;
        }

        private void SetUsage(Control card)
        {
            byte color = GetColorId(card.BackColor);
            toolTip.SetToolTip(card, "" +
                "颜色\t" + GetColorName(color) + "\n" +
                "数字\t" + card.Text + "\n" +
                "类型\t" + GetType(color, GetNumberId(card.Text)) + "\n" +
                "功能\t" + GetUsage(card.Text));
        }

		private void TimBoxToCenter_Tick(object sender, EventArgs e) {
            lblMoving[0].Location = new Point(lblCards[0].Left / 20 * movingCard.progress, lblCards[0].Top / 20 * movingCard.progress + mnuGame.Height);
			if (lblMoving[0].Left >= lblCards[0].Left && lblMoving[0].Top >= lblCards[0].Top)
            {
                movingCard.progress = 0;
                Card[] box = this.Box();
				Card rndCard = box[(int)(box.Length * Rnd())];
				BoxCards.uno[rndCard.color, rndCard.number]--;
				lblBox.Text = box.Length - 1 + "";
				AddLabel(lblCards);
				lblCards[1].BackColor = GetColor(rndCard.color);
				lblCards[1].BringToFront();
				lblCards[1].Font = new Font(lblCards[1].Font.FontFamily, 42);
				lblCards[1].ForeColor = Color.White;
				lblCards[1].Location = new Point(lblCards[0].Left, lblCards[0].Top);
				lblCards[1].Text = GetNumber(rndCard.number);
				lblCards[1].TextAlign = ContentAlignment.MiddleCenter;
                SetUsage(lblCards[1]);
				BackColor = lblCards[1].BackColor;
                if (new Regex("^(Magenta|Black)$").IsMatch(BackColor.Name))
                {
                    movingCard.progress = 0;
                    return;
                }
                timBoxToCenter.Enabled = false;
                lblMoving[0].Location = new Point(-UnoSize.WIDTH, -UnoSize.HEIGHT);
                movingCard.playing = true;
                reverse = Math.Floor(2 * Rnd()) == 0;
                if (form.keys.Length == 0)
                    PlayersTurn(NextPlayer((byte)(4 * Rnd())), true, form.mnuDrawBeforePlaying.Checked);
                else
                {
                    PlayersTurn(0);
                    try
                    {
                        LoadGame(form.keys);
                    }
                    catch
                    {
                        MessageBox.Show("无效的游戏记彔!", "读取游戏", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        FormClosing -= new FormClosingEventHandler(FrmUno_FormClosing);
                        Application.Restart();
                    }
                }
                if (form.mnuWatch.Checked)
                {
                    timWatch.Enabled = true;
                    lblWatch.Visible = true;
                }
            }
            else movingCard.progress++;
		}

		private void TimBoxToPlayers_Tick(object sender, EventArgs e) {
            if (movingCard.quickly)
                lblMoving[0].Location = lblPlayers[movingCard.player].Location;
            else
                lblMoving[0].Location = new Point(lblPlayers[movingCard.player].Left / 20 * movingCard.progress, lblPlayers[movingCard.player].Top / 20 * movingCard.progress + mnuGame.Height);
            if (lblMoving[0].Left >= lblPlayers[movingCard.player].Left && lblMoving[0].Top >= lblPlayers[movingCard.player].Top)
            {
                Card[] box = this.Box();
				Card rndCard = box[(int)(box.Length * Rnd())];
				BoxCards.uno[rndCard.color, rndCard.number]--;
				lblBox.Text = box.Length - 1 + "";
                Players[movingCard.player].uno[rndCard.color, rndCard.number]++;
                lblCounts[movingCard.player].Text = PlayersCards(movingCard.player).Length + "";
                if (movingCard.player == 0 && !movingCard.quickly)
                    Rank();
                if (movingCard.playing)
                {
                    bool drawAll = false;
                    if (int.Parse(lblDraw.Text) > 0 && !movingCard.one && movingCard.downpour <= -1)
                    {
                        lblDraw.Text = int.Parse(lblDraw.Text) - 1 + "";
                        if (lblDraw.Text == "0")
                            drawAll = true;
                    }
                    if (int.Parse(lblDraw.Text) <= 0 || movingCard.one || movingCard.downpour > -1)
                    {
                        lblMoving[0].Location = new Point(-UnoSize.WIDTH, -UnoSize.HEIGHT);
                        timBoxToPlayers.Enabled = false;
                        if (!form.mnuDrawTilCanPlay.Checked && !movingCard.one && movingCard.downpour <= -1)
                            movingCard.drew = !movingCard.unoDraw;
                        if (movingCard.player == 0 && movingCard.quickly && movingCard.downpour <= 3)
                            Rank();
                        if (movingCard.one)
                        {
                            movingCard.one = false;
                            PlayersTurn(movingCard.player);
                        }
                        else if (movingCard.downpour > -1)
                        {
                            movingCard.downpour--;
                            if (movingCard.downpour == 0)
                            {
                                movingCard.downpour = -1;
                                PlayersTurn(NextPlayer(NextPlayer(movingCard.player)), true, form.mnuDrawBeforePlaying.Checked);
                            }
                            else
                            {
                                movingCard.player = NextPlayer(movingCard.player);
                                byte ons = 0;
                                foreach (Label p in lblPlayers)
                                    if (p.Visible)
                                        ons++;
                                if (movingCard.downpour % (ons - 1) == 0)
                                    movingCard.player = NextPlayer(movingCard.player);
                                timBoxToPlayers.Enabled = true;
                            }
                        }
                        else if (movingCard.unoDraw)
                        {
                            movingCard.unoDraw = false;
                            PlayersTurn(NextPlayer(movingCard.player), true, form.mnuDrawBeforePlaying.Checked);
                        }
                        else if (!form.mnuDrawAndPlay.Checked || !form.mnuDrawAllAndPlay.Checked && drawAll)
                        {
                            movingCard.drew = false;
                            PlayersTurn(NextPlayer(movingCard.player), true, form.mnuDrawBeforePlaying.Checked);
                        }
                        else
                            PlayersTurn(movingCard.player);
                    }
				}
                else
                {
                    if (movingCard.player == NextPlayer(0, true)) 
                        if (int.Parse(lblCounts[NextPlayer(0, true)].Text) >= Int32.Parse(form.txtDealt.Text))
                        {
						    timBoxToPlayers.Enabled = false;
                            if (movingCard.quickly)
                                Rank();
						    timBoxToCenter.Enabled = true;
					    }
                    movingCard.player = NextPlayer(movingCard.player);
				}
                movingCard.progress = 0;
            }
            else movingCard.progress++;
		}

        private void TimChallenge_Tick(object sender, EventArgs e)
        {
            timChallenge.Enabled = false;
            Draw(0);
        }

		private void TimPlayersToCenter_Tick(object sender, EventArgs e) {
            int x, y;
            switch (movingCard.player)
            {
				case 0:
                    x = Width / 2 - UnoSize.WIDTH * (lblMoving.ToArray().Length - 1) / 2;
                    y = height - (lblPlayers[movingCard.player].Top - lblCards[0].Top) / 20 * movingCard.progress;
                    for (int c = 1; c < lblMoving.ToArray().Length; c++)
                        lblMoving[c].Location = new Point(x + UnoSize.WIDTH * (c - 1), y);
					if (lblMoving[1].Top <= lblCards[0].Top)
                        goto arrive;
					break;
				case 1:
                    x = lblCards[0].Left / 20 * movingCard.progress;
                    y = height / 2 - UnoSize.HEIGHT * (lblMoving.ToArray().Length - 1) / 2;
                    for (int c = 1; c < lblMoving.ToArray().Length; c++)
                        lblMoving[c].Location = new Point(x, y + UnoSize.HEIGHT * (c - 1));
					if (lblMoving[1].Left >= lblCards[0].Left)
                        goto arrive;
					break;
				case 2:
                    x = Width / 2 - UnoSize.WIDTH * (lblMoving.ToArray().Length - 1) / 2;
                    y = lblCards[0].Top / 20 * movingCard.progress;
                    for (int c = 1; c < lblMoving.ToArray().Length; c++)
                        lblMoving[c].Location = new Point(x + UnoSize.WIDTH * (c - 1), y);
					if (lblMoving[1].Top >= lblCards[0].Top)
                        goto arrive;
					break;
				case 3:
                    x = Width - (lblPlayers[movingCard.player].Left - lblCards[0].Left) / 20 * movingCard.progress;
                    y = height / 2 - UnoSize.HEIGHT * (lblMoving.ToArray().Length - 1) / 2;
                    for (int c = 1; c < lblMoving.ToArray().Length; c++)
                        lblMoving[c].Location = new Point(x, y + UnoSize.HEIGHT * (c - 1));
					if (lblMoving[1].Left <= lblCards[0].Left)
                        goto arrive;
					break;
			}
            movingCard.progress++;
			return;
arrive:     
            movingCard.progress = 0;
			RemoveLabel(lblCards);
			AddLabel(lblCards, lblMoving.ToArray().Length - 1);
			for (int c = 1; c < lblCards.ToArray().Length; c++)
            {
				lblCards[c].BackColor = lblMoving[c].BackColor; lblCards[c].Text = lblMoving[c].Text;
				lblCards[c].BringToFront();
                lblCards[c].Location = new Point((Width / 2 - UnoSize.WIDTH * (lblMoving.ToArray().Length - 1) / 2) + UnoSize.WIDTH * (c - 1), lblCards[0].Top);
                SetUsage(lblCards[c]);
			}
			RemoveLabel(lblMoving);
            BackColor = GetColor(movingCard.color);
            lblCounts[movingCard.player].Text = PlayersCards(movingCard.player).Length + "";
            int downpour = 0, length = lblCards.ToArray().Length - 1;
            bool reversed = false;
            byte ons = 0;
            foreach (Label p in lblPlayers)
                if (p.Visible)
                    ons++;
            if (ons == 2 && lblPlayers[movingCard.player].Visible)
            {
                foreach (Label l in lblCards)
                    if (l.Text == UnoNumberName.REVERSE || l.Text == form.txtBlankText.Text && form.mnuBlankReverse.Checked)
                    {
                        reversed = true;
                        break;
                    }
            }
            else
                foreach (Label l in lblCards)
                    if (l.Text == UnoNumberName.REVERSE || l.Text == form.txtBlankText.Text && form.mnuBlankReverse.Checked)
                    {
                        reverse = !reverse;
                    }
            foreach (Label l in lblCards)
            {
                if (l.Text == UnoNumberName.NULL)
                    continue;
                switch (l.Text)
                {
                    case "0":
                        if (form.mnuSevenZero.Checked)
                        {
                        }
                        break;
                    case "7":
                        if (form.mnuSevenZero.Checked)
                        {
                        }
                        break;
                    case UnoNumberName.SKIP:
                        if (form.mnuSkipPlayers.Checked) skip++;
                        else skips[NextPlayer(movingCard.player)]++;
                        break;
                    case UnoNumberName.REVERSE:
                        break;
                    case UnoNumberName.DRAW_2:
                        AddDraw(2);
                        break;
                    case UnoNumberName.TRADE_HANDS:
                        break;
                    case UnoNumberName.WILD_DOWNPOUR_DRAW_1:
                        downpour += (form.mnuDoubleDraw.Checked ? 2 : 1);
                        break;
                    case UnoNumberName.WILD_DOWNPOUR_DRAW_2:
                        downpour += 2 * (form.mnuDoubleDraw.Checked ? 2 : 1);
                        break;
                    case UnoNumberName.WILD_DOWNPOUR_DRAW_4:
                        downpour += 4 * (form.mnuDoubleDraw.Checked ? 2 : 1);
                        break;
                    case UnoNumberName.WILD_DRAW_4:
                        AddDraw(4);
                        break;
                    case UnoNumberName.WILD_HITFIRE:
                        lblDraw.Text = lblBox.Text;
                        break;
                    default:
                        if (lblCards[1].Text == form.txtBlankText.Text)
                        {
                            if (form.mnuSkipPlayers.Checked)
                                skip += int.Parse(form.txtBlankSkip.Text);
                            else
                                skips[NextPlayer(movingCard.player)] += int.Parse(form.txtBlankSkip.Text);
                            AddDraw(int.Parse(form.txtBlankDraw.Text));
                        }
                        break;
                }
            }
            if (movingCard.player == 0 && (PlayersCards(0).Length == 1 && !rdoUno.Checked || PlayersCards(0).Length != 1 && rdoUno.Checked))
            {
                Action(0, "UNO? +2");
				AddDraw(2);
				PlayersTurn(0, false);
				rdoUno.Checked = false;
                movingCard.unoDraw = true;
			}
            timPlayersToCenter.Enabled = false;
            if (!form.mnuDrawTilCanPlay.Checked)
                movingCard.drew = false;
            if (movingCard.unoDraw)
                timUno.Enabled = true;
            else if (reversed)
                PlayersTurn(movingCard.player, true, form.mnuDrawBeforePlaying.Checked);
            else if (downpour > 0)
                DownpourDraw(movingCard.player, downpour);
            else
                PlayersTurn(movingCard.player = NextPlayer(movingCard.player), true, form.mnuDrawBeforePlaying.Checked);
		}

        private void TimTurn_Tick(object sender, EventArgs e)
        {
            timTurn.Enabled = false;
            string[] tag = timTurn.Tag.ToString().Split(char.Parse(","));
            PlayersTurn(byte.Parse(tag[0]), true, bool.Parse(tag[1]));
        }

        private void TimUno_Tick(object sender, EventArgs e)
        {
            timUno.Enabled = false;
            Draw(0);
        }

        private void TimWatch_Tick(object sender, EventArgs e)
        {
            gametime++;
            double h = Math.Floor(gametime / 3600.0), m = Math.Floor(gametime / 60.0);
            lblWatch.Text = h + "°" + (m - h * 60) + "′" + (gametime - m * 60) + "″";
        }

        private void ToolTip_Popup(object sender, PopupEventArgs e)
        {
            if (e.AssociatedControl.Size == new Size(UnoSize.WIDTH, UnoSize.HEIGHT))
                toolTip.ToolTipTitle = GetNumberName(GetNumberId(e.AssociatedControl.Text));
            else
                toolTip.ToolTipTitle = "";
        }
	}
}
