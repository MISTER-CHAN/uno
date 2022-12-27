using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web.Security;
using System.Windows.Forms;

namespace Uno
{
    public partial class Uno : Form
    {
        private static readonly byte[]
            mvcList = new byte[UnoNumber.MaxValue]
            {
                UnoNumber.WildHitfire,
                UnoNumber.WildDrawColor,
                UnoNumber.WildDraw4,
                UnoNumber.WildDownpourDraw4,
                UnoNumber.WildDownpourDraw2,
                UnoNumber.WildDownpourDraw1,
                UnoNumber.Wild,
                UnoNumber.Blank,
                UnoNumber.DiscardAll,
                UnoNumber.Number,
                UnoNumber.Draw5,
                UnoNumber.Draw2,
                UnoNumber.Draw1,
                UnoNumber.SkipEveryone,
                UnoNumber.Skip,
                UnoNumber.Reverse,
                0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
                UnoNumber.TradeHands
            },
            playlist = new byte[UnoNumber.MaxValue]
            {
                UnoNumber.SkipEveryone,
                UnoNumber.DiscardAll,
                UnoNumber.Skip,
                UnoNumber.Reverse,
                UnoNumber.Draw1,
                UnoNumber.Draw2,
                UnoNumber.Draw5,
                10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0,
                UnoNumber.Number,
                UnoNumber.Blank,
                UnoNumber.TradeHands,
                UnoNumber.Wild,
                UnoNumber.WildDownpourDraw1,
                UnoNumber.WildDownpourDraw2,
                UnoNumber.WildDownpourDraw4,
                UnoNumber.WildDraw4,
                UnoNumber.WildDrawColor,
                UnoNumber.WildHitfire
            },
            wildPlaylist = new byte[UnoNumber.MaxValue]
            {
                UnoNumber.Wild,
                2,
                UnoNumber.Blank,
                UnoNumber.WildDownpourDraw1,
                UnoNumber.WildDownpourDraw2,
                UnoNumber.WildDownpourDraw4,
                UnoNumber.Draw2,
                UnoNumber.WildDraw4,
                UnoNumber.WildDrawColor,
                UnoNumber.WildHitfire,
                UnoNumber.Reverse,
                UnoNumber.SkipEveryone,
                UnoNumber.DiscardAll,
                UnoNumber.Skip,
                UnoNumber.Draw1,
                UnoNumber.Draw5,
                10, 9, 8, 7, 6, 5, 4, 3, 1, 0,
                UnoNumber.Number,
                UnoNumber.TradeHands
            };

        private bool canPlay = false, hasCheat = false, isAutomatic = false, isFair = false, isPlayer0sTurn = false, isSelectingCards = false, reverse = false;
        byte gameOver = 4;
        readonly Cards pile = new();
        readonly Cards[] players = new Cards[4];
        private readonly Downpour downpour = new();
        Graphics gpsPlayer;
        Image imgPlayer;
        readonly Image imgUno;
        private int distance = 20, draw = 0, drawColor = 0, gametime = 0, height = 0, pointing = -1, skip = 0, swpcw = 0, width = 0;
        readonly int[] skips = new int[4];
        readonly Label[] lblCounts = new Label[4];
        public Label[] lblPlayers = new Label[4];
        readonly List<CheckBox> chkPlayer = new();
        private readonly List<Label> lblCards = new();
        private readonly List<Label> lblMovingCards = new();
        private readonly MovingCard movingCard = new();
        readonly Options options;
        private readonly PicPlayer _picPlayer = new();
        private readonly Record record = new();
        readonly string[] playerNames;
        private readonly TradingHands tradingHands = new();

        readonly byte[] colorList = new byte[4]
        {
            UnoColor.Red, UnoColor.Yellow, UnoColor.Green, UnoColor.Blue
        };

        private void Action(byte player, string msg)
        {
            lblAction.Text = msg;
            lblAction.BringToFront();
            switch (player)
            {
                case 0: lblAction.Location = new Point((width >> 1) - (lblAction.Width >> 1), lblCounts[0].Top - lblAction.Height); break;
                case 1: lblAction.Location = new Point(lblPlayers[1].Left + UnoSize.Width, (height >> 1) - (lblAction.Height >> 1)); break;
                case 2: lblAction.Location = new Point((width >> 1) - (lblAction.Width >> 1), lblCounts[2].Top + lblCounts[2].Height); break;
                case 3: lblAction.Location = new Point(lblPlayers[3].Left - lblAction.Width, (height >> 1) - (lblAction.Height >> 1)); break;
            }
        }

        private void AddChkPlayer(int count = 1)
        {
            for (int i = 1; i <= count; ++i)
            {
                int length = chkPlayer.Count;
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
                chkPlayer[length].Size = new Size(UnoSize.Width, UnoSize.Height);
                chkPlayer[length].Tag = length;
                chkPlayer[length].TextAlign = ContentAlignment.MiddleCenter;
            }
        }

        private void AddDraw(int count)
        {
            SetDraw(draw + (options.mnuDoubleDraw.Checked ? 2 : 1) * count);
        }

        private void AddLabel(List<Label> label, int count = 1, Control.ControlCollection controls = null)
        {
            controls ??= Controls;
            for (int c = 1; c <= count; ++c)
            {
                int length = label.Count;
                label.Add(new Label());
                controls.Add(label[length]);
                label[length].AutoSize = false;
                label[length].BackColor = Color.White;
                label[length].BackgroundImageLayout = ImageLayout.Stretch;
                label[length].BorderStyle = BorderStyle.FixedSingle;
                label[length].BringToFront();
                label[length].ForeColor = Color.White;
                label[length].Font = new Font(new FontFamily("MS Gothic"), 42);
                label[length].Location = new Point(-UnoSize.Width, -UnoSize.Height);
                label[length].MouseLeave += Label_MouseLeave;
                label[length].Size = new Size(UnoSize.Width, UnoSize.Height);
                label[length].Text = UnoNumberName.Null;
                label[length].TextAlign = ContentAlignment.MiddleCenter;
            }
        }

