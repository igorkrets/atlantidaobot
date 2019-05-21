using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Atlatntida_o_Bot.Forms;
using Atlatntida_o_Bot.Libs;
using Atlatntida_o_Bot.Libs.BotFunctions;
using Atlatntida_o_Bot.Libs.Other;
using BotSuite.ImageLibrary;
using WeifenLuo.WinFormsUI.Docking;
using System.Diagnostics;
using Awesomium.Core;

namespace Atlatntida_o_Bot
{
    public partial class MainForm : Form
    {
        private Registration registration;
        private SettingsForm settingsForm;
        private WebDocument webDocument;
        private bool TestLicence = true;
        public MainForm()
        {
            //BotBase.DefaultSettings.SaveSetting("axLogin", "demonstration");
            //BotBase.DefaultSettings.SaveSetting("axPassword", "demonstration");
            InitializeComponent();
            BotBase.MainForm = this;
            BotBase.MainPanel = dockPanel1;
            webDocument = new WebDocument(true);
            webDocument.Show(BotBase.MainPanel);
            BotBase.LogForm = new Log();
            BotBase.LogForm.Show(BotBase.MainPanel);
            BotBase.LogForm.DockHandler.DockState = DockState.DockRightAutoHide;
            BotBase.WebControl = webDocument.webControl;
            BotBase.WebControl.Source = new Uri(BotBase.GameUrl);
            BotBase.WebControlHandle = BotBase.WebControl.Handle;
            //if (!Registration.Check())
            //{
            //    TestLicence = true;
            //    RegisterProgramm.Visible = true;
            //    TestModeTimerEl.Visible = true;
            //    TestModeTimer.Enabled = true;
            //    TestModeTimer.Start();
            //}

            Auto.Init();
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            Auto.StartSession();    
        }

        public void OpenTab(WebDocument webDocument)
        {
            webDocument.Show(BotBase.WebControl);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
            Application.ExitThread();
        }

        private void ShowHtml_Click(object sender, EventArgs e)
        {
            Forms.Debug.ShowHtml showHtml = new Forms.Debug.ShowHtml();
            showHtml.Show();
        }

        private void тестToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Auto.GetLocation();
            return;
            List<Auto.Food> food = Auto.GetFood(new string[] { "Пирожок" });
            foreach (Auto.Food foodx in food)
            {
                BotFunc.AddLog(foodx.Id);
            }
            return;
            BotFunc.Find find = BotFunc.ImageSearchInGameWindow(new Bitmap("Images/eleksiry.bmp"));
            if (find.Found)
            {
                BotSuite.Mouse.MoveRelativeToWindow(BotBase.WebControlHandle, find.Coordinates,false);
            }

            //Forms.Debug.ShowHtml showHtml = new Forms.Debug.ShowHtml("http://atlantida-o.ru/locclick.php?&" + BotFunc.GetRandomNumbers(13));
            //showHtml.Show();

            //BotFunc.JsEx("sendpost2('fields.php', {bid: 1, tab: 1});");
            
            //if (respp.Contains("Отказаться"))
            //{
            //    BotFunc.AddLog("Ждем начала");
            //}

            //MessageBox.Show(respp);
        }

        public static int GetMinutes()
        {
            string resp = Requests.PostResponseHtml("http://atlantida-o.ru/fields.php");
            string respp = System.Text.RegularExpressions.Regex.Unescape(resp);
            string resppp = BotFunc.RemoveAllNewLines(respp);
            Regex r = new Regex(@"Арена.*?timeleft.*?>(.*?)мин\.");
            if (r.IsMatch(resppp))
            {
                return Convert.ToInt32(r.Match(resppp).Groups[1].Value);
            }
            else
            {
                return 0;
            }
        }

