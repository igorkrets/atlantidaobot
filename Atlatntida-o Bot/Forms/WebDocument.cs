using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Atlatntida_o_Bot.Libs;
using Awesomium.Core;
using Awesomium.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Atlatntida_o_Bot.Forms
{
    public partial class WebDocument : DockContent
    {
        public bool Game;
        public WebDocument(bool game = false)
        {
            InitializeComponent();
            Initialize();
            Game = game;
        }

        public WebDocument(Uri url)
        {
            InitializeComponent();
            Initialize();
            webControl.Source = url;
        }

        public WebDocument(IntPtr nativeView)
        {
            InitializeComponent();
            Initialize();
            webControl.NativeView = nativeView;
        }

        public void Initialize()
        {
            webControl.TitleChanged += WebControlOnTitleChanged;
        }

        private void WebControlOnTitleChanged(object sender, TitleChangedEventArgs titleChangedEventArgs)
        {
            Text = titleChangedEventArgs.Title;
        }

        public override sealed string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        private void Awesomium_Windows_Forms_WebControl_DocumentReady(object sender, Awesomium.Core.UrlEventArgs urlEventArgs)
        {
            if (urlEventArgs.Url == new Uri(BotBase.GameUrl + "game.php"))
            {
                BotBase.InGame = true;
                BotBase.CcDictionary.Clear();
                string cookies = BotFunc.JsEx("document.cookie");
                string[] cooks = cookies.Split(";".ToCharArray());
                Regex r = new Regex("(.*?)=(.*)");
                foreach (string ms in cooks)
                {
                    Match n = r.Match(ms);
                    try
                    {
                        BotBase.CcDictonaryNet.Add(new Cookie(n.Groups[1].Value, n.Groups[2].Value, "/", BotBase.GameDomain));
                        BotBase.CcDictonaryCollection.Add(new Cookie(n.Groups[1].Value, n.Groups[2].Value));
                    }
                    catch{}
                    BotBase.CcDictionary.Add(n.Groups[1].Value, n.Groups[2].Value);
                }
            }
        }

        private void WebDocument_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Game)
            {
                e.Cancel = true;
            }
        }

        private void Awesomium_Windows_Forms_WebControl_ShowCreatedWebView(object sender, Awesomium.Core.ShowCreatedWebViewEventArgs e)
        {
            IWebView view = sender as IWebView;

            if (view == null)
                return;

            if (!view.IsLive)
                return;

            MainForm mainForm = Application.OpenForms.OfType<MainForm>().FirstOrDefault();

            if (mainForm == null)
                return;

            // Treat popups differently. If IsPopup is true, the event is always
            // the result of 'window.open' (IsWindowOpen is also true, so no need to check it).
            // Our application does not recognize user defined, non-standard specs. 
            // Therefore child views opened with non-standard specs, will not be presented as 
            // popups but as regular new windows (still wrapping the child view however -- se below).
            if (e.IsPopup && !e.IsUserSpecsOnly)
            {
                if (!BotBase.BlockPopUp)
                {
                    // JSWindowOpenSpecs.InitialPosition indicates screen coordinates.
                    Rectangle screenRect = e.Specs.InitialPosition.ToRectangle();

                    // Set the created native view as the underlying view of the
                    // WebControl. This will maintain the relationship between
                    // the parent view and the child, usually required when the new view
                    // is the result of 'window.open' (JS can access the parent window through
                    // 'window.opener'; the parent window can manipulate the child through the 'window'
                    // object returned from the 'window.open' call).
                    WebDocument newWindow = new WebDocument(e.NewViewInstance)
                    {
                        ShowInTaskbar = true,
                        ClientSize = screenRect.Size != Size.Empty ? screenRect.Size : new Size(640, 480)
                    };

                    // If the caller has not indicated a valid size for the new popup window,
                    // let it be opened with the default size specified at design time.
                    if ((screenRect.Width > 0) && (screenRect.Height > 0))
                    {
                        // Assign the indicated size.
                        newWindow.Width = screenRect.Width;
                        newWindow.Height = screenRect.Height;
                    }
                    newWindow.TopMost = true;
                    // Show the window.
                    newWindow.Show();

                    // If the caller has not indicated a valid position for the new popup window,
                    // let it be opened in the default position specified at design time.
                    if (screenRect.Location != Point.Empty)
                        // Move it to the specified coordinates.
                        newWindow.DesktopLocation = screenRect.Location;
                    newWindow.webControl.Source = e.TargetURL;
                }
            }
            else if (e.IsWindowOpen || e.IsPost)
            {
                // No specs or only non-standard specs were specified, but the event is still 
                // the result of 'window.open' or of an HTML form with tagret="_blank" and method="post".
                // We will open a normal window but we will still wrap the new native child view, 
                // maintaining its relationship with the parent window.
                WebDocument doc = new WebDocument(e.TargetURL);
                mainForm.OpenTab(doc);
            }
            else
            {
                // The event is not the result of 'window.open' or of an HTML form with tagret="_blank" 
                // and method="post"., therefore it's most probably the result of a link with target='_blank'. 
                // We will not be wrapping the created view; we let the WebControl hosted in ChildWindow 
                // create its own underlying view. Setting Cancel to true tells the core to destroy the 
                // created child view.
                //
                // Why don't we always wrap the native view passed to ShowCreatedWebView?
                //
                // - In order to maintain the relationship with their parent view,
                // child views execute and render under the same process (awesomium_process)
                // as their parent view. If for any reason this child process crashes,
                // all views related to it will be affected. When maintaining a parent-child 
                // relationship is not important, we prefer taking advantage of the isolated process 
                // architecture of Awesomium and let each view be rendered in a separate process.
                e.Cancel = true;
                // Note that we only explicitly navigate to the target URL, when a new view is 
                // about to be created, not when we wrap the created child view. This is because 
                // navigation to the target URL (if any), is already queued on created child views. 
                // We must not interrupt this navigation as we would still be breaking the parent-child
                // relationship.
                WebDocument doc = new WebDocument(e.TargetURL);
                mainForm.OpenTab(doc);
            }
        }

        private void Awesomium_Windows_Forms_WebControl_LoadingFrameComplete(object sender, FrameEventArgs e)
        {
            if (e.Url == new Uri(BotBase.GameUrl))
            {
                if (Game)
                {
                    string login = BotBase.DefaultSettings.String("login");
                    string password = BotBase.DefaultSettings.String("password");
                    BotFunc.JsEx("document.getElementsByName('login')[0].value = '" + login + "';");
                    BotFunc.JsEx("document.getElementsByName('pass')[0].value = '" + password + "';");
                }
            }
        }
    }
}
