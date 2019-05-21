using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Atlatntida_o_Bot.Libs;
using WeifenLuo.WinFormsUI.Docking;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Linq;
using TreeViewS;

namespace Atlatntida_o_Bot.Forms
{
    public partial class SettingsForm : DockContent
    {
        
        public SettingsForm()
        {
            InitializeComponent();
            LoadSettings();
        }

        public void Initialize()
        {
            SetPlaceHolder(loginTextBox, "Логин");
            SetPlaceHolder(passwordTextBox, "Пароль");
        }

        public void LoadSettings()
        {
            LoadMain();
            LoadBattle();
            Initialize();
        }

        public void SaveSettings()
        {
            SaveMain();
            SaveBattle();
        }

        public void LoadMain()
        {
            settingsControl.SelectedIndex = BotBase.DefaultSettings.Int("SettingsIndex");
            if (BotBase.DefaultSettings.String("login") != "Логин"){loginTextBox.Text = BotBase.DefaultSettings.String("login");}
            if (BotBase.DefaultSettings.String("password") != "Пароль"){passwordTextBox.Text = BotBase.DefaultSettings.String("password");}
            action1.Checked = BotBase.DefaultSettings.Bool("action1");
            action2.Checked = BotBase.DefaultSettings.Bool("action2");
            action2_1.Checked = BotBase.DefaultSettings.Bool("action2_1");
            action3.Checked = BotBase.DefaultSettings.Bool("action3");
            action2_D_1.Checked = BotBase.DefaultSettings.Bool("action2_D_1");
            action2_D_2.Checked = BotBase.DefaultSettings.Bool("action2_D_2");
            ResorceList.SelectedIndex = BotBase.DefaultSettings.Int("sbor");
            wait_seconds.Value = BotBase.DefaultSettings.Int("sbor_wait_seconds");
            XmlHandler s = new XmlHandler();
            s.XmlToTreeView("locations.xml", treeView1);
            checkBox1.Checked = BotBase.DefaultSettings.Bool("БегАктивен");
            foreach (string keyName in BotBase.FileSettings.GetKeyNames("ЛокацииБег"))
            {
                listBox1.Items.Add(BotBase.FileSettings.GetString("ЛокацииБег", keyName, ""));
            }
            sp_1.Checked = BotBase.DefaultSettings.Bool("sp_1");
            sp_2.Checked = BotBase.DefaultSettings.Bool("sp_2");
            sp_3.Checked = BotBase.DefaultSettings.Bool("sp_3");
            sp_4.Checked = BotBase.DefaultSettings.Bool("sp_4");
            sp_5.Checked = BotBase.DefaultSettings.Bool("sp_5");
            sp_6.Checked = BotBase.DefaultSettings.Bool("sp_6");
            sp_7.Checked = BotBase.DefaultSettings.Bool("sp_7");
            sp_1_man.Value = BotBase.DefaultSettings.Int("sp_1_man");
            sp_2_man.Value = BotBase.DefaultSettings.Int("sp_2_man");
            sp_3_man.Value = BotBase.DefaultSettings.Int("sp_3_man");
            sp_4_man.Value = BotBase.DefaultSettings.Int("sp_4_man");
            sp_5_man.Value = BotBase.DefaultSettings.Int("sp_5_man");
            sp_6_man.Value = BotBase.DefaultSettings.Int("sp_6_man");
            sp_7_man.Value = BotBase.DefaultSettings.Int("sp_7_man");
        }

