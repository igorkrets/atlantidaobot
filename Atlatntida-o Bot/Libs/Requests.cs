using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using xNet;

namespace Atlatntida_o_Bot.Libs
{
    public class Requests
    {
        public static int errors = 0;

        public static string GetResponseHtml(string url, bool splitText = false)
        {
            string response = "";
            try
            {
                var get = new Task(() =>
                {
                    try
                    {
                        var req = new HttpRequest
                        {
                            Cookies = BotBase.CcDictionary
                        };
                        //req.AllowAutoRedirect = false;
                        var resp = req.Get(url);
                        response = resp.ToString();
                        req.Dispose();
                    }
                    catch(Exception ex){}
                });
                get.Start();
                for (int i = 0; i < 10000; i += 250)
                {
                    get.Wait(250);
                    Application.DoEvents();
                }
                if (splitText)
                {
                    response = Regex.Replace(response, @"\r", "");
                    response = Regex.Replace(response, @"\n", "");
                    response = Regex.Replace(response, Environment.NewLine, "");
                }
                //MessageBox.Show(response);
                return response;
            }
            catch (Exception exception)
            {
                errors++;
                if (errors == 10)
                {
                    errors = 0;
                    return null;
                }
            }
            return null;
        }

        public static string PostResponseHtml(string url, bool splitText = false)
        {
            var response = "";
            try
            {
                var get = new Task(() =>
                {
                    var req = new HttpRequest
                    {
                        Cookies = BotBase.CcDictionary
                    };
                    var resp = req.Post(new Uri(url));
                    response = resp.ToString();
                    req.Dispose();
                });
                get.Start();
                for (int i = 0; i < 10000; i += 250)
                {
                    get.Wait(250);
                    Application.DoEvents();
                }
                if (splitText)
                {
                    response = Regex.Replace(response, @"\n", "");
                }
                return response;
            }
            catch (Exception exception)
            {
                errors++;
                if (errors == 10)
                {
                    errors = 0;
                    return null;
                }
            }
            return null;
        }
    }
}
