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
            resources.ApplyResources(this.labelCacheId, "labelCacheId");
            this.labelCacheId.Name = "labelCacheId";
            // 
            // textBoxGeocacheId
            // 
            resources.ApplyResources(this.textBoxGeocacheId, "textBoxGeocacheId");
            this.textBoxGeocacheId.Name = "textBoxGeocacheId";
            // 
            // webBrowserPreview
            // 
            resources.ApplyResources(this.webBrowserPreview, "webBrowserPreview");
            this.webBrowserPreview.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowserPreview.Name = "webBrowserPreview";
            this.webBrowserPreview.ScriptErrorsSuppressed = true;
            this.webBrowserPreview.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowserPreview_DocumentCompleted);
            // 
            // labelWebSite
            // 
            resources.ApplyResources(this.labelWebSite, "labelWebSite");
            this.labelWebSite.Name = "labelWebSite";
            // 
            // comboBoxWebsite
            // 
            resources.ApplyResources(this.comboBoxWebsite, "comboBoxWebsite");
            this.comboBoxWebsite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxWebsite.FormattingEnabled = true;
            this.comboBoxWebsite.Items.AddRange(new object[] {
            resources.GetString("comboBoxWebsite.Items"),
            resources.GetString("comboBoxWebsite.Items1")});
            this.comboBoxWebsite.Name = "comboBoxWebsite";
            this.comboBoxWebsite.SelectedIndexChanged += new System.EventHandler(this.comboBoxWebsite_SelectedIndexChanged);
            // 
            // buttonBack
            // 
            resources.ApplyResources(this.buttonBack, "buttonBack");
            this.buttonBack.Image = global::GcDownload.Properties.Resources.NavBack;
            this.buttonBack.Name = "buttonBack";
            this.buttonBack.UseVisualStyleBackColor = true;
            this.buttonBack.Click += new System.EventHandler(this.buttonBack_Click);
            // 
            // buttonHome
            // 
            resources.ApplyResources(this.buttonHome, "buttonHome");
            this.buttonHome.Image = global::GcDownload.Properties.Resources.HomeHS;
            this.buttonHome.Name = "buttonHome";
            this.buttonHome.UseVisualStyleBackColor = true;
            this.buttonHome.Click += new System.EventHandler(this.buttonHome_Click);
            // 
            // buttonSearch
            // 
            resources.ApplyResources(this.buttonSearch, "buttonSearch");
            this.buttonSearch.Image = global::GcDownload.Properties.Resources.SearchWebHS;
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.UseVisualStyleBackColor = true;
            this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);
            // 
            // buttonDownload
            // 
            resources.ApplyResources(this.buttonDownload, "buttonDownload");
            this.buttonDownload.Image = global::GcDownload.Properties.Resources.saveHS;
            this.buttonDownload.Name = "buttonDownload";
            this.buttonDownload.UseVisualStyleBackColor = true;
            this.buttonDownload.Click += new System.EventHandler(this.buttonDownload_Click);
            // 
            // buttonFieldLog
            // 
            resources.ApplyResources(this.buttonFieldLog, "buttonFieldLog");
            this.buttonFieldLog.Image = global::GcDownload.Properties.Resources.book_reportHS;
            this.buttonFieldLog.Name = "buttonFieldLog";
            this.buttonFieldLog.UseVisualStyleBackColor = true;
            this.buttonFieldLog.Click += new System.EventHandler(this.buttonFieldLog_Click);
            // 
            // buttonBrowseCaches
            // 
            resources.ApplyResources(this.buttonBrowseCaches, "buttonBrowseCaches");
            this.buttonBrowseCaches.Image = global::GcDownload.Properties.Resources.openfolderHS;
            this.buttonBrowseCaches.Name = "buttonBrowseCaches";
            this.buttonBrowseCaches.UseVisualStyleBackColor = true;
            this.buttonBrowseCaches.Click += new System.EventHandler(this.buttonBrowseCaches_Click);
            // 
            // buttonSettings
            // 
            resources.ApplyResources(this.buttonSettings, "buttonSettings");
            this.buttonSettings.Name = "buttonSettings";
            this.buttonSettings.UseVisualStyleBackColor = true;
            this.buttonSettings.Click += new System.EventHandler(this.buttonSettings_Click);
            // 
            // MainForm
            // 
            this.AcceptButton = this.buttonSearch;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
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
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
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