        public void SaveMain()
        {
            BotBase.DefaultSettings.SaveSetting("SettingsIndex", settingsControl.SelectedIndex);
            BotBase.DefaultSettings.SaveSetting("login",loginTextBox.Text);
            BotBase.DefaultSettings.SaveSetting("password", passwordTextBox.Text);
            BotBase.DefaultSettings.SaveSetting("action1" , action1.Checked);
            BotBase.DefaultSettings.SaveSetting("action2", action2.Checked);
            BotBase.DefaultSettings.SaveSetting("action2_1", action2_1.Checked);
            BotBase.DefaultSettings.SaveSetting("action3", action3.Checked);
            BotBase.DefaultSettings.SaveSetting("action2_D_1", action2_D_1.Checked);
            BotBase.DefaultSettings.SaveSetting("action2_D_2", action2_D_2.Checked);
            BotBase.DefaultSettings.SaveSetting("sbor", ResorceList.SelectedIndex);
            BotBase.DefaultSettings.SaveSetting("sbor_wait_seconds", (int)wait_seconds.Value);
            BotBase.DefaultSettings.SaveSetting("БегАктивен", checkBox1.Checked);
            XmlHandler s = new XmlHandler();
            s.TreeViewToXml(treeView1, "locations.xml");

            BotBase.FileSettings.DeleteSection("ЛокацииБег");
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                BotBase.FileSettings.WriteValue("ЛокацииБег", i.ToString(), listBox1.Items[i].ToString());
            }
            BotBase.DefaultSettings.SaveSetting("sp_1", sp_1.Checked);
            BotBase.DefaultSettings.SaveSetting("sp_2", sp_2.Checked);
            BotBase.DefaultSettings.SaveSetting("sp_3", sp_3.Checked);
            BotBase.DefaultSettings.SaveSetting("sp_4", sp_4.Checked);
            BotBase.DefaultSettings.SaveSetting("sp_5", sp_5.Checked);
            BotBase.DefaultSettings.SaveSetting("sp_6", sp_6.Checked);
            BotBase.DefaultSettings.SaveSetting("sp_7", sp_7.Checked);
            BotBase.DefaultSettings.SaveSetting("sp_1_man", (int)sp_1_man.Value);
            BotBase.DefaultSettings.SaveSetting("sp_2_man", (int)sp_2_man.Value);
            BotBase.DefaultSettings.SaveSetting("sp_3_man", (int)sp_3_man.Value);
            BotBase.DefaultSettings.SaveSetting("sp_4_man", (int)sp_4_man.Value);
            BotBase.DefaultSettings.SaveSetting("sp_5_man", (int)sp_5_man.Value);
            BotBase.DefaultSettings.SaveSetting("sp_6_man", (int)sp_6_man.Value);
            BotBase.DefaultSettings.SaveSetting("sp_7_man", (int)sp_7_man.Value);
        }

        public void LoadBattle()
        {
            foreach (string keyName in BotBase.MobsIniFile.GetKeyNames("Мобы"))
            {
                string level = BotBase.MobsIniFile.GetString("Мобы", keyName, "");
                bool check = Convert.ToBoolean(BotBase.MobsIniFile.GetString("Нападение", keyName, "false"));
                MobsListBox.Items.Add(keyName + "[" + level + "]", check);
            }

            wait_full_hp.Checked = BotBase.DefaultSettings.Bool("wait_full_hp");
            usloviePitya1.Checked = BotBase.DefaultSettings.Bool("usloviePitya1");
            try
            {
                usloviePitya1_1.Value = BotBase.DefaultSettings.Int("usloviePitya1_1");
            }
            catch { usloviePitya1_1.Value = 30; }
            usloviePitya3.Checked = BotBase.DefaultSettings.Bool("usloviePitya3");
            try
            {
                usloviePitya3_1.Value = BotBase.DefaultSettings.Int("usloviePitya3_1");
            }
            catch {usloviePitya3_1.Value = 1;}

            usloviePitya2.Checked = BotBase.DefaultSettings.Bool("usloviePitya2");
            for (int i = 1; i <= 9; i++)
            {
                Control button = Controls.Find("eliksir_" + i, true)[0];
                if(BotBase.DefaultSettings.String("eliksir_" + i) != "")
                {
                    button.Text = BotBase.DefaultSettings.String("eliksir_" + i);
                }
            }
            if (BotBase.DefaultSettings.Int("Rupor") == 1) { Rupor1.Checked = true; }
            if (BotBase.DefaultSettings.Int("Rupor") == 2) { Rupor2.Checked = true; }
            if (BotBase.DefaultSettings.Int("Rupor") == 3) { Rupor3.Checked = true; }
            Rupor.Checked = BotBase.DefaultSettings.Bool("RuporSettings");

            posledUradov.Text = BotBase.DefaultSettings.String("posledUradov");
            posledUdarov1.Checked = BotBase.DefaultSettings.Bool("posledUdarov1");
            posledUdarov2.Checked = BotBase.DefaultSettings.Bool("posledUdarov2");
            udar1.Checked = BotBase.DefaultSettings.Bool("udar1");
            udar2.Checked = BotBase.DefaultSettings.Bool("udar2");
            udar3.Checked = BotBase.DefaultSettings.Bool("udar3");
            usl_elik_pb_1.Checked = BotBase.DefaultSettings.Bool("usl_elik_pb_1");
            usl_elik_pb_2.Checked = BotBase.DefaultSettings.Bool("usl_elik_pb_2");
            usl_elik_pb_3.Checked = BotBase.DefaultSettings.Bool("usl_elik_pb_3");
        }

