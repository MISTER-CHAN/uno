using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web.Security;
using System.Windows.Forms;

namespace Uno
{
    public partial class Uno : Form {
		public class Card {
            public byte color = UnoColor.MAX_VALUE;
            public byte number = UnoNumber.MAX_VALUE;
		}

		public class Cards {
			public int[,] cards = new int[UnoColor.MAX_VALUE + 1, UnoNumber.MAX_VALUE + 1];
		}

        public class Downpour
        {
            public static byte player = 4;
            public static int count = 0;
        }

		public class MovingCard {
            public static byte color = 0, number = 0, player = 0;
			public static bool drew = false, playing = false, quickly = false, unoDraw = false;
			public static int dbp = 0, downpour = -1, progress = 0;
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
            public const byte SKIP = 11;
            public const byte REVERSE = 12;
            public const byte DRAW_2 = 13;
            public const byte DISCARD_ALL = 14;
            public const byte TRADE_HANDS = 15;
            public const byte NUMBER = 16;
            public const byte BLANK = 17;
            public const byte WILD = 18;
            public const byte WILD_DOWNPOUR_DRAW_1 = 19;
            public const byte WILD_DOWNPOUR_DRAW_2 = 20;
            public const byte WILD_DOWNPOUR_DRAW_4 = 21;
            public const byte WILD_DRAW_4 = 22;
            public const byte WILD_HITFIRE = 23;
            public const byte NULL = 24;
            public const byte MAX_VALUE = 24;
        }

        public class UnoNumberName
        {
            public const string SKIP = "⊘";
            public const string REVERSE = "⇅";
            public const string DRAW_2 = "+2";
            public const string DISCARD_ALL = "×";
            public const string TRADE_HANDS = "<>";
            public const string NUMBER = "#";
            public const string WILD = "∷";
            public const string WILD_DOWNPOUR_DRAW_1 = "!1";
            public const string WILD_DOWNPOUR_DRAW_2 = "!2";
            public const string WILD_DOWNPOUR_DRAW_4 = "!4";
            public const string WILD_DRAW_4 = "+4";
            public const string WILD_HITFIRE = "+?";
            public const string NULL = "   ";
        }

        private bool reverse = false, isAutomatic = false, isSelectingCards = false;
        byte gameOver = 4;
        readonly byte[]
            mvcList = new byte[UnoNumber.MAX_VALUE]
            {
                UnoNumber.WILD_HITFIRE, UnoNumber.WILD_DRAW_4,
                UnoNumber.WILD_DOWNPOUR_DRAW_4, UnoNumber.WILD_DOWNPOUR_DRAW_2, UnoNumber.WILD_DOWNPOUR_DRAW_1,
                UnoNumber.WILD, UnoNumber.BLANK, UnoNumber.DISCARD_ALL, UnoNumber.NUMBER, UnoNumber.DRAW_2, UnoNumber.SKIP, UnoNumber.REVERSE,
                0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, UnoNumber.TRADE_HANDS
            },
            playlist = new byte[UnoNumber.MAX_VALUE] {
            UnoNumber.DISCARD_ALL,
            UnoNumber.SKIP,
            UnoNumber.REVERSE,
            UnoNumber.DRAW_2,
            10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0,
            UnoNumber.NUMBER,
            UnoNumber.TRADE_HANDS,
            UnoNumber.BLANK,
            UnoNumber.WILD,
            UnoNumber.WILD_DOWNPOUR_DRAW_1,
            UnoNumber.WILD_DOWNPOUR_DRAW_2,
            UnoNumber.WILD_DOWNPOUR_DRAW_4,
            UnoNumber.WILD_DRAW_4,
            UnoNumber.WILD_HITFIRE
        };
        readonly Cards Pile = new Cards();
        readonly Cards[] Players = new Cards[4];
        readonly Image imgUno;
        private readonly int distance = 20;
        private int gametime = 0, height = 0, skip = 0, swpcw = 0, width = 0;
        readonly int[] skips = new int[4];
        readonly Label[] lblCounts = new Label[4];
        public Label[] lblPlayers = new Label[4];
        readonly List<CheckBox> chkPlayer = new List<CheckBox>();
        private readonly List<Label> lblCards = new List<Label>();
        private readonly List<Label> lblMovingCards = new List<Label>();
        readonly Options form;

        void Action(byte player, string msg)
        {
            lblAction.Text = msg;
            lblAction.BringToFront();
            switch (player)
            {
                case 0: lblAction.Location = new Point(width / 2 - lblAction.Width / 2, lblCounts[0].Top - lblAction.Height); break;
                case 1: lblAction.Location = new Point(lblPlayers[1].Left + UnoSize.WIDTH, height / 2 - lblAction.Height / 2); break;
                case 2: lblAction.Location = new Point(width / 2 - lblAction.Width / 2, lblCounts[2].Top + lblCounts[2].Height); break;
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
                chkPlayer[length].BackgroundImageLayout = ImageLayout.Stretch;
				chkPlayer[length].BringToFront();
				chkPlayer[length].CheckedChanged += new EventHandler(ChkPlayer_CheckedChanged);
                chkPlayer[length].Enter += new EventHandler(ChkPlayer_Enter);
                chkPlayer[length].FlatStyle = FlatStyle.Flat;
                chkPlayer[length].ForeColor = Color.White;
                chkPlayer[length].KeyDown += new KeyEventHandler(Control_KeyDown);
                chkPlayer[length].MouseDown += new MouseEventHandler(ChkPlayer_MouseDown);
                chkPlayer[length].MouseLeave += new EventHandler(ChkPlayer_MouseLeave);
                chkPlayer[length].MouseMove += new MouseEventHandler(ChkPlayer_MouseMove);
                chkPlayer[length].MouseUp += new MouseEventHandler(ChkPlayer_MouseUp);
                chkPlayer[length].MouseWheel += new MouseEventHandler(ChkPlayer_MouseWheel);
                chkPlayer[length].Size = new Size(UnoSize.WIDTH, UnoSize.HEIGHT);
                chkPlayer[length].Tag = length;
                chkPlayer[length].TextChanged += Card_TextChanged;
				chkPlayer[length].TextAlign = ContentAlignment.MiddleCenter;
            }
		}

        private void Card_TextChanged(object sender, EventArgs e)
        {
            Control control = (Control)sender;
            if (mnuAppearance.Checked)
            {
                byte b = GetNumberId(control.Text);
                Image image = new Bitmap(120, 160);
                Graphics graphics = Graphics.FromImage(image);
                graphics.DrawImage(imgUno, new Rectangle(0, 0, 120, 160), b * 120, GetColorId(control.BackColor) * 160, 120, 160, GraphicsUnit.Pixel);
                control.BackgroundImage = image;
                control.Font = new Font(control.Font.FontFamily, 1);
                graphics.Flush();
                graphics.Dispose();
            }
        }

        void AddDraw(int count)
        {
            lblDraw.Text = int.Parse(lblDraw.Text) + (form.mnuDoubleDraw.Checked ? 2 : 1) * count + "";
        }

		void AddLabel(List<Label> label, int count = 1, Control.ControlCollection controls = null) {
            if (controls == null)
            {
                controls = Controls;
            }
			for (int c = 1; c <= count; c++) {
				int length = label.ToArray().Length;
				label.Add(new Label());
                controls.Add(label[length]);
                label[length].AutoSize = false;
                label[length].BackColor = Color.Black;
                label[length].BackgroundImageLayout = ImageLayout.Stretch;
                label[length].BorderStyle = BorderStyle.FixedSingle;
                label[length].BringToFront();
                label[length].ForeColor = Color.White;
                label[length].Location = new Point(-UnoSize.WIDTH, -UnoSize.HEIGHT);
                label[length].MouseEnter += new EventHandler(Label_MouseEnter);
                label[length].MouseLeave += new EventHandler(Label_MouseLeave);
                label[length].Size = new Size(UnoSize.WIDTH, UnoSize.HEIGHT);
                label[length].Text = UnoNumberName.NULL;
                label[length].TextChanged += Card_TextChanged;
                label[length].TextAlign = ContentAlignment.MiddleCenter;
            }
		}

