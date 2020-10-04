using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Security;
using System.Windows.Forms;

namespace Uno
{
    public partial class Uno : Form {
        public class Card {
            public byte color = UnoColor.MAX_VALUE;
            public byte number = UnoNumber.MAX_VALUE;

            public Card()
            {
                color = UnoColor.MAX_VALUE;
                number = UnoNumber.MAX_VALUE;
            }

            public Card(byte color, byte number)
            {
                this.color = color;
                this.number = number;
            }
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
			public static bool drew = false, isPlaying = false, quickly = false, unoDraw = false;
			public static int dbp = 0, downpour = -1, progress = 0;
		}

        public class PicPlayer
        {
            public static bool isSelecting = false;
            public static bool[] checkeds;
            public static float cardWidth = 0;
            public static int count = 0, pointing = -1, selected = -1, selecting = -1;
            public static List<Card> picPlayer = new List<Card>();
            
        }

        public class Record
        {
            public static bool reverse;
            public static byte firstGettingCard, firstTurn;
            public static List<byte> colors = new List<byte>(), tradeHands = new List<byte>();
            public static List<bool> unos = new List<bool>();
            public static List<Card> pile = new List<Card>();
            public static List<Card[]>[] players = new List<Card[]>[4] { new List<Card[]>(), new List<Card[]>(), new List<Card[]>(), new List<Card[]>() };
        }

        public class ResourceUnoSize
        {
            public const int WIDTH = 120;
            public const int HEIGHT = 160;
        }

        public class TradingHands
        {
            public static byte player1 = 4, player2 = 4;
            public static List<int> x, y;
        }

        public class UnoSize
        {
            public const int WIDTH = 90;
            public const int HEIGHT = 120;
        }

        public class UnoColor {
            public const byte RED = 0,
                YELLOW = 1,
                GREEN = 2,
                CYAN = 3,
                BLUE = 4,
                MAGENTA = 5,
                BLACK = 6,
                WHITE = 7,
                MAX_VALUE = 7;
        }

        public class UnoNumber {
            public const byte SKIP = 11,
                SKIP_EVERYONE = 12,
                REVERSE = 13,
                DRAW_1 = 14,
                DRAW_2 = 15,
                DRAW_5 = 16,
                DISCARD_ALL = 17,
                TRADE_HANDS = 18,
                NUMBER = 19,
                BLANK = 20,
                WILD = 21,
                WILD_DOWNPOUR_DRAW_1 = 22,
                WILD_DOWNPOUR_DRAW_2 = 23,
                WILD_DOWNPOUR_DRAW_4 = 24,
                WILD_DRAW_4 = 25,
                WILD_DRAW_COLOR = 26,
                WILD_HITFIRE = 27,
                NULL = 28,
                MAX_VALUE = 28;
        }

        public class UnoNumberName
        {
            public const string SKIP = "⊘";
            public const string SKIP_EVERYONE = "⟳";
            public const string REVERSE = "⇅";
            public const string DRAW_1 = "+1";
            public const string DRAW_2 = "+2";
            public const string DRAW_5 = "+5";
            public const string DISCARD_ALL = "×";
            public const string TRADE_HANDS = "<>";
            public const string NUMBER = "#";
            public const string WILD = "∷";
            public const string WILD_DOWNPOUR_DRAW_1 = "!1";
            public const string WILD_DOWNPOUR_DRAW_2 = "!2";
            public const string WILD_DOWNPOUR_DRAW_4 = "!4";
            public const string WILD_DRAW_4 = "+4";
            public const string WILD_DRAW_COLOR = "↑";
            public const string WILD_HITFIRE = "+?";
            public const string NULL = "";
        }
        
        private bool canPlay = false, hasCheat = false, isAutomatic = false, isFair = false, isPlayer0sTurn = false, isSelectingCards = false, reverse = false;
        byte gameOver = 4;
        readonly Cards pile = new Cards();
        readonly Cards[] players = new Cards[4];
        Graphics gpsPlayer;
        Image imgPlayer;
        readonly Image imgUno;
        private int distance = 20, draw = 0, drawColor = 0, gametime = 0, height = 0, pointing = -1, skip = 0, swpcw = 0, width = 0;
        readonly int[] skips = new int[4];
        readonly Label[] lblCounts = new Label[4];
        public Label[] lblPlayers = new Label[4];
        readonly List<CheckBox> chkPlayer = new List<CheckBox>();
        private readonly List<Label> lblCards = new List<Label>();
        private readonly List<Label> lblMovingCards = new List<Label>();
        readonly Options options;
        readonly string[] PLAYER_NAMES;

