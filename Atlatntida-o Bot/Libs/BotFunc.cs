using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Awesomium.Core;

namespace Atlatntida_o_Bot.Libs
{
    class BotFunc
    {
        public static class Native
        {
            [DllImport("user32.dll")]
            internal static extern bool ClientToScreen(IntPtr hWnd, out Rectangle lpPoint);
        }

        public struct Find
        {
            public bool Found;
            public Point Coordinates;
            public Size SearchBitmapSize;
            public Size SourceBitmapSize;
            public int TimeSearch;

            public override string ToString()
            {
                return "Found:" + Found + " Coords: " + Coordinates.X + "x" + Coordinates.Y + " FindTime:" + TimeSearch + "ms. Search&SourceBitmapSize: " + SearchBitmapSize + " & " + SourceBitmapSize;
            }
        }

        public static bool MouseOff = true;

        public static bool Human = true;

        public static void AddLog(object Object)
        {
            BotBase.LogForm.Invoke(new Action(() =>
            {
                BotBase.LogForm.TextLog.Text += Object + Environment.NewLine;
            }));
        }

        public static string JsEx(string js)
        {
            string text = "";
            try
            {
                BotBase.WebControl.Invoke(new MethodInvoker(delegate
                {
                    try
                    {
                        text = BotBase.WebControl.ExecuteJavascriptWithResult(js);
                    }
                    catch (Exception)
                    {
                        JsEx(js);
                    }
                }));
            }
            catch (Exception)
            {
                Application.DoEvents();
                JsEx(js);
            }
            return text;
        }

        private static Random rng;
        public static string GetRandomNumbers(int count)
        {
            Random rng = new Random();
            string ret = "";
            for (int i = 0; i < count; i++)
            {
                ret += GenerateDigit(rng);
            }
            return ret;
        }

        public static int GetRandomInt()
        {
            rng = new Random();
            return rng.Next(int.MaxValue);
        }

        public static int GetRandomInt(int max)
        {
            rng = new Random();
            return rng.Next(max);
        }

        public static int GetRandomInt(int min, int max)
        {
            rng = new Random();
            return rng.Next(min, max);
        }

        private static int GenerateDigit(Random rng)
        {
            // Предположим, что здесь много логики
            return rng.Next(10);
        }

        public static Bitmap GetBitmapGameWindow()
        {
            Bitmap p = new Bitmap(1, 1);
            //Метод 1 Нельзя покрывать окнами, зато не лагает
            //BotBase.WebControl.Invoke(new Action(() => { p = BotSuite.ScreenShot.CreateRelativeToControl(BotBase.WebControl, BotBase.WebControl.ClientRectangle); }));
            //Метод 2 Нестабилен, появляются черные квадраты, местами подлагивает
            //p = BotSuite.ScreenShot.CreateFromHidden(BotBase.WebControlHandle);
            //Метод 3 Убивает напрочь игровую механику
            //p = BotSuite.ScreenShot.CreateFromHidden2(BotBase.WebControlHandle);
            //Метод 4 С инвоком слишком нагружает процессор + лагает, но зато можно свернуть в трей
            p = new Bitmap(BotBase.WebControl.Size.Width, BotBase.WebControl.Size.Height);
            BotBase.WebControl.Invoke(new Action(() => { BotBase.WebControl.DrawToBitmap(p, BotBase.WebControl.ClientRectangle); }));
            return p;
        }

        public static void LeftClick(Point clickRectangle)
        {
            if (MouseOff)
            {
                BotBase.WebControl.Invoke(new MethodInvoker(delegate
                {
                    ((IWebView)BotBase.WebControl).InjectMouseMove(clickRectangle.X, clickRectangle.Y);
                    ((IWebView)BotBase.WebControl).InjectMouseDown(MouseButton.Left);
                    ((IWebView)BotBase.WebControl).InjectMouseUp(MouseButton.Left);
                }));
            }
            else
            {
                Rectangle rect;
                Native.ClientToScreen(BotBase.WebControlHandle, out rect);
                int randomX = new Random().Next(0, 3);
                int randomY = new Random().Next(0, 3);
                int randomXPlus = new Random().Next(0, 2);
                int randomYPlus = new Random().Next(0, 2);
                if (randomXPlus == 0){randomX = randomX * (-1);}
                if (randomYPlus == 0){randomY = randomX * (-1);}
                int x = rect.X + clickRectangle.X;
                int y = rect.Y + clickRectangle.Y;
                x += randomX;
                y += randomY;
                BotSuite.Mouse.Move(new Point(x, y), Human, 100);
                BotSuite.Mouse.LeftClick();
            }
        }

