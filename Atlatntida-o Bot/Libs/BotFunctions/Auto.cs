using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using BotSuite.Imaging;
using System.Xml;
using System.IO;
using System.Text;
using System.Reflection;
using System.Globalization;
using Awesomium.Core;

namespace Atlatntida_o_Bot.Libs.BotFunctions
{
    class Auto
    {
        public struct SettingsAutoBot
        {
            public List<string> NameBots;
            public int FalseAttackCount;
        }

        public struct Bot
        {
            public string Name;
            public string Level;
            public string Id;
        }

        public struct Abdiv
        {
            public Bitmap Image;
            public int Id;
            public bool Explore;
        }

        public struct Inventory
        {
            public string Id;
            public string Name;
        }

        public struct Food
        {
            public string Id;
        }

        public enum Action
        {
            Mobs = 1,
            Arena = 2
        }

        public static bool InFight = false;

        public static bool InArena = false;

        public static SettingsAutoBot SettingsBot = new SettingsAutoBot();

        public static void LoadSettings()
        {
            List<string> botsList = new List<string>();
            foreach (string bot_name in BotBase.MobsIniFile.GetKeyNames("Нападение"))
            {
                if (Convert.ToBoolean(BotBase.MobsIniFile.GetString("Нападение", bot_name, "false")))
                {
                    botsList.Add(bot_name);
                }
            }
            SettingsBot.NameBots = botsList;
            SettingsBot.FalseAttackCount = 0;
            arenaInfo.Enter = false;
        }

        public static void StartSession()
        {
            InFight = false;
            InArena = false;
            if (!BotBase.Started)
            {
                if (BotBase.DefaultSettings.Bool("action3"))
                {
                    //Если собираем ресурсы ставим в отдельном потоке =)
                    BotFunc.AddLog("Сессяи Сбор: Старт.");
                    BotBase.SessionResourcer = new Thread(Sbor) { IsBackground = true };
                    BotBase.SessionResourcer.Start();
                }
                BotBase.MainForm.StartButton.Text = "Стоп сессия";
                BotBase.Started = true;
                BotBase.SessionThread = new Thread(Auto.Session) { IsBackground = true };
                BotBase.SessionThread.Start();
                BotBase.SessionTimer = new Stopwatch();
                BotBase.SessionTimer.Start();
            }
            else
            {
                if (BotBase.SessionResourcer != null)
                {
                    BotFunc.AddLog("Сессяи Сбор: Стоп.");
                    BotBase.SessionResourcer.Abort();
                    BotBase.SessionResourcer = null;
                }
                BotBase.MainForm.StartButton.Text = "Старт сессия";
                BotBase.Started = false;
                BotBase.SessionThread.Abort();
                BotBase.SessionTimer.Stop();
                BotFunc.AddLog("Сессия : " + Convert.ToInt32(BotBase.SessionTimer.Elapsed.TotalMinutes) + " минут " + BotBase.SessionTimer.Elapsed.Seconds + " секунд.");
            }
        }