        private void тест2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Auto.Sposobnosti();
            return;
            Auto.InFight = true;
            Auto.OkBtnsOnFight();
            Auto.InFight = false;
            return;
            Auto.Raschet();
            return;
            Auto.Location LoctationX = Auto.GetLocation();
            int need_res = BotBase.DefaultSettings.Int("sbor");
            if (need_res > LoctationX.Resourses.Count) {
                BotFunc.AddLog("Нет доступных ресурсов на локации");
            }
            int i = 0;
            need_res--;
            foreach (Auto.Resource res in LoctationX.Resourses)
            {
                if (i == need_res)
                {
                    //Необходимый ресурс
                    string getRes = Requests.GetResponseHtml("http://atlantida-o.ru/locclick.php?harvest=" + i + "&" + Auto.Time());
                    Regex r = new Regex("val=\"(.*?)\".*?msg>(.*?)<");
                    Match match = r.Match(getRes);
                    int seconds_wait = Convert.ToInt32(match.Groups[1].Value);
                    string message = match.Groups[2].Value;
                    BotFunc.AddLog(message + " : " + seconds_wait + " секунд ждать");
                    break;
                }
                i++;
            }
            return;
            string mobPattern = "<dirx>.*?<name>(.*?)<.*?<level>(.*?)<.*?id>(.*?)<"; // 1 - имя, 2 - уровень, 3 - Id
            Regex MobSearchRegex = new Regex(mobPattern);
            string GetMobsResponse = Requests.GetResponseHtml("http://atlantida-o.ru/locclick.php?&" + BotFunc.GetRandomNumbers(13));
            foreach (Match match in MobSearchRegex.Matches(GetMobsResponse))
            {
                BotFunc.AddLog(match.Groups[1].Value + "[" + match.Groups[2].Value + "]" + " " + match.Groups[3].Value);
            }
        }

        private void тест34ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string mobPattern = "<dirx>.*?<name>(.*?)<.*?<level>(.*?)<.*?id>(.*?)<"; // 1 - имя, 2 - уровень, 3 - Id
            Regex MobSearchRegex = new Regex(mobPattern);
            string GetMobsResponse = Requests.GetResponseHtml("http://atlantida-o.ru/locclick.php?&" + BotFunc.GetRandomNumbers(13));
            foreach (Match match in MobSearchRegex.Matches(GetMobsResponse))
            {
                BotFunc.AddLog(match.Groups[1].Value + "[" + match.Groups[2].Value + "]" + " " + match.Groups[3].Value);
                if (match.Groups[2].Value == "3" || match.Groups[2].Value == "2")
                {
                    BotFunc.AddLog("Нападаю");
                    string AttackMobResponse =
                        Requests.GetResponseHtml("http://atlantida-o.ru/attack.php?bot=" + match.Groups[3].Value + "&0." +
                                                 BotFunc.GetRandomNumbers(17));
                    if (AttackMobResponse.Contains("Ok"))
                    {
                        BotFunc.AddLog("Успешно напали");
                    }
                    return;
                }
            }
        }

        private void парсингБотовToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IniFile file = new IniFile("Mobs.ini");
            List<Auto.Bot> mobsList = Auto.GetMobs();
            foreach (Auto.Bot mob in mobsList)
            {
                bool found = false;
                foreach (string name in file.GetKeyNames("Мобы"))
                {
                    if (name == mob.Name)
                    {
                        found = true;
                    }
                }
                if (!found)
                {
                    BotFunc.AddLog("Добавили " + mob.Name + "[" + mob.Level + "]");
                    file.WriteValue("Мобы", mob.Name, mob.Level);
                }
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (settingsForm == null)
            {
                settingsForm = new SettingsForm();
                settingsForm.Show(BotBase.MainPanel);
                settingsForm.FormClosed += delegate(object o, FormClosedEventArgs args) { settingsForm = null; };
            }
        }

        private int _seconds = 1206;

        private void TestModeTimer_Tick(object sender, EventArgs e)
        {
            _seconds--;
            if (_seconds == 0)
            {
                Process.GetCurrentProcess().Kill();
            }
            var stringtime = "Пробный период: ";
            var span = TimeSpan.FromSeconds(_seconds);
            if (span.Hours != 0)
            {
                stringtime += span.Hours + "ч ";
            }
            if (span.Minutes != 0)
            {
                stringtime += span.Minutes + "м ";
            }
            if (span.Seconds != 0)
            {
                stringtime += span.Seconds + "c";
            }
            TestModeTimerEl.Text = stringtime;
        }

        private void RegisterProgramm_Click(object sender, EventArgs e)
        {
            if (registration == null)
            {
                registration = new Registration();
                registration.Show(BotBase.MainPanel);
                registration.FormClosed += delegate(object o, FormClosedEventArgs args) { registration = null; };
            }
            
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }

        private void dockPanel1_ActiveContentChanged(object sender, EventArgs e)
        {

        }
    }
}
