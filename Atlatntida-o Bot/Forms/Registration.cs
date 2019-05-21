using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Awesomium.Core;
using Atlatntida_o_Bot.Libs;
using WeifenLuo.WinFormsUI.Docking;

namespace Atlatntida_o_Bot.Forms
{
    public partial class Registration : DockContent
    {
        public static Registration RegistrationForm;

        public Registration()
        {
            InitializeComponent();
            RegistrationForm = this;
            textBox1.Text = BotBase.DefaultSettings.String("axLogin");
            textBox2.Text = BotBase.DefaultSettings.String("axPassword");

            pph = new Random().Next(10000000, 50000000);
            string resp = Requests.GetResponseHtml("http://avtor.lh1.in/user.php?pph=" + pph + "&hard=" + BotBase.HARD + "&login=" +
                                      BotBase.DefaultSettings.String("axLogin") + "&password=" +
                                       BotBase.DefaultSettings.String("axPassword") + "&bot=Atlantida", false);
            try
            {
                string[] info = resp.Split(".".ToCharArray());
                string date1 = DES.Decrypt3Des(info[0], "25252525");
                string date2 = DES.Decrypt3Des(info[1], "25252525");
                DateTime time1 = DateTime.Parse(date1);
                DateTime time2 = DateTime.Parse(date2);
                if (time2 > time1)
                {
                    //Есть
                    TimeSpan time3 = time2 - time1;
                    //label5.Text = "Дата окончания: " + time2.ToUniversalTime() + Environment.NewLine + time3.Days + " дня " + time3.Hours + " часа " + time3.Seconds + " секунд";
                    //label6.Text = "Текущая дата: " + time1.ToUniversalTime();
                    try
                    {
                        //label7.Text = "Дополнительная информация: " + Environment.NewLine + info[2];
                    }
                    catch { }
                    linkLabel2_LinkClicked(null, null);
                    LicenseAdd(true);
                }
                else
                {
                    LicenseAdd(false);
                }
            }
            catch (Exception ex)
            {
                LicenseAdd(false);
            }

            webControl1.Source = new Uri(String.Format("http://avtor.lh1.in/buy.php?submit=Войти&username={0}&password={1}&softkey={2}", textBox1.Text, textBox2.Text, DES.Encrypt3Des(BotBase.HARD, "25252525"))); // где my_post_url - адрес, на который отправляются post-данные.

            webControl1.DocumentReady += WebControl1OnDocumentReady;
            webControl1.LoadingFrameComplete += WebControl1OnLoadingFrameComplete;
        }

        private void WebControl1OnLoadingFrameComplete(object sender, FrameEventArgs frameEventArgs)
        {
            Checked();
        }

        private void WebControl1OnDocumentReady(object sender, UrlEventArgs urlEventArgs)
        {

        }

        private void Checked()
        {
            //textBox1.Text = BotBase.DefaultSettings.String("axLogin");
            //textBox2.Text = BotBase.DefaultSettings.String("axPassword");

            pph = new Random().Next(10000000, 50000000);
            string resp = Requests.GetResponseHtml("http://avtor.lh1.in/user.php?pph=" + pph + "&hard=" + BotBase.HARD + "&login=" +
                                      textBox1.Text + "&password=" +
                                       textBox2.Text + "&bot=Atlantida", false);
            try
            {
                string[] info = resp.Split(".".ToCharArray());
                string date1 = DES.Decrypt3Des(info[0], "25252525");
                string date2 = DES.Decrypt3Des(info[1], "25252525");
                DateTime time1 = DateTime.Parse(date1);
                DateTime time2 = DateTime.Parse(date2);
                if (time2 > time1)
                {
                    BotBase.DefaultSettings.SaveSetting("axLogin", textBox1.Text);
                    BotBase.DefaultSettings.SaveSetting("axPassword", textBox2.Text);
                    //Есть
                    TimeSpan time3 = time2 - time1;
                    //label5.Text = "Дата окончания: " + time2.ToUniversalTime() + Environment.NewLine + time3.Days + " дня " + time3.Hours + " часа " + time3.Seconds + " секунд";
                    //label6.Text = "Текущая дата: " + time1.ToUniversalTime();
                    try
                    {
                        //label7.Text = "Дополнительная информация: " + Environment.NewLine + info[2];
                    }
                    catch { }
                    BotBase.MainForm.TestModeTimer.Stop();
                    BotBase.MainForm.TestModeTimerEl.Visible = false;
                    BotBase.MainForm.RegisterProgramm.Visible = false;
                    LicenseAdd(true);
                }
                else
                {
                    LicenseAdd(false);
                }
            }
            catch (Exception ex)
            {
                LicenseAdd(false);
            }
        }