        private Card[] Ai(byte player, bool skipFirstCheating = false)
        {
            if (player > 0 && options.mnuCheater.Checked
                || player == 0 && isFair)
            {
                if (!skipFirstCheating && options.mnuPairs.Checked)
                    CheatPairs(player);
            }
            bool gbp = int.Parse(lblCounts[player].Text) > 7 && GetDbp() > 0;
            byte backColor = GetColorId(BackColor), backNumber = GetNumberId(lblCards[1].Text);
            Card bestCard = new();
            List<Card> cards = new();
            if (!options.mnuStacking.Checked && draw > 0)
            {
                return cards.ToArray();
            }
            int quantityColor, quantityNumber = 0;
            if (backNumber == UnoNumber.Number)
            {
                if (!gbp)
                    quantityColor = GetQuantityByColor(player, backColor);
                else
                    quantityColor = GetColorQuantity(player, backColor);
                for (byte b = 10; b >= 0 && b < byte.MaxValue; --b)
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
                    int q = GetColorQuantity(player, UnoColor.Black);
                    if (q > quantityColor)
                        quantityColor = q;
                }
                if (backNumber < UnoNumber.Wild)
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
                && lblCards[1].Text == UnoNumberName.Draw1 && draw >= 1)
            {
                if (quantityNumber > 0)
                    bestCard.number = UnoNumber.Draw1;
                else if (players[player].cards[UnoColor.Black, UnoNumber.Draw2] > 0)
                {
                    bestCard.color = UnoColor.Black;
                    bestCard.number = UnoNumber.Draw2;
                }
                else if (players[player].cards[backColor, UnoNumber.Draw2] > 0)
                    bestCard.number = UnoNumber.Draw2;
                else if (players[player].cards[backColor, UnoNumber.Draw5] > 0)
                    bestCard.number = UnoNumber.Draw5;
                else if (players[player].cards[UnoColor.Black, UnoNumber.WildDraw4] > 0)
                {
                    bestCard.color = UnoColor.Black;
                    bestCard.number = UnoNumber.WildDraw4;
                }
                else if (options.mnuWildPunch.Checked
                    && players[player].cards[UnoColor.Black, UnoNumber.Reverse] > 0)
                {
                    bestCard.color = UnoColor.Black;
                    bestCard.number = UnoNumber.Reverse;
                }
            }
            else if (!options.mnuPlayOrDrawAll.Checked
                && lblCards[1].Text == UnoNumberName.Draw2 && draw >= 2)
            {
                if (players[player].cards[UnoColor.Black, UnoNumber.Draw2] > 0)
                {
                    bestCard.color = UnoColor.Black;
                    bestCard.number = UnoNumber.Draw2;
                }
                else if (quantityNumber > 0)
                    bestCard.number = UnoNumber.Draw2;
                else if (players[player].cards[backColor, UnoNumber.Draw5] > 0)
                    bestCard.number = UnoNumber.Draw5;
                else if (players[player].cards[UnoColor.Black, UnoNumber.WildDraw4] > 0)
                {
                    bestCard.color = UnoColor.Black;
                    bestCard.number = UnoNumber.WildDraw4;
                }
                else if (options.mnuWildPunch.Checked
                    && players[player].cards[UnoColor.Black, UnoNumber.Reverse] > 0)
                {
                    bestCard.color = UnoColor.Black;
                    bestCard.number = UnoNumber.Reverse;
                }
            }
            else if (!options.mnuPlayOrDrawAll.Checked
                && lblCards[1].Text == UnoNumberName.Draw5 && draw >= 5)
            {
                if (quantityNumber > 0)
                    bestCard.number = UnoNumber.Draw5;
                else if (players[player].cards[UnoColor.Black, UnoNumber.WildDraw4] > 0)
                {
                    bestCard.color = UnoColor.Black;
                    bestCard.number = UnoNumber.WildDraw4;
                }
                else if (options.mnuWildPunch.Checked
                    && players[player].cards[UnoColor.Black, UnoNumber.Reverse] > 0)
                {
                    bestCard.color = UnoColor.Black;
                    bestCard.number = UnoNumber.Reverse;
                }
            }
            else if (!options.mnuPlayOrDrawAll.Checked
                && lblCards[1].Text == UnoNumberName.WildDraw4 && draw >= 4)
            {
                if (players[player].cards[UnoColor.Black, UnoNumber.WildDraw4] > 0)
                {
                    bestCard.color = UnoColor.Black;
                    bestCard.number = UnoNumber.WildDraw4;
                }
                else if (options.mnuWildPunch.Checked
                    && players[player].cards[UnoColor.Black, UnoNumber.Reverse] > 0)
                {
                    bestCard.color = UnoColor.Black;
                    bestCard.number = UnoNumber.Reverse;
                }
            }
            else if (!options.mnuPlayOrDrawAll.Checked
                && lblCards[1].Text == UnoNumberName.WildDrawColor && drawColor > 0)
            {
                if (players[player].cards[UnoColor.Black, UnoNumber.WildDrawColor] > 0)
                {
                    bestCard.color = UnoColor.Black;
                    bestCard.number = UnoNumber.WildDrawColor;
                }
                else if (options.mnuWildPunch.Checked
                    && players[player].cards[UnoColor.Black, UnoNumber.Reverse] > 0)
                {
                    bestCard.color = UnoColor.Black;
                    bestCard.number = UnoNumber.Reverse;
                }
            }
            else if (!options.mnuPlayOrDrawAll.Checked
                && lblCards[1].Text == UnoNumberName.WildHitfire && draw > 0)
            {
                if (players[player].cards[UnoColor.Black, UnoNumber.WildHitfire] > 0)
                {
                    bestCard.color = UnoColor.Black;
                    bestCard.number = UnoNumber.WildHitfire;
                }
                else if (options.mnuWildPunch.Checked
                    && players[player].cards[UnoColor.Black, UnoNumber.Reverse] > 0)
                {
                    bestCard.color = UnoColor.Black;
                    bestCard.number = UnoNumber.Reverse;
                }
            }
            else if (options.mnuWildPunch.Checked && !options.mnuPlayOrDrawAll.Checked
                && lblCards[1].Text == UnoNumberName.Reverse && lblCards.Last().BackColor == Color.Black && draw + drawColor > 0)
            {
                if (players[player].cards[UnoColor.Black, UnoNumber.Reverse] > 0)
                {
                    bestCard.color = UnoColor.Black;
                    bestCard.number = UnoNumber.Reverse;
                }
            }
            else if (quantityColor == 0 && quantityNumber == 0)
            {

                if (backNumber <= 10 && GetNumberQuantity(player, UnoNumber.Number) > 0)
                {
                    bestCard.number = UnoNumber.Number;
                }
                else if (players[player].cards[UnoColor.Magenta, UnoNumber.Blank] > 0)
                {
                    bestCard.color = UnoColor.Magenta;
                    bestCard.number = UnoNumber.Blank;
                }
                else
                    for (byte n = 0; n <= UnoNumber.MaxValue; ++n)
                        if (players[player].cards[UnoColor.Black, n] > 0)
                        {
                            bestCard.color = UnoColor.Black;
                            bestCard.number = n;
                            break;
                        }
            }
            else if (quantityColor >= quantityNumber)
            {
                if (gbp)
                {
                    int mq = 0;
                    for (byte b = 0; b < UnoNumber.Wild; ++b)
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
                    for (byte b = 0; b < UnoNumber.MaxValue; ++b)
                    {
                        byte n = wildPlaylist[b];
                        if (players[player].cards[UnoColor.Black, n] > 0)
                        {
                            int q = GetNumberQuantity(player, n);
                            if (q > mq)
                            {
                                mq = q;
                                bestCard.color = UnoColor.Black;
                                bestCard.number = n;
                            }
                        }
                    }
                }
                else if (!options.mnuPairs.Checked)
                {
                    int mq = 0;
                    for (byte b = 0; b < UnoNumber.Wild; ++b)
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
                    for (byte b = 0; b < UnoNumber.Wild; ++b)
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
            if (players[player].cards[UnoColor.Black, bestCard.number] <= 0)
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
                                movingCard.color = color;
                            }
                            Card card = new()
                            {
                                color = color,
                                number = bestCard.number
                            };
                            for (byte c = 1; c <= players[player].cards[color, card.number]; ++c)
                                cards.Add(card);
                        }
                    if (bestCard.number == UnoNumber.Blank && players[player].cards[UnoColor.Magenta, UnoNumber.Blank] > 0)
                    {
                        if (cards.Count == 0)
                            movingCard.color = backColor;
                        Card card = new()
                        {
                            color = UnoColor.Magenta,
                            number = UnoNumber.Blank
                        };
                        for (byte c = 1; c <= players[player].cards[UnoColor.Magenta, UnoNumber.Blank]; ++c)
                            cards.Add(card);
                    }
                }
                else
                {
                    if (bestCard.color == UnoColor.White)
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
                                    movingCard.color = c;
                                }
                            }
                        }
                        bestCard.color = movingCard.color;
                        if (bestCard.number < UnoNumber.MaxValue)
                        {
                            cards.Add(bestCard);
                        }
                    }
                    else if (players[player].cards[bestCard.color, bestCard.number] > 0)
                    {
                        movingCard.color = bestCard.color == UnoColor.Magenta ? backColor : bestCard.color;
                        cards.Add(bestCard);
                    }
                }
                if (bestCard.number == UnoNumber.Number)
                {
                    List<Card> numbers = new();
                    byte number;
                    for (byte n = 10; n >= 0 && n < byte.MaxValue; --n)
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
                        for (int i = 1; i <= players[player].cards[c, number]; ++i)
                        {
                            Card card = new() { color = c, number = number };
                            numbers.Add(card);
                        }
                    }
                    numbers.AddRange(cards);
                    cards = numbers;
                }
                else if (bestCard.number == UnoNumber.DiscardAll)
                {
                    byte color = lblCards[1].Text == UnoNumberName.DiscardAll ? movingCard.color : backColor;
                    for (byte n = 0; n <= UnoNumber.MaxValue; ++n)
                    {
                        if (n == UnoNumber.DiscardAll)
                            continue;
                        for (int c = 1; c <= players[player].cards[color, n]; ++c)
                        {
                            Card card = new()
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
                        List<Card> number = new();
                        foreach (byte c in colorList)
                        {
                            for (byte u = 1; u <= players[player].cards[c, UnoNumber.Number]; ++u)
                            {
                                Card card = new()
                                {
                                    color = c,
                                    number = UnoNumber.Number
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
                bestCard.color = UnoColor.Black;
                int mp = 0, mq = 0;
                foreach (byte c in colorList)
                {
                    int q = GetQuantityByColor(player, c);
                    int p = GetPointsByColor(player, c);
                    if (q > mq || q == mq && p > mp)
                    {
                        mq = q;
                        movingCard.color = c;
                    }
                }
                if (options.mnuPairs.Checked)
                {
                    for (byte c = 0; c <= UnoColor.MaxValue - 1; ++c)
                        for (byte u = 1; u <= players[player].cards[c, bestCard.number]; ++u)
                        {
                            Card card = new()
                            {
                                color = c,
                                number = bestCard.number
                            };
                            cards.Add(card);
                        }
                }
                else if (players[player].cards[UnoColor.Black, bestCard.number] > 0)
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
                            List<byte> colors = new();
                            List<byte> allColors = new() { 0, 1, 2, 3, 4, 5, 6 };
                            allColors.Remove(bestCard.color);
                            allColors.Insert(0, bestCard.color);
                            foreach (byte c in allColors)
                            {
                                for (int i = 1; i <= pile.cards[c, bestCard.number]; ++i)
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
                movingCard.player = (byte)(4f * Rnd());
                if (!lblPlayers[movingCard.player].Visible)
                    goto rnd;
                record.firstGettingCard = movingCard.player;
            }
            else
                movingCard.player = record.firstGettingCard;
            movingCard.quickly = false;
            lblMovingCards[0].BringToFront();
            timPileToPlayers.Enabled = true;
        }

        private bool CanPlay(List<Card> card, byte color)
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
                    case UnoNumberName.Draw1:
                        if (draw >= 1)
                            if (card[0].number != UnoNumber.Draw1
                                && card[0].number != UnoNumber.Draw2
                                && card[0].number != UnoNumber.Draw5
                                && card[0].number != UnoNumber.WildDraw4
                                && (card[0].number != UnoNumber.Reverse || card.Last().color != UnoColor.Black))
                                goto deny;
                        break;
                    case UnoNumberName.Draw2:
                        if (draw >= 2)
                            if (card[0].number != UnoNumber.Draw2
                                && card[0].number != UnoNumber.Draw5
                                && card[0].number != UnoNumber.WildDraw4
                                && (card[0].number != UnoNumber.Reverse || card.Last().color != UnoColor.Black))
                                goto deny;
                        break;
                    case UnoNumberName.Draw5:
                        if (draw >= 5)
                            if (card[0].number != UnoNumber.Draw5
                                && card[0].number != UnoNumber.WildDraw4
                                && (card[0].number != UnoNumber.Reverse || card.Last().color != UnoColor.Black))
                                goto deny;
                        break;
                    case UnoNumberName.WildDraw4:
                        if (draw >= 4 && card[0].number != UnoNumber.WildDraw4
                            && (card[0].number != UnoNumber.Reverse || card.Last().color != UnoColor.Black))
                            goto deny;
                        break;
                    case UnoNumberName.WildDrawColor:
                        if (drawColor > 0 && card[0].number != UnoNumber.WildDrawColor
                            && (card[0].number != UnoNumber.Reverse || card.Last().color != UnoColor.Black))
                            goto deny;
                        break;
                    case UnoNumberName.WildHitfire:
                        if (draw > 0 && card[0].number != UnoNumber.WildHitfire
                            && (card[0].number != UnoNumber.Reverse || card.Last().color != UnoColor.Black))
                            goto deny;
                        break;
                    case UnoNumberName.Reverse when lblCards.Last().BackColor == Color.Black:
                        if (draw + drawColor > 0 && (card[0].number != UnoNumber.Reverse || card.Last().color != UnoColor.Black))
                            goto deny;
                        break;
                }
            }
            if (options.mnuAttack.Checked)
            {
                if (card[0].number != UnoNumber.DiscardAll)
                    goto wild_number;
                byte discardColor = UnoColor.MaxValue;
                List<byte> discardColors = new();
                if (lblCards[1].Text != UnoNumberName.DiscardAll)
                    discardColor = backColor;
                foreach (Card c in card)
                {
                    switch (c.number)
                    {
                        case UnoNumber.DiscardAll:
                            discardColors.Add(c.color);
                            break;
                        default:
                            if (discardColor == UnoColor.MaxValue && discardColors.Contains(c.color))
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
                if (!card.Select(b => b.number).Contains(UnoNumber.Number))
                    goto number;
                byte n = backNumber;
                foreach (Card c in card)
                {
                    if (c.color == backColor || c.color == UnoColor.Black)
                    {
                        n = UnoNumber.MaxValue;
                        break;
                    }
                }
                if (n > 10 && n < UnoNumber.MaxValue)
                    goto number;
                foreach (Card c in card)
                {
                    if (c.number <= 10)
                    {
                        if (n == UnoNumber.MaxValue)
                            n = c.number;
                        else if (c.number != n && c.color != UnoColor.Black)
                            goto number;
                    }
                    else if (c.number != UnoNumber.Number)
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
            for (int c = 0; c < card.Count; ++c)
                if (card[c].color == UnoColor.Black)
                    goto accept;
            for (int c = 0; c < card.Count; ++c) if (card[c].color != UnoColor.Magenta) goto color;
            goto accept;
        color:
            for (int c = 0; c < card.Count; ++c) if (card[c].color == color) goto btn;
            goto deny;
        btn:
            for (int c = 0; c < card.Count; ++c)
            {
                if (card[c].color == backColor || card[c].color == UnoColor.Magenta)
                    goto accept;
            }
            if (card[0].number == backNumber || backNumber == UnoNumber.Number && card[0].number <= 10)
                goto accept;
            goto deny;
        accept:
            return true;
        deny:
            Action(0, "你不能这样出牌");
            return false;
        }

        private void Card_Enter(object sender, EventArgs e)
        {
            toolTip.ToolTipTitle = GetNumberName(GetNumberId(((Control)sender).Text));
        }

        private void CheatPairs(byte player)
        {
        begin_hacking:
            byte mn = UnoNumber.MaxValue, mnc = UnoColor.MaxValue;
            int mq = 0;
            for (byte b = 0; b < UnoNumber.MaxValue; ++b)
            {
                byte n = mvcList[b];
                int q = GetNumberQuantity(player, n);
                if (q > mq)
                {
                    for (byte c = 0; c < UnoColor.MaxValue; ++c)
                        if (pile.cards[c, n] > 0)
                        {
                            mq = q;
                            mn = n;
                            mnc = c;
                            break;
                        }
                }
            }
            byte fn = UnoNumber.MaxValue, fnc = UnoColor.MaxValue;
            int fq = int.MaxValue;
            for (byte b = UnoNumber.MaxValue - 1; 0 <= b && b < UnoNumber.MaxValue; --b)
            {
                byte n = mvcList[b];
                int q = GetNumberQuantity(player, n);
                if (q < fq && q > 0)
                {
                    for (byte c = 0; c < UnoColor.MaxValue; ++c)
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

        private void CheckPile()
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
                                movingCard.dbp = 0;
                                movingCard.downpour = 0;
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

        private void ChkPlayer_CheckedChanged(object sender, EventArgs e)
        {
            int index = (int)((CheckBox)sender).Tag;
            if (mnuAppearance.Checked)
            {
                chkPlayer[index].Top = chkPlayer[index].Checked ? 0 : UnoSize.Height / 8;
            }
            if (!options.mnuPairs.Checked && !options.mnuAttack.Checked && ((CheckBox)sender).Checked)
                for (int c = 0; c < chkPlayer.Count; ++c)
                    if (c != index) chkPlayer[c].Checked = false;
        }

        private void ChkPlayer_Enter(object sender, EventArgs e)
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

        private void ChkPlayer_MouseDown(object sender, MouseEventArgs e)
        {
            isSelectingCards = true;
        }

        private void ChkPlayer_MouseLeave(object sender, EventArgs e)
        {
            toolTip.Hide(this);
        }

        private void ChkPlayer_MouseMove(object sender, MouseEventArgs e)
        {
            if (isSelectingCards)
            {
                if (0 < e.X && e.X < pnlPlayer.Width / chkPlayer.Count)
                    return;
                int w;
                if (hPlayer.Visible || pnlPlayer.Left > 0)
                    w = UnoSize.Width;
                else
                    w = width / chkPlayer.Count;
                int i = (int)Math.Floor((float)e.X / w) + int.Parse(((CheckBox)sender).Tag + "");
                if (i >= 0 && i < chkPlayer.Count)
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

        private void ChkPlayer_MouseWheel(object sender, MouseEventArgs e)
        {
            if (hPlayer.Visible)
            {
                switch (Math.Sign(e.Delta))
                {
                    case -1:
                        if (hPlayer.Maximum - hPlayer.Value >= width >> 1)
                        {
                            hPlayer.Value += width >> 1;
                        }
                        else
                        {
                            hPlayer.Value = hPlayer.Maximum;
                        }
                        break;
                    case 1:
                        if (hPlayer.Value - hPlayer.Minimum >= width >> 1)
                        {
                            hPlayer.Value -= width >> 1;
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

        private void Control_KeyDown(object sender, KeyEventArgs e)
        {
            if (!isPlayer0sTurn)
            {
                movingCard.quickly = true;
                return;
            }
            if (options.txtPlayer0.Text == "")
                return;
            byte number = UnoNumber.MaxValue;
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
                    number = UnoNumber.Skip;
                    break;
                case Keys.End:
                    number = UnoNumber.Reverse;
                    break;
                case Keys.PageDown:
                    number = UnoNumber.Draw2;
                    break;
                case Keys.Insert:
                    number = UnoNumber.Blank;
                    break;
                case Keys.Home:
                    number = UnoNumber.Wild;
                    break;
                case Keys.PageUp:
                    number = UnoNumber.WildDraw4;
                    break;
                default:
                    return;
            }
            if (options.mnuPairs.Checked)
            {
                if (mnuPicPlayer.Checked)
                {
                    for (int i = 0; i < _picPlayer.count; ++i)
                        _picPlayer.checkeds[i] = _picPlayer.picPlayer[i].number == number;
                    PicPlayer_CheckedChanged();
                }
            }
            else
            {
                if (mnuPicPlayer.Checked)
                {
                    bool b = true;
                    for (int i = 0; i < _picPlayer.count; ++i)
                    {
                        if (_picPlayer.checkeds[i] && _picPlayer.picPlayer[i].number == number)
                        {
                            b = false;
                        }
                        else
                        {
                            _picPlayer.checkeds[i] = false;
                        }
                    }
                    for (int i = 0; i < _picPlayer.count; ++i)
                    {
                        if (!b)
                        {
                            if (_picPlayer.checkeds[i])
                            {
                                b = true;
                                _picPlayer.checkeds[i] = false;
                            }
                            continue;
                        }
                        if (_picPlayer.picPlayer[i].number == number)
                        {
                            _picPlayer.checkeds[i] = true;
                            break;
                        }
                    }
                    PicPlayer_CheckedChanged();
                }
            }
        }

        private void DownpourDraw(byte player)
        {
            downpour.player = player;
            movingCard.downpour = downpour.count;
            movingCard.player = NextPlayer(player);
            movingCard.progress = 0;
            movingCard.quickly = false;
            lblMovingCards[0].BringToFront();
            timPileToPlayers.Enabled = true;
        }

        private void Draw(byte player)
        {
            CheckPile();
            if (!options.isPlayingRecord)
            {
                record.players[player].Add(new Card[0]);
                record.colors.Add(movingCard.color);
                if (player == 0)
                    record.unos.Add(rdoUno.Checked);
            }
            if ((!movingCard.drew || options.mnuDrawToMatch.Checked) && int.Parse(lblPile.Text) > 0)
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
                movingCard.player = player; movingCard.progress = 0; movingCard.quickly = false;
                lblMovingCards[0].BackColor = Color.White;
                lblMovingCards[0].BringToFront();
                timPileToPlayers.Enabled = true;
            }
            else
            {
                Action(player, "过牌");
                movingCard.drew = false;
                PlayersTurn(player, false);
                PlayersTurn(NextPlayer(player), true, GetDbp());
            }
        }

        private string Format(int number)
        {
            return Format(number.ToString(), true);
        }

        private int Format(string number)
        {
            return int.Parse(Format(number, false));
        }

        private string Format(string number, bool optionsat)
        {
            if (optionsat)
            {
                byte b = 0;
                string s = "";
                for (int i = number.Length - 1; i >= 0; --i)
                {
                    if (b >= 4 && number[i] != '-')
                    {
                        s = ',' + s;
                        b = 0;
                    }
                    s = number[i] + s;
                    ++b;
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
                Interaction.SaveSetting("UNO", "RECORD", "REVERSE", record.reverse.ToString());
                Interaction.SaveSetting("UNO", "RECORD", "DEAL", record.firstGettingCard.ToString());
                Interaction.SaveSetting("UNO", "RECORD", "FIRST", record.firstTurn.ToString());
                Interaction.SaveSetting("UNO", "RECORD", "UNOS", string.Join(",", record.unos));
                if (options.mnuSevenZero.Checked || options.mnuTradeHands.Checked)
                    Interaction.SaveSetting("UNO", "RECORD", "TRADE_HANDS", string.Join("P", record.tradeHands));
                Interaction.SaveSetting("UNO", "RECORD", "COLORS", string.Join("C", record.colors));
                if (options.keys.Length > 0)
                    Interaction.SaveSetting("UNO", "RECORD", "GAME", string.Join("K", options.keys));
                else
                    Interaction.SaveSetting("UNO", "RECORD", "GAME", "");
                string s = "";
                foreach (Card c in record.pile)
                    s += c.color + "I" + c.number + "C";
                Interaction.SaveSetting("UNO", "RECORD", "PILE", s.Substring(0, s.Length - 1));
                s = "";
                foreach (List<Card[]> p in record.players)
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
                List<Label> label = new();
                for (byte p = 1; p <= 3; ++p)
                {
                    int playerPos = label.Count;
                    if (mnuByColor.Checked)
                        for (byte color = 0; color <= UnoColor.MaxValue; ++color)
                            for (byte number = 0; number <= UnoNumber.MaxValue; ++number)
                                for (int c = 1; c <= players[p].cards[color, number]; ++c)
                                {
                                    int length = label.Count;
                                    AddLabel(label);
                                    SetCard(label[length], color, number);
                                    if (p == 1 || p == 3)
                                        label[length].Left = lblPlayers[p].Left;
                                    if (p == 2) label[length].Top = lblPlayers[2].Top;
                                }
                    else
                        for (byte number = 0; number <= UnoNumber.MaxValue; ++number)
                            for (byte color = 0; color <= UnoColor.MaxValue; ++color)
                                for (int c = 1; c <= players[p].cards[color, number]; ++c)
                                {
                                    int length = label.Count;
                                    AddLabel(label);
                                    SetCard(label[length], color, number);
                                    if (p == 1 || p == 3)
                                        label[length].Left = lblPlayers[p].Left;
                                    if (p == 2) label[length].Top = lblPlayers[2].Top;
                                }
                    for (int c = playerPos; c < label.Count; ++c)
                    {
                        switch (p)
                        {
                            case 1:
                            case 3:
                                if ((height >> 1) - (UnoSize.Height * (label.Count - playerPos) >> 1) >= 0)
                                    label[c].Top = (height >> 1) - (UnoSize.Height * (label.Count - playerPos) >> 1) + UnoSize.Height * (c - playerPos);
                                else
                                    label[c].Top = height / (label.Count - playerPos) * (c - playerPos);
                                break;
                            case 2:
                                if ((width >> 1) - UnoSize.Width * (label.Count - playerPos) >> 1 >= 0)
                                    label[c].Left = (width >> 1) - (UnoSize.Width * (label.Count - playerPos) >> 1) + UnoSize.Width * (c - playerPos);
                                else
                                    label[c].Left = width / (label.Count - playerPos) * (c - playerPos);
                                break;
                        }
                    }
                }
                lblPlayers[gameOver].Visible = false;
                string msg = (gameOver == 0 ? "你" : playerNames[gameOver]) + " 赢了!\n" +
                    "\n" +
                    (options.mnuWatch.Checked && !options.isPlayingRecord ? $"游戏时长\t{lblWatch.Text}\n" +
                    "\n" : "") +
                    "玩家\t得分";
                for (byte p = 0; p <= 3; ++p)
                    msg += "\n" + (p == 0 ? "你" : playerNames[p]) + "\t" + GetPointsByPlayer(p);
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
                    List<Label> label = new();
                    if (mnuByColor.Checked)
                        for (byte color = 0; color <= UnoColor.MaxValue; ++color)
                            for (byte number = 0; number <= UnoNumber.MaxValue; ++number)
                                for (int c = 1; c <= players[gameOver].cards[color, number]; ++c)
                                {
                                    int length = label.Count;
                                    AddLabel(label);
                                    SetCard(label[length], color, number);
                                    if (gameOver == 1 || gameOver == 3)
                                        label[length].Left = lblPlayers[gameOver].Left;
                                    if (gameOver == 2) label[length].Top = lblPlayers[2].Top;
                                }
                    else
                        for (byte number = 0; number <= UnoNumber.MaxValue; ++number)
                            for (byte color = 0; color <= UnoColor.MaxValue; ++color)
                                for (int c = 1; c <= players[gameOver].cards[color, number]; ++c)
                                {
                                    int length = label.Count;
                                    AddLabel(label);
                                    SetCard(label[length], color, number);
                                    if (gameOver == 1 || gameOver == 3)
                                        label[length].Left = lblPlayers[gameOver].Left;
                                    if (gameOver == 2) label[length].Top = lblPlayers[2].Top;
                                }
                    for (int c = 0; c < label.Count; ++c)
                    {
                        switch (gameOver)
                        {
                            case 1:
                            case 3:
                                if ((height >> 1) - (UnoSize.Height * label.Count >> 1) >= 0)
                                    label[c].Top = (height >> 1) - (UnoSize.Height * label.Count >> 1) + UnoSize.Height * c;
                                else
                                    label[c].Top = height / label.Count * c;
                                break;
                            case 2:
                                if ((width >> 1) - (UnoSize.Width * label.Count >> 1) >= 0)
                                    label[c].Left = (width >> 1) - (UnoSize.Width * label.Count >> 1) + UnoSize.Width * c;
                                else
                                    label[c].Left = width / label.Count * c;
                                break;
                        }
                    }
                }
                lblPlayers[gameOver].Visible = false;
                if (MessageBox.Show(
                    (gameOver == 0 ? "你" : playerNames[gameOver]) + " 输了!\n"
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

        private Card GetBestCard(byte player)
        {
            if (movingCard.isPlaying)
            {
                byte backColor = GetColorId(BackColor), backNumber = GetNumberId(lblCards[1].Text);
                if (backNumber == UnoNumber.Reverse && lblCards.Last().BackColor == Color.Black && !options.mnuPlayOrDrawAll.Checked)
                {
                    if (pile.cards[UnoColor.Black, UnoNumber.Reverse] > 0)
                        return new Card(UnoColor.Black, UnoNumber.Reverse);
                    return null;
                }
                if (backNumber == UnoNumber.WildHitfire && !options.mnuPlayOrDrawAll.Checked)
                {
                    if (pile.cards[UnoColor.Black, UnoNumber.Reverse] > 0)
                        return new Card(UnoColor.Black, UnoNumber.Reverse);
                    if (pile.cards[UnoColor.Black, UnoNumber.WildHitfire] > 0)
                        return new Card(UnoColor.Black, UnoNumber.WildHitfire);
                    return null;
                }
                if (backNumber == UnoNumber.WildDrawColor && !options.mnuPlayOrDrawAll.Checked)
                {
                    if (pile.cards[UnoColor.Black, UnoNumber.Reverse] > 0)
                        return new Card(UnoColor.Black, UnoNumber.Reverse);
                    if (pile.cards[UnoColor.Black, UnoNumber.WildDrawColor] > 0)
                        return new Card(UnoColor.Black, UnoNumber.WildDrawColor);
                    return null;
                }
                if (backNumber == UnoNumber.WildDraw4 && !options.mnuPlayOrDrawAll.Checked)
                {
                    if (pile.cards[UnoColor.Black, UnoNumber.Reverse] > 0)
                        return new Card(UnoColor.Black, UnoNumber.Reverse);
                    if (pile.cards[UnoColor.Black, UnoNumber.WildDraw4] > 0)
                        return new Card(UnoColor.Black, UnoNumber.WildDraw4);
                    return null;
                }
                if (backNumber == UnoNumber.Draw5 && !options.mnuPlayOrDrawAll.Checked)
                {
                    if (pile.cards[UnoColor.Black, UnoNumber.Reverse] > 0)
                        return new Card(UnoColor.Black, UnoNumber.Reverse);
                    if (pile.cards[UnoColor.Black, UnoNumber.WildDraw4] > 0)
                        return new Card(UnoColor.Black, UnoNumber.WildDraw4);
                    foreach (byte b in colorList)
                        if (pile.cards[b, UnoNumber.Draw5] > 0)
                            return new Card(b, UnoNumber.Draw5);
                    return null;
                }
                if (backNumber == UnoNumber.Draw2 && !options.mnuPlayOrDrawAll.Checked)
                {
                    if (pile.cards[UnoColor.Black, UnoNumber.Reverse] > 0)
                        return new Card(UnoColor.Black, UnoNumber.Reverse);
                    if (pile.cards[UnoColor.Black, UnoNumber.WildDraw4] > 0)
                        return new Card(UnoColor.Black, UnoNumber.WildDraw4);
                    foreach (byte b in colorList)
                        if (pile.cards[b, UnoNumber.Draw5] > 0)
                            return new Card(b, UnoNumber.Draw5);
                    if (pile.cards[UnoColor.Black, UnoNumber.Draw2] > 0)
                        return new Card(UnoColor.Black, UnoNumber.Draw2);
                    foreach (byte b in colorList)
                        if (pile.cards[b, UnoNumber.Draw2] > 0)
                            return new Card(b, UnoNumber.Draw2);
                    return null;
                }
                if (backNumber == UnoNumber.Draw1 && !options.mnuPlayOrDrawAll.Checked)
                {
                    if (pile.cards[UnoColor.Black, UnoNumber.Reverse] > 0)
                        return new Card(UnoColor.Black, UnoNumber.Reverse);
                    if (pile.cards[UnoColor.Black, UnoNumber.WildDraw4] > 0)
                        return new Card(UnoColor.Black, UnoNumber.WildDraw4);
                    foreach (byte b in colorList)
                        if (pile.cards[b, UnoNumber.Draw5] > 0)
                            return new Card(b, UnoNumber.Draw5);
                    if (pile.cards[UnoColor.Black, UnoNumber.Draw2] > 0)
                        return new Card(UnoColor.Black, UnoNumber.Draw2);
                    foreach (byte b in colorList)
                        if (pile.cards[b, UnoNumber.Draw2] > 0)
                            return new Card(b, UnoNumber.Draw2);
                    foreach (byte b in colorList)
                        if (pile.cards[b, UnoNumber.Draw1] > 0)
                            return new Card(b, UnoNumber.Draw1);
                    return null;
                }
                for (byte b = 0; b < UnoNumber.MaxValue; ++b)
                {
                    byte n = mvcList[b];
                    if (pile.cards[UnoColor.Black, n] > 0)
                    {
                        return new Card(UnoColor.Black, n);
                    }
                }
                if (pile.cards[UnoColor.Magenta, UnoNumber.Blank] > 0)
                {
                    return new Card(UnoColor.Magenta, UnoNumber.Blank);
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
                    byte mn = UnoNumber.MaxValue;
                    int mq = 0, qn;
                    for (byte b = 0; b < UnoNumber.MaxValue; ++b)
                    {
                        byte n = mvcList[b];
                        int q = GetNumberQuantity(player, n);
                        if (q > mq)
                        {
                            qn = 0;
                            if (n < UnoNumber.Blank && players[player].cards[backColor, n] <= 0 && pile.cards[backColor, n] <= 0)
                                continue;
                            for (byte c = 0; c < UnoColor.MaxValue; ++c)
                                qn += pile.cards[c, n];
                            if (qn > 0)
                            {
                                mq = q;
                                mn = n;
                            }
                        }
                    }
                    if (mn != UnoNumber.MaxValue)
                    {
                        if (pile.cards[backColor, mn] > 0)
                            return new Card(backColor, mn);
                        else
                            foreach (byte c in colorList)
                                if (pile.cards[c, mn] > 0)
                                    return new Card(c, mn);
                    }
                }
                for (byte b = 0; b < UnoNumber.MaxValue; ++b)
                {
                    byte n = mvcList[b];
                    if (pile.cards[backColor, n] > 0)
                    {
                        return new Card(backColor, n);
                    }
                }
            }
            for (byte b = 0; b < UnoNumber.MaxValue; ++b)
            {
                byte n = mvcList[b];
                for (byte c = 0; c < UnoColor.MaxValue; ++c)
                {
                    if (pile.cards[c, n] > 0)
                    {
                        return new Card(c, n);
                    }
                }
            }
            return null;
        }

        private Color GetColor(byte id)
        {
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

        private byte GetColorId(Color color)
        {
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

        private string GetColorName(byte id)
        {
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

        private int GetColorQuantity(byte player, byte color)
        {
            // eg: color = R
            // if [R 0][R 1][Y 1][Y 2][Y 2][Y 2]
            // then return 2
            int mq = 0;
            for (byte n = 0; n < UnoNumber.MaxValue; ++n)
            {
                int q = players[player].cards[color, n];
                if (q > 0)
                {
                    q = 0;
                    for (byte c = 0; c < UnoColor.MaxValue; ++c)
                        q += players[player].cards[c, n];
                    if (q > mq)
                        mq = q;
                }
            }
            return mq;
        }

        private int GetColorQuantityByNumber(byte player, byte number)
        {
            // eg: number = 0
            // if [R 0][R 0][R 0][Y 0][R 1]
            // then return 2
            int i = 0;
            for (byte c = 0; c < UnoColor.MaxValue; ++c)
                i += Math.Sign(players[player].cards[c, number]);
            return i;
        }

        private int GetDbp()
        {
            return (options.mnuDrawBeforePlaying.Checked ? 1 : 0) + (options.mnuDrawTwoBeforePlaying.Checked ? 2 : 0);
        }

        private string GetNumber(byte id)
        {
            return id switch
            {
                UnoNumber.Skip => UnoNumberName.Skip,
                UnoNumber.SkipEveryone => UnoNumberName.SkipEveryone,
                UnoNumber.Reverse => UnoNumberName.Reverse,
                UnoNumber.Draw1 => UnoNumberName.Draw1,
                UnoNumber.Draw2 => UnoNumberName.Draw2,
                UnoNumber.Draw5 => UnoNumberName.Draw5,
                UnoNumber.DiscardAll => UnoNumberName.DiscardAll,
                UnoNumber.TradeHands => UnoNumberName.TradeHands,
                UnoNumber.Number => UnoNumberName.Number,
                UnoNumber.Blank => options.txtBlankText.Text,
                UnoNumber.Wild => UnoNumberName.Wild,
                UnoNumber.WildDownpourDraw1 => UnoNumberName.WildDownpourDraw1,
                UnoNumber.WildDownpourDraw2 => UnoNumberName.WildDownpourDraw2,
                UnoNumber.WildDownpourDraw4 => UnoNumberName.WildDownpourDraw4,
                UnoNumber.WildDraw4 => UnoNumberName.WildDraw4,
                UnoNumber.WildDrawColor => UnoNumberName.WildDrawColor,
                UnoNumber.WildHitfire => UnoNumberName.WildHitfire,
                UnoNumber.Null => UnoNumberName.Null,
                _ => id + "",
            };
        }

        private byte GetNumberId(string number)
        {
            if (number == options.txtBlankText.Text)
            {
                return UnoNumber.Blank;
            }
            return number switch
            {
                UnoNumberName.Skip => UnoNumber.Skip,
                UnoNumberName.SkipEveryone => UnoNumber.SkipEveryone,
                UnoNumberName.Reverse => UnoNumber.Reverse,
                UnoNumberName.Draw1 => UnoNumber.Draw1,
                UnoNumberName.Draw2 => UnoNumber.Draw2,
                UnoNumberName.Draw5 => UnoNumber.Draw5,
                UnoNumberName.DiscardAll => UnoNumber.DiscardAll,
                UnoNumberName.TradeHands => UnoNumber.TradeHands,
                UnoNumberName.Number => UnoNumber.Number,
                UnoNumberName.Wild => UnoNumber.Wild,
                UnoNumberName.WildDownpourDraw1 => UnoNumber.WildDownpourDraw1,
                UnoNumberName.WildDownpourDraw2 => UnoNumber.WildDownpourDraw2,
                UnoNumberName.WildDownpourDraw4 => UnoNumber.WildDownpourDraw4,
                UnoNumberName.WildDraw4 => UnoNumber.WildDraw4,
                UnoNumberName.WildDrawColor => UnoNumber.WildDrawColor,
                UnoNumberName.WildHitfire => UnoNumber.WildHitfire,
                UnoNumberName.Null => UnoNumber.Null,
                _ => byte.Parse(number),
            };
        }

        private string GetNumberName(byte number)
        {
            if (number == UnoNumber.Blank)
            {
                return "Blank";
            }
            return number switch
            {
                UnoNumber.Skip => "Skip",
                UnoNumber.SkipEveryone => "Skip Everyone",
                UnoNumber.Reverse => "Reverse",
                UnoNumber.Draw1 => "Draw One",
                UnoNumber.Draw2 => "Draw Two",
                UnoNumber.Draw5 => "Draw Five",
                UnoNumber.DiscardAll => "Discard All",
                UnoNumber.TradeHands => "Trade Hands",
                UnoNumber.Number => "Wild Number",
                UnoNumber.Wild => "Wild",
                UnoNumber.WildDownpourDraw1 => "Wild Downpour Draw One",
                UnoNumber.WildDownpourDraw2 => "Wild Downpour Draw Two",
                UnoNumber.WildDownpourDraw4 => "Wild Downpour Draw Four",
                UnoNumber.WildDraw4 => "Wild Draw Four",
                UnoNumber.WildDrawColor => "Wild Draw Color",
                UnoNumber.WildHitfire => "Wild Hit-fire",
                UnoNumber.Null => "Null",
                _ => number + "",
            };
        }

        private int GetNumberQuantity(byte player, byte number)
        {
            // eg: number = 0
            // if [R 0][R 0][Y 0][R 1]
            // then return 3
            int i = 0;
            for (byte c = 0; c < UnoColor.MaxValue; ++c)
                i += players[player].cards[c, number];
            return i;
        }

        private int GetOnplayersCards(byte color, byte number)
        {
            int cards = 0;
            if (players[0] == null)
                return 0;
            foreach (Cards p in players)
                cards += p.cards[color, number];
            return cards;
        }

        private Card[] GetPile()
        {
            List<Card> cards = new();
            for (byte color = 0; color <= UnoColor.MaxValue; ++color)
                for (byte number = 0; number <= UnoNumber.MaxValue; ++number)
                    for (int c = 1; c <= pile.cards[color, number]; ++c)
                        cards.Add(new Card(color, number));
            return cards.ToArray();
        }

        private int GetPoints(byte number)
        {
            return number switch
            {
                UnoNumber.Draw1
                => 10,
                UnoNumber.Skip or
                UnoNumber.Reverse or
                UnoNumber.Draw2 or
                UnoNumber.Draw5 or
                UnoNumber.Blank or
                UnoNumber.WildDownpourDraw1 or
                UnoNumber.WildDownpourDraw2
                => 20,
                UnoNumber.SkipEveryone or
                UnoNumber.DiscardAll or
                UnoNumber.TradeHands
                => 30,
                UnoNumber.Number
                => 40,
                UnoNumber.Wild or
                UnoNumber.WildDownpourDraw4 or
                UnoNumber.WildDraw4 or
                UnoNumber.WildHitfire
                => 50,
                UnoNumber.WildDrawColor
                => 60,
                _
                => number,
            };
        }

        private int GetPointsByColor(byte player, byte color)
        {
            int points = 0;
            for (byte n = 0; n < UnoNumber.MaxValue; ++n)
            {
                points += GetPoints(n) * players[player].cards[color, n];
            }
            return points;
        }

        private int GetPointsByPlayer(byte player)
        {
            int points = 0;
            Card[] cards = PlayersCards(player);
            foreach (Card c in cards)
                points += GetPoints(c.number);
            return points;
        }

        private int GetQuantityByColor(byte player, byte color)
        {
            // eg: color = R
            // if [R 0][R 1][R 2][Y 0]
            // then return 3
            int quantity = 0;
            for (byte q = 0; q <= UnoNumber.MaxValue; ++q)
                quantity += players[player].cards[color, q];
            return quantity;
        }

        private int GetQuantityByNumber(byte player, byte number)
        {
            // eg: number = 0
            // if [R 0][R 1][Y 0][G 1][G 2][G 3]
            // then return 2
            int q, quantity = 0;
            for (byte c = 0; c <= UnoColor.MaxValue; ++c)
            {
                if (players[player].cards[c, number] > 0)
                {
                    q = 0;
                    for (byte n = 0; n <= UnoNumber.MaxValue; ++n)
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

        private string GetType(byte color, byte number)
        {
            if (color == UnoColor.Black)
                return "万能";
            else if (number > 10)
                return "功能";
            else
                return "普通";
        }

        private string GetUsage(byte number)
        {
            switch (number)
            {
                case UnoNumber.Skip: return "禁止下家出牌";
                case UnoNumber.SkipEveryone: return "禁止每家出牌";
                case UnoNumber.Reverse: return "反转出牌顺序";
                case UnoNumber.Draw1: return "下家罚抽 1 张牌";
                case UnoNumber.Draw2: return "下家罚抽 2 张牌";
                case UnoNumber.Draw5: return "下家罚抽 5 张牌";
                case UnoNumber.DiscardAll: return "允许玩家打出所有相同颜色的牌";
                case UnoNumber.TradeHands: return "選擇一位玩家交換手牌";
                case UnoNumber.Number: return "表示任意数字";
                case UnoNumber.Blank:
                    int downpourDraw = int.Parse(options.txtBlankDownpourDraw.Text), draw = int.Parse(options.txtBlankDraw.Text), skip = int.Parse(options.txtBlankSkip.Text);
                    string s = ""
                        + (skip > 0 ? $"\n　　\t禁止下家出牌 × {skip}" : "")
                        + (options.mnuBlankReverse.Checked ? "\n　　\t反转出牌顺序" : "")
                        + (draw > 0 ? $"\n　　\t下家罚抽 {draw} 张牌" : "")
                        + (downpourDraw > 0 ? $"\n　　\t所有玩家罚抽 {downpourDraw} 张牌" : "");
                    return s == "" ? "普通的空白牌" : s.Substring(4);
                case UnoNumber.Wild: return "";
                case UnoNumber.WildDownpourDraw1: return "所有玩家罚抽 1 张牌";
                case UnoNumber.WildDownpourDraw2: return "所有玩家罚抽 2 张牌";
                case UnoNumber.WildDownpourDraw4: return "所有玩家罚抽 4 张牌";
                case UnoNumber.WildDraw4: return "下家罚抽 4 张牌";
                case UnoNumber.WildDrawColor: return "下家罚抽牌直至抽到被指定顏色的牌";
                case UnoNumber.WildHitfire: return "下家罚抽牌盒中的所有牌";
                default: return $"普通的 {number} 号牌";
            };
        }

        private void HPlayer_Scroll(object sender, ScrollEventArgs e)
        {
            pnlPlayer.Left = -((HScrollBar)sender).Value;
        }

        private bool IsNumeric(string s)
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

        private void ItmQuit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Label_MouseLeave(object sender, EventArgs e)
        {
            toolTip.Hide(this);
        }

        private void LblCounts_SizeChanged(object sender, EventArgs e)
        {
            Label l = (Label)sender;
            int i = int.Parse(l.Tag + "");
            l.Left = lblPlayers[i].Left + (lblPlayers[i].Width >> 1) - (l.Width >> 1);
        }

        private void LblCounts_TextChanged(object sender, EventArgs e)
        {
            byte index = (byte)((Label)sender).Tag;
            if (options.mnuRevealable.Checked && index > 0)
            {
                Reveal(index);
            }
            if (movingCard.isPlaying)
            {
                byte winners = 0;
                if (int.Parse(lblCounts[index].Text) <= 0)
                {
                    lblPlayers[index].Visible = false;
                    lblCounts[index].Visible = false;
                    if (!options.mnuMultiply0.Checked)
                    {
                        int[] payments = new int[4], points = new int[4];
                        for (byte p = 0; p <= 3; ++p)
                        {
                            if (lblCounts[p].Visible)
                            {
                                points[p] = GetPointsByPlayer(p);
                            }
                        }
                        int win = 0;
                        string msg = "";
                        for (byte p = 0; p <= 3; ++p)
                        {
                            if (lblCounts[p].Visible)
                            {
                                payments[p] = points[p] * options.multiplier;
                                win += payments[p];
                                msg += $"{(p == 0 ? "你" : playerNames[p])}: -{Format(payments[p])}\n";
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
                        Action(index, $"{msg}----------------\n{(index == 0 ? "你" : playerNames[index])}: +{Format(win)}");
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
                    for (byte p = 0; p < lblPlayers.Length; ++p) if (!lblPlayers[p].Visible) ++winners;
                    if (winners == 3)
                    {
                        for (byte p = 0; p < lblPlayers.Length; ++p)
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
                        case UnoNumber.Draw1:
                        case UnoNumber.Draw2:
                        case UnoNumber.Draw5:
                        case UnoNumber.WildDraw4:
                        case UnoNumber.WildDrawColor:
                        case UnoNumber.WildHitfire:
                            Draw(NextPlayer(index));
                            break;
                        case UnoNumber.WildDownpourDraw1:
                        case UnoNumber.WildDownpourDraw2:
                        case UnoNumber.WildDownpourDraw4:
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
                        for (byte c = 0; c <= UnoColor.MaxValue; ++c)
                            for (byte n = 0; n <= UnoNumber.MaxValue; ++n)
                            {
                                string q = pile.cards[c, n] + "";
                                if (q != "0")
                                {
                                    if (q == "1")
                                    {
                                        q = "";
                                    }
                                    cards += "[" + GetColorName(c) + GetNumber(n) + "]" + q;
                                    ++i;
                                    if (i % (int)((float)width / UnoSize.Width) == 0)
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
            if (e.Button == MouseButtons.Right && !options.isPlayingRecord)
            {
                lblPile.MouseDoubleClick -= LblPile_MouseDoubleClick;
                for (byte p = 1; p <= 3; ++p)
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
                case 1: lblPlayers[1].Top = (height >> 1) - (lblPlayers[1].Height >> 1); break;
                case 2: lblPlayers[2].Left = (width >> 1) - (lblPlayers[2].Width >> 1); break;
                case 3: lblPlayers[3].Location = new Point(width - lblPlayers[3].Width, (height >> 1) - (lblPlayers[3].Height >> 1)); break;
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
                case 1: lblPlayers[1].Location = new Point(0, (height >> 1) - (UnoSize.Height >> 1) + (mnuGame.Height >> 1)); break;
                case 2: lblPlayers[2].Location = new Point((width >> 1) - (UnoSize.Width >> 1), mnuGame.Height); break;
                case 3: lblPlayers[3].Location = new Point(width - UnoSize.Width, (height >> 1) - (UnoSize.Height >> 1) + (mnuGame.Height >> 1)); break;
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
            for (byte p = 0; p <= 3; ++p)
            {
                colors = pls[p].Split(char.Parse("C"));
                for (byte c = 0; c <= UnoColor.MaxValue; ++c)
                {
                    string[] numbers = colors[c].Split(char.Parse("N"));
                    for (byte n = 0; n <= UnoNumber.MaxValue; ++n)
                        players[p].cards[c, n] = int.Parse(numbers[n]);
                }
                lblCounts[p].Text = PlayersCardsCount(p).ToString();
            }
            Sort();
            colors = keys[1].Split(char.Parse("C"));
            for (byte c = 0; c <= UnoColor.MaxValue; ++c)
            {
                string[] numbers = colors[c].Split(char.Parse("N"));
                for (byte n = 0; n <= UnoNumber.MaxValue; ++n)
                    pile.cards[c, n] = int.Parse(numbers[n]);
            }
            lblPile.Text = GetPile().Length - 1 + "";
            BackColor = GetColor(byte.Parse(keys[2]));
            foreach (Label card in lblCards)
            {
                byte c = byte.Parse(keys[2]), n = byte.Parse(keys[3]);
                if (n >= UnoNumber.Wild)
                    c = UnoColor.Black;
                SetCard(card, c, n);
            }
            skip = int.Parse(keys[4]);
            string[] sks = keys[5].Split('P');
            for (byte sk = 0; sk <= 3; ++sk)
                skips[sk] = int.Parse(sks[sk]);
            reverse = bool.Parse(keys[6]);
            SetDraw(int.Parse(keys[7]));
            gametime = int.Parse(keys[8]);
        }

        private void LoadRecord()
        {
            record.reverse = bool.Parse(Interaction.GetSetting("UNO", "RECORD", "REVERSE"));
            record.firstGettingCard = byte.Parse(Interaction.GetSetting("UNO", "RECORD", "DEAL"));
            record.firstTurn = byte.Parse(Interaction.GetSetting("UNO", "RECORD", "FIRST"));
            record.unos = new List<bool>(Interaction.GetSetting("UNO", "RECORD", "UNOS").Split(',').Select(bool.Parse));
            if (options.mnuSevenZero.Checked || options.mnuTradeHands.Checked)
                record.tradeHands = new List<byte>(Interaction.GetSetting("UNO", "RECORD", "TRADE_HANDS").Split('P').Select(byte.Parse));
            record.colors = new List<byte>(Interaction.GetSetting("UNO", "RECORD", "COLORS").Split('C').Select(byte.Parse));
            record.pile = new List<Card>(Interaction.GetSetting("UNO", "RECORD", "PILE").Split('C').Select(s => new Card(byte.Parse(s.Split('I')[0]), byte.Parse(s.Split('I')[1]))));
            string[] ps = Interaction.GetSetting("UNO", "RECORD", "PLAYERS").Split('P');
            for (byte p = 0; p < 4; ++p)
            {
                List<string> ts = new(ps[p].Split('T'));
                foreach (string t in ts)
                {
                    if (t == "")
                    {
                        record.players[p].Add(new Card[0]);
                        continue;
                    }
                    string[] cs = t.Split('C');
                    List<Card> cards = new();
                    foreach (string c in cs)
                    {
                        if (c == "")
                            continue;
                        cards.Add(new Card(byte.Parse(c.Split('I')[0]), byte.Parse(c.Split('I')[1])));
                    }
                    record.players[p].Add(cards.ToArray());
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
                    if (c.color == UnoColor.Black || c.color != cards[0].color)
                    {
                        color = $"\n轉{GetColorName(movingCard.color)}色";
                        break;
                    }
                }
                if (MessageBox.Show($"你可以嘗試出:\n{s}{color}\n\n需要我敎你嗎?", "出牌敎程", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2) == DialogResult.OK)
                {
                    if (mnuPicPlayer.Checked)
                    {
                        for (int i = 0; i < _picPlayer.count; ++i)
                        {
                            _picPlayer.checkeds[i] = false;
                            foreach (Card c in cards)
                            {
                                if (_picPlayer.picPlayer[i].color == c.color && _picPlayer.picPlayer[i].number == c.number)
                                {
                                    _picPlayer.checkeds[i] = true;
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
                            byte fromColor = 0, fromNumber = 0, player = 0, toColor = UnoColor.MaxValue, toNumber = UnoNumber.MaxValue;
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
                            for (byte color = fromColor; color <= toColor; ++color)
                                for (byte number = fromNumber; number <= toNumber; ++number)
                                    players[player].cards[color, number] = 0;
                            if (player == 0) Sort();
                            Action(0, $"已淸除 {playerNames[player]} 的手牌");
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
                                for (int c = 1; c <= count; ++c)
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
                            Action(0, $"已将 [{GetColorName(byte.Parse(data[2]))} {GetNumber(byte.Parse(data[3]))}] * {count} 给予 {playerNames[byte.Parse(data[1])]}");
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
                                    movingCard.color = byte.Parse(data[1]);
                                    List<Card> c = new();
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
                                        PlayersTurn(movingCard.player = NextPlayer(movingCard.player), true, GetDbp());
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
                                    Action(0, $"跳过 {playerNames[byte.Parse(data[1])]} {data[2]} 次");
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
                    color = _picPlayer.picPlayer[pointing].color;
                    number = _picPlayer.picPlayer[pointing].number;
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
                    _picPlayer.picPlayer[pointing].color = card.color;
                    _picPlayer.picPlayer[pointing].number = card.number;
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
                $"[{UnoNumberName.Skip}]\tDelete\n" +
                $"[{UnoNumberName.Reverse}]\tEnd\n" +
                $"[{UnoNumberName.Draw2}]\tPageDown\n" +
                $"[{options.txtBlankText.Text}]\tInsert\n" +
                $"[{UnoNumberName.Wild}]\tHome\n" +
                $"[{UnoNumberName.WildDraw4}]\tPageUp\n" +
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
                    movingCard.quickly = true;
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

        private byte NextPlayer(byte currentPlayer, bool reverse = false)
        {
            bool r = reverse;
            if (this.reverse) r = !r;
            byte next = (byte)(!r ? currentPlayer == 3 ? 0 : currentPlayer + 1 : currentPlayer == 0 ? 3 : currentPlayer - 1);
            return lblPlayers[next].Visible ? next : NextPlayer(next, reverse);
        }

        private void PicPlayer_CheckedChanged(bool sort = false)
        {
            if (width <= 0)
                return;
            if (sort)
            {
                _picPlayer.count = PlayersCards(0).Length;
                _picPlayer.checkeds = new bool[_picPlayer.count];
                _picPlayer.selected = -1;
                _picPlayer.picPlayer.Clear();
            }
            if (_picPlayer.count <= 0)
            {
                picPlayer.Visible = false;
                return;
            }
            int i = 0;
            if (_picPlayer.count * UnoSize.Width <= width)
            {
                imgPlayer = new Bitmap(_picPlayer.count * UnoSize.Width, UnoSize.Height + UnoSize.Height / 8);
                gpsPlayer = Graphics.FromImage(imgPlayer);
                if (sort)
                {
                    picPlayer.Width = _picPlayer.count * UnoSize.Width;
                    _picPlayer.cardWidth = UnoSize.Width;
                    if (mnuByColor.Checked)
                        for (byte color = 0; color <= UnoColor.MaxValue; ++color)
                            for (byte number = 0; number <= UnoNumber.MaxValue; ++number)
                                for (int c = 1; c <= players[0].cards[color, number]; ++c)
                                {
                                    _picPlayer.picPlayer.Add(new Card(color, number));
                                    gpsPlayer.DrawImage(imgUno, new Rectangle(i * UnoSize.Width, _picPlayer.checkeds[i] ? 0 : UnoSize.Height / 8, UnoSize.Width, UnoSize.Height), number * ResourceUnoSize.Width, color * ResourceUnoSize.Height, ResourceUnoSize.Width, ResourceUnoSize.Height, GraphicsUnit.Pixel);
                                    ++i;
                                }
                    else
                        for (byte number = 0; number <= UnoNumber.MaxValue; ++number)
                            for (byte color = 0; color <= UnoColor.MaxValue; ++color)
                                for (int c = 1; c <= players[0].cards[color, number]; ++c)
                                {
                                    _picPlayer.picPlayer.Add(new Card(color, number));
                                    gpsPlayer.DrawImage(imgUno, new Rectangle(i * UnoSize.Width, _picPlayer.checkeds[i] ? 0 : UnoSize.Height / 8, UnoSize.Width, UnoSize.Height), number * ResourceUnoSize.Width, color * ResourceUnoSize.Height, ResourceUnoSize.Width, ResourceUnoSize.Height, GraphicsUnit.Pixel);
                                    ++i;
                                }
                    picPlayer.Left = (width >> 1) - (picPlayer.Width >> 1);
                }
                else
                {
                    for (i = 0; i < _picPlayer.count; ++i)
                        gpsPlayer.DrawImage(imgUno, new Rectangle(i * UnoSize.Width, _picPlayer.checkeds[i] ? 0 : UnoSize.Height / 8, UnoSize.Width, UnoSize.Height), _picPlayer.picPlayer[i].number * ResourceUnoSize.Width, _picPlayer.picPlayer[i].color * ResourceUnoSize.Height, ResourceUnoSize.Width, ResourceUnoSize.Height, GraphicsUnit.Pixel);
                }
            }
            else
            {
                imgPlayer = new Bitmap(width, UnoSize.Height + UnoSize.Height / 8);
                gpsPlayer = Graphics.FromImage(imgPlayer);
                if (sort)
                {
                    picPlayer.Width = width;
                    _picPlayer.cardWidth = (float)width / _picPlayer.count;
                    if (mnuByColor.Checked)
                        for (byte color = 0; color <= UnoColor.MaxValue; ++color)
                            for (byte number = 0; number <= UnoNumber.MaxValue; ++number)
                                for (int c = 1; c <= players[0].cards[color, number]; ++c)
                                {
                                    _picPlayer.picPlayer.Add(new Card(color, number));
                                    gpsPlayer.DrawImage(imgUno, new Rectangle((int)(i * _picPlayer.cardWidth), _picPlayer.checkeds[i] ? 0 : UnoSize.Height / 8, UnoSize.Width, UnoSize.Height), number * ResourceUnoSize.Width, color * ResourceUnoSize.Height, ResourceUnoSize.Width, ResourceUnoSize.Height, GraphicsUnit.Pixel);
                                    ++i;
                                }
                    else
                        for (byte number = 0; number <= UnoNumber.MaxValue; ++number)
                            for (byte color = 0; color <= UnoColor.MaxValue; ++color)
                                for (int c = 1; c <= players[0].cards[color, number]; ++c)
                                {
                                    _picPlayer.picPlayer.Add(new Card(color, number));
                                    gpsPlayer.DrawImage(imgUno, new Rectangle((int)(i * _picPlayer.cardWidth), _picPlayer.checkeds[i] ? 0 : UnoSize.Height / 8, UnoSize.Width, UnoSize.Height), number * ResourceUnoSize.Width, color * ResourceUnoSize.Height, ResourceUnoSize.Width, ResourceUnoSize.Height, GraphicsUnit.Pixel);
                                    ++i;
                                }
                    picPlayer.Left = 0;
                }
                else
                {
                    for (i = 0; i < _picPlayer.count; ++i)
                        gpsPlayer.DrawImage(imgUno, new Rectangle((int)(i * _picPlayer.cardWidth), _picPlayer.checkeds[i] ? 0 : UnoSize.Height / 8, UnoSize.Width, UnoSize.Height), _picPlayer.picPlayer[i].number * ResourceUnoSize.Width, _picPlayer.picPlayer[i].color * ResourceUnoSize.Height, ResourceUnoSize.Width, ResourceUnoSize.Height, GraphicsUnit.Pixel);
                }
            }
            picPlayer.Image = imgPlayer;
            _picPlayer.selecting = -1;
        }
        private void PicPlayer_CheckedChanging(int index, bool isChecked)
        {
            gpsPlayer = Graphics.FromImage(imgPlayer);
            gpsPlayer.FillRectangle(new SolidBrush(Color.FromArgb(0x80, Color.Gray)), index * _picPlayer.cardWidth, isChecked ? UnoSize.Height / 8 : 0, _picPlayer.cardWidth, UnoSize.Height); ;
            picPlayer.Image = imgPlayer;
        }

        private void PicPlayer_MouseDown(object sender, MouseEventArgs e)
        {
            _picPlayer.isSelecting = true;
            PicPlayer_MouseMove(sender, e);
        }

        private void PicPlayer_MouseMove(object sender, MouseEventArgs e)
        {
            int i = (int)(e.X / _picPlayer.cardWidth);
            if (i >= _picPlayer.count || i < 0 || e.Y < 0 || e.Y > UnoSize.Height)
                return;
            pointing = i;
            toolTip.ToolTipTitle = GetNumberName(_picPlayer.picPlayer[i].number);
            if (i != _picPlayer.selecting)
            {
                if (i != _picPlayer.pointing)
                {
                    SetUsage(picPlayer, _picPlayer.picPlayer[i].color, _picPlayer.picPlayer[i].number);
                    _picPlayer.pointing = i;
                }
                if (_picPlayer.isSelecting)
                {
                    switch (e.Button)
                    {
                        case MouseButtons.Left:
                            if (!_picPlayer.checkeds[i])
                            {
                                if (!options.mnuPairs.Checked && _picPlayer.selected != -1)
                                    _picPlayer.checkeds[_picPlayer.selected] = false;
                                _picPlayer.checkeds[i] = true;
                                if (!options.mnuPairs.Checked)
                                    _picPlayer.selected = i;
                                PicPlayer_CheckedChanging(i, true);
                            }
                            else
                            {
                                _picPlayer.checkeds[i] = false;
                                PicPlayer_CheckedChanging(i, false);
                            }
                            break;
                        case MouseButtons.Right:
                            PicPlayer_CheckedChanging(i, !_picPlayer.checkeds[i]);
                            _picPlayer.checkeds[i] = false;
                            break;
                    }
                    _picPlayer.selecting = i;
                }
            }
        }

        private void PicPlayer_MouseUp(object sender, MouseEventArgs e)
        {
            _picPlayer.isSelecting = false;
            _picPlayer.selecting = -1;
            _picPlayer.pointing = -1;
            if (e.Y < 0 && isPlayer0sTurn)
            {
                _picPlayer.checkeds[pointing] = true;
                Play(0);
            }
            PicPlayer_CheckedChanged();
        }

        private void PicPlayer_MouseWheel(object sender, MouseEventArgs e)
        {
            if (isFair)
            {
                Card oldCard = _picPlayer.picPlayer[pointing];
                int count = pile.cards[oldCard.color, oldCard.number];
                pile.cards[oldCard.color, oldCard.number] = 1;
                Card[] p = GetPile();
                if (p.Length <= 1)
                    return;
                hasCheat = true;
                int i;
                for (i = 0; i < p.Length; ++i)
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
                _picPlayer.picPlayer[pointing] = newCard;
                PicPlayer_CheckedChanged();
            }
        }

        private void Play(byte player, Card[] playingCards = null)
        {
            List<Card> cards = new();
            if (options.isPlayingRecord)
            {
                if (player == 0)
                {
                    rdoUno.Checked = record.unos[0];
                    record.unos.RemoveAt(0);
                }
                cards = new List<Card>(record.players[player][0]);
                foreach (Card card in cards)
                    players[player].cards[card.color, card.number]--;
                if (player == 0 && cards.Count > 0)
                    Sort();
                record.players[player].RemoveAt(0);
                movingCard.color = record.colors[0];
                record.colors.RemoveAt(0);
            }
            else if (playingCards != null)
            {
                foreach (Card c in playingCards)
                    cards.Add(c);
            }
            else if (player == 0 && !isAutomatic)
            {
                List<Card> discardAll = new(), number = new();
                if (mnuPicPlayer.Checked)
                {
                    for (int i = 0; i < _picPlayer.count; ++i)
                    {
                        if (_picPlayer.checkeds[i])
                        {
                            Card card = _picPlayer.picPlayer[i];
                            switch (card.number)
                            {
                                default:
                                    cards.Add(card);
                                    break;
                                case UnoNumber.DiscardAll:
                                    discardAll.Add(card);
                                    break;
                                case UnoNumber.Number:
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
                            Card card = new()
                            {
                                color = GetColorId(c.BackColor),
                                number = GetNumberId(c.Text)
                            };
                            switch (c.Text)
                            {
                                default:
                                    cards.Add(card);
                                    break;
                                case UnoNumberName.DiscardAll:
                                    discardAll.Add(card);
                                    break;
                                case UnoNumberName.Number:
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
                for (int c = 0; c < cards.Count; ++c)
                {
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
                movingCard.player = player; movingCard.progress = 0;
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
                                    ++ons;
                            b = ons > 2;
                        }
                    }
                    if (b)
                    {
                        FrmColor c = new(colorList);
                        int colors = 0;
                        foreach (Card card in cards)
                        {
                            if (card.color == UnoColor.Black)
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
                                if (card.color != UnoColor.Magenta && card.color != UnoColor.Black)
                                {
                                    c.mnuColor.Items[card.color].Enabled = true;
                                    c.mnuColor.Items[card.color].BackColor = GetColor(card.color);
                                }
                            foreach (ToolStripMenuItem menu in c.mnuColor.Items) if (menu.Enabled) ++colors;
                        }
                        if (colors > 1)
                        {
                            c.ShowDialog();
                            movingCard.color = byte.Parse(c.Tag + "");
                        }
                        else if (cards[0].color != UnoColor.Magenta)
                        {
                            movingCard.color = cards.First().color;
                        }
                        else
                        {
                            movingCard.color = GetColorId(BackColor);
                        }
                    }
                    else
                    {
                        movingCard.color = GetColorId(BackColor);
                    }
                }
                if (!options.isPlayingRecord)
                {
                    record.players[player].Add(cards.ToArray());
                    record.colors.Add(movingCard.color);
                    if (player == 0)
                    {
                        record.unos.Add(rdoUno.Checked);
                    }
                }
                movingCard.number = cards[0].number;
                RemoveLabel(lblMovingCards, pnlMovingCards.Controls);
                AddLabel(lblMovingCards, cards.Count, pnlMovingCards.Controls);
                for (int c = 1; c < lblMovingCards.Count; ++c)
                {
                    SetCard(lblMovingCards[c], cards[c - 1].color, cards[c - 1].number);
                }

                if (lblMovingCards.Count <= swpcw >> 1) // swpcw: Screen's Width Per Card's Width
                {
                    for (int c = 1; c < lblMovingCards.Count; ++c)
                    {
                        lblMovingCards[c].Location = new Point(UnoSize.Width * (c - 1), 0);
                    }
                    pnlMovingCards.Size = new Size(UnoSize.Width * (lblMovingCards.Count - 1), UnoSize.Height);
                }
                else
                {
                    int lines = (int)((lblMovingCards.Count - 1f) / swpcw * 2f);
                    if (lines == 0)
                        lines = 1;
                    int cpl = lblMovingCards.Count / lines;
                    int w = (width >> 1) / cpl;
                    for (int c = 1; c < lblMovingCards.Count; ++c)
                    {
                        lblMovingCards[c].Location = new Point(w * (int)((c - 1f) % cpl),
                            UnoSize.Height * (int)((c - 1f) / cpl));
                    }
                    pnlMovingCards.Size = new Size(width >> 1, UnoSize.Height * lines);
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

        private Card[] PlayersCards(byte player)
        {
            List<Card> cards = new();
            for (byte color = 0; color <= UnoColor.MaxValue; ++color)
                for (byte number = 0; number <= UnoNumber.MaxValue; ++number)
                    for (int c = 1; c <= players[player].cards[color, number]; ++c)
                    {
                        Card card = new()
                        {
                            color = color,
                            number = number
                        };
                        cards.Add(card);
                    };
            return cards.ToArray();
        }

        private int PlayersCardsCount(byte player)
        {
            int i = 0;
            for (byte c = 0; c <= UnoColor.MaxValue; ++c)
                for (byte n = 0; n <= UnoNumber.MaxValue; ++n)
                    i += players[player].cards[c, n];
            return i;
        }

        private void PlayersTurn(byte player, bool turn = true, int dbp = 0, bool delay = true)
        {
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
                if (tradingHands.player1 < 4)
                {
                    if (player == 0 && !isAutomatic && tradingHands.player2 == 5)
                    {
                        TradeHands t = new();
                        for (byte p = 1; p <= 3; ++p)
                        {
                            if (lblPlayers[p].Visible)
                            {
                                t.mnuTradeHands.Items[p].Enabled = true;
                                t.mnuTradeHands.Items[p].Text = p switch { 1 => "◀", 2 => "▲", 3 => "▶", _ => "▼" };
                            }
                        }
                        t.ShowDialog();
                        tradingHands.player2 = (byte)t.Tag;
                        record.tradeHands.Add(tradingHands.player2);
                    }
                    TradeHands();
                    return;
                }
                if (downpour.count > 0)
                {
                    DownpourDraw(player);
                    return;
                }
                if (skip > 0 || skips[player] > 0)
                {
                    if (options.mnuSkipPlayers.Checked)
                    {
                        Action(player, $"禁手 ({skip})");
                        --skip;
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
                movingCard.dbp = dbp; movingCard.player = player; movingCard.progress = 0; movingCard.quickly = false;
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
                    movingCard.dbp = 0;
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
                if (options.mnuThinking.Checked && player > 0 && !movingCard.drew && timThinking.Tag.ToString().Split(char.Parse(","))[0] == "4")
                {
                    Action(player, playerNames[player] + " 的回合");
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

        private void RefillPile()
        {
            int decks = int.Parse(options.txtDecks.Text);
            foreach (byte c in colorList)
            {
                pile.cards[c, 0] = decks - GetOnplayersCards(c, 0);
                for (byte n = 1; n <= 9; ++n)
                    pile.cards[c, n] = 2 * decks - GetOnplayersCards(c, n);
                if (options.mnuDos.Checked)
                {
                    pile.cards[c, 10] = 2 * decks - GetOnplayersCards(c, 10);
                    pile.cards[c, UnoNumber.Number] = 2 * decks - GetOnplayersCards(c, UnoNumber.Number);
                }
                pile.cards[c, UnoNumber.Skip] = 2 * decks - GetOnplayersCards(c, UnoNumber.Skip);
                pile.cards[c, UnoNumber.Reverse] = 2 * decks - GetOnplayersCards(c, UnoNumber.Reverse);
                pile.cards[c, UnoNumber.Draw2] = 2 * decks - GetOnplayersCards(c, UnoNumber.Draw2);
                if (options.mnuFlip.Checked)
                {
                    pile.cards[c, UnoNumber.SkipEveryone] = 2 * decks - GetOnplayersCards(c, UnoNumber.SkipEveryone);
                    pile.cards[c, UnoNumber.Draw1] = 2 * decks - GetOnplayersCards(c, UnoNumber.Draw1);
                    pile.cards[c, UnoNumber.Draw5] = 2 * decks - GetOnplayersCards(c, UnoNumber.Draw5);
                }
                if (options.mnuAttack.Checked)
                {
                    pile.cards[c, UnoNumber.DiscardAll] = decks - GetOnplayersCards(c, UnoNumber.DiscardAll);
                    if (options.mnuTradeHands.Checked)
                        pile.cards[c, UnoNumber.TradeHands] = decks - GetOnplayersCards(c, UnoNumber.TradeHands);
                }
                if (options.mnuBlank.Checked)
                    pile.cards[c, UnoNumber.Blank] = decks - GetOnplayersCards(c, UnoNumber.Blank);
            }
            if (options.mnuDos.Checked)
            {
                pile.cards[UnoColor.Black, 2] = 4 * decks - GetOnplayersCards(UnoColor.Black, 2);
            }
            if (options.mnuBlank.Checked && options.mnuMagentaBlank.Checked)
                pile.cards[UnoColor.Magenta, UnoNumber.Blank] = 1 * decks - GetOnplayersCards(UnoColor.Magenta, UnoNumber.Blank);
            if (options.mnuBlank.Checked && options.mnuBlackBlank.Checked)
                pile.cards[UnoColor.Black, UnoNumber.Blank] = 2 * decks - GetOnplayersCards(UnoColor.Black, UnoNumber.Blank);
            pile.cards[UnoColor.Black, UnoNumber.Wild] = 4 * decks - GetOnplayersCards(UnoColor.Black, UnoNumber.Wild);
            if (options.mnuWater.Checked)
            {
                pile.cards[UnoColor.Black, UnoNumber.WildDownpourDraw1] = decks - GetOnplayersCards(UnoColor.Black, UnoNumber.WildDownpourDraw1);
                pile.cards[UnoColor.Black, UnoNumber.WildDownpourDraw2] = decks - GetOnplayersCards(UnoColor.Black, UnoNumber.WildDownpourDraw2);
                pile.cards[UnoColor.Black, UnoNumber.WildDownpourDraw4] = decks - GetOnplayersCards(UnoColor.Black, UnoNumber.WildDownpourDraw4);
            }
            if (options.mnuFlip.Checked)
                pile.cards[UnoColor.Black, UnoNumber.Draw2] = 4 * decks - GetOnplayersCards(UnoColor.Black, UnoNumber.Draw2);
            pile.cards[UnoColor.Black, UnoNumber.WildDraw4] = 4 * decks - GetOnplayersCards(UnoColor.Black, UnoNumber.WildDraw4);
            if (options.mnuAttack.Checked)
            {
                pile.cards[UnoColor.Black, UnoNumber.WildDownpourDraw1] = 2 * decks - GetOnplayersCards(UnoColor.Black, UnoNumber.WildDownpourDraw1);
            }
            if (options.mnuFlip.Checked)
                pile.cards[UnoColor.Black, UnoNumber.WildDrawColor] = 4 * decks - GetOnplayersCards(UnoColor.Black, UnoNumber.WildDrawColor);
            if (options.mnuWildHitfire.Checked)
            {
                pile.cards[UnoColor.Black, UnoNumber.WildHitfire] = 2 * decks - GetOnplayersCards(UnoColor.Black, UnoNumber.WildHitfire);
            }
            if (options.mnuWildPunch.Checked)
                pile.cards[UnoColor.Black, UnoNumber.Reverse] = 4 * decks - GetOnplayersCards(UnoColor.Black, UnoNumber.Reverse);
            lblPile.Text = GetPile().Length + "";
        }

        private void RemoveChkPlayer()
        {
            while (chkPlayer.Count > 0)
            {
                pnlPlayer.Controls.Remove(chkPlayer[0]);
                chkPlayer.RemoveAt(0);
            }
        }

        private void RemoveLabel(List<Label> label, Control.ControlCollection controls = null)
        {
            controls ??= Controls;
            while (label.Count > 1)
            {
                controls.Remove(label[1]);
                label.RemoveAt(1);
            }
        }

        private void ResizeForm()
        {
            width = ClientRectangle.Width;
            height = ClientRectangle.Height - mnuGame.Height;
            lblPlayers[0].Location = new Point((width >> 1) - (UnoSize.Width >> 1), height - UnoSize.Height);
            lblPlayers[1].Location = new Point(0, (height >> 1) - (UnoSize.Height >> 1) + (mnuGame.Height >> 1));
            lblPlayers[2].Location = new Point((width >> 1) - (UnoSize.Width >> 1), mnuGame.Height);
            lblPlayers[3].Location = new Point(width - UnoSize.Width, (height >> 1) - (UnoSize.Height >> 1) + (mnuGame.Height >> 1));
            for (byte i = 0; i < 4; ++i)
                lblCounts[i].Location = new Point(lblPlayers[i].Left + (lblPlayers[i].Width >> 1) - (lblCounts[i].Width >> 1), lblPlayers[i].Top - lblCounts[i].Height);
            lblCounts[2].Top = lblPlayers[2].Top + UnoSize.Height;
            rdoUno.Location = new Point(width - rdoUno.Width, lblCounts[0].Top);
            pnlPlayer.Top = rdoUno.Top + rdoUno.Height;
            picPlayer.Top = rdoUno.Top + rdoUno.Height;
            hPlayer.Top = picPlayer.Top;
            hPlayer.Width = width;
            swpcw = width / UnoSize.Width;
            if (movingCard.isPlaying)
                Sort();
            lblCards[0].Location = new Point((width >> 1) - (UnoSize.Width >> 1), (height >> 1) - (UnoSize.Height >> 1));
            btnStart.Location = new Point((width >> 1) - (btnStart.Width >> 1), (height >> 1) - (btnStart.Height >> 1));
        }

        private double Rnd()
        {
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
                return b[0] / 256.0;
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

        private string SaveGame()
        {
            string s = "";
            for (byte p = 0; p <= 3; ++p)
            {
                for (byte c = 0; c <= UnoColor.MaxValue; ++c)
                {
                    for (byte n = 0; n <= UnoNumber.MaxValue; ++n)
                    {
                        s += players[p].cards[c, n] + "N";
                    }
                    s += "C";
                }
                s += "P";
            }
            s = s.Substring(0, s.Length - 3);
            s += "K"; // 0
            for (byte c = 0; c <= UnoColor.MaxValue; ++c)
            {
                for (byte n = 0; n <= UnoNumber.MaxValue; ++n)
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

        private void SetCard(Control card, byte color, byte number)
        {
            card.BackColor = GetColor(color);
            card.Text = GetNumber(number);
            if (mnuAppearance.Checked)
            {
                Image image = new Bitmap(ResourceUnoSize.Width, ResourceUnoSize.Height);
                Graphics graphics = Graphics.FromImage(image);
                graphics.DrawImage(imgUno, new Rectangle(0, 0, ResourceUnoSize.Width, ResourceUnoSize.Height), number * ResourceUnoSize.Width, color * ResourceUnoSize.Height, ResourceUnoSize.Width, ResourceUnoSize.Height, GraphicsUnit.Pixel);
                card.BackgroundImage = image;
                card.Font = new Font(card.Font.FontFamily, 1);
                graphics.Flush();
                graphics.Dispose();
            }
            card.MouseEnter += Card_Enter;
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

        private void SetUsage(Control card, byte color, byte number)
        {
            string usage = GetUsage(number);
            usage = color switch
            {
                UnoColor.Magenta => "表示任意顏色" + (usage == "" ? "" : "\n　　\t") + usage,
                UnoColor.Black => "任意指定颜色" + (usage == "" ? "" : "\n　　\t") + usage,
                _ => usage,
            };
            if (color == UnoColor.Black && number == UnoNumber.Reverse)
                usage += "\n　　\t玩家免受懲罰";
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
                for (byte c = 0; c <= UnoColor.MaxValue; ++c)
                    for (byte n = 0; n <= UnoNumber.MaxValue; ++n)
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
                            ++i;
                        }
                    }
            else
                for (byte n = 0; n <= UnoNumber.MaxValue; ++n)
                    for (byte c = 0; c <= UnoColor.MaxValue; ++c)
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
                            ++i;
                        }
                    }
            switch (player)
            {
                case 1: lblPlayers[1].Top = (height >> 1) - (lblPlayers[1].Height >> 1); break;
                case 2: lblPlayers[2].Left = (width >> 1) - (lblPlayers[2].Width >> 1); break;
                case 3: lblPlayers[3].Location = new Point(width - lblPlayers[3].Width, (height >> 1) - (lblPlayers[3].Height >> 1)); break;
            }
            lblPlayers[player].Text = cards;
        }

        private void SetDraw(int draw)
        {
            this.draw = draw;
            lblDraw.Text = draw + "";
        }

        private void SetDrawColor(int draw)
        {
            drawColor = draw;
            lblDraw.Text = draw + "";
        }

        private void Sort()
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
                    for (byte color = 0; color <= UnoColor.MaxValue; ++color)
                        for (byte number = 0; number <= UnoNumber.MaxValue; ++number)
                            for (int c = 1; c <= players[0].cards[color, number]; ++c)
                            {
                                SetCard(chkPlayer[i], color, number);
                                ++i;
                            }
                else
                    for (byte number = 0; number <= UnoNumber.MaxValue; ++number)
                        for (byte color = 0; color <= UnoColor.MaxValue; ++color)
                            for (int c = 1; c <= players[0].cards[color, number]; ++c)
                            {
                                SetCard(chkPlayer[i], color, number);
                                ++i;
                            }
                hPlayer.Visible = false;
                int top = mnuAppearance.Checked ? UnoSize.Height / 8 : 0, width = UnoSize.Width * chkPlayer.Count;
                if (width <= this.width)
                {
                    pnlPlayer.Left = (this.width >> 1) - (width >> 1);
                    pnlPlayer.Width = width;
                    for (i = 0; i < chkPlayer.Count; ++i)
                        chkPlayer[i].Location = new Point(UnoSize.Width * i, top);
                }
                else if (width > this.width * 8 && mnuScrollBar.Checked)
                {
                    pnlPlayer.Left = 0;
                    pnlPlayer.Width = width;
                    for (i = 0; i < chkPlayer.Count; ++i)
                        chkPlayer[i].Location = new Point(UnoSize.Width * i, top);
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
                    for (i = 0; i < chkPlayer.Count; ++i)
                        chkPlayer[i].Location = new Point(this.width / chkPlayer.Count * i, top);
                }
                if (!options.Visible && chkPlayer.Count > 0)
                {
                    chkPlayer[0].Focus();
                }
            }
        }

        private void TimPileToCenter_Tick(object sender, EventArgs e)
        {
            lblMovingCards[0].Location = new Point(lblCards[0].Left / distance * movingCard.progress, lblCards[0].Top / distance * movingCard.progress + mnuGame.Height);
            if (lblMovingCards[0].Left >= lblCards[0].Left && lblMovingCards[0].Top >= lblCards[0].Top)
            {
                movingCard.progress = 0;
                Card[] pile = GetPile();
                Card rndCard;
                if (!options.isPlayingRecord)
                {
                    rndCard = pile[(int)(pile.Length * Rnd())];
                }
                else
                {
                    rndCard = record.pile[0];
                    record.pile.RemoveAt(0);
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
                    movingCard.progress = 0;
                    return;
                }
                if (!options.isPlayingRecord)
                    record.pile.Add(rndCard);
                BackColor = lblCards[1].BackColor;
                timPileToCenter.Enabled = false;
                lblMovingCards[0].Location = new Point(-UnoSize.Width, -UnoSize.Height);
                movingCard.isPlaying = true;
                if (!options.isPlayingRecord)
                {
                    reverse = (int)(2f * Rnd()) == 0;
                    record.reverse = reverse;
                }
                else
                    reverse = record.reverse;
                if (options.keys.Length == 0)
                {
                    byte nextPlayer;
                    if (!options.isPlayingRecord)
                    {
                        if (options.mnuFirst.Checked)
                            nextPlayer = 0;
                        else
                            nextPlayer = NextPlayer((byte)(4 * Rnd()));
                        record.firstTurn = nextPlayer;
                    }
                    else
                    {
                        nextPlayer = record.firstTurn;
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
                            case UnoNumberName.Skip:
                                if (options.mnuSkipPlayers.Checked)
                                    skip = 1;
                                else
                                    skips[nextPlayer] = 1;
                                break;
                            case UnoNumberName.Draw1:
                                AddDraw(1);
                                break;
                            case UnoNumberName.Draw2:
                                AddDraw(2);
                                break;
                            case UnoNumberName.Draw5:
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
            else ++movingCard.progress;
        }

        private void TimPileToPlayers_Tick(object sender, EventArgs e)
        {
            if (movingCard.quickly)
                lblMovingCards[0].Location = lblPlayers[movingCard.player].Location;
            else
                lblMovingCards[0].Location = new Point(lblPlayers[movingCard.player].Left / distance * movingCard.progress, lblPlayers[movingCard.player].Top / distance * movingCard.progress + mnuGame.Height);
            if (lblMovingCards[0].Left >= lblPlayers[movingCard.player].Left && lblMovingCards[0].Top >= lblPlayers[movingCard.player].Top)
            {
                Card[] pile = GetPile();
                Card rndCard;
                if (!options.isPlayingRecord)
                {
                    rndCard = pile[(int)(pile.Length * Rnd())];
                    record.pile.Add(rndCard);
                }
                else
                {
                    rndCard = record.pile[0];
                    record.pile.RemoveAt(0);
                }
                this.pile.cards[rndCard.color, rndCard.number]--;
                lblPile.Text = pile.Length - 1 + "";
                players[movingCard.player].cards[rndCard.color, rndCard.number]++;
                lblCounts[movingCard.player].Text = PlayersCardsCount(movingCard.player) + "";
                if (movingCard.player == 0 && !movingCard.quickly)
                    Sort();
                if (movingCard.isPlaying)
                {
                    bool drawAll = false;
                    lblMovingCards[0].Location = new Point(-UnoSize.Width, -UnoSize.Height);
                    timPileToPlayers.Enabled = false;
                draw:
                    if (movingCard.dbp <= 0 && movingCard.downpour <= 0)
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
                    if (draw <= 0 && drawColor <= 0 && movingCard.dbp <= 0 && movingCard.downpour <= 0)
                    {
                        if (gameOver < 4 && movingCard.downpour <= 0)
                        {
                            GameOver();
                        }
                        if (movingCard.dbp <= 0 && movingCard.downpour <= 0)
                            movingCard.drew = !movingCard.unoDraw;
                        if (movingCard.player == 0 && movingCard.quickly)
                            Sort();
                        if (movingCard.unoDraw)
                        {
                            movingCard.unoDraw = false;
                            PlayersTurn(NextPlayer(movingCard.player), true, GetDbp());
                        }
                        else if (drawAll)
                        {
                            if (options.mnuDrawAllAndPlay.Checked)
                                PlayersTurn(movingCard.player);
                            else
                            {
                                movingCard.drew = false;
                                PlayersTurn(NextPlayer(movingCard.player), true, GetDbp());
                            }
                        }
                        else if (!options.mnuDrawAndPlay.Checked)
                        {
                            movingCard.drew = false;
                            PlayersTurn(NextPlayer(movingCard.player), true, GetDbp());
                        }
                        else
                            PlayersTurn(movingCard.player, true, 0, movingCard.player > 0);
                    }
                    else if (movingCard.dbp > 0)
                    {
                        --movingCard.dbp;
                        if (movingCard.dbp > 0)
                            CheckPile();
                        if (movingCard.dbp <= 0)
                            PlayersTurn(movingCard.player);
                        else
                            timPileToPlayers.Enabled = true;
                    }
                    else if (movingCard.downpour > 0)
                    {
                        --movingCard.downpour;
                        if (movingCard.downpour > 0)
                        {
                            CheckPile();
                            timPileToPlayers.Enabled = true;
                        }
                        else
                        {
                            byte nextPlayer = movingCard.player;
                        downpour_nextplayer:
                            nextPlayer = (byte)(!reverse ? nextPlayer == 3 ? 0 : nextPlayer + 1 : nextPlayer == 0 ? 3 : nextPlayer - 1);
                            if (nextPlayer == downpour.player)
                            {
                                if (movingCard.quickly)
                                    Sort();
                                if (gameOver < 4)
                                {
                                    GameOver();
                                }
                                downpour.count = 0;
                                PlayersTurn(NextPlayer(nextPlayer), true, GetDbp());
                            }
                            else if (!lblPlayers[nextPlayer].Visible)
                                goto downpour_nextplayer;
                            else
                            {
                                movingCard.downpour = downpour.count;
                                movingCard.player = nextPlayer;
                                timPileToPlayers.Enabled = true;
                            }
                        }
                    }
                }
                else
                {
                    byte lastPlayer = NextPlayer(record.firstGettingCard, true);
                    if (movingCard.player == lastPlayer)
                        if (int.Parse(lblCounts[lastPlayer].Text) >= int.Parse(options.txtDealt.Text)
                            || options.keys.Length > 0)
                        {
                            timPileToPlayers.Enabled = false;
                            if (movingCard.quickly)
                                Sort();
                            timPileToCenter.Enabled = true;
                        }
                    movingCard.player = NextPlayer(movingCard.player);
                }
                movingCard.progress = 0;
            }
            else ++movingCard.progress;
        }

        private void TimPlayersToCenter_Tick(object sender, EventArgs e)
        {
            switch (movingCard.player)
            {
                case 0:
                    pnlMovingCards.Location = new Point((width >> 1) - (pnlMovingCards.Width >> 1),
                        height - (lblPlayers[0].Top - lblCards[0].Top) / distance * movingCard.progress - (pnlMovingCards.Height >> 1) - (UnoSize.Height >> 1));
                    break;
                case 1:
                    pnlMovingCards.Location = new Point(lblCards[0].Left / distance * movingCard.progress - (pnlMovingCards.Width >> 1) + (UnoSize.Width >> 1),
                        (height >> 1) - (pnlMovingCards.Height >> 1));
                    break;
                case 2:
                    pnlMovingCards.Location = new Point((width >> 1) - (pnlMovingCards.Width >> 1),
                        lblCards[0].Top / distance * movingCard.progress - (pnlMovingCards.Height >> 1) + (UnoSize.Height >> 1));
                    break;
                case 3:
                    pnlMovingCards.Location = new Point(width - (lblPlayers[movingCard.player].Left - lblCards[0].Left) / distance * movingCard.progress - (pnlMovingCards.Width >> 1) - (UnoSize.Width >> 1),
                        (height >> 1) - (pnlMovingCards.Height >> 1));
                    break;
            }
            if (movingCard.progress >= distance)
                goto arrived;
            ++movingCard.progress;
            return;
        arrived:
            timPlayersToCenter.Enabled = false;
            movingCard.progress = 0;
            pnlMovingCards.Location = new Point((width >> 1) - (pnlMovingCards.Width >> 1), (height >> 1) - (pnlMovingCards.Height >> 1));
            RemoveLabel(lblCards);
            AddLabel(lblCards, lblMovingCards.Count - 1);
            for (int c = 1; c < lblCards.Count; ++c)
            {
                SetCard(lblCards[c], GetColorId(lblMovingCards[c].BackColor), GetNumberId(lblMovingCards[c].Text));
                lblCards[c].BringToFront();
                lblCards[c].Location = new Point(lblMovingCards[c].Left + pnlMovingCards.Left, lblMovingCards[c].Top + pnlMovingCards.Top);
            }
            RemoveLabel(lblMovingCards, pnlMovingCards.Controls);
            pnlMovingCards.Location = new Point(-pnlMovingCards.Width, -pnlMovingCards.Height);
            BackColor = GetColor(movingCard.color);
            lblCounts[movingCard.player].Text = PlayersCardsCount(movingCard.player) + "";
            bool reversed = false;
            byte ons = 0;
            foreach (Label p in lblPlayers)
                if (p.Visible)
                    ++ons;
            switch (lblCards[1].Text)
            {
                case "0" when options.mnuSevenZero.Checked:
                    tradingHands.player1 = movingCard.player;
                    tradingHands.player2 = 4;
                    goto end_action;
                case "7" when options.mnuSevenZero.Checked:
                case UnoNumberName.TradeHands when options.mnuTradeHands.Checked:
                    if (lblPlayers[movingCard.player].Visible)
                    {
                        tradingHands.player1 = movingCard.player;
                        if (!options.isPlayingRecord)
                        {
                            if (movingCard.player != 0 || isAutomatic)
                            {
                                byte fp = movingCard.player, p = NextPlayer(movingCard.player);
                                int fc = int.MaxValue;
                                while (p != movingCard.player)
                                {
                                    int c = PlayersCardsCount(p);
                                    if (c < fc)
                                    {
                                        fc = c;
                                        fp = p;
                                    }
                                    p = NextPlayer(p);
                                }
                                tradingHands.player2 = fp;
                                record.tradeHands.Add(tradingHands.player2);
                            }
                            else
                                tradingHands.player2 = 5;
                        }
                        else
                        {
                            tradingHands.player2 = record.tradeHands[0];
                            record.tradeHands.RemoveAt(0);
                        }
                    }
                    goto end_action;
            }
            for (int i = 1; i < lblCards.Count; ++i)
            {
                Label l = lblCards[i];
                if (l.Text == UnoNumberName.Reverse || l.Text == options.txtBlankText.Text && options.mnuBlankReverse.Checked)
                {
                    if (ons == 2 && lblPlayers[movingCard.player].Visible)
                        reversed = true;
                    else
                        reverse = !reverse;
                }
            }
            for (int i = 1; i < lblCards.Count; ++i)
            {
                Label l = lblCards[i];
                if (l.Text == UnoNumberName.Null)
                    continue;
                switch (l.Text)
                {
                    case UnoNumberName.Skip:
                        if (options.mnuSkipPlayers.Checked) ++skip;
                        else skips[NextPlayer(movingCard.player)]++;
                        break;
                    case UnoNumberName.SkipEveryone:
                        if (lblPlayers[movingCard.player].Visible)
                            reversed = true;
                        break;
                    case UnoNumberName.Reverse:
                        break;
                    case UnoNumberName.Draw1:
                        AddDraw(1);
                        break;
                    case UnoNumberName.Draw2:
                        AddDraw(2);
                        break;
                    case UnoNumberName.Draw5:
                        AddDraw(5);
                        break;
                    case UnoNumberName.WildDownpourDraw1:
                        downpour.count += options.mnuDoubleDraw.Checked ? 2 : 1;
                        break;
                    case UnoNumberName.WildDownpourDraw2:
                        downpour.count += 2 * (options.mnuDoubleDraw.Checked ? 2 : 1);
                        break;
                    case UnoNumberName.WildDownpourDraw4:
                        downpour.count += 4 * (options.mnuDoubleDraw.Checked ? 2 : 1);
                        break;
                    case UnoNumberName.WildDraw4:
                        AddDraw(4);
                        break;
                    case UnoNumberName.WildDrawColor:
                        SetDrawColor(drawColor + 1);
                        break;
                    case UnoNumberName.WildHitfire:
                        SetDraw(int.Parse(lblPile.Text));
                        break;
                    default:
                        if (l.Text == options.txtBlankText.Text)
                        {
                            if (options.mnuSkipPlayers.Checked)
                                skip += int.Parse(options.txtBlankSkip.Text);
                            else
                                skips[NextPlayer(movingCard.player)] += int.Parse(options.txtBlankSkip.Text);
                            AddDraw(int.Parse(options.txtBlankDraw.Text));
                            downpour.count += int.Parse(options.txtBlankDownpourDraw.Text) * (options.mnuDoubleDraw.Checked ? 2 : 1);
                        }
                        break;
                }
            }
        end_action:
            if (gameOver < 4)
            {
                if (downpour.count > 0)
                {
                    DownpourDraw(movingCard.player);
                }
                return;
            }
            if (options.mnuUno.Checked && movingCard.player == 0 && (PlayersCardsCount(0) == 1 && !rdoUno.Checked || PlayersCardsCount(0) != 1 && rdoUno.Checked))
            {
                Action(0, "UNO? +2");
                AddDraw(2);
                PlayersTurn(0, false);
                rdoUno.Checked = false;
                movingCard.unoDraw = true;
            }
            movingCard.drew = false;
            if (movingCard.unoDraw)
                timUno.Enabled = true;
            else if (reversed)
                PlayersTurn(movingCard.player, true, GetDbp());
            else if (tradingHands.player1 < 4)
                PlayersTurn(movingCard.player, true, GetDbp());
            else if (downpour.count > 0)
            {
                PlayersTurn(movingCard.player, true, GetDbp());
            }
            else
                PlayersTurn(movingCard.player = NextPlayer(movingCard.player), true, GetDbp());
        }

        private void TimThinking_Tick(object sender, EventArgs e)
        {
            timThinking.Enabled = false;
            string[] tag = timThinking.Tag.ToString().Split(char.Parse(","));
            PlayersTurn(byte.Parse(tag[0]), true, int.Parse(tag[1]));
        }

        private void TimTradeHands_Tick(object sender, EventArgs e)
        {
            if (tradingHands.player2 < 4)
            {
                int x1 = tradingHands.x[tradingHands.player1], x2 = tradingHands.x[tradingHands.player2],
                    y1 = tradingHands.y[tradingHands.player1], y2 = tradingHands.y[tradingHands.player2];
                lblPlayers[tradingHands.player1].Location = new Point(x1 + (x2 - x1) / distance * movingCard.progress,
                                                                      y1 + (y2 - y1) / distance * movingCard.progress);
                lblPlayers[tradingHands.player2].Location = new Point(x1 + (x2 - x1) / distance * (distance - movingCard.progress),
                                                                      y1 + (y2 - y1) / distance * (distance - movingCard.progress));
            }
            else
            {
                int[] x = tradingHands.x.ToArray(), y = tradingHands.y.ToArray();
                for (byte p = 0; p <= 3; ++p)
                {
                    if (lblPlayers[p].Visible)
                    {
                        byte np = NextPlayer(p);
                        lblPlayers[p].Location = new Point(x[p] + (x[np] - x[p]) / distance * movingCard.progress,
                                                           y[p] + (y[np] - y[p]) / distance * movingCard.progress);
                    }
                }
            }
            if (movingCard.progress >= distance)
                goto arrived;
            ++movingCard.progress;
            return;
        arrived:
            timTradeHands.Enabled = false;
            movingCard.progress = 0;
            lblPlayers[0].Location = new Point((width >> 1) - (UnoSize.Width >> 1), ClientRectangle.Height - UnoSize.Height);
            lblPlayers[1].Location = new Point(0, (height >> 1) - (UnoSize.Height >> 1) + (mnuGame.Height >> 1));
            lblPlayers[2].Location = new Point((width >> 1) - (UnoSize.Width >> 1), mnuGame.Height);
            lblPlayers[3].Location = new Point(width - UnoSize.Width, (height >> 1) - (UnoSize.Height >> 1) + (mnuGame.Height >> 1));
            if (tradingHands.player2 < 4)
            {
                (players[tradingHands.player2].cards, players[tradingHands.player1].cards) =
                    (players[tradingHands.player1].cards, players[tradingHands.player2].cards);
            }
            else
            {
                byte pp, p = movingCard.player;
                int[,] i = players[movingCard.player].cards;
                pp = NextPlayer(p, true);
                do
                {
                    players[p].cards = players[pp].cards;
                    p = pp;
                }
                while ((pp = NextPlayer(p, true)) != movingCard.player);
                players[p].cards = i;
            }
            if (tradingHands.player1 == 0 || tradingHands.player2 == 0 || tradingHands.player2 == 4)
            {
                lblPlayers[0].BackgroundImage = null;
                if (mnuPicPlayer.Checked)
                    picPlayer.Visible = true;
                Sort();
                pnlPlayer.BringToFront();
                picPlayer.BringToFront();
                lblCounts[0].Text = PlayersCardsCount(0).ToString();
            }
            tradingHands.player1 = 4;
            for (byte p = 1; p <= 3; ++p)
            {
                if (options.mnuRevealable.Checked)
                    Reveal(p);
                lblCounts[p].Text = PlayersCardsCount(p).ToString();
            }
            PlayersTurn(NextPlayer(movingCard.player), true, GetDbp());
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
            ++gametime;
            int h = (int)(gametime / 3600f), m = (int)(gametime / 60f);
            lblWatch.Text = h + "°" + (m - h * 60) + "′" + (gametime - m * 60) + "″";
        }

        private void ToolTip_Popup(object sender, PopupEventArgs e)
        {
        }

        private void TradeHands()
        {
            tradingHands.x = new List<int>(lblPlayers.Select(l => l.Left));
            tradingHands.y = new List<int>(lblPlayers.Select(l => l.Top));
            movingCard.progress = 0;
            movingCard.quickly = false;
            if (tradingHands.player1 == 0 || tradingHands.player2 == 0 || tradingHands.player2 == 4)
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
            for (byte p = 1; p <= 3; ++p)
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
            Font font = new(new FontFamily("MS Gothic"), 84f);
            float fontSize = 84f;
            SizeF ms = gpsUno.MeasureString(text, font);
            while (ms.Width > ResourceUnoSize.Width && fontSize >= 21)
            {
                --fontSize;
                font = new Font(font.FontFamily, fontSize);
                ms = gpsUno.MeasureString(text, font);
            }
            for (byte b = 0; b <= UnoColor.MaxValue; ++b)
                gpsUno.DrawString(text, font, brush, new RectangleF(UnoNumber.Blank * ResourceUnoSize.Width, b * ResourceUnoSize.Height, ResourceUnoSize.Width, ResourceUnoSize.Height), new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            lblPile.Top = mnuGame.Height;
            lblPile.BackgroundImageLayout = ImageLayout.Stretch;
            lblPile.BackgroundImage = Properties.Resources.uno_pile;
            isFair = options.mnuBotPro.Checked;
            for (byte b = 0; b < 4; ++b)
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
                lblPlayers[b].Size = new Size(UnoSize.Width, UnoSize.Height);
                lblPlayers[b].Tag = b;
                if (b > 0)
                    lblPlayers[b].BorderStyle = BorderStyle.FixedSingle;
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
                    playerNames = new string[] { options.txtPlayer0.Text, options.txtPlayer1.Text, options.txtPlayer2.Text, options.txtPlayer3.Text };
                    break;
                case 2:
                    ons = new bool[4] { options.on0, true, false, true };
                    playerNames = new string[] { options.txtPlayer0.Text, options.txtPlayer1.Text, "", options.txtPlayer2.Text };
                    break;
                case 1:
                    ons = new bool[4] { options.on0, false, true, false };
                    playerNames = new string[] { options.txtPlayer0.Text, "", options.txtPlayer1.Text, "" };
                    break;
                default:
                    ons = new bool[4] { options.on0, false, false, false };
                    playerNames = new string[] { options.txtPlayer0.Text, "", "", "" };
                    break;
            }
            for (int o = 0; o <= 3; ++o)
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
                    UnoColor.Red, UnoColor.Yellow, UnoColor.Green, UnoColor.Cyan, UnoColor.Blue, UnoColor.Magenta
                };
            }
            else if (options.mnuCyan.Checked)
            {
                colorList = new byte[5]
                {
                    UnoColor.Red, UnoColor.Yellow, UnoColor.Green, UnoColor.Cyan, UnoColor.Blue
                };
            }
            else if (options.mnuMagenta.Checked)
            {
                colorList = new byte[5]
                {
                    UnoColor.Red, UnoColor.Yellow, UnoColor.Green, UnoColor.Blue, UnoColor.Magenta
                };
            }
            RefillPile();
            for (byte p = 0; p < 4; ++p)
                players[p] = new Cards();
            if (options.mnuSevenZero.Checked)
            {
                List<byte> l = new(playlist);
                l.RemoveAt(l.IndexOf(7));
                l.RemoveAt(l.IndexOf(0));
                l.Insert(l.IndexOf(UnoNumber.Wild), 7);
                l.Insert(l.IndexOf(UnoNumber.Wild), 0);
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
            movingCard.quickly = true;
            if (isPlayer0sTurn && movingCard.drew && !options.mnuDrawToMatch.Checked)
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

    public static class UnoColor
    {
        public const byte
            Red = 0,
            Yellow = 1,
            Green = 2,
            Cyan = 3,
            Blue = 4,
            Magenta = 5,
            Black = 6,
            White = 7,
            MaxValue = 7;
    }

    public static class UnoNumber
    {
        public const byte Skip = 11,
            SkipEveryone = 12,
            Reverse = 13,
            Draw1 = 14,
            Draw2 = 15,
            Draw5 = 16,
            DiscardAll = 17,
            TradeHands = 18,
            Number = 19,
            Blank = 20,
            Wild = 21,
            WildDownpourDraw1 = 22,
            WildDownpourDraw2 = 23,
            WildDownpourDraw4 = 24,
            WildDraw4 = 25,
            WildDrawColor = 26,
            WildHitfire = 27,
            Null = 28,
            MaxValue = 28;
    }

    public class Card
    {
        public byte color = UnoColor.MaxValue;
        public byte number = UnoNumber.MaxValue;

        public Card()
        {
            color = UnoColor.MaxValue;
            number = UnoNumber.MaxValue;
        }

        public Card(byte color, byte number)
        {
            this.color = color;
            this.number = number;
        }
    }

    public class MovingCard
    {
        public byte color = 0, number = 0, player = 0;
        public bool drew = false, isPlaying = false, quickly = false, unoDraw = false;
        public int dbp = 0, downpour = -1, progress = 0;
    }

    public class Cards
    {
        public int[,] cards = new int[UnoColor.MaxValue + 1, UnoNumber.MaxValue + 1];
    }

    public class Downpour
    {
        public byte player = 4;
        public int count = 0;
    }

    public class PicPlayer
    {
        public bool isSelecting = false;
        public bool[] checkeds;
        public float cardWidth = 0;
        public int count = 0, pointing = -1, selected = -1, selecting = -1;
        public List<Card> picPlayer = new();
    }

    public class Record
    {
        public bool reverse;
        public byte firstGettingCard, firstTurn;
        public List<byte> colors = new(), tradeHands = new();
        public List<bool> unos = new();
        public List<Card> pile = new();
        public List<Card[]>[] players = new List<Card[]>[4] { new(), new(), new(), new() };
    }

    public class ResourceUnoSize
    {
        public const int Width = 120;
        public const int Height = 160;
    }

    public class TradingHands
    {
        public byte player1 = 4, player2 = 4;
        public List<int> x, y;
    }

    public class UnoSize
    {
        public const int Width = 90;
        public const int Height = 120;
    }

    public class UnoNumberName
    {
        public const string Skip = "⊘";
        public const string SkipEveryone = "⟳";
        public const string Reverse = "⇅";
        public const string Draw1 = "+1";
        public const string Draw2 = "+2";
        public const string Draw5 = "+5";
        public const string DiscardAll = "×";
        public const string TradeHands = "<>";
        public const string Number = "#";
        public const string Wild = "∷";
        public const string WildDownpourDraw1 = "!1";
        public const string WildDownpourDraw2 = "!2";
        public const string WildDownpourDraw4 = "!4";
        public const string WildDraw4 = "+4";
        public const string WildDrawColor = "↑";
        public const string WildHitfire = "+?";
        public const string Null = "";
    }
}