        public static Find ImageSearchInGameWindow(Bitmap bitmap, Rectangle foundRectangle = new Rectangle())
        {
            try
            {
                Find find = new Find();
                Stopwatch sp = new Stopwatch();
                sp.Start();
                const double tolerance = 0;
                //Переделать
                var bmpScreenshot = GetBitmapGameWindow();
                var smallBmp = new Bitmap(bitmap);
                var bigBmp = new Bitmap(bmpScreenshot);
                if (foundRectangle.X > 0 && foundRectangle.Y > 0) // Проверяем нужно ли обрезать?
                {
                    bigBmp = Crop(bmpScreenshot, foundRectangle);
                }
                var smallData =
                    smallBmp.LockBits(new Rectangle(0, 0, smallBmp.Width, smallBmp.Height),
                        ImageLockMode.ReadOnly,
                        PixelFormat.Format24bppRgb);
                var bigData =
                    bigBmp.LockBits(new Rectangle(0, 0, bigBmp.Width, bigBmp.Height),
                        ImageLockMode.ReadOnly,
                        PixelFormat.Format24bppRgb);

                int smallStride = smallData.Stride;
                int bigStride = bigData.Stride;

                int srednayatochkaSmallwidth = smallBmp.Width / 2;
                int srednayatochkaSmallheight = smallBmp.Height / 2;

                int bigWidth = bigBmp.Width;
                int bigHeight = bigBmp.Height - smallBmp.Height + 1;
                int smallWidth = smallBmp.Width * 3;
                int smallHeight = smallBmp.Height;
                int margin = Convert.ToInt32(255.0 * tolerance);

                unsafe
                {
                    byte* pSmall = (byte*)(void*)smallData.Scan0;
                    byte* pBig = (byte*)(void*)bigData.Scan0;

                    int bigOffset = bigStride - bigBmp.Width * 3;

                    bool matchFound = true;

                    for (int y = 0; y < bigHeight; y++)
                    {
                        for (int x = 0; x < bigWidth; x++)
                        {
                            byte* pBigBackup = pBig;
                            byte* pSmallBackup = pSmall;

                            //Look for the small picture.
                            for (int i = 0; i < smallHeight; i++)
                            {
                                int j;
                                matchFound = true;
                                for (j = 0; j < smallWidth; j++)
                                {
                                    //С прозрачностью: значение pSmall должен быть между полями.
                                    int inf = pBig[0] - margin;
                                    int sup = pBig[0] + margin;
                                    if (sup < pSmall[0] || inf > pSmall[0])
                                    {
                                        matchFound = false;
                                        break;
                                    }

                                    pBig++;
                                    pSmall++;
                                }

                                if (!matchFound) break;

                                //Восстанавливаем указатели
                                pSmall = pSmallBackup;
                                pBig = pBigBackup;

                                //Следующие ряды маленькой и большой картинки
                                pSmall += smallStride * (1 + i);
                                pBig += bigStride * (1 + i);
                            }

                            //Если нашли возвращаем
                            if (matchFound)
                            {
                                find.Found = true;
                                find.Coordinates = new Point(foundRectangle.X + x + srednayatochkaSmallwidth,
                                    foundRectangle.Y + y + srednayatochkaSmallheight);
                                find.SearchBitmapSize = smallBmp.Size;
                                find.SourceBitmapSize = bigBmp.Size;
                                break;
                            }
                            //Если не нашли возвращаем указатели и продолжаем поиск
                            pBig = pBigBackup;
                            pSmall = pSmallBackup;
                            pBig += 3;
                        }

                        if (matchFound) break;

                        pBig += bigOffset;
                    }
                }
                bmpScreenshot.Dispose();
                bigBmp.UnlockBits(bigData);
                smallBmp.UnlockBits(smallData);
                smallBmp.Dispose();
                bigBmp.Dispose();
                sp.Stop();
                find.TimeSearch = (int)sp.ElapsedMilliseconds;
                return find;
            }
            catch (Exception)
            {
                BotFunc.AddLog("Невозможно найти изображение");
                return new Find();
            }
        }

        public static Bitmap ResourceBitmap(string name)
        {
            System.IO.Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
            stream.Position = 0;
            Bitmap p = (Bitmap)Bitmap.FromStream(stream);
            stream.Close();
            return p;
        }

        public static Bitmap Crop(Bitmap b, Rectangle r)
        {
            Bitmap nb = new Bitmap(r.Width, r.Height);
            Graphics g = Graphics.FromImage(nb);
            g.DrawImage(b, -r.X, -r.Y);
            return nb;
        }

        public static Bitmap LoadPicture(string url)
        {
            CookieAwareWebClient client = new CookieAwareWebClient(BotBase.CcDictonaryNet);
            Stream s = client.OpenRead(url);
            Bitmap bmp = new Bitmap(s);
            return bmp;
        }

        public class CookieAwareWebClient : WebClient
        {
            public CookieContainer CookieContainer { get; set; }
            public Uri Uri { get; set; }

            public CookieAwareWebClient()
                : this(new CookieContainer())
            {
            }

            public CookieAwareWebClient(CookieContainer cookies)
            {
                this.CookieContainer = cookies;
            }

            protected override WebRequest GetWebRequest(Uri address)
            {
                WebRequest request = base.GetWebRequest(address);
                if (request is HttpWebRequest)
                {
                    (request as HttpWebRequest).CookieContainer = this.CookieContainer;
                }
                HttpWebRequest httpRequest = (HttpWebRequest)request;
                httpRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                return httpRequest;
            }

            protected override WebResponse GetWebResponse(WebRequest request)
            {
                WebResponse response = base.GetWebResponse(request);
                String setCookieHeader = response.Headers[HttpResponseHeader.SetCookie];

                if (setCookieHeader != null)
                {
                    //do something if needed to parse out the cookie.
                    if (setCookieHeader != null)
                    {
                        Cookie cookie = new Cookie(); //create cookie
                        this.CookieContainer.Add(cookie);
                    }
                }
                return response;
            }
        }

        public static string RemoveAllNewLines(string input)
        {
            char[] denied = new[] { ' ', '\n', '\t', '\r' };
            StringBuilder newString = new StringBuilder();
            foreach (var ch in input)
            {
                if (!denied.Contains(ch))
                    newString.Append(ch);
            }
            return newString.ToString();
        }
    }
}