        Card[] Ai(byte player) {
            byte backColor = GetColorId(BackColor), backNumber = GetNumberId(lblCards[1].Text);
            Card bestCard = new Card();
            List<Card> cards = new List<Card>();
            if (!form.mnuStackDraw.Checked && int.Parse(lblDraw.Text) > 0)
            {
                goto exit;
            }
            int mostQuantity, quantity, quantityColor = GetQuantityByColor(player, backColor), quantityNumber = 0;
            if (backNumber == UnoNumber.NUMBER)
            {
                for (byte b = 10; b >= 0 && b < byte.MaxValue; b--)
                {
                    int i = GetQuantityByNumber(player, b);
                    if (i > quantityNumber)
                    {
                        quantityNumber = i;
                        backNumber = b;
                    }

                }
            }
            else
                quantityNumber = GetQuantityByNumber(player, backNumber);
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

                if (backNumber <= 10 && GetQuantityByNumber(player, UnoNumber.NUMBER) > 0)
                {
                    bestCard.number = UnoNumber.NUMBER;
                }
                else if (Players[player].cards[UnoColor.MAGENTA, UnoNumber.BLANK] > 0)
                {
                    bestCard.color = UnoColor.MAGENTA;
                    bestCard.number = UnoNumber.BLANK;
                }
                else
                    for ( byte n = 0; n <= UnoNumber.MAX_VALUE; n++)
                        if (Players[player].cards[UnoColor.BLACK, n] > 0)
                        {
                            bestCard.color = UnoColor.BLACK;
                            bestCard.number = n;
                            break;
                        }
            }
            else if (quantityColor >= quantityNumber)
            {
                if (!form.mnuPairs.Checked)
                {
                    for (byte b = 0; b <= UnoNumber.BLANK; b++)
                    {
                        byte n = playlist[b];
                        if (Players[player].cards[backColor, n] > 0)
                        {
                            bestCard.color = backColor;
                            bestCard.number = n;
                            break;
                        }
                    }
                }
                else if (int.Parse(lblCounts[player].Text) > 7 && GetDbp() > 0)
                {
                    mostQuantity = 0;
                    for (byte b = 0; b <= UnoNumber.BLANK; b++)
                    {
                        byte n = playlist[b];
                        if (Players[player].cards[backColor, n] <= 0)
                        {
                            continue;
                        }
                        quantity = GetColorQuantityByNumber(player, n);
                        if (quantity > mostQuantity)
                        {
                            mostQuantity = quantity;
                            bestCard.color = backColor;
                            bestCard.number = n;
                        }
                    }
                }
                else
                {
                    int fewestQuantity = 5;
                    for (byte b = 0; b <= UnoNumber.BLANK; b++)
                    {
                        byte n = playlist[b];
                        if (Players[player].cards[backColor, n] <= 0)
                        {
                            continue;
                        }
                         quantity = GetColorQuantityByNumber(player, n);
                        if (0 < quantity && quantity < fewestQuantity)
                        {
                            fewestQuantity = quantity;
                            bestCard.color = backColor;
                            bestCard.number = n;
                        }
                    }
                }
            }
            else
            {
                bestCard.number = backNumber;
            }
            if (Players[player].cards[UnoColor.BLACK, bestCard.number] <= 0)
            {
                if (form.mnuPairs.Checked)
                {
                    int mp = 0, mq = 0;
                    for (byte color = UnoColor.RED; color <= UnoColor.BLUE; color++)
                        if (Players[player].cards[color, bestCard.number] > 0)
                        {
                            int q = GetQuantityByColor(player, color);
                            int p = GetPointsByColor(player, color);
                            if (q > mq || q == mq && p > mp)
                            {
                                mq = q;
                                MovingCard.color = color;
                            }
                            Card card = new Card
                            {
                                color = color,
                                number = bestCard.number
                            };
                            for (byte c = 1; c <= Players[player].cards[color, card.number]; c++)
                                cards.Add(card);
                        }
                    if (bestCard.number == UnoNumber.BLANK && Players[player].cards[UnoColor.MAGENTA, UnoNumber.BLANK] > 0)
                    {
                        if (cards.Count == 0)
                            MovingCard.color = backColor;
                        Card card = new Card
                        {
                            color = UnoColor.MAGENTA,
                            number = UnoNumber.BLANK
                        };
                        for (byte c = 1; c <= Players[player].cards[UnoColor.MAGENTA, UnoNumber.BLANK]; c++)
                            cards.Add(card);
                    }
                }
                else
                {
                    if (bestCard.color == UnoColor.WHITE)
                    {
                        int mp = 0, mq = 0;
                        for (byte c = UnoColor.RED; c <= UnoColor.BLUE; c++)
                        {
                            if (Players[player].cards[c, bestCard.number] > 0)
                            {
                                int q = GetQuantityByColor(player, c);
                                int p = GetPointsByColor(player, c);
                                if (q > mq || q == mq && p > mp)
                                {
                                    mq = q;
                                    MovingCard.color = c;
                                }
                            }
                        }
                        bestCard.color = MovingCard.color;
                        if (bestCard.number < UnoNumber.MAX_VALUE)
                        {
                            cards.Add(bestCard);
                        }
                    }
                    else if (Players[player].cards[bestCard.color, bestCard.number] > 0)
                    {
                        MovingCard.color = bestCard.color == UnoColor.MAGENTA ? backColor : bestCard.color;
                        cards.Add(bestCard);
                    }
                }
                if (bestCard.number == UnoNumber.NUMBER)
                {
                    List<Card> numbers = new List<Card>();
                    byte number;
                    for (byte n = 10; n >= 0 && n < byte.MaxValue; n--)
                    {
                        for (byte c = UnoColor.RED; c <= UnoColor.BLUE; c++)
                        {
                            if (Players[player].cards[c, n] > 0)
                            {
                                number = n;
                                goto number_color;
                            }
                        }
                    }
                    goto exit;
number_color:
                    for (byte c = UnoColor.RED; c <= UnoColor.BLUE; c++)
                    {
                        for (int i = 1; i <= Players[player].cards[c, number]; i++)
                        {
                            Card card = new Card() { color = c, number = number };
                            numbers.Add(card);
                        }
                    }
                    numbers.AddRange(cards);
                    cards = numbers;
                }
                else if (bestCard.number == UnoNumber.DISCARD_ALL)
                {
                    byte color = lblCards[1].Text == UnoNumberName.DISCARD_ALL ? MovingCard.color : backColor;
                    for (byte n = 0; n <= UnoNumber.MAX_VALUE; n++)
                    {
                        if (n == UnoNumber.DISCARD_ALL)
                            continue;
                        for (int c = 1; c <= Players[player].cards[color, n]; c++)
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
                else if (form.mnuDos.Checked)
                {
                    if (bestCard.number <= 10)
                    {
                        List<Card> number = new List<Card>();
                        for (byte c = UnoColor.RED; c <= UnoColor.BLUE; c++)
                        {
                            for (byte u = 1; u <= Players[player].cards[c, UnoNumber.NUMBER]; u++)
                            {
                                Card card = new Card
                                {
                                    color = c,
                                    number = UnoNumber.NUMBER
                                };
                                number.Add(card);
                            }
                        }
                        if (cards.Count + number.Count == int.Parse(lblCounts[player].Text))
                            cards.AddRange(number);
                    }
                }
            }
            else
            {
                bestCard.color = UnoColor.BLACK;
                int mp = 0, mq = 0;
                for (byte c = UnoColor.RED; c <= UnoColor.BLUE; c++)
                {
                    int q = GetQuantityByColor(player, c);
                    int p = GetPointsByColor(player, c);
                    if (q > mq || q == mq && p > mp)
                    {
                        mq = q;
                        MovingCard.color = c;
                    }
                }
                if (form.mnuPairs.Checked)
                {
                    for (byte c = UnoColor.RED; c <= UnoColor.BLACK; c++)
                        for (byte u = 1; u <= Players[player].cards[c, bestCard.number]; u++)
                        {
                            Card card = new Card
                            {
                                color = c,
                                number = bestCard.number
                            };
                            cards.Add(card);
                        }
                }
                else if (Players[player].cards[UnoColor.BLACK, bestCard.number] > 0)
                    cards.Add(bestCard);
            }
exit:
            return cards.ToArray();
		}

        private void BtnChallenge_Click(object sender, EventArgs e)
        {
            pnlCtrl.Visible = false;
            Action(0, "检举失败! +2");
            AddDraw(2);
            if (!form.mnuDrawTilCanPlay.Checked) MovingCard.drew = false;
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
            {
                btnPlay.BackColor = btnPlay.BackColor.Name switch
                {
                    "Red" => Color.Yellow,
                    "Yellow" => Color.Lime,
                    "Lime" => Color.Blue,
                    _ => Color.Red,
                };
            }
        }

		bool CanPlay(List<Card> card, byte color)
        {
            if (!form.mnuStackDraw.Checked && int.Parse(lblDraw.Text) > 0)
            {
                goto deny;
            }
            byte backColor = GetColorId(BackColor), backNumber = GetNumberId(lblCards[1].Text);
            if (form.mnuDaWah.Checked)
            {
                byte discardColor = UnoColor.MAX_VALUE;
                if (lblCards[1].Text != UnoNumberName.DISCARD_ALL)
                    discardColor = backColor;
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
                        goto draw;
                    }
                if (!form.mnuPairs.Checked)
                    if (card.ToArray().Length > 1)
                        goto deny;
            }
            if (form.mnuDos.Checked)
            {
                byte n = backNumber;
                foreach (Card c in card)
                {
                    if (c.color == backColor)
                    {
                        n = UnoNumber.MAX_VALUE;
                        break;
                    }
                }
                if (n > 10 && n < UnoNumber.MAX_VALUE)
                    goto number;
                foreach (Card c in card)
                {
                    if (c.number <= 10)
                    {
                        if (n == UnoNumber.MAX_VALUE)
                            n = c.number;
                        else if (c.number != n)
                            goto number;
                    }
                    else if (c.number != UnoNumber.NUMBER)
                        goto number;
                }
                goto accept;
            }
number:
            foreach (Card c in card)
            {
                if (c.number != card[0].number)
                    goto deny;
            }
draw:
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
            for (int c = 0; c < card.ToArray().Length; c++)
            {
                if (card[c].color == backColor || card[c].color == UnoColor.MAGENTA)
                    goto accept;
            }
            if (card[0].number == backNumber || backNumber == UnoNumber.NUMBER)
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
            if (mnuAppearance.Checked)
            {
                chkPlayer[index].Top = chkPlayer[index].Checked ? 0 : UnoSize.HEIGHT / 8;
            }
            if (!form.mnuPairs.Checked && !form.mnuDaWah.Checked && ((CheckBox)sender).Checked)
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
                if (c.Left + pnlPlayer.Left < 0 || c.Left + c.Width + pnlPlayer.Left > width)
                    if (c.Left >= pnlPlayer.Width - width)
                        pnlPlayer.Left = -pnlPlayer.Width + width;
                    else
                        pnlPlayer.Left = -c.Left;
            }
        }

        void ChkPlayer_MouseDown(object sender, MouseEventArgs e)
        {
            isSelectingCards = true;
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
                int w;
                if (hPlayer.Visible || pnlPlayer.Left > 0)
                    w = UnoSize.WIDTH;
                else
                    w = width / chkPlayer.ToArray().Length;
                int i = (int)Math.Floor((double)e.X / w) + int.Parse(((CheckBox)sender).Tag + "");
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
                        if (hPlayer.Maximum - hPlayer.Value >= width / 2)
                        {
                            hPlayer.Value += width / 2;
                        }
                        else
                        {
                            hPlayer.Value = hPlayer.Maximum;
                        }
                        break;
                    case 1:
                        if (hPlayer.Value - hPlayer.Minimum >= width / 2)
                        {
                            hPlayer.Value -= width / 2;
                        }
                        else
                        {
                            hPlayer.Value = hPlayer.Minimum;
                        }
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
                MovingCard.quickly = true;
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
                    if (mnuRadioBox.Checked)
                        return;
                    BtnPlay_MouseDown(btnPlay, new MouseEventArgs(MouseButtons.Right, 1, 0, 0, 0));
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
            if (form.mnuPairs.Checked)
            {
                foreach (CheckBox c in chkPlayer)
                {
                    if (c.Text == number)
                    {
                        c.Checked = true;
                        c.Focus();
                    }
                    else
                    {
                        c.Checked = false;
                    }
                }
            }
            else
            {
                bool b = true;
                foreach (CheckBox c in chkPlayer)
                {
                    if (c.Checked && c.Text == number)
                    {
                        b = false;
                    }
                    else
                    {
                        c.Checked = false;
                    }
                }
                foreach (CheckBox c in chkPlayer)
                {
                    if (!b)
                    {
                        if (c.Checked)
                        {
                            b = true;
                            c.Checked = false;
                        }
                        continue;
                    }
                    if (c.Text == number)
                    {
                        if (c.Left + pnlPlayer.Left < 0 || c.Left + c.Width + pnlPlayer.Left > width)
                            pnlPlayer.Left = -c.Left;
                        c.Checked = true;
                        c.Focus();
                        break;
                    }
                }
            }
        }

