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
        public int animation = 20;
        public string[] keys = {};

		public Options() {
			InitializeComponent();
            mnuOptions.Font = new Font(mnuOptions.Font.FontFamily, mnuOptions.Font.Size * 2);
            Size = new Size(mnuQuit.Width * 8, mnuOptions.Height);
            mnuLoadGame.Text = Interaction.GetSetting("UNO", "GAME", "SAVE", "KKKKKKKKK(空)").Split(char.Parse("K"))[8];
            if (mnuLoadGame.Text == "(空)")
                mnuLoadGame.Enabled = false;
		}

        private void LoadGame(string save)
        {
            string[] keys = save.Split(char.Parse("K"));
            this.keys = keys;
            txtDecks.Text = keys[9];
            mnuRygbBlank.Checked = bool.Parse(keys[10]);
            mnuMagentaBlank.Checked = bool.Parse(keys[11]);
            mnuBlackBlank.Checked = bool.Parse(keys[12]);
            mnuWildDownpourDraw.Checked = bool.Parse(keys[13]);
            mnuDaWah.Checked = bool.Parse(keys[14]);
            mnuWildHitfire.Checked = bool.Parse(keys[15]);
            mnuDos.Checked = bool.Parse(keys[16]);
            mnuCanShowCards.Checked = bool.Parse(keys[17]);
            mnuPairs.Checked = bool.Parse(keys[18]);
            mnuStackDraw.Checked = bool.Parse(keys[19]);
            mnuPlayOrDrawAll.Checked = bool.Parse(keys[20]);
            mnuDrawTilCanPlay.Checked = bool.Parse(keys[21]);
            mnuDrawAllAndPlay.Checked = bool.Parse(keys[22]);
            mnuDrawAndPlay.Checked = bool.Parse(keys[23]);
            mnuChallenges.Checked = bool.Parse(keys[24]);
            mnuDoubleDraw.Checked = bool.Parse(keys[25]);
            mnuDrawBeforePlaying.Checked = bool.Parse(keys[26]);
            mnuDrawTwoBeforePlaying.Checked = bool.Parse(keys[27]);
            mnuSkipPlayers.Checked = bool.Parse(keys[28]);
            mnuSkipTimes.Checked = bool.Parse(keys[29]);
            mnuOneWinner.Checked = bool.Parse(keys[30]);
            mnuOneLoser.Checked = bool.Parse(keys[31]);
            mnuUno.Checked = bool.Parse(keys[32]);
            mnuCheat.Checked = bool.Parse(keys[33]);
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
            mnuDos.Checked = false;
            mnuPairs.Checked = true;
            mnuStackDraw.Checked = true;
            mnuPlayOrDrawAll.Checked = true;
            mnuDrawTilCanPlay.Checked = true;
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

        private void MnuDurable_Click(object sender, EventArgs e)
        {
            txtDealt.Text = "35";
            txtDecks.Text = "3";
            mnuRygbBlank.Checked = true;
            mnuMagentaBlank.Checked = true;
            mnuBlackBlank.Checked = true;
            mnuWildDownpourDraw.Checked = true;
            mnuDaWah.Checked = true;
            mnuDos.Checked = true;
            mnuPairs.Checked = true;
            mnuStackDraw.Checked = true;
            mnuPlayOrDrawAll.Checked = false;
            mnuDrawTilCanPlay.Checked = true;
            mnuDrawAllAndPlay.Checked = true;
            mnuDrawAndPlay.Checked = true;
            mnuChallenges.Checked = false;
            mnuDoubleDraw.Checked = false;
            mnuDrawBeforePlaying.Checked = false;
            mnuDrawTwoBeforePlaying.Checked = true;
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

        private void MnuLoadGame_Click(object sender, EventArgs e)
        {
            mnuLoad.Enabled = false;
            txtDealt.Text = "1";
            LoadGame(Interaction.GetSetting("UNO", "GAME", "SAVE"));
            MnuStart_Click(mnuStart, new EventArgs());
        }

        private void MnuLoadRules_Click(object sender, EventArgs e)
        {
            mnuQuick.HideDropDown();
            LoadGame(Interaction.GetSetting("UNO", "GAME", "RULES"));
        }

        private void MnuNew_Click(object sender, EventArgs e)
        {
            MnuStandard_Click(mnuStandard, new EventArgs());
            MnuStart_Click(mnuStart, new EventArgs());
        }

        private void MnuQuit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void MnuSaveRules_Click(object sender, EventArgs e)
        {
            Interaction.SaveSetting("UNO", "GAME", "RULES", "KKKKKKKKK" + SaveRules());
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
            mnuDos.Checked = false;
            mnuPairs.Checked = false;
            mnuStackDraw.Checked = false;
            mnuPlayOrDrawAll.Checked = false;
            mnuDrawTilCanPlay.Checked = false;
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
            s += txtDecks.Text + "K"; // 9
            s += mnuRygbBlank.Checked + "K"; // 10
            s += mnuMagentaBlank.Checked + "K"; // 11
            s += mnuBlackBlank.Checked + "K"; // 12
            s += mnuWildDownpourDraw.Checked + "K"; // 13
            s += mnuDaWah.Checked + "K"; // 14
            s += mnuWildHitfire.Checked + "K"; // 15
            s += mnuDos.Checked + "K"; // 16
            s += mnuCanShowCards.Checked + "K"; // 17
            s += mnuPairs.Checked + "K"; // 18
            s += mnuStackDraw.Checked + "K"; // 19
            s += mnuPlayOrDrawAll.Checked + "K"; // 20
            s += mnuDrawTilCanPlay.Checked + "K"; // 21
            s += mnuDrawAllAndPlay.Checked + "K"; // 22
            s += mnuDrawAndPlay.Checked + "K"; // 23
            s += mnuChallenges.Checked + "K"; // 24
            s += mnuDoubleDraw.Checked + "K"; // 25
            s += mnuDrawBeforePlaying.Checked + "K"; // 26
            s += mnuDrawTwoBeforePlaying.Checked + "K"; // 27
            s += mnuSkipPlayers.Checked + "K"; // 28
            s += mnuSkipTimes.Checked + "K"; // 29
            s += mnuOneWinner.Checked + "K"; // 30
            s += mnuOneLoser.Checked + "K"; // 31
            s += mnuUno.Checked; // 32
            s += mnuCheat.Checked; // 33
            return s;
        }

        public void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = ((ToolStripMenuItem)sender);
            menuItem.Checked = !menuItem.Checked;
        }
    }
}