        public void SaveBattle()
        {
            for (int i = 0; i < MobsListBox.Items.Count; i++)
            {
                bool check = MobsListBox.GetItemChecked(i);
                Regex regex_name = new Regex(@"(.*?)\[.*?\]");
                string name = regex_name.Match(MobsListBox.Items[i].ToString()).Groups[1].Value;
                BotBase.MobsIniFile.WriteValue("Нападение", name, check.ToString());
            }
            BotBase.DefaultSettings.SaveSetting("wait_full_hp", wait_full_hp.Checked);
            BotBase.DefaultSettings.SaveSetting("usloviePitya1", usloviePitya1.Checked);
            BotBase.DefaultSettings.SaveSetting("usloviePitya1_1", (int)usloviePitya1_1.Value);
            BotBase.DefaultSettings.SaveSetting("usloviePitya2", usloviePitya2.Checked);
            BotBase.DefaultSettings.SaveSetting("usloviePitya3", usloviePitya3.Checked);
            BotBase.DefaultSettings.SaveSetting("usloviePitya3_1", (int)usloviePitya3_1.Value);
            for (int i = 1; i <= 9; i++)
            {
                Control button = Controls.Find("eliksir_" + i, true)[0];
                BotBase.DefaultSettings.SaveSetting("eliksir_" + i, button.Text);
            }
            if (Rupor1.Checked) { BotBase.DefaultSettings.SaveSetting("Rupor", 1); }
            if (Rupor2.Checked) { BotBase.DefaultSettings.SaveSetting("Rupor", 2); }
            if (Rupor3.Checked) { BotBase.DefaultSettings.SaveSetting("Rupor", 3); }
            BotBase.DefaultSettings.SaveSetting("RuporSettings", Rupor.Checked);

            BotBase.DefaultSettings.SaveSetting("posledUradov", posledUradov.Text);
            BotBase.DefaultSettings.SaveSetting("posledUdarov1", posledUdarov1.Checked);
            BotBase.DefaultSettings.SaveSetting("posledUdarov2", posledUdarov2.Checked);
            BotBase.DefaultSettings.SaveSetting("udar1", udar1.Checked);
            BotBase.DefaultSettings.SaveSetting("udar2", udar2.Checked);
            BotBase.DefaultSettings.SaveSetting("udar3", udar3.Checked);
            BotBase.DefaultSettings.SaveSetting("usl_elik_pb_1", usl_elik_pb_1.Checked);
            BotBase.DefaultSettings.SaveSetting("usl_elik_pb_2", usl_elik_pb_2.Checked);
            BotBase.DefaultSettings.SaveSetting("usl_elik_pb_3", usl_elik_pb_3.Checked);
        }