        void DownpourDraw(byte player, int draw)
        {
            byte ons = 0;
            for (byte p = 0; p <= 3; p++)
                if (lblPlayers[p].Visible && p != player)
                    ons++;
            MovingCard.downpour = draw * ons;
            MovingCard.player = NextPlayer(player);
            MovingCard.progress = 0;
            MovingCard.quickly = false;
            lblMovingCards[0].BringToFront();
            timPileToPlayers.Enabled = true;
        }

        void Draw(byte player)
        {
            CheckPile();
            if ((!MovingCard.drew || form.mnuDrawTilCanPlay.Checked) && int.Parse(lblPile.Text) > 0)
            {
                Action(player, "摸牌");
                if (player == 0)
                {
                    pnlCtrl.Visible = false;
                    if (form.mnuUno.Checked && rdoUno.Checked)
                    {
                        AddDraw(2);
                        Action(0, "UNO? +2");
                    }
                }
                MovingCard.player = player; MovingCard.progress = 0; MovingCard.quickly = false;
                lblMovingCards[0].BringToFront();
                timPileToPlayers.Enabled = true;
            }
            else
            {
                Action(player, "过牌");
                MovingCard.drew = false;
                PlayersTurn(player, false);
                PlayersTurn(NextPlayer(player), true, GetDbp());
			}
		}

		public Uno(Options form) 
        {
            InitializeComponent();
            height = Height - mnuGame.Height;
            this.form = form;
            imgUno = Properties.Resources.uno;
            lblPile.Top = mnuGame.Height;
            distance = form.animation;
            int interval = form.animation * 50;
            if (interval <= 0)
            {
                distance = 1;
                interval = 1;
            }
            timTurn.Interval = interval;
            timChallenge.Interval = interval;
            timUno.Interval = interval;
            if (mnuRightClick.Checked) btnPlay.BackColor = Color.Red;
            if (form.mnuJumpin.Checked) btnJumpin.Visible = true;
			for (byte i = 0; i < 4; i++) {
                lblPlayers[i] = new Label();
				Controls.Add(lblPlayers[i]);
                lblPlayers[i].AutoSize = form.mnuCanShowCards.Checked;
                lblPlayers[i].BackColor = Color.Black;
                lblPlayers[i].ForeColor = Color.White;
                lblPlayers[i].Tag = i;
                lblPlayers[i].Text = "UNO";
                lblPlayers[i].TextAlign = ContentAlignment.MiddleCenter;
                lblPlayers[i].Size = new Size(UnoSize.WIDTH, 120);
                lblPlayers[i].Tag = i;
				if (i > 0) lblPlayers[i].BorderStyle = BorderStyle.FixedSingle;
                lblPlayers[i].BackColorChanged += new EventHandler(Control_BackColorChanged);
                lblPlayers[i].Click += new EventHandler(LblPlayers_Click);
                lblCounts[i] = new Label();
				Controls.Add(lblCounts[i]);
                lblCounts[i].AutoSize = true;
                lblCounts[i].Font = new Font("微軟正黑體 Light", lblCounts[i].Font.Size);
				lblCounts[i].TextAlign = ContentAlignment.MiddleCenter;
				lblCounts[i].Tag = i;
				lblCounts[i].Text = "0";
                lblCounts[i].SizeChanged += LblCounts_SizeChanged;
                lblCounts[i].BackColorChanged += new EventHandler(Control_BackColorChanged);
				lblCounts[i].TextChanged += new EventHandler(LblCounts_TextChanged);
            }
            lblPlayers[0].BackColor = Color.Transparent;
            lblPlayers[0].Text = "";
            for (int o = 0; o < lblPlayers.Length; o++)
            {
                lblPlayers[o].Visible = ((ToolStripMenuItem)form.mnuOns.DropDownItems[o]).Checked;
                lblCounts[o].Visible = ((ToolStripMenuItem)form.mnuOns.DropDownItems[o]).Checked;
            }
            lblCards.Add(new Label());
			Controls.Add(lblCards[0]);
            lblCards[0].Visible = false;
            ResizeForm();
            int decks = int.Parse(form.txtDecks.Text);
			for (byte c = UnoColor.RED; c <= UnoColor.BLUE; c++)
            {
				Pile.cards[c, 0] = decks;
                for (byte n = 1; n <= 9; n++)
                    Pile.cards[c, n] = 2 * decks;
                if (form.mnuDos.Checked)
                {
                    Pile.cards[c, 10] = 2 * decks;
                    Pile.cards[c, UnoNumber.NUMBER] = 2 * decks;
                }
                for (byte n = UnoNumber.SKIP; n <= UnoNumber.DRAW_2; n++)
                    Pile.cards[c, n] = 2 * decks;
                if (form.mnuDaWah.Checked)
                {
                    Pile.cards[c, UnoNumber.DISCARD_ALL] = decks;
                    // Pile.uno[c, UnoNumber.TRADE_HANDS] = decks;
                }
                if (form.mnuRygbBlank.Checked)
                {
                    Pile.cards[c, UnoNumber.BLANK] = 1 * decks;
                }
			}
            if (form.mnuDos.Checked)
            {
                Pile.cards[UnoColor.BLACK, 2] = 4 * decks;
            }
            if (form.mnuRygbBlank.Checked && form.mnuMagentaBlank.Checked)
            {
                Pile.cards[UnoColor.MAGENTA, UnoNumber.BLANK] = 1 * decks;
            }
            if (form.mnuWildDownpourDraw.Checked && form.mnuBlackBlank.Checked)
            {
                Pile.cards[UnoColor.BLACK, UnoNumber.BLANK] = 2 * decks;
            }
            Pile.cards[UnoColor.BLACK, UnoNumber.WILD] = 4 * decks;
            if (form.mnuWildDownpourDraw.Checked)
            {
                Pile.cards[UnoColor.BLACK, UnoNumber.WILD_DOWNPOUR_DRAW_1] = decks;
                Pile.cards[UnoColor.BLACK, UnoNumber.WILD_DOWNPOUR_DRAW_2] = decks;
                Pile.cards[UnoColor.BLACK, UnoNumber.WILD_DOWNPOUR_DRAW_4] = decks;
            }
            Pile.cards[UnoColor.BLACK, UnoNumber.WILD_DRAW_4] = 4 * decks;
            if (form.mnuDaWah.Checked)
            {
                Pile.cards[UnoColor.BLACK, UnoNumber.WILD_DOWNPOUR_DRAW_1] = 2 * decks;
            }
            if (form.mnuWildHitfire.Checked)
            {
                Pile.cards[UnoColor.BLACK, UnoNumber.WILD_HITFIRE] = 2 * decks;
            }
			lblPile.Text = GetPile().Length + "";
            for (byte p = 0; p < 4; p++)
                Players[p] = new Cards();
            AddLabel(lblMovingCards);
            lblMovingCards[0].BringToFront();
		}

        private void Uno_Click(object sender, EventArgs e) {
            MovingCard.quickly = true;
        }

        private void Uno_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("退出游戏?", "退出", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
            {
                FormClosing -= new FormClosingEventHandler(Uno_FormClosing);
                Application.Exit();
            }
            else e.Cancel = true;
        }

		private void Uno_Load(object sender, EventArgs e) {
            for (byte p = 0; p < 4; p++)
                if (lblPlayers[p].Visible)
                {
                    MovingCard.player = p;
                    goto play;
                }
            FormClosing -= new FormClosingEventHandler(Uno_FormClosing);
            Application.Exit();
            return;
play:
            lblMovingCards[0].BringToFront();
            timPileToPlayers.Enabled = true;
		}

		private void Uno_Resize(object sender, EventArgs e) {
			ResizeForm();
		}

        private void GameOver()
        {
            timPileToCenter.Enabled = false;
            timPileToPlayers.Enabled = false;
            timTurn.Enabled = false;
            timPlayersToCenter.Enabled = false;
            timWatch.Enabled = false;
            FormClosing -= new FormClosingEventHandler(Uno_FormClosing);
            if (form.mnuOneWinner.Checked)
            {
                List<Label> label = new List<Label>();
                for (byte p = 1; p <= 3; p++)
                {
                    int playerPos = label.ToArray().Length;
                    if (mnuByColor.Checked)
                        for (byte color = 0; color <= UnoColor.MAX_VALUE; color++)
                            for (byte number = 0; number <= UnoNumber.MAX_VALUE; number++)
                                for (int c = 1; c <= Players[p].cards[color, number]; c++)
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
                                for (int c = 1; c <= Players[p].cards[color, number]; c++)
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
                                if (width / 2 - UnoSize.WIDTH * (label.ToArray().Length - playerPos) / 2 >= 0)
                                    label[c].Left = width / 2 - UnoSize.WIDTH * (label.ToArray().Length - playerPos) / 2 + UnoSize.WIDTH * (c - playerPos);
                                else
                                    label[c].Left = width / (label.ToArray().Length - playerPos) * (c - playerPos);
                                break;
                        }
                    }
                }
                string msg = (gameOver == 0 ? "你" : "Player" + gameOver) + " 赢了!\n" +
                    "\n" +
                    (form.mnuWatch.Checked ? "游戏时长\t" + lblWatch.Text + "\n" +
                    "\n" : "") +
                    "玩家\t得分";
                for (byte p = 0; p <= 3; p++)
                    msg += "\n" + (p == 0 ? "你" : "Player" + p) + "\t" + GetPointsByPlayer(p);
                if (MessageBox.Show(msg, "结束", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Retry) goto retry;
            }
            else
            {
                if (gameOver != 0)
                {
                    List<Label> label = new List<Label>();
                    if (mnuByColor.Checked)
                        for (byte color = 0; color <= UnoColor.MAX_VALUE; color++)
                            for (byte number = 0; number <= UnoNumber.MAX_VALUE; number++)
                                for (int c = 1; c <= Players[gameOver].cards[color, number]; c++)
                                {
                                    int length = label.ToArray().Length;
                                    AddLabel(label);
                                    label[length].BackColor = GetColor(color);
                                    if (new Regex("^(1|3)$").IsMatch(gameOver + "")) label[length].Left = lblPlayers[gameOver].Left;
                                    label[length].Text = GetNumber(number);
                                    if (gameOver == 2) label[length].Top = lblPlayers[2].Top;
                                }
                    else
                        for (byte number = 0; number <= UnoNumber.MAX_VALUE; number++)
                            for (byte color = 0; color <= UnoColor.MAX_VALUE; color++)
                                for (int c = 1; c <= Players[gameOver].cards[color, number]; c++)
                                {
                                    int length = label.ToArray().Length;
                                    AddLabel(label);
                                    label[length].BackColor = GetColor(color);
                                    if (new Regex("^(1|3)$").IsMatch(gameOver + "")) label[length].Left = lblPlayers[gameOver].Left;
                                    label[length].Text = GetNumber(number);
                                    if (gameOver == 2) label[length].Top = lblPlayers[2].Top;
                                }
                    for (int c = 0; c < label.ToArray().Length; c++)
                    {
                        switch (gameOver)
                        {
                            case 1:
                            case 3:
                                if (height / 2 - UnoSize.HEIGHT * label.ToArray().Length / 2 >= 0)
                                    label[c].Top = height / 2 - UnoSize.HEIGHT * label.ToArray().Length / 2 + UnoSize.HEIGHT * c;
                                else
                                    label[c].Top = height / label.ToArray().Length * c;
                                break;
                            case 2:
                                if (width / 2 - UnoSize.WIDTH * label.ToArray().Length / 2 >= 0)
                                    label[c].Left = width / 2 - UnoSize.WIDTH * label.ToArray().Length / 2 + UnoSize.WIDTH * c;
                                else
                                    label[c].Left = width / label.ToArray().Length * c;
                                break;
                        }
                    }
                }
                if (MessageBox.Show((gameOver == 0 ? "你" : "Player" + gameOver) + " 输了!\n" +
                    (form.mnuWatch.Checked ? "\n" +
                    "游戏时长\t" + lblWatch.Text : ""), "结束", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Retry) goto retry;
            }
            Application.Exit();
            return;
        retry:
            Application.Restart();
        }

