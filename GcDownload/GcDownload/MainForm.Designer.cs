namespace GcDownload
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.labelCacheId = new System.Windows.Forms.Label();
            this.textBoxGeocacheId = new System.Windows.Forms.TextBox();
            this.webBrowserPreview = new System.Windows.Forms.WebBrowser();
            this.labelWebSite = new System.Windows.Forms.Label();
            this.comboBoxWebsite = new System.Windows.Forms.ComboBox();
            this.buttonBack = new System.Windows.Forms.Button();
            this.buttonHome = new System.Windows.Forms.Button();
            this.buttonSearch = new System.Windows.Forms.Button();
            this.buttonDownload = new System.Windows.Forms.Button();
            this.buttonFieldLog = new System.Windows.Forms.Button();
            this.buttonBrowseCaches = new System.Windows.Forms.Button();
            this.buttonSettings = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelCacheId
            // 
            this.labelCacheId.AutoSize = true;
            this.labelCacheId.Location = new System.Drawing.Point(337, 18);
            this.labelCacheId.Name = "labelCacheId";
            this.labelCacheId.Size = new System.Drawing.Size(55, 13);
            this.labelCacheId.TabIndex = 4;
            this.labelCacheId.Text = "Cache &ID:";
            // 
            // textBoxGeocacheId
            // 
            this.textBoxGeocacheId.Location = new System.Drawing.Point(398, 15);
            this.textBoxGeocacheId.Name = "textBoxGeocacheId";
            this.textBoxGeocacheId.Size = new System.Drawing.Size(118, 20);
            this.textBoxGeocacheId.TabIndex = 5;
            // 
            // webBrowserPreview
            // 
            this.webBrowserPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.webBrowserPreview.Location = new System.Drawing.Point(16, 74);
            this.webBrowserPreview.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowserPreview.Name = "webBrowserPreview";
            this.webBrowserPreview.ScriptErrorsSuppressed = true;
            this.webBrowserPreview.Size = new System.Drawing.Size(1171, 688);
            this.webBrowserPreview.TabIndex = 8;
            this.webBrowserPreview.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowserPreview_DocumentCompleted);
            // 
            // labelWebSite
            // 
            this.labelWebSite.AutoSize = true;
            this.labelWebSite.Location = new System.Drawing.Point(560, 18);
            this.labelWebSite.Name = "labelWebSite";
            this.labelWebSite.Size = new System.Drawing.Size(49, 13);
            this.labelWebSite.TabIndex = 0;
            this.labelWebSite.Text = "&Website:";
            // 
            // comboBoxWebsite
            // 
            this.comboBoxWebsite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxWebsite.FormattingEnabled = true;
            this.comboBoxWebsite.Items.AddRange(new object[] {
            "www.geocaching.com",
            "www.opencaching.de"});
            this.comboBoxWebsite.Location = new System.Drawing.Point(615, 15);
            this.comboBoxWebsite.Name = "comboBoxWebsite";
            this.comboBoxWebsite.Size = new System.Drawing.Size(158, 21);
            this.comboBoxWebsite.TabIndex = 1;
            this.comboBoxWebsite.SelectedIndexChanged += new System.EventHandler(this.comboBoxWebsite_SelectedIndexChanged);
            // 
            // buttonBack
            // 
            this.buttonBack.Image = global::GcDownload.Properties.Resources.NavBack;
            this.buttonBack.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonBack.Location = new System.Drawing.Point(16, 43);
            this.buttonBack.Name = "buttonBack";
            this.buttonBack.Size = new System.Drawing.Size(100, 25);
            this.buttonBack.TabIndex = 3;
            this.buttonBack.Text = "&Back";
            this.buttonBack.UseVisualStyleBackColor = true;
            this.buttonBack.Click += new System.EventHandler(this.buttonBack_Click);
            // 
            // buttonHome
            // 
            this.buttonHome.Image = global::GcDownload.Properties.Resources.HomeHS;
            this.buttonHome.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonHome.Location = new System.Drawing.Point(16, 12);
            this.buttonHome.Name = "buttonHome";
            this.buttonHome.Size = new System.Drawing.Size(100, 25);
            this.buttonHome.TabIndex = 2;
            this.buttonHome.Text = "&Home";
            this.buttonHome.UseVisualStyleBackColor = true;
            this.buttonHome.Click += new System.EventHandler(this.buttonHome_Click);
            // 
            // buttonSearch
            // 
            this.buttonSearch.Image = global::GcDownload.Properties.Resources.SearchWebHS;
            this.buttonSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonSearch.Location = new System.Drawing.Point(228, 12);
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.Size = new System.Drawing.Size(100, 25);
            this.buttonSearch.TabIndex = 6;
            this.buttonSearch.Text = "&Search";
            this.buttonSearch.UseVisualStyleBackColor = true;
            this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);
            // 
            // buttonDownload
            // 
            this.buttonDownload.Image = global::GcDownload.Properties.Resources.saveHS;
            this.buttonDownload.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonDownload.Location = new System.Drawing.Point(122, 12);
            this.buttonDownload.Name = "buttonDownload";
            this.buttonDownload.Size = new System.Drawing.Size(100, 25);
            this.buttonDownload.TabIndex = 7;
            this.buttonDownload.Text = "&Download";
            this.buttonDownload.UseVisualStyleBackColor = true;
            this.buttonDownload.Click += new System.EventHandler(this.buttonDownload_Click);
            // 
            // buttonFieldLog
            // 
            this.buttonFieldLog.Image = global::GcDownload.Properties.Resources.book_reportHS;
            this.buttonFieldLog.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonFieldLog.Location = new System.Drawing.Point(228, 43);
            this.buttonFieldLog.Name = "buttonFieldLog";
            this.buttonFieldLog.Size = new System.Drawing.Size(100, 25);
            this.buttonFieldLog.TabIndex = 9;
            this.buttonFieldLog.Text = "&Field Log";
            this.buttonFieldLog.UseVisualStyleBackColor = true;
            this.buttonFieldLog.Click += new System.EventHandler(this.buttonFieldLog_Click);
            // 
            // buttonBrowseCaches
            // 
            this.buttonBrowseCaches.Image = global::GcDownload.Properties.Resources.openfolderHS;
            this.buttonBrowseCaches.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonBrowseCaches.Location = new System.Drawing.Point(122, 43);
            this.buttonBrowseCaches.Name = "buttonBrowseCaches";
            this.buttonBrowseCaches.Size = new System.Drawing.Size(100, 25);
            this.buttonBrowseCaches.TabIndex = 10;
            this.buttonBrowseCaches.Text = "&List Caches";
            this.buttonBrowseCaches.UseVisualStyleBackColor = true;
            this.buttonBrowseCaches.Click += new System.EventHandler(this.buttonBrowseCaches_Click);
            // 
            // buttonSettings
            // 
            this.buttonSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSettings.Location = new System.Drawing.Point(1082, 12);
            this.buttonSettings.Name = "buttonSettings";
            this.buttonSettings.Size = new System.Drawing.Size(105, 25);
            this.buttonSettings.TabIndex = 11;
            this.buttonSettings.Text = "S&ettings";
            this.buttonSettings.UseVisualStyleBackColor = true;
            this.buttonSettings.Click += new System.EventHandler(this.buttonSettings_Click);
            // 
            // MainForm
            // 
            this.AcceptButton = this.buttonSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1199, 774);
            this.Controls.Add(this.buttonSettings);
            this.Controls.Add(this.buttonBrowseCaches);
            this.Controls.Add(this.buttonFieldLog);
            this.Controls.Add(this.comboBoxWebsite);
            this.Controls.Add(this.labelWebSite);
            this.Controls.Add(this.buttonBack);
            this.Controls.Add(this.buttonHome);
            this.Controls.Add(this.buttonSearch);
            this.Controls.Add(this.webBrowserPreview);
            this.Controls.Add(this.buttonDownload);
            this.Controls.Add(this.textBoxGeocacheId);
            this.Controls.Add(this.labelCacheId);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Geocache Download for Garmin Dakota";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelCacheId;
        private System.Windows.Forms.TextBox textBoxGeocacheId;
        private System.Windows.Forms.Button buttonDownload;
        private System.Windows.Forms.WebBrowser webBrowserPreview;
        private System.Windows.Forms.Button buttonSearch;
        private System.Windows.Forms.Button buttonHome;
        private System.Windows.Forms.Button buttonBack;
        private System.Windows.Forms.Label labelWebSite;
        private System.Windows.Forms.ComboBox comboBoxWebsite;
        private System.Windows.Forms.Button buttonFieldLog;
        private System.Windows.Forms.Button buttonBrowseCaches;
        private System.Windows.Forms.Button buttonSettings;
    }
}