        readonly byte[]
            colorList = new byte[4]
            {
                UnoColor.RED, UnoColor.YELLOW, UnoColor.GREEN, UnoColor.BLUE
            }
            ,
            mvcList = new byte[UnoNumber.MAX_VALUE]
            {
                UnoNumber.WILD_HITFIRE,
                UnoNumber.WILD_DRAW_COLOR,
                UnoNumber.WILD_DRAW_4,
                UnoNumber.WILD_DOWNPOUR_DRAW_4,
                UnoNumber.WILD_DOWNPOUR_DRAW_2,
                UnoNumber.WILD_DOWNPOUR_DRAW_1,
                UnoNumber.WILD,
                UnoNumber.BLANK,
                UnoNumber.DISCARD_ALL,
                UnoNumber.NUMBER,
                UnoNumber.DRAW_5,
                UnoNumber.DRAW_2,
                UnoNumber.DRAW_1,
                UnoNumber.SKIP_EVERYONE,
                UnoNumber.SKIP,
                UnoNumber.REVERSE,
                0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
                UnoNumber.TRADE_HANDS
            },
            playlist = new byte[UnoNumber.MAX_VALUE]
            {
                UnoNumber.SKIP_EVERYONE,
                UnoNumber.DISCARD_ALL,
                UnoNumber.SKIP,
                UnoNumber.REVERSE,
                UnoNumber.DRAW_1,
                UnoNumber.DRAW_2,
                UnoNumber.DRAW_5,
                10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0,
                UnoNumber.NUMBER,
                UnoNumber.BLANK,
                UnoNumber.TRADE_HANDS,
                UnoNumber.WILD,
                UnoNumber.WILD_DOWNPOUR_DRAW_1,
                UnoNumber.WILD_DOWNPOUR_DRAW_2,
                UnoNumber.WILD_DOWNPOUR_DRAW_4,
                UnoNumber.WILD_DRAW_4,
                UnoNumber.WILD_DRAW_COLOR,
                UnoNumber.WILD_HITFIRE
            },
            wildPlaylist = new byte[UnoNumber.MAX_VALUE]
            {
                UnoNumber.WILD,
                2,
                UnoNumber.BLANK,
                UnoNumber.WILD_DOWNPOUR_DRAW_1,
                UnoNumber.WILD_DOWNPOUR_DRAW_2,
                UnoNumber.WILD_DOWNPOUR_DRAW_4,
                UnoNumber.DRAW_2,
                UnoNumber.WILD_DRAW_4,
                UnoNumber.WILD_DRAW_COLOR,
                UnoNumber.WILD_HITFIRE,
                UnoNumber.REVERSE,
                UnoNumber.SKIP_EVERYONE,
                UnoNumber.DISCARD_ALL,
                UnoNumber.SKIP,
                UnoNumber.DRAW_1,
                UnoNumber.DRAW_5,
                10, 9, 8, 7, 6, 5, 4, 3, 1, 0,
                UnoNumber.NUMBER,
                UnoNumber.TRADE_HANDS
            };

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
                chkPlayer[length].BackColor = Color.White;
                chkPlayer[length].BackgroundImageLayout = ImageLayout.Stretch;
				chkPlayer[length].BringToFront();
				chkPlayer[length].CheckedChanged += ChkPlayer_CheckedChanged;
                if (isFair || options.mnuCheat.Checked)
                    chkPlayer[length].ContextMenuStrip = mnuCheating;
                chkPlayer[length].Enter += ChkPlayer_Enter;
                chkPlayer[length].FlatStyle = FlatStyle.Flat;
                chkPlayer[length].ForeColor = Color.White;
                chkPlayer[length].Font = new Font(new FontFamily("MS Gothic"), 42);
                chkPlayer[length].KeyDown += Control_KeyDown;
                chkPlayer[length].MouseDown += ChkPlayer_MouseDown;
                chkPlayer[length].MouseLeave += ChkPlayer_MouseLeave;
                chkPlayer[length].MouseMove += ChkPlayer_MouseMove;
                chkPlayer[length].MouseUp += ChkPlayer_MouseUp;
                chkPlayer[length].MouseWheel += ChkPlayer_MouseWheel;
                chkPlayer[length].Size = new Size(UnoSize.WIDTH, UnoSize.HEIGHT);
                chkPlayer[length].Tag = length;
				chkPlayer[length].TextAlign = ContentAlignment.MiddleCenter;
            }
		}

        void AddDraw(int count)
        {
            SetDraw(draw + (options.mnuDoubleDraw.Checked ? 2 : 1) * count);
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
                label[length].BackColor = Color.White;
                label[length].BackgroundImageLayout = ImageLayout.Stretch;
                label[length].BorderStyle = BorderStyle.FixedSingle;
                label[length].BringToFront();
                label[length].ForeColor = Color.White;
                label[length].Font = new Font(new FontFamily("MS Gothic"), 42);
                label[length].Location = new Point(-UnoSize.WIDTH, -UnoSize.HEIGHT);
                label[length].MouseLeave += Label_MouseLeave;
                label[length].Size = new Size(UnoSize.WIDTH, UnoSize.HEIGHT);
                label[length].Text = UnoNumberName.NULL;
                label[length].TextAlign = ContentAlignment.MiddleCenter;
            }
		}

        Card[] Ai(byte player, bool skipFirstCheating = false) {
            if (player > 0 && options.mnuCheater.Checked
                || player == 0 && isFair)
            {
                if (!skipFirstCheating && options.mnuPairs.Checked)
                    CheatPairs(player);
            }
            bool gbp = int.Parse(lblCounts[player].Text) > 7 && GetDbp() > 0;
            byte backColor = GetColorId(BackColor), backNumber = GetNumberId(lblCards[1].Text);
            Card bestCard = new Card();
            List<Card> cards = new List<Card>();
            if (!options.mnuStacking.Checked && draw > 0)
            {
                return cards.ToArray();
            }
            int quantityColor, quantityNumber = 0;
            if (backNumber == UnoNumber.NUMBER)
            {
                if (!gbp)
                    quantityColor = GetQuantityByColor(player, backColor);
                else
                    quantityColor = GetColorQuantity(player, backColor);
                for (byte b = 10; b >= 0 && b < byte.MaxValue; b--)
                {
                    int i;
                    if (!gbp)
                        i = GetQuantityByNumber(player, b);
                    else
                        i = GetNumberQuantity(player, b);
                    if (i > quantityNumber)
                    {
                        quantityNumber = i;
                        backNumber = b;
                    }
                }
            }
            else
            {
                if (!gbp)
                    quantityColor = GetQuantityByColor(player, backColor);
                else
                {
                    quantityColor = GetColorQuantity(player, backColor);
                    int q = GetColorQuantity(player, UnoColor.BLACK);
                    if (q > quantityColor)
                        quantityColor = q;
                }
                if (backNumber < UnoNumber.WILD)
                {
                    if (!gbp)
                        quantityNumber = GetQuantityByNumber(player, backNumber);
                    else
                        quantityNumber = GetNumberQuantity(player, backNumber);
                }
                if (gbp && quantityColor == quantityNumber && quantityColor > 0)
                {
                    quantityColor = GetQuantityByColor(player, backColor);
                    quantityNumber = GetQuantityByNumber(player, backNumber);
                }
            }
            if (!options.mnuPlayOrDrawAll.Checked
                && lblCards[1].Text == UnoNumberName.DRAW_1 && draw >= 1)
            {
                if (quantityNumber > 0)
                    bestCard.number = UnoNumber.DRAW_1;
                else if (players[player].cards[UnoColor.BLACK, UnoNumber.DRAW_2] > 0)
                {
                    bestCard.color = UnoColor.BLACK;
                    bestCard.number = UnoNumber.DRAW_2;
                }
                else if (players[player].cards[backColor, UnoNumber.DRAW_2] > 0)
                    bestCard.number = UnoNumber.DRAW_2;
                else if (players[player].cards[backColor, UnoNumber.DRAW_5] > 0)
                    bestCard.number = UnoNumber.DRAW_5;
                else if (players[player].cards[UnoColor.BLACK, UnoNumber.WILD_DRAW_4] > 0)
                {
                    bestCard.color = UnoColor.BLACK;
                    bestCard.number = UnoNumber.WILD_DRAW_4;
                }
                else if (options.mnuWildPunch.Checked
                    && players[player].cards[UnoColor.BLACK, UnoNumber.REVERSE] > 0)
                {
                    bestCard.color = UnoColor.BLACK;
                    bestCard.number = UnoNumber.REVERSE;
                }
            }
            else if (!options.mnuPlayOrDrawAll.Checked
                && lblCards[1].Text == UnoNumberName.DRAW_2 && draw >= 2)
            {
                if (players[player].cards[UnoColor.BLACK, UnoNumber.DRAW_2] > 0)
                {
                    bestCard.color = UnoColor.BLACK;
                    bestCard.number = UnoNumber.DRAW_2;
                }
                else if (quantityNumber > 0)
                    bestCard.number = UnoNumber.DRAW_2;
                else if (players[player].cards[backColor, UnoNumber.DRAW_5] > 0)
                    bestCard.number = UnoNumber.DRAW_5;
                else if (players[player].cards[UnoColor.BLACK, UnoNumber.WILD_DRAW_4] > 0)
                {
                    bestCard.color = UnoColor.BLACK;
                    bestCard.number = UnoNumber.WILD_DRAW_4;
                }
                else if (options.mnuWildPunch.Checked
                    && players[player].cards[UnoColor.BLACK, UnoNumber.REVERSE] > 0)
                {
                    bestCard.color = UnoColor.BLACK;
                    bestCard.number = UnoNumber.REVERSE;
                }
            }
            else if (!options.mnuPlayOrDrawAll.Checked
                && lblCards[1].Text == UnoNumberName.DRAW_5 && draw >= 5)
            {
                if (quantityNumber > 0)
                    bestCard.number = UnoNumber.DRAW_5;
                else if (players[player].cards[UnoColor.BLACK, UnoNumber.WILD_DRAW_4] > 0)
                {
                    bestCard.color = UnoColor.BLACK;
                    bestCard.number = UnoNumber.WILD_DRAW_4;
                }
                else if (options.mnuWildPunch.Checked
                    && players[player].cards[UnoColor.BLACK, UnoNumber.REVERSE] > 0)
                {
                    bestCard.color = UnoColor.BLACK;
                    bestCard.number = UnoNumber.REVERSE;
                }
            }
            else if (!options.mnuPlayOrDrawAll.Checked
                && lblCards[1].Text == UnoNumberName.WILD_DRAW_4 && draw >= 4)
            {
                if (players[player].cards[UnoColor.BLACK, UnoNumber.WILD_DRAW_4] > 0)
                {
                    bestCard.color = UnoColor.BLACK;
                    bestCard.number = UnoNumber.WILD_DRAW_4;
                }
                else if (options.mnuWildPunch.Checked
                    && players[player].cards[UnoColor.BLACK, UnoNumber.REVERSE] > 0)
                {
                    bestCard.color = UnoColor.BLACK;
                    bestCard.number = UnoNumber.REVERSE;
                }
            }
            else if (!options.mnuPlayOrDrawAll.Checked
                && lblCards[1].Text == UnoNumberName.WILD_DRAW_COLOR && drawColor > 0)
            {
                if (players[player].cards[UnoColor.BLACK, UnoNumber.WILD_DRAW_COLOR] > 0)
                {
                    bestCard.color = UnoColor.BLACK;
                    bestCard.number = UnoNumber.WILD_DRAW_COLOR;
                }
                else if (options.mnuWildPunch.Checked
                    && players[player].cards[UnoColor.BLACK, UnoNumber.REVERSE] > 0)
                {
                    bestCard.color = UnoColor.BLACK;
                    bestCard.number = UnoNumber.REVERSE;
                }
            }
            else if (!options.mnuPlayOrDrawAll.Checked
                && lblCards[1].Text == UnoNumberName.WILD_HITFIRE && draw > 0)
            {
                if (players[player].cards[UnoColor.BLACK, UnoNumber.WILD_HITFIRE] > 0)
                {
                    bestCard.color = UnoColor.BLACK;
                    bestCard.number = UnoNumber.WILD_HITFIRE;
                }
                else if (options.mnuWildPunch.Checked
                    && players[player].cards[UnoColor.BLACK, UnoNumber.REVERSE] > 0)
                {
                    bestCard.color = UnoColor.BLACK;
                    bestCard.number = UnoNumber.REVERSE;
                }
            }
            else if (options.mnuWildPunch.Checked && !options.mnuPlayOrDrawAll.Checked
                && lblCards[1].Text == UnoNumberName.REVERSE && lblCards.Last().BackColor == Color.Black && draw + drawColor > 0)
            {
                if (players[player].cards[UnoColor.BLACK, UnoNumber.REVERSE] > 0)
                {
                    bestCard.color = UnoColor.BLACK;
                    bestCard.number = UnoNumber.REVERSE;
                }
            }
            else if (quantityColor == 0 && quantityNumber == 0)
            {

                if (backNumber <= 10 && GetNumberQuantity(player, UnoNumber.NUMBER) > 0)
                {
                    bestCard.number = UnoNumber.NUMBER;
                }
                else if (players[player].cards[UnoColor.MAGENTA, UnoNumber.BLANK] > 0)
                {
                    bestCard.color = UnoColor.MAGENTA;
                    bestCard.number = UnoNumber.BLANK;
                }
                else
                    for ( byte n = 0; n <= UnoNumber.MAX_VALUE; n++)
                        if (players[player].cards[UnoColor.BLACK, n] > 0)
                        {
                            bestCard.color = UnoColor.BLACK;
                            bestCard.number = n;
                            break;
                        }
            }
            else if (quantityColor >= quantityNumber)
            {
                if (gbp)
                {
                    int mq = 0;
                    for (byte b = 0; b < UnoNumber.WILD; b++)
                    {
                        byte n = playlist[b];
                        if (players[player].cards[backColor, n] > 0)
                        {
                            int q = GetNumberQuantity(player, n);
                            if (q > mq)
                            {
                                mq = q;
                                bestCard.color = backColor;
                                bestCard.number = n;
                            }
                        }
                    }
                    for (byte b = 0; b < UnoNumber.MAX_VALUE; b++)
                    {
                        byte n = wildPlaylist[b];
                        if (players[player].cards[UnoColor.BLACK, n] > 0)
                        {
                            int q = GetNumberQuantity(player, n);
                            if (q > mq)
                            {
                                mq = q;
                                bestCard.color = UnoColor.BLACK;
                                bestCard.number = n;
                            }
                        }
                    }
                }
                else if (!options.mnuPairs.Checked)
                {
                    int mq = 0;
                    for (byte b = 0; b < UnoNumber.WILD; b++)
                    {
                        byte n = playlist[b];
                        if (players[player].cards[backColor, n] > 0)
                        {
                            int q = GetNumberQuantity(player, n);
                            if (q > mq)
                            {
                                mq = q;
                                bestCard.color = backColor;
                                bestCard.number = n;
                            }
                        }
                    }
                }
                else
                {
                    int fq = int.MaxValue;
                    for (byte b = 0; b < UnoNumber.WILD; b++)
                    {
                        byte n = playlist[b];
                        if (players[player].cards[backColor, n] <= 0)
                        {
                            continue;
                        }
                        int q = GetColorQuantityByNumber(player, n);
                        if (0 < q && q < fq)
                        {
                            fq = q;
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
            if (players[player].cards[UnoColor.BLACK, bestCard.number] <= 0)
            {
                if (options.mnuPairs.Checked)
                {
                    int mp = 0, mq = 0;
                    foreach (byte color in colorList)
                        if (players[player].cards[color, bestCard.number] > 0)
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
                            for (byte c = 1; c <= players[player].cards[color, card.number]; c++)
                                cards.Add(card);
                        }
                    if (bestCard.number == UnoNumber.BLANK && players[player].cards[UnoColor.MAGENTA, UnoNumber.BLANK] > 0)
                    {
                        if (cards.Count == 0)
                            MovingCard.color = backColor;
                        Card card = new Card
                        {
                            color = UnoColor.MAGENTA,
                            number = UnoNumber.BLANK
                        };
                        for (byte c = 1; c <= players[player].cards[UnoColor.MAGENTA, UnoNumber.BLANK]; c++)
                            cards.Add(card);
                    }
                }
                else
                {
                    if (bestCard.color == UnoColor.WHITE)
                    {
                        int mp = 0, mq = 0;
                        foreach (byte c in colorList)
                        {
                            if (players[player].cards[c, bestCard.number] > 0)
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
                    else if (players[player].cards[bestCard.color, bestCard.number] > 0)
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
                        foreach (byte c in colorList)
                        {
                            if (players[player].cards[c, n] > 0)
                            {
                                if (n == backNumber || c == backColor)
                                {
                                    number = n;
                                    goto number_color;
                                }
                            }
                        }
                    }
                    goto exit;
number_color:
                    foreach (byte c in colorList)
                    {
                        for (int i = 1; i <= players[player].cards[c, number]; i++)
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
                        for (int c = 1; c <= players[player].cards[color, n]; c++)
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
                else if (options.mnuDos.Checked)
                {
                    if (bestCard.number <= 10)
                    {
                        List<Card> number = new List<Card>();
                        foreach (byte c in colorList)
                        {
                            for (byte u = 1; u <= players[player].cards[c, UnoNumber.NUMBER]; u++)
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
                foreach (byte c in colorList)
                {
                    int q = GetQuantityByColor(player, c);
                    int p = GetPointsByColor(player, c);
                    if (q > mq || q == mq && p > mp)
                    {
                        mq = q;
                        MovingCard.color = c;
                    }
                }
                if (options.mnuPairs.Checked)
                {
                    for (byte c = 0; c <= UnoColor.MAX_VALUE - 1; c++)
                        for (byte u = 1; u <= players[player].cards[c, bestCard.number]; u++)
                        {
                            Card card = new Card
                            {
                                color = c,
                                number = bestCard.number
                            };
                            cards.Add(card);
                        }
                }
                else if (players[player].cards[UnoColor.BLACK, bestCard.number] > 0)
                    cards.Add(bestCard);
            }
exit:
            if (options.mnuPro.Checked || options.mnuCheater.Checked)
            {
                if (cards.Count <= 0 && (isAutomatic && isFair || player > 0))
                {
                    bestCard = GetBestCard(player);
                    if (bestCard != null)
                    {
                        if (options.mnuPairs.Checked)
                        {
                            List<byte> colors = new List<byte>();
                            List<byte> allColors = new List<byte>() { 0, 1, 2, 3, 4, 5, 6 };
                            allColors.Remove(bestCard.color);
                            allColors.Insert(0, bestCard.color);
                            foreach (byte c in allColors)
                            {
                                for (int i = 1; i <= pile.cards[c, bestCard.number]; i++)
                                { 
                                    Card card = PlayersCards(player)[0];
                                    players[player].cards[card.color, card.number]--;
                                    pile.cards[card.color, card.number]++;
                                    colors.Add(c);
                                    if (PlayersCardsCount(player) <= 0)
                                        goto get_best;
                                }
                            }
get_best:
                            foreach (byte c in colors)
                            {
                                pile.cards[c, bestCard.number]--;
                                players[player].cards[c, bestCard.number]++;
                            }
                        }
                        else
                        {
                            Card card = PlayersCards(player)[0];
                            players[player].cards[card.color, card.number]--;
                            pile.cards[card.color, card.number]++;
                            pile.cards[bestCard.color, bestCard.number]--;
                            players[player].cards[bestCard.color, bestCard.number]++;
                        }
                        return Ai(player, true);
                    }
                }
            }
            return cards.ToArray();
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            if (!options.mnuMultiply0.Checked)
            {
                if (options.money <= 0)
                {
                    MessageBox.Show("你已負債累累, 等你有錢再來玩吧!", "開始", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                options.money -= options.multiplier;
                Interaction.SaveSetting("UNO", "ACCOUNT", "MONEY", options.money + "");
                mnuMoney.Text = Format(options.money);
            }
            if (!options.on0 && options.ons <= 0)
            {
                FormClosing -= new FormClosingEventHandler(Uno_FormClosing);
                if (MessageBox.Show(
                    "打和!\n"
                    + (options.mnuWatch.Checked && !options.isPlayingRecord ? $"\n游戏时长\t{lblWatch.Text}" : "")
                    + "\n"
                    + (hasCheat ? "\n(你在本局中出了老千)" : "")
                    + (options.mnuCheat.Checked ? "\n(本局允許作弊)" : "")
                    , "结束", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Retry)
                    Application.Restart();
                else
                    Application.Exit();
                return;
            }
            btnStart.Visible = false;
            lblMovingCards[0].BringToFront();
            mnuChat.Enabled = true;
            if (!options.isPlayingRecord)
            {
rnd:
                MovingCard.player = (byte)(4f * Rnd());
                if (!lblPlayers[MovingCard.player].Visible)
                    goto rnd;
                Record.firstGettingCard = MovingCard.player;
            }
            else
                MovingCard.player = Record.firstGettingCard;
            MovingCard.quickly = false;
            lblMovingCards[0].BringToFront();
            timPileToPlayers.Enabled = true;
        }

        bool CanPlay(List<Card> card, byte color)
        {
            if (canPlay)
                goto accept;
            if (!options.mnuStacking.Checked && draw > 0)
            {
                goto deny;
            }
            byte backColor = GetColorId(BackColor), backNumber = GetNumberId(lblCards[1].Text);
            if (!options.mnuPlayOrDrawAll.Checked)
            {
                switch (lblCards[1].Text)
                {
                    case UnoNumberName.DRAW_1:
                        if (draw >= 1)
                            if (card[0].number != UnoNumber.DRAW_1
                                && card[0].number != UnoNumber.DRAW_2
                                && card[0].number != UnoNumber.DRAW_5
                                && card[0].number != UnoNumber.WILD_DRAW_4
                                && (card[0].number != UnoNumber.REVERSE || card[0].color != UnoColor.BLACK))
                                goto deny;
                        break;
                    case UnoNumberName.DRAW_2:
                        if (draw >= 2)
                            if (card[0].number != UnoNumber.DRAW_2
                                && card[0].number != UnoNumber.DRAW_5
                                && card[0].number != UnoNumber.WILD_DRAW_4
                                && (card[0].number != UnoNumber.REVERSE || card.Last().color != UnoColor.BLACK))
                                goto deny;
                        break;
                    case UnoNumberName.DRAW_5:
                        if (draw >= 5)
                            if (card[0].number != UnoNumber.DRAW_5
                                && card[0].number != UnoNumber.WILD_DRAW_4
                                && (card[0].number != UnoNumber.REVERSE || card.Last().color != UnoColor.BLACK))
                                goto deny;
                        break;
                    case UnoNumberName.WILD_DRAW_4:
                        if (draw >= 4 && card[0].number != UnoNumber.WILD_DRAW_4
                            && (card[0].number != UnoNumber.REVERSE || card.Last().color != UnoColor.BLACK))
                            goto deny;
                        break;
                    case UnoNumberName.WILD_DRAW_COLOR:
                        if (drawColor > 0 && card[0].number != UnoNumber.WILD_DRAW_COLOR
                            && (card[0].number != UnoNumber.REVERSE || card.Last().color != UnoColor.BLACK))
                            goto deny;
                        break;
                    case UnoNumberName.WILD_HITFIRE:
                        if (draw > 0 && card[0].number != UnoNumber.WILD_HITFIRE
                            && (card[0].number != UnoNumber.REVERSE || card.Last().color != UnoColor.BLACK))
                            goto deny;
                        break;
                    case UnoNumberName.REVERSE when lblCards.Last().BackColor == Color.Black:
                        if (draw + drawColor > 0 && (card[0].number != UnoNumber.REVERSE || card.Last().color != UnoColor.BLACK))
                            goto deny;
                        break;
                }
            }
            if (options.mnuAttack.Checked)
            {
                if (card[0].number != UnoNumber.DISCARD_ALL)
                    goto wild_number;
                byte discardColor = UnoColor.MAX_VALUE;
                List<byte> discardColors = new List<byte>();
                if (lblCards[1].Text != UnoNumberName.DISCARD_ALL)
                    discardColor = backColor;
                foreach (Card c in card)
                {
                    switch (c.number)
                    {
                        case UnoNumber.DISCARD_ALL:
                            discardColors.Add(c.color);
                            break;
                        default:
                            if (discardColor == UnoColor.MAX_VALUE && discardColors.Contains(c.color))
                                discardColor = c.color;
                            if (c.color != discardColor)
                                goto deny;
                            break;
                    }
                }
                if (!discardColors.Contains(discardColor))
                    goto deny;
                goto end_number;
            }
wild_number:
            if (options.mnuDos.Checked)
            {
                if (!card.Cast<Card>().Select(b => b.number).Contains(UnoNumber.NUMBER))
                    goto number;
                byte n = backNumber;
                foreach (Card c in card)
                {
                    if (c.color == backColor || c.color == UnoColor.BLACK)
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
                        else if (c.number != n && c.color != UnoColor.BLACK)
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
end_number:
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
            if (card[0].number == backNumber || backNumber == UnoNumber.NUMBER && card[0].number <= 10)
                goto accept;
			goto deny;
accept:		
            return true;
deny:
            Action(0, "你不能这样出牌");
            return false;
        }

        void CheatPairs(byte player)
        {
begin_hacking:
            byte mn = UnoNumber.MAX_VALUE, mnc = UnoColor.MAX_VALUE;
            int mq = 0;
            for (byte b = 0; b < UnoNumber.MAX_VALUE; b++)
            {
                byte n = mvcList[b];
                int q = GetNumberQuantity(player, n);
                if (q > mq)
                {
                    for (byte c = 0; c < UnoColor.MAX_VALUE; c++)
                        if (pile.cards[c, n] > 0)
                        {
                            mq = q;
                            mn = n;
                            mnc = c;
                            break;
                        }
                }
            }
            byte fn = UnoNumber.MAX_VALUE, fnc = UnoColor.MAX_VALUE;
            int fq = int.MaxValue;
            for (byte b = UnoNumber.MAX_VALUE - 1; 0 <= b && b < UnoNumber.MAX_VALUE; b--)
            {
                byte n = mvcList[b];
                int q = GetNumberQuantity(player, n);
                if (q < fq && q > 0)
                {
                    for (byte c = 0; c < UnoColor.MAX_VALUE; c++)
                        if (players[player].cards[c, n] > 0)
                        {
                            fq = q;
                            fn = n;
                            fnc = c;
                            break;
                        }
                }
            }
            if (PlayersCardsCount(player) <= 1)
                goto start;
            if (mq <= fq)
                goto start;
            pile.cards[fnc, fn]++;
            players[player].cards[fnc, fn]--;
            players[player].cards[mnc, mn]++;
            pile.cards[mnc, mn]--;
            goto begin_hacking;
        start:
            return;
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
                        DialogResult result = MessageBox.Show("废牌堆无牌!", "牌已用尽", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error, MessageBoxDefaultButton.Button3);
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
                                SetDraw(0);
                                MovingCard.dbp = 0;
                                MovingCard.downpour = 0;
                                drawColor = 0;
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

        private void ChkPlayer_CheckedChanged(object sender, EventArgs e) {
			int index = (int)((CheckBox) sender).Tag;
            if (mnuAppearance.Checked)
            {
                chkPlayer[index].Top = chkPlayer[index].Checked ? 0 : UnoSize.HEIGHT / 8;
            }
            if (!options.mnuPairs.Checked && !options.mnuAttack.Checked && ((CheckBox)sender).Checked)
                for (int c = 0; c < chkPlayer.ToArray().Length; c++)
                    if (c != index) chkPlayer[c].Checked = false;
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
                if (0 < e.X && e.X < pnlPlayer.Width / chkPlayer.Count)
                    return;
                int w;
                if (hPlayer.Visible || pnlPlayer.Left > 0)
                    w = UnoSize.WIDTH;
                else
                    w = width / chkPlayer.ToArray().Length;
                int i = (int)Math.Floor((float)e.X / w) + int.Parse(((CheckBox)sender).Tag + "");
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
            if (e.Y < 0 && isPlayer0sTurn)
            {
                ((CheckBox)sender).Checked = true;
                Play(0);
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
            if (!isPlayer0sTurn)
            {
                MovingCard.quickly = true;
                return;
            }
            if (options.txtPlayer0.Text == "")
                return;
            byte number = UnoNumber.MAX_VALUE;
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    if (isPlayer0sTurn)
                        Play(0);
                    return;
                case Keys.Add:
                    if (isPlayer0sTurn)
                        Draw(0);
                    return;
                case Keys.Subtract:
                    rdoUno.Checked = true;
                    break;
                case Keys.Divide:
                    // ChkPlayer_MouseWheel(new object(), new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, 1));
                    return;
                case Keys.Multiply:
                    // ChkPlayer_MouseWheel(new object(), new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, -1));
                    return;
                case Keys.T:
                case Keys.OemQuestion:
                    MnuChat_Click(mnuChat, new EventArgs());
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
                    number = (byte)(e.KeyCode - Keys.NumPad0);
                    break;
                case Keys.Delete:
                    number = UnoNumber.SKIP;
                    break;
                case Keys.End:
                    number = UnoNumber.REVERSE;
                    break;
                case Keys.PageDown:
                    number = UnoNumber.DRAW_2;
                    break;
                case Keys.Insert:
                    number = UnoNumber.BLANK;
                    break;
                case Keys.Home:
                    number = UnoNumber.WILD;
                    break;
                case Keys.PageUp:
                    number = UnoNumber.WILD_DRAW_4;
                    break;
                default:
                    return;
            }
            if (options.mnuPairs.Checked)
            {
                if (mnuPicPlayer.Checked)
                {
                    for (int i = 0; i < PicPlayer.count; i++)
                        PicPlayer.checkeds[i] = PicPlayer.picPlayer[i].number == number;
                    PicPlayer_CheckedChanged();
                }
            }
            else
            {
                if (mnuPicPlayer.Checked)
                {
                    bool b = true;
                    for (int i = 0; i < PicPlayer.count; i++)
                    {
                        if (PicPlayer.checkeds[i] && PicPlayer.picPlayer[i].number == number)
                        {
                            b = false;
                        }
                        else
                        {
                            PicPlayer.checkeds[i] = false;
                        }
                    }
                    for (int i = 0; i < PicPlayer.count; i++)
                    {
                        if (!b)
                        {
                            if (PicPlayer.checkeds[i])
                            {
                                b = true;
                                PicPlayer.checkeds[i] = false;
                            }
                            continue;
                        }
                        if (PicPlayer.picPlayer[i].number == number)
                        {
                            PicPlayer.checkeds[i] = true;
                            break;
                        }
                    }
                    PicPlayer_CheckedChanged();
                }
            }
        }

        void DownpourDraw(byte player)
        {
            Downpour.player = player;
            MovingCard.downpour = Downpour.count;
            MovingCard.player = NextPlayer(player);
            MovingCard.progress = 0;
            MovingCard.quickly = false;
            lblMovingCards[0].BringToFront();
            timPileToPlayers.Enabled = true;
        }

        void Draw(byte player)
        {
            CheckPile();
            if (!options.isPlayingRecord)
            {
                Record.players[player].Add(new Card[0]);
                Record.colors.Add(MovingCard.color);
                if (player == 0)
                    Record.unos.Add(rdoUno.Checked);
            }
            if ((!MovingCard.drew || options.mnuDrawToMatch.Checked) && int.Parse(lblPile.Text) > 0)
            {
                if (gameOver >= 4)
                    Action(player, "摸牌");
                if (player == 0)
                {
                    isPlayer0sTurn = false;
                    if (options.mnuUno.Checked && rdoUno.Checked)
                    {
                        AddDraw(2);
                        Action(0, "UNO? +2");
                    }
                }
                MovingCard.player = player; MovingCard.progress = 0; MovingCard.quickly = false;
                lblMovingCards[0].BackColor = Color.White;
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

        string Format(int number)
        {
            return Format(number.ToString(), true);
        }

        int Format(string number)
        {
            return int.Parse(Format(number, false));
        }

        string Format(string number, bool optionsat)
        {
            if (optionsat)
            {
                byte b = 0;
                string s = "";
                for (int i = number.Length - 1; i >= 0; i--)
                {
                    if (b >= 4 && number[i] != '-')
                    {
                        s = ',' + s;
                        b = 0;
                    }
                    s = number[i] + s;
                    b++;
                }
                return "$" + s;
            }
            else
            {
                return number.Replace(",", "").Substring(1);
            }
        }

        private void GameOver()
        {
            timPileToCenter.Enabled = false;
            timPileToPlayers.Enabled = false;
            timTurn.Enabled = false;
            timPlayersToCenter.Enabled = false;
            timWatch.Enabled = false;
            if (!options.isPlayingRecord && options.mnuBeginner.Checked && !hasCheat)
            {
                Interaction.SaveSetting("UNO", "RECORD", "REVERSE", Record.reverse.ToString());
                Interaction.SaveSetting("UNO", "RECORD", "DEAL", Record.firstGettingCard.ToString());
                Interaction.SaveSetting("UNO", "RECORD", "FIRST", Record.firstTurn.ToString());
                Interaction.SaveSetting("UNO", "RECORD", "UNOS", string.Join(",", Record.unos));
                if (options.mnuSevenZero.Checked || options.mnuTradeHands.Checked)
                    Interaction.SaveSetting("UNO", "RECORD", "TRADE_HANDS", string.Join("P", Record.tradeHands));
                Interaction.SaveSetting("UNO", "RECORD", "COLORS", string.Join("C", Record.colors));
                if (options.keys.Length > 0)
                    Interaction.SaveSetting("UNO", "RECORD", "GAME", string.Join("K", options.keys));
                else
                    Interaction.SaveSetting("UNO", "RECORD", "GAME", "");
                string s = "";
                foreach (Card c in Record.pile)
                    s += c.color + "I" + c.number + "C";
                Interaction.SaveSetting("UNO", "RECORD", "PILE", s.Substring(0, s.Length - 1));
                s = "";
                foreach (List<Card[]> p in Record.players)
                {
                    foreach (Card[] t in p)
                    {
                        foreach (Card c in t)
                            s += c.color + "I" + c.number + "C";
                        s += "T";
                    }
                    s += "P";
                }
                Interaction.SaveSetting("UNO", "RECORD", "PLAYERS", s.Substring(0, s.Length - 1));
                Interaction.SaveSetting("UNO", "RECORD", "RULES", "KKKKKKKKK" + options.SaveRules());
            }
            FormClosing -= new FormClosingEventHandler(Uno_FormClosing);
            if (options.mnuOneWinner.Checked)
            {
                List<Label> label = new List<Label>();
                for (byte p = 1; p <= 3; p++)
                {
                    int playerPos = label.ToArray().Length;
                    if (mnuByColor.Checked)
                        for (byte color = 0; color <= UnoColor.MAX_VALUE; color++)
                            for (byte number = 0; number <= UnoNumber.MAX_VALUE; number++)
                                for (int c = 1; c <= players[p].cards[color, number]; c++)
                                {
                                    int length = label.ToArray().Length;
                                    AddLabel(label);
                                    SetCard(label[length], color, number);
                                    if (p == 1 || p == 3)
                                        label[length].Left = lblPlayers[p].Left;
                                    if (p == 2) label[length].Top = lblPlayers[2].Top;
                                }
                    else
                        for (byte number = 0; number <= UnoNumber.MAX_VALUE; number++)
                            for (byte color = 0; color <= UnoColor.MAX_VALUE; color++)
                                for (int c = 1; c <= players[p].cards[color, number]; c++)
                                {
                                    int length = label.ToArray().Length;
                                    AddLabel(label);
                                    SetCard(label[length], color, number);
                                    if (p == 1 || p == 3)
                                        label[length].Left = lblPlayers[p].Left;
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
                lblPlayers[gameOver].Visible = false;
                string msg = (gameOver == 0 ? "你" : PLAYER_NAMES[gameOver]) + " 赢了!\n" +
                    "\n" +
                    (options.mnuWatch.Checked && !options.isPlayingRecord ? $"游戏时长\t{lblWatch.Text}\n" +
                    "\n" : "") +
                    "玩家\t得分";
                for (byte p = 0; p <= 3; p++)
                    msg += "\n" + (p == 0 ? "你" : PLAYER_NAMES[p]) + "\t" + GetPointsByPlayer(p);
                msg += "\n";
                if (hasCheat)
                    msg += "\n(你在本局中出了老千)";
                if (options.mnuCheat.Checked)
                    msg += "\n(本局允許作弊)";
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
                                for (int c = 1; c <= players[gameOver].cards[color, number]; c++)
                                {
                                    int length = label.ToArray().Length;
                                    AddLabel(label);
                                    SetCard(label[length], color, number);
                                    if (gameOver == 1 || gameOver == 3)
                                        label[length].Left = lblPlayers[gameOver].Left;
                                    if (gameOver == 2) label[length].Top = lblPlayers[2].Top;
                                }
                    else
                        for (byte number = 0; number <= UnoNumber.MAX_VALUE; number++)
                            for (byte color = 0; color <= UnoColor.MAX_VALUE; color++)
                                for (int c = 1; c <= players[gameOver].cards[color, number]; c++)
                                {
                                    int length = label.ToArray().Length;
                                    AddLabel(label);
                                    SetCard(label[length], color, number);
                                    if (gameOver == 1 || gameOver == 3)
                                        label[length].Left = lblPlayers[gameOver].Left;
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
                lblPlayers[gameOver].Visible = false;
                if (MessageBox.Show(
                    (gameOver == 0 ? "你" : PLAYER_NAMES[gameOver]) + " 输了!\n"
                    + (options.mnuWatch.Checked && !options.isPlayingRecord ? $"\n游戏时长\t{lblWatch.Text}" : "")
                    + "\n"
                    + (hasCheat ? "\n(你在本局中出了老千)" : "")
                    + (options.mnuCheat.Checked ? "\n(本局允許作弊)" : "")
                    , "结束", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Retry)
                    goto retry;
            }
            Application.Exit();
            return;
        retry:
            Application.Restart();
        }

        Card GetBestCard(byte player)
        {
            if (MovingCard.isPlaying)
            {
                byte backColor = GetColorId(BackColor), backNumber = GetNumberId(lblCards[1].Text);
                if (backNumber == UnoNumber.REVERSE && lblCards.Last().BackColor == Color.Black && !options.mnuPlayOrDrawAll.Checked)
                {
                    if (pile.cards[UnoColor.BLACK, UnoNumber.REVERSE] > 0)
                        return new Card(UnoColor.BLACK, UnoNumber.REVERSE);
                    return null;
                }
                if (backNumber == UnoNumber.WILD_HITFIRE && !options.mnuPlayOrDrawAll.Checked)
                {
                    if (pile.cards[UnoColor.BLACK, UnoNumber.REVERSE] > 0)
                        return new Card(UnoColor.BLACK, UnoNumber.REVERSE);
                    if (pile.cards[UnoColor.BLACK, UnoNumber.WILD_HITFIRE] > 0)
                        return new Card(UnoColor.BLACK, UnoNumber.WILD_HITFIRE);
                    return null;
                }
                if (backNumber == UnoNumber.WILD_DRAW_COLOR && !options.mnuPlayOrDrawAll.Checked)
                {
                    if (pile.cards[UnoColor.BLACK, UnoNumber.REVERSE] > 0)
                        return new Card(UnoColor.BLACK, UnoNumber.REVERSE);
                    if (pile.cards[UnoColor.BLACK, UnoNumber.WILD_DRAW_COLOR] > 0)
                        return new Card(UnoColor.BLACK, UnoNumber.WILD_DRAW_COLOR);
                    return null;
                }
                if (backNumber == UnoNumber.WILD_DRAW_4 && !options.mnuPlayOrDrawAll.Checked)
                {
                    if (pile.cards[UnoColor.BLACK, UnoNumber.REVERSE] > 0)
                        return new Card(UnoColor.BLACK, UnoNumber.REVERSE);
                    if (pile.cards[UnoColor.BLACK, UnoNumber.WILD_DRAW_4] > 0)
                        return new Card(UnoColor.BLACK, UnoNumber.WILD_DRAW_4);
                    return null;
                }
                if (backNumber == UnoNumber.DRAW_5 && !options.mnuPlayOrDrawAll.Checked)
                {
                    if (pile.cards[UnoColor.BLACK, UnoNumber.REVERSE] > 0)
                        return new Card(UnoColor.BLACK, UnoNumber.REVERSE);
                    if (pile.cards[UnoColor.BLACK, UnoNumber.WILD_DRAW_4] > 0)
                        return new Card(UnoColor.BLACK, UnoNumber.WILD_DRAW_4);
                    foreach (byte b in colorList)
                        if (pile.cards[b, UnoNumber.DRAW_5] > 0)
                            return new Card(b, UnoNumber.DRAW_5);
                    return null;
                }
                if (backNumber == UnoNumber.DRAW_2 && !options.mnuPlayOrDrawAll.Checked)
                {
                    if (pile.cards[UnoColor.BLACK, UnoNumber.REVERSE] > 0)
                        return new Card(UnoColor.BLACK, UnoNumber.REVERSE);
                    if (pile.cards[UnoColor.BLACK, UnoNumber.WILD_DRAW_4] > 0)
                        return new Card(UnoColor.BLACK, UnoNumber.WILD_DRAW_4);
                    foreach (byte b in colorList)
                        if (pile.cards[b, UnoNumber.DRAW_5] > 0)
                            return new Card(b, UnoNumber.DRAW_5);
                    if (pile.cards[UnoColor.BLACK, UnoNumber.DRAW_2] > 0)
                        return new Card(UnoColor.BLACK, UnoNumber.DRAW_2);
                    foreach (byte b in colorList)
                        if (pile.cards[b, UnoNumber.DRAW_2] > 0)
                            return new Card(b, UnoNumber.DRAW_2);
                    return null;
                }
                if (backNumber == UnoNumber.DRAW_1 && !options.mnuPlayOrDrawAll.Checked)
                {
                    if (pile.cards[UnoColor.BLACK, UnoNumber.REVERSE] > 0)
                        return new Card(UnoColor.BLACK, UnoNumber.REVERSE);
                    if (pile.cards[UnoColor.BLACK, UnoNumber.WILD_DRAW_4] > 0)
                        return new Card(UnoColor.BLACK, UnoNumber.WILD_DRAW_4);
                    foreach (byte b in colorList)
                        if (pile.cards[b, UnoNumber.DRAW_5] > 0)
                            return new Card(b, UnoNumber.DRAW_5);
                    if (pile.cards[UnoColor.BLACK, UnoNumber.DRAW_2] > 0)
                        return new Card(UnoColor.BLACK, UnoNumber.DRAW_2);
                    foreach (byte b in colorList)
                        if (pile.cards[b, UnoNumber.DRAW_2] > 0)
                            return new Card(b, UnoNumber.DRAW_2);
                    foreach (byte b in colorList)
                        if (pile.cards[b, UnoNumber.DRAW_1] > 0)
                            return new Card(b, UnoNumber.DRAW_1);
                    return null;
                }
                for (byte b = 0; b < UnoNumber.MAX_VALUE; b++)
                {
                    byte n = mvcList[b];
                    if (pile.cards[UnoColor.BLACK, n] > 0)
                    {
                        return new Card(UnoColor.BLACK, n);
                    }
                }
                if (pile.cards[UnoColor.MAGENTA, UnoNumber.BLANK] > 0)
                {
                    return new Card(UnoColor.MAGENTA, UnoNumber.BLANK);
                }
                foreach (byte c in colorList)
                {
                    if (pile.cards[c, backNumber] > 0)
                    {
                        return new Card(c, backNumber);
                    }
                }
                if (options.mnuPairs.Checked)
                {
                    byte mn = UnoNumber.MAX_VALUE;
                    int mq = 0, qn;
                    for (byte b = 0; b < UnoNumber.MAX_VALUE; b++)
                    {
                        byte n = mvcList[b];
                        int q = GetNumberQuantity(player, n);
                        if (q > mq)
                        {
                            qn = 0;
                            if (n < UnoNumber.BLANK && players[player].cards[backColor, n] <= 0 && pile.cards[backColor, n] <= 0)
                                continue;
                            for (byte c = 0; c < UnoColor.MAX_VALUE; c++)
                                qn += pile.cards[c, n];
                            if (qn > 0)
                            {
                                mq = q;
                                mn = n;
                            }
                        }
                    }
                    if (mn != UnoNumber.MAX_VALUE)
                    {
                        if (pile.cards[backColor, mn] > 0)
                            return new Card(backColor, mn);
                        else
                            foreach (byte c in colorList)
                                if (pile.cards[c, mn] > 0)
                                    return new Card(c, mn);
                    }
                }
                for (byte b = 0; b < UnoNumber.MAX_VALUE; b++)
                {
                    byte n = mvcList[b];
                    if (pile.cards[backColor, n] > 0)
                    {
                        return new Card(backColor, n);
                    }
                }
            }
            for (byte b = 0; b < UnoNumber.MAX_VALUE; b++)
            {
                byte n = mvcList[b];
                for (byte c = 0; c < UnoColor.MAX_VALUE; c++)
                {
                    if (pile.cards[c, n] > 0)
                    {
                        return new Card(c, n);
                    }
                }
            }
            return null;
        }

		Color GetColor(byte id) {
            return id switch
            {
                0 => Color.Red,
                1 => Color.Yellow,
                2 => Color.Lime,
                3 => Color.Cyan,
                4 => Color.Blue,
                5 => Color.Magenta,
                6 => Color.Black,
                _ => Color.White,
            };
        }

		byte GetColorId(Color color) {
            return color.Name switch
            {
                "Red" => 0,
                "Yellow" => 1,
                "Lime" => 2,
                "Cyan" => 3,
                "Blue" => 4,
                "Magenta" => 5,
                "Black" => 6,
                _ => 7,
            };
        }

        string GetColorName(byte id) {
            return id switch
            {
                0 => "红",
                1 => "黃",
                2 => "绿",
                3 => "靑",
                4 => "蓝",
                5 => "紫",
                6 => "黑",
                _ => "白",
            };
        }

        int GetColorQuantity(byte player, byte color)
        {
            // eg: color = R
            // if [R 0][R 1][Y 1][Y 2][Y 2][Y 2]
            // then return 2
            int mq = 0;
            for (byte n = 0; n < UnoNumber.MAX_VALUE; n++)
            {
                int q = players[player].cards[color, n];
                if (q > 0)
                {
                    q = 0;
                    for (byte c = 0; c < UnoColor.MAX_VALUE; c++)
                        q += players[player].cards[c, n];
                    if (q > mq)
                        mq = q;
                }
            }
            return mq;
        }

        int GetColorQuantityByNumber(byte player, byte number)
        {
            // eg: number = 0
            // if [R 0][R 0][R 0][Y 0][R 1]
            // then return 2
            int i = 0;
            for (byte c = 0; c < UnoColor.MAX_VALUE; c++)
                i += Math.Sign(players[player].cards[c, number]);
            return i;
        }

        int GetDbp()
        {
            return (options.mnuDrawBeforePlaying.Checked ? 1 : 0) + (options.mnuDrawTwoBeforePlaying.Checked ? 2 : 0);
        }

        string GetNumber(byte id) {
            return id switch
            {
                UnoNumber.SKIP => UnoNumberName.SKIP,
                UnoNumber.SKIP_EVERYONE => UnoNumberName.SKIP_EVERYONE,
                UnoNumber.REVERSE => UnoNumberName.REVERSE,
                UnoNumber.DRAW_1 => UnoNumberName.DRAW_1,
                UnoNumber.DRAW_2 => UnoNumberName.DRAW_2,
                UnoNumber.DRAW_5 => UnoNumberName.DRAW_5,
                UnoNumber.DISCARD_ALL => UnoNumberName.DISCARD_ALL,
                UnoNumber.TRADE_HANDS => UnoNumberName.TRADE_HANDS,
                UnoNumber.NUMBER => UnoNumberName.NUMBER,
                UnoNumber.BLANK => options.txtBlankText.Text,
                UnoNumber.WILD => UnoNumberName.WILD,
                UnoNumber.WILD_DOWNPOUR_DRAW_1 => UnoNumberName.WILD_DOWNPOUR_DRAW_1,
                UnoNumber.WILD_DOWNPOUR_DRAW_2 => UnoNumberName.WILD_DOWNPOUR_DRAW_2,
                UnoNumber.WILD_DOWNPOUR_DRAW_4 => UnoNumberName.WILD_DOWNPOUR_DRAW_4,
                UnoNumber.WILD_DRAW_4 => UnoNumberName.WILD_DRAW_4,
                UnoNumber.WILD_DRAW_COLOR => UnoNumberName.WILD_DRAW_COLOR,
                UnoNumber.WILD_HITFIRE => UnoNumberName.WILD_HITFIRE,
                UnoNumber.NULL => UnoNumberName.NULL,
                _ => id + "",
            };
        }

		byte GetNumberId(string number) {
            if (number == options.txtBlankText.Text)
            {
                return UnoNumber.BLANK;
            }
            return number switch
            {
                UnoNumberName.SKIP => UnoNumber.SKIP,
                UnoNumberName.SKIP_EVERYONE => UnoNumber.SKIP_EVERYONE,
                UnoNumberName.REVERSE => UnoNumber.REVERSE,
                UnoNumberName.DRAW_1 => UnoNumber.DRAW_1,
                UnoNumberName.DRAW_2 => UnoNumber.DRAW_2,
                UnoNumberName.DRAW_5 => UnoNumber.DRAW_5,
                UnoNumberName.DISCARD_ALL => UnoNumber.DISCARD_ALL,
                UnoNumberName.TRADE_HANDS => UnoNumber.TRADE_HANDS,
                UnoNumberName.NUMBER => UnoNumber.NUMBER,
                UnoNumberName.WILD => UnoNumber.WILD,
                UnoNumberName.WILD_DOWNPOUR_DRAW_1 => UnoNumber.WILD_DOWNPOUR_DRAW_1,
                UnoNumberName.WILD_DOWNPOUR_DRAW_2 => UnoNumber.WILD_DOWNPOUR_DRAW_2,
                UnoNumberName.WILD_DOWNPOUR_DRAW_4 => UnoNumber.WILD_DOWNPOUR_DRAW_4,
                UnoNumberName.WILD_DRAW_4 => UnoNumber.WILD_DRAW_4,
                UnoNumberName.WILD_DRAW_COLOR => UnoNumber.WILD_DRAW_COLOR,
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
                UnoNumber.SKIP_EVERYONE => "Skip Everyone",
                UnoNumber.REVERSE => "Reverse",
                UnoNumber.DRAW_1 => "Draw One",
                UnoNumber.DRAW_2 => "Draw Two",
                UnoNumber.DRAW_5 => "Draw Five",
                UnoNumber.DISCARD_ALL => "Discard All",
                UnoNumber.TRADE_HANDS => "Trade Hands",
                UnoNumber.NUMBER => "Wild Number",
                UnoNumber.WILD => "Wild",
                UnoNumber.WILD_DOWNPOUR_DRAW_1 => "Wild Downpour Draw One",
                UnoNumber.WILD_DOWNPOUR_DRAW_2 => "Wild Downpour Draw Two",
                UnoNumber.WILD_DOWNPOUR_DRAW_4 => "Wild Downpour Draw Four",
                UnoNumber.WILD_DRAW_4 => "Wild Draw Four",
                UnoNumber.WILD_DRAW_COLOR => "Wild Draw Color",
                UnoNumber.WILD_HITFIRE => "Wild Hit-fire",
                UnoNumber.NULL => "Null",
                _ => number + "",
            };
        }

        int GetNumberQuantity(byte player, byte number)
        {
            // eg: number = 0
            // if [R 0][R 0][Y 0][R 1]
            // then return 3
            int i = 0;
            for (byte c = 0; c < UnoColor.MAX_VALUE; c++)
                i += players[player].cards[c, number];
            return i;
        }

        int GetOnplayersCards(byte color, byte number)
        {
            int cards = 0;
            if (players[0] == null)
                return 0;
            foreach (Cards p in players) cards += p.cards[color, number];
            return cards;
        }

        Card[] GetPile()
        {
            List<Card> cards = new List<Card>();
            for (byte color = 0; color <= UnoColor.MAX_VALUE; color++)
                for (byte number = 0; number <= UnoNumber.MAX_VALUE; number++)
                    for (int c = 1; c <= pile.cards[color, number]; c++)
                    {
                        cards.Add(new Card(color, number));
                    };
            return cards.ToArray();
        }

        int GetPoints(byte number)
        {
            switch (number)
            {
                default:
                    return number;
                case UnoNumber.DRAW_1:
                    return 10;
                case UnoNumber.SKIP:
                case UnoNumber.REVERSE:
                case UnoNumber.DRAW_2:
                case UnoNumber.DRAW_5:
                case UnoNumber.BLANK:
                case UnoNumber.WILD_DOWNPOUR_DRAW_1:
                case UnoNumber.WILD_DOWNPOUR_DRAW_2:
                    return 20;
                case UnoNumber.SKIP_EVERYONE:
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
                case UnoNumber.WILD_DRAW_COLOR:
                    return 60;
            }
        }

        int GetPointsByColor(byte player, byte color)
        {
            int points = 0;
            for (byte n = 0; n < UnoNumber.MAX_VALUE; n++)
            {
                points += GetPoints(n) * players[player].cards[color, n];
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

        int GetQuantityByColor(byte player, byte color)
        {
            // eg: color = R
            // if [R 0][R 1][R 2][Y 0]
            // then return 3
            int quantity = 0;
            for (byte q = 0; q <= UnoNumber.MAX_VALUE; q++)
                quantity += players[player].cards[color, q];
            return quantity;
        }

        int GetQuantityByNumber(byte player, byte number)
        {
            // eg: number = 0
            // if [R 0][R 1][Y 0][G 1][G 2][G 3]
            // then return 2
            int q, quantity = 0;
            for (byte c = 0; c <= UnoColor.MAX_VALUE; c++)
            {
                if (players[player].cards[c, number] > 0)
                {
                    q = 0;
                    for (byte n = 0; n <= UnoNumber.MAX_VALUE; n++)
                    {
                        q += players[player].cards[c, n];
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
            else if (number > 10)
                return "功能";
            else
                return "普通";
        }

        string GetUsage(byte number)
        {
            switch (number)
            {
                case UnoNumber.SKIP: return "禁止下家出牌";
                case UnoNumber.SKIP_EVERYONE: return "禁止每家出牌";
                case UnoNumber.REVERSE: return "反转出牌顺序";
                case UnoNumber.DRAW_1: return "下家罚抽 1 张牌";
                case UnoNumber.DRAW_2: return "下家罚抽 2 张牌";
                case UnoNumber.DRAW_5: return "下家罚抽 5 张牌";
                case UnoNumber.DISCARD_ALL: return "允许玩家打出所有相同颜色的牌";
                case UnoNumber.TRADE_HANDS: return "選擇一位玩家交換手牌";
                case UnoNumber.NUMBER: return "表示任意数字";
                case UnoNumber.BLANK:
                    int downpourDraw = int.Parse(options.txtBlankDownpourDraw.Text), draw = int.Parse(options.txtBlankDraw.Text), skip = int.Parse(options.txtBlankSkip.Text);
                    string s = ""
                        + (skip > 0 ? $"\n　　\t禁止下家出牌 × {skip}" : "")
                        + (options.mnuBlankReverse.Checked ? "\n　　\t反转出牌顺序" : "")
                        + (draw > 0 ? $"\n　　\t下家罚抽 {draw} 张牌" : "")
                        + (downpourDraw > 0 ? $"\n　　\t所有玩家罚抽 {downpourDraw} 张牌" : "");
                    return s == "" ? "普通的空白牌" : s.Substring(4);
                case UnoNumber.WILD: return "";
                case UnoNumber.WILD_DOWNPOUR_DRAW_1: return "所有玩家罚抽 1 张牌";
                case UnoNumber.WILD_DOWNPOUR_DRAW_2: return "所有玩家罚抽 2 张牌";
                case UnoNumber.WILD_DOWNPOUR_DRAW_4: return "所有玩家罚抽 4 张牌";
                case UnoNumber.WILD_DRAW_4: return "下家罚抽 4 张牌";
                case UnoNumber.WILD_DRAW_COLOR: return "下家罚抽牌直至抽到被指定顏色的牌";
                case UnoNumber.WILD_HITFIRE: return "下家罚抽牌盒中的所有牌";
                default: return $"普通的 {number} 号牌";
            };
        }

        private void HPlayer_Scroll(object sender, ScrollEventArgs e)
        {
            pnlPlayer.Left = -((HScrollBar)sender).Value;
        }

        bool IsNumeric(string s)
        {
            return new Regex(@"^[+-]?\d+[.\d]*$").IsMatch(s);
        }

        private void ItmAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "作者\n" +
                "MISTER_CHAN\n\n" +
                "版本\n" +
                Application.ProductVersion
                , "关于", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

		private void ItmQuit_Click(object sender, EventArgs e) {
            Application.Exit();
		}

        void Label_MouseLeave(object sender, EventArgs e)
        {
            toolTip.Hide(this);
        }

        private void LblCounts_SizeChanged(object sender, EventArgs e)
        {
            Label l = (Label)sender;
            int i = int.Parse(l.Tag + "");
            l.Left = lblPlayers[i].Left + lblPlayers[i].Width / 2 - l.Width / 2;
        }

        private void LblCounts_TextChanged(object sender, EventArgs e) {
            byte index = (byte)((Label)sender).Tag;
            if (options.mnuRevealable.Checked && index > 0)
            {
                Reveal(index);
            }
            if (MovingCard.isPlaying)
            {
                byte winners = 0;
                if (int.Parse(lblCounts[index].Text) <= 0)
                {
                    lblPlayers[index].Visible = false;
                    lblCounts[index].Visible = false;
                    if (!options.mnuMultiply0.Checked)
                    {
                        int[] payments = new int[4], points = new int[4];
                        for (byte p = 0; p <= 3; p++)
                        {
                            if (lblCounts[p].Visible)
                            {
                                points[p] = GetPointsByPlayer(p);
                            }
                        }
                        int win = 0;
                        string msg = "";
                        for (byte p = 0; p <= 3; p++)
                        {
                            if (lblCounts[p].Visible)
                            {
                                payments[p] = points[p] * options.multiplier;
                                win += payments[p];
                                msg += $"{(p == 0 ? "你" : PLAYER_NAMES[p])}: -{Format(payments[p])}\n";
                            }
                        }
                        if (index == 0)
                        {
                            options.money += win;
                            mnuMoney.Text = Format(options.money);
                            Interaction.SaveSetting("UNO", "ACCOUNT", "MONEY", options.money + "");
                        }
                        else if (payments[0] > 0)
                        {
                            options.money -= payments[0];
                            mnuMoney.Text = Format(options.money);
                            Interaction.SaveSetting("UNO", "ACCOUNT", "MONEY", options.money + "");
                        }
                        Action(index, $"{msg}----------------\n{(index == 0 ? "你" : PLAYER_NAMES[index])}: +{Format(win)}");
                    }
                }
                byte player;
                if (options.mnuOneWinner.Checked)
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
                if (options.mnuOneWinner.Checked)
                {
                    byte currentNumber = GetNumberId(lblCards[1].Text);
                    switch (currentNumber)
                    {
                        case UnoNumber.DRAW_1:
                        case UnoNumber.DRAW_2:
                        case UnoNumber.DRAW_5:
                        case UnoNumber.WILD_DRAW_4:
                        case UnoNumber.WILD_DRAW_COLOR:
                        case UnoNumber.WILD_HITFIRE:
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

        private void LblPile_MouseClick(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (isPlayer0sTurn)
                        Draw(0);
                    break;
                case MouseButtons.Right:
                    if (options.mnuRevealable.Checked || isFair)
                    {
                        hasCheat = true;
                        int i = 0;
                        string cards = "";
                        for (byte c = 0; c <= UnoColor.MAX_VALUE; c++)
                            for (byte n = 0; n <= UnoNumber.MAX_VALUE; n++)
                            {
                                string q = pile.cards[c, n] + "";
                                if (q != "0")
                                {
                                    if (q == "1")
                                    {
                                        q = "";
                                    }
                                    cards += "[" + GetColorName(c) + GetNumber(n) + "]" + q;
                                    i++;
                                    if (i % (int)((float)width / UnoSize.WIDTH) == 0)
                                    {
                                        cards += "\n";
                                    }
                                }
                            }
                        Action(0, cards);
                    }
                    break;
            }

        }

        private void LblPile_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                lblPile.MouseDoubleClick -= LblPile_MouseDoubleClick;
                for (byte p = 1; p <= 3; p++)
                {
                    lblPlayers[p].MouseDown += LblPlayers_MouseDown;
                    lblPlayers[p].MouseUp += LblPlayers_MouseUp;
                }
                if (mnuPicPlayer.Checked)
                    picPlayer.ContextMenuStrip = mnuCheating;
                else
                    foreach (CheckBox c in chkPlayer)
                        c.ContextMenuStrip = mnuCheating;
                isFair = true;
                mnuCheatAll.Visible = options.mnuPairs.Checked;
                MessageBox.Show("你現在可以使用某些操作來出術.", "你好像從牌堆中發現了甚麼");
            }
        }

        private void LblPlayers_MouseDown(object sender, MouseEventArgs e)
        {
            hasCheat = true;
            byte index = (byte)((Label)sender).Tag;
            lblPlayers[index].AutoSize = true;
            lblPlayers[index].BackgroundImage = null;
            Reveal(index);
            switch (index)
            {
                case 1: lblPlayers[1].Top = height / 2 - lblPlayers[1].Height / 2; break;
                case 2: lblPlayers[2].Left = width / 2 - lblPlayers[2].Width / 2; break;
                case 3: lblPlayers[3].Location = new Point(width - lblPlayers[3].Width, height / 2 - lblPlayers[3].Height / 2); break;
            }
        }

        private void LblPlayers_MouseUp(object sender, MouseEventArgs e)
        {
            byte index = (byte)((Label)sender).Tag;
            lblPlayers[index].AutoSize = false;
            lblPlayers[index].BackgroundImage = Properties.Resources.uno_back;
            lblPlayers[index].Text = "";
            switch (index)
            {
                case 1: lblPlayers[1].Location = new Point(0, height / 2 - UnoSize.HEIGHT / 2 + mnuGame.Height / 2); break;
                case 2: lblPlayers[2].Location = new Point(width / 2 - UnoSize.WIDTH / 2, mnuGame.Height); break;
                case 3: lblPlayers[3].Location = new Point(width - UnoSize.WIDTH, height / 2 - UnoSize.HEIGHT / 2 + mnuGame.Height / 2); break;
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
                        players[p].cards[c, n] = int.Parse(numbers[n]);
                }
                lblCounts[p].Text = PlayersCardsCount(p).ToString();
            }
            Sort();
            colors = keys[1].Split(char.Parse("C"));
            for (byte c = 0; c <= UnoColor.MAX_VALUE; c++)
            {
                string[] numbers = colors[c].Split(char.Parse("N"));
                for (byte n = 0; n <= UnoNumber.MAX_VALUE; n++)
                    pile.cards[c, n] = int.Parse(numbers[n]);
            }
            lblPile.Text = GetPile().Length - 1 + "";
            BackColor = GetColor(byte.Parse(keys[2]));
            foreach (Label card in lblCards)
            {
                byte c = byte.Parse(keys[2]), n = byte.Parse(keys[3]);
                if (n >= UnoNumber.WILD)
                    c = UnoColor.BLACK;
                SetCard(card, c, n);
            }
            skip = int.Parse(keys[4]);
            string[] sks = keys[5].Split('P');
            for (byte sk = 0; sk <= 3; sk++)
                skips[sk] = int.Parse(sks[sk]);
            reverse = bool.Parse(keys[6]);
            SetDraw(int.Parse(keys[7]));
            gametime = int.Parse(keys[8]);
        }

        private void LoadRecord()
        {
            Record.reverse = bool.Parse(Interaction.GetSetting("UNO", "RECORD", "REVERSE"));
            Record.firstGettingCard = byte.Parse(Interaction.GetSetting("UNO", "RECORD", "DEAL"));
            Record.firstTurn = byte.Parse(Interaction.GetSetting("UNO", "RECORD", "FIRST"));
            Record.unos = new List<bool>(Interaction.GetSetting("UNO", "RECORD", "UNOS").Split(',').Cast<string>().Select(s => bool.Parse(s)));
            if (options.mnuSevenZero.Checked || options.mnuTradeHands.Checked)
                Record.tradeHands = new List<byte>(Interaction.GetSetting("UNO", "RECORD", "TRADE_HANDS").Split('P').Cast<string>().Select(s => byte.Parse(s)));
            Record.colors = new List<byte>(Interaction.GetSetting("UNO", "RECORD", "COLORS").Split('C').Cast<string>().Select(s => byte.Parse(s)));
            Record.pile = new List<Card>(Interaction.GetSetting("UNO", "RECORD", "PILE").Split('C').Cast<string>().Select(s => new Card(byte.Parse(s.Split('I')[0]), byte.Parse(s.Split('I')[1]))));
            string[] ps = Interaction.GetSetting("UNO", "RECORD", "PLAYERS").Split('P');
            for (byte p = 0; p < 4; p++)
            {
                List<string> ts = new List<string>(ps[p].Split('T'));
                foreach (string t in ts)
                {
                    if (t == "")
                    {
                        Record.players[p].Add(new Card[0]);
                        continue;
                    }
                    string[] cs = t.Split('C');
                    List<Card> cards = new List<Card>();
                    foreach (string c in cs)
                    {
                        if (c == "")
                            continue;
                        cards.Add(new Card(byte.Parse(c.Split('I')[0]), byte.Parse(c.Split('I')[1])));
                    }
                    Record.players[p].Add(cards.ToArray());
                }
            }
        }

        private void MnuAppearance_Click(object sender, EventArgs e)
        {
            ((ToolStripMenuItem)sender).Checked = !((ToolStripMenuItem)sender).Checked;
        }

        private void MnuAuto_Click(object sender, EventArgs e)
        {
            isAutomatic = !isAutomatic;
            mnuAuto.Text = (isAutomatic ? "☑" : "☐") + "託管 (&A)";
            if (isPlayer0sTurn)
            {
                isPlayer0sTurn = false;
                mnuSaveGame.Enabled = false;
                Play(0);
            }
        }

        private void MnuCanPlay_Click(object sender, EventArgs e)
        {
            if (!isPlayer0sTurn)
            {
                MessageBox.Show("請在到你出牌時再來學習此內容.", "出牌敎程", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            bool isFair = this.isFair;
            this.isFair = false;
            Card[] cards = Ai(0);
            this.isFair = isFair;
            string s = "";
            foreach (Card c in cards)
            {
                s += "[" + GetColorName(c.color) + GetNumber(c.number) + "]";
            }
            if (s != "")
            {
                string color = "";
                foreach (Card c in cards)
                {
                    if (c.color == UnoColor.BLACK || c.color != cards[0].color)
                    {
                        color = $"\n轉{GetColorName(MovingCard.color)}色";
                        break;
                    }
                }
                if (MessageBox.Show($"你可以嘗試出:\n{s}{color}\n\n需要我敎你嗎?", "出牌敎程", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2) == DialogResult.OK)
                {
                    if (mnuPicPlayer.Checked)
                    {
                        for (int i = 0; i < PicPlayer.count; i++)
                        {
                            PicPlayer.checkeds[i] = false;
                            foreach (Card c in cards)
                            {
                                if (PicPlayer.picPlayer[i].color == c.color && PicPlayer.picPlayer[i].number == c.number)
                                {
                                    PicPlayer.checkeds[i] = true;
                                    break;
                                }
                            }
                        }
                        PicPlayer_CheckedChanged();
                    }
                    else
                    {
                        foreach (CheckBox chk in chkPlayer)
                        {
                            chk.Checked = false;
                            foreach (Card c in cards)
                            {
                                if (GetColorId(chk.BackColor) == c.color && GetNumberId(chk.Text) == c.number)
                                {
                                    chk.Checked = true;
                                    break;
                                }
                            }
                        }
                    }
                    Action(0, "提示");
                }
            }
            else
            {
                MessageBox.Show("無可出的牌, 請摸牌!", "出牌敎程", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void MnuChat_Click(object sender, EventArgs e)
        {
            string cmd = Interaction.InputBox(">", "Chat");
            int count = 0;
            cmd = cmd.Trim();
            while (cmd.Contains("  "))
                cmd = cmd.Replace("  ", " ");
            if (cmd == "")
                return;
            if (cmd.Substring(0, 1) == "/")
            {
                if (options.mnuCheat.Checked)
                {
                    string[] data = cmd.Split(char.Parse(" "));
                    switch (data[0].ToLower())
                    {
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
                                    players[player].cards[color, number] = 0;
                            if (player == 0) Sort();
                            Action(0, $"已淸除 {PLAYER_NAMES[player]} 的手牌");
                            lblCounts[player].Text = PlayersCardsCount(player) + "";
                            break;
                        case "/currard":
                            if (data.Length > 2)
                            {
                                BackColor = GetColor(byte.Parse(data[1]));
                                foreach (Label card in lblCards)
                                    SetCard(card, byte.Parse(data[1]), byte.Parse(data[2]));
                            }
                            else if (data.Length == 2)
                                BackColor = GetColor(byte.Parse(data[1]));
                            Action(0, $"你的回合被指定为 [{GetColorName(byte.Parse(data[1]))} {lblCards[1].Text}]");
                            break;
                        case "/decks":
                            if (data.Length > 1)
                                if (IsNumeric(data[1]))
                                {
                                    options.txtDecks.Text = data[1];
                                    Action(0, $"已准备 {data[1]} 副牌");
                                }
                            break;
                        case "/draw":
                            if (data.Length == 2)
                                SetDraw(int.Parse(data[1]));
                            else
                            {
                                count = 1;
                                if (data.Length > 2)
                                    count = int.Parse(data[2]);
                                for (int c = 1; c <= count; c++)
                                {
                                    Card[] pile = GetPile();
                                    Card rndCard = pile[(int)(pile.Length * Rnd())];
                                    this.pile.cards[rndCard.color, rndCard.number]--;
                                    lblPile.Text = pile.Length - 1 + "";
                                    players[int.Parse(data[1])].cards[rndCard.color, rndCard.number]++;
                                    lblCounts[int.Parse(data[1])].Text = PlayersCardsCount(byte.Parse(data[1])) + "";
                                }
                                if (data[1] == "0")
                                    Sort();
                            }
                            break;
                        case "/give":
                            if (data.Length < 4) return;
                            count = 1;
                            if (data.Length >= 5) count = Int32.Parse(data[4]);
                            players[Byte.Parse(data[1])].cards[Byte.Parse(data[2]), Byte.Parse(data[3])] += count;
                            if (data[1] == "0")
                                Sort();
                            lblCounts[int.Parse(data[1])].Text = PlayersCardsCount(byte.Parse(data[1])) + "";
                            Action(0, $"已将 [{GetColorName(byte.Parse(data[2]))} {GetNumber(byte.Parse(data[3]))}] * {count} 给予 {PLAYER_NAMES[byte.Parse(data[1])]}");
                            break;
                        case "/help":
                        case "/?":
                            string page = "1";
                            if (data.Length > 1)
                                page = data[1];
                            string help = page switch
                            { 
                                "1" => "" +
                                    "/clear [byte player] [byte color] [byte number]\n" +
                                    "/currard <byte color> [byte number]\n" +
                                    "/decks <int decks>\n" +
                                    "/draw [byte player] <int draw>\n" +
                                    "/give <byte player> <byte color> <byte number> [int count]\n" +
                                    "/help [int page]\n" +
                                    "/load [string data]\n" +
                                    "/me <string action>\n" +
                                    "/play <bool canPlay>\n" +
                                    "/play <byte color> <card card_0> <card card_1> ...\n" +
                                    "/pause [options | pause | quit | restart]\n" +
                                    "/reverse [bool reverse]\n" +
                                    "/save\n" +
                                    "/say <string message>\n" +
                                    "/skip [[byte player] <int skip>]\n" +
                                    "/time true | false | pause | <int gametime> | <int h>:<int m>:<int s>\n" +
                                    "/uno [bool isUno]\n" +
                                    "/? [int page]",
                                _ => ""
                            };
                            MessageBox.Show($"=== 显示指令列表第 {page} 页, 共 1 页 ===\n{help}", "帮助", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                            if (data.Length > 1)
                                Action(0, "* 你 " + cmd.Substring(cmd.IndexOf(" ") + 1));
                            break;
                        case "/play":
                            if (data.Length > 1)
                            {
                                if (data[1].ToLower() == "false" || data[1].ToLower() == "true")
                                {
                                    canPlay = bool.Parse(data[1]);
                                    Action(0, "已切換強制出牌");
                                }
                                else if (isPlayer0sTurn)
                                {
                                    bool b = canPlay;
                                    canPlay = true;
                                    MovingCard.color = byte.Parse(data[1]);
                                    List<Card> c = new List<Card>();
                                    for (int i = 2; i < data.Length; i += 2)
                                        c.Add(new Card { color = byte.Parse(data[i]), number = byte.Parse(data[i + 1]) });
                                    Play(0, c.ToArray());
                                    canPlay = b;
                                }
                            }
                            break;
                        case "/pause":
                            if (data.Length == 1)
                            {
                                options.Show();
                            }
                            else
                            {
                                switch (data[1])
                                {
                                    default:
                                        options.Show();
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
                                    if (isPlayer0sTurn)
                                    {
                                        PlayersTurn(0, false);
                                        PlayersTurn(MovingCard.player = NextPlayer(MovingCard.player), true, GetDbp());
                                        Action(0, "跳过");
                                    }
                                    else
                                    {
                                        SetInterval(0);
                                        Action(0, "跳过動畫");
                                    }
                                    break;
                                case 2:
                                    skip = int.Parse(data[1]);
                                    Action(0, $"跳过 {skip} 个玩家");
                                    break;
                                default:
                                    skips[byte.Parse(data[1])] = int.Parse(data[2]);
                                    Action(0, $"跳过 {PLAYER_NAMES[byte.Parse(data[1])]} {data[2]} 次");
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

        private void MnuCheat_Click(object sender, EventArgs e)
        {
            Card card = GetBestCard(0);
            if (card != null)
            {
                hasCheat = true;
                byte color, number;
                Control control = mnuCheating.SourceControl;
                if (mnuPicPlayer.Checked)
                {
                    color = PicPlayer.picPlayer[pointing].color;
                    number = PicPlayer.picPlayer[pointing].number;
                }
                else
                {
                    color = GetColorId(control.BackColor);
                    number = GetNumberId(control.Text);
                }
                pile.cards[card.color, card.number]--;
                players[0].cards[color, number]--;
                pile.cards[color, number]++;
                players[0].cards[card.color, card.number]++;
                if (mnuPicPlayer.Checked)
                {
                    PicPlayer.picPlayer[pointing].color = card.color;
                    PicPlayer.picPlayer[pointing].number = card.number;
                    PicPlayer_CheckedChanged();
                }
                else
                    SetCard(control, card.color, card.number);
            }
        }

        private void MnuCheatAll_Click(object sender, EventArgs e)
        {
            CheatPairs(0);
            Sort();
        }

        private void MnuContent_Click(object sender, EventArgs e)
        {
            MessageBox.Show("將手中的牌打完卽獲勝.", "遊戲玩法", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MnuControl_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "[0 ~ 9]\tNumpad0~9\n" +
                $"[{UnoNumberName.SKIP}]\tDelete\n" +
                $"[{UnoNumberName.REVERSE}]\tEnd\n" +
                $"[{UnoNumberName.DRAW_2}]\tPageDown\n" +
                $"[{options.txtBlankText.Text}]\tInsert\n" +
                $"[{UnoNumberName.WILD}]\tHome\n" +
                $"[{UnoNumberName.WILD_DRAW_4}]\tPageUp\n" +
                "上一页\tNumpad/\n" +
                "下一页\tNumpad*\n" +
                "光标移动\t←↑→↓\n" +
                "选定卡牌\tSpace\n" +
                "选择颜色\tNumpad0~3\n" +
                "出牌\tEnter\n" +
                "摸牌\tNumpad+\n" +
                "喊 UNO!\tNumpad-\n" +
                "聊天\tT\n" +
                "指令\t/\n"
                , "按键说明", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MnuForward_Click(object sender, EventArgs e)
        {
            if (options.animation > 0)
            {
                if (1 < distance)
                {
                    SetInterval(0);
                    mnuPlayPause.Text = "⏵播放 (&P)";
                    MovingCard.quickly = true;
                }
                else
                {
                    SetInterval(options.animation);
                    mnuPlayPause.Text = "⏸暫停 (&P)";
                }
            }
        }

        private void MnuNew_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("放弃本局?", "新游戏", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Cancel)
                return;
            FormClosing -= new FormClosingEventHandler(Uno_FormClosing);
            Application.Restart();
        }

        private void MnuPicPlayer_Click(object sender, EventArgs e)
        {
            mnuPicPlayer.Checked = !mnuPicPlayer.Checked;
            pnlPlayer.Visible = !mnuPicPlayer.Checked;
            picPlayer.Visible = mnuPicPlayer.Checked;
            Sort();
        }

        private void MnuPlayPause_Click(object sender, EventArgs e)
        {
            if (1 < distance && distance < short.MaxValue)
            {
                SetInterval(short.MaxValue);
                mnuPlayPause.Text = "⏵播放 (&P)";
            }
            else
            {
                SetInterval(options.animation);
                mnuPlayPause.Text = "⏸暫停 (&P)";
            }
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

        private void MnuStop_Click(object sender, EventArgs e)
        {
            Application.Exit();
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

        void PicPlayer_CheckedChanged(bool sort = false)
        {
            if (width <= 0)
                return;
            if (sort)
            {
                PicPlayer.count = PlayersCards(0).Length;
                PicPlayer.checkeds = new bool[PicPlayer.count];
                PicPlayer.selected = -1;
                PicPlayer.picPlayer.Clear();
            }
            if (PicPlayer.count <= 0)
            {
                picPlayer.Visible = false;
                return;
            }
            int i = 0;
            if (PicPlayer.count * UnoSize.WIDTH <= width)
            {
                imgPlayer = new Bitmap(PicPlayer.count * UnoSize.WIDTH, UnoSize.HEIGHT + UnoSize.HEIGHT / 8);
                gpsPlayer = Graphics.FromImage(imgPlayer);
                if (sort)
                {
                    picPlayer.Width = PicPlayer.count * UnoSize.WIDTH;
                    PicPlayer.cardWidth = UnoSize.WIDTH;
                    if (mnuByColor.Checked)
                        for (byte color = 0; color <= UnoColor.MAX_VALUE; color++)
                            for (byte number = 0; number <= UnoNumber.MAX_VALUE; number++)
                                for (int c = 1; c <= players[0].cards[color, number]; c++)
                                {
                                    PicPlayer.picPlayer.Add(new Card(color, number));
                                    gpsPlayer.DrawImage(imgUno, new Rectangle(i * UnoSize.WIDTH, PicPlayer.checkeds[i] ? 0 : UnoSize.HEIGHT / 8, UnoSize.WIDTH, UnoSize.HEIGHT), number * ResourceUnoSize.WIDTH, color * ResourceUnoSize.HEIGHT, ResourceUnoSize.WIDTH, ResourceUnoSize.HEIGHT, GraphicsUnit.Pixel);
                                    i++;
                                }
                    else
                        for (byte number = 0; number <= UnoNumber.MAX_VALUE; number++)
                            for (byte color = 0; color <= UnoColor.MAX_VALUE; color++)
                                for (int c = 1; c <= players[0].cards[color, number]; c++)
                                {
                                    PicPlayer.picPlayer.Add(new Card(color, number));
                                    gpsPlayer.DrawImage(imgUno, new Rectangle(i * UnoSize.WIDTH, PicPlayer.checkeds[i] ? 0 : UnoSize.HEIGHT / 8, UnoSize.WIDTH, UnoSize.HEIGHT), number * ResourceUnoSize.WIDTH, color * ResourceUnoSize.HEIGHT, ResourceUnoSize.WIDTH, ResourceUnoSize.HEIGHT, GraphicsUnit.Pixel);
                                    i++;
                                }
                    picPlayer.Left = width / 2 - picPlayer.Width / 2;
                }
                else
                {
                    for (i = 0; i < PicPlayer.count; i++)
                        gpsPlayer.DrawImage(imgUno, new Rectangle(i * UnoSize.WIDTH, PicPlayer.checkeds[i] ? 0 : UnoSize.HEIGHT / 8, UnoSize.WIDTH, UnoSize.HEIGHT), PicPlayer.picPlayer[i].number * ResourceUnoSize.WIDTH, PicPlayer.picPlayer[i].color * ResourceUnoSize.HEIGHT, ResourceUnoSize.WIDTH, ResourceUnoSize.HEIGHT, GraphicsUnit.Pixel);
                }
            }
            else
            {
                imgPlayer = new Bitmap(width, UnoSize.HEIGHT + UnoSize.HEIGHT / 8);
                gpsPlayer = Graphics.FromImage(imgPlayer);
                if (sort)
                {
                    picPlayer.Width = width;
                    PicPlayer.cardWidth = (float)width / PicPlayer.count;
                    if (mnuByColor.Checked)
                        for (byte color = 0; color <= UnoColor.MAX_VALUE; color++)
                            for (byte number = 0; number <= UnoNumber.MAX_VALUE; number++)
                                for (int c = 1; c <= players[0].cards[color, number]; c++)
                                {
                                    PicPlayer.picPlayer.Add(new Card(color, number));
                                    gpsPlayer.DrawImage(imgUno, new Rectangle((int)(i * PicPlayer.cardWidth), PicPlayer.checkeds[i] ? 0 : UnoSize.HEIGHT / 8, UnoSize.WIDTH, UnoSize.HEIGHT), number * ResourceUnoSize.WIDTH, color * ResourceUnoSize.HEIGHT, ResourceUnoSize.WIDTH, ResourceUnoSize.HEIGHT, GraphicsUnit.Pixel);
                                    i++;
                                }
                    else
                        for (byte number = 0; number <= UnoNumber.MAX_VALUE; number++)
                            for (byte color = 0; color <= UnoColor.MAX_VALUE; color++)
                                for (int c = 1; c <= players[0].cards[color, number]; c++)
                                {
                                    PicPlayer.picPlayer.Add(new Card(color, number));
                                    gpsPlayer.DrawImage(imgUno, new Rectangle((int)(i * PicPlayer.cardWidth), PicPlayer.checkeds[i] ? 0 : UnoSize.HEIGHT / 8, UnoSize.WIDTH, UnoSize.HEIGHT), number * ResourceUnoSize.WIDTH, color * ResourceUnoSize.HEIGHT, ResourceUnoSize.WIDTH, ResourceUnoSize.HEIGHT, GraphicsUnit.Pixel);
                                    i++;
                                }
                    picPlayer.Left = 0;
                }
                else
                {
                    for (i = 0; i < PicPlayer.count; i++)
                        gpsPlayer.DrawImage(imgUno, new Rectangle((int)(i * PicPlayer.cardWidth), PicPlayer.checkeds[i] ? 0 : UnoSize.HEIGHT / 8, UnoSize.WIDTH, UnoSize.HEIGHT), PicPlayer.picPlayer[i].number * ResourceUnoSize.WIDTH, PicPlayer.picPlayer[i].color * ResourceUnoSize.HEIGHT, ResourceUnoSize.WIDTH, ResourceUnoSize.HEIGHT, GraphicsUnit.Pixel);
                }
            }
            picPlayer.Image = imgPlayer;
            PicPlayer.selecting = -1;
        }
        void PicPlayer_CheckedChanging(int index, bool isChecked)
        {
            gpsPlayer = Graphics.FromImage(imgPlayer);
            gpsPlayer.FillRectangle(new SolidBrush(Color.FromArgb(0x80, Color.Gray)), index * PicPlayer.cardWidth, isChecked ? UnoSize.HEIGHT / 8 : 0, PicPlayer.cardWidth, UnoSize.HEIGHT); ;
            picPlayer.Image = imgPlayer;
        }

        private void PicPlayer_MouseDown(object sender, MouseEventArgs e)
        {
            PicPlayer.isSelecting = true;
            PicPlayer_MouseMove(sender, e);
        }

        private void PicPlayer_MouseMove(object sender, MouseEventArgs e)
        {
            int i = (int)(e.X / PicPlayer.cardWidth);
            if (i >= PicPlayer.count || i < 0 || e.Y < 0 || e.Y > UnoSize.HEIGHT)
                return;
            pointing = i;
            if (i != PicPlayer.selecting)
            {
                if (i != PicPlayer.pointing)
                {
                    SetUsage(picPlayer, PicPlayer.picPlayer[i].color, PicPlayer.picPlayer[i].number);
                    PicPlayer.pointing = i;
                }
                if (PicPlayer.isSelecting)
                {
                    switch (e.Button)
                    {
                        case MouseButtons.Left:
                            if (!PicPlayer.checkeds[i])
                            {
                                if (!options.mnuPairs.Checked && PicPlayer.selected != -1)
                                    PicPlayer.checkeds[PicPlayer.selected] = false;
                                PicPlayer.checkeds[i] = true;
                                if (!options.mnuPairs.Checked)
                                    PicPlayer.selected = i;
                                PicPlayer_CheckedChanging(i, true);
                            }
                            else
                            {
                                PicPlayer.checkeds[i] = false;
                                PicPlayer_CheckedChanging(i, false);
                            }
                            break;
                        case MouseButtons.Right:
                            PicPlayer_CheckedChanging(i, !PicPlayer.checkeds[i]);
                            PicPlayer.checkeds[i] = false;
                            break;
                    }
                    PicPlayer.selecting = i;
                }
            }
        }

        private void PicPlayer_MouseUp(object sender, MouseEventArgs e)
        {
            PicPlayer.isSelecting = false;
            PicPlayer_CheckedChanged();
            PicPlayer.selecting = -1;
            PicPlayer.pointing = -1;
            if (e.Y < 0 && isPlayer0sTurn)
            {
                PicPlayer.checkeds[pointing] = true;
                Play(0);
            }
        }

        private void PicPlayer_MouseWheel(object sender, MouseEventArgs e)
        {
            if (isFair)
            {
                Card oldCard = PicPlayer.picPlayer[pointing];
                int count = pile.cards[oldCard.color, oldCard.number];
                pile.cards[oldCard.color, oldCard.number] = 1;
                Card[] p = GetPile();
                if (p.Length <= 1)
                    return;
                hasCheat = true;
                int i;
                for (i = 0; i < p.Length; i++)
                    if (p[i].color == oldCard.color && p[i].number == oldCard.number)
                        break;
                pile.cards[oldCard.color, oldCard.number] = count;
                Card newCard = null;
                switch (Math.Sign(e.Delta))
                {
                    case -1:
                        if (i > 0)
                            newCard = p[i - 1];
                        else
                            newCard = p.Last();
                        break;
                    case 1:
                        if (i < p.Length - 1)
                            newCard = p[i + 1];
                        else
                            newCard = p.First();
                        break;
                }
                players[0].cards[oldCard.color, oldCard.number]--;
                pile.cards[oldCard.color, oldCard.number]++;
                pile.cards[newCard.color, newCard.number]--;
                players[0].cards[newCard.color, newCard.number]++;
                PicPlayer.picPlayer[pointing] = newCard;
                PicPlayer_CheckedChanged();
            }
        }

        void Play(byte player, Card[] playingCards = null)
        {
            List<Card> cards = new List<Card>();
            if (options.isPlayingRecord)
            {
                if (player == 0)
                {
                    rdoUno.Checked = Record.unos[0];
                    Record.unos.RemoveAt(0);
                }
                cards = new List<Card>(Record.players[player][0]);
                foreach (Card card in cards)
                    players[player].cards[card.color, card.number]--;
                if (player == 0 && cards.Count > 0)
                    Sort();
                Record.players[player].RemoveAt(0);
                MovingCard.color = Record.colors[0];
                Record.colors.RemoveAt(0);
            }
            else if (playingCards != null)
            {
                foreach (Card c in playingCards)
                    cards.Add(c);
            }
			else if (player == 0 && !isAutomatic)
            {
                List<Card> discardAll = new List<Card>(), number = new List<Card>();
                if (mnuPicPlayer.Checked)
                {
                    for (int i = 0; i < PicPlayer.count; i++)
                    {
                        if (PicPlayer.checkeds[i])
                        {
                            Card card = PicPlayer.picPlayer[i];
                            switch (card.number)
                            {
                                default:
                                    cards.Add(card);
                                    break;
                                case UnoNumber.DISCARD_ALL:
                                    discardAll.Add(card);
                                    break;
                                case UnoNumber.NUMBER:
                                    number.Add(card);
                                    break;
                            }
                        }
                    }
                }
                else
                {
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
                }
                discardAll.AddRange(cards);
                cards = discardAll;
                cards.AddRange(number);
                if (!CanPlay(cards, cards.First().color))
                    return;
				for (int c = 0; c < cards.ToArray().Length; c++) {
					players[0].cards[cards[c].color, cards[c].number]--;
				}
                Sort();
            }
            else
            {
                Card[] ais = Ai(player);
                foreach (Card card in ais)
                {
                    cards.Add(card);
                    players[player].cards[card.color, card.number]--;
                }
                if (player == 0)
                {
                    if (cards.Count > 0)
                    {
                        Sort();
                        rdoUno.Checked = PlayersCardsCount(player) == 1;
                    }
                }
            }
            if (player == 0)
                Action(0, options.mnuUno.Checked && rdoUno.Checked ? "UNO!" : "出牌");
            else
                Action(player, options.mnuUno.Checked && (player != 0 || isAutomatic) && PlayersCardsCount(player) == 1 ? "UNO!" : "出牌");
			PlayersTurn(player, false);
            if (cards.Count > 0)
            {
                MovingCard.player = player; MovingCard.progress = 0;
                if (player == 0 && !isAutomatic && playingCards == null)
                {
                    bool b = PlayersCardsCount(0) > 0;
                    if (!b)
                    {
                        if (options.mnuOneLoser.Checked)
                        {
                            byte ons = 0;
                            foreach (Label l in lblPlayers)
                                if (l.Visible)
                                    ons++;
                            b = ons > 2;
                        }
                    }
                    if (b)
                    {
                        FrmColor c = new FrmColor(colorList);
                        int colors = 0;
                        foreach (Card card in cards)
                        {
                            if (card.color == UnoColor.BLACK)
                            {
                                foreach (ToolStripMenuItem menu in c.mnuColor.Items)
                                {
                                    menu.Enabled = true;
                                    menu.BackColor = GetColor(byte.Parse(menu.Tag + ""));
                                }
                                colors = colorList.Length;
                                break;
                            }
                        }
                        if (colors <= 0)
                        {
                            foreach (Card card in cards)
                                if (card.color != UnoColor.MAGENTA && card.color != UnoColor.BLACK)
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
                    {
                        MovingCard.color = GetColorId(BackColor);
                    }
                }
                if (!options.isPlayingRecord)
                {
                    Record.players[player].Add(cards.ToArray());
                    Record.colors.Add(MovingCard.color);
                    if (player == 0)
                    {
                        Record.unos.Add(rdoUno.Checked);
                    }
                }
                MovingCard.number = cards[0].number;
                RemoveLabel(lblMovingCards, pnlMovingCards.Controls);
                AddLabel(lblMovingCards, cards.Count, pnlMovingCards.Controls);
                for (int c = 1; c < lblMovingCards.Count; c++)
                {
                    SetCard(lblMovingCards[c], cards[c - 1].color, cards[c - 1].number);
                }

                if (lblMovingCards.Count <= swpcw / 2) // swpcw: Screen's Width Per Card's Width
                {
                    for (int c = 1; c < lblMovingCards.Count; c++)
                    {
                        lblMovingCards[c].Location = new Point(UnoSize.WIDTH * (c - 1), 0);
                    }
                    pnlMovingCards.Size = new Size(UnoSize.WIDTH * (lblMovingCards.Count - 1), UnoSize.HEIGHT);
                }
                else
                {
                    int lines = (int)((lblMovingCards.Count - 1f) / swpcw * 2f);
                    if (lines == 0)
                        lines = 1;
                    int cpl = lblMovingCards.Count / lines;
                    int w = width / 2 / cpl;
                    for (int c = 1; c < lblMovingCards.Count; c++)
                    {
                        lblMovingCards[c].Location = new Point(w * (int)((c - 1f) % cpl),
                            UnoSize.HEIGHT * (int)((c - 1f) / cpl));
                    }
                    pnlMovingCards.Size = new Size(width / 2, UnoSize.HEIGHT * lines);
                }
                timPlayersToCenter.Enabled = true;
                pnlMovingCards.Location = new Point(-pnlMovingCards.Width, -pnlMovingCards.Height);
                pnlMovingCards.BringToFront();
            }
            else if (player > 0 || isAutomatic)
            {
                Draw(player);
            }
        }

		Card[] PlayersCards(byte player) {
			List<Card> cards = new List<Card>();
			for (byte color = 0; color <= UnoColor.MAX_VALUE; color++)
				for (byte number = 0; number <= UnoNumber.MAX_VALUE; number++)
                    for (int c = 1; c <= players[player].cards[color, number]; c++) {
                        Card card = new Card
                        {
                            color = color,
                            number = number
                        };
                        cards.Add(card);
					};
			return cards.ToArray();
		}

        int PlayersCardsCount(byte player)
        {
            int i = 0;
            for (byte c = 0; c <= UnoColor.MAX_VALUE; c++)
                for (byte n = 0; n <= UnoNumber.MAX_VALUE; n++)
                    i += players[player].cards[c, n];
            return i;
        }

		void PlayersTurn(byte player, bool turn = true, int dbp = 0, bool delay = true) {
            int count = int.Parse(lblCounts[player].Text);
            if (count <= 7 * Math.Pow(2, dbp - 1))
            {
                if (count <= 1)
                    dbp = 0;
                else
                    dbp = (int)Math.Log((count - 1) / 7, 2) + 1;
            }
            if (turn)
            {
                if (delay && timTurn.Tag.ToString().Split(char.Parse(","))[0] == "4" && timThinking.Tag.ToString().Split(char.Parse(","))[0] == "4")
                {
                    timTurn.Tag = player + "," + dbp;
                    timTurn.Enabled = true;
                    return;
                }
                timTurn.Tag = "4,";
                if (TradingHands.player1 < 4)
                {
                    if (player == 0 && !isAutomatic && TradingHands.player2 == 5)
                    {
                        TradeHands t = new TradeHands();
                        for (byte p = 1; p <= 3; p++)
                        {
                            if (lblPlayers[p].Visible)
                            {
                                t.mnuTradeHands.Items[p].Enabled = true;
                                t.mnuTradeHands.Items[p].Text = p switch { 1 => "◀", 2 => "▲", 3 => "▶", _ => "▼" };
                            }
                        }
                        t.ShowDialog();
                        TradingHands.player2 = (byte)t.Tag;
                        Record.tradeHands.Add(TradingHands.player2);
                    }
                    TradeHands();
                    return;
                }
                if (Downpour.count > 0)
                {
                    DownpourDraw(player);
                    return;
                }
                if (skip > 0 || skips[player] > 0)
                {
                    if (options.mnuSkipPlayers.Checked)
                    {
                        Action(player, $"禁手 ({skip})");
                        skip--;
                    }
                    else
                    {
                        Action(player, $"禁手 ({skips[player]})");
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
                if (turn)
                {
                    MovingCard.dbp = 0;
                    Action(0, "你的回合");
                    rdoUno.Checked = false;
                    if (mnuPicPlayer.Checked)
                    {
                        picPlayer.BringToFront();
                    }
                    else
                    {
                        pnlPlayer.BringToFront();
                        if (!options.Visible)
                            chkPlayer[0].Focus();
                    }
                    if (distance == 1 && options.animation > 0)
                        SetInterval(options.animation);
                    if (options.mnuAutoSave.Checked && options.mnuMultiply0.Checked)
                        Interaction.SaveSetting("UNO", "GAME", "AUTO", SaveGame());
                }
                isPlayer0sTurn = turn;
                rdoUno.Visible = turn;
                if (options.mnuMultiply0.Checked)
                    mnuSaveGame.Enabled = turn;
            }
            else if (turn)
            {
                if (options.mnuThinking.Checked && player > 0 && !MovingCard.drew && timThinking.Tag.ToString().Split(char.Parse(","))[0] == "4")
                {
                    Action(player, PLAYER_NAMES[player] + " 的回合");
                    timThinking.Interval = (int)(distance * 100 * Rnd()) + 1;
                    timThinking.Tag = player + "," + dbp;
                    timThinking.Enabled = true;
                    return;
                }
                timThinking.Tag = "4,";
                if (player == 0)
                {
                    rdoUno.Checked = false;
                }
                Play(player);
            }
        }

        void RefillPile()
        {
            int decks = int.Parse(options.txtDecks.Text);
            foreach (byte c in colorList)
            {
                pile.cards[c, 0] = decks - GetOnplayersCards(c, 0);
                for (byte n = 1; n <= 9; n++)
                    pile.cards[c, n] = 2 * decks - GetOnplayersCards(c, n);
                if (options.mnuDos.Checked)
                {
                    pile.cards[c, 10] = 2 * decks - GetOnplayersCards(c, 10);
                    pile.cards[c, UnoNumber.NUMBER] = 2 * decks - GetOnplayersCards(c, UnoNumber.NUMBER);
                }
                pile.cards[c, UnoNumber.SKIP] = 2 * decks - GetOnplayersCards(c, UnoNumber.SKIP);
                pile.cards[c, UnoNumber.REVERSE] = 2 * decks - GetOnplayersCards(c, UnoNumber.REVERSE);
                pile.cards[c, UnoNumber.DRAW_2] = 2 * decks - GetOnplayersCards(c, UnoNumber.DRAW_2);
                if (options.mnuFlip.Checked)
                {
                    pile.cards[c, UnoNumber.SKIP_EVERYONE] = 2 * decks - GetOnplayersCards(c, UnoNumber.SKIP_EVERYONE);
                    pile.cards[c, UnoNumber.DRAW_1] = 2 * decks - GetOnplayersCards(c, UnoNumber.DRAW_1);
                    pile.cards[c, UnoNumber.DRAW_5] = 2 * decks - GetOnplayersCards(c, UnoNumber.DRAW_5);
                }
                if (options.mnuAttack.Checked)
                {
                    pile.cards[c, UnoNumber.DISCARD_ALL] = decks - GetOnplayersCards(c, UnoNumber.DISCARD_ALL);
                    if (options.mnuTradeHands.Checked)
                        pile.cards[c, UnoNumber.TRADE_HANDS] = decks - GetOnplayersCards(c, UnoNumber.TRADE_HANDS);
                }
                if (options.mnuBlank.Checked)
                    pile.cards[c, UnoNumber.BLANK] = decks - GetOnplayersCards(c, UnoNumber.BLANK);
            }
            if (options.mnuDos.Checked)
            {
                pile.cards[UnoColor.BLACK, 2] = 4 * decks - GetOnplayersCards(UnoColor.BLACK, 2);
            }
            if (options.mnuBlank.Checked && options.mnuMagentaBlank.Checked)
                pile.cards[UnoColor.MAGENTA, UnoNumber.BLANK] = 1 * decks - GetOnplayersCards(UnoColor.MAGENTA, UnoNumber.BLANK);
            if (options.mnuBlank.Checked && options.mnuBlackBlank.Checked)
                pile.cards[UnoColor.BLACK, UnoNumber.BLANK] = 2 * decks - GetOnplayersCards(UnoColor.BLACK, UnoNumber.BLANK);
            pile.cards[UnoColor.BLACK, UnoNumber.WILD] = 4 * decks - GetOnplayersCards(UnoColor.BLACK, UnoNumber.WILD);
            if (options.mnuWater.Checked)
            {
                pile.cards[UnoColor.BLACK, UnoNumber.WILD_DOWNPOUR_DRAW_1] = decks - GetOnplayersCards(UnoColor.BLACK, UnoNumber.WILD_DOWNPOUR_DRAW_1);
                pile.cards[UnoColor.BLACK, UnoNumber.WILD_DOWNPOUR_DRAW_2] = decks - GetOnplayersCards(UnoColor.BLACK, UnoNumber.WILD_DOWNPOUR_DRAW_2);
                pile.cards[UnoColor.BLACK, UnoNumber.WILD_DOWNPOUR_DRAW_4] = decks - GetOnplayersCards(UnoColor.BLACK, UnoNumber.WILD_DOWNPOUR_DRAW_4);
            }
            if (options.mnuFlip.Checked)
                pile.cards[UnoColor.BLACK, UnoNumber.DRAW_2] = 4 * decks - GetOnplayersCards(UnoColor.BLACK, UnoNumber.DRAW_2);
            pile.cards[UnoColor.BLACK, UnoNumber.WILD_DRAW_4] = 4 * decks - GetOnplayersCards(UnoColor.BLACK, UnoNumber.WILD_DRAW_4);
            if (options.mnuAttack.Checked)
            {
                pile.cards[UnoColor.BLACK, UnoNumber.WILD_DOWNPOUR_DRAW_1] = 2 * decks - GetOnplayersCards(UnoColor.BLACK, UnoNumber.WILD_DOWNPOUR_DRAW_1);
            }
            if (options.mnuFlip.Checked)
                pile.cards[UnoColor.BLACK, UnoNumber.WILD_DRAW_COLOR] = 4 * decks - GetOnplayersCards(UnoColor.BLACK, UnoNumber.WILD_DRAW_COLOR);
            if (options.mnuWildHitfire.Checked)
            {
                pile.cards[UnoColor.BLACK, UnoNumber.WILD_HITFIRE] = 2 * decks - GetOnplayersCards(UnoColor.BLACK, UnoNumber.WILD_HITFIRE);
            }
            if (options.mnuWildPunch.Checked)
                pile.cards[UnoColor.BLACK, UnoNumber.REVERSE] = 4 * decks - GetOnplayersCards(UnoColor.BLACK, UnoNumber.REVERSE);
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
            height = ClientRectangle.Height - mnuGame.Height;
            lblPlayers[0].Location = new Point(width / 2 - UnoSize.WIDTH / 2, height - UnoSize.HEIGHT);
            lblPlayers[1].Location = new Point(0, height / 2 - UnoSize.HEIGHT / 2 + mnuGame.Height / 2);
            lblPlayers[2].Location = new Point(width / 2 - UnoSize.WIDTH / 2, mnuGame.Height);
            lblPlayers[3].Location = new Point(width - UnoSize.WIDTH, height / 2 - UnoSize.HEIGHT / 2 + mnuGame.Height / 2);
			for (byte i = 0; i < 4; i++)
                lblCounts[i].Location = new Point(lblPlayers[i].Left + lblPlayers[i].Width / 2 - lblCounts[i].Width / 2, lblPlayers[i].Top - lblCounts[i].Height);
            lblCounts[2].Top = lblPlayers[2].Top + UnoSize.HEIGHT;
            rdoUno.Location = new Point(width - rdoUno.Width, lblCounts[0].Top);
            pnlPlayer.Top = rdoUno.Top + rdoUno.Height;
            picPlayer.Top = rdoUno.Top + rdoUno.Height;
            hPlayer.Top = picPlayer.Top;
            hPlayer.Width = width;
            swpcw = width / UnoSize.WIDTH;
            if (MovingCard.isPlaying)
                Sort();
            lblCards[0].Location = new Point(width / 2 - UnoSize.WIDTH / 2, height / 2 - UnoSize.HEIGHT / 2);
            btnStart.Location = new Point(width / 2 - btnStart.Width / 2, height / 2 - btnStart.Height / 2);
        }

		double Rnd() {
            if (options.mnuSeed.Checked)
                return 0;
            if (options.mnuGuid.Checked)
            {
                return new Random(Guid.NewGuid().GetHashCode()).NextDouble();
            }
            else if (options.mnuRNGCryptoServiceProvider.Checked)
            {
                byte[] b = new byte[1];
                new RNGCryptoServiceProvider().GetBytes(b);
                return b[0] / 256d;
            }
            else if (options.mnuMembership.Checked)
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
                        s += players[p].cards[c, n] + "N";
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
                    s += pile.cards[c, n] + "N";
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
            s += draw + "K"; // 7
            s += gametime + "K"; // 8
            s += options.SaveRules();
            return s;
        }

        void SetCard(Control card, byte color, byte number)
        {
            card.BackColor = GetColor(color);
            card.Text = GetNumber(number);
            if (mnuAppearance.Checked)
            {
                Image image = new Bitmap(ResourceUnoSize.WIDTH, ResourceUnoSize.HEIGHT);
                Graphics graphics = Graphics.FromImage(image);
                graphics.DrawImage(imgUno, new Rectangle(0, 0, ResourceUnoSize.WIDTH, ResourceUnoSize.HEIGHT), number * ResourceUnoSize.WIDTH, color * ResourceUnoSize.HEIGHT, ResourceUnoSize.WIDTH, ResourceUnoSize.HEIGHT, GraphicsUnit.Pixel);
                card.BackgroundImage = image;
                card.Font = new Font(card.Font.FontFamily, 1);
                graphics.Flush();
                graphics.Dispose();
            }
            SetUsage(card, color, number);
        }

        private void SetInterval(int distance)
        {
            this.distance = distance;
            int interval = distance * 50;
            if (interval <= 0)
            {
                this.distance = 1;
                interval = 1;
            }
            timTurn.Interval = interval;
            timUno.Interval = interval;
        }

        void SetUsage(Control card, byte color, byte number)
        {
            string usage = GetUsage(number);
            usage = color switch
            {
                UnoColor.MAGENTA => "表示任意顏色" + (usage == "" ? "" : "\n　　\t") + usage,
                UnoColor.BLACK => "任意指定颜色" + (usage == "" ? "" : "\n　　\t") + usage,
                _ => usage,
            };
            if (color == UnoColor.BLACK && number == UnoNumber.REVERSE)
                usage += "\n　　\t玩家免受懲罰";
            toolTip.ToolTipTitle = GetNumberName(number);
            toolTip.SetToolTip(card, "" +
                "颜色\t" + GetColorName(color) + "\n" +
                "数字\t" + GetNumber(number) + "\n" +
                "點數\t" + GetPoints(number) + "\n" +
                "类型\t" + GetType(color, number) + "\n" +
                "功能\t" + usage);
        }

        private void Reveal(byte player)
        {
            int i = 0;
            string cards = "";
            if (mnuByColor.Checked)
                for (byte c = 0; c <= UnoColor.MAX_VALUE; c++)
                    for (byte n = 0; n <= UnoNumber.MAX_VALUE; n++)
                    {
                        string q = players[player].cards[c, n] + "";
                        if (q != "0")
                        {
                            if (q == "1")
                                q = "";
                            string s = "[" + GetColorName(c) + GetNumber(n) + "]" + q;
                            cards += s;
                            if (player == 1 || player == 3)
                                cards += "\n";
                            i++;
                        }
                    }
            else
                for (byte n = 0; n <= UnoNumber.MAX_VALUE; n++)
                    for (byte c = 0; c <= UnoColor.MAX_VALUE; c++)
                    {
                        string q = players[player].cards[c, n] + "";
                        if (q != "0")
                        {
                            if (q == "1")
                                q = "";
                            string s = "[" + GetColorName(c) + GetNumber(n) + "]" + q;
                            cards += s;
                            if (player == 1 || player == 3)
                                cards += "\n";
                            i++;
                        }
                    }
            switch (player)
            {
                case 1: lblPlayers[1].Top = height / 2 - lblPlayers[1].Height / 2; break;
                case 2: lblPlayers[2].Left = width / 2 - lblPlayers[2].Width / 2; break;
                case 3: lblPlayers[3].Location = new Point(width - lblPlayers[3].Width, height / 2 - lblPlayers[3].Height / 2); break;
            }
            lblPlayers[player].Text = cards;
        }

        void SetDraw(int draw)
        {
            this.draw = draw;
            lblDraw.Text = draw + "";
        }

        void SetDrawColor(int draw)
        {
            drawColor = draw;
            lblDraw.Text = draw + "";
        }

        void Sort()
        {
            if (mnuPicPlayer.Checked)
            {
                PicPlayer_CheckedChanged(true);
            }
            else
            {
                RemoveChkPlayer();
                AddChkPlayer(PlayersCards(0).Length);
                int i = 0;
                if (mnuByColor.Checked)
                    for (byte color = 0; color <= UnoColor.MAX_VALUE; color++)
                        for (byte number = 0; number <= UnoNumber.MAX_VALUE; number++)
                            for (int c = 1; c <= players[0].cards[color, number]; c++)
                            {
                                SetCard(chkPlayer[i], color, number);
                                i++;
                            }
                else
                    for (byte number = 0; number <= UnoNumber.MAX_VALUE; number++)
                        for (byte color = 0; color <= UnoColor.MAX_VALUE; color++)
                            for (int c = 1; c <= players[0].cards[color, number]; c++)
                            {
                                SetCard(chkPlayer[i], color, number);
                                i++;
                            }
                hPlayer.Visible = false;
                int top = mnuAppearance.Checked ? UnoSize.HEIGHT / 8 : 0, width = UnoSize.WIDTH * chkPlayer.Count;
                if (width <= this.width)
                {
                    pnlPlayer.Left = this.width / 2 - width / 2;
                    pnlPlayer.Width = width;
                    for (i = 0; i < chkPlayer.ToArray().Length; i++)
                        chkPlayer[i].Location = new Point(UnoSize.WIDTH * i, top);
                }
                else if (width > this.width * 8 && mnuScrollBar.Checked)
                {
                    pnlPlayer.Left = 0;
                    pnlPlayer.Width = width;
                    for (i = 0; i < chkPlayer.ToArray().Length; i++)
                        chkPlayer[i].Location = new Point(UnoSize.WIDTH * i, top);
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
                    pnlPlayer.Width = this.width;
                    for (i = 0; i < chkPlayer.ToArray().Length; i++)
                        chkPlayer[i].Location = new Point(this.width / chkPlayer.ToArray().Length * i, top);
                }
                if (!options.Visible && chkPlayer.Count > 0)
                {
                    chkPlayer[0].Focus();
                }
            }
        }

        private void TimPileToCenter_Tick(object sender, EventArgs e) {
            lblMovingCards[0].Location = new Point(lblCards[0].Left / distance * MovingCard.progress, lblCards[0].Top / distance * MovingCard.progress + mnuGame.Height);
			if (lblMovingCards[0].Left >= lblCards[0].Left && lblMovingCards[0].Top >= lblCards[0].Top)
            {
                MovingCard.progress = 0;
                Card[] pile = GetPile();
                Card rndCard;
                if (!options.isPlayingRecord)
                {
                    rndCard = pile[(int)(pile.Length * Rnd())];
                }
                else
                {
                    rndCard = Record.pile[0];
                    Record.pile.RemoveAt(0);
                }
				this.pile.cards[rndCard.color, rndCard.number]--;
				lblPile.Text = pile.Length - 1 + "";
				AddLabel(lblCards);
				lblCards[1].BringToFront();
				lblCards[1].Font = new Font(lblCards[1].Font.FontFamily, 42);
				lblCards[1].ForeColor = Color.White;
				lblCards[1].Location = new Point(lblCards[0].Left, lblCards[0].Top);
				lblCards[1].TextAlign = ContentAlignment.MiddleCenter;
                SetCard(lblCards[1], rndCard.color, rndCard.number);
                if (lblCards[1].BackColor == Color.Magenta || lblCards[1].BackColor == Color.Black)
                {
                    MovingCard.progress = 0;
                    return;
                }
                if (!options.isPlayingRecord)
                    Record.pile.Add(rndCard);
                BackColor = lblCards[1].BackColor;
                timPileToCenter.Enabled = false;
                lblMovingCards[0].Location = new Point(-UnoSize.WIDTH, -UnoSize.HEIGHT);
                MovingCard.isPlaying = true;
                if (!options.isPlayingRecord)
                {
                    reverse = (int)(2f * Rnd()) == 0;
                    Record.reverse = reverse;
                }
                else
                    reverse = Record.reverse;
                if (options.keys.Length == 0)
                {
                    byte nextPlayer;
                    if (!options.isPlayingRecord)
                    {
                        if (options.mnuFirst.Checked)
                            nextPlayer = 0;
                        else
                            nextPlayer = NextPlayer((byte)(4 * Rnd()));
                        Record.firstTurn = nextPlayer;
                    }
                    else
                    {
                        nextPlayer = Record.firstTurn;
                    }
                    if (lblCards[1].Text == options.txtBlankText.Text)
                    {
                        if (options.mnuSkipPlayers.Checked)
                            skip = int.Parse(options.txtBlankSkip.Text);
                        else
                            skips[nextPlayer] = int.Parse(options.txtBlankSkip.Text);
                        AddDraw(int.Parse(options.txtBlankDraw.Text));
                    }
                    else
                    {
                        switch (lblCards[1].Text)
                        {
                            case UnoNumberName.SKIP:
                                if (options.mnuSkipPlayers.Checked)
                                    skip = 1;
                                else
                                    skips[nextPlayer] = 1;
                                break;
                            case UnoNumberName.DRAW_1:
                                AddDraw(1);
                                break;
                            case UnoNumberName.DRAW_2:
                                AddDraw(2);
                                break;
                            case UnoNumberName.DRAW_5:
                                AddDraw(5);
                                break;
                        }
                    }
                    if (options.multiplier > 0)
                        new Multiplier(options).ShowDialog();
                    int i = options.multiplier;
                    PlayersTurn(nextPlayer, true, GetDbp());
                }
                else
                {
                    try
                    {
                        LoadGame(options.keys);

                    }
                    catch
                    {
                        MessageBox.Show("无效的游戏记彔!", "读取游戏", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        FormClosing -= new FormClosingEventHandler(Uno_FormClosing);
                        Application.Restart();
                    }
                    PlayersTurn(0);
                }
                if (options.mnuWatch.Checked)
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
                Card rndCard;
                if (!options.isPlayingRecord)
                {
                    rndCard = pile[(int)(pile.Length * Rnd())];
                    Record.pile.Add(rndCard);
                }
                else
                {
                    rndCard = Record.pile[0];
                    Record.pile.RemoveAt(0);
                }
                this.pile.cards[rndCard.color, rndCard.number]--;
				lblPile.Text = pile.Length - 1 + "";
                players[MovingCard.player].cards[rndCard.color, rndCard.number]++;
                lblCounts[MovingCard.player].Text = PlayersCardsCount(MovingCard.player) + "";
                if (MovingCard.player == 0 && !MovingCard.quickly)
                    Sort();
                if (MovingCard.isPlaying)
                {
                    bool drawAll = false;
                    lblMovingCards[0].Location = new Point(-UnoSize.WIDTH, -UnoSize.HEIGHT);
                    timPileToPlayers.Enabled = false;
draw:
                    if (MovingCard.dbp <= 0 && MovingCard.downpour <= 0)
                    {
                        if (draw > 0)
                        {
                            SetDraw(draw - 1);
                            if (draw == 0)
                                drawAll = true;
                            else if (draw > 0)
                                CheckPile();
                            if (draw <= 0)
                                goto draw;
                            else
                                timPileToPlayers.Enabled = true;
                        }
                        else if (drawColor > 0)
                        {
                            if (rndCard.color == GetColorId(BackColor))
                                SetDrawColor(drawColor - 1);
                            if (drawColor > 0)
                            {
                                CheckPile();
                                if (drawColor <= 0)
                                    goto draw;
                                timPileToPlayers.Enabled = true;
                            }
                        }
                    }
                    if (draw <= 0 && drawColor <= 0 && MovingCard.dbp <= 0 && MovingCard.downpour <= 0)
                    {
                        if (gameOver < 4 && MovingCard.downpour <= 0)
                        {
                            GameOver();
                        }
                        if (MovingCard.dbp <= 0 && MovingCard.downpour <= 0)
                            MovingCard.drew = !MovingCard.unoDraw;
                        if (MovingCard.player == 0 && MovingCard.quickly)
                            Sort();
                        if (MovingCard.unoDraw)
                        {
                            MovingCard.unoDraw = false;
                            PlayersTurn(NextPlayer(MovingCard.player), true, GetDbp());
                        }
                        else if (drawAll)
                        {
                            if (options.mnuDrawAllAndPlay.Checked)
                                PlayersTurn(MovingCard.player);
                            else
                            {
                                MovingCard.drew = false;
                                PlayersTurn(NextPlayer(MovingCard.player), true, GetDbp());
                            }
                        }
                        else if (!options.mnuDrawAndPlay.Checked)
                        {
                            MovingCard.drew = false;
                            PlayersTurn(NextPlayer(MovingCard.player), true, GetDbp());
                        }
                        else
                            PlayersTurn(MovingCard.player, true, 0, MovingCard.player > 0);
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
                    else if (MovingCard.downpour > 0)
                    {
                        MovingCard.downpour--;
                        if (MovingCard.downpour > 0)
                        {
                            CheckPile();
                            timPileToPlayers.Enabled = true;
                        }
                        else
                        {
                            byte nextPlayer = MovingCard.player;
downpour_nextplayer:
                            nextPlayer = (byte)(!reverse ? nextPlayer == 3 ? 0 : nextPlayer + 1 : nextPlayer == 0 ? 3 : nextPlayer - 1);
                            if (nextPlayer == Downpour.player)
                            {
                                if (MovingCard.quickly)
                                    Sort();
                                if (gameOver < 4)
                                {
                                    GameOver();
                                }
                                Downpour.count = 0;
                                PlayersTurn(NextPlayer(nextPlayer), true, GetDbp());
                            }
                            else if (!lblPlayers[nextPlayer].Visible)
                                goto downpour_nextplayer;
                            else
                            {
                                MovingCard.downpour = Downpour.count;
                                MovingCard.player = nextPlayer;
                                timPileToPlayers.Enabled = true;
                            }
                        }
                    }
                }
                else
                {
                    byte lastPlayer = NextPlayer(Record.firstGettingCard, true);
                    if (MovingCard.player == lastPlayer) 
                        if (int.Parse(lblCounts[lastPlayer].Text) >= int.Parse(options.txtDealt.Text)
                            || options.keys.Length > 0)
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

        private void TimPlayersToCenter_Tick(object sender, EventArgs e)
        {
            switch (MovingCard.player)
            {
				case 0:
                    pnlMovingCards.Location = new Point(width / 2 - pnlMovingCards.Width / 2,
                        height - (lblPlayers[0].Top - lblCards[0].Top) / distance * MovingCard.progress - pnlMovingCards.Height / 2 - UnoSize.HEIGHT / 2);
					break;
				case 1:
                    pnlMovingCards.Location = new Point(lblCards[0].Left / distance * MovingCard.progress - pnlMovingCards.Width / 2 + UnoSize.WIDTH / 2,
                        height / 2 - pnlMovingCards.Height / 2);
					break;
				case 2:
                    pnlMovingCards.Location = new Point(width / 2 - pnlMovingCards.Width / 2,
                        lblCards[0].Top / distance * MovingCard.progress - pnlMovingCards.Height / 2 + UnoSize.HEIGHT / 2);
					break;
				case 3:
                    pnlMovingCards.Location = new Point(width - (lblPlayers[MovingCard.player].Left - lblCards[0].Left) / distance * MovingCard.progress - pnlMovingCards.Width / 2 - UnoSize.WIDTH / 2,
                        height / 2 - pnlMovingCards.Height / 2);
					break;
			}
			if (MovingCard.progress >= distance)
                goto arrived;
            MovingCard.progress++;
			return;
arrived:
            timPlayersToCenter.Enabled = false;
            MovingCard.progress = 0;
            pnlMovingCards.Location = new Point(width / 2 - pnlMovingCards.Width / 2, height / 2 - pnlMovingCards.Height / 2);
            RemoveLabel(lblCards);
			AddLabel(lblCards, lblMovingCards.ToArray().Length - 1);
			for (int c = 1; c < lblCards.ToArray().Length; c++)
            {
                SetCard(lblCards[c], GetColorId(lblMovingCards[c].BackColor), GetNumberId(lblMovingCards[c].Text));
				lblCards[c].BringToFront();
                lblCards[c].Location = new Point(lblMovingCards[c].Left + pnlMovingCards.Left, lblMovingCards[c].Top + pnlMovingCards.Top);
			}
			RemoveLabel(lblMovingCards, pnlMovingCards.Controls);
            pnlMovingCards.Location = new Point(-pnlMovingCards.Width, -pnlMovingCards.Height);
            BackColor = GetColor(MovingCard.color);
            lblCounts[MovingCard.player].Text = PlayersCardsCount(MovingCard.player) + "";
            bool reversed = false;
            byte ons = 0;
            foreach (Label p in lblPlayers)
                if (p.Visible)
                    ons++;
            switch (lblCards[1].Text)
            {
                case "0" when options.mnuSevenZero.Checked:
                    TradingHands.player1 = MovingCard.player;
                    TradingHands.player2 = 4;
                    goto end_action;
                case "7" when options.mnuSevenZero.Checked:
                case UnoNumberName.TRADE_HANDS when options.mnuTradeHands.Checked:
                    if (lblPlayers[MovingCard.player].Visible)
                    {
                        TradingHands.player1 = MovingCard.player;
                        if (!options.isPlayingRecord)
                        {
                            if (MovingCard.player != 0 || isAutomatic)
                            {
                                byte fp = MovingCard.player, p = NextPlayer(MovingCard.player);
                                int fc = int.MaxValue;
                                while (p != MovingCard.player)
                                {
                                    int c = PlayersCardsCount(p);
                                    if (c < fc)
                                    {
                                        fc = c;
                                        fp = p;
                                    }
                                    p = NextPlayer(p);
                                }
                                TradingHands.player2 = fp;
                                Record.tradeHands.Add(TradingHands.player2);
                            }
                            else
                                TradingHands.player2 = 5;
                        }
                        else
                        {
                            TradingHands.player2 = Record.tradeHands[0];
                            Record.tradeHands.RemoveAt(0);
                        }
                    }
                    goto end_action;
            }
            for (int i = 1; i < lblCards.Count; i++)
            {
                Label l = lblCards[i];
                if (l.Text == UnoNumberName.REVERSE || l.Text == options.txtBlankText.Text && options.mnuBlankReverse.Checked)
                {
                    if (ons == 2 && lblPlayers[MovingCard.player].Visible)
                        reversed = true;
                    else
                        reverse = !reverse;
                }
            }
            for (int i = 1; i < lblCards.Count; i++)
            {
                Label l = lblCards[i];
                if (l.Text == UnoNumberName.NULL)
                    continue;
                switch (l.Text)
                {
                    case UnoNumberName.SKIP:
                        if (options.mnuSkipPlayers.Checked) skip++;
                        else skips[NextPlayer(MovingCard.player)]++;
                        break;
                    case UnoNumberName.SKIP_EVERYONE:
                        if (lblPlayers[MovingCard.player].Visible)
                            reversed = true;
                        break;
                    case UnoNumberName.REVERSE:
                        break;
                    case UnoNumberName.DRAW_1:
                        AddDraw(1);
                        break;
                    case UnoNumberName.DRAW_2:
                        AddDraw(2);
                        break;
                    case UnoNumberName.DRAW_5:
                        AddDraw(5);
                        break;
                    case UnoNumberName.WILD_DOWNPOUR_DRAW_1:
                        Downpour.count += options.mnuDoubleDraw.Checked ? 2 : 1;
                        break;
                    case UnoNumberName.WILD_DOWNPOUR_DRAW_2:
                        Downpour.count += 2 * (options.mnuDoubleDraw.Checked ? 2 : 1);
                        break;
                    case UnoNumberName.WILD_DOWNPOUR_DRAW_4:
                        Downpour.count += 4 * (options.mnuDoubleDraw.Checked ? 2 : 1);
                        break;
                    case UnoNumberName.WILD_DRAW_4:
                        AddDraw(4);
                        break;
                    case UnoNumberName.WILD_DRAW_COLOR:
                        SetDrawColor(drawColor + 1);
                        break;
                    case UnoNumberName.WILD_HITFIRE:
                        SetDraw(int.Parse(lblPile.Text));
                        break;
                    default:
                        if (l.Text == options.txtBlankText.Text)
                        {
                            if (options.mnuSkipPlayers.Checked)
                                skip += int.Parse(options.txtBlankSkip.Text);
                            else
                                skips[NextPlayer(MovingCard.player)] += int.Parse(options.txtBlankSkip.Text);
                            AddDraw(int.Parse(options.txtBlankDraw.Text));
                            Downpour.count += int.Parse(options.txtBlankDownpourDraw.Text) * (options.mnuDoubleDraw.Checked ? 2 : 1);
                        }
                        break;
                }
            }
end_action:
            if (gameOver < 4)
            {
                if (Downpour.count > 0)
                {
                    DownpourDraw(MovingCard.player);
                }
                return;
            }
            if (options.mnuUno.Checked && MovingCard.player == 0 && (PlayersCardsCount(0) == 1 && !rdoUno.Checked || PlayersCardsCount(0) != 1 && rdoUno.Checked))
            {
                Action(0, "UNO? +2");
				AddDraw(2);
				PlayersTurn(0, false);
				rdoUno.Checked = false;
                MovingCard.unoDraw = true;
			}
            MovingCard.drew = false;
            if (MovingCard.unoDraw)
                timUno.Enabled = true;
            else if (reversed)
                PlayersTurn(MovingCard.player, true, GetDbp());
            else if (TradingHands.player1 < 4)
                PlayersTurn(MovingCard.player, true, GetDbp());
            else if (Downpour.count > 0)
            {
                PlayersTurn(MovingCard.player, true, GetDbp());
            }
            else
                PlayersTurn(MovingCard.player = NextPlayer(MovingCard.player), true, GetDbp());
        }

        private void TimThinking_Tick(object sender, EventArgs e)
        {
            timThinking.Enabled = false;
            string[] tag = timThinking.Tag.ToString().Split(char.Parse(","));
            PlayersTurn(byte.Parse(tag[0]), true, int.Parse(tag[1]));
        }

        private void TimTradeHands_Tick(object sender, EventArgs e)
        {
            if (TradingHands.player2 < 4)
            {
                int x1 = TradingHands.x[TradingHands.player1], x2 = TradingHands.x[TradingHands.player2],
                    y1 = TradingHands.y[TradingHands.player1], y2 = TradingHands.y[TradingHands.player2];
                lblPlayers[TradingHands.player1].Location = new Point(x1 + (x2 - x1) / distance * MovingCard.progress,
                                                                      y1 + (y2 - y1) / distance * MovingCard.progress);
                lblPlayers[TradingHands.player2].Location = new Point(x1 + (x2 - x1) / distance * (distance - MovingCard.progress),
                                                                      y1 + (y2 - y1) / distance * (distance - MovingCard.progress));
            }
            else
            {
                int[] x = TradingHands.x.ToArray(), y = TradingHands.y.ToArray();
                for (byte p = 0; p <= 3; p++)
                {
                    if (lblPlayers[p].Visible)
                    {
                        byte np = NextPlayer(p);
                        lblPlayers[p].Location = new Point(x[p] + (x[np] - x[p]) / distance * MovingCard.progress,
                                                           y[p] + (y[np] - y[p]) / distance * MovingCard.progress);
                    }
                }
            }
            if (MovingCard.progress >= distance)
                goto arrived;
            MovingCard.progress++;
            return;
arrived:
            timTradeHands.Enabled = false;
            MovingCard.progress = 0;
            lblPlayers[0].Location = new Point(width / 2 - UnoSize.WIDTH / 2, ClientRectangle.Height - UnoSize.HEIGHT);
            lblPlayers[1].Location = new Point(0, height / 2 - UnoSize.HEIGHT / 2 + mnuGame.Height / 2);
            lblPlayers[2].Location = new Point(width / 2 - UnoSize.WIDTH / 2, mnuGame.Height);
            lblPlayers[3].Location = new Point(width - UnoSize.WIDTH, height / 2 - UnoSize.HEIGHT / 2 + mnuGame.Height / 2);
            if (TradingHands.player2 < 4)
            {
                int[,] i = players[TradingHands.player1].cards;
                players[TradingHands.player1].cards = players[TradingHands.player2].cards;
                players[TradingHands.player2].cards = i;
            }
            else
            {
                byte pp, p = MovingCard.player;
                int[,] i = players[MovingCard.player].cards;
                pp = NextPlayer(p, true);
                do
                {
                    players[p].cards = players[pp].cards;
                    p = pp;
                }
                while ((pp = NextPlayer(p, true)) != MovingCard.player);
                players[p].cards = i;
            }
            if (TradingHands.player1 == 0 || TradingHands.player2 == 0 || TradingHands.player2 == 4)
            {
                lblPlayers[0].BackgroundImage = null;
                if (mnuPicPlayer.Checked)
                    picPlayer.Visible = true;
                Sort();
                pnlPlayer.BringToFront();
                picPlayer.BringToFront();
                lblCounts[0].Text = PlayersCardsCount(0).ToString();
            }
            TradingHands.player1 = 4;
            for (byte p = 1; p <= 3; p++)
            {
                if (options.mnuRevealable.Checked)
                    Reveal(p);
                lblCounts[p].Text = PlayersCardsCount(p).ToString();
            }
            PlayersTurn(NextPlayer(MovingCard.player), true, GetDbp());
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
            int h = (int)(gametime / 3600f), m = (int)(gametime / 60f);
            lblWatch.Text = h + "°" + (m - h * 60) + "′" + (gametime - m * 60) + "″";
        }

        private void ToolTip_Popup(object sender, PopupEventArgs e)
        {
        }

        void TradeHands()
        {
            TradingHands.x = new List<int>(lblPlayers.Cast<Label>().Select(l => l.Left));
            TradingHands.y = new List<int>(lblPlayers.Cast<Label>().Select(l => l.Top));
            MovingCard.progress = 0;
            MovingCard.quickly = false;
            if (TradingHands.player1 == 0 || TradingHands.player2 == 0 || TradingHands.player2 == 4)
            {
                if (mnuPicPlayer.Checked)
                {
                    picPlayer.Visible = false;
                }    
                else
                    RemoveChkPlayer();
                lblPlayers[0].BackgroundImage = Properties.Resources.uno_back;
                lblPlayers[0].BringToFront();
            }
            for (byte p = 1; p <= 3; p++)
                lblPlayers[p].BringToFront();
            timTradeHands.Enabled = true;
        }
        public Uno(Options options)
        {
            InitializeComponent();
            height = ClientRectangle.Height - mnuGame.Height;
            this.options = options;
            imgUno = Properties.Resources.uno;
            Graphics gpsUno = Graphics.FromImage(imgUno);
            Brush brush = new SolidBrush(Color.Black);
            string text = options.txtBlankText.Text;
            Font font = new Font(new FontFamily("MS Gothic"), 84f);
            float fontSize = 84f;
            SizeF ms = gpsUno.MeasureString(text, font);
            while (ms.Width > ResourceUnoSize.WIDTH && fontSize >= 21)
            {
                fontSize--;
                font = new Font(font.FontFamily, fontSize);
                ms = gpsUno.MeasureString(text, font);
            }
            for (byte b = 0; b <= UnoColor.MAX_VALUE; b++)
                gpsUno.DrawString(text, font, brush, new RectangleF(UnoNumber.BLANK * ResourceUnoSize.WIDTH, b * ResourceUnoSize.HEIGHT, ResourceUnoSize.WIDTH, ResourceUnoSize.HEIGHT), new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            lblPile.Top = mnuGame.Height;
            lblPile.BackgroundImageLayout = ImageLayout.Stretch;
            lblPile.BackgroundImage = Properties.Resources.uno_pile;
            isFair = options.mnuBotPro.Checked;
            for (byte b = 0; b < 4; b++)
            {
                lblPlayers[b] = new Label();
                Controls.Add(lblPlayers[b]);
                lblPlayers[b].AutoSize = options.mnuRevealable.Checked;
                lblPlayers[b].BackgroundImageLayout = ImageLayout.Stretch;
                if (b > 0 && !options.mnuRevealable.Checked)
                    lblPlayers[b].BackgroundImage = Properties.Resources.uno_back;
                lblPlayers[b].ForeColor = Color.White;
                lblPlayers[b].Tag = b;
                lblPlayers[b].TextAlign = ContentAlignment.MiddleCenter;
                lblPlayers[b].Size = new Size(UnoSize.WIDTH, UnoSize.HEIGHT);
                lblPlayers[b].Tag = b;
                if (b > 0) lblPlayers[b].BorderStyle = BorderStyle.FixedSingle;
                lblPlayers[b].BackColorChanged += new EventHandler(Control_BackColorChanged);
                lblCounts[b] = new Label();
                Controls.Add(lblCounts[b]);
                lblCounts[b].AutoSize = true;
                lblCounts[b].TextAlign = ContentAlignment.MiddleCenter;
                lblCounts[b].Tag = b;
                lblCounts[b].Text = "0";
                lblCounts[b].BackColorChanged += new EventHandler(Control_BackColorChanged);
                lblCounts[b].SizeChanged += LblCounts_SizeChanged;
                lblCounts[b].TextChanged += new EventHandler(LblCounts_TextChanged);
            }
            lblPlayers[0].BackColor = Color.Transparent;
            lblPlayers[0].Text = "";
            mnuMoney.Text = Format(options.money);
            if (options.mnuMultiply0.Checked || options.multiplier <= 0 || options.isPlayingRecord)
            {
                options.multiplier = 0;
            }
            bool[] ons;
            switch (options.ons)
            {
                case 3:
                    ons = new bool[4] { options.on0, true, true, true };
                    PLAYER_NAMES = new string[] { options.txtPlayer0.Text, options.txtPlayer1.Text, options.txtPlayer2.Text, options.txtPlayer3.Text };
                    break;
                case 2:
                    ons = new bool[4] { options.on0, true, false, true };
                    PLAYER_NAMES = new string[] { options.txtPlayer0.Text, options.txtPlayer1.Text, "" , options.txtPlayer2.Text};
                    break;
                case 1:
                    ons = new bool[4] { options.on0, false, true, false };
                    PLAYER_NAMES = new string[] { options.txtPlayer0.Text, "", options.txtPlayer1.Text, "" };
                    break;
                default:
                    ons = new bool[4] { options.on0, false, false, false };
                    PLAYER_NAMES = new string[] { options.txtPlayer0.Text, "", "", ""};
                    break;
            }
            for (int o = 0; o <= 3; o++)
            {
                lblPlayers[o].Visible = ons[o];
                lblCounts[o].Visible = ons[o];
            }
            lblCards.Add(new Label());
            Controls.Add(lblCards[0]);
            lblCards[0].Visible = false;
            lblCards[0].BackColor = Color.White;
            if (!options.mnuMultiply0.Checked)
            {
                btnStart.Text = $"開始 (-{Format(options.multiplier)})";
            }
            ResizeForm();
            SetInterval(options.animation);
            if (options.mnuPairs.Checked)
            {
                mnuByColor.Checked = false;
                mnuByNumber.Checked = true;
            }
            if (options.mnuCyan.Checked && options.mnuMagenta.Checked)
            {
                colorList = new byte[6]
                {
                    UnoColor.RED, UnoColor.YELLOW, UnoColor.GREEN, UnoColor.CYAN, UnoColor.BLUE, UnoColor.MAGENTA
                };
            }
            else if (options.mnuCyan.Checked)
            {
                colorList = new byte[5]
                {
                    UnoColor.RED, UnoColor.YELLOW, UnoColor.GREEN, UnoColor.CYAN, UnoColor.BLUE
                };
            }
            else if (options.mnuMagenta.Checked)
            {
                colorList = new byte[5]
                {
                    UnoColor.RED, UnoColor.YELLOW, UnoColor.GREEN, UnoColor.BLUE, UnoColor.MAGENTA
                };
            }
            RefillPile();
            for (byte p = 0; p < 4; p++)
                players[p] = new Cards();
            if (options.mnuSevenZero.Checked)
            {
                List<byte> l = new List<byte>(playlist);
                l.RemoveAt(l.IndexOf(7));
                l.RemoveAt(l.IndexOf(0));
                l.Insert(l.IndexOf(UnoNumber.WILD), 7);
                l.Insert(l.IndexOf(UnoNumber.WILD), 0);
            }
            AddLabel(lblMovingCards);
            lblMovingCards[0].BackgroundImageLayout = ImageLayout.Stretch;
            lblMovingCards[0].BackgroundImage = Properties.Resources.uno_back;
            lblMovingCards[0].BringToFront();
            if (options.isPlayingRecord)
            {
                LoadRecord();
                mnuChat.Visible = false;
                mnuAuto.Visible = false;
                itmHelp.Visible = false;
                isAutomatic = true;
                mnuPlayPause.Visible = true;
                mnuStop.Visible = true;
                mnuForward.Visible = true;
            }

        }

        private void Uno_Click(object sender, EventArgs e)
        {
            MovingCard.quickly = true;
            if (isPlayer0sTurn && MovingCard.drew && !options.mnuDrawToMatch.Checked)
                Draw(0);
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

        private void Uno_Resize(object sender, EventArgs e)
        {
            ResizeForm();
        }
    }
}