		Color GetColor(byte id) {
            return id switch
            {
                0 => Color.Red,
                1 => Color.Yellow,
                2 => Color.Lime,
                3 => Color.Blue,
                4 => Color.Magenta,
                5 => Color.Black,
                6 => Color.Cyan,
                _ => Color.White,
            };
        }

		byte GetColorId(Color color) {
            return color.Name switch
            {
                "Red" => 0,
                "Yellow" => 1,
                "Lime" => 2,
                "Blue" => 3,
                "Magenta" => 4,
                "Black" => 5,
                "Cyan" => 6,
                _ => 7,
            };
        }

        string GetColorName(byte id) {
            return id switch
            {
                0 => "红",
                1 => "黃",
                2 => "绿",
                3 => "蓝",
                4 => "紫",
                5 => "黑",
                6 => "靑",
                _ => "白",
            };
        }

        int GetColorQuantityByNumber(byte player, byte number)
        {
            int i = 0;
            for (byte b = 0; b <= UnoColor.MAX_VALUE; b++)
                i += Math.Sign(Players[player].cards[b, number]);
            return i;
        }

        int GetDbp()
        {
            return (form.mnuDrawBeforePlaying.Checked ? 1 : 0) + (form.mnuDrawTwoBeforePlaying.Checked ? 2 : 0);
        }

        string GetNumber(byte id) {
            return id switch
            {
                UnoNumber.SKIP => UnoNumberName.SKIP,
                UnoNumber.REVERSE => UnoNumberName.REVERSE,
                UnoNumber.DRAW_2 => UnoNumberName.DRAW_2,
                UnoNumber.DISCARD_ALL => UnoNumberName.DISCARD_ALL,
                UnoNumber.TRADE_HANDS => UnoNumberName.TRADE_HANDS,
                UnoNumber.NUMBER => UnoNumberName.NUMBER,
                UnoNumber.BLANK => form.txtBlankText.Text,
                UnoNumber.WILD => UnoNumberName.WILD,
                UnoNumber.WILD_DOWNPOUR_DRAW_1 => UnoNumberName.WILD_DOWNPOUR_DRAW_1,
                UnoNumber.WILD_DOWNPOUR_DRAW_2 => UnoNumberName.WILD_DOWNPOUR_DRAW_2,
                UnoNumber.WILD_DOWNPOUR_DRAW_4 => UnoNumberName.WILD_DOWNPOUR_DRAW_4,
                UnoNumber.WILD_DRAW_4 => UnoNumberName.WILD_DRAW_4,
                UnoNumber.WILD_HITFIRE => UnoNumberName.WILD_HITFIRE,
                UnoNumber.NULL => UnoNumberName.NULL,
                _ => id + "",
            };
        }

		byte GetNumberId(String number) {
            if (number == form.txtBlankText.Text)
            {
                return UnoNumber.BLANK;
            }
            return number switch
            {
                UnoNumberName.SKIP => UnoNumber.SKIP,
                UnoNumberName.REVERSE => UnoNumber.REVERSE,
                UnoNumberName.DRAW_2 => UnoNumber.DRAW_2,
                UnoNumberName.DISCARD_ALL => UnoNumber.DISCARD_ALL,
                UnoNumberName.TRADE_HANDS => UnoNumber.TRADE_HANDS,
                UnoNumberName.NUMBER => UnoNumber.NUMBER,
                UnoNumberName.WILD => UnoNumber.WILD,
                UnoNumberName.WILD_DOWNPOUR_DRAW_1 => UnoNumber.WILD_DOWNPOUR_DRAW_1,
                UnoNumberName.WILD_DOWNPOUR_DRAW_2 => UnoNumber.WILD_DOWNPOUR_DRAW_2,
                UnoNumberName.WILD_DOWNPOUR_DRAW_4 => UnoNumber.WILD_DOWNPOUR_DRAW_4,
                UnoNumberName.WILD_DRAW_4 => UnoNumber.WILD_DRAW_4,
                UnoNumberName.WILD_HITFIRE => UnoNumber.WILD_HITFIRE,
                UnoNumberName.NULL => UnoNumber.NULL,
                _ => byte.Parse(number),
            };
        }

        string GetNumberName(byte number)
        {
            if (number == UnoNumber.BLANK)
            {
                return "Blank";
            }
            return number switch
            {
                UnoNumber.SKIP => "Skip",
                UnoNumber.REVERSE => "Reverse",
                UnoNumber.DRAW_2 => "Draw Two",
                UnoNumber.DISCARD_ALL => "Discard All",
                UnoNumber.TRADE_HANDS => "Trade Hands",
                UnoNumber.NUMBER => "Wild Number",
                UnoNumber.WILD => "Wild",
                UnoNumber.WILD_DOWNPOUR_DRAW_1 => "Wild Downpour Draw One",
                UnoNumber.WILD_DOWNPOUR_DRAW_2 => "Wild Downpour Draw Two",
                UnoNumber.WILD_DOWNPOUR_DRAW_4 => "Wild Downpour Draw Four",
                UnoNumber.WILD_DRAW_4 => "Wild Draw Four",
                UnoNumber.WILD_HITFIRE => "Wild Hit-fire",
                UnoNumber.NULL => "Null",
                _ => number + "",
            };
        }

		bool IsNumeric(string s) {
			return new Regex(@"^[+-]?\d+[.\d]*$").IsMatch(s);
		}

        int GetOnplayersCards(byte color, byte number)
        {
            int cards = 0;
            foreach (Cards p in Players) cards += p.cards[color, number];
            return cards;
        }

        Card[] GetPile()
        {
            List<Card> cards = new List<Card>();
            for (byte color = 0; color <= UnoColor.MAX_VALUE; color++)
                for (byte number = 0; number <= UnoNumber.MAX_VALUE; number++)
                    for (int c = 1; c <= Pile.cards[color, number]; c++)
                    {
                        Card card = new Card
                        {
                            color = color,
                            number = number
                        };
                        cards.Add(card);
                    };
            return cards.ToArray();
        }

        int GetPoints(byte number)
        {
            switch (number)
            {
                default:
                    return number;
                case UnoNumber.SKIP:
                case UnoNumber.REVERSE:
                case UnoNumber.DRAW_2:
                case UnoNumber.BLANK:
                case UnoNumber.WILD_DOWNPOUR_DRAW_1:
                case UnoNumber.WILD_DOWNPOUR_DRAW_2:
                    return 20;
                case UnoNumber.DISCARD_ALL:
                case UnoNumber.TRADE_HANDS:
                    return 30;
                case UnoNumber.NUMBER:
                    return 40;
                case UnoNumber.WILD:
                case UnoNumber.WILD_DOWNPOUR_DRAW_4:
                case UnoNumber.WILD_DRAW_4:
                case UnoNumber.WILD_HITFIRE:
                    return 50;
            }
        }

        int GetPointsByColor(byte player, byte color)
        {
            int points = 0;
            for (byte n = 0; n < UnoNumber.MAX_VALUE; n++)
            {
                points += GetPoints(n) * Players[player].cards[color, n];
            }
            return points;
        }

        int GetPointsByPlayer(byte player)
        {
            int points = 0;
            Card[] cards = PlayersCards(player);
            foreach (Card c in cards)
                points += GetPoints(c.number);
            return points;
        }

        int GetQuantityByColor(byte player, byte color) {
            int quantity = 0;
            for (byte q = 0; q <= UnoNumber.MAX_VALUE; q++)
                quantity += Players[player].cards[color, q];
            return quantity;
        }

