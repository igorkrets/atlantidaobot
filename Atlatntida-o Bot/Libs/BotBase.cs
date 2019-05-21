using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Atlatntida_o_Bot.Forms;
using Atlatntida_o_Bot.Libs.Other;
using Awesomium.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Atlatntida_o_Bot.Libs
{
    class BotBase
    {
        public static bool Started;

        public static Thread SessionThread;

        public static Thread SessionResourcer;

        public static Stopwatch SessionTimer;

        public static string HARD = Hard.GetHardSerial();

        public static xNet.CookieDictionary CcDictionary = new xNet.CookieDictionary();

        public static CookieContainer CcDictonaryNet = new CookieContainer();

        public static CookieCollection CcDictonaryCollection = new CookieCollection();

        public static string GameUrl = "http://atlantida-o.ru/";
        public static string GameDomain = "atlantida-o.ru";

        public static WebControl WebControl;

        public static DockPanel MainPanel;

        public static Log LogForm;

        public static MainForm MainForm;

        public static TimeSpan time1;

        public static bool BlockPopUp = false;

        public static bool InGame = false;

        public static IntPtr WebControlHandle;

        public static string FilePath = AppDomain.CurrentDomain.BaseDirectory + @"\Settings.ini"; // Путь файл настроек

        public static IniFile FileSettings = new IniFile(FilePath); // Файл настроек бота

        public static IniFile MobsIniFile = new IniFile("Mobs.ini");

        public static class DefaultSettings
        {
            public static string String(string name)
            {
                return FileSettings.GetString("DefaultSettings", name, "");
            }

            public static int Int(string name)
            {
                int value = 0;
                try
                {
                    value = FileSettings.GetInt32("DefaultSettings", name, 0);
                }
                catch
                {
                    return 0;
                }
                return value;
            }

            public static bool Bool(string name)
            {
                try
                {
                    return Convert.ToBoolean(FileSettings.GetString("DefaultSettings", name, "False"));
                }
                catch
                {
                    return false;
                }

            }

            public static double Double(string name)
            {
                return FileSettings.GetDouble("DefaultSettings", name, 0);
            }

            public static void SaveSetting(string name, string value)
            {
                FileSettings.WriteValue("DefaultSettings", name, value);
            }
            public static void SaveSetting(string name, int value)
            {
                FileSettings.WriteValue("DefaultSettings", name, value);
            }
            public static void SaveSetting(string name, bool value)
            {
                FileSettings.WriteValue("DefaultSettings", name, value.ToString());
            }
            public static void SaveSetting(string name, double value)
            {
                FileSettings.WriteValue("DefaultSettings", name, value);
            }
        }
    }
}
