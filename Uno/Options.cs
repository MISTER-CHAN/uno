﻿using Microsoft.VisualBasic;
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

        public string[] keys = {};
        bool isPlaying = false;

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
            mnuBlank.Checked = bool.Parse(keys[10]);
            mnuMagentaBlank.Checked = bool.Parse(keys[11]);
            mnuBlackBlank.Checked = bool.Parse(keys[12]);
            mnuDownpourDraw.Checked = bool.Parse(keys[13]);
            mnuAttack.Checked = bool.Parse(keys[14]);
            mnuCanShowCards.Checked = bool.Parse(keys[15]);
            mnuPairs.Checked = bool.Parse(keys[16]);
            mnuPlayOrDrawAll.Checked = bool.Parse(keys[17]);
            mnuDrawTilCanPlay.Checked = bool.Parse(keys[18]);
            mnuDrawAllAndPlay.Checked = bool.Parse(keys[19]);
            mnuDrawAndPlay.Checked = bool.Parse(keys[20]);
            mnuChallenges.Checked = bool.Parse(keys[21]);
            mnuDoubleDraw.Checked = bool.Parse(keys[22]);
            mnuDrawBeforePlaying.Checked = bool.Parse(keys[23]);
            mnuSkipPlayers.Checked = bool.Parse(keys[24]);
            mnuSkipTimes.Checked = bool.Parse(keys[25]);
            mnuOneWinner.Checked = bool.Parse(keys[26]);
            mnuOneLoser.Checked = bool.Parse(keys[27]);
            mnuCheat.Checked = bool.Parse(keys[28]);
        }

        private void MnuAdvanced_Click(object sender, EventArgs e)
        {
            txtDealt.Text = "11";
            txtDecks.Text = "1";
            mnuBlank.Checked = true;
            mnuMagentaBlank.Checked = true;
            mnuBlackBlank.Checked = false;
            mnuDownpourDraw.Checked = false;
            mnuAttack.Checked = false;
            mnuPairs.Checked = true;
            mnuPlayOrDrawAll.Checked = true;
            mnuDrawTilCanPlay.Checked = true;
            mnuDrawAllAndPlay.Checked = true;
            mnuDrawAndPlay.Checked = true;
            mnuDoubleDraw.Checked = false;
            mnuDrawBeforePlaying.Checked = false;
            mnuChallenges.Checked = false;
            mnuJumpin.Checked = false;
            mnuSevenZero.Checked = false;
            mnuSkipPlayers.Checked = false;
            mnuSkipTimes.Checked = true;
            mnuOneWinner.Checked = false;
            mnuOneLoser.Checked = true;
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
            txtDealt.Text = "57";
            txtDecks.Text = "3";
            mnuBlank.Checked = true;
            mnuMagentaBlank.Checked = true;
            mnuBlackBlank.Checked = true;
            mnuDownpourDraw.Checked = true;
            mnuAttack.Checked = true;
            mnuPairs.Checked = true;
            mnuPlayOrDrawAll.Checked = false;
            mnuDrawTilCanPlay.Checked = true;
            mnuDrawAllAndPlay.Checked = false;
            mnuDrawAndPlay.Checked = true;
            mnuChallenges.Checked = false;
            mnuDoubleDraw.Checked = true;
            mnuDrawBeforePlaying.Checked = true;
            mnuJumpin.Checked = false;
            mnuSevenZero.Checked = false;
            mnuSkipPlayers.Checked = false;
            mnuSkipTimes.Checked = true;
            mnuOneWinner.Checked = true;
            mnuOneLoser.Checked = false;
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
            mnuBlank.Checked = false;
            mnuMagentaBlank.Checked = false;
            mnuBlackBlank.Checked = false;
            mnuDownpourDraw.Checked = false;
            mnuAttack.Checked = false;
            mnuPairs.Checked = false;
            mnuPlayOrDrawAll.Checked = false;
            mnuDrawTilCanPlay.Checked = false;
            mnuDrawAllAndPlay.Checked = false;
            mnuDrawAndPlay.Checked = true;
            mnuDoubleDraw.Checked = false;
            mnuDrawBeforePlaying.Checked = false;
            mnuChallenges.Checked = true;
            mnuJumpin.Checked = false;
            mnuSevenZero.Checked = false;
            mnuSkipPlayers.Checked = true;
            mnuSkipTimes.Checked = false;
            mnuOneWinner.Checked = true;
            mnuOneLoser.Checked = false;
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
            s += txtDecks.Text + "K"; // 9
            s += mnuBlank.Checked + "K"; // 10
            s += mnuMagentaBlank.Checked + "K"; // 11
            s += mnuBlackBlank.Checked + "K"; // 12
            s += mnuDownpourDraw.Checked + "K"; // 13
            s += mnuAttack.Checked + "K"; // 14
            s += mnuCanShowCards.Checked + "K"; // 15
            s += mnuPairs.Checked + "K"; // 16
            s += mnuPlayOrDrawAll.Checked + "K"; // 17
            s += mnuDrawTilCanPlay.Checked + "K"; // 18
            s += mnuDrawAllAndPlay.Checked + "K"; // 19
            s += mnuDrawAndPlay.Checked + "K"; // 20
            s += mnuChallenges.Checked + "K"; // 21
            s += mnuDoubleDraw.Checked + "K"; // 22
            s += mnuDrawBeforePlaying.Checked + "K"; // 23
            s += mnuSkipPlayers.Checked + "K"; // 24
            s += mnuSkipTimes.Checked + "K"; // 25
            s += mnuOneWinner.Checked + "K"; // 26
            s += mnuOneLoser.Checked + "K"; // 27
            s += mnuCheat.Checked; // 28
            return s;
        }

        private void ToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = ((ToolStripMenuItem)sender);
            foreach (ToolStripItem m in menuItem.DropDownItems)
                if (m.Tag + "" != "e")
                    m.Enabled = menuItem.Checked;
        }

        public void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = ((ToolStripMenuItem)sender);
            menuItem.Checked = !menuItem.Checked;
        }
	}
}