        public void SetPlaceHolder(Control control, string PlaceHolderText)
        {
            if (control.Text.Length == 0)
            {
                control.Text = PlaceHolderText;
                control.GotFocus += (sender, args) => 
                {
                    if (control.Text == PlaceHolderText)
                    {
                        control.Text = "";
                    }
                };
                control.LostFocus += (sender, args) => 
                {
                    if (control.Text.Length == 0)
                    {
                        control.Text = PlaceHolderText;
                    }
                };
            }
        }

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveSettings();
            Close();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            groupBox5.Enabled = action2.Checked;
            groupBox14.Enabled = action2.Checked;
        }

        private Button tempButton = null;

        private void пустоToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tempButton.Text = "П";
        }

        private void эликсирЖизниЖToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tempButton.Text = "Ж";
        }

        private void эликсирГигантаГToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tempButton.Text = "Г";
        }

        private void eliksir_1_Click(object sender, EventArgs e)
        {
            tempButton = eliksir_1;
            changeEliksir.Show(MousePosition);
        }

        private void eliksir_2_Click(object sender, EventArgs e)
        {
            tempButton = eliksir_2;
            changeEliksir.Show(MousePosition);
        }

        private void eliksir_3_Click(object sender, EventArgs e)
        {
            tempButton = eliksir_3;
            changeEliksir.Show(MousePosition);
        }

        private void eliksir_4_Click(object sender, EventArgs e)
        {
            tempButton = eliksir_4;
            changeEliksir.Show(MousePosition);
        }

        private void eliksir_5_Click(object sender, EventArgs e)
        {
            tempButton = eliksir_5;
            changeEliksir.Show(MousePosition);
        }

        private void eliksir_6_Click(object sender, EventArgs e)
        {
            tempButton = eliksir_6;
            changeEliksir.Show(MousePosition);
        }

        private void eliksir_7_Click(object sender, EventArgs e)
        {
            tempButton = eliksir_7;
            changeEliksir.Show(MousePosition);
        }

        private void eliksir_8_Click(object sender, EventArgs e)
        {
            tempButton = eliksir_8;
            changeEliksir.Show(MousePosition);
        }

        private void eliksir_9_Click(object sender, EventArgs e)
        {
            tempButton = eliksir_9;
            changeEliksir.Show(MousePosition);
        }

        private void эликсирУсиленияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tempButton.Text = "У";
        }

        private void posledUdarov2_CheckedChanged(object sender, EventArgs e)
        {
            groupBox13.Enabled = posledUdarov2.Checked;
            Check();
            addposledUradov1.Enabled = udar1.Checked;
            addposledUradov2.Enabled = udar2.Checked;
            addposledUradov3.Enabled = udar3.Checked;
        }

        private void Check()
        {
            int cnt = 0;
            if (udar1.Checked) { cnt++; }
            if (udar2.Checked) { cnt++; }
            if (udar3.Checked) { cnt++; }
            if (cnt <= 1)
            {
                label3.Text = "Должно быть выбрано больше 1 используемого удара";
            }
            else
            {
                label3.Text = "";
            }
        }

        private void udar1_CheckedChanged(object sender, EventArgs e)
        {
            if (!udar1.Checked)
            {
                posledUradov.Text = Regex.Replace(posledUradov.Text, "С", "");
            }
            Check();
            addposledUradov1.Enabled = udar1.Checked;
        }

        private void udar2_CheckedChanged(object sender, EventArgs e)
        {
            if (!udar2.Checked)
            {
                posledUradov.Text = Regex.Replace(posledUradov.Text, "У", "");
            }
            Check();
            addposledUradov2.Enabled = udar2.Checked;
        }

        private void udar3_CheckedChanged(object sender, EventArgs e)
        {
            if (!udar3.Checked)
            {
                posledUradov.Text = Regex.Replace(posledUradov.Text, "З", "");
            }
            Check();
            addposledUradov3.Enabled = udar3.Checked;
        }

        private void addposledUradov1_Click(object sender, EventArgs e)
        {
            posledUradov.Text += "С";
        }

        private void addposledUradov2_Click(object sender, EventArgs e)
        {
            posledUradov.Text += "У";
        }

        private void addposledUradov3_Click(object sender, EventArgs e)
        {
            posledUradov.Text += "З";
        }

        private void posledUdarov1_CheckedChanged(object sender, EventArgs e)
        {
            addposledUradov1.Enabled = udar1.Checked;
            addposledUradov2.Enabled = udar2.Checked;
            addposledUradov3.Enabled = udar3.Checked;
        }

        private void action3_CheckedChanged(object sender, EventArgs e)
        {
            groupBox5.Enabled = action2.Checked;
            groupBox14.Enabled = action2.Checked;
        }

        private void action1_CheckedChanged(object sender, EventArgs e)
        {
            groupBox5.Enabled = action2.Checked;
            groupBox14.Enabled = action2.Checked;
        }

        private void редактироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MobsListBox.SelectedItem == null)
            {
                //Че за хуйня
                MessageBox.Show("Не выбран элемент редактирования.");
                return;
            }
            Regex r = new Regex(@"(.*?)\[(.*?)\]");
            Match m = r.Match(MobsListBox.SelectedItem.ToString());
            RedactMob rm = new RedactMob();
            rm.RedactMobSetting(m.Groups[1].Value, Convert.ToInt32(m.Groups[2].Value));
            rm.ShowDialog();
            string mob_name = rm.textBox1.Text;
            int mob_level = (int)rm.numericUpDown1.Value;
            //Удаляем предыдущий кей если несовпадает имя и создадим новое
            if (m.Groups[1].Value != mob_name)
            {
                BotBase.MobsIniFile.DeleteKey("Мобы", m.Groups[1].Value);
                BotBase.MobsIniFile.WriteValue("Мобы", mob_name, mob_level);
            }
            else
            {
                //Просто обновим значение
                BotBase.MobsIniFile.WriteValue("Мобы", mob_name, mob_level);
            }
            ReloadMobs();
        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MobsListBox.SelectedItem == null)
            {
                //Че за хуйня
                MessageBox.Show("Не выбран элемент удаления.");
                return;
            }
            var message = MessageBox.Show("Удалить", "Вы действительно хотите удалить " + MobsListBox.SelectedItem.ToString(), MessageBoxButtons.YesNo);
            if (message == System.Windows.Forms.DialogResult.Yes)
            {
                Regex r = new Regex(@"(.*?)\[");
                Match math = r.Match(MobsListBox.SelectedItem.ToString());
                BotBase.MobsIniFile.DeleteKey("Мобы",math.Groups[1].Value);
                ReloadMobs();
            }
        }

        private void ReloadMobs()
        {
            MobsListBox.Items.Clear();
            foreach (string keyName in BotBase.MobsIniFile.GetKeyNames("Мобы"))
            {
                string level = BotBase.MobsIniFile.GetString("Мобы", keyName, "");
                bool check = Convert.ToBoolean(BotBase.MobsIniFile.GetString("Нападение", keyName, "false"));
                MobsListBox.Items.Add(keyName + "[" + level + "]", check);
            }
        }

        private void добавитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddMob rm = new AddMob();
            rm.ShowDialog();
            string mob_name = rm.textBox1.Text;
            int mob_level = (int)rm.numericUpDown1.Value;
            if (mob_level != 0 && mob_name != "")
            {
                BotBase.MobsIniFile.WriteValue("Мобы", mob_name, mob_level);
                ReloadMobs();
            }
        }

        private void action1_Click(object sender, EventArgs e)
        {

        }

        private void action2_Click(object sender, EventArgs e)
        {
            
        }

        private void action2_CheckedChanged(object sender, EventArgs e)
        {
            if (action1.Checked && action3.Checked)
            {
                MessageBox.Show("Внимание: Бои на арене не работают с обычным режимом убийства монстров и сбором ресурса");
                action1.Checked = false;
                action3.Checked = false;
            }
            groupBox5.Enabled = action2.Checked;
            groupBox14.Enabled = action2.Checked;
        }

        private void action3_Click(object sender, EventArgs e)
        {
            //
        }

        private void action1_CheckedChanged_1(object sender, EventArgs e)
        {
            //
        }

        TreeNode CurrentNode;
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node != null)
            {
                CurrentNode = e.Node;
                добавитьВToolStripMenuItem.Text = "Добавить в " + CurrentNode.Text;
            }
            else
            {
                CurrentNode = null;
            }
        }

        private void добавитьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AddLocation add = new AddLocation();
            add.ShowDialog();
            string name = add.textBox1.Text;
            if (name != "" && name != null)
            {
                treeView1.Nodes.Add(name);
            }
        }

        private void добавитьВToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentNode == null)
            {
                MessageBox.Show("Для начала выберите локацию или создайте новую!");
                return;
            }
            AddLocation add = new AddLocation();
            add.ShowDialog();
            string name = add.textBox1.Text;
            if (name != "" && name != null)
            {
                if (CurrentNode != null)
                {
                    //Добавляем в эту ноду
                    CurrentNode.Nodes.Add(name);
                    CurrentNode.ExpandAll();
                }
            }
        }

        private void редактироватьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (CurrentNode == null)
            {
                MessageBox.Show("Для начала выберите локацию или создайте новую!");
                return;
            }
            RedactLocation redact = new RedactLocation();
            redact.ShowDialog();
            string new_name = redact.textBox1.Text;
            if (new_name != "" && new_name != null)
            {
                CurrentNode.Text = new_name;
            }
        }

        private void удалитьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (CurrentNode == null)
            {
                MessageBox.Show("Для начала выберите локацию или создайте новую!");
                return;
            }
            CurrentNode.Remove();
        }

        private void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Middle)
            {
                if (treeView1.GetNodeAt(e.X, e.Y) != null)
                {
                    CurrentNode = treeView1.GetNodeAt(e.Location);
                }
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node != null)
            {
                treeView1.SelectedNode = e.Node;
                CurrentNode = e.Node;
                добавитьВToolStripMenuItem.Text = "Добавить в " + CurrentNode.Text;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            treeView1.ExpandAll();
        }

        private void бегатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentNode == null)
            {
                MessageBox.Show("Для начала выберите локацию или создайте новую!");
                return;
            }
            listBox1.Items.Add(CurrentNode.Text);
        }

        private void удалитToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                listBox1.Items.RemoveAt(listBox1.SelectedIndex);
            }
        }
    }
}
