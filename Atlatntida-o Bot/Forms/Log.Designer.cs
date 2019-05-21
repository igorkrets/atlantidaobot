namespace Atlatntida_o_Bot.Forms
{
    partial class Log
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
            this.TextLog = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // TextLog
            // 
            this.TextLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextLog.Location = new System.Drawing.Point(0, 0);
            this.TextLog.Name = "TextLog";
            this.TextLog.Size = new System.Drawing.Size(293, 438);
            this.TextLog.TabIndex = 0;
            this.TextLog.Text = "";
            this.TextLog.TextChanged += new System.EventHandler(this.TextLog_TextChanged);
            // 
            // Log
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(293, 438);
            this.Controls.Add(this.TextLog);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Name = "Log";
            this.Text = "Log";
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.RichTextBox TextLog;

    }
}