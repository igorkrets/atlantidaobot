namespace Atlatntida_o_Bot.Forms.Debug
{
    partial class ShowHtml
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
            this.urlBox = new System.Windows.Forms.TextBox();
            this.unescape = new System.Windows.Forms.RichTextBox();
            this.splitText = new System.Windows.Forms.CheckBox();
            this.unescapeCheck = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // urlBox
            // 
            this.urlBox.Location = new System.Drawing.Point(12, 12);
            this.urlBox.Name = "urlBox";
            this.urlBox.Size = new System.Drawing.Size(707, 20);
            this.urlBox.TabIndex = 0;
            this.urlBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.urlBox_KeyDown);
            // 
            // unescape
            // 
            this.unescape.Location = new System.Drawing.Point(12, 61);
            this.unescape.Name = "unescape";
            this.unescape.Size = new System.Drawing.Size(707, 347);
            this.unescape.TabIndex = 1;
            this.unescape.Text = "";
            // 
            // splitText
            // 
            this.splitText.AutoSize = true;
            this.splitText.Checked = true;
            this.splitText.CheckState = System.Windows.Forms.CheckState.Checked;
            this.splitText.Location = new System.Drawing.Point(12, 38);
            this.splitText.Name = "splitText";
            this.splitText.Size = new System.Drawing.Size(65, 17);
            this.splitText.TabIndex = 2;
            this.splitText.Text = "splitText";
            this.splitText.UseVisualStyleBackColor = true;
            // 
            // unescapeCheck
            // 
            this.unescapeCheck.AutoSize = true;
            this.unescapeCheck.Checked = true;
            this.unescapeCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.unescapeCheck.Location = new System.Drawing.Point(83, 38);
            this.unescapeCheck.Name = "unescapeCheck";
            this.unescapeCheck.Size = new System.Drawing.Size(73, 17);
            this.unescapeCheck.TabIndex = 3;
            this.unescapeCheck.Text = "unescape";
            this.unescapeCheck.UseVisualStyleBackColor = true;
            // 
            // ShowHtml
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(731, 420);
            this.Controls.Add(this.unescapeCheck);
            this.Controls.Add(this.splitText);
            this.Controls.Add(this.unescape);
            this.Controls.Add(this.urlBox);
            this.Name = "ShowHtml";
            this.Text = "ShowHtml";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox urlBox;
        private System.Windows.Forms.RichTextBox unescape;
        private System.Windows.Forms.CheckBox splitText;
        private System.Windows.Forms.CheckBox unescapeCheck;
    }
}