        private int pph = 0;

        public static void LicenseAdd(bool who)
        {
            if (who)
            {
                RegistrationForm.label5.ForeColor = Color.Green;
                RegistrationForm.label5.Text = "Лицензия есть";
            }
            else
            {
                RegistrationForm.label5.ForeColor = Color.Red;
                RegistrationForm.label5.Text = "Нет лицензии";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pph = new Random().Next(10000000, 50000000);
            string resp = Requests.GetResponseHtml("http://avtor.lh1.in/user.php?pph=" + pph + "&hard=" + BotBase.HARD + "&login=" +
                                      textBox1.Text + "&password=" +
                                      textBox2.Text + "&bot=Atlantida", false);
            try
            {
                string[] info = resp.Split(".".ToCharArray());
                string date1 = DES.Decrypt3Des(info[0], "25252525");
                string date2 = DES.Decrypt3Des(info[1], "25252525");
                DateTime time1 = DateTime.Parse(date1);
                DateTime time2 = DateTime.Parse(date2);
                if (time2 > time1)
                {
                    BotBase.DefaultSettings.SaveSetting("axLogin", textBox1.Text);
                    BotBase.DefaultSettings.SaveSetting("axPassword", textBox2.Text);
                    //Есть
                    TimeSpan time3 = time2 - time1;
                    //label5.Text = "Дата окончания: " + time2.ToUniversalTime() + Environment.NewLine + time3.Days + " дня " + time3.Hours + " часа " + time3.Seconds + " секунд";
                    //label6.Text = "Текущая дата: " + time1.ToUniversalTime();
                    try
                    {
                        //label7.Text = "Дополнительная информация: " + Environment.NewLine + info[2];
                    }
                    catch { }
                    //groupBox1.Enabled = false;
                    //linkLabel2_LinkClicked(null, null);
                    BotBase.MainForm.TestModeTimer.Stop();
                    BotBase.MainForm.TestModeTimerEl.Visible = false;
                    BotBase.MainForm.RegisterProgramm.Visible = false;
                    LicenseAdd(true);
                }
                else
                {
                    LicenseAdd(false);
                }
            }
            catch (Exception ex)
            {
                LicenseAdd(false);
            }

            webControl1.Source = new Uri(String.Format("http://avtor.lh1.in/buy.php?submit=Войти&username={0}&password={1}&softkey={2}", textBox1.Text, textBox2.Text, DES.Encrypt3Des(BotBase.HARD, "25252525"))); // где my_post_url - адрес, на который отправляются post-данные.
        }

        public static bool Check()
        {
            int pph = new Random().Next(10000000, 50000000);
            string resp = Requests.GetResponseHtml("http://avtor.lh1.in/user.php?pph=" + pph + "&hard=" + BotBase.HARD + "&login=" +
                                      BotBase.DefaultSettings.String("axLogin") + "&password=" +
                                       BotBase.DefaultSettings.String("axPassword") + "&bot=Atlantida", false);
            try
            {
                string[] info = resp.Split(".".ToCharArray());
                string date1 = DES.Decrypt3Des(info[0], "25252525");
                string date2 = DES.Decrypt3Des(info[1], "25252525");
                DateTime time1 = DateTime.Parse(date1);
                DateTime time2 = DateTime.Parse(date2);
                if (time2 > time1)
                {
                    TimeSpan time3 = time2 - time1;
                    BotBase.time1 = time3;
                    //Есть
                    return true;
                    LicenseAdd(true);
                }
                else
                {
                    return false;
                    LicenseAdd(false);
                }
            }
            catch (Exception ex)
            {
                return false;
                LicenseAdd(false);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://vk.com/id207164633");
        }

        private bool skirit = false;

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!skirit)
            {
                groupBox3.Size = new Size(73, 20);
                skirit = true;
                linkLabel2.Text = "Раскрыть";
            }
            else
            {
                groupBox3.Size = new Size(371, 135);
                skirit = false;
                linkLabel2.Text = "Скрыть";
            }
        }
    }
}
