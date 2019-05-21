namespace Atlatntida_o_Bot.Forms
{
    partial class WebDocument
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Awesomium.Core.WebPreferences webPreferences1 = new Awesomium.Core.WebPreferences(true);
            this.webControl = new Awesomium.Windows.Forms.WebControl(this.components);
            this.webSession = new Awesomium.Windows.Forms.WebSessionProvider(this.components);
            this.SuspendLayout();
            // 
            // webControl
            // 
            this.webControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webControl.Location = new System.Drawing.Point(0, 0);
            this.webControl.Size = new System.Drawing.Size(657, 431);
            this.webControl.TabIndex = 0;
            this.webControl.ViewType = Awesomium.Core.WebViewType.Offscreen;
            this.webControl.ShowCreatedWebView += new Awesomium.Core.ShowCreatedWebViewEventHandler(this.Awesomium_Windows_Forms_WebControl_ShowCreatedWebView);
            this.webControl.DocumentReady += new Awesomium.Core.UrlEventHandler(this.Awesomium_Windows_Forms_WebControl_DocumentReady);
            this.webControl.LoadingFrameComplete += new Awesomium.Core.FrameEventHandler(this.Awesomium_Windows_Forms_WebControl_LoadingFrameComplete);
            // 
            // webSession
            // 
            webPreferences1.AcceptLanguage = "ru-ru,ru";
            webPreferences1.DefaultEncoding = "utf-8";
            webPreferences1.EnableGPUAcceleration = true;
            webPreferences1.SmoothScrolling = true;
            webPreferences1.WebGL = true;
            this.webSession.Preferences = webPreferences1;
            this.webSession.Views.Add(this.webControl);
            // 
            // WebDocument
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(657, 431);
            this.Controls.Add(this.webControl);
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Name = "WebDocument";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WebDocument";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.WebDocument_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private Awesomium.Windows.Forms.WebSessionProvider webSession;
        public Awesomium.Windows.Forms.WebControl webControl;
    }
}