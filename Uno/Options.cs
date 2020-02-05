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

        private void LoadGame(string save)
        {
            string[] keys = save.Split(char.Parse("K"));
            if (save[0] != 'K')
                this.keys = keys;
            mnuPlayer0.Checked = bool.Parse(keys[9]);
            mnuPlayer1.Checked = bool.Parse(keys[10]);
            mnuPlayer2.Checked = bool.Parse(keys[11]);
            mnuPlayer3.Checked = bool.Parse(keys[12]);
            txtDealt.Text = keys[13];
            txtDecks.Text = keys[14];
            mnuBeginner.Checked = bool.Parse(keys[15]);
            mnuPro.Checked = bool.Parse(keys[16]);
            mnuHacker.Checked = bool.Parse(keys[17]);
            mnuRygbBlank.Checked = bool.Parse(keys[18]);
            mnuMagentaBlank.Checked = bool.Parse(keys[19]);
            mnuBlackBlank.Checked = bool.Parse(keys[20]);
            mnuWildDownpourDraw.Checked = bool.Parse(keys[21]);
            mnuDaWah.Checked = bool.Parse(keys[22]);
            mnuWildHitfire.Checked = bool.Parse(keys[23]);
            mnuDos.Checked = bool.Parse(keys[24]);
            mnuCanShowCards.Checked = bool.Parse(keys[25]);
            mnuPairs.Checked = bool.Parse(keys[26]);
            mnuStacking.Checked = bool.Parse(keys[27]);
            mnuPlayOrDrawAll.Checked = bool.Parse(keys[28]);
            mnuDrawToMatch.Checked = bool.Parse(keys[29]);
            mnuDrawAllAndPlay.Checked = bool.Parse(keys[30]);
            mnuDrawAndPlay.Checked = bool.Parse(keys[31]);
            mnuChallenges.Checked = bool.Parse(keys[32]);
            mnuDoubleDraw.Checked = bool.Parse(keys[33]);
            mnuDrawBeforePlaying.Checked = bool.Parse(keys[34]);
            mnuDrawTwoBeforePlaying.Checked = bool.Parse(keys[35]);
            mnuSkipPlayers.Checked = bool.Parse(keys[36]);
            mnuSkipTimes.Checked = bool.Parse(keys[37]);
            mnuOneWinner.Checked = bool.Parse(keys[38]);
            mnuOneLoser.Checked = bool.Parse(keys[39]);
            mnuUno.Checked = bool.Parse(keys[40]);
            mnuFair.Checked = bool.Parse(keys[41]);
            mnuCheat.Checked = bool.Parse(keys[42]);
        }

        private void MnuAdvanced_Click(object sender, EventArgs e)
        {
            txtDealt.Text = "11";
            txtDecks.Text = "1";
            mnuRygbBlank.Checked = true;
            mnuMagentaBlank.Checked = true;
            mnuBlackBlank.Checked = false;
            mnuWildDownpourDraw.Checked = false;
            mnuDaWah.Checked = false;
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
            mnuChallenges.Checked = false;
            mnuJumpin.Checked = false;
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
            mnuWildHitfire.Enabled = mnuDaWah.Checked;
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
            mnuMagentaBlank.Enabled = mnuRygbBlank.Checked;
            mnuBlankText.Enabled = mnuRygbBlank.Checked;
            mnuBlankSkip.Enabled = mnuRygbBlank.Checked;
            mnuBlankReverse.Enabled = mnuRygbBlank.Checked;
            mnuBlankDraw.Enabled = mnuRygbBlank.Checked;
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
            mnuHacker.Checked = false;
            ((ToolStripMenuItem)sender).Checked = true;
        }

        private void MnuDurable_Click(object sender, EventArgs e)
        {
            txtDealt.Text = "56";
            txtDecks.Text = "5";
            mnuRygbBlank.Checked = true;
            mnuMagentaBlank.Checked = true;
            mnuBlackBlank.Checked = true;
            mnuWildDownpourDraw.Checked = true;
            mnuDaWah.Checked = true;
            mnuWildHitfire.Checked = true;
            mnuDos.Checked = true;
            mnuPairs.Checked = true;
            mnuStacking.Checked = true;
            mnuPlayOrDrawAll.Checked = false;
            mnuDrawToMatch.Checked = true;
            mnuDrawAllAndPlay.Checked = false;
            mnuDrawAndPlay.Checked = true;
            mnuChallenges.Checked = false;
            mnuDoubleDraw.Checked = true;
            mnuDrawBeforePlaying.Checked = true;
            mnuDrawTwoBeforePlaying.Checked = true;
            mnuJumpin.Checked = false;
            mnuSevenZero.Checked = false;
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
            mnuRygbBlank.Checked = true;
            mnuMagentaBlank.Checked = true;
            mnuBlackBlank.Checked = true;
            mnuWildDownpourDraw.Checked = true;
            mnuDaWah.Checked = true;
            mnuWildHitfire.Checked = false;
            mnuDos.Checked = true;
            mnuPairs.Checked = true;
            mnuStacking.Checked = true;
            mnuPlayOrDrawAll.Checked = false;
            mnuDrawToMatch.Checked = true;
            mnuDrawAllAndPlay.Checked = true;
            mnuDrawAndPlay.Checked = true;
            mnuChallenges.Checked = false;
            mnuDoubleDraw.Checked = false;
            mnuDrawBeforePlaying.Checked = true;
            mnuDrawTwoBeforePlaying.Checked = false;
            mnuJumpin.Checked = false;
            mnuSevenZero.Checked = false;
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
            mnuPro.Checked = false;
            mnuHacker.Checked = false;
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
            mnuRygbBlank.Checked = false;
            mnuMagentaBlank.Checked = false;
            mnuBlackBlank.Checked = false;
            mnuWildDownpourDraw.Checked = false;
            mnuDaWah.Checked = false;
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
            mnuChallenges.Checked = true;
            mnuJumpin.Checked = false;
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

        private void MnuWater_CheckedChanged(object sender, EventArgs e)
        {
            mnuBlackBlank.Enabled = mnuWildDownpourDraw.Checked;
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
            s += mnuPlayer0.Checked + "K";
            s += mnuPlayer1.Checked + "K";
            s += mnuPlayer2.Checked + "K";
            s += mnuPlayer3.Checked + "K";
            s += txtDealt.Text + "K";
            s += txtDecks.Text + "K";
            s += mnuBeginner.Checked + "K";
            s += mnuPro.Checked + "K";
            s += mnuHacker.Checked + "K";
            s += mnuRygbBlank.Checked + "K";
            s += mnuMagentaBlank.Checked + "K";
            s += mnuBlackBlank.Checked + "K";
            s += mnuWildDownpourDraw.Checked + "K";
            s += mnuDaWah.Checked + "K";
            s += mnuWildHitfire.Checked + "K";
            s += mnuDos.Checked + "K";
            s += mnuCanShowCards.Checked + "K";
            s += mnuPairs.Checked + "K";
            s += mnuStacking.Checked + "K";
            s += mnuPlayOrDrawAll.Checked + "K";
            s += mnuDrawToMatch.Checked + "K";
            s += mnuDrawAllAndPlay.Checked + "K";
            s += mnuDrawAndPlay.Checked + "K";
            s += mnuChallenges.Checked + "K";
            s += mnuDoubleDraw.Checked + "K";
            s += mnuDrawBeforePlaying.Checked + "K";
            s += mnuDrawTwoBeforePlaying.Checked + "K";
            s += mnuSkipPlayers.Checked + "K";
            s += mnuSkipTimes.Checked + "K";
            s += mnuOneWinner.Checked + "K";
            s += mnuOneLoser.Checked + "K";
            s += mnuUno.Checked + "K";
            s += mnuFair.Checked + "K";
            s += mnuCheat.Checked + "K";
            s += DateAndTime.Now.ToString();
            return s;
        }

        public void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = ((ToolStripMenuItem)sender);
            menuItem.Checked = !menuItem.Checked;
        }
    }
}
