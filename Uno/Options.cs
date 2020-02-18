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
        public int animation = 20;
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
        }

        private void KeepSafeForCheating_CheckedChanged(object sender, EventArgs e)
        {
            if (!mnuBeginner.Checked && mnuCanShowCards.Checked)
            {
                MessageBox.Show("正規比賽中禁止明牌！", "正規比賽", MessageBoxButtons.OK, MessageBoxIcon.Error);
                mnuCanShowCards.Checked = false;
            }
        }

        private void LoadGame(string save)
        {
            string[] keys = save.Split(char.Parse("K"));
            if (save[0] != 'K')
                this.keys = keys;
            on0 = bool.Parse(keys[9]);
            txtPlayer0.Visible = on0;
            mnuAddRemovePlayer.Text = (on0 ? "移除" : "新增") + "玩家";
            ons = byte.Parse(keys[10]);
            ShowPlayers();
            txtPlayer0.Text = keys[11];
            txtPlayer1.Text = keys[12];
            txtPlayer2.Text = keys[13];
            txtPlayer3.Text = keys[14];
            txtDealt.Text = keys[15];
            txtDecks.Text = keys[16];
            mnuBeginner.Checked = bool.Parse(keys[17]);
            mnuPro.Checked = bool.Parse(keys[18]);
            mnuCheater.Checked = bool.Parse(keys[19]);
            mnuBlank.Checked = bool.Parse(keys[20]);
            mnuMagentaBlank.Checked = bool.Parse(keys[21]);
            mnuBlackBlank.Checked = bool.Parse(keys[22]);
            mnuWater.Checked = bool.Parse(keys[23]);
            mnuAttack.Checked = bool.Parse(keys[24]);
            mnuWildHitfire.Checked = bool.Parse(keys[25]);
            mnuDos.Checked = bool.Parse(keys[26]);
            mnuCanShowCards.Checked = bool.Parse(keys[27]);
            mnuPairs.Checked = bool.Parse(keys[28]);
            mnuStacking.Checked = bool.Parse(keys[29]);
            mnuPlayOrDrawAll.Checked = bool.Parse(keys[30]);
            mnuDrawToMatch.Checked = bool.Parse(keys[31]);
            mnuDrawAllAndPlay.Checked = bool.Parse(keys[32]);
            mnuDrawAndPlay.Checked = bool.Parse(keys[33]);
            mnuDoubleDraw.Checked = bool.Parse(keys[34]);
            mnuDrawBeforePlaying.Checked = bool.Parse(keys[35]);
            mnuDrawTwoBeforePlaying.Checked = bool.Parse(keys[36]);
            mnuSkipPlayers.Checked = bool.Parse(keys[37]);
            mnuSkipTimes.Checked = bool.Parse(keys[38]);
            mnuOneWinner.Checked = bool.Parse(keys[39]);
            mnuOneLoser.Checked = bool.Parse(keys[40]);
            mnuUno.Checked = bool.Parse(keys[41]);
            mnuCheat.Checked = bool.Parse(keys[42]);
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
            mnuDos.Checked = false;
            mnuPairs.Checked = true;
            mnuStacking.Checked = true;
            mnuPlayOrDrawAll.Checked = true;
            mnuDrawToMatch.Checked = true;
            mnuDrawAllAndPlay.Checked = true;
            mnuDrawAndPlay.Checked = true;
            mnuDoubleDraw.Checked = false;
            mnuDrawBeforePlaying.Checked = false;
            mnuDrawTwoBeforePlaying.Checked = false;
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
            txtDecks.Text = "5";
            mnuBlank.Checked = true;
            mnuMagentaBlank.Checked = true;
            mnuBlackBlank.Checked = true;
            mnuWater.Checked = true;
            mnuAttack.Checked = true;
            mnuWildHitfire.Checked = true;
            mnuDos.Checked = true;
            mnuPairs.Checked = true;
            mnuStacking.Checked = true;
            mnuPlayOrDrawAll.Checked = false;
            mnuDrawToMatch.Checked = true;
            mnuDrawAllAndPlay.Checked = false;
            mnuDrawAndPlay.Checked = true;
            mnuDoubleDraw.Checked = true;
            mnuDrawBeforePlaying.Checked = true;
            mnuDrawTwoBeforePlaying.Checked = true;
            mnuSkipPlayers.Checked = false;
            mnuSkipTimes.Checked = true;
            mnuOneWinner.Checked = true;
            mnuOneLoser.Checked = false;
            mnuUno.Checked = false;
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
            mnuDos.Checked = true;
            mnuPairs.Checked = true;
            mnuStacking.Checked = true;
            mnuPlayOrDrawAll.Checked = false;
            mnuDrawToMatch.Checked = true;
            mnuDrawAllAndPlay.Checked = true;
            mnuDrawAndPlay.Checked = true;
            mnuDoubleDraw.Checked = false;
            mnuDrawBeforePlaying.Checked = true;
            mnuDrawTwoBeforePlaying.Checked = false;
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
            mnuCanShowCards.Checked = true;
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
            mnuDos.Checked = false;
            mnuPairs.Checked = false;
            mnuStacking.Checked = false;
            mnuPlayOrDrawAll.Checked = false;
            mnuDrawToMatch.Checked = false;
            mnuDrawAllAndPlay.Checked = false;
            mnuDrawAndPlay.Checked = true;
            mnuDoubleDraw.Checked = false;
            mnuDrawBeforePlaying.Checked = false;
            mnuDrawTwoBeforePlaying.Checked = false;
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
            s += mnuBlank.Checked + "K";
            s += mnuMagentaBlank.Checked + "K";
            s += mnuBlackBlank.Checked + "K";
            s += mnuWater.Checked + "K";
            s += mnuAttack.Checked + "K";
            s += mnuWildHitfire.Checked + "K";
            s += mnuDos.Checked + "K";
            s += mnuCanShowCards.Checked + "K";
            s += mnuPairs.Checked + "K";
            s += mnuStacking.Checked + "K";
            s += mnuPlayOrDrawAll.Checked + "K";
            s += mnuDrawToMatch.Checked + "K";
            s += mnuDrawAllAndPlay.Checked + "K";
            s += mnuDrawAndPlay.Checked + "K";
            s += mnuDoubleDraw.Checked + "K";
            s += mnuDrawBeforePlaying.Checked + "K";
            s += mnuDrawTwoBeforePlaying.Checked + "K";
            s += mnuSkipPlayers.Checked + "K";
            s += mnuSkipTimes.Checked + "K";
            s += mnuOneWinner.Checked + "K";
            s += mnuOneLoser.Checked + "K";
            s += mnuUno.Checked + "K";
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

        public void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = ((ToolStripMenuItem)sender);
            menuItem.Checked = !menuItem.Checked;
        }
    }
}