        public static string CheckErrorList = "";
        public static bool CheckErrors()
        {
            CheckErrorList = "";
            bool error = false;
            LoadSettings();
            if (SettingsBot.NameBots.Count == 0)
            {
                if (!BotBase.DefaultSettings.Bool("action2"))
                {
                    CheckErrorList += "Не указано на кого нападать в настройках" + Environment.NewLine;
                    error = true;
                }
            }
            if (!BotBase.InGame)
            {
                CheckErrorList += "Не авторизованы в игре" + Environment.NewLine;
                error = true;
            }
            if (error)
            {
                MessageBox.Show(CheckErrorList, "Ошибки запуска сессии", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return error;
        }

        public static void Session()
        {
            if (CheckErrors())
            {
                StartSession();
            }

            SetRupor();

            if (BotBase.DefaultSettings.Bool("action1"))
            {
                //Бить мобов
                AttackMob(Action.Mobs);
            }
            if (BotBase.DefaultSettings.Bool("action2"))
            {
                //Бить на полях битв
                if (BotBase.DefaultSettings.Bool("action2_1"))
                {
                    // На арене
                    Arena(Action.Arena);
                }
            }
        }

        public static void Sbor()
        {
            int errors = 0;
            while (true)
            {
                while (true)
                {
                    if (!InFight)
                    {
                        break;
                    }
                    Thread.Sleep(1000);
                }
                int wait_seconds = BotBase.DefaultSettings.Int("sbor_wait_seconds");
                Location LoctationX = GetLocation();
                int need_res = BotBase.DefaultSettings.Int("sbor");
                if (need_res > LoctationX.Resourses.Count + 1)
                {
                    BotFunc.AddLog("Нет доступных ресурсов на локации, жду " + wait_seconds + " секунд.");
                    Thread.Sleep(wait_seconds * 1000);
                }
                int i = 0;
                need_res--;
                foreach (Resource res in LoctationX.Resourses)
                {
                    if (i == need_res)
                    {
                        if (res.Count == 0)
                        {
                            BotFunc.AddLog("Ресурсы закончились, ждем завоза " + wait_seconds + " секунд.");
                            Thread.Sleep(wait_seconds * 1000);
                        }
                        //Необходимый ресурс
                        string getRes = Requests.GetResponseHtml("http://atlantida-o.ru/locclick.php?harvest=" + i + "&" + Time());
                        Regex r = new Regex("val=\"(.*?)\".*?msg>(.*?)<");
                        Match match = r.Match(getRes);
                        int seconds_wait = Convert.ToInt32(match.Groups[1].Value);
                        string message = match.Groups[2].Value;
                        BotFunc.AddLog(message + " : " + seconds_wait + " секунд ждать");
                        Thread.Sleep(seconds_wait * 1000);
                        break;
                    }
                    i++;
                }
                BotFunc.AddLog("Ожидаем возможность фарма 1 минуту.");
                Thread.Sleep(60000);
                if (errors > 0)
                {
                    BotFunc.AddLog("Сбор ресурсов завершен досрочно. Причина: Не палимся.");
                    break;
                }
            }
        }

        public static void SetRupor()
        {
            if (!BotBase.DefaultSettings.Bool("RuporSettings"))
            {
                BotFunc.JsEx("sendspecial('skill" + BotBase.DefaultSettings.Int("Rupor") + "');");
            }
        }

        public static int Stavka = 0;

        public struct ArenaInfo
        {
            public bool Enter;
            public int Minutes;
        }

        public static ArenaInfo arenaInfo = GetArenaInfo();

        public static void ShowBattle()
        {
            BotFunc.JsEx("showbattle();");
        }

        public static void Arena(Action action)
        {
            arenaInfo = GetArenaInfo();
            //Поставим ждать на арену
            if (!arenaInfo.Enter)
            {
                string jsresp = BotFunc.JsEx("sendpost2('fields.php', {bid: 1, tab: 1});");
                Thread.Sleep(1000);
                arenaInfo = GetArenaInfo();
                if (arenaInfo.Enter)
                {
                    Stavka = 0;
                    BotFunc.AddLog("Ждем начала");
                    WaitBattleStart(action);
                    BattleLogic(action);
                }
                else
                {
                    if (Stavka == 0)
                    {
                        Stavka++;
                        BotFunc.AddLog("Пробуем поставить еще раз");
                        Arena(action);
                    }
                    else
                    {
                        StartSession();
                    }
                }
            }
            else
            {
                Stavka = 0;
                BotFunc.AddLog("Ждем начала");
                WaitBattleStart(action);
                BattleLogic(action);
            }
        }



        public static ArenaInfo GetArenaInfo()
        {
            arenaInfo = new ArenaInfo();
            string resp = Requests.PostResponseHtml("http://atlantida-o.ru/fields.php");
            string respp = Regex.Unescape(resp);
            string resppp = BotFunc.RemoveAllNewLines(respp);
            Regex r = new Regex(@"Арена.*?timeleft.*?>(.*?)мин\..*?bid:1,tab:1");
            if (r.IsMatch(resppp))
            {
                arenaInfo.Minutes = Convert.ToInt32(r.Match(resppp).Groups[1].Value);
                arenaInfo.Enter = resppp.Contains("Отказаться");
                return arenaInfo;
            }
            else
            {
                arenaInfo.Enter = false;
                arenaInfo.Minutes = 0;
                return arenaInfo;
            }
        }

        public static Dictionary<int, string> Eliksiry = new Dictionary<int, string>();

        public static bool GigantAfterBattle = false;

        public static int UdarovAfterEleksir = 0;
        public static int UdarovAfterEleksirMochi = 0;

        public static void ReloadEliksiry()
        {
            UdarovAfterEleksir = 0;
            UdarovAfterEleksirMochi = 0;
            GigantAfterBattle = false;
            Eliksiry.Clear(); // Удалим
            //Обновим
            for (int i = 1; i <= 9; i++)
            {
                string value = BotBase.DefaultSettings.String("eliksir_" + i);
                if (value != "П")
                {
                    Eliksiry.Add(i, value);
                }
            }
        }

        public static void Gig(int alludars)
        {
            foreach (KeyValuePair<int, string> value in Eliksiry)
            {
                if (value.Value == "Г")
                {
                    combination += "Г" + "-";
                    ClickEleksir(value.Key);
                    Eliksiry.Remove(value.Key);
                    Thread.Sleep(1000);
                    break;
                }
            }
        }

        public static void Jizn(int alludars)
        {
            if (GetHp() < BotBase.DefaultSettings.Int("usloviePitya1_1"))
            {
                //Если наше HP, меньше чем условие питья
                foreach (KeyValuePair<int, string> value in Eliksiry)
                {
                    if (value.Value == "Ж")
                    {
                        combination += "Ж" + "-";
                        ClickEleksir(value.Key);
                        Eliksiry.Remove(value.Key);
                        UdarovAfterEleksir = 1;
                        Thread.Sleep(1000);
                        break;
                    }
                }
            }
        }

        public static void Usil(int alludars)
        {
            if (alludars % BotBase.DefaultSettings.Int("usloviePitya3_1") == 0)
            {
                //Если удар компанарен условию
                foreach (KeyValuePair<int, string> value in Eliksiry)
                {
                    if (value.Value == "У")
                    {
                        combination += "У" + "-";
                        ClickEleksir(value.Key);
                        Thread.Sleep(3000);
                        UdarovAfterEleksirMochi = 1;
                        break;
                    }
                }
            }
        }

        public static void PitEliksiry(int alludars)
        {
            if(!GigantAfterBattle && BotBase.DefaultSettings.Bool("usloviePitya2"))
            {
                if (BotBase.DefaultSettings.Bool("action2") && BotBase.DefaultSettings.Bool("usl_elik_pb_2"))
                {
                    //Если поля битв и по условию пропустить питье этого эликсира
                    if (InArena)
                    {
                        Gig(alludars);    
                    }
                    goto usl_elik_pb_2;
                }
                //Если не пили эликсир гиганта и по условию надо
                Gig(alludars);
            }
            usl_elik_pb_2:
            if (BotBase.DefaultSettings.Bool("usloviePitya1") && UdarovAfterEleksir == 0)
            {
                if (BotBase.DefaultSettings.Bool("action2") && BotBase.DefaultSettings.Bool("usl_elik_pb_1"))
                {
                    //Если поля битв и по условию пропустить питье этого эликсира
                    if (InArena)
                    {
                        Jizn(alludars);
                    }
                    goto usl_elik_pb_1;
                }
                //Если пить и ударов после последнего приема эликсира = 0
                Jizn(alludars);
            }
            usl_elik_pb_1:
            if (BotBase.DefaultSettings.Bool("usloviePitya3") && UdarovAfterEleksirMochi == 0)
            {
                if (BotBase.DefaultSettings.Bool("action2") && BotBase.DefaultSettings.Bool("usl_elik_pb_3"))
                {
                    //Если поля битв и по условию пропустить питье этого эликсира
                    if (InArena)
                    {
                        Usil(alludars);
                    }
                    return;
                }
                Usil(alludars);
            }
        }

        public static void ClickEleksir(int element)
        {
            int[] x = {0, +30, +17, +22, +33, +64, +34, -13, -29, -26 };
            int[] y = {0, -170, -122, -73, -26, +17, -235, -209, -155, -96 };
            BotFunc.Find find = BotFunc.ImageSearchInGameWindow(new Bitmap("Images/eleksiry.bmp"));
            if (find.Found)
            {
                Point p = new Point(find.Coordinates.X + x[element], find.Coordinates.Y + y[element]);
                BotFunc.LeftClick(p);
            }
        }

        public static int GetHp()
        {
            try
            {
                string getHp = BotFunc.JsEx("document.getElementById('hp').innerHTML"); // 161 / 161
                //BotBase.DefaultSettings.SaveSetting("usloviePitya1", usloviePitya1.Checked);
                //BotBase.DefaultSettings.SaveSetting("usloviePitya1_1", (int)usloviePitya1_1.Value);
                //BotBase.DefaultSettings.SaveSetting("usloviePitya2", usloviePitya2.Checked);
                string[] splitted = Regex.Split(getHp, " / ");
                return GetPercent(Convert.ToInt32(splitted[0]), Convert.ToInt32(splitted[1]));
            }
            catch
            {
                return 50;
            }
        }

        /// <summary>
        /// Велосипед получение процента
        /// </summary>
        /// <param name="b">Число 1</param>
        /// <param name="a">Число 2</param>
        /// <returns>% от 2 чисел</returns>
        public static Int32 GetPercent(Int32 b, Int32 a)
        {
            if (b == 0) return 0;

            return (Int32)(b / (a / 100M));
        }

        public static void OkBtnsOnFight()
        {
            if(InFight)
            {
                var find3 = BotFunc.ImageSearchInGameWindow(new Bitmap("Images/ok_btn1.bmp"));
                var find2 = BotFunc.ImageSearchInGameWindow(new Bitmap("Images/ok_btn2.bmp"));
                if (find3.Found || find2.Found)
                {
                    BotFunc.AddLog("Убираем неприятности");
                    if (find2.Found)
                    {
                        BotFunc.LeftClick(find2.Coordinates);
                    }
                    if (find3.Found)
                    {
                        BotFunc.LeftClick(find3.Coordinates);
                    }
                }
            }
        }

        public static void Sposobnosti()
        {
            if (!InFight)
            {
                return;
            }
            int user_mana = 0;
            try
            {
                user_mana = Convert.ToInt32(BotFunc.JsEx("document.getElementById('mana').innerHTML"));
            }
            catch
            {
                try
                {
                    string mana = BotFunc.JsEx("document.getElementById('mana').innerHTML");
                    Regex r = new Regex("(.*?) / (.*?)");
                    Match match = r.Match(mana);
                    user_mana = Convert.ToInt32(match.Groups[1].Value);
                }
                catch { }
                
            }
            //int user_mana_max = Convert.ToInt32(BotFunc.JsEx("usermaxmana"));
            BotFunc.AddLog(user_mana);
            bool sp_1 = BotBase.DefaultSettings.Bool("sp_1");
            bool sp_2 = BotBase.DefaultSettings.Bool("sp_2");
            bool sp_3 = BotBase.DefaultSettings.Bool("sp_3");
            bool sp_4 = BotBase.DefaultSettings.Bool("sp_4");
            bool sp_5 = BotBase.DefaultSettings.Bool("sp_5");
            bool sp_6 = BotBase.DefaultSettings.Bool("sp_6");
            bool sp_7 = BotBase.DefaultSettings.Bool("sp_7");
            var find3 = BotFunc.ImageSearchInGameWindow(new Bitmap("Images/sposobnost_check.bmp"));
            List<Point> actives = new List<Point>();
            if (find3.Found)
            {
                find3.Coordinates.X += 7 + 26;
                find3.Coordinates.Y += 8 + 26;
                for (int i = 0; i < 7; i++)
                {
                    int sp_index = i + 1;
                    int add = (i * 55);
                    if (BotBase.DefaultSettings.Bool("sp_" + sp_index) && BotBase.DefaultSettings.Int("sp_" + sp_index + "_man") < user_mana)
                    {
                        actives.Add(new Point(find3.Coordinates.X, find3.Coordinates.Y + add));
                        
                    }
                }
            }
            if (actives.Count > 1)
            {
                int rand = BotFunc.GetRandomInt(0, actives.Count);
                BotFunc.LeftClick(actives[rand]);
            }
            else if (actives.Count == 1)
            {
                BotFunc.LeftClick(actives[0]);
            }
        }

        public static void SearchLocation()
        {
            List<string> awail_locations = new List<string>(); // Локации уже в которых были
            List<string> locations = new List<string>(); // Локации
            foreach (string keyName in BotBase.FileSettings.GetKeyNames("ЛокацииБег"))
            {
                string location = BotBase.FileSettings.GetString("ЛокацииБег", keyName, "");
                locations.Add(location);
                BotFunc.AddLog("Локация адд " + location);
            }

            while (true)
            {
                //поиск монстра
                Location loc = GetLocation();
                bool finded_svyaz = false;
                Link finded_loc = new Link();
                foreach (string location in locations)
                {
                    foreach (Link link in loc.Links)
                    {
                        //BotFunc.AddLog(location + " : " + link.Name);
                        if (location == link.Name && location != loc.Name)
                        {
                            bool find = false;
                            foreach (string l in awail_locations)
                            {
                                if (l == link.Name)
                                {
                                    find = true;
                                }
                            }
                            if (!find)
                            {
                                finded_loc = link;
                                finded_svyaz = true;
                            }
                        }
                        if (finded_svyaz)
                        {
                            break;
                        }
                    }
                    if (finded_svyaz)
                    {
                        break;
                    }
                }

                BotFunc.AddLog("Буду следовать " + finded_loc.Name);

                if (!finded_svyaz)
                {
                    break;
                }

                if (loc.MoveTime > 0)
                {
                    BotFunc.AddLog("Ждем " + loc.MoveTime + " прежде чем перейдем в " + finded_loc.Name);
                    Thread.Sleep(loc.MoveTime * 1000);
                    loc = GetLocation();
                }

                if (loc.MoveTime == 0)
                {
                    string construct_time = Time().ToString();
                    int id = finded_loc.Id;
                    Requests.GetResponseHtml("http://atlantida-o.ru/locclick.php?goto=" + id + "&" + construct_time);
                    Thread.Sleep(500);
                    loc = GetLocation();
                    if (loc.Id == finded_loc.Id)
                    {
                        BotFunc.AddLog("Успешный переход по локации");
                        //Не нашли записываем что уже тут были
                        bool find = false;
                        foreach (string l in awail_locations)
                        {
                            BotFunc.AddLog("Были на локациях " + l);
                            if (l == loc.Name)
                            {
                                find = true;
                                break;
                            }
                        }
                        if (!find)
                        {
                            awail_locations.Add(loc.Name);
                        }
                    }
                    else
                    {
                        BotFunc.AddLog("Не перешли по локации. Остановка!");
                        StartSession();
                    }
                }

                //Переходим и короче ищем мобов
                List<Bot> Mobs = GetMobs();
                if (Mobs.Count == 0)
                {
                    //Не нашли записываем что уже тут были
                    bool find = false;
                    foreach (string l in awail_locations)
                    {
                        if (l == finded_loc.Name)
                        {
                            find = true;
                            break;
                        }
                    }
                    if (!find)
                    {
                        awail_locations.Add(finded_loc.Name);
                    }
                }
                else
                {
                    BotFunc.AddLog("Нашли монстров, останавливаем поиск.");
                    return;
                }

                if (awail_locations.Count == locations.Count)
                {
                    BotFunc.AddLog("Не нашли монстров вообще ни на одной локации. Останавливаем.");
                    StartSession();
                }

                Thread.Sleep(5); // Ждем 5 секунд прежде чем что-то делать
            }
        }

        public static void AttackMob(Action action)
        {
            List<Bot> mobs_all = GetMobs();
            WaitFullHP(BotBase.DefaultSettings.Bool("wait_full_hp"));
            bool attacked = false;
            if (mobs_all.Count == 0)
            {
                BotFunc.AddLog("Не найдены монстры на этой локации");
                if (BotBase.DefaultSettings.Bool("БегАктивен"))
                {
                    BotFunc.AddLog("Но у нас активен бег по локациям. Жду разрешения перехода.");
                    SearchLocation();
                    mobs_all = GetMobs();
                }
                else
                {
                    BotFunc.AddLog("Не знаю что делать закрываюсь.");
                    StartSession();
                }
            }
            foreach (Bot mob in mobs_all)
            {
                if (mob.Name == SettingsBot.NameBots[BotFunc.GetRandomInt(SettingsBot.NameBots.Count)])
                {
                    BotFunc.AddLog("Нападаю на " + mob.Name + "[" + mob.Level + "]");
                    string AttackMobResponse =
                        Requests.GetResponseHtml("http://atlantida-o.ru/attack.php?bot=" + mob.Id + "&0." +
                                                 BotFunc.GetRandomNumbers(17));
                    if (AttackMobResponse.Contains("ok"))
                    {
                        InFight = true;
                        SettingsBot.FalseAttackCount = 0;
                        BotFunc.AddLog("Напали");
                        attacked = true;
                        Thread.Sleep(500);
                        ShowBattle();
                    }
                    else if (AttackMobResponse.Contains("abdiv"))
                    {
                        BotFunc.AddLog("Нашли сундук");
                        AbdivDispose(mob.Id);
                        Thread.Sleep(1000);
                        AttackMob(action);
                    }
                    break;
                }
            }
            if (attacked)
            {
                BotFunc.AddLog("Начинаем ждать мобов");
                WaitBattleStart(Action.Mobs);
                BattleLogic(action);
            }
            else
            {
                SettingsBot.FalseAttackCount++;
                BotFunc.AddLog("Не напали, ждем");
                if (BotBase.DefaultSettings.Bool("БегАктивен"))
                {
                    BotFunc.AddLog("Но у нас активен бег по локациям. Жду разрешения перехода.");
                    SearchLocation();
                    mobs_all = GetMobs();
                }
                else
                {
                    if (SettingsBot.FalseAttackCount == 5)
                    {
                        MessageBox.Show(CheckErrorList, "Не напали на моба 5 раз подряд, что-то не так.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        StartSession();
                    }
                }
                Thread.Sleep(5000);
                AttackMob(action);
            }
        }

        public static void AbdivDispose(string id)
        {
            foreach (Abdiv abdiv in DispatcherAbdiv(id))
            {
                if (abdiv.Explore)
                {
                    BotFunc.AddLog("Сканирование: " + abdiv.Id + " нормальный сундук"); 
                    BotFunc.JsEx("sendspecial('chest"+abdiv.Id+"');");
                }
            }    
        }

        public static List<Abdiv> DispatcherAbdiv(string id)
        {
            List<Abdiv> newAbdivsList = new List<Abdiv>();
            foreach (Abdiv abdiv in SearchAbdiv(id))
            {
                Abdiv newAbdiv = new Abdiv();
                newAbdiv.Image = abdiv.Image;
                newAbdiv.Id = abdiv.Id;
                newAbdiv.Explore = true;
                Rectangle rect = Template.Image(new ImageData(abdiv.Image), new ImageData("Images/minus.bmp"));
                if (rect.Location.X > 0)
                {
                    BotFunc.AddLog("Сканирование:" + abdiv.Id + " плохой сундук #1");
                    newAbdiv.Explore = false;
                }
                rect = Template.Image(new ImageData(abdiv.Image), new ImageData("Images/nol.bmp"));
                if (rect.Location.X > 0)
                {
                    BotFunc.AddLog("Сканирование:" + abdiv.Id + " плохой сундук #2");
                    newAbdiv.Explore = false;
                }
                newAbdivsList.Add(newAbdiv);
            }
            return newAbdivsList;
        }

        public static List<Abdiv> SearchAbdiv(string id)
        {
            List<Abdiv> tempAbdivsList = new List<Abdiv>();
            string AttackMobResponse =
                        Requests.GetResponseHtml("http://atlantida-o.ru/attack.php?bot=" + id + "&0." +
                                                 BotFunc.GetRandomNumbers(16));
            string PatternAbdiv = @"url\((.*?)\)";
            Regex SearchAbdiv = new Regex(PatternAbdiv);
            foreach (Match match in SearchAbdiv.Matches(AttackMobResponse))
            {
                Abdiv abdiv = new Abdiv();
                string url = match.Groups[1].Value;
                Bitmap abdivBitmap = BotFunc.LoadPicture(BotBase.GameUrl + url);
                Regex NumAbdiv = new Regex(@"n=(\d)");
                int num = Convert.ToInt32(NumAbdiv.Match(url).Groups[1].Value);
                abdiv.Id = num;
                abdiv.Image = abdivBitmap;
                abdiv.Explore = true;
                tempAbdivsList.Add(abdiv);
            }
            return tempAbdivsList;
        }

        public static List<Bot> GetMobs()
        {
            List<Bot> tempBotList = new List<Bot>();
            string mobPattern = "<bot.*?id>(.*?)<.*?name>(.*?)<.*?level>(.*?)<"; // 2 - имя, 3 - уровень, 1 - Id
            Regex MobSearchRegex = new Regex(mobPattern);
            string GetMobsResponse = Requests.GetResponseHtml("http://atlantida-o.ru/locclick.php?&" + BotFunc.GetRandomNumbers(13),true);
            foreach (Match match in MobSearchRegex.Matches(GetMobsResponse))
            {
                //BotFunc.AddLog(match.Groups[1].Value + "[" + match.Groups[2].Value + "]" + " " + match.Groups[3].Value);
                Bot bot = new Bot();
                bot.Id = match.Groups[1].Value;
                bot.Name = match.Groups[2].Value;
                bot.Level = match.Groups[3].Value;
                tempBotList.Add(bot);
            }
            return tempBotList;
        }

        public static void WaitBattleStart(Action action)
        {
            if (action == Action.Mobs)
            {
                InArena = false;
                while (true)
                {
                    var find = BotFunc.ImageSearchInGameWindow(new Bitmap("Images/udar1.bmp"));
                    var find2 = BotFunc.ImageSearchInGameWindow(new Bitmap("Images/udar11.bmp"));
                    if (find.Found || find2.Found)
                    {
                        return;
                    }
                    Thread.Sleep(1000);
                }
            }
            if (action == Action.Arena)
            {
                while (true)
                {
                    if (BotBase.DefaultSettings.Bool("action2_D_1"))
                    {
                        //Еще и бои проводить...
                        ArenaInfo info = GetArenaInfo();
                        BotFunc.AddLog(info.Minutes + " минут до начала.");
                        if (info.Minutes > 2 && info.Enter)
                        {
                            InArena = false;
                            BotFunc.JsEx("closemain2();");
                            BotFunc.AddLog("Закрыть окно арены.");
                            WaitFullHP(BotBase.DefaultSettings.Bool("wait_full_hp"));
                            //Если минут больше 2 и вошли в арену, то бьемся с монстрами
                            AttackMob(action);
                            BattleLogic(action);
                        }
                        else
                        {
                            while (true)
                            {
                                InArena = true;
                                var find3 = BotFunc.ImageSearchInGameWindow(new Bitmap("Images/udar1.bmp"));
                                var find2 = BotFunc.ImageSearchInGameWindow(new Bitmap("Images/udar11.bmp"));
                                if (find3.Found || find2.Found)
                                {
                                    InFight = false;
                                    return;
                                }
                                Thread.Sleep(2000);
                            }
                        }
                    }
                    var find = BotFunc.ImageSearchInGameWindow(new Bitmap("Images/udar1.bmp"));
                    var find23 = BotFunc.ImageSearchInGameWindow(new Bitmap("Images/udar11.bmp"));
                    if (find.Found || find23.Found)
                    {
                        BotFunc.AddLog("Дождались битвы на арене.");
                        return;
                    }
                    Thread.Sleep(1000);
                }
            }
        }

        public static string combination = "";
        public static int alludars = 1;

        public static void BattleLogic(Action action)
        {
            ReloadEliksiry();
            combination = "";
            alludars = 1;
            while (true)
            {
                OkBtnsOnFight();
                if (BotBase.DefaultSettings.Bool("posledUdarov2"))
                {
                    BotFunc.AddLog("Удары свои");
                    foreach (int udar_nominal in ParcePosledUdarov())
                    {
                        Udars(action,udar_nominal);
                    }
                }
                if (BotBase.DefaultSettings.Bool("posledUdarov1"))
                {
                    BotFunc.AddLog("Удары случай");
                    int random = BotFunc.GetRandomInt(1, 4);
                    while (true)
                    {
                        random = BotFunc.GetRandomInt(1, 4);
                        if (BotBase.DefaultSettings.Bool("udar" + random))
                        {
                            break;
                        }
                    }
                    Udars(action, random);
                }
            }
        }

        public static void Udars(Action action, int action_udar)
        {
            PitEliksiry(alludars);
            while (true)
            {
                if (BattleEnd(action))
                {
                    BotFunc.AddLog("Комбо: " + combination);
                    WaitFullHP(BotBase.DefaultSettings.Bool("wait_full_hp"));
                    if (action == Action.Mobs)
                    {
                        AttackMob(action);
                    }
                    if (action == Action.Arena)
                    {
                        Arena(action);
                    }
                }
                OkBtnsOnFight();
                Sposobnosti();
                Point coordPoint = new Point();
                var find = BotFunc.ImageSearchInGameWindow(new Bitmap("Images/udar" + action_udar + ".bmp"));
                var find2 = BotFunc.ImageSearchInGameWindow(new Bitmap("Images/udar" + action_udar + action_udar + ".bmp"));
                if (find.Found){coordPoint = find.Coordinates;}
                if (find2.Found) { coordPoint = find2.Coordinates; }
                if (coordPoint.X > 0)
                {
                    combination += action_udar + "-";
                    int clicks = BotFunc.GetRandomInt(1, 3);
                    //BotFunc.AddLog("Clicks " + clicks);
                    for (int i = 1; i <= clicks; i++)
                    {
                        BotFunc.LeftClick(find.Coordinates);
                    }
                    alludars++;
                    if (UdarovAfterEleksir != 0)
                    {
                        UdarovAfterEleksir--;
                    }
                    if (UdarovAfterEleksirMochi != 0)
                    {
                        UdarovAfterEleksirMochi--;
                    }
                    Thread.Sleep(2000);
                    break;
                }
                Thread.Sleep(2000);
            }
            
        }

        public static List<int> ParcePosledUdarov()
        {
            string posled = BotBase.DefaultSettings.String("posledUradov");
            List<int> real_posled = new List<int>();
            foreach (char c in posled)
            {
                if (c == Convert.ToChar("С")) { real_posled.Add(1); }
                if (c == Convert.ToChar("У")) { real_posled.Add(2); }
                if (c == Convert.ToChar("З")) { real_posled.Add(3); }
            }
            return real_posled;
        }

        public static bool BattleEnd(Action action)
        {
            var find = BotFunc.ImageSearchInGameWindow(new Bitmap("Images/exit.bmp"));
            var find2 = BotFunc.ImageSearchInGameWindow(new Bitmap("Images/exit2.bmp"));
            if (find.Found || find2.Found)
            {
                if (action == Action.Arena)
                {
                    BotFunc.LeftClick(find.Coordinates);
                }
                InFight = false;
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void WaitFullHP(bool wait = false)
        {
            if (wait)
            {
                BotFunc.AddLog("Жду восстановления жизней");
                while (true)
                {
                    if (BotBase.DefaultSettings.Bool("action2_D_1"))
                    {
                        
                    }
                    if (GetHp() == 100)
                    {
                        return;
                    }
                    Thread.Sleep(2000);
                }
            }
            else
            {
                //Реализовано в самой игре :((
                //EatFood();
            }
        }


        /// <summary>
        /// Реализовано в самой игре
        /// </summary>
        public static void EatFood()
        {
            foreach (Food food in GetFood(new string[]{"Пирожок"}))
            {
                BotFunc.JsEx("useitem(" + food.Id + ");");
                break;
            }
        }

        public static List<Food> GetFood(string[] foods)
        {
            List<Inventory> tempListInventories = new List<Inventory>();
            List<Food> tempListFoods = new List<Food>();
            string getInventory =
                Requests.GetResponseHtml("http://atlantida-o.ru/inventory.php?section=1&0." +
                                         BotFunc.GetRandomNumbers(16));
            string toolPattern = @"settooltip\('invitem(.*?)_.*?',.*?'(.*?)'"; // 1 - id, 2 - имя
            Regex toolRegex = new Regex(toolPattern);
            foreach (Match match in toolRegex.Matches(getInventory))
            {
                Inventory inventory = new Inventory();
                inventory.Id = match.Groups[1].Value;
                inventory.Name = match.Groups[2].Value;
                tempListInventories.Add(inventory);
            }
            foreach (Inventory tempListInventory in tempListInventories)
            {
                for (int i = 0; i < foods.Length; i++)
                {
                    if (tempListInventory.Name.Contains(foods[i]))
                    {
                        Food food = new Food();
                        food.Id = tempListInventory.Id;
                        tempListFoods.Add(food);
                    }
                }    
            }
            return tempListFoods;
        }

        public struct Location
        {
            public int Id;
            public string Name;
            public int ResDelay;
            public int MoveTime;
            public List<Resource> Resourses;
            public List<Mob> Mobs;
            public List<Link> Links;
        }

        public struct Resource
        {
            public int Id;
            public int Count;
        }

        public struct Mob
        {
            public int Id;
            public int X;
            public int Y;
            public int MinX;
            public int MinY;
            public int MaxX;
            public int MaxY;
            public double SpeedX;
            public double SpeedY;
            public string Name;
            public int Level;
        }

        public struct Link
        {
            public int Id;
            public string Name;
            public string Direction;
        }

        public static Location GetLocation()
        {
            Location loc = new Location();
            string construct_time = Time().ToString();
            string getLoc = Requests.GetResponseHtml("http://atlantida-o.ru/locclick.php?&" + construct_time);
            BotFunc.AddLog(construct_time);
            if (getLoc != "")
            {
                XmlDocument xml = GetXML(getLoc);
                //xml.Descendants("location").First().Attribute("code").Value;
                string locid = xml["location"].Attributes["id"].Value; //Id локации
                string name = xml["location"].Attributes["name"].Value; //Имя локации
                string resdelay = xml["location"].Attributes["resdelay"].Value; //Секунд сбора ресурса
                string movetime = xml["location"].Attributes["movetime"].Value; //Секунд до перехода
                BotFunc.AddLog(locid + " : " + name + " : " + resdelay + " : " + movetime);

                try
                {
                    loc.Id = Convert.ToInt32(locid);
                    loc.Name = name;
                    loc.ResDelay = Convert.ToInt32(resdelay);
                    loc.MoveTime = Convert.ToInt32(movetime);
                }
                catch { }
                

                List<Resource> resy = new List<Resource>();
                XmlNodeList list = xml["location"].GetElementsByTagName("resource");
                if (list != null)
                {
                    foreach(XmlElement doc in list){
                        Resource res = new Resource();
                        try
                        {
                            res.Id = Convert.ToInt32(doc.GetElementsByTagName("id")[0].InnerText);
                            res.Count = Convert.ToInt32(doc.GetElementsByTagName("qty")[0].InnerText);
                        }
                        catch { }
                        BotFunc.AddLog(doc.GetElementsByTagName("id")[0].InnerText);
                        BotFunc.AddLog(doc.GetElementsByTagName("qty")[0].InnerText);
                        resy.Add(res);
                    }
                }

                loc.Resourses = resy;

                List<Mob> moby = new List<Mob>();
                list = xml["location"].GetElementsByTagName("bot");
                if (list != null)
                {
                    foreach (XmlElement doc in list)
                    {
                        Mob res = new Mob();
                        try
                        {
                            res.Id = Convert.ToInt32(doc.GetElementsByTagName("id")[0].InnerText);
                            res.Name = doc.GetElementsByTagName("name")[0].InnerText;
                            res.Level = Convert.ToInt32(doc.GetElementsByTagName("level")[0].InnerText);
                            res.X = Convert.ToInt32(doc.GetElementsByTagName("x")[0].InnerText);
                            res.Y = Convert.ToInt32(doc.GetElementsByTagName("y")[0].InnerText);
                            res.MinX = Convert.ToInt32(doc.GetElementsByTagName("minx")[0].InnerText);
                            res.MinY = Convert.ToInt32(doc.GetElementsByTagName("miny")[0].InnerText);
                            res.MaxX = Convert.ToInt32(doc.GetElementsByTagName("maxx")[0].InnerText);
                            res.MaxY = Convert.ToInt32(doc.GetElementsByTagName("maxy")[0].InnerText);
                            res.SpeedX = double.Parse(doc.GetElementsByTagName("speedx")[0].InnerText, CultureInfo.InvariantCulture);
                            res.SpeedY = double.Parse(doc.GetElementsByTagName("speedy")[0].InnerText, CultureInfo.InvariantCulture);
                        }catch{

                        }
                        BotFunc.AddLog(res.Name + "[" + res.Level + "]");
                        moby.Add(res);
                    }
                }

                loc.Mobs = moby;

                List<Link> links = new List<Link>();
                list = xml["location"].GetElementsByTagName("link");
                if (list != null)
                {
                    foreach (XmlElement doc in list)
                    {
                        Link res = new Link();
                        try
                        {
                            res.Id = Convert.ToInt32(doc.GetElementsByTagName("id")[0].InnerText);
                            res.Name = doc.GetElementsByTagName("name")[0].InnerText;
                            res.Direction = doc.GetElementsByTagName("direction")[0].InnerText;
                        }
                        catch { }
                        
                        BotFunc.AddLog(res.Id + "[" + res.Name + "]" + " " + res.Direction);
                        links.Add(res);
                    }
                }

                loc.Links = links;
                //return xml.Root.Elements().First(el => int.Parse(el.Attribute("name").Value) == name).Value;
            }
            return loc;
        }

        public static XmlDocument GetXML(string response)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(response);
            return doc;
        }

        public static readonly DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static int Time()
        {
            return (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static string[] array;

        public static void Raschet() 
        {
            TreeViewS.XmlHandler s = new TreeViewS.XmlHandler();
            TreeView w = new TreeView();
            s.XmlToTreeView("locations.xml", w);
            w = Search(w);
            s.TreeViewToXml(w, "temp.xml");
        }

        public static TreeView Search(TreeView w, string name = "")
        {
            for (int i = 0; i < w.Nodes.Count; i++)
            {
                
            }
            return w;
        }

        public static void Init()
        {
        }
    }
}

//while (true)
//{
//    BotFunc.AddLog("http://atlantida-o.ru/attack.php?bot=15&0." + BotFunc.GetRandomNumbers(17));
//    ////Поиск одного моба
//    //ImageData screen = new ImageData(BotSuite.ScreenShot.Create());
//    //for (int i = 1; i <= 3; i++)
//    //{
//    //    ImageData image = new ImageData("osa3_" + i + ".bmp");
//    //    Rectangle coordRectangle = BotSuite.ImageLibrary.Template.Image(screen, image, (uint)10);
//    //    BotFunc.AddLog("i:" + i + "c:" + coordRectangle);
//    //    if (coordRectangle.Location.X > 0)
//    //    {
//    //        BotSuite.Mouse.Move(coordRectangle, false);
//    //        BotSuite.Mouse.LeftClick();
//    //        break;
//    //    }
//    //    Dictionary<string, ImageData> data = new Dictionary<string, ImageData>();
//    //    BotSuite.ImageLibrary.CommonFunctions.IdentifyImage(screen, data);

//    //}



//    //Rectangle test = BotSuite.ImageLibrary.Template.Image(new ImageData(BotSuite.ScreenShot.Create()),new ImageData("test.bmp"), (uint)30);
//    //AddLog(test);
//    //if (test.Location.X > 0)
//    //{
//    //    BotSuite.Mouse.Move(test, false);
//    //    BotSuite.Mouse.LeftClick();
//    //}
//    Thread.Sleep(500);
//    if (!BotBase.Started)
//    {
//        return;
//    }
//}