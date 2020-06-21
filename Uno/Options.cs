using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Uno {
	public partial class Options : Form {

        bool isPlaying = false;
        public bool on0 = true;
        public byte ons = 3;
        public bool isPlayingRecord = false;
        public int animation = 20, multiplier = 0, money = 0;
        public string[] keys = {};

		public Options() {
			InitializeComponent();
            mnuOptions.Font = new Font(mnuOptions.Font.FontFamily, mnuOptions.Font.Size * 2);
            Size = new Size(mnuQuit.Width * 8, mnuOptions.Height);
            string s = Interaction.GetSetting("UNO", "GAME", "SAVE", "");
            if (s != "")
            {
                mnuLoadGame.Text = s.Substring(s.LastIndexOf('K') + 1);
                mnuLoadGame.Enabled = true;
            }
            s = Interaction.GetSetting("UNO", "GAME", "AUTO", "");
            if (s != "")
            {
                mnuLoadAutosavedGame.Text = "(自動) " + s.Substring(s.LastIndexOf('K') + 1);
                mnuLoadAutosavedGame.Enabled = true;
            }
            s = Interaction.GetSetting("UNO", "RULES", "SAVE", "");
            if (s != "")
            {
                mnuLoadRules.Text = s.Substring(s.LastIndexOf('K') + 1);
                mnuLoadRules.Enabled = true;
            }
            s = Interaction.GetSetting("UNO", "RECORD", "RULES", "");
            if (s != "")
            {
                mnuPlayRecord.Text = "(錄像) " + s.Substring(s.LastIndexOf('K') + 1);
                mnuPlayRecord.Enabled = true;
            }
            money = int.Parse(Interaction.GetSetting("UNO", "ACCOUNT", "MONEY", "0"));
            mnuMe.Text = "$" + money;
        }

        private void KeepSafeForCheating_CheckedChanged(object sender, EventArgs e)
        {
            if (!mnuBeginner.Checked && mnuRevealable.Checked)
            {
                MessageBox.Show("正規比賽中禁止明牌！", "正規比賽", MessageBoxButtons.OK, MessageBoxIcon.Error);
                mnuRevealable.Checked = false;
            }
        }

        private void LoadGame(string save)
        {
            string[] keys = save.Split(char.Parse("K"));
            if (save[0] != 'K')
                this.keys = keys;
            on0 = bool.Parse(keys[10]);
            txtPlayer0.Visible = on0;
            mnuAddRemovePlayer.Text = (on0 ? "移除" : "新增") + "玩家";
            ons = byte.Parse(keys[11]);
            ShowPlayers();
            txtPlayer0.Text = keys[12];
            txtPlayer1.Text = keys[13];
            txtPlayer2.Text = keys[14];
            txtPlayer3.Text = keys[15];
            txtDealt.Text = keys[16];
            txtDecks.Text = keys[17];
            mnuBeginner.Checked = bool.Parse(keys[18]);
            mnuPro.Checked = bool.Parse(keys[19]);
            mnuCheater.Checked = bool.Parse(keys[20]);
            mnuBotBeginner.Checked = bool.Parse(keys[21]);
            mnuBotPro.Checked = bool.Parse(keys[22]);
            mnuBlank.Checked = bool.Parse(keys[23]);
            mnuMagentaBlank.Checked = bool.Parse(keys[24]);
            mnuBlackBlank.Checked = bool.Parse(keys[25]);
            txtBlankText.Text = keys[26];
            txtBlankSkip.Text = keys[27];
            mnuBlankReverse.Checked = bool.Parse(keys[28]);
            txtBlankDraw.Text = keys[29];
            txtBlankDownpourDraw.Text = keys[30];
            mnuWater.Checked = bool.Parse(keys[31]);
            mnuAttack.Checked = bool.Parse(keys[32]);
            mnuWildHitfire.Checked = bool.Parse(keys[33]);
            mnuTradeHands.Checked = bool.Parse(keys[34]);
            mnuDos.Checked = bool.Parse(keys[35]);
            mnuFlip.Checked = bool.Parse(keys[36]);
            mnuCyan.Checked = bool.Parse(keys[37]);
            mnuMagenta.Checked = bool.Parse(keys[38]);
            mnuRevealable.Checked = bool.Parse(keys[39]);
            mnuPairs.Checked = bool.Parse(keys[40]);
            mnuStacking.Checked = bool.Parse(keys[41]);
            mnuPlayOrDrawAll.Checked = bool.Parse(keys[42]);
            mnuDrawToMatch.Checked = bool.Parse(keys[43]);
            mnuDrawAllAndPlay.Checked = bool.Parse(keys[44]);
            mnuDrawAndPlay.Checked = bool.Parse(keys[45]);
            mnuDoubleDraw.Checked = bool.Parse(keys[46]);
            mnuDrawBeforePlaying.Checked = bool.Parse(keys[47]);
            mnuDrawTwoBeforePlaying.Checked = bool.Parse(keys[48]);
            mnuSevenZero.Checked = bool.Parse(keys[49]);
            mnuSkipPlayers.Checked = bool.Parse(keys[50]);
            mnuSkipTimes.Checked = bool.Parse(keys[51]);
            mnuOneWinner.Checked = bool.Parse(keys[52]);
            mnuOneLoser.Checked = bool.Parse(keys[53]);
            mnuUno.Checked = bool.Parse(keys[54]);
            mnuMultiply0.Checked = bool.Parse(keys[55]);
            mnuMultiply1.Checked = bool.Parse(keys[56]);
            mnuMultiply10000.Checked = bool.Parse(keys[57]);
            mnuMultiply10.Checked = bool.Parse(keys[58]);
            mnuMultiply100.Checked = bool.Parse(keys[59]);
            mnuMultiply1000.Checked = bool.Parse(keys[60]);
            mnuCheat.Checked = bool.Parse(keys[61]);
        }

        private void MnuAddBot_Click(object sender, EventArgs e)
        {
            ons++;
            ShowPlayers();
        }

        private void MnuAddRemovePlayer_Click(object sender, EventArgs e)
        {
            on0 = !on0;
            txtPlayer0.Visible = on0;
            mnuAddRemovePlayer.Text = (on0 ? "移除" : "新增") + "玩家";
        }

        private void MnuAdvanced_Click(object sender, EventArgs e)
        {
            txtDealt.Text = "11";
            txtDecks.Text = "1";
            mnuBlank.Checked = true;
            mnuMagentaBlank.Checked = true;
            mnuBlackBlank.Checked = false;
            mnuWater.Checked = false;
            mnuAttack.Checked = false;
            mnuWildHitfire.Checked = false;
            mnuTradeHands.Checked = false;
            mnuDos.Checked = false;
            mnuFlip.Checked = false;
            mnuCyan.Checked = false;
            mnuMagenta.Checked = false;
            mnuPairs.Checked = true;
            mnuStacking.Checked = true;
            mnuPlayOrDrawAll.Checked = true;
            mnuDrawToMatch.Checked = true;
            mnuDrawAllAndPlay.Checked = true;
            mnuDrawAndPlay.Checked = true;
            mnuDoubleDraw.Checked = false;
            mnuDrawBeforePlaying.Checked = false;
            mnuDrawTwoBeforePlaying.Checked = false;
            mnuSevenZero.Checked = false;
            mnuSkipPlayers.Checked = false;
            mnuSkipTimes.Checked = true;
            mnuOneWinner.Checked = false;
            mnuOneLoser.Checked = true;
            mnuUno.Checked = true;
        }

        private void MnuAnimation_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            if (menuItem.Checked)
            {
                foreach (ToolStripMenuItem mnu in mnuAnimation.DropDownItems)
                {
                    if (mnu != menuItem)
                        mnu.Checked = false;
                }
            }
            animation = int.Parse((string)menuItem.Tag);
        }

        private void MnuAttack_CheckedChanged(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem mnu in mnuAttack.DropDownItems)
                mnu.Visible = mnuAttack.Checked;
        }

        private void MnuBack_Click(object sender, EventArgs e)
        {
            mnuNew.Visible = true;
            mnuLoad.Visible = true;
            mnuCustom.Visible = true;
            mnuQuit.Visible = true;
            mnuStart.Visible = false;
            mnuQuick.Visible = false;
            mnuPlayer.Visible = false;
            mnuCards.Visible = false;
            mnuRules.Visible = false;
            mnuBack.Visible = false;
        }

        private void MnuBlank_CheckedChanged(object sender, EventArgs e)
        {
            foreach (ToolStripItem itm in mnuBlank.DropDownItems)
                itm.Visible = mnuBlank.Checked;
        }

        private void MnuBotDifficulty_Click(object sender, EventArgs e)
        {
            mnuBotBeginner.Checked = false;
            mnuBotPro.Checked = false;
            ((ToolStripMenuItem)sender).Checked = true;
        }

        private void MnuBuyIn_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;
            foreach (ToolStripMenuItem itm in mnuGamble.DropDownItems)
                itm.Checked = false;
            menuItem.Checked = true;
            multiplier = int.Parse(menuItem.Tag + "");
        }

        private void MnuCheat_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem_Click(sender, e);
            if (mnuGamble.Checked && mnuCheat.Checked)
            {
                mnuGamble.Checked = false;
                MessageBox.Show("無法在賭博中作弊!", "作弊", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void MnuCustom_Click(object sender, EventArgs e)
        {
            mnuNew.Visible = false;
            mnuLoad.Visible = false;
            mnuCustom.Visible = false;
            mnuQuit.Visible = false;
            mnuStart.Visible = true;
            mnuQuick.Visible = true;
            mnuPlayer.Visible = true;
            mnuCards.Visible = true;
            mnuRules.Visible = true;
            mnuBack.Visible = true;
        }

        private void MnuDifficulty_Click(object sender, EventArgs e)
        {
            mnuBeginner.Checked = false;
            mnuPro.Checked = false;
            mnuCheater.Checked = false;
            ((ToolStripMenuItem)sender).Checked = true;
        }

        private void MnuDurable_Click(object sender, EventArgs e)
        {
            txtDealt.Text = "56";
            txtDecks.Text = "10";
            mnuBlank.Checked = true;
            mnuMagentaBlank.Checked = true;
            mnuBlackBlank.Checked = true;
            mnuWater.Checked = true;
            mnuAttack.Checked = true;
            mnuWildHitfire.Checked = true;
            mnuTradeHands.Checked = true;
            mnuDos.Checked = true;
            mnuFlip.Checked = true;
            mnuCyan.Checked = true;
            mnuMagenta.Checked = false;
            mnuPairs.Checked = true;
            mnuStacking.Checked = true;
            mnuPlayOrDrawAll.Checked = false;
            mnuDrawToMatch.Checked = true;
            mnuDrawAllAndPlay.Checked = false;
            mnuDrawAndPlay.Checked = true;
            mnuDoubleDraw.Checked = false;
            mnuDrawBeforePlaying.Checked = true;
            mnuDrawTwoBeforePlaying.Checked = true;
            mnuSevenZero.Checked = true;
            mnuSkipPlayers.Checked = false;
            mnuSkipTimes.Checked = true;
            mnuOneWinner.Checked = true;
            mnuOneLoser.Checked = false;
            mnuUno.Checked = false;
        }

        private void MnuFlip_CheckedChanged(object sender, EventArgs e)
        {
            bool b = ((ToolStripMenuItem)sender).Checked;
            mnuCyan.Visible = b;
            mnuMagenta.Visible = b;
        }

        private void MnuGoWild_Click(object sender, EventArgs e)
        {
            txtDealt.Text = "35";
            txtDecks.Text = "3";
            mnuBlank.Checked = true;
            mnuMagentaBlank.Checked = true;
            mnuBlackBlank.Checked = true;
            mnuWater.Checked = true;
            mnuAttack.Checked = true;
            mnuWildHitfire.Checked = false;
            mnuTradeHands.Checked = false;
            mnuDos.Checked = true;
            mnuCyan.Checked = true;
            mnuMagenta.Checked = false;
            mnuFlip.Checked = true;
            mnuPairs.Checked = true;
            mnuStacking.Checked = true;
            mnuPlayOrDrawAll.Checked = false;
            mnuDrawToMatch.Checked = true;
            mnuDrawAllAndPlay.Checked = true;
            mnuDrawAndPlay.Checked = true;
            mnuDoubleDraw.Checked = false;
            mnuDrawBeforePlaying.Checked = true;
            mnuDrawTwoBeforePlaying.Checked = false;
            mnuSevenZero.Checked = false;
            mnuSkipPlayers.Checked = false;
            mnuSkipTimes.Checked = true;
            mnuOneWinner.Checked = true;
            mnuOneLoser.Checked = false;
            mnuUno.Checked = false;
        }

        private void MnuHardcore_Click(object sender, EventArgs e)
        {
            txtDealt.Text = "56";
            txtDecks.Text = "20";
            mnuBlank.Checked = true;
            mnuMagentaBlank.Checked = true;
            mnuBlackBlank.Checked = true;
            mnuWater.Checked = true;
            mnuAttack.Checked = true;
            mnuWildHitfire.Checked = true;
            mnuTradeHands.Checked = true;
            mnuDos.Checked = true;
            mnuFlip.Checked = true;
            mnuCyan.Checked = true;
            mnuMagenta.Checked = false;
            mnuPairs.Checked = true;
            mnuStacking.Checked = true;
            mnuPlayOrDrawAll.Checked = false;
            mnuDrawToMatch.Checked = true;
            mnuDrawAllAndPlay.Checked = false;
            mnuDrawAndPlay.Checked = true;
            mnuDoubleDraw.Checked = true;
            mnuDrawBeforePlaying.Checked = true;
            mnuDrawTwoBeforePlaying.Checked = true;
            mnuSevenZero.Checked = true;
            mnuSkipPlayers.Checked = false;
            mnuSkipTimes.Checked = true;
            mnuOneWinner.Checked = true;
            mnuOneLoser.Checked = false;
            mnuUno.Checked = false;
        }

        private void MnuImportGame_Click(object sender, EventArgs e)
        {
            string s = Interaction.InputBox("导入存档", "读取存档", Clipboard.GetText());
            if (s == "")
                return;
            try
            {
                LoadGame(s);
            }
            catch
            {
                MessageBox.Show("无效的游戏记彔!", "读取游戏", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            mnuLoad.Enabled = false;
            txtDealt.Text = "1";
            MnuStart_Click(mnuStart, new EventArgs());
        }

        private void MnuImportRules_Click(object sender, EventArgs e)
        {
            string s = Interaction.InputBox("导入玩法", "快速游戏", Clipboard.GetText());
            if (s != "")
                try
                {
                    LoadGame(s);
                }
                catch
                {
                    MessageBox.Show("无效的游戏玩法!", "读取玩法", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
        }

        private void MnuLoadAutosavedGame_Click(object sender, EventArgs e)
        {
            mnuLoad.Enabled = false;
            LoadGame(Interaction.GetSetting("UNO", "GAME", "AUTO"));
            MnuStart_Click(mnuStart, new EventArgs());
        }

        private void MnuLoadGame_Click(object sender, EventArgs e)
        {
            mnuLoad.Enabled = false;
            LoadGame(Interaction.GetSetting("UNO", "GAME", "SAVE"));
            MnuStart_Click(mnuStart, new EventArgs());
        }

        private void MnuLoadRules_Click(object sender, EventArgs e)
        {
            LoadGame(Interaction.GetSetting("UNO", "RULES", "SAVE"));
        }

        private void MnuNew_Click(object sender, EventArgs e)
        {
            MnuStandard_Click(mnuStandard, new EventArgs());
            MnuStart_Click(mnuStart, new EventArgs());
        }

        private void MnuPairs_CheckedChanged(object sender, EventArgs e)
        {
            mnuCheater.Visible = mnuPairs.Checked;
            if (mnuCheater.Checked && !mnuCheater.Visible)
            {
                mnuCheater.Checked = false;
                mnuPro.Checked = true;
            }
        }

        private void MnuPlayRecord_Click(object sender, EventArgs e)
        {
            mnuLoad.Enabled = false;
            isPlayingRecord = true;
            string s = Interaction.GetSetting("UNO", "RECORD", "GAME", "");
            if (s == "")
                LoadGame(Interaction.GetSetting("UNO", "RECORD", "RULES"));
            else
                LoadGame(s);
            mnuBeginner.Checked = true;
            mnuCheater.Checked = false;
            mnuRevealable.Checked = true;
            mnuCheat.Checked = false;
            MnuStart_Click(mnuStart, new EventArgs());
        }

        private void MnuQuit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MnuRandom_Click(object sender, EventArgs e)
        {
            mnuRandom.Checked = false;
            mnuGuid.Checked = false;
            mnuRNGCryptoServiceProvider.Checked = false;
            mnuMembership.Checked = false;
            ((ToolStripMenuItem)sender).Checked = true;
        }

        private void MnuRemoveBot_Click(object sender, EventArgs e)
        {
            ons--;
            ShowPlayers();
        }

        private void MnuSaveRules_Click(object sender, EventArgs e)
        {
            Interaction.SaveSetting("UNO", "RULES", "SAVE", "KKKKKKKKK" + SaveRules());
            mnuLoadRules.Text = DateAndTime.Now.ToString();
            mnuLoadRules.Enabled = true;
        }

        private void MnuSkip_Click(object sender, EventArgs e)
        {
            mnuSkipPlayers.Checked = false;
            mnuSkipTimes.Checked = false;
            ((ToolStripMenuItem)sender).Checked = true;
        }

        private void MnuStandard_Click(object sender, EventArgs e)
        {
            txtDealt.Text = "7";
            txtDecks.Text = "1";
            mnuBlank.Checked = false;
            mnuMagentaBlank.Checked = false;
            mnuBlackBlank.Checked = false;
            mnuWater.Checked = false;
            mnuAttack.Checked = false;
            mnuWildHitfire.Checked = false;
            mnuTradeHands.Checked = false;
            mnuDos.Checked = false;
            mnuFlip.Checked = false;
            mnuCyan.Checked = false;
            mnuMagenta.Checked = false;
            mnuPairs.Checked = false;
            mnuStacking.Checked = false;
            mnuPlayOrDrawAll.Checked = false;
            mnuDrawToMatch.Checked = false;
            mnuDrawAllAndPlay.Checked = false;
            mnuDrawAndPlay.Checked = true;
            mnuDoubleDraw.Checked = false;
            mnuDrawBeforePlaying.Checked = false;
            mnuDrawTwoBeforePlaying.Checked = false;
            mnuSevenZero.Checked = false;
            mnuSkipPlayers.Checked = true;
            mnuSkipTimes.Checked = false;
            mnuOneWinner.Checked = true;
            mnuOneLoser.Checked = false;
            mnuUno.Checked = true;
        }

        private void MnuStart_Click(object sender, EventArgs e)
        {
            if (!isPlaying)
            {
                isPlaying = true;
                MnuCustom_Click(mnuCustom, new EventArgs());
                mnuStart.Text = "继续";
                mnuOns.Enabled = false;
                mnuDifficulty.Enabled = false;
                mnuCheat.Enabled = false;
                mnuAnimation.Enabled = false;
                mnuBack.Enabled = false;
                new Uno(this).Show();
            }
            Hide();
        }

        private void MnuWinnerLoser_Click(object sender, EventArgs e)
        {
            mnuOneWinner.Checked = false;
            mnuOneLoser.Checked = false;
            ((ToolStripMenuItem)sender).Checked = true;
        }

        public string SaveRules()
        {
            string s = "";
            s += on0 + "K";
            s += ons + "K";
            s += txtPlayer0.Text + "K";
            s += txtPlayer1.Text + "K";
            s += txtPlayer2.Text + "K";
            s += txtPlayer3.Text + "K";
            s += txtDealt.Text + "K";
            s += txtDecks.Text + "K";
            s += mnuBeginner.Checked + "K";
            s += mnuPro.Checked + "K";
            s += mnuCheater.Checked + "K";
            s += mnuBotBeginner.Checked + "K";
            s += mnuBotPro.Checked + "K";
            s += mnuBlank.Checked + "K";
            s += mnuMagentaBlank.Checked + "K";
            s += mnuBlackBlank.Checked + "K";
            s += txtBlankText.Text + "K";
            s += txtBlankSkip.Text + "K";
            s += mnuBlankReverse.Checked + "K";
            s += txtBlankDraw.Text + "K";
            s += txtBlankDownpourDraw.Text + "K";
            s += mnuWater.Checked + "K";
            s += mnuAttack.Checked + "K";
            s += mnuWildHitfire.Checked + "K";
            s += mnuTradeHands.Checked + "K";
            s += mnuDos.Checked + "K";
            s += mnuFlip.Checked + "K";
            s += mnuCyan.Checked + "K";
            s += mnuMagenta.Checked + "K";
            s += mnuRevealable.Checked + "K";
            s += mnuPairs.Checked + "K";
            s += mnuStacking.Checked + "K";
            s += mnuPlayOrDrawAll.Checked + "K";
            s += mnuDrawToMatch.Checked + "K";
            s += mnuDrawAllAndPlay.Checked + "K";
            s += mnuDrawAndPlay.Checked + "K";
            s += mnuDoubleDraw.Checked + "K";
            s += mnuDrawBeforePlaying.Checked + "K";
            s += mnuDrawTwoBeforePlaying.Checked + "K";
            s += mnuSevenZero.Checked + "K";
            s += mnuSkipPlayers.Checked + "K";
            s += mnuSkipTimes.Checked + "K";
            s += mnuOneWinner.Checked + "K";
            s += mnuOneLoser.Checked + "K";
            s += mnuUno.Checked + "K";
            s += mnuMultiply0.Checked + "K";
            s += mnuMultiply1.Checked + "K";
            s += mnuMultiply10000.Checked + "K";
            s += mnuMultiply10.Checked + "K";
            s += mnuMultiply100.Checked + "K";
            s += mnuMultiply1000.Checked + "K";
            s += mnuCheat.Checked + "K";
            s += DateAndTime.Now.ToString();
            return s;
        }

        void ShowPlayers()
        {
            txtPlayer1.Visible = ons >= 1;
            txtPlayer2.Visible = ons >= 2;
            txtPlayer3.Visible = ons >= 3;
            mnuAddBot.Visible = ons < 3;
            mnuRemoveBot.Visible = ons > 0;
        }

        private void MnuDailyMoney_Click(object sender, EventArgs e)
        {
            string s = Interaction.GetSetting("UNO", "ACCOUNT", "LAST", "");
            if (s == "")
            {
                MessageBox.Show("歡迎新人, 請收下新手禮品.", "新手禮品", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Interaction.SaveSetting("UNO", "ACCOUNT", "MONEY", "50000000");
                money = 50000000;
                mnuMe.Text = "$" + money;
                MessageBox.Show("+$5000,0000", "新手禮品", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Interaction.SaveSetting("UNO", "ACCOUNT", "LAST", DateTime.Today.Ticks + "");
            }
            else
            {
                if (new TimeSpan(DateTime.Today.Ticks - long.Parse(s)).TotalDays >= 1)
                {
                    Interaction.SaveSetting("UNO", "ACCOUNT", "LAST", DateTime.Today.Ticks + "");
                    Interaction.SaveSetting("UNO", "ACCOUNT", "MONEY", money + 10000000 + "");
                    money += 10000000;
                    mnuMe.Text = "$" + money;
                    MessageBox.Show("+$1000,0000", "簽到", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("請明日再來", "簽到", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = ((ToolStripMenuItem)sender);
            menuItem.Checked = !menuItem.Checked;
        }
    }
}