        int GetQuantityByNumber(byte player, byte number) {
            int q = 0, quantity = 0;
            for (byte c = 0; c <= UnoColor.MAX_VALUE; c++)
            {
                if (Players[player].cards[c, number] > 0)
                {
                    for (byte n = 0; n <= UnoNumber.MAX_VALUE; n++)
                    {
                        q += Players[player].cards[c, n];
                    }
                    if (q > quantity)
                    {
                        quantity = q;
                    }
                }
            }
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
            return number switch
            {
                UnoNumberName.SKIP => usages[0],
                UnoNumberName.REVERSE => usages[1],
                UnoNumberName.DRAW_2 => usages[2].Replace("%AMOUNT%", "2"),
                UnoNumberName.DISCARD_ALL => "允许玩家打出所有相同颜色的牌.",
                UnoNumberName.TRADE_HANDS => "所有玩家互相交換手牌.",
                UnoNumberName.NUMBER => "任意指定数字.",
                UnoNumberName.WILD => "任意指定颜色.",
                UnoNumberName.WILD_DOWNPOUR_DRAW_1 => usage.Replace("%AMOUNT%", "1"),
                UnoNumberName.WILD_DOWNPOUR_DRAW_2 => usage.Replace("%AMOUNT%", "2"),
                UnoNumberName.WILD_DOWNPOUR_DRAW_4 => usage.Replace("%AMOUNT%", "4"),
                UnoNumberName.WILD_DRAW_4 => "任意指定颜色幷且下家罚抽 4 张牌.",
                UnoNumberName.WILD_HITFIRE => "任意指定颜色幷且下家罚抽牌盒中的所有牌.",
                _ => "普通的 " + number + " 号牌.",
            };
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

        private void LblPile_Click(object sender, EventArgs e)
        {
            if (form.mnuCanShowCards.Checked)
            {
                int i = 0;
                string cards = "";
                for (byte c = 0; c <= UnoColor.MAX_VALUE; c++)
                    for (byte n = 0; n <= UnoNumber.MAX_VALUE; n++)
                    {
                        string q = Pile.cards[c, n] + "";
                        if (q != "0")
                        {
                            if (q == "1")
                            {
                                q = "";
                            }
                            cards += "[" + GetColorName(c) + GetNumber(n) + "]" + q;
                            i++;
                            if (i % Math.Floor((double)width / UnoSize.WIDTH) == 0)
                            {
                                cards += "\n";
                            }
                        }
                    }
                Action(0, cards);
            }
            else if (btnDraw.Visible)
                BtnDraw_Click(sender, e);
        }

        private void LblPlayers_Click(object sender, EventArgs e)
        {
            byte p = (byte)((Label)sender).Tag;
            lblPlayers[p].AutoSize = form.mnuCanShowCards.Checked;
            if (form.mnuCanShowCards.Checked)
            {
                int i = 0;
                string cards = "";
                for (byte c = 0; c <= UnoColor.MAX_VALUE; c++)
                    for (byte n = 0; n <= UnoNumber.MAX_VALUE; n++)
                    {
                        string q = Players[p].cards[c, n] + "";
                        if (q != "0")
                        {
                            if (q == "1")
                            {
                                q = "";
                            }
                            cards += "[" + GetColorName(c) + GetNumber(n) + "]" + q;
                            i++;
                            if (i % Math.Floor((double)width / UnoSize.WIDTH) == 0)
                            {
                                cards += "\n";
                            }
                        }
                    }
                Action(p, cards);
            }
        }

        void CheckPile()
        {
            if (int.Parse(lblPile.Text) <= 0)
            {
                bool btc = timPileToCenter.Enabled, btp = timPileToPlayers.Enabled;
                if (btc) timPileToCenter.Enabled = false;
                if (btp) timPileToPlayers.Enabled = false;
                if (MessageBox.Show("牌已用尽, 使用废牌堆的牌?", "牌已用尽", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    RefillPile();
                    if (lblPile.Text == "0")
                    {
                        DialogResult result = MessageBox.Show("废牌堆无牌!", "牌已用尽", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
                        switch (result)
                        {
                            default:
                                FormClosing -= new FormClosingEventHandler(Uno_FormClosing);
                                Application.Exit();
                                break;
                            case DialogResult.Retry:
                                FormClosing -= new FormClosingEventHandler(Uno_FormClosing);
                                Application.Restart();
                                break;
                            case DialogResult.Ignore:
                                lblDraw.Text = "0";
                                MovingCard.dbp = 0;
                                MovingCard.downpour = -1;
                                break;
                        }
                    }
                    timPileToCenter.Enabled = btc;
                    timPileToPlayers.Enabled = btp;
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

        private void LblCounts_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < 4; i++)
                {
                    lblCounts[i].Left = lblPlayers[i].Left + lblPlayers[i].Width / 2 - lblCounts[i].Width / 2;
                }
            }
            catch
            {

            }
        }

        private void LblCounts_TextChanged(object sender, EventArgs e) {
            byte index = (byte)((Label)sender).Tag;
            if (form.mnuCanShowCards.Checked)
            {
                ShowCards();
            }
            if (MovingCard.playing)
            {
                byte winners = 0;
                if (int.Parse(lblCounts[index].Text) <= 0)
                {
                    lblPlayers[index].Visible = false;
                    lblCounts[index].Visible = false;
                }
                byte player;
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
                gameOver = player;
                if (form.mnuOneWinner.Checked)
                {
                    byte currentNumber = GetNumberId(lblCards[1].Text);
                    switch (currentNumber)
                    {
                        case UnoNumber.DRAW_2:
                        case UnoNumber.WILD_DRAW_4:
                            Draw(NextPlayer(index));
                            break;
                        case UnoNumber.WILD_DOWNPOUR_DRAW_1:
                        case UnoNumber.WILD_DOWNPOUR_DRAW_2:
                        case UnoNumber.WILD_DOWNPOUR_DRAW_4:
                            break;
                        default:
                            GameOver();
                            break;
                    }
                }
                else
                {
                    GameOver();
                }
            }
		}

        private void LblWatch_Resize(object sender, EventArgs e)
        {
            lblWatch.Left = width - lblWatch.Width;
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
                        Players[p].cards[c, n] = int.Parse(numbers[n]);
                }
                lblCounts[p].Text = PlayersCards(p).Length + "";
            }
            Sort();
            colors = keys[1].Split(char.Parse("C"));
            for (byte c = 0; c <= UnoColor.MAX_VALUE; c++)
            {
                string[] numbers = colors[c].Split(char.Parse("N"));
                for (byte n = 0; n <= UnoNumber.MAX_VALUE; n++)
                    Pile.cards[c, n] = int.Parse(numbers[n]);
            }
            lblPile.Text = GetPile().Length - 1 + "";
            BackColor = GetColor(byte.Parse(keys[2]));
            foreach (Label card in lblCards)
            {
                card.BackColor = BackColor;
                card.Text = GetNumber(byte.Parse(keys[3]));
            }
            skip = int.Parse(keys[4]);
            string[] sks = keys[5].Split(char.Parse("P"));
            for (byte sk = 0; sk <= 3; sk++)
                skips[sk] = int.Parse(sks[sk]);
            reverse = bool.Parse(keys[6]);
            lblDraw.Text = keys[7];
            gametime = int.Parse(keys[8]);
        }

        private void MnuAppearance_Click(object sender, EventArgs e)
        {
            ((ToolStripMenuItem)sender).Checked = !((ToolStripMenuItem)sender).Checked;
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
                        case "/auto":
                            isAutomatic = !isAutomatic;
                            pnlCtrl.Visible = false;
                            mnuSaveGame.Enabled = false;
                            Action(0, "已切換託管");
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
                                    Players[player].cards[color, number] = 0;
                            if (player == 0) Sort();
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
                        case "/decks":
                            if (data.Length > 1)
                                if (IsNumeric(data[1]))
                                {
                                    form.txtDecks.Text = data[1];
                                    Action(0, "已准备 " + data[1] + " 副牌");
                                }
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
                                    Card[] pile = GetPile();
                                    Card rndCard = pile[(int)(pile.Length * Rnd())];
                                    Pile.cards[rndCard.color, rndCard.number]--;
                                    lblPile.Text = pile.Length - 1 + "";
                                    Players[int.Parse(data[1])].cards[rndCard.color, rndCard.number]++;
                                    lblCounts[int.Parse(data[1])].Text = PlayersCards(byte.Parse(data[1])).Length + "";
                                }
                                if (data[1] == "0")
                                    Sort();
                            }
                            break;
                        case "/give":
                            if (data.Length < 4) return;
                            count = 1;
                            if (data.Length >= 5) count = Int32.Parse(data[4]);
                            Players[Byte.Parse(data[1])].cards[Byte.Parse(data[2]), Byte.Parse(data[3])] += count;
                            if (data[1] == "0")
                                Sort();
                            lblCounts[int.Parse(data[1])].Text = PlayersCards(byte.Parse(data[1])).Length + "";
                            Action(0, "已将 [" + GetColorName(byte.Parse(data[2])) + " " + GetNumber(byte.Parse(data[3])) + "] * " + count + " 给予 Player" + data[1]);
                            break;
                        case "/help":
                        case "/?":
                            string page = "1";
                            if (data.Length > 1) page = data[1];
                            string help = 
                                "/clear [byte player] [byte color] [byte number]\n" +
                                "/currard <byte color> [byte number]\n" +
                                "/decks <int decks>\n" +
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
                                        FormClosing -= new FormClosingEventHandler(Uno_FormClosing);
                                        Application.Exit();
                                        break;
                                    case "restart":
                                        FormClosing -= new FormClosingEventHandler(Uno_FormClosing);
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
                                    PlayersTurn(MovingCard.player = NextPlayer(MovingCard.player), true, GetDbp());
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
                "上一页\tNumpad/\n" +
                "下一页\tNumpad*\n" +
                "光标移动\t←↑→↓\n" +
                "选定卡牌\tSpace\n" +
                "选择颜色\tNumpad0~3\n" +
                "切換颜色\tNumpad.\n" +
                "出牌\tEnter\n" +
                "摸牌\tNumpad+\n" +
                "喊 UNO!\tNumpad-\n" +
                "聊天\tT\n" +
                "指令\t/\n", "按键说明", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MnuNew_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("放弃本局?", "新游戏", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Cancel)
                return;
            FormClosing -= new FormClosingEventHandler(Uno_FormClosing);
            Application.Restart();
        }

        private void MnuRank_Click(object sender, EventArgs e)
        {
            mnuByColor.Checked = false;
            mnuByNumber.Checked = false;
            ((ToolStripMenuItem)sender).Checked = true;
            Sort();
        }

        private void MnuSaveGame_Click(object sender, EventArgs e)
        {
            Interaction.SaveSetting("UNO", "GAME", "SAVE", SaveGame());
            Action(0, "储存成功");
        }

        private void MnuScrollBar_Click(object sender, EventArgs e)
        {
            ((ToolStripMenuItem)sender).Checked = !((ToolStripMenuItem)sender).Checked;
            Sort();
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
			if (player == 0 && !isAutomatic) {
                List<Card> discardAll = new List<Card>(), number = new List<Card>();
                foreach (CheckBox c in chkPlayer)
                {
                    if (c.Checked)
                    {
                        Card card = new Card
                        {
                            color = GetColorId(c.BackColor),
                            number = GetNumberId(c.Text)
                        };
                        switch (c.Text)
                        {
                            default:
                                cards.Add(card);
                                break;
                            case UnoNumberName.DISCARD_ALL:
                                discardAll.Add(card);
                                break;
                            case UnoNumberName.NUMBER:
                                number.Add(card);
                                break;
                        }
                    }
                }
                discardAll.AddRange(cards);
                cards = discardAll;
                cards.AddRange(number);
                if (mnuRadioBox.Checked)
                {
                    if (!CanPlay(cards, cards.First().color)) return;
                }
                else if (!CanPlay(cards, GetColorId(btnPlay.BackColor))) return;
				for (int c = 0; c < cards.ToArray().Length; c++) {
					Players[0].cards[cards[c].color, cards[c].number]--;
				}
				for (int c = 0; c < chkPlayer.ToArray().Length; c++) if (chkPlayer[c].Checked) {
					btnPlay.Visible = true;
					goto play;
				}
				btnPlay.Visible = false;
play:   		Sort();
                btnPlay.Visible = false;
            }
            else if (player == 0 && isAutomatic)
            {
                Card[] ais = Ai(player);
                foreach (Card card in ais)
                {
                    cards.Add(card);
                    Players[0].cards[card.color, card.number]--;
                }
                Sort();
            }
            else
            {
                Card[] ais = Ai(player);
				foreach (Card card in ais) cards.Add(card);
				for (int c = 0; c < cards.ToArray().Length; c++) Players[player].cards[cards[c].color, cards[c].number]--;
			}
            if (player == 0 && !isAutomatic)
                Action(0, form.mnuUno.Checked && rdoUno.Checked ? "UNO!" : "出牌");
            else
                Action(player, form.mnuUno.Checked && (player != 0 || isAutomatic) && PlayersCards(player).Length == 1 ? "UNO!" : "出牌");
			PlayersTurn(player, false);
            if (cards.Count > 0)
            {
                MovingCard.player = player; MovingCard.progress = 0;
                if (player == 0 && !isAutomatic)
                {
                    if (int.Parse(lblCounts[0].Text) - cards.Count > 0 || form.mnuOneLoser.Checked)
                    {
                        if (mnuRadioBox.Checked)
                        {
                            FrmColor c = new FrmColor();
                            int colors = 0;
                            if (cards.Last().color == UnoColor.BLACK)
                            {
                                foreach (ToolStripMenuItem menu in c.mnuColor.Items)
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
                                        c.mnuColor.Items[card.color].Enabled = true;
                                        c.mnuColor.Items[card.color].BackColor = GetColor(card.color);
                                    }
                                foreach (ToolStripMenuItem menu in c.mnuColor.Items) if (menu.Enabled) colors++;
                            }
                            if (colors > 1)
                            {
                                c.ShowDialog();
                                MovingCard.color = byte.Parse(c.Tag + "");
                            }
                            else if (cards[0].color != UnoColor.MAGENTA)
                            {
                                MovingCard.color = cards.First().color;
                            }
                            else
                            {
                                MovingCard.color = GetColorId(BackColor);
                            }
                        }
                        else
                            MovingCard.color = GetColorId(btnPlay.BackColor);
                    }
                    else
                    {
                        MovingCard.color = GetColorId(BackColor);
                    }
                }
                MovingCard.number = cards[0].number;
                RemoveLabel(lblMovingCards, pnlMovingCards.Controls);
                AddLabel(lblMovingCards, cards.Count, pnlMovingCards.Controls);
                for (int c = 1; c < lblMovingCards.Count; c++)
                {
                    lblMovingCards[c].BackColor = GetColor(cards[c - 1].color == UnoColor.BLACK ? MovingCard.color : cards[c - 1].color);
                    lblMovingCards[c].Text = GetNumber(cards[c - 1].number);
                }
                if (lblMovingCards.Count > swpcw / 2)
                {
                    for (int c = 1; c < lblMovingCards.Count; c++)
                    {
                        lblMovingCards[c].Location = new Point(UnoSize.WIDTH * ((c - 1) % (swpcw / 2)),
                            UnoSize.HEIGHT * (int)Math.Floor((decimal)(c - 1) / swpcw * 2));
                    }
                    pnlMovingCards.Size = new Size(UnoSize.WIDTH * swpcw / 2,
                        UnoSize.HEIGHT * (int)Math.Floor((decimal)(lblMovingCards.Count - 1) / swpcw * 2));
                }
                else
                {
                    for (int c = 1; c < lblMovingCards.Count; c++)
                    {
                        lblMovingCards[c].Location = new Point(UnoSize.WIDTH * (c - 1), 0);
                    }
                    pnlMovingCards.Size = new Size(UnoSize.WIDTH * (lblMovingCards.Count - 1), UnoSize.HEIGHT);
                }
                timPlayersToCenter.Enabled = true;
                pnlMovingCards.Location = new Point(-pnlMovingCards.Width, -pnlMovingCards.Height);
                pnlMovingCards.BringToFront();
            }
            else if (player > 0 || isAutomatic) {
                Draw(player);
            }
		}

		Card[] PlayersCards(byte player) {
			List<Card> cards = new List<Card>();
			for (byte color = 0; color <= UnoColor.MAX_VALUE; color++)
				for (byte number = 0; number <= UnoNumber.MAX_VALUE; number++)
                    for (int c = 1; c <= Players[player].cards[color, number]; c++) {
                        Card card = new Card
                        {
                            color = color,
                            number = number
                        };
                        cards.Add(card);
					};
			return cards.ToArray();
		}

		void PlayersTurn(byte player, bool turn = true, int dbp = 0) {
            if (int.Parse(lblCounts[player].Text) <= dbp * 7)
            {
                dbp = (int.Parse(lblCounts[player].Text) - 1) / 7;
            }
            if (turn)
            {
                if (timTurn.Tag.ToString().Split(char.Parse(","))[0] == "4")
                {
                    timTurn.Tag = player + "," + dbp;
                    timTurn.Enabled = true;
                    return;
                }
                timTurn.Tag = "4,";
                if (Downpour.count > 0)
                {
                    DownpourDraw(Downpour.player, Downpour.count);
                    Downpour.count = 0;
                    return;
                }
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
                    PlayersTurn(NextPlayer(player), true, GetDbp());
                    return;
                }
            }
            if (dbp > 0 && int.Parse(lblCounts[player].Text) > 7)
            {
                Action(player, "摸牌");
                MovingCard.dbp = dbp; MovingCard.player = player; MovingCard.progress = 0; MovingCard.quickly = false;
                CheckPile();
                if (int.Parse(lblPile.Text) > 0)
                {
                    lblMovingCards[0].BringToFront();
                    timPileToPlayers.Enabled = true;
                }
            } 
            else if (player == 0 && !isAutomatic && (skip <= 0 || skips[0] <= 0))
            {
                pnlCtrl.Visible = turn;
                mnuSaveGame.Enabled = turn;
                if (turn)
                {
                    MovingCard.dbp = 0;
                    Action(0, "你的回合");
                    btnChallenge.Visible = form.mnuChallenges.Checked && lblCards[1].Text == UnoNumberName.WILD_DRAW_4 && int.Parse(lblDraw.Text) >= 4;
                    rdoUno.Checked = false;
                    if (!form.Visible)
                        chkPlayer[0].Focus();
                }
            }
            else if (turn)
                Play(player);
        }

        private void ShowCards()
        {
            for (byte p = 1; p <= 3; p++)
            {
                string cards = "";
                for (byte c = 0; c <= UnoColor.MAX_VALUE; c++)
                    for (byte n = 0; n <= UnoNumber.MAX_VALUE; n++)
                    {
                        string q = Players[p].cards[c, n] + "";
                        if (q != "0")
                        {
                            if (q == "1")
                            {
                                q = "";
                            }
                            cards += "[" + GetColorName(c) + GetNumber(n) + "]" + q;
                            if (p == 1 || p == 3)
                            {
                                cards += "\n";
                            }
                        }
                    }
                lblPlayers[p].Text = cards;
                switch (p)
                {
                    case 1: lblPlayers[p].Top = height / 2 - lblPlayers[p].Height / 2; break;
                    case 2: lblPlayers[p].Left = width / 2 - lblPlayers[p].Width / 2; break;
                    case 3: lblPlayers[p].Location = new Point(width - lblPlayers[p].Width, height / 2 - lblPlayers[p].Height / 2); break;
                }
            }
        }

        void Sort() {
			RemoveChkPlayer();
			int i = 0;
            if (mnuByColor.Checked)
			    for (byte color = 0; color <= UnoColor.MAX_VALUE; color++)
                    for (byte number = 0; number <= UnoNumber.MAX_VALUE; number++)
                        for (int c = 1; c <= Players[0].cards[color, number]; c++) {
				            AddChkPlayer();
				            chkPlayer[i].BackColor = GetColor(color);
                            chkPlayer[i].Text = GetNumber(number);
                            SetUsage(chkPlayer[i]);
				            i++;
			            }
            else
                for (byte number = 0; number <= UnoNumber.MAX_VALUE; number++)
                    for (byte color = 0; color <= UnoColor.MAX_VALUE; color++)
                        for (int c = 1; c <= Players[0].cards[color, number]; c++)
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
            if (width <= this.width)
            {
                pnlPlayer.Left = this.width / 2 - width / 2;
                if (mnuAppearance.Checked)
                {
                    for (i = 0; i < chkPlayer.ToArray().Length; i++)
                        chkPlayer[i].Location = new Point(UnoSize.WIDTH * i, chkPlayer[i].Checked ? 0 : UnoSize.HEIGHT / 8);
                }
                else
                {
                    for (i = 0; i < chkPlayer.ToArray().Length; i++)
                        chkPlayer[i].Left = UnoSize.WIDTH * i;
                }
            }
            else if (width > this.width * 2 && mnuScrollBar.Checked)
            {
                if (mnuAppearance.Checked)
                {
                    for (i = 0; i < chkPlayer.ToArray().Length; i++)
                        chkPlayer[i].Location = new Point(UnoSize.WIDTH * i, chkPlayer[i].Checked ? 0 : UnoSize.HEIGHT / 8);
                }
                else
                {
                    for (i = 0; i < chkPlayer.ToArray().Length; i++)
                        chkPlayer[i].Left = UnoSize.WIDTH * i;
                }
                hPlayer.Maximum = width - this.width;
                hPlayer.Visible = true;
                if (pnlPlayer.Left > 0 || pnlPlayer.Left + pnlPlayer.Width < width)
                {
                    HPlayer_Scroll(hPlayer, new ScrollEventArgs(new ScrollEventType(), hPlayer.Value));
                }
            }
            else
            {
                pnlPlayer.Left = 0;
                if (mnuAppearance.Checked)
                {
                    for (i = 0; i < chkPlayer.ToArray().Length; i++)
                    {
                        chkPlayer[i].Location = new Point(this.width / chkPlayer.ToArray().Length * i, chkPlayer[i].Checked ? 0 : UnoSize.HEIGHT / 8);
                    }
                }
                else
                {
                    for (i = 0; i < chkPlayer.ToArray().Length; i++)
                        chkPlayer[i].Left = this.width / chkPlayer.ToArray().Length * i;
                }
            }
            if (!form.Visible && chkPlayer.Count > 0)
            {
                chkPlayer[0].Focus();
            }
		}

        void RefillPile()
        {
            int decks = int.Parse(form.txtDecks.Text);
            for (byte c = UnoColor.RED; c <= UnoColor.BLUE; c++)
            {
                Pile.cards[c, 0] = decks - GetOnplayersCards(c, 0);
                for (byte n = 1; n <= 9; n++)
                    Pile.cards[c, n] = 2 * decks - GetOnplayersCards(c, n);
                if (form.mnuDos.Checked)
                {
                    Pile.cards[c, 10] = 2 * decks - GetOnplayersCards(c, 10);
                    Pile.cards[c, UnoNumber.NUMBER] = 2 * decks - GetOnplayersCards(c, UnoNumber.NUMBER);
                }
                for (byte n = UnoNumber.SKIP; n <= UnoNumber.DRAW_2; n++)
                    Pile.cards[c, n] = 2 * decks - GetOnplayersCards(c, n);
                if (form.mnuDaWah.Checked)
                {
                    Pile.cards[c, UnoNumber.DISCARD_ALL] = decks - GetOnplayersCards(c, UnoNumber.DISCARD_ALL);
                    // Pile.uno[c, UnoNumber.TRADE_HANDS] = decks - getOnplayersCards(c, UnoNumber.TRADE_HANDS);
                }
                if (form.mnuRygbBlank.Checked)
                    Pile.cards[c, UnoNumber.BLANK] = decks - GetOnplayersCards(c, UnoNumber.BLANK);
            }
            if (form.mnuDos.Checked)
            {
                Pile.cards[UnoColor.BLACK, 2] = 4 * decks - GetOnplayersCards(UnoColor.BLACK, 2);
            }
            if (form.mnuRygbBlank.Checked && form.mnuMagentaBlank.Checked)
                Pile.cards[UnoColor.MAGENTA, UnoNumber.BLANK] = 1 * decks - GetOnplayersCards(UnoColor.MAGENTA, UnoNumber.BLANK);
            if (form.mnuWildDownpourDraw.Checked && form.mnuBlackBlank.Checked)
                Pile.cards[UnoColor.BLACK, UnoNumber.BLANK] = 2 * decks - GetOnplayersCards(UnoColor.BLACK, UnoNumber.BLANK);
            Pile.cards[UnoColor.BLACK, UnoNumber.WILD] = 4 * decks - GetOnplayersCards(UnoColor.BLACK, UnoNumber.WILD);
            if (form.mnuWildDownpourDraw.Checked)
            {
                Pile.cards[UnoColor.BLACK, UnoNumber.WILD_DOWNPOUR_DRAW_1] = decks - GetOnplayersCards(UnoColor.BLACK, UnoNumber.WILD_DOWNPOUR_DRAW_1);
                Pile.cards[UnoColor.BLACK, UnoNumber.WILD_DOWNPOUR_DRAW_2] = decks - GetOnplayersCards(UnoColor.BLACK, UnoNumber.WILD_DOWNPOUR_DRAW_2);
                Pile.cards[UnoColor.BLACK, UnoNumber.WILD_DOWNPOUR_DRAW_4] = decks - GetOnplayersCards(UnoColor.BLACK, UnoNumber.WILD_DOWNPOUR_DRAW_4);
            }
            Pile.cards[UnoColor.BLACK, UnoNumber.WILD_DRAW_4] = 4 * decks - GetOnplayersCards(UnoColor.BLACK, UnoNumber.WILD_DRAW_4);
            if (form.mnuDaWah.Checked)
            {
                Pile.cards[UnoColor.BLACK, UnoNumber.WILD_DOWNPOUR_DRAW_1] = 2 * decks - GetOnplayersCards(UnoColor.BLACK, UnoNumber.WILD_DOWNPOUR_DRAW_1);
            }
            if (form.mnuWildHitfire.Checked)
            {
                Pile.cards[UnoColor.BLACK, UnoNumber.WILD_HITFIRE] = 2 * decks - GetOnplayersCards(UnoColor.BLACK, UnoNumber.WILD_HITFIRE);
            }
            lblPile.Text = GetPile().Length + "";
        }

		void RemoveChkPlayer() {
			while (chkPlayer.ToArray().Length > 0) {
				pnlPlayer.Controls.Remove(chkPlayer[0]);
                chkPlayer.RemoveAt(0);
			}
		}

		void RemoveLabel(List<Label> label, Control.ControlCollection controls = null)
        {
            if (controls == null)
                controls = Controls;
			while (label.Count > 1)
            {
				controls.Remove(label[1]);
				label.RemoveAt(1);
			}
		}

		void ResizeForm() {
            width = ClientRectangle.Width;
            height = Height - mnuGame.Height;
            lblPlayers[0].Location = new Point(width / 2 - UnoSize.WIDTH / 2, height - UnoSize.HEIGHT);
            lblPlayers[1].Location = new Point(0, height / 2 - UnoSize.HEIGHT / 2 + mnuGame.Height / 2);
            lblPlayers[2].Location = new Point(width / 2 - UnoSize.WIDTH / 2, mnuGame.Height);
            lblPlayers[3].Location = new Point(width - UnoSize.WIDTH, height / 2 - UnoSize.HEIGHT / 2 + mnuGame.Height / 2);
			for (byte i = 0; i < 4; i++)
                lblCounts[i].Location = new Point(lblPlayers[i].Left + lblPlayers[i].Width / 2 - lblCounts[i].Width / 2, lblPlayers[i].Top - lblCounts[i].Height);
            lblCounts[2].Top = lblPlayers[2].Top + UnoSize.HEIGHT;
            lblWatch.Left = width - lblWatch.Width;
			pnlCtrl.Location = new Point(0, lblPlayers[0].Top - pnlCtrl.Height);
            btnPlay.Width = width / 5;
            btnDraw.Left = width / 5;
            btnDraw.Width = width / 5;
            btnChallenge.Left = width / 5 * 2;
            btnChallenge.Width = width / 5;
            rdoUno.Left = width / 5 * 4;
            rdoUno.Width = width / 5;
            pnlCtrl.Width = width;
			lblCounts[0].Top = pnlCtrl.Top - lblCounts[0].Height;
            pnlPlayer.Top = pnlCtrl.Top + pnlCtrl.Height;
            hPlayer.Top = pnlPlayer.Top;
            hPlayer.Width = width;
            swpcw = width / UnoSize.WIDTH;
            if (MovingCard.playing)
                Sort();
            lblCards[0].Location = new Point(width / 2 - UnoSize.WIDTH / 2, height / 2 - UnoSize.HEIGHT / 2);
		}

		double Rnd() {
            if (form.mnuSeed.Checked)
                return 0;
            if (form.mnuGuid.Checked)
            {
                return new Random(Guid.NewGuid().GetHashCode()).NextDouble();
            }
            else if (form.mnuRNGCryptoServiceProvider.Checked)
            {
                byte[] b = new byte[1];
                new RNGCryptoServiceProvider().GetBytes(b);
                return b[0] / 256d;
            }
            else if (form.mnuMembership.Checked)
            {
                string s = "!#$%&()*+-./:;<=>?@[]^_{|}";
                return (double)s.IndexOf(char.Parse(Membership.GeneratePassword(1, 1))) / s.Length;
            }
            else
            {
                return new Random().NextDouble();
            }
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
                        s += Players[p].cards[c, n] + "N";
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
                    s += Pile.cards[c, n] + "N";
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
            s += form.SaveRules();
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

		private void TimPileToCenter_Tick(object sender, EventArgs e) {
            lblMovingCards[0].Location = new Point(lblCards[0].Left / distance * MovingCard.progress, lblCards[0].Top / distance * MovingCard.progress + mnuGame.Height);
			if (lblMovingCards[0].Left >= lblCards[0].Left && lblMovingCards[0].Top >= lblCards[0].Top)
            {
                MovingCard.progress = 0;
                Card[] pile = GetPile();
				Card rndCard = pile[(int)(pile.Length * Rnd())];
				Pile.cards[rndCard.color, rndCard.number]--;
				lblPile.Text = pile.Length - 1 + "";
				AddLabel(lblCards);
				lblCards[1].BackColor = GetColor(rndCard.color);
				lblCards[1].BringToFront();
				lblCards[1].Font = new Font(lblCards[1].Font.FontFamily, 42);
				lblCards[1].ForeColor = Color.White;
				lblCards[1].Location = new Point(lblCards[0].Left, lblCards[0].Top);
				lblCards[1].Text = GetNumber(rndCard.number);
				lblCards[1].TextAlign = ContentAlignment.MiddleCenter;
                SetUsage(lblCards[1]);
                if (new Regex("^(Magenta|Black)$").IsMatch(lblCards[1].BackColor.Name))
                {
                    MovingCard.progress = 0;
                    return;
                }
                BackColor = lblCards[1].BackColor;
                timPileToCenter.Enabled = false;
                lblMovingCards[0].Location = new Point(-UnoSize.WIDTH, -UnoSize.HEIGHT);
                MovingCard.playing = true;
                reverse = Math.Floor(2 * Rnd()) == 0;
                if (form.keys.Length == 0)
                {
                    byte nextPlayer = NextPlayer((byte)(4 * Rnd()));
                    if (lblCards[1].Text == form.txtBlankText.Text)
                    {
                        if (form.mnuSkipPlayers.Checked)
                            skip = int.Parse(form.txtBlankSkip.Text);
                        else
                            skips[nextPlayer] = int.Parse(form.txtBlankSkip.Text);
                        lblDraw.Text = form.txtBlankDraw.Text;
                    }
                    else
                    {
                        switch (lblCards[1].Text)
                        {
                            case UnoNumberName.SKIP:
                                if (form.mnuSkipPlayers.Checked)
                                    skip = 1;
                                else
                                    skips[nextPlayer] = 1;
                                break;
                            case UnoNumberName.DRAW_2:
                                lblDraw.Text = "2";
                                break;
                        }
                    }
                    PlayersTurn(nextPlayer, true, GetDbp());
                }
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
                        FormClosing -= new FormClosingEventHandler(Uno_FormClosing);
                        Application.Restart();
                    }
                }
                if (form.mnuWatch.Checked)
                {
                    timWatch.Enabled = true;
                    lblWatch.Visible = true;
                }
            }
            else MovingCard.progress++;
		}

		private void TimPileToPlayers_Tick(object sender, EventArgs e) {
            if (MovingCard.quickly)
                lblMovingCards[0].Location = lblPlayers[MovingCard.player].Location;
            else
                lblMovingCards[0].Location = new Point(lblPlayers[MovingCard.player].Left / distance * MovingCard.progress, lblPlayers[MovingCard.player].Top / distance * MovingCard.progress + mnuGame.Height);
            if (lblMovingCards[0].Left >= lblPlayers[MovingCard.player].Left && lblMovingCards[0].Top >= lblPlayers[MovingCard.player].Top)
            {
                Card[] pile = GetPile();
                Card rndCard = new Card();
                if (MovingCard.player == 0 || form.mnuBeginner.Checked)
                {
                    rndCard = pile[(int)(pile.Length * Rnd())];
                }
                else
                {
                    if (MovingCard.playing)
                    {
                        byte backColor = GetColorId(BackColor), backNumber = GetNumberId(lblCards[1].Text);
                        for (byte b = 0; b < UnoNumber.MAX_VALUE; b++)
                        {
                            byte n = mvcList[b];
                            if (Pile.cards[UnoColor.BLACK, n] > 0)
                            {
                                rndCard.color = UnoColor.BLACK; rndCard.number = n;
                                goto end_rnd_card;
                            }
                        }
                        if (Pile.cards[UnoColor.MAGENTA, UnoNumber.BLANK] > 0)
                        {
                            rndCard.color = UnoColor.MAGENTA; rndCard.number = UnoNumber.BLANK;
                            goto end_rnd_card;
                        }
                        for (byte c = 0; c < UnoColor.BLACK; c++)
                        {
                            if (Pile.cards[c, backNumber] > 0)
                            {
                                rndCard.color = c; rndCard.number = backNumber;
                                goto end_rnd_card;
                            }
                        }
                        for (byte b = 0; b < UnoNumber.MAX_VALUE; b++)
                        {
                            byte n = mvcList[b];
                            if (Pile.cards[backColor, n] > 0)
                            {
                                rndCard.color = backColor; rndCard.number = n;
                                goto end_rnd_card;
                            }
                        }
                    }
                    for (byte b = 0; b < UnoNumber.MAX_VALUE; b++)
                    {
                        byte n = mvcList[b];
                        for (byte c = 0; c < UnoColor.MAX_VALUE; c++)
                        {
                            if (Pile.cards[c, n] > 0)
                            {
                                rndCard.color = c; rndCard.number = n;
                                goto end_rnd_card;
                            }
                        }
                    }
                }
end_rnd_card:
				Pile.cards[rndCard.color, rndCard.number]--;
				lblPile.Text = pile.Length - 1 + "";
                Players[MovingCard.player].cards[rndCard.color, rndCard.number]++;
                lblCounts[MovingCard.player].Text = PlayersCards(MovingCard.player).Length + "";
                if (MovingCard.player == 0 && !MovingCard.quickly)
                    Sort();
                if (MovingCard.playing)
                {
                    bool drawAll = false;
                    lblMovingCards[0].Location = new Point(-UnoSize.WIDTH, -UnoSize.HEIGHT);
                    timPileToPlayers.Enabled = false;
draw:
                    if (int.Parse(lblDraw.Text) > 0 && MovingCard.dbp <= 0 && MovingCard.downpour <= -1)
                    {
                        lblDraw.Text = int.Parse(lblDraw.Text) - 1 + "";
                        if (lblDraw.Text == "0")
                            drawAll = true;
                        if (int.Parse(lblDraw.Text) > 0)
                            CheckPile();
                        if (int.Parse(lblDraw.Text) <= 0)
                            goto draw;
                        else
                            timPileToPlayers.Enabled = true;
                    }
                    else if (int.Parse(lblDraw.Text) <= 0 && MovingCard.dbp <= 0 && MovingCard.downpour <= -1)
                    {
                        if (gameOver < 4 && MovingCard.downpour <= -1)
                        {
                            GameOver();
                        }
                        if (!form.mnuDrawTilCanPlay.Checked && MovingCard.dbp <= 0 && MovingCard.downpour <= -1)
                            MovingCard.drew = !MovingCard.unoDraw;
                        if (MovingCard.player == 0 && MovingCard.quickly)
                            Sort();
                        if (MovingCard.unoDraw)
                        {
                            MovingCard.unoDraw = false;
                            PlayersTurn(NextPlayer(MovingCard.player), true, GetDbp());
                        }
                        else if (!form.mnuDrawAndPlay.Checked || !form.mnuDrawAllAndPlay.Checked && drawAll)
                        {
                            MovingCard.drew = false;
                            PlayersTurn(NextPlayer(MovingCard.player), true, GetDbp());
                        }
                        else
                            PlayersTurn(MovingCard.player);
                    }
                    else if (MovingCard.dbp > 0)
                    {
                        MovingCard.dbp--;
                        if (MovingCard.dbp > 0)
                            CheckPile();
                        if (MovingCard.dbp <= 0)
                            PlayersTurn(MovingCard.player);
                        else
                            timPileToPlayers.Enabled = true;
                    }
                    else if (MovingCard.downpour > -1)
                    {
                        MovingCard.downpour--;
                        if (MovingCard.player == 0 && MovingCard.quickly && MovingCard.downpour < 3)
                            Sort();
                        if (MovingCard.downpour > 0)
                            CheckPile();
                        if (MovingCard.downpour <= 0)
                        {
                            if (gameOver < 4)
                            {
                                GameOver();
                            }
                            MovingCard.downpour = -1;
                            if (NextPlayer(MovingCard.player) == Downpour.player)
                                PlayersTurn(NextPlayer(NextPlayer(MovingCard.player)), true, GetDbp());
                            else
                                PlayersTurn(NextPlayer(MovingCard.player), true, GetDbp());
                        }
                        else
                        {
                            MovingCard.player = NextPlayer(MovingCard.player);
                            if (MovingCard.player == Downpour.player)
                                MovingCard.player = NextPlayer(MovingCard.player);
                            timPileToPlayers.Enabled = true;
                        }
                    }
                }
                else
                {
                    if (MovingCard.player == NextPlayer(0, true)) 
                        if (int.Parse(lblCounts[NextPlayer(0, true)].Text) >= int.Parse(form.txtDealt.Text))
                        {
						    timPileToPlayers.Enabled = false;
                            if (MovingCard.quickly)
                                Sort();
						    timPileToCenter.Enabled = true;
					    }
                    MovingCard.player = NextPlayer(MovingCard.player);
				}
                MovingCard.progress = 0;
            }
            else MovingCard.progress++;
		}

        private void TimChallenge_Tick(object sender, EventArgs e)
        {
            timChallenge.Enabled = false;
            Draw(0);
        }

        private void TimPlayersToCenter_Tick(object sender, EventArgs e)
        {
            switch (MovingCard.player)
            {
				case 0:
                    pnlMovingCards.Location = new Point(width / 2 - pnlMovingCards.Width / 2,
                        height - (lblPlayers[0].Top - lblCards[0].Top) / distance * MovingCard.progress - pnlMovingCards.Height / 2 - UnoSize.HEIGHT / 2);
					if (MovingCard.progress >= distance)
                        goto arrived;
					break;
				case 1:
                    pnlMovingCards.Location = new Point(lblCards[0].Left / distance * MovingCard.progress - pnlMovingCards.Width / 2 + UnoSize.WIDTH / 2,
                        height / 2 - pnlMovingCards.Height / 2);
					if (MovingCard.progress >= distance)
                        goto arrived;
					break;
				case 2:
                    pnlMovingCards.Location = new Point(width / 2 - pnlMovingCards.Width / 2,
                        lblCards[0].Top / distance * MovingCard.progress - pnlMovingCards.Height / 2 + UnoSize.HEIGHT / 2);
					if (MovingCard.progress >= distance)
                        goto arrived;
					break;
				case 3:
                    pnlMovingCards.Location = new Point(width - (lblPlayers[MovingCard.player].Left - lblCards[0].Left) / distance * MovingCard.progress - pnlMovingCards.Width / 2 - UnoSize.WIDTH / 2,
                        height / 2 - pnlMovingCards.Height / 2);
					if (MovingCard.progress >= distance)
                        goto arrived;
					break;
			}
            MovingCard.progress++;
			return;
arrived:
            pnlMovingCards.Location = new Point(width / 2 - pnlMovingCards.Width / 2, height / 2 - pnlMovingCards.Height / 2);
            MovingCard.progress = 0;
			RemoveLabel(lblCards);
			AddLabel(lblCards, lblMovingCards.ToArray().Length - 1);
			for (int c = 1; c < lblCards.ToArray().Length; c++)
            {
				lblCards[c].BackColor = lblMovingCards[c].BackColor;
                lblCards[c].Text = lblMovingCards[c].Text;
				lblCards[c].BringToFront();
                lblCards[c].Location = new Point(lblMovingCards[c].Left + pnlMovingCards.Left, lblMovingCards[c].Top + pnlMovingCards.Top);
                SetUsage(lblCards[c]);
			}
			RemoveLabel(lblMovingCards, pnlMovingCards.Controls);
            pnlMovingCards.Location = new Point(-pnlMovingCards.Width, -pnlMovingCards.Height);
            BackColor = GetColor(MovingCard.color);
            lblCounts[MovingCard.player].Text = PlayersCards(MovingCard.player).Length + "";
            int downpour = 0;
            bool reversed = false;
            byte ons = 0;
            foreach (Label p in lblPlayers)
                if (p.Visible)
                    ons++;
            if (ons == 2 && lblPlayers[MovingCard.player].Visible)
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
                        else skips[NextPlayer(MovingCard.player)]++;
                        break;
                    case UnoNumberName.REVERSE:
                        break;
                    case UnoNumberName.DRAW_2:
                        AddDraw(2);
                        break;
                    case UnoNumberName.TRADE_HANDS:
                        break;
                    case UnoNumberName.WILD_DOWNPOUR_DRAW_1:
                        downpour += form.mnuDoubleDraw.Checked ? 2 : 1;
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
                        lblDraw.Text = lblPile.Text;
                        break;
                    default:
                        if (lblCards[1].Text == form.txtBlankText.Text)
                        {
                            if (form.mnuSkipPlayers.Checked)
                                skip += int.Parse(form.txtBlankSkip.Text);
                            else
                                skips[NextPlayer(MovingCard.player)] += int.Parse(form.txtBlankSkip.Text);
                            AddDraw(int.Parse(form.txtBlankDraw.Text));
                        }
                        break;
                }
            }
            timPlayersToCenter.Enabled = false;
            if (gameOver < 4)
            {
                if (downpour > 0)
                {
                    DownpourDraw(MovingCard.player, downpour);
                }
                return;
            }
            if (form.mnuUno.Checked && MovingCard.player == 0 && !isAutomatic && (PlayersCards(0).Length == 1 && !rdoUno.Checked || PlayersCards(0).Length != 1 && rdoUno.Checked))
            {
                Action(0, "UNO? +2");
				AddDraw(2);
				PlayersTurn(0, false);
				rdoUno.Checked = false;
                MovingCard.unoDraw = true;
			}
            if (!form.mnuDrawTilCanPlay.Checked)
                MovingCard.drew = false;
            if (MovingCard.unoDraw)
                timUno.Enabled = true;
            else if (reversed)
                PlayersTurn(MovingCard.player, true, GetDbp());
            else if (downpour > 0)
            {
                Downpour.player = MovingCard.player;
                Downpour.count = downpour;
                PlayersTurn(MovingCard.player, true, GetDbp());
            }
            else
                PlayersTurn(MovingCard.player = NextPlayer(MovingCard.player), true, GetDbp());
		}

        private void TimTurn_Tick(object sender, EventArgs e)
        {
            timTurn.Enabled = false;
            string[] tag = timTurn.Tag.ToString().Split(char.Parse(","));
            PlayersTurn(byte.Parse(tag[0]), true, int.Parse(tag[1]));
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
